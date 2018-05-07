// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Koopman.CheckPoint.Common;
using Koopman.CheckPoint.Exceptions;
using Koopman.CheckPoint.Internal;
using Koopman.CheckPoint.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// Static ObjectSummary fields
    /// </summary>
    public static class ObjectSummary
    {
        #region Static Fields

        /// <summary>
        /// The Any object.
        /// </summary>
        public static readonly IObjectSummary Any = GenericObjectSummary.Any;

        #endregion Static Fields
    }

    /// <summary>
    /// The basic summary information of a Check Point object
    /// </summary>
    /// <typeparam name="T">Type from derived classes</typeparam>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public abstract class ObjectSummary<T> : ChangeTracking, IObjectSummary where T : IObjectSummary
    {
        #region Fields

        internal Domain _domain;
        internal string _name;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSummary" /> class where the Type is
        /// defined by the class name in camel case.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level.</param>
        protected internal ObjectSummary(Session session, DetailLevels detailLevel)
        {
            Session = session;
            DetailLevel = detailLevel;

            // Set Type for New Objects.
            Type = GetType().Name.CamelCaseToRegular("-").ToLower();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSummary" /> class.
        /// </summary>
        /// <param name="session">The current session.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <param name="type">
        /// The Check Point type. Must match type property returned in JSON data.
        /// </param>
        protected internal ObjectSummary(Session session, DetailLevels detailLevel, string type)
        {
            Session = session;
            DetailLevel = detailLevel;
            Type = type;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the current detail level of retrieved objects. You will not be able to get the
        /// values of some properties if the detail level is too low. You can still set property
        /// values however to override existing values.
        /// </summary>
        /// <value>The current detail level.</value>
        public DetailLevels DetailLevel { get; protected internal set; }

        /// <summary>
        /// Information about the domain the object belongs to.
        /// </summary>
        /// <value>The domain.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "domain", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Domain Domain
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _domain : null;
            private set => _domain = value;
        }

        /// <summary>
        /// Object name. Should be unique in the domain.
        /// </summary>
        /// <value>The object's name.</value>
        /// <remarks>Requires <see cref="IObjectSummary.DetailLevel" /> of at least <see cref="DetailLevels.Standard" /></remarks>
        [JsonProperty(PropertyName = "name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Name
        {
            get => (_name != null || TestDetailLevel(DetailLevels.Standard)) ? _name : null;

            set
            {
                _name = value;
                if (IsDeserializing)
                {
                    OldName = value;
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Type of the object.
        /// </summary>
        /// <value>The type.</value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }

        /// <summary>
        /// Object unique identifier.
        /// </summary>
        /// <value>The uid.</value>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; internal set; }

        internal bool HasUpdatedGenericMembers { get; private set; } = false;

        /// <summary>
        /// Gets the name of the object before any changes. Used when sending updates that may
        /// include a name change.
        /// </summary>
        /// <value>The old name of the object.</value>
        [JsonProperty(PropertyName = "OldName")]
        protected internal string OldName { get; private set; }

        /// <summary>
        /// Gets the current session that will process all requests to the Check Point management web API.
        /// </summary>
        /// <value>The session.</value>
        protected internal Session Session { get; private set; }

        /// <summary>
        /// Gets the contract resolver used when converting request into JSON data for adding new objects.
        /// </summary>
        /// <value>The add contract resolver.</value>
        protected virtual IContractResolver AddContractResolver => ChangeTrackingContractResolver.AddInstance;

        /// <summary>
        /// Gets the contract resolver used when converting request into JSON data for updating
        /// existing objects.
        /// </summary>
        /// <value>The set contract resolver.</value>
        protected virtual IContractResolver SetContractResolver => ChangeTrackingContractResolver.SetInstance;

        private bool IsReadOnly => GetType().GetTypeInfo().IsGenericType;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Same as calling <see cref="ObjectSummary{T}.AcceptChanges(Ignore, CancellationToken)" />
        /// with a value of <see cref="Ignore.No" />;
        /// </summary>
        public override Task AcceptChanges(CancellationToken cancellationToken = default) => AcceptChanges(Ignore.No, cancellationToken);

        /// <summary>
        /// Posts all changes to Check Point server. If successful all object properties will be
        /// updated with results.
        /// </summary>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        public async virtual Task AcceptChanges(Ignore ignore, CancellationToken cancellationToken = default)
        {
            if (IsChanged)
            {
                var jsonSerializerSettings = new JsonSerializerSettings() { Converters = { new MembershipChangeTrackingConverter() } };
                string command;

                if (IsNew)
                {
                    command = $"add-{Type}";
                    jsonSerializerSettings.ContractResolver = AddContractResolver;
                }
                else
                {
                    command = $"set-{Type}";
                    jsonSerializerSettings.ContractResolver = SetContractResolver;
                }

                var jo = JObject.FromObject(this, JsonSerializer.Create(jsonSerializerSettings));

                jo.AddIgnore(ignore);

                string jsonData = JsonConvert.SerializeObject(jo, Session.JsonFormatting);

                string result = await Session.PostAsync(command, jsonData, cancellationToken);

                DetailLevel = DetailLevels.Full;

                JsonConvert.PopulateObject(result, this, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevels.Full, DetailLevels.Standard) } });
            }
        }

        /// <summary>
        /// Deletes the current object.
        /// </summary>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        /// <exception cref="Exception">Cannot delete a new object.</exception>
        public virtual Task Delete(Ignore ignore = Internal.Delete.Defaults.ignore, CancellationToken cancellationToken = default)
        {
            if (IsNew) { throw new Exception("Cannot delete a new object."); }

            return Internal.Delete.Invoke(
                Session: Session,
                Command: $"delete-{Type}",
                Value: UID,
                Ignore: ignore,
                cancellationToken: cancellationToken
                );
        }

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        /// <exception cref="InvalidOperationException">Cannot add unsaved object.</exception>
        public string GetIdentifier() => (IsPropertyChanged(nameof(Name)) || string.IsNullOrWhiteSpace(_name)) ? UID : _name;

        /// <summary>
        /// Reloads the current object. Used to either reset changes made without saving, or to
        /// increased the <paramref name="detailLevel" /> to <see cref="DetailLevels.Full" />
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>this</c></returns>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        /// <exception cref="Exception">Cannot reload a new object.</exception>
        public async virtual Task<T> Reload(bool OnlyIfPartial = false, DetailLevels detailLevel = DetailLevels.Standard, CancellationToken cancellationToken = default)
        {
            if (IsNew) { throw new Exception("Cannot reload a new object."); }
            if (OnlyIfPartial && DetailLevel == DetailLevels.Full) { return (T)(IObjectSummary)this; }

            var data = new Dictionary<string, dynamic>
            {
                { UID != null ? "uid" : "name", UID ?? _name },
                { "details-level", detailLevel.ToString() }
            };

            string jsonData = JsonConvert.SerializeObject(data, Session.JsonFormatting);

            string result = await Session.PostAsync($"show-{Type}", jsonData, cancellationToken);

            DetailLevel = DetailLevels.Full;

            JsonConvert.PopulateObject(result, this, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevels.Full, detailLevel) } });

            return (T)(IObjectSummary)this;
        }

        /// <summary>
        /// Reloads the current object. Used to either reset changes made without saving, or to
        /// increased the <paramref name="detailLevel" /> to <see cref="DetailLevels.Full" />
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level of child objects to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>IObjectSummary of reloaded object</returns>
        async Task<IObjectSummary> IObjectSummary.Reload(bool OnlyIfPartial, DetailLevels detailLevel, CancellationToken cancellationToken) => await Reload(OnlyIfPartial, detailLevel, cancellationToken);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this object.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this object.</returns>
        public override string ToString() => (string.IsNullOrEmpty(_name)) ? UID : _name;

        /// <summary>
        /// Update any GenericMembers with ObjectConverter cache if exists.
        /// </summary>
        internal virtual void UpdateGenericMembers(ObjectConverter objectConverter) => HasUpdatedGenericMembers = true;

        /// <summary>
        /// Tests the current detail level and takes action if too low.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="detailLevelAction">The action to take if too low.</param>
        /// <returns>
        /// <c>true</c> if detail level matches required value or if auto reload was successful;
        /// otherwise <c>false</c>
        /// </returns>
        /// <exception cref="DetailLevelException">
        /// Thrown if detail level too low and action <see cref="DetailLevelActions.ThrowException" />
        /// </exception>
        protected bool TestDetailLevel(DetailLevels minValue, DetailLevelActions detailLevelAction = DetailLevelActions.SessionDefault)
        {
            if (DetailLevel < minValue)
            {
                if (IsDeserializing || IsSerializing)
                    return false;

                var action =
                    (detailLevelAction == DetailLevelActions.SessionDefault) ?
                        Session.DetailLevelAction :
                        detailLevelAction;

                switch (action)
                {
                    case DetailLevelActions.ReturnNull:
                        return false;

                    case DetailLevelActions.AutoReload:
                        Reload().GetAwaiter().GetResult();
                        return (DetailLevel == DetailLevels.Full);

                    default:
                        throw new DetailLevelException(DetailLevel, minValue);
                }
            }
            else
            {
                return true;
            }
        }

        #endregion Methods
    }
}
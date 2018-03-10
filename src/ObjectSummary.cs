﻿// MIT License
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
using System.Diagnostics;

namespace Koopman.CheckPoint
{
    /// <summary>
    /// The basic summary information of a Check Point object
    /// </summary>
    /// <seealso cref="Koopman.CheckPoint.Common.ChangeTracking" />
    public class ObjectSummary : ChangeTracking
    {
        #region Static Fields

        /// <summary>
        /// The Any object.
        /// </summary>
        public static readonly ObjectSummary Any = new ObjectSummary(null, DetailLevels.Full, "CpmiAnyObject")
        {
            UID = "97aeb369-9aea-11d5-bd16-0090272ccb30",
            Name = "Any",
            Domain = Domain.DataDomain
        };

        #endregion Static Fields

        #region Fields

        private Domain _domain;
        private string _name;

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

            // Set Type for New Objects.
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
        /// <para type="description">Information about the domain the object belongs to..</para>
        /// </summary>
        [JsonProperty(PropertyName = "domain", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Domain Domain
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _domain : null;
            private set => _domain = value;
        }

        /// <summary>
        /// <para type="description">Object name. Should be unique in the domain.</para>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Name
        {
            get => (TestDetailLevel(DetailLevels.Standard)) ? _name : null;

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
        /// <para type="description">Type of the object.</para>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; private set; }

        /// <summary>
        /// <para type="description">Object unique identifier.</para>
        /// </summary>
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; private set; }

        /// <summary>
        /// Gets the name of the object before any changes. Used when sending updates that may
        /// include a name change.
        /// </summary>
        /// <value>The old name of the object.</value>
        [JsonProperty(PropertyName = "OldName", NullValueHandling = NullValueHandling.Ignore)]
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

        /// <summary>
        /// Gets the identifier that is used when adding this object to a group.
        /// </summary>
        /// <returns>Name if not null else the UID</returns>
        /// <exception cref="InvalidOperationException">Cannot add unsaved object.</exception>
        protected internal string GetMembershipID()
        {
            if (IsNew) throw new InvalidOperationException("Cannot add unsaved object.");

            return (IsPropertyChanged(nameof(Name)) || String.IsNullOrWhiteSpace(Name)) ? UID : Name;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Same as calling <see cref="ObjectSummary.AcceptChanges(Ignore)" /> with a value of <see cref="Ignore.No" />;
        /// </summary>
        public override void AcceptChanges()
        {
            AcceptChanges(Ignore.No);
        }

        /// <summary>
        /// Posts all changes to Check Point server. If successful all object properties will be
        /// updated with results.
        /// </summary>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        public virtual void AcceptChanges(Ignore ignore)
        {
            if (this.GetType() == typeof(ObjectSummary)) { throw new System.NotImplementedException($"Check Point type of {Type} is not fully implemented yet."); }

            if (IsChanged)
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { Converters = { new MembershipChangeTrackingConverter() } };
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

                JObject jo = JObject.FromObject(this, JsonSerializer.Create(jsonSerializerSettings));

                jo.AddIgnore(ignore);

                string jsonData = JsonConvert.SerializeObject(jo, Session.JsonFormatting);

                string result = Session.Post(command, jsonData);

                DetailLevel = DetailLevels.Full;

                JsonConvert.PopulateObject(result, this, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevels.Full, DetailLevels.Standard) } });
            }
        }

        /// <summary>
        /// Deletes the current object.
        /// </summary>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        /// <exception cref="Exception">Cannot delete a new object.</exception>
        public virtual void Delete(Ignore ignore = Internal.Delete.Defaults.ignore)
        {
            if (this.GetType() == typeof(ObjectSummary)) { throw new System.NotImplementedException(); }
            if (IsNew) { throw new Exception("Cannot delete a new object."); }

            Internal.Delete.Invoke(
                Session: Session,
                Command: $"delete-{Type}",
                Value: UID,
                Ignore: ignore);
        }

        /// <summary>
        /// Reloads the current object. Used to either reset changes made without saving, or to
        /// increased the <paramref name="detailLevel" /> to <see cref="DetailLevels.Full" />
        /// </summary>
        /// <param name="OnlyIfPartial">
        /// Only perform reload if <paramref name="detailLevel" /> is not already <see cref="DetailLevels.Full" />
        /// </param>
        /// <param name="detailLevel">The detail level to retrieve.</param>
        /// <exception cref="System.NotImplementedException">
        /// Thrown when the objects of this Type have not been fully implemented yet.
        /// </exception>
        public virtual void Reload(bool OnlyIfPartial = Internal.Reload.Defaults.OnlyIfPartial, DetailLevels detailLevel = Internal.Reload.Defaults.DetailLevel)
        {
            if (this.GetType() == typeof(ObjectSummary)) { throw new System.NotImplementedException(); }

            Internal.Reload.Invoke($"show-{Type}", this, OnlyIfPartial, detailLevel);
        }

        /// <summary>
        /// Conditional Property Serialization for Domain
        /// </summary>
        /// <returns>true if Domain should be serialised.</returns>
        public bool ShouldSerializeDomain()
        {
            return Domain != null && !Domain.Equals(Domain.Default);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this object.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this object.</returns>
        public override string ToString()
        {
            return (string.IsNullOrEmpty(Name)) ? UID : Name;
        }

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
                DetailLevelActions action =
                    (detailLevelAction == DetailLevelActions.SessionDefault) ?
                        Session.Options.DetailLevelAction :
                        detailLevelAction;

                switch (action)
                {
                    case DetailLevelActions.ReturnNull:
                        return false;

                    case DetailLevelActions.AutoReload:
                        Reload();
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
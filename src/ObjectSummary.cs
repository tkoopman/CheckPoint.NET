// MIT License
//
// Copyright (c) 2018 Tim Koopman
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
    public class ObjectSummary : ChangeTracking
    {
        #region Static Fields

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

        protected internal ObjectSummary(Session session, DetailLevels detailLevel)
        {
            Session = session;
            DetailLevel = detailLevel;

            // Set Type for New Objects.
            Type = GetType().Name.CamelCaseToRegular("-").ToLower();
        }

        protected internal ObjectSummary(Session session, DetailLevels detailLevel, string type)
        {
            Session = session;
            DetailLevel = detailLevel;

            // Set Type for New Objects.
            Type = type;
        }

        #endregion Constructors

        #region Properties

        public DetailLevels DetailLevel { get; protected internal set; }

        /// <summary>
        /// <para type="description">Information about the domain the object belongs to..</para>
        /// </summary>
        [JsonProperty(PropertyName = "domain")]
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

        [JsonProperty(PropertyName = "OldName", NullValueHandling = NullValueHandling.Ignore)]
        protected internal string OldName { get; private set; }

        protected internal Session Session { get; private set; }

        protected virtual IContractResolver AddContractResolver => ChangeTrackingContractResolver.AddInstance;

        protected virtual IContractResolver SetContractResolver => ChangeTrackingContractResolver.SetInstance;

        protected internal string GetMembershipID()
        {
            if (IsNew) throw new InvalidOperationException("Cannot add unsaved object.");

            return (IsPropertyChanged(nameof(Name)) || String.IsNullOrWhiteSpace(Name)) ? UID : Name;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Same as calling AcceptChanges(Ignore.No);
        /// </summary>
        public override void AcceptChanges()
        {
            AcceptChanges(Ignore.No);
        }

        /// <summary>
        /// Posts all changes to Check Point server. If successful all object properties will be updated with results.
        /// </summary>
        /// <param name="ignore">Weather warnings or errors should be ignored</param>
        /// <exception cref="System.NotImplementedException"></exception>
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

                string jsonData = JsonConvert.SerializeObject(jo);

                string result = Session.Post(command, jsonData);

                DetailLevel = DetailLevels.Full;

                JsonConvert.PopulateObject(result, this, new JsonSerializerSettings() { Converters = { new ObjectConverter(Session, DetailLevels.Full, DetailLevels.Standard) } });
            }
        }

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

        public override string ToString()
        {
            return (string.IsNullOrEmpty(Name)) ? UID : Name;
        }

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
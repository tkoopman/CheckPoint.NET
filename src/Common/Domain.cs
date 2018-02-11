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

using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Koopman.CheckPoint.Common
{
    public class Domain
    {
        #region Fields

        public readonly static Domain DataDomain = new Domain("Check Point Data", "a0bbbc99-adef-4ef8-bb6d-defdefdefdef", "data domain");
        public readonly static Domain Default = new Domain("SMC User", "41e821a0-3720-11e3-aa6e-0800200c9fde", "domain");

        #endregion Fields

        #region Constructors

        /// <summary>
        /// JSON Constructor for Domain information
        /// </summary>
        [JsonConstructor]
        private Domain(string name, string uid, string domainType)
        {
            Name = name;
            UID = uid;
            DomainType = domainType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <para type="description">Domain type.</para>
        /// </summary>
        [JsonProperty(PropertyName = "domain-type", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        [DefaultValue("domain")]
        public string DomainType { get; private set; }

        /// <summary>
        /// <para type="description">Object name. Should be unique in the domain.</para>
        /// </summary>
        [JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        [DefaultValue("SMC User")]
        public string Name { get; private set; }

        /// <summary>
        /// <para type="description">Object unique identifier.</para>
        /// </summary>
        [JsonProperty(PropertyName = "uid", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        [DefaultValue("41e821a0-3720-11e3-aa6e-0800200c9fde")]
        public string UID { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// <para type="description">Returns true if object UIDs match</para>
        /// </summary>
        public override bool Equals(object obj)
        {
            try
            {
                Domain OBJ = (Domain)obj;
                return this.UID.Equals(OBJ.UID);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        /// <summary>
        /// <para type="description">Returns Hashcode of object UID</para>
        /// </summary>
        public override int GetHashCode()
        {
            return this.UID.GetHashCode();
        }

        /// <summary>
        /// <para type="description">Convert domain object to string. (Domain name)</para>
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}
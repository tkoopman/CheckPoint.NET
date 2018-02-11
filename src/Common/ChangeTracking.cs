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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Allows for tracking of property changes, so only changed properties are posted.
    /// </summary>
    /// <seealso cref="System.ComponentModel.IChangeTracking" />
    /// <seealso cref="Koopman.CheckPoint.Json.ChangeTrackingContractResolver" />
    public abstract class ChangeTracking : SimpleChangeTracking
    {
        #region Fields

        private List<string> ChangedProperties = new List<string>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// <c>true</c> if any properties have been changed or if any contained
        /// MembershipChangeTracking properties have had membership modifications.
        /// </value>
        public override bool IsChanged
        {
            get
            {
                if (ChangedProperties.Count > 0)
                {
                    return true;
                }
                else
                {
                    foreach (var p in GetType().GetProperties())
                    {
                        if (typeof(IChangeTracking).IsAssignableFrom(p.PropertyType))
                        {
                            IChangeTracking v = (IChangeTracking)p.GetValue(this);
                            if (v != null && v.IsChanged)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        #endregion Properties

        #region Methods

        protected internal string[] getChangedProperties()
        {
            return ChangedProperties.ToArray();
        }

        protected internal bool IsPropertyChanged(string propertyName)
        {
            return ChangedProperties.Contains(propertyName);
        }

        protected override void OnDeserialized()
        {
            ChangedProperties.Clear();
        }

        /// <summary>
        /// Should be called, by the property setter, when any property value is updated. Does not
        /// need to be called for properties that implement IChangeTracking.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated.</param>
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (!IsDeserializing && !IsPropertyChanged(propertyName))
            {
                ChangedProperties.Add(propertyName);
            }
        }

        #endregion Methods
    }
}
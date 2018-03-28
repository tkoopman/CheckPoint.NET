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

using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Koopman.CheckPoint.Common
{
    /// <summary>
    /// Allows for tracking of property changes, so only changed properties are posted.
    /// </summary>
    /// <seealso cref="System.ComponentModel.IChangeTracking" />
    public abstract class ChangeTracking : SimpleChangeTracking
    {
        #region Fields

        private List<string> ChangedProperties = new List<string>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the object's changed status. Includes checking status of all IChangeTracking properties.
        /// </summary>
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
                    foreach (var p in GetType().GetTypeInfo().GetProperties())
                    {
                        if (typeof(IChangeTracking).GetTypeInfo().IsAssignableFrom(p.PropertyType))
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

        /// <summary>
        /// Gets array of properties that have been changed.
        /// </summary>
        /// <returns>Array of property names</returns>
        protected internal string[] getChangedProperties()
        {
            return ChangedProperties.ToArray();
        }

        /// <summary>
        /// Determines whether a specific property has been changed.
        /// </summary>
        /// <param name="propertyName">Name of the property to check.</param>
        /// <returns><c>true</c> if the property has been changed; otherwise, <c>false</c>.</returns>
        protected internal bool IsPropertyChanged(string propertyName)
        {
            return ChangedProperties.Contains(propertyName);
        }

        /// <summary>
        /// Called when deserialized.
        /// </summary>
        protected override void OnDeserialized()
        {
            ChangedProperties.Clear();
        }

        /// <summary>
        /// Should be called, by the property setter, when any property value is updated.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated.</param>
        protected override void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (!IsDeserializing && !IsPropertyChanged(propertyName))
            {
                ChangedProperties.Add(propertyName);
            }
        }

        #endregion Methods
    }
}
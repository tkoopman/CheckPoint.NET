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
using System.Linq;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    /// <summary>
    /// Used to deserialize objects that require the <see cref="Session" /> object to be passed to
    /// the constructor.
    /// </summary>
    internal class SessionConstructorConverter : JsonConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionConstructorConverter" /> class.
        /// </summary>
        /// <param name="session">The current active session to management server.</param>
        internal SessionConstructorConverter(Session session)
        {
            Session = session;
        }

        #endregion Constructors

        #region Properties

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanWrite => false;

        protected Session Session { get; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            //ConstructorInfo ci = objectType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Session) }, null);
            var ci = objectType.GetTypeInfo().DeclaredConstructors.SingleOrDefault(c => (c.GetParameters().Length == 1 && c.GetParameters().First().ParameterType == typeof(Session)));
            return ci != null;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = existingValue;

            if (result == null)
            {
                //ConstructorInfo ci = objectType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Session) }, null);
                var ci = objectType.GetTypeInfo().DeclaredConstructors.Single(c => c.GetParameters().Length == 1 && c.GetParameters().First().ParameterType == typeof(Session));
                if (ci == null) { throw new Exception("Unable to find constructor that accepts Session parameter"); }
                result = ci.Invoke(new object[] { Session });
            }

            serializer.Populate(reader, result);
            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

        #endregion Methods
    }
}
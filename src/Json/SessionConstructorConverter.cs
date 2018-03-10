using Newtonsoft.Json;
using System;
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
            ConstructorInfo ci = objectType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Session) }, null);
            return ci != null;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Object result = existingValue;

            if (result == null)
            {
                ConstructorInfo ci = objectType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Session) }, null);
                if (ci == null) { throw new Exception("Unable to find constructor that accepts Session parameter"); }
                result = ci.Invoke(new object[] { Session });
            }

            serializer.Populate(reader, result);
            return result;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Koopman.CheckPoint.Json
{
    internal class SessionConstructorConverter : JsonConverter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConverter" /> class.
        /// </summary>
        /// <param name="Session">The current active session to management server.</param>
        /// <param name="parentDetailLevel">Detail level returned for top level objects.</param>
        /// <param name="childDetailLevel">Detail level returned for all child objects.</param>
        public SessionConstructorConverter(Session session)
        {
            Session = session;
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead => true;

        public override bool CanWrite => false;

        protected Session Session { get; }

        #endregion Properties

        #region Methods

        public override bool CanConvert(Type objectType)
        {
            ConstructorInfo ci = objectType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Session) }, null);
            return ci != null;
        }

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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
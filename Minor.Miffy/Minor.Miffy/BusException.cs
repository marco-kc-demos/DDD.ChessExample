using System;
using System.Runtime.Serialization;

namespace Minor.Miffy
{
    [Serializable]
    public class BusException : Exception
    {
        public BusException()
        {
        }

        public BusException(string message) : base(message)
        {
        }

        public BusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
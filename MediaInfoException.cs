using System;
using System.Runtime.Serialization;

namespace MediaInfoSharp
{
    public class MediaInfoException : Exception
    {
        public MediaInfoException() { }
        public MediaInfoException(string message) : base(message) { }
        public MediaInfoException(string message, Exception innerException) : base(message, innerException) { }
        protected MediaInfoException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

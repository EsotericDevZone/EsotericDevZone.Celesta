using System;
using System.Runtime.Serialization;

namespace EsotericDevZone.Celesta.AST
{
    [Serializable]
    public class ReturnTypeMismatchException : Exception
    {
        public ReturnTypeMismatchException()
        {
        }

        public ReturnTypeMismatchException(string message) : base(message)
        {
        }

        public ReturnTypeMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReturnTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
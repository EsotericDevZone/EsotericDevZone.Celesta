using System;
using System.Runtime.Serialization;

namespace EsotericDevZone.Celesta.Parser
{
    [Serializable]
    internal class LiteralException : Exception
    {
        public LiteralException()
        {
        }

        public LiteralException(string message) : base(message)
        {
        }

        public LiteralException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LiteralException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace EsotericDevZone.Celesta.Providers
{
    [Serializable]
    internal class MultipleDefinitionException : Exception
    {
        public MultipleDefinitionException()
        {
        }

        public MultipleDefinitionException(string message) : base(message)
        {
        }

        public MultipleDefinitionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipleDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace EsotericDevZone.Celesta.Definitions
{
    [Serializable]
    internal class SymbolInitializationException : Exception
    {
        public SymbolInitializationException()
        {
        }

        public SymbolInitializationException(string message) : base(message)
        {
        }

        public SymbolInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SymbolInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
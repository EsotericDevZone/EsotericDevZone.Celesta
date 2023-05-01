using System;
using System.Runtime.Serialization;

namespace EsotericDevZone.Celesta.AST
{
    [Serializable]
    internal class ASTBuildNodeException : Exception
    {
        public ASTBuildNodeException()
        {
        }

        public ASTBuildNodeException(string message) : base(message)
        {
        }

        public ASTBuildNodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ASTBuildNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
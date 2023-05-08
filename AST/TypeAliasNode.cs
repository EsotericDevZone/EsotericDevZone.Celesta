using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    internal class TypeAliasNode : AbstractASTNode
    {
        internal TypeAliasNode(IASTNode parent, DataType newType, DataType referencedType) : base(parent)
        {
        }

        public DataType DefinedType { get; }
        public DataType ReferencedType { get; }        
    }
}

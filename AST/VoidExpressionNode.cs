using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class VoidExpressionNode : AbstractExpressionNode
    {
        public VoidExpressionNode(IASTNode parent, DataType voidType) : base(parent, voidType) { }
    }
}

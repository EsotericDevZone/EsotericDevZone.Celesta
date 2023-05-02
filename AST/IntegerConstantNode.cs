using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class IntegerConstantNode : AbstractExpressionNode
    {
        public int Value { get; }

        public IntegerConstantNode(IASTNode parent, int value, DataType type) : base(parent, type)
        {
            Value = value;            
        }

        public override string ToString() => $"{Value}i";
    }
}

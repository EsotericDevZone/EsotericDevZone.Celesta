using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    internal class RealConstantNode : AbstractExpressionNode
    {
        public double Value { get; }
        public RealConstantNode(IASTNode parent, double value, DataType type) : base(parent, type)
        {
            Value = value;
        }

        public override string ToString() => $"{Value}r";        
    }
}

using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.AST
{
    internal class StringConstantNode : AbstractExpressionNode
    {
        public string Value { get; }
        public StringConstantNode(IASTNode parent, string value, DataType type) : base(parent, type)
        {
            Value = value;
        }

        public override string ToString() => $"{Value.ToLiteral()}";
    }
}

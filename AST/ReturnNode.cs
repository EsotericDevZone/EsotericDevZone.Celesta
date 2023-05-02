using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class ReturnNode : AbstractASTNode, IExpressionNode
    {
        public ReturnNode(IASTNode parent) : base(parent) { }
        public IExpressionNode ReturnedExpression { get; internal set; }
        public DataType OutputType => ReturnedExpression.OutputType;

        public override string ToString() => $"return {ReturnedExpression}";
    }
}

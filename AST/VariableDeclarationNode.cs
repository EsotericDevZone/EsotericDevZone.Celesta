using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class VariableDeclarationNode : AbstractASTNode
    {
        
        public VariableNode VariableNode { get; internal set; }        
        public DataType DataType => VariableNode.OutputType;
        public IExpressionNode AssignedExpression { get; internal set; }

        internal VariableDeclarationNode(IASTNode parent, VariableNode variable = null, IExpressionNode assignedExpression = null)
            : base(parent)
        {
            VariableNode = variable;            
            AssignedExpression = assignedExpression;
        }

        public override string ToString() => $"{DataType.FullName} {VariableNode} = {AssignedExpression}";
    }
}

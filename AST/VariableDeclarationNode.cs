using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    internal class VariableDeclarationNode : AbstractASTNode
    {
        
        public VariableNode VariableNode { get; internal set; }        
        public DataType DataType => VariableNode.OutputType;
        public IASTNode AssignedExpression { get; internal set; }

        internal VariableDeclarationNode(IASTNode parent, VariableNode variable = null, IASTNode assignedExpression = null)
            : base(parent)
        {
            VariableNode = variable;            
            AssignedExpression = assignedExpression;
        }

        public override string ToString() => $"{DataType.FullName} {VariableNode} = {AssignedExpression}";
    }
}

using System;

namespace EsotericDevZone.Celesta.AST
{
    internal class AssignmentNode : AbstractASTNode
    {
        internal AssignmentNode(IASTNode parent) : base(parent)
        {
        }

        public IExpressionNode LeftHandSide { get; internal set; }
        public IExpressionNode RightHandSide { get; internal set; }               

        public override string ToString() => $"{LeftHandSide}={RightHandSide}";     
    }
}

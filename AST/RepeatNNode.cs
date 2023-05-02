using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.AST
{
    public class RepeatNNode : AbstractASTNode
    {
        internal RepeatNNode(IASTNode parent) : base(parent) { }
        public IExpressionNode Count { get; internal set; }
        public IASTNode LoopLogic { get; internal set; }
        public override string ToString() => $"repeat {Count} do\n{LoopLogic.ToString().Indent("    ")}\nendrepeat";        
    }
}

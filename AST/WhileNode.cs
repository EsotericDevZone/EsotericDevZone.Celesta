using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.AST
{
    public class WhileNode : AbstractASTNode
    {
        internal WhileNode(IASTNode parent) : base(parent) { }
        public IExpressionNode Condition { get; internal set; }
        public IASTNode LoopLogic { get; internal set; }
        public override string ToString()
        {
            return $"while {Condition} do\n{LoopLogic.ToString().Indent("    ")}\nendwhile";
        }
    }
}

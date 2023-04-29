using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class WhileBlock : IParseTreeNode
    {
        public IParseTreeNode Condition { get; }
        public IParseTreeNode Loop { get; }

        public WhileBlock(IParseTreeNode condition, IParseTreeNode loop)
        {
            Condition = condition;
            Loop = loop;
        }

        public override string ToString() => $"while {Condition} do\n{Loop.ToString().Indent("    ")}\nendwhile";
    }
}

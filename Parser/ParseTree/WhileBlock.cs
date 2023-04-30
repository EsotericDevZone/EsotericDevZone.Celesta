using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class WhileBlock : IParseTreeNode
    {
        public IParseTreeNode Condition { get; }
        public Block Loop { get; }

        public WhileBlock(IParseTreeNode condition, Block loop)
        {
            Condition = condition;
            Loop = loop;
        }

        public override string ToString() => $"while {Condition} do\n{Loop.ToString().Indent("    ")}\nendwhile";
    }
}

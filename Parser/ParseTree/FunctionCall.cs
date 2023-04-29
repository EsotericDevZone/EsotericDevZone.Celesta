using System.Linq;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class FunctionCall : IParseTreeNode
    {
        public IParseTreeNode Function { get; }
        public IParseTreeNode[] Arguments { get; }

        public FunctionCall(IParseTreeNode function, IParseTreeNode[] arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public override string ToString() => $"f:{Function}({string.Join(", ", Arguments.ToList())})";

    }
}

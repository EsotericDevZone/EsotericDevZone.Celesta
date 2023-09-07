using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    internal class CallParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode Target { get; }
        public IParseTreeNode[] Arguments { get; }

        public CallParseTreeNode(IParseTreeNode target, IParseTreeNode[] arguments)
            : base(target.StartPosition, arguments.Length == 0 ? target.EndPosition.Advance(2) : arguments.Last().EndPosition.Advance(1))
        {
            Target = target;
            Arguments = arguments.ToArray();
        }

        public override string ToString() => $"{Target}({Arguments.JoinToString(", ")})";
    }
}

using EsotericDevZone.Core;
using System.Collections.Generic;
using System.Linq;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class Block : IParseTreeNode
    {
        public IParseTreeNode[] Children { get; }

        public Block(IParseTreeNode[] children)
        {
            Children = children;
        }

        public override string ToString()
        {            
            return Children.Select(_ => _.ToString() + ";").JoinToString("\n");
        }
    }
}

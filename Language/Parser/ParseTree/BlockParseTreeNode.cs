using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class BlockParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode[] Nodes { get; }

        public BlockParseTreeNode(IParseTreeNode[] nodes, ParsePosition beginPosition, ParsePosition endPosition) : base(beginPosition, endPosition)
        {
            Nodes = nodes;
        }

        public BlockParseTreeNode(IParseTreeNode[] nodes, Token beginToken, Token endToken)
            : base(beginToken.ParsePosition, endToken.ParsePosition)
        {
            Nodes = nodes;
        }

        public override string ToString()
        {
            var br = Environment.NewLine;
            return $"{{{br}{Nodes.Select(n => n.ToString() + $"{{{n.StartPosition?.Index}:{n.EndPosition?.Index}}}").JoinToString(br).Indent(br, 4)}{br}}}";
        }
    }
}

using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class RootParseTreeNode : BlockParseTreeNode
    {
        public RootParseTreeNode(IParseTreeNode[] nodes, ParsePosition beginPosition, ParsePosition endPosition) : base(nodes, beginPosition, endPosition)
        {
        }
    }
}

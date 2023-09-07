using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class AccessorParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode Target { get; }
        public SymbolParseTreeNode Member { get; }

        public AccessorParseTreeNode(IParseTreeNode target, SymbolParseTreeNode member)
            : base(target.StartPosition, member.EndPosition)
        {
            Target = target;
            Member = member;
        }

        public override string ToString() => $"({Target}.{Member})";
    }
}

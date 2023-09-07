using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class SymbolParseTreeNode : AbstractParseTreeNode
    {
        public string SymbolName { get; }

        public SymbolParseTreeNode(Token token) : base(token.ParsePosition, token.ParsePosition)
        {
            SymbolName = token.Text;
        }

        public override string ToString() => $"Symbol{{{SymbolName}}}";
    }
}

using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class StringLiteralParseTreeNode : AbstractParseTreeNode
    {
        public string Value { get; set; }

        public StringLiteralParseTreeNode(Token token) : base(token.ParsePosition, token.ParsePosition)
        {
            Value = token.Text;
        }

        public override string ToString() => Value;        
    }
}

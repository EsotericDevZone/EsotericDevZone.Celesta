using EsotericDevZone.Celesta.Parser;
using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class IntegerLiteralParseTreeNode : AbstractParseTreeNode
    {
        public string Value { get; }

        public IntegerLiteralParseTreeNode(Token token)
            : base(token.ParsePosition, token.ParsePosition)
        {
            Value = token.Text;
        }

        public override string ToString() => Value.ToString();        

    }
}

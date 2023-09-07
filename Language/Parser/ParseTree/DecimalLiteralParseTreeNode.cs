using EsotericDevZone.Celesta.Tokenizer;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class DecimalLiteralParseTreeNode : AbstractParseTreeNode
    {
        public string TextValue { get; }
        public DecimalLiteralParseTreeNode(Token token)
            : base(token.ParsePosition, token.ParsePosition)
        {
            TextValue = token.Text;
        }

        public override string ToString() => TextValue;        
    }
}

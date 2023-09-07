using EsotericDevZone.Celesta.Parser;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class AbstractParseTreeNode : IParseTreeNode
    {
        public AbstractParseTreeNode(ParsePosition startPosition, ParsePosition endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public ParsePosition StartPosition { get; }
        public ParsePosition EndPosition { get; }
    }
}

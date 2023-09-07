using EsotericDevZone.Celesta.Parser;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public interface IParseTreeNode
    {
        ParsePosition StartPosition { get; }
        ParsePosition EndPosition { get; }
    }
}

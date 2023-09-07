using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public abstract class AbstractASTNode : IASTNode
    {
        protected AbstractASTNode(IParseTreeNode parseTreeNode, Scope scope)
        {
            ParseTreeNode = parseTreeNode;            
            ResidingScope = scope;
        }

        public IParseTreeNode ParseTreeNode { get; }
        public ParsePosition BeginPosition => ParseTreeNode.StartPosition;
        public ParsePosition EndPosition => ParseTreeNode.EndPosition;
        public Scope ResidingScope { get; }
    }
}

using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System.Linq;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public class BlockNode : AbstractASTNode
    {
        public BlockNode(IParseTreeNode parseTreeNode, Scope scope, Scope innerScope, IASTNode[] children) : base(parseTreeNode, scope)
        {
            InnerScope = innerScope;
            Children = children.ToArray();            
        }

        public Scope InnerScope { get; }

        private IASTNode[] Children { get; }

        public IASTNode[] GetChildren() => Children.ToArray();

        public override string ToString()
        {
            return $"block {InnerScope}\r\n{Children.JoinToString("\r\n").Indent("\r\n", 2)}\r\nend block {InnerScope}";
        }

    }
}

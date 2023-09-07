using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    internal class InvalidSymbolNode : AbstractDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo>, IErrorNode
    {
        public string SymbolName { get; set; }

        public string ErrorMessage => $"Unknown symbol '{SymbolName}'";

        public InvalidSymbolNode(IParseTreeNode parseTreeNode, Scope scope, string symbolName) : base(parseTreeNode, scope)
        {
            SymbolName = symbolName;
        }

        public override string ToString() => ErrorMessage;
    }
}

using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    internal class InvalidAccessorNode : AbstractDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo>, IErrorNode
    {
        public IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> Parent { get; }
        public string SymbolName { get; set; }
        public string ErrorMessage => $"Unknown accessor '{SymbolName}' for {Parent}";

        public InvalidAccessorNode(IParseTreeNode parseTreeNode, Scope scope, IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> parent, string symbolName) : base(parseTreeNode, scope)
        {
            Parent = parent;
            SymbolName = symbolName;                                    
        }
        

        public override string ToString() => ErrorMessage;
    }
}

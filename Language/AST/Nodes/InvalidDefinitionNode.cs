using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public class InvalidDefinitionNode<D, DI> : AbstractDefinitionHolderNode<D, DI>, IErrorNode
        where D : IDefinition<DI>
        where DI:DefinitionInfo
    {
        public IDefinition<DefinitionInfo> ActualDefinition { get; }

        public InvalidDefinitionNode(IParseTreeNode parseTreeNode, Scope scope, IDefinition<DefinitionInfo> actualDefinition) : base(parseTreeNode, scope)
        {
            ActualDefinition = actualDefinition;
            ErrorMessage = IsSpeculative
                ? $"Undefined identifier provided as {typeof(D).Name}"
                : $"Expected {typeof(D).Name}, got {actualDefinition.GetType().Name}: '{actualDefinition.FullName}'";
        }

        public string ErrorMessage { get; }

        public override string ToString() => ErrorMessage;        
    }
}

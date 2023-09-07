using EsotericDevZone.Celesta.Language.AST.Nodes.DefinitionHolders;
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
    public abstract class AbstractDefinitionHolderNode<D,DI> : AbstractASTNode, IDefinitionHolderNode<D, DI>
        where D : IDefinition<DI>
        where DI:DefinitionInfo
    {
        protected AbstractDefinitionHolderNode(IParseTreeNode parseTreeNode, Scope scope) : base(parseTreeNode, scope)
        {
            EditableDefHolder = new DefinitionHolder<D, DI>();
        }

        public IDefinitionHolder<D, DI> DefinitionHolder => EditableDefHolder;
        
        protected DefinitionHolder<D,DI> EditableDefHolder { get; }

        public D Definition => DefinitionHolder.Definition;

        public bool IsSpeculative => DefinitionHolder.IsSpeculative;        

        public DD GetDefinition<DD>() where DD:class, IDefinition<DefinitionInfo>
        {
            if (Definition == null) return null;

            var result = Definition as DD;

            if (result == null)
                throw new InvalidCastException($"Invalid definition: expected {typeof(DD)}, got {typeof(D)}");

            return result;            
        }
    }
}

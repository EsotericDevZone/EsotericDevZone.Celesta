using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using EsotericDevZone.Celesta.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{
    public class PackageNode : AbstractDefinitionHolderNode<Package, PackageInfo>
    {
        public string PackageName { get; }
        public Package Package => Definition;        

        public PackageNode(IParseTreeNode parseTreeNode, Scope scope, Package package) : base(parseTreeNode, scope)
        {
            EditableDefHolder.Definition = package;            
        }       

        public override string ToString()
        {
            if (IsSpeculative)
                return $"speculative package '{PackageName}'";
            else
                return $"package '{Package}'";
        }
    }
}

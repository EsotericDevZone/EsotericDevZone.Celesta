using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Language.AST.Nodes;
using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace EsotericDevZone.Celesta.Language.AST.Builder
{
    public class DefaultASTBuilder : ASTBuilder
    {
        #region Blocks

        

        public BlockNode BuildNode(PackageParseTreeNode node, BuildContext bc, Scope scope)
        {
            var pkg = bc.Definitions.CreateOrAddPackage(new PackageInfo(scope, node.PackageName));
            var pkgScope = pkg.InnerScope;
            var children = node.Content.Nodes.Select(_ => base.Build(_, bc, pkgScope)).ToArray();
            return new BlockNode(node, scope, pkgScope, children);
        }

        public BlockNode BuildNode(RootParseTreeNode node, BuildContext bc, Scope scope)
        {            
            var children = node.Nodes.Select(_ => base.Build(_, bc, bc.GlobalScope)).ToArray();
            return new BlockNode(node, scope, bc.GlobalScope, children);
        }

        public BlockNode BuildNode(BlockParseTreeNode node, BuildContext bc, Scope scope)
        {
            var blockScope = new Scope(bc.ScopeIdProvider, scope);
            var children = node.Nodes.Select(_ => base.Build(_, bc, blockScope)).ToArray();
            return new BlockNode(node, scope, blockScope, children);
        }

        #endregion


        #region Symbols & Accessors
        public IASTNode BuildNode(AccessorParseTreeNode node, BuildContext bc, Scope scope)
        {
            var lhs = Build(node.Target, bc, scope);
            var accessedName = node.Member.SymbolName;

            if (lhs is InvalidSymbolNode unk)
                return unk;

            if (lhs is IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> defNode && defNode.IsSpeculative)           
                return new InvalidSymbolNode(node, scope, accessedName);

            if (lhs is PackageNode packageNode) 
            {
                var definition = bc.Definitions
                    .FindDefinitions(packageNode.Package.InnerScope, accessedName, rightUnderScope: false)
                    .FirstOrDefault();

                return DefinitionToNode(node.Member, scope, definition) ?? new InvalidAccessorNode(node, scope, packageNode, accessedName);                    
            }
                         
            if (!(lhs is IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> defHolder))
                throw new InvalidOperationException($"Accessed node is not a definition holder");

            return new InvalidAccessorNode(node, scope, defHolder, accessedName);
            
        }

        public IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> BuildNode(SymbolParseTreeNode node, BuildContext bc, Scope scope)
        {
            var definition = bc.Definitions.FindDefinitions(scope, node.SymbolName, rightUnderScope: false)
                .FirstOrDefault();

            return DefinitionToNode(node, scope, definition) ?? new InvalidSymbolNode(node, scope, node.SymbolName);
        }

        private IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> DefinitionToNode(SymbolParseTreeNode node, Scope scope, IDefinition<DefinitionInfo> definition)
        {
            if (definition is DataType dataType)
            {
                return new DataTypeNode(node, scope, dataType);
            }
            if (definition is Package package)
            {
                return new PackageNode(node, scope, package);
            }
            return null;            
        }

        #endregion


        public IASTNode BuildNode(VariableDeclarationParseTreeNode node, BuildContext bc, Scope scope)
        {
            var __dataTypeNode = Build(node.DataTypeIdentifier, bc, scope);
            if (!(__dataTypeNode is IDefinitionHolderNode<IDefinition<DefinitionInfo>, DefinitionInfo> defHolderNode))
                throw new InvalidOperationException("DataType node is not a definition holder");


            if (!(defHolderNode is IDefinitionHolderNode<DataType, DataTypeInfo> dataTypeNode))
                return new InvalidDefinitionNode<DataType, DataTypeInfo>(node, scope, defHolderNode.Definition);

            var dataTypeDefinition = dataTypeNode.Definition;

            var existingDef = bc.Definitions.FindDefinitions(scope, node.VariableName, rightUnderScope: true).FirstOrDefault();
            if (existingDef != null)
                return new ErrorNode(node, scope, $"Duplicate definition of {node.VariableName}");

            var variable = bc.Definitions.CreateVariable(new VariableInfo(scope, node.VariableName, dataTypeDefinition.DefinitionInfo));

            return new VariableDeclarationNode(node, scope, dataTypeNode, node.VariableName);



        }
    }
}

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
    public class VariableDeclarationNode : AbstractASTNode
    {
        public VariableDeclarationNode(IParseTreeNode parseTreeNode, Scope scope, IDefinitionHolderNode<DataType, DataTypeInfo> dataTypeNode, string variableName) : base(parseTreeNode, scope)
        {
            DataTypeNode = dataTypeNode;
            VariableName = variableName;
        }
        public IDefinitionHolderNode<DataType, DataTypeInfo> DataTypeNode { get; }
        public string VariableName { get; }

        public override string ToString() => $"vardecl {VariableName} as {DataTypeNode}";
    }
}

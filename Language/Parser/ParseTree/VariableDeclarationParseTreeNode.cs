using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class VariableDeclarationParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode DataTypeIdentifier { get; }
        public string VariableName { get; }
        public IParseTreeNode InitialValueExpression { get; }

        public VariableDeclarationParseTreeNode(IParseTreeNode dataTypeIdentifier, SymbolParseTreeNode variableName, IParseTreeNode initialValueExpression)
            : base(dataTypeIdentifier.StartPosition, initialValueExpression.EndPosition)
        {
            DataTypeIdentifier = dataTypeIdentifier;
            VariableName = variableName.SymbolName;
            InitialValueExpression = initialValueExpression;
        }

        public VariableDeclarationParseTreeNode(IParseTreeNode dataTypeIdentifier, SymbolParseTreeNode variableName)
            : base(dataTypeIdentifier.StartPosition, variableName.EndPosition)
        {
            DataTypeIdentifier = dataTypeIdentifier;
            VariableName = variableName.SymbolName;
        }

        public override string ToString() => $"{DataTypeIdentifier} {VariableName} = {InitialValueExpression?.ToString() ?? "<unset>"}";
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class BinaryOperatorParseTreeNode : AbstractParseTreeNode
    {
        public string Operator { get; }
        public IParseTreeNode Operand1 { get; }
        public IParseTreeNode Operand2 { get; }
        public BinaryOperatorParseTreeNode(string @operator, IParseTreeNode operand1, IParseTreeNode operand2)
            : base(operand1.StartPosition, operand2.EndPosition)
        {
            Operator = @operator;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public override string ToString() => $"({Operand1} {Operator} {Operand2})";
    }
}

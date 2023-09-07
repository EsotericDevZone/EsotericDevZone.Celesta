using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    public class PostFixOperatorParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode Operand { get; }
        public string Operator { get; set; }

        public PostFixOperatorParseTreeNode(IParseTreeNode operand, string @operator) :
            base(operand.StartPosition, operand.EndPosition.Advance(1))
        {
            Operand = operand;
            Operator = @operator;
        }

        public override string ToString() => $"postfix({Operand},{Operator})";
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Parser.ParseTree
{
    internal class PrefixOperatorParseTreeNode : AbstractParseTreeNode
    {
        public IParseTreeNode Operand { get; }
        public string Operator { get; set; }

        public PrefixOperatorParseTreeNode(IParseTreeNode operand, string @operator)
            : base(operand.StartPosition.Advance(-1), operand.EndPosition)
        {
            Operand = operand;
            Operator = @operator;
        }

        public override string ToString() => $"prefix({Operand},{Operator})";
    }
}

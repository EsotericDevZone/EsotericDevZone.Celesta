using EsotericDevZone.Celesta.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    public class BooleanConstantNode : AbstractExpressionNode
    {
        public bool Value { get; }

        public BooleanConstantNode(IASTNode parent, bool value, DataType type) : base(parent, type)
        {
            Value = value;
        }

        public override string ToString() => $"{Value}b";
    }
}

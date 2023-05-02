using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class VariableNode : AbstractExpressionNode
    {
        public Variable Variable { get; }

        internal VariableNode(IASTNode parent, Variable variable) : base(parent, variable.DataType)
        {
            Variable = variable;
        }

        public override string ToString() => Variable.FullName + Variable.ScopeName;
    }
}

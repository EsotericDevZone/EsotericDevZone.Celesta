using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta.AST
{
    public class VariableNode : AbstractExpressionNode
    {
        public Variable Variable { get; }
        public bool IsParameter => Variable.IsParameter;
        public int ParamId => Variable.ParamId;

        internal VariableNode(IASTNode parent, Variable variable) : base(parent, variable.DataType)
        {
            Variable = variable;
        }

        public override string ToString() => Variable.FullName + Variable.ScopeName;
    }
}

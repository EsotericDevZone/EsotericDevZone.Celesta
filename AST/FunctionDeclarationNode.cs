using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Definitions.Functions;
using EsotericDevZone.Core;
using System.Linq;

namespace EsotericDevZone.Celesta.AST
{
    public class FunctionDeclarationNode : AbstractASTNode, IExpressionNode
    {
        internal FunctionDeclarationNode(IASTNode parent) : base(parent)
        {
        }

        public Function Function { get; internal set; }

        public IASTNode Body { get; internal set; }

        public bool IsUserDefined => Function is UserDefinedFunction;
        public bool IsSyscall => Function is SyscallFunction;

        public DataType OutputType => Function.OutputType;

        public override string ToString()
        {
            if (Function is UserDefinedFunction udf)
                return $"function {udf.FullName}({udf.FormalParameters.Select(p => $"{p.Type} {p.Name}").JoinToString(",")}) : {OutputType}\n" +
                    $"begin\n{Body.ToString().Indent("    ")}\nendfunction";
            else if (Function is BuiltInFunction bif)
                return $"<builtin> function {bif.FullName}({bif.ArgumentTypes.JoinToString(",")}) : {OutputType}\n";
            return Function.ToString();
        }


    }
}

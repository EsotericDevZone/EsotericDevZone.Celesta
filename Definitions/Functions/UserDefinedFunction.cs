using EsotericDevZone.Celesta.AST;

namespace EsotericDevZone.Celesta.Definitions.Functions
{
    public class UserDefinedFunction : Function
    {
        public UserDefinedFunction(string name, string package, string scope, DataType[] argumentTypes, DataType outputType) 
            : base(name, package, scope, argumentTypes, outputType)
        {
        }

        public FunctionFormalParameter[] FormalParameters { get; internal set; }
        public IASTNode Body { get; internal set; }
    }

    public class FunctionFormalParameter
    {
        public FunctionFormalParameter(string name, DataType type, int index)
        {
            Name = name;
            Type = type;
            Index = index;
        }

        public string Name { get; }
        public DataType Type { get; }
        public int Index { get; }
    }
}

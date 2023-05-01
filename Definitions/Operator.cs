using EsotericDevZone.Core.Collections;

namespace EsotericDevZone.Celesta.Definitions
{
    public class Operator : AbstractSymbol
    {
        private Operator(string name, string package, string scope, DataType[] argumentTypes, DataType outputType) : base(name, package, scope)
        {
            ArgumentTypes = argumentTypes;
            OutputType = outputType;
        }

        public DataType[] ArgumentTypes { get; }
        public DataType OutputType { get; }

        public bool IsUnary => ArgumentTypes.Length == 1;
        public bool IsBinary => ArgumentTypes.Length == 2;

        public static Operator BinaryOperator(string name, DataType argType1, DataType argType2, DataType outputType)
            => new Operator(name, null, null, Arrays.Of(argType1, argType2), outputType);
        public static Operator UnaryOperator(string name, DataType argType, DataType outputType)
            => new Operator(name, null, null, Arrays.Of(argType), outputType);
    }
}

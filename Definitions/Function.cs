using System.Linq;

namespace EsotericDevZone.Celesta.Definitions
{
    public class Function : AbstractSymbol
    {
        public Function(string name, string package, string scope, DataType[] argumentTypes, DataType outputType) 
            : base(name, package, scope)
        {
            ArgumentTypes = argumentTypes.ToArray();
            OutputType = outputType;
        }

        public DataType[] ArgumentTypes { get; }
        public DataType OutputType { get; }
    }
}

using EsotericDevZone.Core;
using System.Linq;

namespace EsotericDevZone.Celesta.Definitions
{
    public abstract class Function : AbstractSymbol
    {
        public Function(string name, string package, string scope, DataType[] argumentTypes, DataType outputType) 
            : base(name, package, scope)
        {
            ArgumentTypes = argumentTypes.ToArray();
            OutputType = outputType;
        }

        public DataType[] ArgumentTypes { get; }
        public DataType OutputType { get; }

        public override string ToString() => $"{base.ToString()}({ArgumentTypes.JoinToString(",")})->{OutputType?.ToString() ?? "?"}";
    }
}

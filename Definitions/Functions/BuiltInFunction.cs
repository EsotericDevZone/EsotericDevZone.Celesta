namespace EsotericDevZone.Celesta.Definitions.Functions
{
    public class BuiltInFunction : Function
    {
        public BuiltInFunction(string name, string package, DataType[] argumentTypes, DataType outputType)
            : base(name, package, null, argumentTypes, outputType)
        { }
    }
}

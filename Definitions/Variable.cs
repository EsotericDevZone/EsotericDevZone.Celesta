namespace EsotericDevZone.Celesta.Definitions
{
    public class Variable : AbstractSymbol
    {
        public bool IsParameter { get; }
        public int ParamId { get; }

        public Variable(string name, string package, string scope, DataType dataType, bool isParameter=false, int paramId=0) : base(name, package, scope)
        {
            DataType = dataType;
            IsParameter = isParameter;
            ParamId = paramId;
        }

        public DataType DataType { get; }
    }
}

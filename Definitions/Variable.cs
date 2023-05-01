namespace EsotericDevZone.Celesta.Definitions
{
    public class Variable : AbstractSymbol
    {
        public Variable(string name, string package, string scope, DataType dataType) : base(name, package, scope)
        {
            DataType = dataType;
        }

        public DataType DataType { get; }
    }
}

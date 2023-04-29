namespace EsotericDevZone.Celesta.Definitions
{
    public class DataType : AbstractSymbol
    {      
        public bool IsPrimitive { get; }
        public DataType BaseType { get; }
        
        public bool IsDerivedFrom(DataType dataType)
        {
            return dataType == BaseType || (BaseType?.IsDerivedFrom(dataType) ?? false);
        }

        public DataType(string name, string package, string scope, bool isPrimitive, DataType baseType)
            : base(name, package, scope)
        {
            IsPrimitive = isPrimitive;

            if (IsPrimitive && baseType != null) 
            {
                throw new SymbolInitializationException("Primitive types can not be derived");                
            }

            if (baseType?.IsPrimitive ?? false)
            {
                throw new SymbolInitializationException("Cannot inherit from primitive type");
            }                
        }

        public static DataType Primitive(string name, string package, string scope)
            => new DataType(name, package, scope, true, null);
    }
}

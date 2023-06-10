namespace EsotericDevZone.Celesta.Definitions
{
    public enum DataTypeProperties
    {
        Primitive = 1<<0,
        Alias = 1<<1,
        Isolated = 1<<2,
    }

    public class DataType : AbstractSymbol
    {      
        public DataTypeProperties Properties { get; }

        public bool IsPrimitive => Properties.HasFlag(DataTypeProperties.Primitive);
        public DataType BaseType { get; }
        public DataType AliasedType { get; }
        public bool IsAlias => Properties.HasFlag(DataTypeProperties.Alias);
        public bool IsIsolated => Properties.HasFlag(DataTypeProperties.Isolated);

        public bool IsDerivedFrom(DataType dataType)
        {
            return dataType == BaseType || (BaseType?.IsDerivedFrom(dataType) ?? false);
        }

        public bool IsAliasOf(DataType dataType)
        {
            return IsAlias && ((AliasedType == dataType && !IsIsolated) || AliasedType.IsAliasOf(dataType));
        }

        public DataType(string name, string package, string scope, DataTypeProperties properties, DataType baseType, DataType aliasedType)
            : base(name, package, scope)
        {
            Properties = properties;            

            if (IsPrimitive && baseType != null) 
            {
                throw new SymbolInitializationException("Primitive types can not be derived");                
            }

            if (baseType?.IsPrimitive ?? false)
            {
                throw new SymbolInitializationException("Cannot inherit from primitive type");
            }
            AliasedType = aliasedType;
        }

        public static DataType Primitive(string name, string package, string scope)
            => new DataType(name, package, scope, DataTypeProperties.Primitive, null, null);

        public static DataType Alias(string name, string package, string scope, DataType referencedType, bool isolated)
            => isolated
                ? new DataType(name, package, scope, DataTypeProperties.Alias | DataTypeProperties.Isolated, null, referencedType)
                : new DataType(name, package, scope, DataTypeProperties.Alias, null, referencedType);

        public bool Substitutes(DataType type)
        {
            return this == type || IsAliasOf(type);
        }
    }
}

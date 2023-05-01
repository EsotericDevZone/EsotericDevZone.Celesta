namespace EsotericDevZone.Celesta.Definitions
{
    public abstract class AbstractSymbol : ISymbol
    {
        public string Name { get; }
        public string PackageName { get; }
        public string ScopeName { get; }
        public string FullName => PackageName == "" ? Name : $"{PackageName}#{Name}";

        protected AbstractSymbol(string name, string package, string scope)
        {
            Name = name;
            PackageName = package;
            ScopeName = scope;
        }

        public override string ToString() => $"{PackageName}.{Name}@{ScopeName}";
    }

}
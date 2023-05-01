using System.Linq;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    public class Identifier : IParseTreeNode
    {
        public string PackageName { get; }
        public string Name { get; }

        internal Identifier(string[] symbols)
        {
            PackageName = string.Join("#", symbols.Take(symbols.Length - 1));
            Name = symbols.Last();
        }

        public override string ToString() => $"Identifier('{FullName}')";        

        public string FullName => PackageName == "" ? Name : $"{PackageName}#{Name}";
    }
}

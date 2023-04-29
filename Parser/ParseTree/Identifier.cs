using System.Linq;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class Identifier : IParseTreeNode
    {
        public string PackageName { get; }
        public string Name { get; }

        public Identifier(string[] symbols)
        {
            PackageName = string.Join("#", symbols.Take(symbols.Length - 1));
            Name = symbols.Last();
        }

        public override string ToString()
        {
            if (PackageName == "")
                return $"Identifier('{Name}')";
            else
                return $"Identifier('{PackageName}#{Name}')";
        }
    }
}

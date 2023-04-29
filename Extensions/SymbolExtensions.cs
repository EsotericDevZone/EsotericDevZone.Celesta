using EsotericDevZone.Celesta.Definitions;

namespace EsotericDevZone.Celesta
{
    public static partial class Extensions
    {
        public static bool IsVisibleFromScope(this ISymbol symbol, string scopeName)
            => scopeName.StartsWith(symbol.ScopeName);
    }
}

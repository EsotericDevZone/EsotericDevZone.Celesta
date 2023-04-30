using System;

namespace EsotericDevZone.Celesta.AST
{
    public interface IASTNode
    {
        IASTNode Parent { get; }

        string GetScopeName();
        string GetPackageName();

        bool IsIncludedIn(Type nodeType);        
        bool IsDirectlyIncludedIn(Type nodeType);        
        

    }
}

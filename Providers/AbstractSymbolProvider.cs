using EsotericDevZone.Celesta.Definitions;
using EsotericDevZone.Celesta.Parser.ParseTree;
using EsotericDevZone.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsotericDevZone.Celesta.Providers
{
    internal class AbstractSymbolProvider<T> : ISymbolProvider<T> where T : class, ISymbol
    {
        List<T> Symbols = new List<T>();

        public IEnumerable<T> Find(string name, string scope)
        {
            return Symbols.Where(s => s.Name == name && scope.StartsWith(s.ScopeName));
        }

        public IEnumerable<T> Find(string package, string name, string scope)
        {
            return Symbols.Where(s => s.Name == name && (s.ScopeName==null || scope.StartsWith(s.ScopeName))
                && s.PackageName == package);
        }

        public IEnumerable<T> Find(Identifier identifier, string scope)
            => Find(identifier.PackageName, identifier.Name, scope);

        public void Add(T symbol)
        {
            Symbols.Add(symbol);
        }

        public IEnumerable<T> GetAll() => Symbols.ToList();

        public void Clear()
        {
            Symbols.Clear();
        }

        public void AddFromProvider(ISymbolProvider<T> provider)
        {
            Symbols.AddRange(provider.GetAll());
        }

        public void CopyFromProvider(ISymbolProvider<T> provider)
        {
            Clear();
            AddFromProvider(provider);
        }

        public T Resolve(Identifier identifier, string scope, bool strict)
        {
            T[] candidates;            
            if (identifier.PackageName == "") 
            {                
                candidates = Find(identifier.Name, scope).ToArray();
            }
            else
                candidates = Find(identifier, scope).ToArray();
            if (candidates.Length == 0)
                return null;
            if (strict && candidates.Length > 1)
                throw new MultipleDefinitionException($"The identifier '{identifier}' is ambiguous:\n{candidates.JoinToString("\n").Indent("  ")}");
            return candidates[0];
        }

        public IEnumerable<T> Where(Func<T, bool> predicate) => Symbols.Where(predicate);

        public T Resolve(Func<T, bool> predicate, bool strict)
        {
            var candidates = Where(predicate).ToArray();
            if (candidates.Length == 0)
                return null;
            if (strict && candidates.Length > 1) 
                throw new MultipleDefinitionException($"Multiple definitions are matcing constraints:\n{candidates.JoinToString("\n").Indent("  ")}");
            return candidates[0];
        }
    }
}

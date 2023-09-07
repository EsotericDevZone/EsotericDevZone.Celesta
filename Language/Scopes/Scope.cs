using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Scopes
{
    public class Scope
    {
        public string Name { get; }
        public string Id { get; }
        public Scope Parent { get; }
        public List<Scope> VisibleScopes { get; } = new List<Scope>();

        public Scope(IScopeIdProvider idProvider, Scope parent = null)
        {
            Id = idProvider.GetScopeId();
            Parent = parent;
        }

        public Scope(IScopeIdProvider idProvider, string name, Scope parent = null)
        {
            Id = idProvider.GetScopeId();
            Name = name;
            Parent = parent;
        }

        public bool IsAddressable => string.IsNullOrEmpty(Name);
        public string FullName => Parent == null ? (Name ?? Id) : $"{Parent.FullName}.{Name ?? Id}";

        public IEnumerable<Scope> GetAccessibleScopes()
        {
            var scopes = new HashSet<Scope>();
            scopes.Add(this);
            var q = new Queue<Scope>(VisibleScopes);
            if (Parent != null) q.Enqueue(Parent);
            while (q.Count > 0)
            {
                var scope = q.Dequeue();
                if (scopes.Contains(scope))
                    continue;
                scopes.Add(scope);
                scope.VisibleScopes.ForEach(q.Enqueue);
                if (scope.Parent != null) q.Enqueue(scope.Parent);
            }
            return scopes;
        }

        public override string ToString() => FullName;        
    }
}

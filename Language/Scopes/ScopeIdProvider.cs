using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Scopes
{
    public interface IScopeIdProvider
    {
        string GetScopeId();
    }

    public class NumericScopeIdProvider : IScopeIdProvider
    {
        public int Counter = 0;

        public string GetScopeId() => $"@__S{Counter++}";
    }

    public class ContextOrientedScopeIdProvider : IScopeIdProvider
    { 
        public IScopeIdProvider InnerScopeIdProvider { get; }
        public string Context { get; }

        public ContextOrientedScopeIdProvider(IScopeIdProvider innerScopeIdProvider, string context)
        {
            InnerScopeIdProvider = innerScopeIdProvider;
            Context = context;
        }

        public string GetScopeId() => $"{InnerScopeIdProvider.GetScopeId()}__{Context}__";
    }

}

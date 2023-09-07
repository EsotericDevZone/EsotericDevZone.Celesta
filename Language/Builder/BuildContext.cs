using EsotericDevZone.Celesta.Language.Definitions;
using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Language.Builder
{
    public class BuildContext
    {
        public IScopeIdProvider ScopeIdProvider { get; }

        public DefinitionsContainer Definitions { get; }
        public Scope GlobalScope => Definitions.GlobalScope;

        public BuildContext()
        {
            ScopeIdProvider = new NumericScopeIdProvider();
            Definitions = new DefinitionsContainer(ScopeIdProvider);
        }
    }
}

using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public class ScopedDefinition<DI> : Definition<DI>, IScopeOwner where DI : DefinitionInfo
    {
        public ScopedDefinition(DI definitionInfo, IScopeIdProvider scopeIdProvider) : base(definitionInfo)
        {
            InnerScope = new Scope(scopeIdProvider, Name, DefinitionInfo.Scope);
        }

        public Scope InnerScope { get; }
    }
}

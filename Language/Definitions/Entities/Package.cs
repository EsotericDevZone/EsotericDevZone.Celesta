using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public class Package : ScopedDefinition<PackageInfo>
    {
        public Package(PackageInfo definitionInfo, IScopeIdProvider scopeIdProvider) : base(definitionInfo, scopeIdProvider)
        {
        }        
    }
}

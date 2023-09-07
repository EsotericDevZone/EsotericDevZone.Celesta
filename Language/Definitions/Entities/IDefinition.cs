using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public interface IDefinition<out DI> where DI : DefinitionInfo
    {
        DI DefinitionInfo { get; }

        string Name { get; }
        Scope Scope { get; }
        string FullName { get; }
    }    
}

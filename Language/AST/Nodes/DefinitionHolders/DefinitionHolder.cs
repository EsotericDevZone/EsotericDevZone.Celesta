using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.AST.Nodes.DefinitionHolders
{
    public interface IDefinitionHolder<out D, out DI> where DI:DefinitionInfo where D:IDefinition<DI>
    {
        D Definition { get; }
        bool IsSpeculative { get; }        
    }

    public class DefinitionHolder<D, DI> : IDefinitionHolder<D, DI> where DI : DefinitionInfo where D:IDefinition<DI>
    {
        public D Definition { get; set; }
        public bool IsSpeculative => Definition == null;
    }
}

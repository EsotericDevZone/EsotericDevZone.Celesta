using EsotericDevZone.Celesta.Language.AST.Nodes.DefinitionHolders;
using EsotericDevZone.Celesta.Language.Builder;
using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.AST.Nodes
{    
    public interface IDefinitionHolderNode<out D, out DI> : IASTNode where D:IDefinition<DI> where DI : DefinitionInfo
    {
        IDefinitionHolder<D, DI> DefinitionHolder { get; }
        D Definition { get; }        
        bool IsSpeculative { get; }        

        DD GetDefinition<DD>() where DD : class, IDefinition<DefinitionInfo>;
    }       
}

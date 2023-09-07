using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;

namespace EsotericDevZone.Celesta.Language.Definitions.Entities
{
    public abstract class Definition<DI> : IDefinition<DI> where DI : DefinitionInfo
    {
        protected Definition(DI definitionInfo)
        {
            DefinitionInfo = definitionInfo;
        }

        public DI DefinitionInfo { get; }

        public string Name => DefinitionInfo.Name;
        public Scope Scope => DefinitionInfo.Scope;
        public string FullName => DefinitionInfo.FullName;


        public override string ToString() => $"{GetType().Name}{{{DefinitionInfo}}}";

    }
}

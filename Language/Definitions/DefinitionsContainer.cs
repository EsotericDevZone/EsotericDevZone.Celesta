using EsotericDevZone.Celesta.Language.Definitions.Entities;
using EsotericDevZone.Celesta.Language.Definitions.Info;
using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EsotericDevZone.Celesta.Language.Definitions
{
    public class DefinitionsContainer
    {
        private readonly IScopeIdProvider ScopeIdProvider;        

        private Scope MasterScope { get; }

        public Scope GlobalScope { get; }

        public DefinitionsContainer(IScopeIdProvider scopeIdProvider)
        {
            ScopeIdProvider = scopeIdProvider ?? new NumericScopeIdProvider();

            MasterScope = new Scope(ScopeIdProvider);

            var globalPackage = CreateOrAddPackage(new PackageInfo(MasterScope, "global"));
            GlobalScope = globalPackage.InnerScope;

            //GlobalScope = new Scope(ScopeIdProvider, "global");                      
        }

        private Dictionary<DefinitionInfo, IDefinition<DefinitionInfo>> Definitions { get; } = new Dictionary<DefinitionInfo, IDefinition<DefinitionInfo>>();        
        public Package CreateOrAddPackage(PackageInfo packageInfo)
        {
            return GetDefinitionFromInfo<Package>(packageInfo)
                ?? AddDefinition(new Package(packageInfo, new ContextOrientedScopeIdProvider(ScopeIdProvider, "pkg")));
        }

        public DataType CreateDataType(DataTypeInfo datatypeInfo)
        {            
            if (datatypeInfo.IsPrimitive)
                return AddDefinition(new PrimitiveDataType(datatypeInfo));

            throw new NotImplementedException();
        }

        public Variable CreateVariable(VariableInfo variableInfo) 
        {
            var varType = GetDefinitionFromInfo<DataType>(variableInfo.VariableType);
            if (varType == null)
                throw new ArgumentException($"Could not find type {variableInfo.VariableType}");
            return AddDefinition(new Variable(variableInfo, varType));
        }

        public IEnumerable<IDefinition<DefinitionInfo>> FindDefinitions() => Definitions.Values;
        public IEnumerable<IDefinition<DefinitionInfo>> FindDefinitions(Scope scope, bool rightUnderScope)
        {
            if (scope == null) return Enumerable.Empty<IDefinition<DefinitionInfo>>();
            if (rightUnderScope)
                return Definitions.Values.Where(_ => _.Scope == scope);
            var accessibleScopes = scope.GetAccessibleScopes().ToArray();
            return Definitions.Values.Where(_ => accessibleScopes.Contains(_.Scope));
        }

        public IEnumerable<IDefinition<DefinitionInfo>> FindDefinitions(Scope scope, string name, bool rightUnderScope)
        {
            if (scope == null) return Enumerable.Empty<IDefinition<DefinitionInfo>>();
            if (rightUnderScope)
                return Definitions.Values.Where(_ => _.Scope == scope && _.Name == name);
            var accessibleScopes = scope.GetAccessibleScopes().ToArray();
            return Definitions.Values.Where(_ => accessibleScopes.Contains(_.Scope) && _.Name == name);
        }

        public IEnumerable<D> FindDefinitions<D>(Scope scope, string name, bool rightUnderScope) where D : class, IDefinition<DefinitionInfo>
            => FindDefinitions(scope, name, rightUnderScope).Where(_ => _ is D).Select(_ => _ as D);

        public IDefinition<DefinitionInfo> FindDefinition(Scope scope, string name)
            => FindDefinitions(scope, name, rightUnderScope: true).FirstOrDefault();        

        public IDefinition<DefinitionInfo> FindDefinition(Scope scope, string name, Func<IDefinition<DefinitionInfo>, bool> predicate)
            => FindDefinitions(scope, name, rightUnderScope: true).Where(predicate).FirstOrDefault();

        public IDefinition<DefinitionInfo> FindDefinition<D>(Scope scope, string name, Func<D, bool> predicate) where D:class, IDefinition<DefinitionInfo>
            => FindDefinitions<D>(scope, name, rightUnderScope: true).Where(predicate).FirstOrDefault();

        private D GetDefinitionFromInfo<D>(DefinitionInfo info) where D : class, IDefinition<DefinitionInfo>
            => Definitions.ContainsKey(info) ? Definitions[info] as D : null;
        private D AddDefinition<D>(D definition) where D : IDefinition<DefinitionInfo>
        {
            var info = definition.DefinitionInfo;
            if (Definitions.ContainsKey(info))
                throw new InvalidOperationException($"Duplicate definition of {info.Name} in scope '{info.Scope.FullName}'");
            

            if (GetNamesInScope(info.Scope).Contains(info.Name))
                throw new InvalidOperationException($"Duplicate name '{info.Name}' in scope '{info.Scope.FullName}'");

            Definitions.Add(definition.DefinitionInfo, definition);
            Debug.WriteLine($"DEF ADDED!!! {definition}");
            return definition;
        }

        private IEnumerable<string> GetNamesInScope(Scope scope)
            => from def in Definitions.Values
               where def.DefinitionInfo.Scope == scope
               select def.Name;
    }
}

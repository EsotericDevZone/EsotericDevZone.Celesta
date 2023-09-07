using EsotericDevZone.Celesta.Extensions;
using EsotericDevZone.Celesta.Language.AST;
using EsotericDevZone.Celesta.Language.AST.Nodes;
using EsotericDevZone.Celesta.Language.Parser.ParseTree;
using EsotericDevZone.Celesta.Language.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;


namespace EsotericDevZone.Celesta.Language.Builder
{
    using BuildMethod = Func<IParseTreeNode, BuildContext, Scope, IASTNode>;
    public abstract class ASTBuilder
    {        
        private Dictionary<Type, BuildMethod> BuildMethods = new Dictionary<Type, BuildMethod>();

        public IASTNode Build(IParseTreeNode parseTreeNode, BuildContext bc, Scope scope)
        {
            if (parseTreeNode == null)
                throw new ArgumentNullException(nameof(parseTreeNode));
            var parseTreeNodeType = parseTreeNode.GetType();
            if (!BuildMethods.ContainsKey(parseTreeNodeType))
                throw new ArgumentException($"Unknown method to construct AST node from {parseTreeNodeType}");
            return BuildMethods[parseTreeNodeType].Invoke(parseTreeNode, bc, scope);
        }

        public ASTBuilder()
        {
            RegisterBuildMethods();
        }


        private void RegisterBuildMethods()
        {
            (from method in GetType().GetRuntimeMethods()
             where IsBuildMethod(method) && method.DeclaringType != typeof(ASTBuilder)
             select method)
            .ForEach(RegisterMethod);
        }

        private void RegisterMethod(MethodInfo method)
        {
            var parseTreeNodeType = method.GetParameters()[0].ParameterType;
            BuildMethods[parseTreeNodeType] = (ptn, bc, s) => method.Invoke(this, new object[] { ptn, bc, s }) as IASTNode;
        }

        private static bool IsBuildMethod(MethodInfo method)
        {
            if (!typeof(IASTNode).IsAssignableFrom(method.ReturnType))
                return false;
            if (method.GetParameters().Count() != 3) return false;
            if (!typeof(IParseTreeNode).IsAssignableFrom(method.GetParameters()[0].ParameterType))
                return false;
            if (!typeof(BuildContext).IsAssignableFrom(method.GetParameters()[1].ParameterType))
                return false;
            if (!typeof(Scope).IsAssignableFrom(method.GetParameters()[2].ParameterType))
                return false;
            return true;
        }

    }    
}

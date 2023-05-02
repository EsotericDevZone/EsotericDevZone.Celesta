using EsotericDevZone.Core;
using System;

namespace EsotericDevZone.Celesta.AST
{
    public abstract class AbstractASTNode : IASTNode
    {
        public IASTNode Parent { get; internal set; }

        protected AbstractASTNode(IASTNode parent)
        {
            Parent = parent;
        }

        public bool IsDirectlyIncludedIn(Type nodeType)
        {
            CheckTypeIsIASTNode(nodeType);
            return Parent?.GetType() == nodeType;
        }

        public bool IsIncludedIn(Type nodeType)
        {
            CheckTypeIsIASTNode(nodeType);
            return IsDirectlyIncludedIn(nodeType) || (Parent?.IsIncludedIn(nodeType) ?? false);      
        }

        protected static void CheckTypeIsIASTNode(Type nodeType)
        {
            Validation.ThrowIf(!nodeType.ImplementsInterface(typeof(IASTNode))
                            , new ArgumentException("Node Type must be IASTNode derived"));
        }

        public virtual string GetScopeName() => GetScope(Parent);

        public virtual string GetPackageName() => GetPackage(Parent);        

        internal static string GetScope(IASTNode node)
        {
            return node?.GetScopeName() ?? "@main";
        }

        internal static string GetPackage(IASTNode node)
        {
            return node?.GetPackageName() ?? "";
        }


        public T GetClosestParent<T>() where T : class, IASTNode
        {            
            return GetClosestParentRec<T>();            
        }

        private T GetClosestParentRec<T>() where T : class, IASTNode
        {
            if (Parent == null) return null;
            if (Parent is T result)
                return result;
            return Parent.GetClosestParent<T>();
        }
    }
}

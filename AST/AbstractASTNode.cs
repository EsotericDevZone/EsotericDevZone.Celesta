using EsotericDevZone.Core;
using EsotericDevZone.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            if (Parent == null) return null;
            if (Parent is T result)
                return result;
            return Parent.GetClosestParent<T>();
        }        

        public static IEnumerable<IASTNode> GetImmediateChildren(IASTNode node)
        {
            var nodeType = node.GetType();            

            var nodeProps = nodeType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>p.Name != "Parent" && typeof(IASTNode).IsAssignableFrom(p.PropertyType))
                .ToArray();
            var nodeArrayProps = nodeType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IASTNode[]).IsAssignableFrom(p.PropertyType)).ToArray();
            var nodeListProps = nodeType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(List<IASTNode>).IsAssignableFrom(p.PropertyType)).ToArray();

            //Console.WriteLine("Type = " + node.GetType());
            //Console.WriteLine("Props = " + nodeArrayProps.JoinToString(","));

            var children = new List<IASTNode>();

            children.AddRange(nodeProps
                .Select(p => p.GetValue(node) as IASTNode)
                .Where(_ => _ != null));
            children.AddRange(nodeArrayProps
                .Select(p => p.GetValue(node) as IASTNode[])
                .Flatten()
                .Where(_ => _ != null));
            children.AddRange(nodeListProps
                .Select(p => p.GetValue(node) as List<IASTNode>)
                .Flatten()
                .Where(_ => _ != null));
            
            return children;
        }

        public static IEnumerable<IASTNode> GetAllNodes(IASTNode node)
        {
            var children = GetImmediateChildren(node);
            foreach(var child in children)
            {
                yield return child;
                foreach (var nd in GetAllNodes(child))
                    yield return nd;
            }
            yield break;
        }

        public static List<IASTNode> SelectNodes(IASTNode node, Func<IASTNode, bool> pred)
        {
            return GetAllNodes(node).Where(pred).ToList();
        }

        public static List<T> SelectNodes<T>(IASTNode node, Func<T, bool> pred = null) where T : class, IASTNode
        {
            if (pred == null)
                return GetAllNodes(node).Where(_ => _ is T).Select(_ => _ as T).ToList();
            return GetAllNodes(node).Where(_ => _ is T).Select(_ => _ as T).Where(pred).ToList();
        }

        public static void ForeachNode<T>(IASTNode node, Action<T> action) where T : class, IASTNode
        {
            SelectNodes<T>(node).ForEach(action);
        }

        public static void AssertAllNodes<T>(IASTNode node, Func<T, bool> pred, Action<T> errorCallback) where T : class, IASTNode
        {
            SelectNodes<T>(node).ForEach(n =>
            {
                if (!pred(n))
                    errorCallback(n);
            });
        }        

        public static List<T> SelectAllNonNestedInTheSameType<P,T>(P node) 
            where P : class, IASTNode
            where T : class, IASTNode
        {
            return SelectNodes<T>(node, nd => nd.GetClosestParent<P>() == node);
        }

        public static void AssertAllNonNestedInType<P, T>(IASTNode node, Func<P,T, bool> pred, Action<P,T> errorCallback)
            where P : class, IASTNode
            where T : class, IASTNode
        {
            ForeachNode<P>(node, parent =>
            {
                SelectAllNonNestedInTheSameType<P, T>(parent).ForEach(n =>
                {
                    if (!pred(parent, n))
                        errorCallback(parent, n);
                });
            });                        
        }

        public static void AssertAllNonNestedInTheSameType<P,T>(P node, Func<T, bool> pred, Action<T> errorCallback) 
            where P : class, IASTNode
            where T : class, IASTNode
        {
            SelectAllNonNestedInTheSameType<P,T>(node).ForEach(n =>
            {
                if (!pred(n))
                    errorCallback(n);
            });
        }
    }
}

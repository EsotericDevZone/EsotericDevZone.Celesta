using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.AST
{
    public class BlockNode : AbstractASTNode, IBlockNode
    {
        internal List<IASTNode> Children { get; }
        public IASTNode[] GetChildren() => Children.ToArray();
        public BlockNode(IASTNode parent) : base(parent) { }
        internal void AddChild(IASTNode node)
        {
            (node as AbstractASTNode).Parent = this;
            Children.Add(node);
        }
    }
}

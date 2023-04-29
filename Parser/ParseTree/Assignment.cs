using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class Assignment : IParseTreeNode
    {
        public IParseTreeNode LeftHandSide { get; }
        public IParseTreeNode RightHandSide { get; }

        public Assignment(IParseTreeNode leftHandSide, IParseTreeNode rightHandSide)
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }

        public override string ToString() => $"{LeftHandSide} = {RightHandSide}";
    }
}

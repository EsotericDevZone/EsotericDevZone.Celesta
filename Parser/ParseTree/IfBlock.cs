using EsotericDevZone.Core;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class IfBlock : IParseTreeNode
    {
        public IParseTreeNode Condition { get; }

        public Block ThenBranch { get; }
        public Block ElseBranch { get; }

        public IfBlock(IParseTreeNode condition, Block thenBranch, Block elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public IfBlock(IParseTreeNode condition, Block thenBranch) : this(condition, thenBranch, null) { }


        public override string ToString()
        {
            var str = $"if {Condition} then\n";
            if(ThenBranch!=null)
            {                
                str += ThenBranch.ToString().Indent("    ") + "\n";
                if (ElseBranch != null)
                {
                    str += $"else\n{ElseBranch.ToString().Indent("    ")}\n";                    
                }
            }
            str += "endif";
            return str;

        }                        
    }
}

using EsotericDevZone.RuleBasedParser;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class BinaryOperator : IParseTreeNode
    {
        public Token OperatorToken { get; }

        public IParseTreeNode FirstMember { get; }
        public IParseTreeNode SecondMember { get; }

        public BinaryOperator(Token operatorToken, IParseTreeNode firstMember, IParseTreeNode secondMember)
        {
            OperatorToken = operatorToken;
            FirstMember = firstMember;
            SecondMember = secondMember;
        }

        public override string ToString() => $"({FirstMember} {OperatorToken.Value} {SecondMember})";
    }
}

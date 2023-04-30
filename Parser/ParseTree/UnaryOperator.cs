using EsotericDevZone.RuleBasedParser;

namespace EsotericDevZone.Celesta.Parser.ParseTree
{
    internal class UnaryOperator : IParseTreeNode
    {
        public Token OperatorToken { get; }
        public IParseTreeNode Expression { get; }

        public UnaryOperator(Token operatorToken, IParseTreeNode expression)
        {
            OperatorToken = operatorToken;
            Expression = expression;
        }

        public override string ToString() => $"u:{OperatorToken.Value}{Expression}";
    }
}

using EsotericDevZone.Celesta.Tokenizer;
using System.Diagnostics;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class SingleTokenRegex : AbstractPattern
    {
        public string Regex { get; }
        public string PatternName { get; set; }


        public SingleTokenRegex SetPatternName(string name)
        {
            PatternName = name;
            return this;
        }
        public SingleTokenRegex(string regex)
        {
            Regex = regex;
        }

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            if (parsePosition.IsOutOfRange)
                return errorCollector.Add(PatternMatchResult.Failed(parsePosition).SetErrorMessage("Unexpected end of input"));

            var newPosition = parsePosition.ConsumeToken(out Token token);

            if (!System.Text.RegularExpressions.Regex.IsMatch(token.Text, Regex))
                return errorCollector.Add(PatternMatchResult.Failed(parsePosition)
                    .SetErrorMessage(PatternName != null
                    ? $"'{token.Text}' is not a {PatternName}"
                    : $"'{token.Text}' does not satisfy regex {Regex}"));

            return PatternMatchResult.Succeeded(newPosition, token).SetLabel(Label);
        }
    }
}

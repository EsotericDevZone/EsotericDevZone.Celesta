using EsotericDevZone.Celesta.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public sealed class Literal : AbstractPattern
    {
        public string Value { get; set; }
        public Literal(string value)
        {
            Value = value;
        }

        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            if (parsePosition.IsOutOfRange)
                return errorCollector.Add(PatternMatchResult.Failed(parsePosition).SetErrorMessage("Unexpected end of input"));

            var newPosition = parsePosition.ConsumeToken(out Token token);

            if (token.Text != Value)
                return errorCollector.Add(PatternMatchResult.Failed(parsePosition)
                    .SetErrorMessage($"Expected '{Value}', got '{token.Text}' instead"));

            return PatternMatchResult.Succeeded(newPosition, token).SetLabel(Label);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public class Nothing : AbstractPattern
    {
        public override PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector)
        {
            return PatternMatchResult.Succeeded<object>(parsePosition, null);
        }
    }
}

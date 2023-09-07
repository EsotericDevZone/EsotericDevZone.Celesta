using System;
using System.Collections.Generic;
using System.Text;

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public abstract class AbstractPattern : IPattern
    {
        public string Label { get; set; }

        public abstract PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector);

        public IPattern SetLabel(string label)
        {
            Label = label;
            return this;
        }
    }
}

namespace EsotericDevZone.Celesta.Parser.Patterns
{
    public interface IPattern
    {
        string Label { get; set; }

        IPattern SetLabel(string label);

        PatternMatchResult Parse(ParsePosition parsePosition, ParseErrorCollector errorCollector);
    }
}

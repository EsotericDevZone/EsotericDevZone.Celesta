using EsotericDevZone.Celesta.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsotericDevZone.Celesta.Tokenizer
{
    public class Tokenizer
    {
        private static List<string> SplitBreakingRules = new List<string>
        { "'", "\"" };

        private static List<string> SplitAtoms = new List<string>
        { @"\+\+", @"\-\-", @"\+", @"\-", @"\*", @"\/", @"\%", @"\=", @"\;", @"\:", @"\(", @"\)", @"\[", @"\]", @"\.", @"\,",
          @"\<\=", @"\>\=", @"\<", @"\>", @"\=\=", @"\!\=", @"\<\>",
          @"\&", @"\|", @"\&\&", @"\|\|", @"\?", @"\^",
          @"\+\=", @"\-\=", @"\*\=", @"\/\=", @"\%\=", @"\&\=", @"\|\=", @"\^\=", @"\<\<\=", @"\>\>\=",
          @"\<\<", @"\>\>", @"((?<!\w)\d+\.\d*(?!\w))",
        
        }.OrderByDescending(_ => _.Length).Distinct().ToList();

        TokenSplitOptions TokenSplitOptions = new TokenSplitOptions(SplitBreakingRules, SplitAtoms);

        CommentStyle CommentStyle = new CommentStyle(new List<string> { "//" }, new List<(string, string)> { ("/*", "*/") });

        public IEnumerable<Token> FindUnsplittableStrings(string input)
           => Regex.Matches(input, RegexHelper.GetSplitBreakingRegexStringsOnly(TokenSplitOptions.SplitBreakingRules))
                .Cast<Match>()
                .Select(m => new Token(m.Value, m.Index, input.GetLineAndColumn(m.Index)));

        public List<Token> FindComments(string input)
        {
            var blocks = CommentStyle.BlockComments.Length == 0 ? new List<Match>() : Regex
                .Matches(input, RegexHelper.GetBlockCommentsRegex(CommentStyle.BlockComments), RegexOptions.Singleline)
                .Cast<Match>();
            var inlines = CommentStyle.InlineComments.Length == 0 ? new List<Match>() : Regex
                .Matches(input, RegexHelper.GetInlineCommentsRegex(CommentStyle.InlineComments), RegexOptions.Multiline)
                .Cast<Match>()
                .Where(_ => !blocks.Any(b => _.Index.IsBetween(b.Index, b.Index + b.Length))); // remove inline comments that start inside blocks


            var comments = blocks.Concat(inlines)
                .Select(m => new Token(m.Value, m.Index, input.GetLineAndColumn(m.Index)))
                .OrderBy(_ => _.Position)
                .ToList();

            var strings = FindUnsplittableStrings(input);
            // remove strings completely included in comments
            strings = strings.Where(s => comments
                .Any(c => c.IncludesIndex(s.Position) && c.IncludesIndex(s.Position + s.Length - 1)));          

            var _commentsCpy = comments.ToArray();
            // remove comments included in other comments
            comments = comments.Where(inner => !_commentsCpy
                .Any(outer => outer.Position < inner.Position && inner.Position + inner.Length <= outer.Position + outer.Length))
                .ToList();

            // remove comments that start inside strings to obtain true comments
            comments = comments.Where(c => !strings.Any(str => str.IncludesIndex(c.Position)))
                .OrderBy(c => c.Position)
                .ToList();

            return comments;
        }

        public List<Token> TokenizeBetweenTokens(string input, List<Token> tokens)
        {
            List<Token> result = new List<Token>();
            int startIndex = 0;
            foreach(var token in tokens)
            {
                if(startIndex == token.Position)
                {
                    startIndex += token.Length;
                    continue;
                }
                var substr = input.Substring(startIndex, token.Position - startIndex);
                result.Add(new Token(substr, startIndex, input.GetLineAndColumn(startIndex)));
                startIndex = token.Position + token.Length;
            }
            if(startIndex<input.Length)
            {
                var substr = input.Substring(startIndex);
                result.Add(new Token(substr, startIndex, input.GetLineAndColumn(startIndex)));
            }
            return result;
        }

        public List<Token> SplitToTokens(string input)
        {
            List<Token> comments = FindComments(input);

            var splitBreakingRegex = RegexHelper.GetSplitBreakingRegex(TokenSplitOptions.SplitBreakingRules);            

            var tokens = TokenizeBetweenTokens(input, comments)
                // remove spaces and isolate "strings" from the rest
                .Select(token =>
                    Regex.Matches(token.Text, splitBreakingRegex).Cast<Match>()
                        .Select(m => new Token(m.Value, token.Position + m.Index, input.GetLineAndColumn(token.Position + m.Index)))
                ).SelectMany(_ => _)
                // split by atoms
                .Select(token =>
                {
                    if (TokenSplitOptions.Atoms.GetAtomsSplitRegex() == "") 
                        return new List<Token> { token };
                    return token.Text.IsSplitBreakingString(TokenSplitOptions.SplitBreakingRules)
                        ? new List<Token> { token }
                        : token.Text.RemoveRedundantWhitespace()
                            .Select(m => new Token(m.Value, token.Position + m.Index, input.GetLineAndColumn(token.Position + m.Index)))
                            .Select(tk => Regex.Split(tk.Text, TokenSplitOptions.Atoms.GetAtomsSplitRegex())
                            .ToTokens(input, tk.Position)).SelectMany(_ => _);
                }).SelectMany(_ => _)
                // cleanup
                .Where(token => !string.IsNullOrWhiteSpace(token.Text))
                .Peek(token => token.Type = TokenTypes.Code)                
                .ToList();
            var list = comments.Peek(_ => _.Type = TokenTypes.Comment).Union(tokens).OrderBy(_ => _.Position)
                .ToList();

            for (int i = 0; i < list.Count; i++)
                list[i] = list[i].Reposition(new Parser.ParsePosition(list, i));

            return list;
        }
    }
}

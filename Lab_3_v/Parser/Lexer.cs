using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    enum TokenType
    {
        Identifier,
        Whitespaces,
        Punctuation,
        String,
        Number,
        Comment,
        End,
    }
    class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public Token(TokenType type, string lexeme)
        {
            Type = type;
            Lexeme = lexeme;
        }
        public override string ToString()
        {
            return Type + @":" + Lexeme;
        }
    }
    class Lexer
    {
        public static IEnumerable<Token> GetTokens(string sourceText)
        {
            var lexemeRegex = new Regex(@"  
                (?<String>@""(""""|[^""])*"")|
                (?<Identifier>[a-zA-Z_][a-zA-Z_0-9]*)|
                (?<Punctuation>(\=\>)|(\!\=)|(\+\+)|[\[\]\*/(){}<>.,;:\-\+\=])|
                (?<Number>[0-9]+)|
                (?<Comment>(/\*(.|\n)*?\*/))|
                (?<Whitespaces>[\ \t\n\r]+)",
                RegexOptions.IgnorePatternWhitespace);
            int position = 0;
            while (position < sourceText.Length)
            {
                var match = lexemeRegex.Match(sourceText, position);
                if (match.Index != position)
                {
                    throw new Exception(@"Regular expression does not describe all types of tokens that are contained in the input string");
                }
                position = match.Index + match.Length;
                if (match.Groups[@"Identifier"].Success)
                {
                    yield return new Token(TokenType.Identifier, match.Value);
                }
                else if (match.Groups[@"Whitespaces"].Success)
                {
                    yield return new Token(TokenType.Whitespaces, match.Value);
                }
                else if (match.Groups[@"Punctuation"].Success)
                {
                    yield return new Token(TokenType.Punctuation, match.Value);
                }
                else if (match.Groups[@"String"].Success)
                {
                    yield return new Token(TokenType.String, match.Value);
                }
                else if (match.Groups[@"Number"].Success)
                {
                    yield return new Token(TokenType.Number, match.Value);
                }
                else if (match.Groups[@"Comment"].Success)
                {
                    yield return new Token(TokenType.Comment, match.Value);
                }
                else
                {
                    throw new Exception(@"if does not exist for this group");
                }
            }
        }
    }
}

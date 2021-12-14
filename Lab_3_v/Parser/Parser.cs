using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Parser
    {
        IReadOnlyList<Token> Tokens;
        int position = 0;
        public Parser(string source)
        {
            Tokens = GetTokens(source);
        }
        IReadOnlyList<Token> GetTokens(string source) => FixTokens(Lexer.GetTokens(source)).ToList();
        public static IEnumerable<Token> FixTokens(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.Type != TokenType.Whitespaces && token.Type != TokenType.Comment)
                {
                    yield return token;
                }
            }
            yield return new Token(TokenType.End, "");
        }
        public InsertStmt ParseInsertStmt()
        {
            Expect("INSERT");
            Expect("INTO");
            var tableName = ParseIdentifier();
            var columnNames = ParseColumnNames();
            Expect("VALUES");
            var values = ParseValues();
            if (!CurrentIsEnd())
            {
                throw new Exception("Не все токены были обработаны парсером");
            }
            return new InsertStmt(tableName, columnNames.ToList(), values);
        }
        List<Identifier> ParseColumnNames()
        {
            var columnNames = new List<Identifier>();
            Expect("(");
            while (true)
            {
                columnNames.Add(ParseIdentifier());
                if (!SkipIf(","))
                {
                    break;
                }
            }
            Expect(")");
            return columnNames;
        }
        List<List<IExpression>> ParseValues()
        {
            var values = new List<List<IExpression>>();
            while (true)
            {
                var items = new List<IExpression>();
                Expect("(");
                while (true)
                {
                    items.Add(ParseAddSub());
                    if (!SkipIf(","))
                    {
                        break;
                    }
                }
                Expect(")");
                values.Add(items);
                if (!SkipIf(","))
                {
                    break;
                }
            }
            return values;
        }
        IExpression ParseAddSub()
        {
            var left = ParseMultDiv();
            while (true)
            {
                BinaryOpType binaryOperator;
                if (SkipIf("+"))
                {
                    binaryOperator = BinaryOpType.Addition;
                }
                else if (SkipIf("-"))
                {
                    binaryOperator = BinaryOpType.Subtraction;
                }
                else
                {
                    break;
                }
                var right = ParseAddSub();
                left = new BinaryOperation(left, binaryOperator, right);
            }
            return left;
        }
        IExpression ParseMultDiv()
        {
            var left = ParsePrimary();
            while (true)
            {
                BinaryOpType binaryOperator;
                if (SkipIf("*"))
                {
                    binaryOperator = BinaryOpType.Multiplication;
                }
                else if (SkipIf("/"))
                {
                    binaryOperator = BinaryOpType.Division;
                }
                else
                {
                    break;
                }
                var right = ParseMultDiv();
                left = new BinaryOperation(left, binaryOperator, right);
            }
            return left;
        }
        IExpression ParsePrimary()
        {
            if (GetCurrentToken().Type == TokenType.Number)
            {
                return ParseNumber();
            }
            if (GetCurrentToken().Type == TokenType.Identifier)
            {
                return ParseIdentifier();
            }
            return ParseParentheses();
        }
        Parentheses ParseParentheses()
        {
            Expect("(");
            var child = ParseAddSub();
            Expect(")");
            return new Parentheses(child);
        }
        Identifier ParseIdentifier()
        {
            if (GetCurrentToken().Type == TokenType.Identifier)
            {
                Skip();
                return new Identifier(Tokens[position - 1].Lexeme);
            }
            throw new Exception($"Ожидался токен: {TokenType.Identifier}, а был получен: {GetCurrentToken().Type}");
        }
        Number ParseNumber()
        {
            if (GetCurrentToken().Type == TokenType.Number)
            {
                Skip();
                return new Number(Tokens[position - 1].Lexeme);
            }
            throw new Exception($"Ожидался токен: {TokenType.Number}, а был получен: {GetCurrentToken().Type}");
        }
        void Skip() => position++;
        Token GetCurrentToken() => Tokens[position];
        void Expect(string lexeme)
        {
            if (!SkipIf(lexeme))
            {
                throw new Exception($"Ожидалась лексема: {lexeme}, а была получена: {GetCurrentToken().Lexeme}");
            }
        }
        bool SkipIf(string lexeme)
        {
            if (!CurrentIsEnd() && CurrentIs(lexeme))
            {
                Skip();
                return true;
            }
            return false;
        }
        bool CurrentIs(string lexeme) => GetCurrentToken().Lexeme == lexeme;
        bool CurrentIsEnd() => GetCurrentToken().Type == TokenType.End;
    }
}

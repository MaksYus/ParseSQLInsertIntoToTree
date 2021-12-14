using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    static class TextWriterExtentions
    {
        public static void WriteIndent(this TextWriter o, int depth)
        {
            o.Write(new string(' ', depth * 2));
        }
    }
    interface INode
    {
        string ToFormattedString();
        void DebugPrint(TextWriter o, int depth);
    }
    interface IExpression : INode { }
    class Number : IExpression
    {
        public string Lexeme { get; }
        public Number(string lexeme)
        {
            Lexeme = lexeme;
        }
        public string ToFormattedString()
        {
            return $"{Lexeme}";
        }
        public void DebugPrint(TextWriter o, int depth)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Number)}(\"{Lexeme}\")");
        }
    }
    class Identifier : IExpression
    {
        public string Lexeme { get; }
        public Identifier(string lexeme)
        {
            Lexeme = lexeme;
        }
        public string ToFormattedString()
        {
            return $"{Lexeme}";
        }
        public void DebugPrint(TextWriter o, int depth)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Identifier)}(\"{Lexeme}\")");
        }
    }
    enum BinaryOpType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
    }
    class BinaryOperation : IExpression
    {
        public IExpression Left { get; }
        public BinaryOpType Operator { get; }
        public IExpression Right { get; }
        public BinaryOperation(IExpression left, BinaryOpType binaryOperator, IExpression right)
        {
            Left = left;
            Operator = binaryOperator;
            Right = right;
        }
        public string GetOperator(BinaryOpType binaryOperator)
        {
            switch (binaryOperator)
            {
                case BinaryOpType.Addition: return "+";
                case BinaryOpType.Subtraction: return "-";
                case BinaryOpType.Multiplication: return "*";
                case BinaryOpType.Division: return "/";
            }
            throw new InvalidOperationException();
        }
        public string ToFormattedString()
        {
            return $"{Left.ToFormattedString()} {GetOperator(Operator)} {Right.ToFormattedString()}";
        }
        public void DebugPrint(TextWriter o, int depth)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(BinaryOperation)}(\n");
            Left.DebugPrint(o, depth + 1);
            o.Write(",\n");
            o.WriteIndent(depth + 1);
            o.Write($"{nameof(BinaryOpType)}.{Operator}");
            o.Write(",\n");
            Right.DebugPrint(o, depth + 1);
            o.Write("\n");
            o.WriteIndent(depth);
            o.Write($")");
        }
    }
    class InsertStmt : INode
    {
        public Identifier TableName { get; }
        public IReadOnlyList<Identifier> ColumnNames { get; }
        public IReadOnlyList<IReadOnlyList<IExpression>> Values { get; }
        public InsertStmt(Identifier tableName, IReadOnlyList<Identifier> columnNames, IReadOnlyList<IReadOnlyList<IExpression>> values)
        {
            TableName = tableName;
            ColumnNames = columnNames;
            Values = values;
        }
        public string ToFormattedString()
        {
            return $"INSERT INTO {TableName.ToFormattedString()} " +
                $"({string.Join(", ", ColumnNames.Select(c => c.ToFormattedString()))})" +
                $" VALUES{string.Join(", ", Values.Select(v => $"({string.Join(", ", v.Select(vv => vv.ToFormattedString()))})"))}";
        }
        public void DebugPrint(TextWriter o, int depth)
        {
            o.Write($"new {nameof(InsertStmt)}(\n");
            TableName.DebugPrint(o, depth + 1);
            o.Write(",\n");
            o.WriteIndent(depth + 1);
            o.Write($"new List<{nameof(Identifier)}> {{\n");
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                if (i > 0)
                {
                    o.Write(",\n");
                }
                ColumnNames[i].DebugPrint(o, depth + 2);
            }
            o.Write("\n");
            o.WriteIndent(depth + 1);
            o.Write("},\n");
            o.WriteIndent(depth + 1);
            o.Write($"new List<List<{nameof(IExpression)}>> {{\n");
            for (int i = 0; i < Values.Count; i++)
            {
                o.WriteIndent(depth + 2);
                o.Write($"new List<{nameof(IExpression)}> {{\n");
                for (int j = 0; j < Values[i].Count; j++)
                {
                    Values[i][j].DebugPrint(o, depth + 3);
                    o.Write(j != Values[i].Count - 1 ? ",\n" : "\n");
                }
                o.WriteIndent(depth + 2);
                o.Write(i != Values.Count - 1 ? "},\n" : "}\n");
            }
            o.WriteIndent(depth + 1);
            o.Write("}\n");
            o.WriteIndent(depth);
            o.Write(");\n");
        }
    }
    static class NodeExtensions
    {
        public static string ToDebugString(this INode node)
        {
            using (var o = new StringWriter())
            {
                node.DebugPrint(o, 0);
                o.Write("\n");
                return o.ToString();
            }
        }
    }
    class Parentheses : IExpression
    {
        public IExpression Child { get; }
        public Parentheses(IExpression child)
        {
            Child = child;
        }
        public string ToFormattedString()
        {
            return $"({Child.ToFormattedString()})";
        }
        public void DebugPrint(TextWriter o, int depth)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Parentheses)}(\n");
            Child.DebugPrint(o, depth + 1);
            o.Write("\n");
            o.WriteIndent(depth);
            o.Write(")");
        }
    }
}

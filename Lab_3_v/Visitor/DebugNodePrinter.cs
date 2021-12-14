using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Visitor
{
    static class TextWriterExtentions
    {
        public static void WriteIndent(this TextWriter o, int depth)
        {
            o.Write(new string(' ', depth * 2));
        }
    }
    class DebugNodePrinter : INodeVisitor
    {
        readonly TextWriter o;
        int depth;
        public DebugNodePrinter(TextWriter o)
        {
            this.o = o;
        }
        void Print(INode node)
        {
            node.AcceptVisitor(this);
        }
        public void VisitInsertStmt(InsertStmt insertStmt)
        {
            o.Write($"new {nameof(InsertStmt)}(\n");
            depth++;
            Print(insertStmt.TableName);
            o.Write(",\n");
            o.WriteIndent(depth);
            depth--;
            o.Write($"new List<{nameof(Identifier)}> {{\n");
            for (int i = 0; i < insertStmt.ColumnNames.Count; i++)
            {
                if (i > 0)
                {
                    o.Write(",\n");
                }
                depth += 2;
                Print(insertStmt.ColumnNames[i]);
                depth -= 2;
            }
            o.Write("\n");
            o.WriteIndent(depth + 1);
            o.Write("},\n");
            o.WriteIndent(depth + 1);
            o.Write($"new List<List<{nameof(IExpression)}>> {{\n");
            for (int i = 0; i < insertStmt.Values.Count; i++)
            {
                o.WriteIndent(depth + 2);
                o.Write($"new List<{nameof(IExpression)}> {{\n");
                for (int j = 0; j < insertStmt.Values[i].Count; j++)
                {
                    depth += 3;
                    Print(insertStmt.Values[i][j]);
                    depth -= 3;
                    o.Write(j != insertStmt.Values[i].Count - 1 ? ",\n" : "\n");
                }
                o.WriteIndent(depth + 2);
                o.Write(i != insertStmt.Values.Count - 1 ? "},\n" : "}\n");
            }
            o.WriteIndent(depth + 1);
            o.Write("}\n");
            o.WriteIndent(depth);
            o.Write(");\n");
        }
        public void VisitBinaryOperation(BinaryOperation binaryOperation)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(BinaryOperation)}(\n");
            depth++;
            Print(binaryOperation.Left);
            o.Write(",\n");
            o.WriteIndent(depth);
            o.Write($"{nameof(BinaryOpType)}.{binaryOperation.Operator}");
            o.Write(",\n");
            Print(binaryOperation.Right);
            depth--;
            o.Write("\n");
            o.WriteIndent(depth);
            o.Write($")");
        }
        public void VisitParentheses(Parentheses parentheses)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Parentheses)}(\n");
            depth++;
            Print(parentheses.Child);
            depth--;
            o.Write("\n");
            o.WriteIndent(depth);
            o.Write(")");
        }
        public void VisitIdentifier(Identifier identifier)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Identifier)}(\"{identifier.Lexeme}\")");
        }
        public void VisitNumber(Number number)
        {
            o.WriteIndent(depth);
            o.Write($"new {nameof(Number)}(\"{number.Lexeme}\")");
        }
    }
}

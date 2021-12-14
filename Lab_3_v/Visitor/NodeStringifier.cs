using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    class NodeStringifier : INodeVisitor<string>
    {
        string ToString(INode node) => node.AcceptVisitor(this);
        public string VisitInsertStmt(InsertStmt insertStmt)
        {
            return $"INSERT INTO {ToString(insertStmt.TableName)} " +
               $"({string.Join(", ", insertStmt.ColumnNames.Select(c => ToString(c)))})" +
               $" VALUES{string.Join(", ", insertStmt.Values.Select(v => $"({string.Join(", ", v.Select(vv => ToString(vv)))})"))}";
        }
        public string VisitBinaryOperation(BinaryOperation binaryOperation)
        {
            return $"{ToString(binaryOperation.Left)} {GetOperator(binaryOperation.Operator)} {ToString(binaryOperation.Right)}";
        }
        public string VisitParentheses(Parentheses parentheses)
        {
            return $"({ToString(parentheses.Child)})";
        }
        public string VisitIdentifier(Identifier identifier)
        {
            return $"{identifier.Lexeme}";
        }
        public string VisitNumber(Number number)
        {
            return $"{number.Lexeme}";
        }
        string GetOperator(BinaryOpType binaryOperator)
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
    }
}

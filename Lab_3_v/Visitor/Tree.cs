using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    interface INode
    {
        T AcceptVisitor<T>(INodeVisitor<T> visitor);
        void AcceptVisitor(INodeVisitor visitor);
    }
    interface IExpression : INode { }
    class Number : IExpression
    {
        public string Lexeme { get; }
        public Number(string lexeme)
        {
            Lexeme = lexeme;
        }
        public T AcceptVisitor<T>(INodeVisitor<T> visitor)
        {
            return visitor.VisitNumber(this);
        }
        public void AcceptVisitor(INodeVisitor visitor)
        {
            visitor.VisitNumber(this);
        }
    }
    class Identifier : IExpression
    {
        public string Lexeme { get; }
        public Identifier(string lexeme)
        {
            Lexeme = lexeme;
        }
        public T AcceptVisitor<T>(INodeVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }
        public void AcceptVisitor(INodeVisitor visitor)
        {
            visitor.VisitIdentifier(this);
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
        public T AcceptVisitor<T>(INodeVisitor<T> visitor)
        {
            return visitor.VisitBinaryOperation(this);
        }
        public void AcceptVisitor(INodeVisitor visitor)
        {
            visitor.VisitBinaryOperation(this);
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
        public T AcceptVisitor<T>(INodeVisitor<T> visitor)
        {
            return visitor.VisitInsertStmt(this);
        }
        public void AcceptVisitor(INodeVisitor visitor)
        {
            visitor.VisitInsertStmt(this);
        }
    }
    class Parentheses : IExpression
    {
        public IExpression Child { get; }
        public Parentheses(IExpression child)
        {
            Child = child;
        }
        public T AcceptVisitor<T>(INodeVisitor<T> visitor)
        {
            return visitor.VisitParentheses(this);
        }
        public void AcceptVisitor(INodeVisitor visitor)
        {
            visitor.VisitParentheses(this);
        }
    }
}

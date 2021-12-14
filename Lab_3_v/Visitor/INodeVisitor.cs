using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    interface INodeVisitor<T>
    {
        T VisitInsertStmt(InsertStmt insertStmt);
        T VisitBinaryOperation(BinaryOperation binaryOperation);
        T VisitParentheses(Parentheses parentheses);
        T VisitNumber(Number number);
        T VisitIdentifier(Identifier identifier);

    }
    interface INodeVisitor
    {
        void VisitInsertStmt(InsertStmt insertStmt);
        void VisitBinaryOperation(BinaryOperation binaryOperation);
        void VisitParentheses(Parentheses parentheses);
        void VisitNumber(Number number);
        void VisitIdentifier(Identifier identifier);
    }
}

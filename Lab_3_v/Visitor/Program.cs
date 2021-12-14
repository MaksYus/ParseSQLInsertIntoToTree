using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = "INSERT INTO t1 (col1, col2) VALUES((1 + 2) * 3 + (5 - 6))";
            var tree = new Parser(source).ParseInsertStmt();
            Console.WriteLine(tree.AcceptVisitor(new NodeStringifier()));
            tree.AcceptVisitor(new DebugNodePrinter(Console.Out));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = "INSERT INTO t1 (col1, col2) VALUES((1 + 2) * 3, (1 - 4) / 5), (6, 7)";
            var tree = new Parser(source).ParseInsertStmt();
            Console.WriteLine(tree.ToFormattedString());
            Console.WriteLine(tree.ToDebugString());
            Console.ReadKey();
        }
    }
}

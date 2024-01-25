using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string Filepath = "C17_bench.txt";

            Parser Parser = new Parser();
            List<Wire> Wires  = Parser.ParseWires(Filepath);
            List<Assignment> Assignments = Parser.ParseAssigns(Filepath, Wires);
        }
    }
}

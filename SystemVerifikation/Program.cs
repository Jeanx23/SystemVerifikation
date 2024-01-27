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
            string Filepath = "test_bench.txt";

            Parser parser = new Parser();
            List<Wire> inputs  = parser.ParseInputs(Filepath);
            List<Wire> outputs = parser.ParseOutputs(Filepath);
            List<Wire> wires = parser.ParseWires(Filepath);
            List<Assignment> assignments = parser.ParseAssigns(Filepath);
            
            Simulation simulation = new Simulation(inputs, outputs, wires, assignments);
        }
    }
}

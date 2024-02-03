using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            if (args.Length > 0)
            {
                filePath = args[0];
            }
            else
            {
                Console.Write("Bitte geben Sie den Dateipfad ein: ");
                filePath = Console.ReadLine();         
            }

            using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath))
            {
                writer.WriteLine(filePath);
                writer.WriteLine();
                writer.WriteLine();
            }
            Parser parser = new Parser();
            List<Wire> inputs  = parser.ParseInputs(filePath);
            List<Wire> outputs = parser.ParseOutputs(filePath);
            List<Wire> wires = parser.ParseWires(filePath);
            List<Assignment> assignments = parser.ParseAssigns(filePath);
            
            Simulation simulation = new Simulation(inputs, outputs, wires, assignments);
        }
    }
}

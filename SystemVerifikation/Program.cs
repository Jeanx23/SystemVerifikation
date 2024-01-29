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

            // Überprüfen, ob Befehlszeilenargumente vorhanden sind
            if (args.Length > 0)
            {
                // Verwenden Sie das erste Befehlszeilenargument als Dateipfad
                filePath = args[0];
            }
            else
            {
                // Aufforderung zur Eingabe des Dateipfads, wenn kein Befehlszeilenargument vorhanden ist
                /*Console.Write("Bitte geben Sie den Dateipfad ein: ");
                filePath = Console.ReadLine();*/
                filePath = "C17_bench.txt";
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

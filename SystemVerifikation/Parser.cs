using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    public class Parser
    {
        public List<Wire> ParseWires(string Filepath)
        {
            List<Wire> Wires = new List<Wire>();
            bool IsParsing = false;

            try
            {
                using (StreamReader sr = new StreamReader(Filepath))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    { 
                        if (line.Contains("input"))
                        {
                            IsParsing = true;                            
                        }
                        else if (line.Contains("assign"))
                        {
                            IsParsing = false;
                            break;
                        }
                        if (IsParsing == true)
                        {
                            string[] words = line.Split(new char[] { ',',';' });
                            foreach (string word in words)
                            {
                                string trimmedWord = word.Trim();
                                if (!string.IsNullOrEmpty(trimmedWord))
                                {
                                    trimmedWord = trimmedWord.Replace("input", string.Empty).Replace("output", string.Empty).Replace("wire", string.Empty).Trim();
                                    Wires.Add(new Wire { Name = trimmedWord });
                                }
                            }
                        }                    
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return Wires;
        }

        // Hier muss für jede Zeile die Wires Liste durchsucht werden und ein Assign Element erstellt werden. Der Assign Konstruktor enthält die Eingangswires, die Ausgangswires und die logischen Operationen
        // string Ergebnis = Wires.Find(s => s == gesuchterString);
        public List<Assignment> ParseAssigns(string Filepath, List<Wire> Wires)
        {
            List<Assignment> parsedAssignments = new List<Assignment>();

            try
            {
                // Lese alle Zeilen aus der Textdatei
                string[] lines = File.ReadAllLines(Filepath);

                foreach (string line in lines)
                {
                    // Überprüfe, ob die Zeile mit "endmodule" erscheint und beende das Parsen
                    if (line.Trim().ToLower() == "endmodule")
                    {
                        break;
                    }

                    // Überprüfe, ob die Zeile mit "assign" beginnt
                    if (line.Trim().StartsWith("assign"))
                    {
                        // Extrahiere die relevanten Informationen
                        Assignment assignment = ParseAssignLine(line);

                        // Füge die geparste Zuweisung zur Liste hinzu
                        parsedAssignments.Add(assignment);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Lesen der Datei: " + ex.Message);
            }

            return parsedAssignments;
        }

        static Assignment ParseAssignLine(string line)
        {
            // Entferne "assign" und führende/trailing Leerzeichen
            string trimmedLine = line.Replace("assign", "").Trim();

            // Extrahiere die Operanden und den Operator
            Match match = Regex.Match(trimmedLine, @"(\S+)\s*=\s*(~?\S+)\s*(?:([&|])\s*(~?\S+))?");

            Assignment assignment = new Assignment();

            if (match.Success)
            {
                assignment.LeftOperand = match.Groups[1].Value;
                assignment.Operator = match.Groups[3].Success ? match.Groups[3].Value : null;

                // Prüfe, ob der rechte Operand negiert ist
                if (match.Groups[2].Value.StartsWith("~"))
                {
                    assignment.RightOperand1IsNegated = true;
                    assignment.RightOperand1 = match.Groups[2].Value.Substring(1); // Entferne das Negationszeichen "~"
                }
                else
                {
                    assignment.RightOperand1IsNegated = false;
                    assignment.RightOperand1 = match.Groups[2].Value;
                }

                // Prüfe, ob ein zweiter Operand vorhanden ist
                if (match.Groups[4].Success && !string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    // Prüfe, ob der zweite Operand negiert ist
                    if (match.Groups[4].Value.StartsWith("~"))
                    {
                        assignment.RightOperand2IsNegated = true;
                        assignment.RightOperand2 = match.Groups[4].Value.Substring(1); // Entferne das Negationszeichen "~"
                    }
                    else
                    {
                        assignment.RightOperand2IsNegated = false;
                        assignment.RightOperand2 = match.Groups[4].Value;
                    }
                }
            }

            return assignment;
        }

    }
}

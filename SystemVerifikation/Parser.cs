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
        public List<Wire> ParseInputs(string Filepath)
        {
            List<Wire> Inputs = new List<Wire>();
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
                        else if (line.Contains("output"))
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
                                    Inputs.Add(new Wire { Name = trimmedWord });
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
            return Inputs;
        }

        internal List<Wire> ParseOutputs(string Filepath)
        {
            List<Wire> Outputs = new List<Wire>();
            bool IsParsing = false;

            try
            {
                using (StreamReader sr = new StreamReader(Filepath))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("output"))
                        {
                            IsParsing = true;
                        }
                        else if (line.Contains("wire"))
                        {
                            IsParsing = false;
                            break;
                        }
                        if (IsParsing == true)
                        {
                            string[] words = line.Split(new char[] { ',', ';' });
                            foreach (string word in words)
                            {
                                string trimmedWord = word.Trim();
                                if (!string.IsNullOrEmpty(trimmedWord))
                                {
                                    trimmedWord = trimmedWord.Replace("input", string.Empty).Replace("output", string.Empty).Replace("wire", string.Empty).Trim();
                                    Outputs.Add(new Wire { Name = trimmedWord });
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
            return Outputs;
        }

        internal List<Wire> ParseWires(string Filepath)
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
                        if (line.Contains("wire"))
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
                            string[] words = line.Split(new char[] { ',', ';' });
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

        public List<Assignment> ParseAssigns(string Filepath)
        {
            List<Assignment> parsedAssignments = new List<Assignment>();
            try
            {
                string[] lines = File.ReadAllLines(Filepath);

                foreach (string line in lines)
                {
                    if (line.Trim().ToLower() == "endmodule")
                    {
                        break;
                    }
                    if (line.Trim().StartsWith("assign"))
                    {
                        Assignment assignment = ParseAssignLine(line);
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
            string trimmedLine = line.Replace("assign", "").Trim();
            Match match = Regex.Match(trimmedLine, @"(\S+)\s*=\s*(~?\S+)\s*(?:([&|])\s*(~?\S+))?");

            Assignment assignment = new Assignment();

            if (match.Success)
            {
                assignment.LeftOperand = match.Groups[1].Value;
                assignment.Operator = match.Groups[3].Success ? match.Groups[3].Value : null;

                // Check Negation
                if (match.Groups[2].Value.StartsWith("~"))
                {
                    assignment.RightOperand1IsNegated = true;
                    assignment.RightOperand1 = match.Groups[2].Value.Substring(1); 
                }
                else
                {
                    assignment.RightOperand1IsNegated = false;
                    assignment.RightOperand1 = match.Groups[2].Value;
                }

                // Check Negation
                if (match.Groups[4].Success && !string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    if (match.Groups[4].Value.StartsWith("~"))
                    {
                        assignment.RightOperand2IsNegated = true;
                        assignment.RightOperand2 = match.Groups[4].Value.Substring(1); 
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

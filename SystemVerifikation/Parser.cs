using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                            continue;
                        }
                        else if (line.Contains("assign"))
                        {
                            IsParsing = false;
                            break;
                        }
                        if (IsParsing == true)
                        {
                            string[] words = line.Split(new char[] { ',' });
                            foreach (string word in words)
                            {
                                string trimmedWord = word.Trim();
                                if (!string.IsNullOrEmpty(trimmedWord))
                                {
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
        public List<Assign> ParseAssigns(string Filepath, List<Wire> Wires) 
        {  
            List<Assign> Assigns = new List<Assign>();
            bool IsParsing = false;

            try
            {
                using(StreamReader sr = new StreamReader(Filepath)) 
                {
                    string line;

                    while((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("assign"))
                        {
                            IsParsing = true;
                            continue;
                        }
                        else if (line.Contains("endmodule"))
                        {
                            IsParsing = false;
                            continue;
                        }
                        if (IsParsing == true)
                        {
                            // Hier muss für jede Zeile die Wires Liste durchsucht werden und ein Assign Element erstellt werden. Der Assign Konstruktor enthält die Eingangswires, die Ausgangswires und die logischen Operationen
                            string Ergebnis = Wires.Find(s => s == gesuchterString);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            throw new NotImplementedException();
        }
    }
}

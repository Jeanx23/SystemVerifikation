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
    }
}

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SystemVerifikation
{
    public class Simulation
    {
        public List<Wire> Inputs { get; set; }
        public List<Wire> Outputs { get; set; }
        public List<Wire> Wires { get; set; }
        public List<Assignment> Assignments { get; set; }
        public Dictionary<Assignment, List<Assignment>> adjList;

        public Simulation(List<Wire> inputs, List<Wire> outputs, List<Wire> wires, List<Assignment> assignments)
        {
            Inputs = inputs ?? throw new ArgumentNullException(nameof(inputs), "Inputs list cannot be null");
            Outputs = outputs ?? throw new ArgumentNullException(nameof(outputs), "Outputs list cannot be null");
            Wires = wires ?? throw new ArgumentNullException(nameof(wires), "Wires list cannot be null");
            Assignments = assignments ?? throw new ArgumentNullException(nameof(assignments), "Assignments list cannot be null");
            

            foreach (var assignment in Assignments)
            {
                assignment.ParentSimulation = this;
            }
            var ResultsGoldenCircuit = RunGoldenCircuit();
            PrintResults(ResultsGoldenCircuit);
            var ResultsBadCircuit = RunBadCircuit();           

            RunCompareCircuit(ResultsGoldenCircuit,ResultsBadCircuit);
            Console.ReadLine();
        }

        public void PrintResults(List<List<KeyValuePair<String, bool>>> Results)
        {
            int numInputs = Inputs.Count;
            int x = 1; 
            int i = 0;
            foreach (var Result in Results)
            {
                Console.WriteLine("Inputs for Result" + " " + x + ":");
                for (int j = 0; j < numInputs; ++j)
                {
                    bool inputValue = ((i >> j) & 1) == 1;
                    Console.WriteLine(inputValue);
                }
                Console.WriteLine("Result" + " " + x + ":");
                foreach (var Item in Result)
                {
                    Console.WriteLine(Item);
                }
                Console.WriteLine();
                Console.WriteLine();
                x++;
                i++;
            }
        }
        private void RunCompareCircuit(List<List<KeyValuePair<String, bool>>> GoldenCircuitResults, List<KeyValuePair<String, List<List<KeyValuePair<String, bool>>>>> BadCircuitResults)
        {
            int i = 0;
            List<Wire> FaultableWires = GiveInputsAndWires();

            while (i + 1 < BadCircuitResults.Count) // Überprüfe auf Indexüberlauf
            {
                Wire wire = FaultableWires[i / 2];
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Test of wire: " + wire.Name);
                Console.ResetColor();
                Console.WriteLine();
                using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
                {
                    writer.WriteLine("Test of wire: " + wire.Name);
                }
                
                AreResultsEqual(GoldenCircuitResults, BadCircuitResults[i]);                
                AreResultsEqual(GoldenCircuitResults, BadCircuitResults[i + 1]);
                           
                i += 2;
            }
        }
        public void AreResultsEqual(List<List<KeyValuePair<string, bool>>> goldenCircuitResults, KeyValuePair<string, List<List<KeyValuePair<string, bool>>>> badResults)
        {
            Console.WriteLine("Testing of: " + badResults.Key);
            using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
            {
                writer.WriteLine();
                writer.WriteLine("Testing of: " + badResults.Key);
                writer.WriteLine();
            }
            if (goldenCircuitResults.Count != badResults.Value.Count)
            {
                Console.WriteLine("Computing Error: Count of Golden Circuit Outputs != Count of Bad Circuit Outputs");
            }

            for (int j = 0; j < goldenCircuitResults.Count; j++)
            {
                var goldenResultsList = goldenCircuitResults[j];
                var badResultList = badResults.Value[j];

                if (!goldenResultsList.SequenceEqual(badResultList, KeyValuePairComparer.Instance))
                {
                    Console.WriteLine("Stuck At Detected with Input States: ");
                    using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
                    {
                        writer.WriteLine("Stuck At Detected with Input States: ");
                    }
                    int numInputs = Inputs.Count;                  
                    for (int k = 0; k < numInputs; ++k)
                    {
                        bool inputValue = ((j >> k) & 1) == 1;
                        Console.Write($"{Inputs[k].Name}: {inputValue} ");
                        using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
                        {
                            writer.WriteLine($"{Inputs[k].Name}: {inputValue} ");
                        }
                    }
                    Console.WriteLine();
                }
                else 
                {
                    Console.WriteLine("Stuck At not Detected with Input States: ");
                    using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
                    {
                        writer.WriteLine("Stuck At not Detected with Input States: ");
                    }
                    int numInputs = Inputs.Count;
                    for (int k = 0; k < numInputs; ++k)
                    {
                        bool inputValue = ((j >> k) & 1) == 1;
                        Console.Write($"{Inputs[k].Name}: {inputValue} ");
                        using (StreamWriter writer = new StreamWriter(StaticSettings.OutputPath, true))
                        {
                            writer.WriteLine($"{Inputs[k].Name}: {inputValue} ");
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }       
        public List<KeyValuePair<String, List<List<KeyValuePair<String, bool>>>>> RunBadCircuit()
        {          
            List<Wire> FaultableWires = GiveInputsAndWires();            
            List<KeyValuePair<String,List<List<KeyValuePair<String, bool>>>>> BadCircuitSimulationResults = new List<KeyValuePair<String, List<List<KeyValuePair<String, bool>>>>>();
            foreach (Wire wire in  FaultableWires) 
            {               
                wire.SetFault(true, false); // Setze das Wire Stuck at 0
                var TmpBadStuckAtFalseResult = RunGoldenCircuit();           
                KeyValuePair<String, List<List<KeyValuePair<String, bool>>>> StuckAtFalse = new KeyValuePair<string, List<List<KeyValuePair<string, bool>>>>("StuckAtFalse", TmpBadStuckAtFalseResult);
                BadCircuitSimulationResults.Add(StuckAtFalse);

                wire.SetFault(true, true);  // Setze das Wire Stuck at 1
                var TmpBadStuckAtTrueResult = RunGoldenCircuit();
                KeyValuePair<String, List<List<KeyValuePair<String, bool>>>> StuckAtTrue = new KeyValuePair<string, List<List<KeyValuePair<string, bool>>>>("StuckAtTrue", TmpBadStuckAtTrueResult);
                BadCircuitSimulationResults.Add(StuckAtTrue);

                Console.WriteLine("Stuck at 1 / True at: " + wire.Name);
                PrintResults(TmpBadStuckAtTrueResult);
                Console.WriteLine("Stuck at 0 / False");
                PrintResults(TmpBadStuckAtFalseResult);
                wire.SetFault(false, true);
            }
            return BadCircuitSimulationResults; // Results für alle Wires für Stuck at 0 und Stuck at 1
        }
        private List<Wire> GiveInputsAndWires()
        {
            List<Wire> InputsWires = new List<Wire>();
            InputsWires.AddRange(Wires);
            InputsWires.AddRange(Inputs);
            return InputsWires;
        }
        public void BuildGraph()
        {
            adjList = new Dictionary<Assignment, List<Assignment>>();
            adjList.Clear();
                      
            foreach (Assignment a in Assignments) 
            {
                if (a.LeftOperand != null)
                {
                    foreach (var RightOrLeftOperand in Assignments)
                    {
                        if (RightOrLeftOperand.RightOperand1 == a.LeftOperand || RightOrLeftOperand.RightOperand2 == a.LeftOperand)
                        {
                            if (!adjList.ContainsKey(a))
                            {
                                adjList[a] = new List<Assignment>();
                            }
                            adjList[a].Add(RightOrLeftOperand);
                        }                   
                    }
                }
            }
        }
        public void TopologicalSortUtil(Assignment assignment, Dictionary<Assignment, bool> visited, Stack<Assignment> stack)
        {
            visited[assignment] = true;

            // Recursively call this function for all assignments dependent on this assignment
            if (adjList.ContainsKey(assignment))
            {
                foreach (var dependentAssignment in adjList[assignment])
                {
                    if (!visited[dependentAssignment])
                    {
                        TopologicalSortUtil(dependentAssignment, visited, stack);
                    }
                }
            }
            // Push current assignment to stack which stores the result
            stack.Push(assignment);
        }
        public Stack<Assignment> TopologicalSort()
        {
            Stack<Assignment> stack = new Stack<Assignment>();
            Dictionary<Assignment, bool> visited = new Dictionary<Assignment, bool>();

            // Mark all the assignments as not visited
            foreach (var assignment in Assignments)
            {
                visited[assignment] = false;
            }

            // Call the recursive helper function to store the Topological Sort
            foreach (var assignment in Assignments)
            {
                if (!visited[assignment])
                {
                    TopologicalSortUtil(assignment, visited, stack);
                }
            }

            return stack;
        }
        public List<List<KeyValuePair<String, bool>>> RunGoldenCircuit()
        {
            int numInputs = Inputs.Count;
            int numOutputs = Outputs.Count;
            int numCombinations = 1 << numInputs;

            if (numCombinations > 32) // If computung time will be to high - reduce the number of Input Combinations
            {
                numCombinations = 16;         
            }
            else
            {
                numCombinations = 1 << numInputs;
            }

            // Build the graph for the circuit
            BuildGraph();

            // Perform topological sort to get assignments in the order they should be computed
            Stack<Assignment> sortedAssignments = TopologicalSort();

            List<List<KeyValuePair<string, bool>>> simulationResults = new List<List<KeyValuePair<String, bool>>>();

            for (int i = 0; i < numCombinations; ++i)
            {
                for (int j = 0; j < numInputs; ++j)
                {
                    bool inputValue = ((i >> j) & 1) == 1;
                    Inputs[j].InputState = inputValue;                  
                }

                // Compute outputs using topologically sorted assignments
                Stack<Assignment> tempSortedAssignments = new Stack<Assignment>(sortedAssignments.Reverse()); // Copy sorted assignments for this iteration
                while (tempSortedAssignments.Count > 0)
                {
                    Assignment currentAssignment = tempSortedAssignments.Pop();
                    currentAssignment.LogicOperation();                 
                }              
                List<KeyValuePair<string, bool>> currentOutput = new List<KeyValuePair<string, bool>>();
                for (int k = 0; k < numOutputs; ++k)
                {
                    currentOutput.Add(new KeyValuePair<string, bool>(Outputs[k].Name, Outputs[k].GiveValue()));
                }
                // Store the collected output values
                simulationResults.Add(currentOutput);               
            }

            return simulationResults;

        }
        public bool FindWireByName(string Operand)
        {
            // Iterate through inputs to find the wire
            foreach (var wire in GiveInputsAndWires())
            {
                if (wire.Name == Operand)
                {
                    bool tmpValue = wire.GiveValue();
                    return tmpValue;
                }                                     
            }

            // Iterate through outputs to find the wire
            foreach (var wire in Outputs)
            {
                if (wire.Name == Operand)
                {
                    bool tmpValue = wire.GiveValue();
                    return tmpValue;
                }                 
            }

            // Iterate through internal wires to find the wire
            foreach (var wire in Wires)
            {
                if (wire.Name == Operand)
                {
                    bool tmpValue = wire.GiveValue();
                    return tmpValue;
                }                   
            }

            return false; // Wire not found
        }
    }
}

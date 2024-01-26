using System;
using System.Collections.Generic;
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
            var test = RunGoldenCircuit();      
        }

 
        public void BuiltGraph()
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

        public List<List<bool>> RunGoldenCircuit()
        {
            int numInputs = Inputs.Count;
            int numOutputs = Outputs.Count;
            int numCombinations = 1 << numInputs;

            // Build the graph for the circuit
            BuiltGraph();

            // Perform topological sort to get assignments in the order they should be computed
            Stack<Assignment> sortedAssignments = TopologicalSort();

            List<List<bool>> simulationResults = new List<List<bool>>();

            for (int i = 0; i < numCombinations; ++i)
            {
                for (int j = 0; j < numInputs; ++j)
                {
                    bool inputValue = ((i >> j) & 1) == 1;
                    Inputs[j].InputState = inputValue;
                }

                // Compute outputs using topologically sorted assignments
                Stack<Assignment> tempSortedAssignments = new Stack<Assignment>(sortedAssignments); // Copy sorted assignments for this iteration
                while (tempSortedAssignments.Count > 0)
                {
                    Assignment currentAssignment = tempSortedAssignments.Peek();
                    currentAssignment.LogicOperation();
                    tempSortedAssignments.Pop();
                }

                // Collect output values for this combination
                List<bool> currentOutput = new List<bool>();
                for (int k = 0; k < numOutputs; ++k)
                {
                    currentOutput.Add(Outputs[k].GiveValue());
                }

                // Store the collected output values
                simulationResults.Add(currentOutput);               
            }

            return simulationResults;

        }

        public bool FindWireByName(string Operand)
        {
            // Iterate through inputs to find the wire
            foreach (var wire in Inputs)
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

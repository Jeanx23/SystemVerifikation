using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    public class Assignment
    {
        public string LeftOperand { get; set; }
        public string RightOperand1 { get; set; }
        public string RightOperand2 { get; set; }
        public string Operator { get; set; }
        public bool RightOperand1IsNegated { get; set; }
        public bool RightOperand2IsNegated { get; set; }
        public bool RightOperand1Value { get; set; }
        public bool RightOperand2Value { get; set; }
        public bool LeftOperandValue { get; set; }
        public Simulation ParentSimulation { get; set; }
        public void LogicOperation()
        {
            RightOperand1Value = FindWireValue(RightOperand1);
            RightOperand2Value = FindWireValue(RightOperand2);

            if (RightOperand1IsNegated == true)
            {               
                RightOperand1Value = !RightOperand1Value;
            }
            if (RightOperand2IsNegated == true)
            {              
                RightOperand2Value = !RightOperand2Value;
            }
            if (Operator == "|")
            {
                if (RightOperand1Value || RightOperand2Value)
                {
                    Wire Result = FindWire(LeftOperand);
                    Result.InputState = true;
                }    
            }           
            if (Operator == "&")
            {
                if(RightOperand1Value && RightOperand2Value)
                {                    
                    Wire Result = FindWire(LeftOperand);
                    Result.InputState = true;                  
                }
            }      
            if (Operator == null)
            {
                Wire Result = FindWire(LeftOperand);
                Result.InputState = RightOperand1Value; 
            }
        }

        private Wire FindWire(string leftOperand)
        {
            Wire foundWire = ParentSimulation.Wires.Find(wire => wire.Name == leftOperand);

            // Wenn in der Wires-Liste nicht gefunden, in der Outputs-Liste suchen
            if (foundWire == null)
            {
                foundWire = ParentSimulation.Outputs.Find(wire => wire.Name == leftOperand);
            }

            return foundWire;
        }

        public bool FindWireValue(string operand)
        {
            if (ParentSimulation != null)
            {
                return ParentSimulation.FindWireByName(operand);
            }

            // Handle den Fall, wenn ParentSimulation nicht gesetzt ist
            return false;
        }

    }
}
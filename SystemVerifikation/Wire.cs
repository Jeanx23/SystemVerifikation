using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemVerifikation
{
    public class Wire
    {
        public string Name { get; set; }        
        public bool InputState { get; set; } 
        public bool FaultedBool { get; set; } // Ich bin stuck, ja oder nein
        public bool FaultedValue { get; set; } // Wert auf das der Value stuck ist


        public bool GiveValue()
        {
            if (FaultedBool)
            {
                return FaultedValue;
            }
            else
            {
                return InputState;
            } 
        }

        public void SetFault(bool FaultedBool, bool FaultedValue)
        {
            this.FaultedBool = FaultedBool;
            this.FaultedValue = FaultedValue;
        }
    }
}

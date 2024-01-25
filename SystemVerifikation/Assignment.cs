using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

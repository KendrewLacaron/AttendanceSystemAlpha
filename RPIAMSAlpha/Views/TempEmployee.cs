using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Views
{
     public class TempEmployee
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public TimeSpan EmpTIAM { get; set; }
        public TimeSpan EmpTOAM { get; set; }
        public TimeSpan EmpTIPM { get; set; }
        public TimeSpan EmpTOPM { get; set; }
        public int Active { get; set; }
        public string empAvat { get; set; }
    }
}

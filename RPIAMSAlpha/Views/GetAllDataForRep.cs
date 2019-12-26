using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Views
{
    public class GetAllDataForRep
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public DateTime TDate { get; set; }
        public TimeSpan EmpTIAM { get; set; }
        public TimeSpan EmpTOAM { get; set; }
        public TimeSpan EmpTIPM { get; set; }
        public TimeSpan EmpTOPM { get; set; }
        public TimeSpan TIAM { get; set; }
        public TimeSpan TOAM { get; set; }
        public TimeSpan TIPM { get; set; }
        public TimeSpan TOPM { get; set; }
        public double AMLate { get; set; }
        public double AMUnder { get; set; }
        public double PMLate { get; set; }
        public double PMUnder { get; set; }
    }
}

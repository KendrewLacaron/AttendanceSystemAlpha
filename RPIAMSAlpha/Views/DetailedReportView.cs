using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace RPIAMSAlpha.Views
{
    public class DetailedReportView
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string TDate { get; set; }
        public string TIAM { get; set; }
        public string TOAM { get; set; }
        public string TIPM { get; set; }
        public string TOPM { get; set; }
        public string TIAMLate { get; set; }
        public string TOAMUnderTime { get; set; }
        public string TIPMLate { get; set; }
        public string TOPMUnderTime { get; set; }

    }
}

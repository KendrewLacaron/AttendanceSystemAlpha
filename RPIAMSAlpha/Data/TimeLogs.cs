using System;
using System.Collections.Generic;
using SQLite;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class TimeLogs
    {
        [PrimaryKey, AutoIncrement]
        public int DocNum { get; set; }
        public DateTime TDate { get; set; }
        public string EmpCode { get; set; }
        public string EmpRFID { get; set; }
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

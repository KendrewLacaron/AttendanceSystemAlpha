using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class Employee
    {
        [PrimaryKey, AutoIncrement]
        public int EmpID { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpRFID { get; set; }
        public TimeSpan EmpTIAM { get; set; }
        public TimeSpan EmpTOAM { get; set; }
        public TimeSpan EmpTIPM { get; set; }
        public TimeSpan EmpTOPM { get; set; }
        public int Active { get; set; }
        public string empAvat { get; set; }
        public string template { get; set; }
        
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class InOutOnly
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string EmpCode { get; set; }
        public DateTime TDate { get; set; }
        public TimeSpan TimeLog { get; set; }
        public int ScanType { get; set; }
    }
}

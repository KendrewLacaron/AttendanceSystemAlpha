using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class TblMain1
    {
        [PrimaryKey, AutoIncrement]
        public int DocNum { get; set; }
        public DateTime TDate { get; set; }
        public string EmpCode { get; set; }
        public string EmpRFID { get; set; }
        public int LogTypeCode { get; set; }
        public TimeSpan TimeLog { get; set; }
        public string LogTypeInfo { get; set; }
        public string DevCode { get; set; }
        public int ExportStat { get; set; }
    }
}

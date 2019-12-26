using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class DevicePref
    {
        [PrimaryKey,AutoIncrement]
        public int PrefID { get; set; }
        public int PrefStrType { get; set; }
        public string PrefDevCode { get; set; }
        public TimeSpan PrefExpTime { get; set; }
    } 
}

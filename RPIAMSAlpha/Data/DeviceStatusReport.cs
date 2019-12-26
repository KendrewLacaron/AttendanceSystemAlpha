using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace RPIAMSAlpha.Data
{
    public class DeviceStatusReport
    {
        [PrimaryKey, AutoIncrement]
        public int StatusID { get; set; }
        public DateTime StatusTime { get; set; }
        public int StatusCode { get; set; }
        public string StatusInfo { get; set; }
    }
}

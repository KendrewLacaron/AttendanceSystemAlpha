using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace RPIAMSAlpha.Data
{
    public class IPConfig
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public string ConfigIP { get; set; }
        public string ConfigPort { get; set; }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPIAMSAlpha.Data
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int userID { get; set; }
        public string userName { get; set; }
        public string userPass { get; set; }
        public int userStatus { get; set; }
    }
}

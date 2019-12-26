using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using RPIAMSAlpha.Data;
using System.Threading.Tasks;
using RPIAMSAlpha.Views;
using System.Data;
using System.ComponentModel;
//using OfficeOpenXml;
using System.IO;
//using OfficeOpenXml.Style;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using RPIAMSAlpha.Enums;

namespace RPIAMSAlpha.Database
{
    public class EmployeeDatabase
    {
        readonly SQLiteAsyncConnection database; 

        static int currentSTRType;
        static int exportResetter;

        public int CurrentSTRType { get { return currentSTRType; } set { currentSTRType = value; } }
        public int ExportResetter { get { return exportResetter; } set { exportResetter = value; } }

        public EmployeeDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Employee>().Wait();
            database.CreateTableAsync<TblMain1>().Wait();
            database.CreateTableAsync<TimeLogs>().Wait();
            database.CreateTableAsync<User>().Wait();
            database.CreateTableAsync<IPConfig>().Wait();
            database.CreateTableAsync<DevicePref>().Wait();
            database.CreateTableAsync<DeviceStatusReport>().Wait();
        }

        public Task<int> SavePref(int strType , string devCode, TimeSpan expTime)
        {
            DevicePref localconfig = new DevicePref()
            {
                PrefStrType = strType,
                PrefDevCode = devCode,
                PrefExpTime = expTime
            };
            ExportResetter = 0;
            return database.InsertAsync(localconfig);
        }

        public Task<DevicePref> GetLatestPref()
        {
            return database.Table<DevicePref>().OrderByDescending(x => x.PrefID).FirstOrDefaultAsync();
        }

        //public Task<int> SaveFirstPref(DevicePref pref)
        //{
        //    return database.InsertAsync(pref);
        //}

        public void AddUser()
        {
            User user = new User();
            user.userName = "Admin";
            user.userPass = "1234";
            SaveFirstUserAsync(user);
        }
        public Task<List<Employee>> GetEmployeeAsync()
        {
            //return database.Table<Employee>().ToListAsync();
            return database.QueryAsync<Employee>("SELECT * FROM [Employee] WHERE[Active] = 0");
            //return database.Table<Employee>().ToArrayAsync();
        }
        public Task<List<Employee>> GetEmployeeListAsync()
        {
            return database.Table<Employee>().Where(x => x.Active == 0).ToListAsync();
        }
        
        public List<DetailedReportView> GetDetailedReportsBetweenDate( DateTime sdt , DateTime edt)
        {
            //var la1 = database.QueryAsync<GetAllDataForRep>("SELECT TblMain1.EmpCode, Employee.EmpName, TblMain1.TDate," +
            //    " (SELECT TblMain1.TimeLog FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode " +
            //    " WHERE TblMain1.LogTypeCode = 1 AND Employee.EmpCode = TblMain1.EmpCode AND TblMain1.TDate ) AS TIAM," +
            //    " (SELECT TblMain1.TimeLog FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode" +
            //    " WHERE TblMain1.LogTypeCode = 2 AND Employee.EmpCode = TblMain1.EmpCode AND TblMain1.TDate ) AS TOAM," +
            //    " (SELECT TblMain1.TimeLog FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode" +
            //    " WHERE TblMain1.LogTypeCode = 3 AND Employee.EmpCode = TblMain1.EmpCode AND TblMain1.TDate ) AS TIPM," +
            //    " (SELECT TblMain1.TimeLog FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode" +
            //    " WHERE TblMain1.LogTypeCode = 4 AND Employee.EmpCode = TblMain1.EmpCode AND TblMain1.TDate ) AS TOPM" +
            //    " " +
            //    " FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode" +
            //    " WHERE TblMain1.TDate BETWEEN ? AND ? " +
            //    //" GROUP BY TblMain1.EmpCode AND TblMain1.TDate " + 
            //    " ORDER BY Employee.EmpCode, TblMain1.TDate",sdt,edt);

            var la = database.QueryAsync<GetAllDataForRep>("SELECT TimeLogs.EmpCode, Employee.EmpName, TimeLogs.TDate," +
               " TimeLogs.TIAM, TimeLogs.TOAM, TimeLogs.TIPM, TimeLogs.TOPM, " +
               " TimeLogs.AMLate, TimeLogs.AMUnder, TimeLogs.PMLate, TimeLogs.PMUnder " +
               " FROM Employee INNER JOIN TimeLogs ON Employee.EmpCode = TimeLogs.EmpCode" +
               " WHERE TimeLogs.TDate BETWEEN ? AND ? " +
               //" GROUP BY TblMain1.TDate , Employee.EmpCode " +
               " ORDER BY Employee.EmpCode, TimeLogs.TDate", sdt, edt);

            ////var la = database.QueryAsync<GetAllDataForRep>("SELECT TblMain1.EmpCode, Employee.EmpName, TblMain1.TDate," +
            ////   "Employee.EmpTIAM, Employee.EmpTOAM, Employee.EmpTIPM, Employee.EmpTOPM ," +
            ////   " CASE WHEN TblMain1.LogTypeCode= '1' THEN TblMain1.TimeLog END TIAM," +
            ////   " CASE WHEN TblMain1.LogTypeCode= '2' THEN TblMain1.TimeLog END TOAM," +
            ////   " CASE WHEN TblMain1.LogTypeCode= '3' THEN TblMain1.TimeLog END TIPM," +
            ////   " CASE WHEN TblMain1.LogTypeCode= '4' THEN TblMain1.TimeLog END TOPM " +
            ////   " FROM Employee INNER JOIN TblMain1 ON Employee.EmpCode = TblMain1.EmpCode" +
            ////   " WHERE TblMain1.TDate BETWEEN ? AND ? " +
            ////   " GROUP BY TblMain1.TDate , Employee.EmpCode " +
            ////   " ORDER BY Employee.EmpCode, TblMain1.TDate", sdt, edt);

            //= (isPortrait ? StackOrientation.Vertical : StackOrientation.Horizontal);
            return la.Result.ConvertAll(x => new DetailedReportView { EmpCode = x.EmpCode, EmpName = x.EmpName,

                TIAM = (x.TIAM.Ticks != 0 ? DateTime.Today.AddTicks(x.TIAM.Ticks).ToShortTimeString() : null),
                TOAM = (x.TOAM.Ticks != 0 ? DateTime.Today.AddTicks(x.TOAM.Ticks).ToShortTimeString() : null),
                TIPM = (x.TIPM.Ticks != 0 ? DateTime.Today.AddTicks(x.TIPM.Ticks).ToShortTimeString() : null),
                TOPM = (x.TOPM.Ticks != 0 ? DateTime.Today.AddTicks(x.TOPM.Ticks).ToShortTimeString() : null),
                TDate = x.TDate.ToShortDateString(),
                TIAMLate = x.AMLate.ToString(),
                TOAMUnderTime = x.AMUnder.ToString(),
                TIPMLate = x.PMLate.ToString(),
                TOPMUnderTime = x.PMUnder.ToString(),
                //TIAMLate = (x.TIAM.Ticks !=0 ? (x.EmpTIAM.Subtract(x.TIAM).TotalMinutes *-1).ToString() : null),
                //TIPMLate = (x.TIPM.Ticks != 0 ? (x.EmpTIPM.Subtract(x.TIPM).TotalMinutes *-1).ToString() : null),
                //TOAMUnderTime = (x.TOAM.Ticks != 0 ? (x.EmpTOAM.Subtract(x.TOAM).TotalMinutes*-1).ToString() : null),
                //TOPMUnderTime = (x.TOPM.Ticks != 0 ? (x.EmpTOPM.Subtract(x.TOPM).TotalMinutes*-1).ToString() : null),

            });

        }

        public List<SummaryReportView> GetSummaryReportsBetweenDate(DateTime sdt, DateTime edt)
        {
            var la = database.QueryAsync<GetAllDataForRep>("SELECT TimeLogs.EmpCode, Employee.EmpName, TimeLogs.TDate," +
             " Employee.EmpTIAM, Employee.EmpTOAM, Employee.EmpTIPM,Employee.EmpTOPM, " +
             " SUM (TimeLogs.AMLate) as AMLate, SUM (TimeLogs.AMUnder) as AMUnder, SUM (TimeLogs.PMLate) as PMLate, SUM (TimeLogs.PMUnder) as PMUnder " +
             " FROM Employee INNER JOIN TimeLogs ON Employee.EmpCode = TimeLogs.EmpCode" +
             " WHERE TimeLogs.TDate BETWEEN ? AND ? " +
             " GROUP BY Employee.EmpCode " +
             " ORDER BY Employee.EmpCode", sdt, edt);

            //var q = database.QueryAsync<GetAllDataForRep>("SELECT e.EmpName , t.TimeLog , t.LogTypeInfo , t.LogTypeCode, t.TDate FROM Employee e "
            //    + "INNER JOIN TblMain1 t "
            //    + "ON e.EmpCode = t.EmpCode WHERE t.TDate BETWEEN ? AND ? ORDER BY t.TDate , t.LogTypeCode , e.EmpCode", sdt, edt);

            ////= (isPortrait ? StackOrientation.Vertical : StackOrientation.Horizontal);
            ///

            return la.Result.ConvertAll(x => new SummaryReportView
            {

                EmpCode = x.EmpCode,
                EmpName = x.EmpName,
                TIAMLate = x.AMLate.ToString(),
                TOAMUnderTime = x.AMUnder.ToString(),
                TIPMLate = x.PMLate.ToString(),
                TOPMUnderTime = x.PMUnder.ToString()
                //TIAMLate = x.TIAM.Ticks != 0 ? (x.EmpTIAM.Subtract(x.TIAM).TotalMinutes * -1).ToString() : null,
                //TIPMLate = x.TIPM.Ticks != 0 ? (x.EmpTIPM.Subtract(x.TIPM).TotalMinutes * -1).ToString() : null,
                //TOAMUnderTime = x.TOAM.Ticks != 0 ? (x.EmpTOAM.Subtract(x.TOAM).TotalMinutes * -1).ToString() : null,
                //TOPMUnderTime = x.TOPM.Ticks != 0 ? (x.EmpTOPM.Subtract(x.TOPM).TotalMinutes * -1).ToString() : null,

            });

            

        }

        public Task<int> GetCountSQLITEExport(DateTime sdt , DateTime edt)
        {
            return database.Table<TblMain1>().Where(x => x.TDate >=sdt && x.TDate <= edt).CountAsync();
        }
        public async Task<int> GetCountSQLExport(DateTime sdt , DateTime edt , string deviceCode)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(await IPMaker() + "/API/IW/GetCountResult?DTBeginDate=" + sdt + "&DTEndDate="+ edt +"&strDeviceCode=" + deviceCode );
            var noresult = JsonConvert.DeserializeObject<int>(response);
            return noresult;
        }

        public async Task<List<SQLEmployeeView>> GetSQLEmployeeAsync()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(await IPMaker() + "/API/Import/Employee");
            var employees = JsonConvert.DeserializeObject<List<SQLEmployeeView>>(response);
            return employees;
            //ConvertAll(x => new SQLEmployeeView
            //{   empAvat = x.empAvat , Active = x.Active.ToString() , EmpCode = x.EmpCode ,
            //    EmpName = x.EmpName,
            //    EmpTIAM = (x.EmpTIAM != null ? DateTime.Today.AddTicks(Convert.ToInt64(x.EmpTIAM)).ToShortTimeString() : null),
            //    EmpTOAM = (x.EmpTOAM != null ? DateTime.Today.AddTicks(Convert.ToInt64(x.EmpTOAM)).ToShortTimeString() : null),
            //    EmpTIPM = (x.EmpTIPM != null ? DateTime.Today.AddTicks(Convert.ToInt64(x.EmpTIPM)).ToShortTimeString() : null),
            //    EmpTOPM = (x.EmpTOPM != null ? DateTime.Today.AddTicks(Convert.ToInt64(x.EmpTOPM)).ToShortTimeString() : null)
            //});
        }

        public async Task<List<Employee>> GetSQLEmployeeAsyncRaw()
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(await IPMaker() + "/API/Import/EmployeeRaw");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(response);
            return employees;
        }


        public List<EntryLogData> GetEmployeeTodayLogAsync()
        {
            var q = database.QueryAsync<EntryLogData>("SELECT e.EmpName , e.empAvat , e.EmpCode , t.TimeLog , t.LogTypeInfo FROM Employee e " 
                + "INNER JOIN TblMain1 t " 
                + "ON e.EmpCOde = t.EmpCode WHERE t.TDate = ? ORDER BY t.TimeLog DESC",DateTime.Today);
                
            return q.Result.ConvertAll(x => new EntryLogData { empAvat = x.empAvat , EmpCode = x.EmpCode , EmpName = x.EmpName, LogTypeInfo = x.LogTypeInfo, TimeLog = DateTime.Today.AddTicks(long.Parse(x.TimeLog)).ToShortTimeString()});
        }

        public Task<Employee> GetEmployeeAsync(int id)
        {
            return database.Table<Employee>().Where(i => i.EmpID == id).FirstOrDefaultAsync();
        }

        public Task<Employee> Sum(int id)
        {
            return database.Table<Employee>().Where(i => i.EmpID == id).FirstOrDefaultAsync();
        }

        public Task<double> SumofTotal()
        {
           var la = database.QueryAsync<double>("Select SUM(TIAMLate) FROM Timelogs");
            
           return  database.FindWithQueryAsync<double>("SELECT SUM(AMLate) FROM Timelogs");
        
           //return la.Result.FirstOrDefault();
        }

        public Task<Employee> GetEmployeeByCodeAsync(string code)
        {
            return database.Table<Employee>().Where(i => i.EmpCode == code && i.Active == 0).FirstOrDefaultAsync();
        }

        public Task<Employee> GetEmployeeByRFID(string code)
        {
            return database.Table<Employee>().Where(i => i.EmpRFID == code && i.Active == 0).FirstOrDefaultAsync();
        }

        public Task<Employee> GetEmployeeByCodeAsyncRegardless(string code)
        {
            return database.Table<Employee>().Where(i => i.EmpCode == code).FirstOrDefaultAsync();
        }


        public Task<User> GetUserByUsernamePass( string user, string pass)
        {
            return database.Table<User>().Where(i => i.userName.ToLower() == user.ToLower() && i.userPass == pass ).FirstOrDefaultAsync();
        }


        public Task<TblMain1> GetTblMainByCodeAsync(string code)
        {
            return database.Table<TblMain1>().Where(i => i.EmpCode == code).FirstOrDefaultAsync();
        }

        public Task<TblMain1> GetTblMainByCodeListAsync(string code , int logtypecode)
        {
            return database.Table<TblMain1>().Where(i => (i.EmpCode == code && i.TDate == DateTime.Today && i.LogTypeCode == logtypecode)).FirstOrDefaultAsync();
        }

        public Task<TblMain1> GetTblMainByRFIDListAsync(string code, int logtypecode)
        {
            return database.Table<TblMain1>().Where(i => (i.EmpRFID == code && i.TDate == DateTime.Today && i.LogTypeCode == logtypecode)).FirstOrDefaultAsync();
        }

        public  List<TimeLogStringFormatView> GetTblMain1ByDateToLIST(DateTime sdt, DateTime edt)
        {
            var la = database.Table<TblMain1>().Where(i => i.TDate >= sdt && i.TDate <= edt).ToListAsync();

            return la.Result.ConvertAll(x => new TimeLogStringFormatView
            {
                TDate = x.TDate,
                EmpCode = x.EmpCode.ToString(),
                LogTypeCode = x.LogTypeCode.ToString(),
                Timelog = (x.TimeLog.Ticks != 0 ? DateTime.Today.AddTicks(x.TimeLog.Ticks).ToShortTimeString() : null),
                DeviceCode = x.DevCode
            });
            
            
            
        }

        public List<TimeLogStringFormatView> GetTblMain1ByDateToLIST()
        {
            var la = database.Table<TblMain1>().Where(i => i.ExportStat == 0).ToListAsync();

            return la.Result.ConvertAll(x => new TimeLogStringFormatView
            {
                TDate = x.TDate,
                EmpCode = x.EmpCode.ToString(),
                LogTypeCode = x.LogTypeCode.ToString(),
                Timelog = (x.TimeLog.Ticks != 0 ? DateTime.Today.AddTicks(x.TimeLog.Ticks).ToShortTimeString() : null),
                DeviceCode = x.DevCode
            });



        }

        public List<TimeLogStringFormatView> GetTblMain1ByDateToLISTRegardless (DateTime sdt , DateTime edt)
        {
            var la = database.Table<TblMain1>().Where(x => x.TDate >= sdt && x.TDate <= edt).ToListAsync();

            return la.Result.ConvertAll(x => new TimeLogStringFormatView
            {
                TDate = x.TDate,
                EmpCode = x.EmpCode.ToString(),
                LogTypeCode = x.LogTypeCode.ToString(),
                Timelog = (x.TimeLog.Ticks != 0 ? DateTime.Today.AddTicks(x.TimeLog.Ticks).ToShortTimeString() : null),
                DeviceCode = x.DevCode
            });



        }
        public  async void ChangeExportStatus(int status)
        {
          List<TblMain1> updates = new List<TblMain1>();
          int realStat = (status > 0 ? 0:1);

          List<TblMain1> tbl = await database.Table<TblMain1>().Where(i => i.ExportStat == realStat).ToListAsync();

            foreach (var item in tbl)
            {
                item.ExportStat = status;
                updates.Add(item);
            }
            
            await database.UpdateAllAsync(updates); 
        }

        public Task<User> GetUser()
        {
            return database.Table<User>().FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.userID != 0)
            {
                return database.UpdateAsync(user);
            }
            else
            {
                return database.InsertOrReplaceAsync(user);
            }
        }
        
        public async Task<int> SaveAllEmployee(List<SQLEmployeeView> emp)
        {
            int o = 0;
            List<Employee> updates = new List<Employee>();

            List<Employee> inserts = new List<Employee>();

            foreach (var e in emp)
            {
                var p =  await GetEmployeeByCodeAsyncRegardless(e.EmpCode);

                //DateTime dtTiam;
                //DateTime dtToam;
                //TimeSpan tstiam;
                //TimeSpan tstoam;
                //if (!DateTime.TryParseExact(e.EmpTIAM,"HH:mm:tt",System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out dtTiam))
                //{
                //    // Error
                //}
                //else
                //{
                //     tstiam = dtTiam.TimeOfDay;
                //}

                //DateTime dateTime = DateTime.ParseExact(text,
                //                    "hh:mm tt", CultureInfo.InvariantCulture);
                //TimeSpan span = dateTime.TimeOfDay;
                //string str = "12:01 AM";
                //TimeSpan ts = Convert.ToDateTime(str).TimeOfDay;
                //if (!DateTime.TryParseExact(e.EmpTOAM, "HH:mm:tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtToam))
                //{
                //    // Error
                //}
                //else
                //{
                //    tstoam = dtToam.TimeOfDay;
                //}

                Employee ee = new Employee()
                {
                    //EmpID = p.EmpID,
                    empAvat = "Avatars/boy.png" ,
                    EmpCode = e.EmpCode,
                    EmpRFID = e.EmpRFID,
                    EmpName = e.EmpName,
                    EmpTIAM = Convert.ToDateTime(e.EmpTIAM).TimeOfDay,
                    EmpTOAM = Convert.ToDateTime(e.EmpTOAM).TimeOfDay,
                    Active = 0,
            };
                //e.empAvat = "Avatars/boy.png";
                if (p != null)
                {
                    o++;
                    ee.EmpID = p.EmpID;
                    //e.EmpID = p.EmpID;
                    updates.Add(ee);
                } 
                else
                {
                    inserts.Add(ee);
                }
            }
             
             await  database.UpdateAllAsync(updates);
             await  database.InsertAllAsync(inserts);
             return o;
        }


        public Task<int> SaveFirstUserAsync(User user)
        {
            return database.InsertAsync(user);
        }
        public Task<int> SaveEmployeeAsync(Employee Employee)
        {
            if (Employee.EmpID != 0)
            {
                return database.UpdateAsync(Employee);
            }
            else
            {
                return database.InsertAsync(Employee);
            }
        }


        public async Task<int> SaveScanData(TblMain1 tblMain1)
        {
           return await database.InsertAsync(tblMain1);
        }

        public Task<TimeLogs> GetLogsByCode(string code)
        {
            return database.Table<TimeLogs>().Where(i => (i.EmpCode == code && i.TDate == DateTime.Today)).FirstOrDefaultAsync();
        }


        public async Task<int> SaveLogDataAsync(TimeLogs logs , int logtype)
        {
            if(await GetLogsByCode(logs.EmpCode) != null)
            {
                TimeLogs pp = await GetLogsByCode(logs.EmpCode);
                logs.DocNum = pp.DocNum;
                //TimeLogs a = await GetLogsByTIAM(logs.EmpCode);
                //TimeLogs b = await GetLogsByTOAM(logs.EmpCode);
                //TimeLogs c = await GetLogsByTIPM(logs.EmpCode);
                //TimeLogs d = await GetLogsByTOPM(logs.EmpCode);

                if (pp.TIAM != TimeSpan.Zero)
                {
                    logs.TIAM = pp.TIAM;
                }
                if (pp.TOAM != TimeSpan.Zero)
                {
                    logs.TOAM = pp.TOAM;
                }
                if (pp.TIPM != TimeSpan.Zero)
                {
                    logs.TIPM = pp.TIPM;
                }
                if (pp.TOPM != TimeSpan.Zero)
                {
                    logs.TOPM = pp.TOPM;
                }
                if (pp.AMLate != 0)
                {
                    logs.AMLate = pp.AMLate;
                }
                if (pp.AMUnder != 0)
                {
                    logs.AMUnder = pp.AMUnder;
                }
                if (pp.PMLate != 0)
                {
                    logs.PMLate = pp.PMLate;
                }
                if (pp.PMUnder != 0)
                {
                    logs.PMUnder = pp.PMUnder;
                }
                return await database.UpdateAsync(logs);
            }
            else
            {
                return await database.InsertAsync(logs);
            }
        }

        public Task<int> CheckScanData(TblMain1 tblMain1)
        {
            if (tblMain1.LogTypeCode != 0 && tblMain1.TDate != Convert.ToDateTime(0))
            {
                return null;
            }
            else
            {
                return database.InsertAsync(tblMain1);
            }
        }


        public Task<int> RemoveEmployeeAsync(Employee Employee)
        {
            return database.DeleteAsync(Employee);
        }

        public Task<int> DeleteEmployeeAsync(Employee Employee)
        {
            if (Employee.EmpID != 0)
            {
                Employee.Active = 1;
                return database.UpdateAsync(Employee);
            }
            else
            {
                return database.InsertAsync(Employee);
            }

        }



        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public void ListToExcel<T>(List<T> query)
        {
            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    //Create the worksheet
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Result");

            //    //get our column headings
            //    var t = typeof(T);
            //    var Headings = t.GetProperties();
            //    for (int i = 0; i < Headings.Count(); i++)
            //    {

            //        ws.Cells[1, i + 1].Value = Headings[i].Name;
            //    }

            //    //populate our Data
            //    if (query.Count() > 0)
            //    {
            //        ws.Cells["A2"].LoadFromCollection(query);
            //    }

            //    //Format the header
            //    using (ExcelRange rng = ws.Cells["A1:BZ1"])
            //    {
            //        rng.Style.Font.Bold = true;
            //        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;   //Set Pattern for the background to Solid
            //        rng.Style.Fill.BackgroundColor.SetColor(Xamarin.Forms.Color.Red);  //Set color to dark blue
            //        rng.Style.Font.Color.SetColor(Xamarin.Forms.Color.White);
            //    }

            //    ws.Protection.IsProtected = true;
            //    ws.Protection.AllowSelectLockedCells = false;

            //    //Write it back to the client
            //    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    //Response.AddHeader("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
            //    //Response.BinaryWrite(pck.GetAsByteArray());
            //    //Response.End();
            //    pck.SaveAs(new FileInfo("Reports.xlsx"));
            //}
        }

        public Task<int> SaveIPConfiguration(string ip, string port)
        {
            IPConfig localconfig = new IPConfig()
            {
                ConfigIP = ip,
                ConfigPort = port
            };
            return SaveFirstIPConfig(localconfig);
        }

        public Task<IPConfig> GetLatestConfig()
        {
            return database.Table<IPConfig>().OrderByDescending(x => x.ID).FirstOrDefaultAsync();
        }

        public Task<int> SaveFirstIPConfig(IPConfig config)
        {
            return database.InsertAsync(config);
        }

        public async Task<string> IPMaker()
        {
            string url;

            IPConfig ipconfig = await App.EmployeeDatabase.GetLatestConfig();

            if (ipconfig.ConfigPort == null || ipconfig.ConfigPort == string.Empty)
            {
                url = "http://" + ipconfig.ConfigIP;
            }
            else
            {
                url = "http://" + ipconfig.ConfigIP + ":" + ipconfig.ConfigPort;
            }

            return url;
        }

        public Task<List<TblMain1>> GetTBLMAIN1List()
        {
            return database.Table<TblMain1>().ToListAsync();
        }

        public Task<int> ResetAll()
        {
            var a = database.Table<TblMain1>().Where(x => x.ExportStat == 1).ToListAsync();
            database.DeleteAsync(a);
            return database.DeleteAllAsync<TimeLogs>();
        } 

        public Task<int> ResetAllTimelogs()
        {
            var a = database.Table<TblMain1>().Where(x => x.ExportStat == 1).ToListAsync();
            database.DeleteAsync(a);
            return  database.DeleteAllAsync<TimeLogs>();
        }

        public Task<int> ResetAllTimelogs(DateTime sdt , DateTime edt)
        {
            var a = database.Table<TblMain1>().Where(x => x.ExportStat == 1).Where(x => x.TDate >= sdt && x.TDate <= edt).ToListAsync();
            database.DeleteAsync(a);
            return database.DeleteAllAsync<TimeLogs>();
        }

        public async void ExportAllRecords()
        {
            var requestedList =   App.EmployeeDatabase.GetTblMain1ByDateToLIST();
            DataTable td =  App.EmployeeDatabase.ConvertToDataTable(requestedList);

                var json1 = JsonConvert.SerializeObject(td);
                var content1 = new StringContent(json1, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                try
                {
                    var result1 = await client.PostAsync(await App.EmployeeDatabase.IPMaker() + "/API/Report/TimeLog", content1);
                    if (result1.StatusCode == HttpStatusCode.Created)
                    {
                      SaveDeviceStatusReport(ExportReportEnums.Success, result1.StatusCode.ToString());
                      ChangeExportStatus(1);
                      
                    }
                    else
                    {
                       SaveDeviceStatusReport(ExportReportEnums.Error, result1.StatusCode.ToString());

                    }

                }
                catch (Exception ex)
                {
                      SaveDeviceStatusReport(ExportReportEnums.ErrorList, ex.Message.ToString());
                }

        }

        public Task<DeviceStatusReport> GetLatestExportReport(ExportReportEnums _repEnum)
        {
            if(_repEnum == ExportReportEnums.Error)
            {
                return database.Table<DeviceStatusReport>().Where(x => x.StatusCode != 1).OrderByDescending(x => x.StatusID).FirstOrDefaultAsync();
            }
            else if (_repEnum == ExportReportEnums.Success)
            {
                return database.Table<DeviceStatusReport>().Where(x => x.StatusCode == 1).OrderByDescending(x => x.StatusID).FirstOrDefaultAsync();
            }
            else
            {
                return database.Table<DeviceStatusReport>().Where(x => x.StatusCode == 1).OrderByDescending(x => x.StatusID).FirstOrDefaultAsync();
            }
        }
        public Task<List<DeviceStatusReport>> GetReportsList(ExportReportEnums _repEnum)
        {
            if (_repEnum == ExportReportEnums.SuccessList)
            {
                return database.Table<DeviceStatusReport>().Where(x => x.StatusCode == 1).OrderByDescending(x => x.StatusID).ToListAsync();
            }
            else if (_repEnum == ExportReportEnums.ErrorList)
            {
                return database.Table<DeviceStatusReport>().Where(x => x.StatusCode != 1).OrderByDescending(x => x.StatusID).ToListAsync();
            }
            else if (_repEnum == ExportReportEnums.All)
            {
                return database.Table<DeviceStatusReport>().OrderByDescending(x => x.StatusID).ToListAsync();
            }
            else
            {
                return database.Table<DeviceStatusReport>().OrderByDescending(x => x.StatusID).ToListAsync();
            }
        }

        public void SaveDeviceStatusReport(ExportReportEnums _repEnum,string additonalMessage)
        {
            DeviceStatusReport dvs = new DeviceStatusReport();

            if(_repEnum == ExportReportEnums.Success)
            {
                dvs.StatusCode = 1;
                dvs.StatusInfo = "Export Success 001 : " + additonalMessage + " on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                dvs.StatusTime = DateTime.Now;

            }else if (_repEnum == ExportReportEnums.Error)
            {
                dvs.StatusCode = 2;
                dvs.StatusInfo = "Connection Error 002 : " + additonalMessage + " on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                dvs.StatusTime = DateTime.Now;

            }else
            {
                dvs.StatusCode = 3;
                dvs.StatusInfo = "Error 003 : " + additonalMessage +" on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                dvs.StatusTime = DateTime.Now;
            }

            database.InsertAsync(dvs);
        }
    }
}
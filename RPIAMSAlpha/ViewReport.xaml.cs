//using OfficeOpenXml;
//using OfficeOpenXml.Style;
using RPIAMSAlpha.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Security.Permissions;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;

namespace RPIAMSAlpha
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewReport : ContentPage
    {
        public List<DetailedReportView> e;
        public DateTime sdt, edt;
        public ViewReport()
        {
            InitializeComponent();
        }

        private async void BtnExporttoMSSQL_Clicked(object sender, EventArgs e)
        {
            //DataTable dt = App.EmployeeDatabase.ConvertToDataTable(App.EmployeeDatabase.GetDetailedReportsBetweenDate(sdt, edt));
            var requestedList = App.EmployeeDatabase.GetTblMain1ByDateToLIST();
            DataTable td = App.EmployeeDatabase.ConvertToDataTable( requestedList);
            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Detailed Report");
            //    //ExcelRange firstCell = ws.Cells[1, 1, 3, dt.Columns.Count + 4];
            //    //firstCell.Merge = true;
            //    //firstCell.Value = " Detailed Report From " + sdt.ToShortDateString() + " to " + edt.ToShortDateString() + " ";
            //    //firstCell.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //    //firstCell.Style.Font.Size = 15;
            //    //firstCell.Style.Font.Bold = true;
            //    //firstCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    //firstCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //    //ws.Cells["A5"].LoadFromDataTable(App.EmployeeDatabase.ConvertToDataTable(App.EmployeeDatabase.GetDetailedReportsBetweenDate(sdt, edt)), true);

            //    //ws.Protection.IsProtected = true;
            //    //ws.Protection.AllowSelectLockedCells = false;


            //    //pck.SaveAs(new FileInfo("Reports.xlsx"));
                
            //}

            //BatchBulkCopy(dt,"dbo.tblDetailedReport",100);

            bool oo = await DisplayAlert("Hey", "This will update the table\r\nAre Previous Records Cleared?", "Alright" ,"Cancel That");

            if (oo) {

              
                //var json = JsonConvert.SerializeObject(dt);
                //var content = new StringContent(json, Encoding.UTF8, "application/json");

                var json1 = JsonConvert.SerializeObject(td);
                var content1 = new StringContent(json1, Encoding.UTF8, "application/json");
                await PopupNavigation.Instance.PushAsync(new LoadingPopup(), true);
                HttpClient client = new HttpClient();
                try
                {
                    //var result = await client.PostAsync(await App.EmployeeDatabase.IPMaker() + "/API/Report/Detailed", content);
                    var result1 = await client.PostAsync(await App.EmployeeDatabase.IPMaker() + "/API/Report/TimeLog", content1);
                    if (/*result.StatusCode == HttpStatusCode.Created &&*/ result1.StatusCode == HttpStatusCode.Created)
                    {
                        await DisplayAlert("Hey", "Records has been added", "Alright");
                        //await DisplayAlert("Hey", "Status " /*+ result.StatusCode.ToString() */+ "\r\n Status " + result1.StatusCode.ToString(), "Alright");
                        App.EmployeeDatabase.ChangeExportStatus(1);
                    }
                    else
                    {
                        // await DisplayAlert("MessageBox",result+"","Ok");
                        await DisplayAlert("Hey", "Record has not been added", "Alright");
                        await DisplayAlert("Hey", /*"Status "/* + result.StatusCode.ToString()*/ "\r\n Status " + result1.StatusCode.ToString(), "Alright");

                    }
                    await Navigation.PopAsync();

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Reminder..", ex.Message, "Okay");
                    await DisplayAlert("Hey", "No Connection Found", "Alright");
                }
                finally
                {
                    await PopupNavigation.Instance.PopAsync(true);
                }

            }
            else
            {
                await DisplayAlert("Hey", "Operation Canceled", "Alright");
            }
           
            

        }

        public void BatchBulkCopy(DataTable dataTable, string DestinationTbl, int batchSize)
        {
            // Get the DataTable 
            DataTable dtInsertRows = dataTable;
            string
            plsMyConMSSQL = "Server =  WINSERVER; Database = DOCTORCARE_BE; User ID = server2008; Password = Mssqlone1; Trusted_Connection = False;";

            using (SqlBulkCopy sbc = new SqlBulkCopy(plsMyConMSSQL, SqlBulkCopyOptions.KeepIdentity))
            {
                sbc.DestinationTableName = DestinationTbl;

                // Number of records to be processed in one go
                sbc.BatchSize = batchSize;

                // Add your column mappings here
                //foreach (var mapping in columnMapping)
                //{
                //    var split = mapping.Split(new[] { ',' });
                //    sbc.ColumnMappings.Add(split.First(), split.Last());
                //}
                //sbc.ColumnMappings.Add("field1", "field3");
                //sbc.ColumnMappings.Add("foo", "bar");
                //AutoMapColumns(sbc, dataTable);

                // Finally write to server
                sbc.WriteToServer(dtInsertRows);
            }
        }

        public static void AutoMapColumns(SqlBulkCopy sbc, DataTable dt)
        {
            foreach (DataColumn column in dt.Columns)
            {
                sbc.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
        
            listView.ItemsSource = e;
            lblSdt.Text = sdt.ToShortDateString();
            lblEdt.Text = edt.ToShortDateString();

        }

        private void BtnReturn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }





    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RPIAMSAlpha.Views;
//using OfficeOpenXml;
using System.Data;
//using OfficeOpenXml.Style;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;

namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewReportSummary : ContentPage
	{
        public List<SummaryReportView> e;
        public DateTime sdt, edt;

        public ViewReportSummary ()
		{
			InitializeComponent ();
		}

        private async void BtnExporttoMSSQL_Clicked(object sender, EventArgs e)
        {

            var requestedList = App.EmployeeDatabase.GetTblMain1ByDateToLIST();
            DataTable td = App.EmployeeDatabase.ConvertToDataTable(requestedList);

            //DataTable dt = App.EmployeeDatabase.ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(sdt, edt));
            //DataTable td = App.EmployeeDatabase.ConvertToDataTable(App.EmployeeDatabase.GetTblMain1ByDateToLIST(sdt, edt));
            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Summary Report");
            //    //ExcelRange firstCell = ws.Cells[1, 1, 3, dt.Columns.Count + 4];
            //    //firstCell.Merge = true;
            //    //firstCell.Value = " Summary Report From " + sdt.ToShortDateString() + " to " + edt.ToShortDateString() + " ";
            //    //firstCell.Style.Border.BorderAround(ExcelBorderStyle.Medium);
            //    //firstCell.Style.Font.Size = 15;
            //    //firstCell.Style.Font.Bold = true;
            //    //firstCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //    //firstCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //    //var pp = ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(dtSdt.Date, dtEdt.Date));

            //    //double o = 0;

            //    //foreach (DataRow row in pp.Rows)
            //    // {
            //    //     o += Convert.ToDouble(row["TIAMLate"].ToString());
            //    // }

            //    ws.Cells["A5"].LoadFromDataTable(App.EmployeeDatabase.ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(sdt, edt)), true);

            //    ws.Protection.IsProtected = true;
            //    ws.Protection.AllowSelectLockedCells = false;

            //    //StorageFolder devices = Windows.Storage.KnownFolders.RemovableDevices;
            //    //StorageFolder sdCard = (await devices.GetFoldersAsync()).FirstOrDefault();

            //    //if (sdCard != null)
            //    //{
            //    //    pck.SaveAs(new FileInfo(sdCard.Path + "Reports.xlsx"));
            //    //    await DisplayAlert("Yosh...", "Summary Report Exported", "Sure");
            //    //    await Navigation.PopToRootAsync();
            //    //}
            //    //else
            //    //{
            //    //    pck.SaveAs(new FileInfo("Reports.xlsx"));
            //    //    await DisplayAlert("Yosh...", "USB not found or recognized", "Sure");
            //    //}


            //}

            bool oo = await DisplayAlert("Hey", "This will update the table\r\nAre Previous Records Cleared?", "Alright", "Cancel That");
            if (oo) {

                //var json = JsonConvert.SerializeObject(dt);
                //var content = new StringContent(json, Encoding.UTF8, "application/json");

                var json1 = JsonConvert.SerializeObject(td);
                var content1 = new StringContent(json1, Encoding.UTF8, "application/json");


                await PopupNavigation.Instance.PushAsync(new LoadingPopup(), true);

                HttpClient client = new HttpClient();
                try
                {
                    //var result = await client.PostAsync(await App.EmployeeDatabase.IPMaker() + "/API/Report/Summary", content);
                    var result1 = await client.PostAsync(await App.EmployeeDatabase.IPMaker() + "/API/Report/TimeLog", content1);
                    if (/*result.StatusCode == HttpStatusCode.Created && */result1.StatusCode == HttpStatusCode.Created)
                    {
                        await DisplayAlert("Hey", "Records has been added", "Alright");
                        //await DisplayAlert("Hey", "Status " + result.StatusCode.ToString(), "Alright");
                        //await DisplayAlert("Hey", "Status " + result1.StatusCode.ToString(), "Alright");
                        App.EmployeeDatabase.ChangeExportStatus(1);
                    }
                    else
                    {

                        // await DisplayAlert("MessageBox",result+"","Ok");
                        await DisplayAlert("Hey", "Record has not been added", "Alright");
                        //await DisplayAlert("Hey", "Status " + result.StatusCode.ToString(), "Alright");
                        await DisplayAlert("Hey", "Status " + result1.StatusCode.ToString(), "Alright");
                    }
                    await Navigation.PopAsync();

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Reminder..", ex.Message, "Okay");
                    await DisplayAlert("Reminder..", "No Connection Found", "Okay");

                }
                finally
                {
                    await PopupNavigation.Instance.PopAsync(true);
                }

            } else
            {
                await DisplayAlert("Hey", "Operation Canceled", "Alright");
            }

        }

        private void BtnReturn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();

            listView.ItemsSource = e;
            lblSdt.Text = sdt.ToShortDateString();
            lblEdt.Text = edt.ToShortDateString();

        }
    }
}
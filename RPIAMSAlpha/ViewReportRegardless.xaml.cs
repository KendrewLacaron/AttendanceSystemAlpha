using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;
using RPIAMSAlpha.Views;
namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewReportRegardless : ContentPage
	{
        public List<DetailedReportView> e;
        public DateTime sdt, edt;

        public ViewReportRegardless ()
		{
			InitializeComponent ();
		}

        private async void BtnExporttoMSSQL_Clicked(object sender, EventArgs e)
        {
            var requestedList = App.EmployeeDatabase.GetTblMain1ByDateToLISTRegardless(sdt,edt);
            DataTable td = App.EmployeeDatabase.ConvertToDataTable(requestedList);
            bool oo = await DisplayAlert("Hey", "This will update the table\r\nAre Previous Records Cleared?", "Alright", "Cancel That");

            if (oo)
            {


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
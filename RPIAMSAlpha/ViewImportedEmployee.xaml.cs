using Newtonsoft.Json;
using RPIAMSAlpha.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RPIAMSAlpha.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;

namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewImportedEmployee : ContentPage
	{
        public List<SQLEmployeeView> ee;
        //public List<Employee> r;

        public ViewImportedEmployee ()
		{
			InitializeComponent ();

		}

        protected override void OnAppearing()
        {

            base.OnAppearing();
            listView.ItemsSource = ee;
        
        }

        private void BtnReturn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void BtnExporttoSQLite_Clicked(object sender, EventArgs e)
        {
            bool o = await DisplayAlert("Reminder", "This will add records and update existing ones", "Okay", "Cancel");

            if (o)
            {
                await PopupNavigation.Instance.PushAsync(new LoadingPopup(), true);
                try
                {
                    var k = await App.EmployeeDatabase.SaveAllEmployee(ee);
                    if (k > 0)
                    {
                        await DisplayAlert("Okay..", "Updated " + k + " Records", "Okay");
                    }

                    await DisplayAlert("Okay..", "Succesfully Imported to SQLITE ", "Okay");
                    await Navigation.PopAsync();

                }
                catch (Exception ex)
                {

                    await DisplayAlert("Reminder..", ex.Message, "Okay");

                }
                finally
                {
                    await PopupNavigation.Instance.PopAsync(true);

                }
            }
         

        }
    }
}
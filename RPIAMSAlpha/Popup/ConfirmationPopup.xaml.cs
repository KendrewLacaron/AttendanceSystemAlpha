using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha.Popup
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfirmationPopup : PopupPage
	{
        public string ConfirmMessage;
        public ConfirmDeleteEnums ConfirmEnum;
        public DateTime Sdt, Edt;

		public ConfirmationPopup ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            lblConfirmText.Text = ConfirmMessage;
        }

        private void TxtConfirmDelete_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtConfirmDelete.Text.Equals(lblConfirmText.Text))
            {
                btnConfirmDelete.IsEnabled = true;
            }
            else
            {
                btnConfirmDelete.IsEnabled = false;
            }
        }

        private async void BtnConfirmDelete_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopup());
            try
            {
                if (ConfirmEnum == ConfirmDeleteEnums.Tally)
                {
                    await App.EmployeeDatabase.ResetAllTimelogs(Sdt,Edt);
                }
                else if (ConfirmEnum == ConfirmDeleteEnums.Untally)
                {
                    await App.EmployeeDatabase.ResetAllTimelogs(Sdt, Edt);
                }
                else if (ConfirmEnum == ConfirmDeleteEnums.Exported)
                {
                    await App.EmployeeDatabase.ResetAllTimelogs();
                }
                else if (ConfirmEnum == ConfirmDeleteEnums.All)
                {
                    await App.EmployeeDatabase.ResetAll();
                }

                await DisplayAlert("Alert ...", "Data Cleared", "Okay");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert ...", ex.Message, "Okay");
            }
            finally
            {
                await PopupNavigation.Instance.PopAsync();
                await PopupNavigation.Instance.PopAsync();
            }
        }
    }
}
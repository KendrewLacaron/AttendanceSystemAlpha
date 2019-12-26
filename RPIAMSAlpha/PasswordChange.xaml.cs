using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPIAMSAlpha.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordChange : ContentPage
	{
		public PasswordChange ()
		{
			InitializeComponent ();
		}

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            User us = await App.EmployeeDatabase.GetUser();
            if(us.userPass == txtOldPass.Text)
            {
                User user = new User();
                user.userPass = txtNewPass.Text;
                user.userID = us.userID;
                user.userName = us.userName;
                await App.EmployeeDatabase.SaveUserAsync(user);

                await DisplayAlert("Baka..", "Password Succesfully Changed", "Yosh");
                await Navigation.PopAsync(); 

            }
            else
            {
                await DisplayAlert("Baka..", "Incorrect Password", "Yosh");
            }

        }

        private void TxtConfirmPass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue == txtNewPass.Text)
            {
                lblWarning.Text = " ";
                btnSubmit.IsEnabled = true;
            }

            else
            {
                lblWarning.Text = "Password Does Not Match";
                btnSubmit.IsEnabled = false;
            }
        }

        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TxtNewPass_Focused(object sender, FocusEventArgs e)
        {
            txtConfirmPass.Text = string.Empty;
        }
    }
}
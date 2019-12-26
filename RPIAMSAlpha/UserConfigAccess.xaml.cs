using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserConfigAccess : ContentPage
	{
		public UserConfigAccess ()
		{
			InitializeComponent ();
		}

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            var user = await App.EmployeeDatabase.GetUser();

            if (string.Equals( txtUser.Text,user.userName,StringComparison.OrdinalIgnoreCase) && string.Equals( txtPass.Text ,user.userPass,StringComparison.OrdinalIgnoreCase))
            {
                await Navigation.PushAsync(new ConfigTab());
            }
            else
            {
                //await Navigation.PushAsync(new ConfigTab());
                await DisplayAlert("Baka..", "Incorrect Username or Password", "Ara ara");
            }
        }

        private void BtnChangePass_Clicked(object sender, EventArgs e)
        {
            //DisplayAlert("Baka..", "Password cant be changed right now ", "Ara ara");
             Navigation.PushAsync(new PasswordChange());
        }

        private void BtnAddUser_Clicked(object sender, EventArgs e)
        {

            //App.EmployeeDatabase.AddUser();
            Navigation.PopToRootAsync();
        }
    }
}
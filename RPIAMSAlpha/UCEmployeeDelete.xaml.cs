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
	public partial class UCEmployeeDelete : ContentPage
	{
		public UCEmployeeDelete ()
		{
			InitializeComponent ();
            BindingContext = new Employee();
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            if (txtEmpCode.Text != null && txtEmpCode.Text != null)
            {
                DeleteEmployee();
                await DisplayAlert("Yosh...", "Employee Deleted", "Yosh");
            }
            else
            {
                await DisplayAlert("Baka...", "Complete All Fields", "Oh! No!");
            }
        }

        private async void DeleteEmployee()
        {
            var employee = (Employee)BindingContext;
            await App.EmployeeDatabase.DeleteEmployeeAsync(employee);

            await Navigation.PopToRootAsync();
        }
    }
}
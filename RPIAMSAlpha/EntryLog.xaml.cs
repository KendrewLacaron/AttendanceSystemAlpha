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
	public partial class EntryLog : ContentPage
	{
		public EntryLog ()
		{
			InitializeComponent ();
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtEmpId = -1;
            var e =  App.EmployeeDatabase.GetEmployeeTodayLogAsync();
            listView.ItemsSource = e;
        }

        private  void ToolbarItem_Clicked(object sender, EventArgs e)
        {
             Navigation.PopAsync();
        }
    }
}
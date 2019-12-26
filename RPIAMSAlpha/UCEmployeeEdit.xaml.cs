using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPIAMSAlpha.Data;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UCEmployeeEdit : ContentPage
	{
		public UCEmployeeEdit ()
		{
			InitializeComponent ();
            
		}

        protected override async void OnAppearing()
        {
            
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtEmpId = -1;

            var e = await App.EmployeeDatabase.GetEmployeeAsync();
            listView.ItemsSource = e;
            
        }


        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //((App)App.Current).ResumeAtTodoId = (e.SelectedItem as TodoItem).ID;
            //Debug.WriteLine("setting ResumeAtTodoId = " + (e.SelectedItem as TodoItem).ID);
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new UCEmployeeAdd{BindingContext = e.SelectedItem as Employee} ) ;
            }
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                listView.ItemsSource = await App.EmployeeDatabase.GetEmployeeAsync();
            }
            else
            {
                List<Employee> ls;
                ls = await App.EmployeeDatabase.GetEmployeeAsync();
                listView.ItemsSource = ls.Where(x => x.EmpName.Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase) || x.EmpCode.ToString().Contains(e.NewTextValue, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}   
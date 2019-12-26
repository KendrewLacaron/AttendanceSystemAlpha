using Rg.Plugins.Popup.Pages;
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
	public partial class ExportReportLog : PopupPage
	{
		public ExportReportLog ()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //var i = await App.EmployeeDatabase.GetTBLMAIN1List();
            var i = await App.EmployeeDatabase.GetReportsList(Enums.ExportReportEnums.All);

            if (i == null)
            {

            }
            else
            {
                expLogList.ItemsSource = i;
            }
        }
    }
}
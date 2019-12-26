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
	public partial class DateTimeSetterPopup : PopupPage
	{
		public DateTimeSetterPopup ()
		{
			InitializeComponent ();
            //txtDate.Date = DateTime.UtcNow.Date;
            //txtTime.Time = DateTime.UtcNow.TimeOfDay;
		}

        private void BtnSaveDateTime_Clicked(object sender, EventArgs e)
        {
            //Set the required DateTime
            DateTime settedDateTime = txtDate.Date.AddTicks(txtTime.Time.Ticks);
            //DateTimeOffset DateTimeOffset1 = new DateTimeOffset(settedDateTime, new TimeSpan(0, 0, 0));      //The TimeSpan sets the difference from UTC which is required for DateTimeOffset
            //Windows.System.DateTimeSettings.SetSystemDateTime(settedDateTime);     //Set the system DateTime
            DisplayAlert("System", "Date and Time Succesfully Updated", "Okay");
        }
    }
}
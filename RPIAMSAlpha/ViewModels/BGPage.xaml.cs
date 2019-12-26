using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha.ViewModels
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BGPage : ContentPage
	{
		public BGPage ()
		{
			InitializeComponent ();
            BindingContext = new ViewModels.SecondPagViewModel();
        }
	}
}
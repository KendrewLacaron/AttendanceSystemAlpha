using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RPIAMSAlpha.ViewModels
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SecondPagViewModel : INotifyPropertyChanged
	{
        private string logtype;
        public string LogType
        {
            get { return logtype; }
            set
            {
                logtype = value;
                OnPropertyChanged("LogType");
            }
        }

        public ICommand WinterSelected { get; private set; }
        public ICommand SpringSelected { get; private set; }
        public ICommand SummerSelected { get; private set; }
        public ICommand AutumnSelected { get; private set; }

        public SecondPagViewModel()
        {
            WinterSelected = new Command(SetTIAM);
            SpringSelected = new Command(SetTOAM);
            SummerSelected = new Command(SetTIPM);
            AutumnSelected = new Command(SetTOPM);
        }


        void SetTIAM()
        {
            LogType = "Time-in AM";
        }

        void SetTOAM()
        {
            LogType = "Time-out AM";
        }

        void SetTIPM()
        {
            LogType = "Time-In PM";
        }

        void SetTOPM()
        {
            LogType = "Time-out PM";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName = "") =>
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

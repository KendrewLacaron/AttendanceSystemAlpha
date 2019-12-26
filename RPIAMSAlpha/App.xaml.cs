using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RPIAMSAlpha.Database;
using RPIAMSAlpha.Enums;
using System.IO;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RPIAMSAlpha
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }

        static EmployeeDatabase employeeDatabase;
        static ExportReportEnums exportEnums;


        public App()
        {
            InitializeComponent();
            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            
            MainPage = new NavigationPage(new MainPage());
        }

        public static EmployeeDatabase EmployeeDatabase
        {
            get
            {
                if (employeeDatabase == null)
                {

                    //var d = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Avatars");
                    //Directory.CreateDirectory(d);
                    employeeDatabase = new EmployeeDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RPIAMSSQLite.db3"));
                    EmployeeDatabase.AddUser();
                }
                return employeeDatabase;
            }
        }

        public static ExportReportEnums ExportRepEnums
        {
            get
            {
                return exportEnums;
            }
        }

        public int ResumeAtEmpId { get; set; }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

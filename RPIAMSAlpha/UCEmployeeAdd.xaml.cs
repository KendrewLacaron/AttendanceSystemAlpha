using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RPIAMSAlpha.Data;
using RPIAMSAlpha.Database;
using System.IO;
//using FlexCodeSDK;

namespace RPIAMSAlpha
{
	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UCEmployeeAdd : ContentPage
	{
        //FinFPReg reg;
        string template;
        static string[] p = {"Avatars/boy.png","Avatars/boy1.png","Avatars/girl.png","Avatars/girl1.png","Avatars/man.png","Avatars/man1.png",
                      "Avatars/man2.png","Avatars/man3.png","Avatars/man5.png"};
        int w = 0;
        string[] q = Device.RuntimePlatform == Device.Android ? p : Directory.GetFiles("Avatars\\");
        Random r = new Random();

        public UCEmployeeAdd()
        {
            InitializeComponent();
            BindingContext = new Employee();
            //InitialFlexCode();
        }
       

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if(txtEmpCode.Text != null && txtEmpCode.Text !=null && empAvat.Source != null && txtEmpCode.Text.Length == 10)
            {
                Employee j = await App.EmployeeDatabase.GetEmployeeByRFID(txtEmpCode.Text);
                if ( j  != null)
                {
                    empID.Text = j.EmpID.ToString();
                    bool answer = await DisplayAlert("Reminder...", "Existing Employee \r\nSave?" , "Yes", "No");
                    if (answer)
                    {
                        //byte[] imageData = ReadFile(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp");
                        //reg.FPRegistrationStart("MySecretKey" + txtEmpCode.Text);
                        TransactSave();
                        await DisplayAlert("Great...", "Employee Updated", "Sure");
                        await Navigation.PopAsync();


                    }
                }
                else
                {
                    //byte[] imageData = ReadFile(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp");
                    //reg.FPRegistrationStart("MySecretKey" + txtEmpCode.Text);
                    TransactSave();
                    await DisplayAlert("Great...", "Employee Saved", "Sure");
                }

            }
            else
            {
                await DisplayAlert("Reminder...", "Complete and Check All Fields ", "Oh! No!");
            }

        }
        private async void TransactSave()
        {
            var employee = (Employee)BindingContext;
            employee.empAvat = q[w];
            employee.EmpRFID = txtEmpCode.Text;
            employee.EmpTIAM = cboTIAM.Time;
            employee.EmpTOAM = cboTOAM.Time;
            employee.template = template;
            await App.EmployeeDatabase.SaveEmployeeAsync(employee);
            ClearScreen();
        }

        public void ClearScreen()
        {
            txtEmpCode.Text = string.Empty;
            txtEmpName.Text = string.Empty;
            lblTIAM.Text = string.Empty;
            lblTOAM.Text = string.Empty;
        }
        async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        public async void btnTest(object sender, EventArgs e)
        {

        
            if (w < q.Length-1)
            {
                w++;
                empAvat.Source = q[w];
            
            }
            else
            {
                w = 0;
            }

          
        }

        private void ContentPage_SizeChanged(object sender, EventArgs e)
        {
            //bool isPortrait = this.Height > this.Width;
            //stackLayout.Orientation = (isPortrait ? StackOrientation.Vertical : StackOrientation.Horizontal);
        }



     
        //private void InitialFlexCode()
        //{
        //    //Initialize FlexCodeSDK for Registration
        //    //1. Initialize Event Handler
        //    reg = new FlexCodeSDK.FinFPReg();
        //    reg.FPSamplesNeeded += new __FinFPReg_FPSamplesNeededEventHandler(reg_FPSamplesNeeded);
        //    reg.FPRegistrationTemplate += new __FinFPReg_FPRegistrationTemplateEventHandler(reg_FPRegistrationTemplate);
        //    reg.FPRegistrationImage += new __FinFPReg_FPRegistrationImageEventHandler(reg_FPRegistrationImage);
        //    reg.FPRegistrationStatus += new __FinFPReg_FPRegistrationStatusEventHandler(reg_FPRegistrationStatus);

        //    //2. Input the activation code
        //    reg.AddDeviceInfo("GX00E020313", "348A39607B1DF06", "T8SCA16CEAE3220F33B98EJQ");
        //    //3. Define fingerprint image
        //    reg.PictureSampleHeight = (short)(imgFinger.Height * 15); //FlexCodeSDK use Twips. 1 pixel = 15 twips
        //    reg.PictureSampleWidth = (short)(imgFinger.Width * 15); //FlexCodeSDK use Twips. 1 pixel = 15 twips
        //    reg.PictureSamplePath = AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp";

        //}

        //void reg_FPRegistrationImage()
        //{
        //    FileStream fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp", FileMode.Open, FileAccess.Read);
        //    imgFinger.Source = ImageSource.FromStream(() => new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp", FileMode.Open, FileAccess.Read));
        //    fs.Close();
        //}


        //void reg_FPRegistrationTemplate(string FPTemplate)
        //{
        //     template = FPTemplate;
        //}

        //async void reg_FPSamplesNeeded(short Samples)
        //{
        //    await DisplayAlert( "Samples Needed : ",Convert.ToString(Samples),"Hai hai");
        //}

        //void reg_FPRegistrationStatus(RegistrationStatus Status)
        //{
        //    if (Status == RegistrationStatus.r_OK)
        //    {
        //        TransactSave();
        //    }
        //}

        byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes to read from file.
            //In this case we want to read entire file. So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);
            fStream.Close();
            return data;
        }


    }
}
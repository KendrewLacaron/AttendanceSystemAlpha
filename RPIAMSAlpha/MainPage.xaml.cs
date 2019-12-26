using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RPIAMSAlpha.Data;
using SkiaSharp;
using System.Reflection;
using System.IO;
using SkiaSharp.Views.Forms;
//using FlexCodeSDK;
//using Windows.Storage;
using System.Diagnostics;
//using Windows.System;
//using Windows.Web;
//using Windows;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;

namespace RPIAMSAlpha
{
    public partial class MainPage : ContentPage
    {
        //DispatcherTimer timer = new DispatcherTimer();
        bool boolMenuVisible = false;
        //Image File;
        string strImagePath = null;
        //button press = 1=btnTIAM, 2=btnTOAM, 3=btnTIPM, 4=btnTOPMa
        TimeSpan expTime;

        int strButtonpressed , resetter;
        string strDocNum, strEmpCode , devCode;
        string sqlstatement;
        //FinFPVer ver;

        //---------------------------------------------------------

        SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };

        SKPaint whiteStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Round,
            IsAntialias = true
        };

        SKPaint whiteFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White
        };

        SKPaint greenFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.PaleGreen
        };

        SKPaint blackStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
            StrokeWidth = 20,
            StrokeCap = SKStrokeCap.Round
        };

        SKPaint grayFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };

        SKPaint backgroundFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill
        };

        SKPath catEarPath = new SKPath();
        SKPath catEyePath = new SKPath();
        SKPath catPupilPath = new SKPath();
        SKPath catTailPath = new SKPath();

        SKPath hourHandPath = SKPath.ParseSvgPathData(
            "M 0 -60 C 0 -30 20 -30 5 -20 L 5 0 C 5 7.5 -5 7.5 -5 0 L -5 -20 C -20 -30 0 -30 0 -60");
        SKPath minuteHandPath = SKPath.ParseSvgPathData(
            "M 0 -80 C 0 -75 0 -70 2.5 -60 L 2.5 0 C 2.5 5 -2.5 5 -2.5 0 L -2.5 -60 C 0 -70 0 -75 0 -80");

 

        private void canvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.DrawPaint(backgroundFillPaint);

            int width = e.Info.Width;
            int height = e.Info.Height;

            // Set transforms
            canvas.Translate(width / 2, height / 2);
            canvas.Scale(Math.Min(width / 210f, height / 520f));

            // Get DateTime
            DateTime dateTime = DateTime.Now;

            //// Head
            canvas.DrawCircle(0, -160, 75, blackFillPaint);

            // Draw ears and eyes
            //for (int i = 0; i < 2; i++)
            //{
            //    canvas.Save();
            //    canvas.Scale(2 * i - 1, 1);

            //    canvas.Save();
            //    canvas.Translate(-65, -255);
            //    canvas.DrawPath(catEarPath, blackFillPaint);
            //    canvas.Restore();

            //    canvas.Save();
            //    canvas.Translate(10, -170);
            //    canvas.DrawPath(catEyePath, greenFillPaint);
            //    canvas.DrawPath(catPupilPath, blackFillPaint);
            //    canvas.Restore();

            //    // Draw whiskers
            //    canvas.DrawLine(10, -120, 100, -100, whiteStrokePaint);
            //    canvas.DrawLine(10, -125, 100, -120, whiteStrokePaint);
            //    canvas.DrawLine(10, -130, 100, -140, whiteStrokePaint);
            //    canvas.DrawLine(10, -135, 100, -160, whiteStrokePaint);

            //    canvas.Restore();
            //}

        // Move Tail
        float t = (float)Math.Sin((dateTime.Second % 2 + dateTime.Millisecond / 1000.0) * Math.PI);
            //catTailPath.Reset();
            //catTailPath.MoveTo(0, 100);
            //SKPoint point1 = new SKPoint(-50 * t, 200);
            //SKPoint point2 = new SKPoint(0, 250 - Math.Abs(50 * t));
            //SKPoint point3 = new SKPoint(50 * t, 250 - Math.Abs(75 * t));
            //catTailPath.CubicTo(point1, point2, point3);

            canvas.DrawPath(catTailPath, blackStrokePaint);

            // Clock background
            canvas.DrawCircle(0, 0, 100, blackFillPaint);

            // Hour and minute marks
            for (int angle = 0; angle < 360; angle += 6)
            {
                canvas.DrawCircle(0, -90, angle % 30 == 0 ? 4 : 2, whiteFillPaint);
                canvas.RotateDegrees(6);
            }

            // Hour hand
            canvas.Save();
            canvas.RotateDegrees(30 * dateTime.Hour + dateTime.Minute / 2f);
            canvas.DrawPath(hourHandPath, grayFillPaint);
            canvas.DrawPath(hourHandPath, whiteStrokePaint);
            canvas.Restore();

            // Minute hand
            canvas.Save();
            canvas.RotateDegrees(6 * dateTime.Minute + dateTime.Second / 10f);
            canvas.DrawPath(minuteHandPath, grayFillPaint);
            canvas.DrawPath(minuteHandPath, whiteStrokePaint);
            canvas.Restore();

            // Second hand
            canvas.Save();
            float seconds = dateTime.Second + dateTime.Millisecond / 1000f;
            canvas.RotateDegrees(6 * seconds);
            whiteStrokePaint.StrokeWidth = 2;
            canvas.DrawLine(0, 10, 0, -80, whiteStrokePaint);
            canvas.Restore();
        }
        // -----------Clock ---------------------------------------

        struct HandParams
        {
            public HandParams(double width, double height, double offset) : this()
            {
                Width = width;
                Height = height;
                Offset = offset;
            }

            public double Width { private set; get; }   // fraction of radius  
            public double Height { private set; get; }  // ditto  
            public double Offset { private set; get; }  // relative to center pivot  
        }

        static readonly HandParams secondParams = new HandParams(0.02, 1.1, 0.85);
        static readonly HandParams minuteParams = new HandParams(0.05, 0.8, 0.9);
        static readonly HandParams hourParams = new HandParams(0.125, 0.65, 0.9);

        BoxView[] tickMarks = new BoxView[60];

        // ----------------------------------------------------
            
        public MainPage()
        {
            InitializeComponent();
            //StartVerify();

            //btnReset.Clicked += btnReset_Click;
            //btnTIAM.BackgroundColor = Color.PaleVioletRed;
            //btnTIAM.Clicked += btnTIAM_Click;
            //btnTOAM.Clicked += btnTOAM_Click;
            //btnTIPM.Clicked += btnTIPM_Click;
            //btnTOPM.Clicked += btnTOPM_Click;
            btnConfig.Clicked += lblSetup_Click;
            txtSwipeCard.Focus();
            btnLog.Clicked += BtnLog_Clicked;
            //btnRefresh.Clicked += BtnRefresh_Clicked;
            btnExportLog.Clicked += BtnShowIP_CLICKED;

            //TICK  <---------------------------------------------------------------------------------<<

            //// Make cat ear path
            //catEarPath.MoveTo(0, 0);
            //catEarPath.LineTo(0, 75);
            //catEarPath.LineTo(100, 75);
            //catEarPath.Close();

            //// Make cat eye path
            //catEyePath.MoveTo(0, 0);
            //catEyePath.ArcTo(50, 50, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 50, 0);
            //catEyePath.ArcTo(50, 50, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 0, 0);
            //catEyePath.Close();

            //// Make eye pupil path
            //catPupilPath.MoveTo(25, -5);
            //catPupilPath.ArcTo(6, 6, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 25, 5);
            //catPupilPath.ArcTo(6, 6, 0, SKPathArcSize.Small, SKPathDirection.Clockwise, 25, -5);
            //catPupilPath.Close();

            //// Make cat tail path
            //catTailPath.MoveTo(0, 100);
            //catTailPath.CubicTo(50, 200, 0, 250, -50, 200);

            // Create Shader
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using (Stream stream = assembly.GetManifestResourceStream("RPIAMSOne.WoodGrain.png"))
            //using (SKManagedStream skStream = new SKManagedStream(stream))
            //using (SKBitmap bitmap = SKBitmap.Decode(skStream))
            //using (SKShader shader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Mirror, SKShaderTileMode.Mirror))
            //{
            //    backgroundFillPaint.Shader = shader;
            //}

            Device.StartTimer(TimeSpan.FromSeconds(1f / 60), () =>
            {
                canvasView.InvalidateSurface();
                return true;
            });
            //for (int i = 0; i < tickMarks.Length; i++)
            //{
            //    tickMarks[i] = new BoxView { Color = Color.Black };
            //    absoluteLayout.Children.Add(tickMarks[i]);
            //}

            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 60), OnTimerTick);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                lblTime.Text = DateTime.Now.ToString("hh:mm tt");
                lblDate.Text = DateTime.Now.ToString("yyyy/MM/dd");

                if(DateTime.Now.ToShortTimeString().Equals("12:00 AM"/*DateTime.Today.AddTicks(expTime.Ticks).AddTicks(TimeSpan.TicksPerMinute * 2).ToShortTimeString()*/))
                {
                    App.EmployeeDatabase.ExportResetter = 0;
                 
                }

                if (DateTime.Now.ToShortTimeString().Equals(DateTime.Today.AddTicks(expTime.Ticks).ToShortTimeString()) && App.EmployeeDatabase.ExportResetter == 0)
                {
                    App.EmployeeDatabase.ExportAllRecords();
                    App.EmployeeDatabase.ExportResetter = 1;
                }
                return true; // True = Repeat again, False = Stop the timer
            });

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            txtSwipeCard.Focus();

            var a = await App.EmployeeDatabase.GetLatestPref();

            if (a == null)
            {
                lblDevType.Text = "SETUP DEVICE FIRST";
                txtSwipeCard.IsEnabled = false;
            }
            else
            {
                if (a.PrefStrType == 1)
                {
                    lblDevType.Text = "TIME-IN";
                }
                else
                {
                    lblDevType.Text = "TIME-OUT";
                }
                strButtonpressed = a.PrefStrType;
                devCode = a.PrefDevCode;
                expTime = a.PrefExpTime;
                txtSwipeCard.IsEnabled = true;
            }
            txtSwipeCard.Focus();
        }
        //public async System.Threading.Tasks.Task<string> CheckFingerprintAvailability()
        //{
        //    string returnMessage = "";

        //    try
        //    {
        //        // Check the availability of fingerprint authentication.
        //        var ucvAvailability = await Windows.Security.Credentials.UI.UserConsentVerifier.CheckAvailabilityAsync();

        //        switch (ucvAvailability)
        //        {
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.Available:
        //                returnMessage = "Fingerprint verification is available.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceBusy:
        //                returnMessage = "Biometric device is busy.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DeviceNotPresent:
        //                returnMessage = "No biometric device found.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.DisabledByPolicy:
        //                returnMessage = "Biometric verification is disabled by policy.";
        //                break;
        //            case Windows.Security.Credentials.UI.UserConsentVerifierAvailability.NotConfiguredForUser:
        //                returnMessage = "The user has no fingerprints registered. Please add a fingerprint to the " +
        //                                "fingerprint database and try again.";
        //                break;
        //            default:
        //                returnMessage = "Fingerprints verification is currently unavailable.";
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        returnMessage = "Fingerprint authentication availability check failed: " + ex.ToString();
        //    }

        //    return returnMessage;
        //}

        private async void BtnLog_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntryLog());
        }

        //---------------------------------START OF CLOCK -----------------------------------

        void MainPageSizeChanged(object sender, EventArgs e)
        {
            bool isPortrait = this.Height > this.Width;
            stackLayout.Orientation = (isPortrait ? StackOrientation.Vertical : StackOrientation.Horizontal);

        }

      
        void LayoutHand(BoxView boxView, HandParams handParams, Point center, double radius)
        {
            double width = handParams.Width * radius;
            double height = handParams.Height * radius;
            double offset = handParams.Offset;

            AbsoluteLayout.SetLayoutBounds(boxView,
                new Rectangle(center.X - 0.5 * width,
                              center.Y - offset * height,
                              width, height));

            // Set the AnchorY property for rotations.  
            boxView.AnchorY = handParams.Offset;
        }

        bool OnTimerTick()
        {
            // Set rotation angles for hour and minute hands.  
            DateTime dateTime = DateTime.Now;
            //hourHand.Rotation = 30 * (dateTime.Hour % 12) + 0.5 * dateTime.Minute;
            //minuteHand.Rotation = 6 * dateTime.Minute + 0.1 * dateTime.Second;

            // Do an animation for the second hand.  
            double t = dateTime.Millisecond / 1000.0;

            if (t < 0.5)
            {
                t = 0.5 * Easing.SpringIn.Ease(t / 0.5);
            }
            else
            {
                t = 0.5 * (1 + Easing.SpringOut.Ease((t - 0.5) / 0.5));
            }

            //secondHand.Rotation = 6 * (dateTime.Second + t);
            return true;
        }

        //----------------------------------END OF CLOCK -------------------------------

        string inf = "";
        private string pristrEmpCode;

        private async void lblSetup_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserConfigAccess());
            //this.Frame.Navigate(typeof(frmLogin));
        }

        private async void BtnRefresh_Clicked(object sender, EventArgs e)
        {
            await App.EmployeeDatabase.ResetAllTimelogs();
            //await PopupNavigation.Instance.PushAsync(new DateTimeSetterPopup());
        }

        private async void BtnShowIP_CLICKED(object sender, EventArgs e)
        {
            //await DisplayAlert("Alert..", DateTime.Today.AddTicks( expTime.Ticks).ToShortTimeString(), "Noice");
            //await DisplayAlert("Alert..", DateTime.Now.ToShortTimeString(), "Noice");
            await PopupNavigation.Instance.PushAsync(new ExportReportLog());
            //var a = await App.EmployeeDatabase.GetLatestConfig();
            //var p = await App.EmployeeDatabase.GetEmployeeByCodeAsyncRegardless("00001");
            //if (a == null)
            //{

            //}
            //else
            //{
            //    await DisplayAlert("Nice...", a.ConfigIP + "" + a.ConfigPort, "Okay");
            //}

            //await DisplayAlert("Nice...", p.EmpTIAM.ToString() + "" + p.EmpTOAM.ToString(), "Okay");

        }

        private async void btnReset_Click(object sender, EventArgs e)
        {
            txtSwipeCard.Text = string.Empty;
           
            ClearStats();
            //await DisplayAlert("Reminder..", await CheckFingerprintAvailability(), "Alright");
            //this.Frame.Navigate(typeof(frmLogin));
        }



        //private void btnTIAM_Click(object sender, EventArgs e)
        //{
        //    strButtonpressed = 1;
        //    btnTIAM.BackgroundColor = Color.PaleVioletRed;
        //    btnTOAM.BackgroundColor = Color.FloralWhite;
        //    btnTIPM.BackgroundColor = Color.FloralWhite;
        //    btnTOPM.BackgroundColor = Color.FloralWhite;
        //    //stackBG.BackgroundColor = Color.LightGoldenrodYellow;
        //    txtSwipeCard.Focus();
        //}

        //private void btnTOAM_Click(object sender, EventArgs e)
        //{
        //    strButtonpressed = 2;
        //    btnTIAM.BackgroundColor = Color.FloralWhite;
        //    btnTOAM.BackgroundColor = Color.PaleVioletRed;
        //    btnTIPM.BackgroundColor = Color.FloralWhite;
        //    btnTOPM.BackgroundColor = Color.FloralWhite;
        //    txtSwipeCard.Focus();

        //    //stackBG.BackgroundColor = Color.Yellow;

        //}

        //private void btnTIPM_Click(object sender, EventArgs e)
        //{
        //    strButtonpressed = 3;
        //    btnTIAM.BackgroundColor = Color.FloralWhite;
        //    btnTOAM.BackgroundColor = Color.FloralWhite;
        //    btnTIPM.BackgroundColor = Color.PaleVioletRed;
        //    btnTOPM.BackgroundColor = Color.FloralWhite;
        //    txtSwipeCard.Focus();

        //    //stackBG.BackgroundColor = Color.Orange;

        //}

        //private void btnTOPM_Click(object sender, EventArgs e)
        //{
        //    strButtonpressed = 4;
        //    btnTIAM.BackgroundColor = Color.FloralWhite;
        //    btnTOAM.BackgroundColor = Color.FloralWhite;
        //    btnTIPM.BackgroundColor = Color.FloralWhite;
        //    btnTOPM.BackgroundColor = Color.PaleVioletRed;
        //    txtSwipeCard.Focus();

        //    //stackBG.BackgroundColor = Color.OrangeRed;

        //}
        private void Loglabel()
        {
            if (strButtonpressed == 1)
            {
                txtLogTimeLabel.Text = "Time-IN";
            }
            else if (strButtonpressed == 2)
            {
                txtLogTimeLabel.Text = "Time-OUT";
            }
            else if (strButtonpressed == 3)
            {
                txtLogTimeLabel.Text = "Afternoon Time-In";
            }
            else if (strButtonpressed == 4)
            {
                txtLogTimeLabel.Text = "Afternoon Time-Out";
            }
        }

        private async void ShowStats()
        {
            TimeSpan Ts1 = new TimeSpan();
            Ts1 = TimeSpan.Parse(Convert.ToDateTime(lblTime.Text).ToString("HH:mm:ss"));
            double a = 0;
            Employee emp = await App.EmployeeDatabase.GetEmployeeByCodeAsync(txtSwipeCard.Text);
            if (strButtonpressed == 1)
            {
                 a = emp.EmpTIAM.Subtract(Ts1).TotalMinutes;
                //    txtAcctLogTimeStat.Text = DateTime.Today.AddTicks(emp.EmpTIAM.Ticks).ToShortTimeString();
                //    txtLogTimeLabelStat.Text = "Morning Time-In";
            }
            if (strButtonpressed == 2)
            {
                a = emp.EmpTOAM.Subtract(Ts1).TotalMinutes;
                //txtAcctLogTimeStat.Text = DateTime.Today.AddTicks(emp.EmpTOAM.Ticks).ToShortTimeString();
                //txtLogTimeLabelStat.Text = "Morning Time-Out";

            }
            if (strButtonpressed == 3)
            {
                a = emp.EmpTIPM.Subtract(Ts1).TotalMinutes;
                    //    txtAcctLogTimeStat.Text = DateTime.Today.AddTicks(emp.EmpTIPM.Ticks).ToShortTimeString();
                    //    txtLogTimeLabelStat.Text = "Afternoon Time-In";
                }
                if (strButtonpressed == 4)
            {
                a = emp.EmpTOPM.Subtract(Ts1).TotalMinutes;
                //txtAcctLogTimeStat.Text = DateTime.Today.AddTicks(emp.EmpTOPM.Ticks).ToShortTimeString();
                //txtLogTimeLabelStat.Text = "Afternoon Time-Out";
            }
            //txtLogTimeStat.Text = lblTime.Text;
            if(a < 0)
            {
                a *= -1;
                if(strButtonpressed == 1 || strButtonpressed == 3)
                {
                    inf = " Minutes Late";
                }
                if(strButtonpressed ==2 || strButtonpressed == 4)
                {
                    inf = " Minutes Over";
                }
            }
            else
            {
                if (strButtonpressed == 1 || strButtonpressed == 3)
                {
                    inf = " Minutes Early";
                }
                if (strButtonpressed == 2 || strButtonpressed == 4)
                {
                    inf = " Minutes Advance";
                }
            }
            //txtDifference.Text = a.ToString() + inf;
        }
        private async void TransactSave()
        {
            TimeSpan Ts = new TimeSpan();
            Ts = TimeSpan.Parse( Convert.ToDateTime( lblTime.Text).ToString("HH:mm:ss"));
            Employee emp = await App.EmployeeDatabase.GetEmployeeByRFID(txtSwipeCard.Text);
            //if(emp == null)
            //{
            //    await DisplayAlert( "No User Exist","","ok");
            //}
            //else
            //{
                var tblmain = new TblMain1();
                tblmain.EmpCode = emp.EmpCode;
                tblmain.EmpRFID = emp.EmpRFID;
                tblmain.TimeLog = Ts;
                tblmain.TDate = DateTime.Today;
                tblmain.LogTypeCode = strButtonpressed;
                tblmain.DevCode = devCode;
                tblmain.LogTypeInfo = txtLogTimeLabel.Text;
                await App.EmployeeDatabase.SaveScanData(tblmain);
            //}
           
           

        }

        private async void TransactSaveAnother()
        {
            var timeLog1 = new TimeLogs();
            Employee emp = await App.EmployeeDatabase.GetEmployeeByRFID(txtSwipeCard.Text);

            timeLog1.EmpRFID = emp.EmpRFID;
            timeLog1.EmpCode = emp.EmpCode;
            timeLog1.TDate = DateTime.Today;
            TimeSpan Ts = new TimeSpan();
            Ts = TimeSpan.Parse(Convert.ToDateTime(lblTime.Text).ToString("HH:mm:ss"));
            double a;
            if (strButtonpressed == 1)
            {
                a = emp.EmpTIAM.Subtract(Ts).TotalMinutes;
                timeLog1.TIAM = Ts;
                timeLog1.TOAM = TimeSpan.Zero;
                timeLog1.TIPM = TimeSpan.Zero;
                timeLog1.TOPM = TimeSpan.Zero;
                if(a < 0)
                {
                    timeLog1.AMLate = a*-1;
                }
            }
                
            if (strButtonpressed == 2)
            {
                a = emp.EmpTOAM.Subtract(Ts).TotalMinutes;
                timeLog1.TIAM = TimeSpan.Zero;
                timeLog1.TIPM = TimeSpan.Zero;
                timeLog1.TOPM = TimeSpan.Zero;
                timeLog1.TOAM = Ts;
                if (a > 0)
                {
                    timeLog1.AMUnder = a;
                }
            }
               
            if (strButtonpressed == 3)
            {
                a = emp.EmpTIPM.Subtract(Ts).TotalMinutes;

                timeLog1.TOAM = TimeSpan.Zero;
                timeLog1.TIAM = TimeSpan.Zero;
                timeLog1.TOPM = TimeSpan.Zero;
                timeLog1.TIPM = Ts;

                if (a < 0)
                {
                    timeLog1.PMLate = a * -1;
                }
            }

           
            if (strButtonpressed == 4)
            {
                a = emp.EmpTOPM.Subtract(Ts).TotalMinutes;

                timeLog1.TOAM = TimeSpan.Zero;
                timeLog1.TIPM = TimeSpan.Zero;
                timeLog1.TIAM = TimeSpan.Zero;
                timeLog1.TOPM = Ts;
                if (a > 0)
                {
                    timeLog1.PMUnder = a;
                }
            }
           
            await App.EmployeeDatabase.SaveLogDataAsync(timeLog1,strButtonpressed);

        }

        //private async void DisplayData()
        //{
        //    Loglabel();
        //    txtLogTime.Text = lblTime.Text;
        //    Employee emp = await App.EmployeeDatabase.GetEmployeeByCodeAsync(long.Parse(txtSwipeCard.Text));
        //    txtEmpName.Text = emp.EmpName;

        //}

        private async void OnScan_Swipe(object sender, EventArgs e)
        {
            if (txtSwipeCard.Text.Length == 10)
            {
                txtSwipeCard.Unfocus();
                Employee emp = await App.EmployeeDatabase.GetEmployeeByRFID(txtSwipeCard.Text);
                TblMain1 tbl = await App.EmployeeDatabase.GetTblMainByRFIDListAsync(txtSwipeCard.Text,strButtonpressed);
                if (emp != null)
                {
                    txtEmpName.Text = emp.EmpName;
                    Loglabel();
                    txtLogTime.Text = lblTime.Text;
                    empmAvat.Source = emp.empAvat;
                    if (tbl == null)
                    {
                        //true
                        TransactSave();
                        //ShowStats();
                        TransactSaveAnother();
                        lblMsg.Text = "Successfully Scanned";
                    }
                    else
                    {
                        // data exists
                        ClearStats();
                        
                        lblMsg.Text = "Already Scanned";

                    }


                }
                else
                {
                    ClearStats();
                    lblMsg.Text = "No User Exists";

                }
                txtSwipeCard.Text = string.Empty;
                //await Task.Delay(2000);
                //{
                    txtSwipeCard.Focus();
                //};
                
            }
        }

        public void WaitForFocus()
        {
            try
            {
                Device.StartTimer(TimeSpan.FromDays(3), () =>
                {
                    return false; // True = Repeat again, False = Stop the timer
                });
            }
            finally
            {
                txtSwipeCard.Focus();

            }

        }

        public void ClearStats()
        {
            txtEmpName.Text = string.Empty;
            txtLogTime.Text = string.Empty;
            txtLogTimeLabel.Text = string.Empty;
            empmAvat.Source = string.Empty;
            //txtAcctLogTimeStat.Text = string.Empty;
            //txtLogTimeLabelStat.Text = string.Empty;
            //txtLogTimeStat.Text = string.Empty;
            //txtDifference.Text = string.Empty;
        }


        //private void StartVerify()
        //{
        //    //Initialize FlexCodeSDK for Verification
        //    //1. Initialize Event Handler
        //    ver = new FinFPVer();
        //    ver.FPVerificationID += new __FinFPVer_FPVerificationIDEventHandler(ver_FPVerificationID);
        //    ver.FPVerificationImage += new __FinFPVer_FPVerificationImageEventHandler(ver_FPVerificationImage);
        //    ver.FPVerificationStatus += new __FinFPVer_FPVerificationStatusEventHandler(ver_FPVerificationStatus);

        //    //2. Input the activation code
        //    ver.AddDeviceInfo("GX00E020313", "348A39607B1DF06", "T8SCA16CEAE3220F33B98EJQ");

        //    //3. Define fingerprint image
        //    ver.PictureSampleHeight = (short)(empmAvat.Height * 15); //FlexCodeSDK use Twips. 1 pixel = 15 twips
        //    ver.PictureSampleWidth = (short)(empmAvat.Width * 15); //FlexCodeSDK use Twips. 1 pixel = 15 twips
        //    ver.PictureSamplePath = AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp";

        //    //4. Load templates from database to FlexCodeSDK
      

        //    //5. Start verification process
        //    ver.FPVerificationStart();
        //}

        //void ver_FPVerificationImage()
        //{
        //    //pictureBox1.Load(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp");
        //    FileStream fs = new System.IO.FileStream(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp", FileMode.Open, FileAccess.Read);
        //    empmAvat.Source = ImageSource.FromStream(() => new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Finger.bmp", FileMode.Open, FileAccess.Read));
        //    fs.Close();
        //}

        //void ver_FPVerificationID(string PKNo, FingerNumber FingerNr)
        //{
        //    txtSwipeCard.Text = PKNo;

        //}

        //void ver_FPVerificationStatus(VerificationStatus Status)
        //{
        //    if (Status == VerificationStatus.v_OK)
        //    {

                

        //    }
        //    else if (Status == VerificationStatus.v_NotMatch)
        //    {
               

        //    }
        //}

    }
}

//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RPIAMSAlpha.Database;
using System.Web;
//using OfficeOpenXml.Style;
using RPIAMSAlpha.Views;
using System.ComponentModel;
using System.Drawing;
//using Windows.Storage;
using System.Diagnostics;
//using Windows.System;
//using Windows.Web;
using RPIAMSAlpha.Data;
using Rg.Plugins.Popup.Services;
using RPIAMSAlpha.Popup;

namespace RPIAMSAlpha
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigTab : TabbedPage
    {
        #region App
        int reportType = 1;
        int empType = 1;
        public ConfigTab ()
        {
            InitializeComponent();
            btnDetailed.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            btnEmpType.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            btnSummary.Clicked += BtnSummary_Clicked;
            btnDetailed.Clicked += BtnDetailed_Clicked;
            btnExportRegard.Clicked += BtnExportRegard_Clicked;
        }

        protected async override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();

            var a = await App.EmployeeDatabase.GetLatestPref();
            if ( a == null)
            {

            }
            else
            {
               if(a.PrefStrType == 1)
                {
                    btnTIN.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
                    btnOUT.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
                }
                else
                {
                    btnTIN.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
                    btnOUT.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
                }

                txtDevCode.Text = a.PrefDevCode;
                pickExpTime.Time = a.PrefExpTime;
            }
        }
        #endregion

        #region  Export
        private void BtnDetailed_Clicked(object sender, EventArgs e)
        {
            reportType = 1;

            btnDetailed.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            btnSummary.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
            btnExportRegard.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
        }

        private void BtnSummary_Clicked(object sender, EventArgs e)
        {
            reportType = 2;

            btnDetailed.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
            btnSummary.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            btnExportRegard.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
        }

        private void BtnExportRegard_Clicked(object sender, EventArgs e)
        {
            reportType = 3;

            btnDetailed.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
            btnSummary.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
            btnExportRegard.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
        }

        private async void BtnTally_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopup(), true);
            var a = await App.EmployeeDatabase.GetLatestPref();
            int b = 0 , c = 0;

            try
            {
                 b = await App.EmployeeDatabase.GetCountSQLITEExport(dtSdt.Date , dtEdt.Date);
                 c = await App.EmployeeDatabase.GetCountSQLExport(dtSdt.Date , dtEdt.Date , a.PrefDevCode);

            }
            catch (Exception ex)
            {

                await DisplayAlert("Baka", ex.Message, "Yaro");
                await DisplayAlert("Baka", "No Connection Found", "Yaro");
            }
            finally
            {
                await PopupNavigation.Instance.PopAsync(true);
            }

            bool d = await DisplayAlert("Result ...","Tally Export from " + dtSdt.Date.ToShortDateString() + " to " + dtEdt.Date.ToShortDateString() + "\r\n\r\nNo of Processed Reports : " + b + "\r\n\r\nNo of Received Reports   : " + c,"Clear Records","Cancel");

                if (d)
                {
                    if(b != c)
                    {
                        bool f = await DisplayAlert("Alert", "Processed and Received Reports are Uneven\r\n\r\nContinue? ", "Yes", "No");


                        if (f)
                        {
                            // Execute Payroll
                            await PopupNavigation.Instance.PushAsync(new ConfirmationPopup() { ConfirmMessage = "DELETE UNEVEN TALLY" , ConfirmEnum = Enums.ConfirmDeleteEnums.Untally , Sdt = dtSdt.Date , Edt = dtEdt.Date });
                        }
                    }
                    else
                    {
                        //Execute Payroll
                        await PopupNavigation.Instance.PushAsync(new ConfirmationPopup() { ConfirmMessage = "DELETE TALLY" , ConfirmEnum = Enums.ConfirmDeleteEnums.Tally , Sdt = dtSdt.Date, Edt = dtEdt.Date });
                    }
                }
          


        }

        private async void BtnExport_Clicked(object sender, EventArgs e)
        {

            // Set the file name and get the output directory
            //var fileName = "Example-CRM-" + DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xlsx";
            //string outputDir = @"C:\Users\Kenneth\AppData\New.xlsx";

            //// Create the file using the FileInfo object
            //var file = new FileInfo(outputDir + fileName);

            //OOP();
            if (reportType == 1)
            {
                await Navigation.PushAsync(new ViewReport {  e = App.EmployeeDatabase.GetDetailedReportsBetweenDate(dtSdt.Date, dtEdt.Date) , sdt = dtSdt.Date , edt = dtEdt.Date});

                //using (ExcelPackage pck = new ExcelPackage())
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Detailed Report");
                //    DataTable dt = ConvertToDataTable(App.EmployeeDatabase.GetDetailedReportsBetweenDate(dtSdt.Date, dtEdt.Date));
                //    ExcelRange firstCell = ws.Cells[1,1,3,dt.Columns.Count+4];
                //    firstCell.Merge = true;
                //    firstCell.Value = " Detailed Report From " + dtSdt.Date.ToShortDateString() +" to "+ dtEdt.Date.ToShortDateString() +" ";
                //    firstCell.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //    firstCell.Style.Font.Size = 30;
                //    firstCell.Style.Font.Bold = true;
                //    firstCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    firstCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //    //ExcelRange empCode = ws.Cells[4,1];
                //    //empCode.Merge = true;
                //    //empCode.Value = " EmpCode ";
                //    //empCode.Style.Font.Size = 10;
                //    //empCode.Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    

                //    //ExcelRange empName = ws.Cells[4, 2];
                //    //empCode.Merge = true;
                //    //empCode.Value = " EmpCode ";
                //    //empCode.Style.Font.Size = 10;
                //    //empCode.Style.Border.BorderAround(ExcelBorderStyle.Thick);

                //    //ExcelRange empDate = ws.Cells[4, 3];
                //    //empCode.Merge = true;
                //    //empCode.Value = " EmpCode ";
                //    //empCode.Style.Font.Size = 10;
                //    //empCode.Style.Border.BorderAround(ExcelBorderStyle.Thick);

                //    ExcelRangeBase rs = ws.Cells["A5"].LoadFromDataTable(ConvertToDataTable(App.EmployeeDatabase.GetDetailedReportsBetweenDate(dtSdt.Date, dtEdt.Date)), true);
                  
                //    //ws.Cells[5,1,5,dt.Columns.Count].Clear();
                //    //rs.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                //    ws.Protection.IsProtected = true;
                //    ws.Protection.AllowSelectLockedCells = false;

                //    StorageFolder devices = Windows.Storage.KnownFolders.RemovableDevices;
                //    StorageFolder sdCard = (await devices.GetFoldersAsync()).FirstOrDefault();

                //    Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;

                //    StorageFolder externalDevices = KnownFolders.RemovableDevices;
                //    IReadOnlyList<StorageFolder> externalDrives = await externalDevices.GetFoldersAsync();
                //    StorageFolder x = externalDrives[0];

                //    string filepath = x.Path + "reps.xlsx";
                //    byte[] bin = pck.GetAsByteArray();

                //    string www = Path.Combine(installedLocation.Path, "Reports.xlsx");

                //    //pck.SaveAs(new FileInfo("Reports.xlsx"));
                //    //File.WriteAllBytes(www, bin);

                //    //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RPIAMSSQLite.db3")
                //    //File.WriteAllBytes(filepath, bin);

                //    //var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                //    //folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Unspecified;
                //    //folderPicker.FileTypeFilter.Add("*");

                //    //Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                //    //if (folder != null)
                //    //{
                //    //    // Application now has read/write access to all contents in the picked folder
                //    //    // (including other sub-folder contents)
                //    //    Windows.Storage.AccessCache.StorageApplicationPermissions.
                //    //    FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                //    //}
                //    //else
                //    //{
                //    //}

                //    //-------------------------------------------

                //    ////clear the buffer stream
                //    //Response.ClearHeaders();
                //    //Response.Clear();
                //    //Response.Buffer = true;

                //    ////set the correct contenttype
                //    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //    ////set the correct length of the data being send
                //    //Response.AddHeader("content-length", bin.Length.ToString());

                //    ////set the filename for the excel package
                //    //Response.AddHeader("content-disposition", "attachment; filename=\"ExcelDemo.xlsx\"");

                //    ////send the byte array to the browser
                //    //Response.OutputStream.Write(bin, 0, bin.Length);

                //    ////cleanup
                //    //Response.Flush();
                //    //HttpContext.Current.ApplicationInstance.CompleteRequest();

                //    //---------------------------
                //    //pck.SaveAs(new FileInfo(installedLocation+"Reports.xlsx"));

                //    //await DisplayAlert("Baka...", "Report Exported to "+installedLocation.DisplayName, "Mataku");

                //    //if (sdCard != null)
                //    //{
                //    //    // Application now has read/write access to all contents in the picked folder
                //    //    // (including other sub-folder contents)
                //    //    //pck.SaveAs(new FileInfo(sdCard.Path + "Reports.xlsx"));

                //    //    //Windows.Storage.AccessCache.StorageApplicationPermissions.
                //    //    //FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                //    //    pck.SaveAs(new FileInfo("E:\\Reports.xlsx"));
                //    //    await DisplayAlert("Yosh...", "Detailed Report Exported", "Sure");
                //    //    await Navigation.PopToRootAsync();

                //    //}
                //    //else
                //    //{
                //    //    await DisplayAlert("Baka...", "USB not found or recognized", "Mataku");
                //    //}

                //}
            }
            else if (reportType == 2)
            {
                await Navigation.PushAsync(new ViewReportSummary { e = App.EmployeeDatabase.GetSummaryReportsBetweenDate(dtSdt.Date, dtEdt.Date), sdt = dtSdt.Date, edt = dtEdt.Date });


                //using (ExcelPackage pck = new ExcelPackage())
                //{
                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Summary Report");
                //    DataTable dt = ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(dtSdt.Date, dtEdt.Date));
                //    ExcelRange firstCell = ws.Cells[1, 1, 3, dt.Columns.Count + 4];
                //    firstCell.Merge = true;
                //    firstCell.Value = " Summary Report From " + dtSdt.Date.ToShortDateString() + " to " + dtEdt.Date.ToShortDateString() + " ";
                //    firstCell.Style.Border.BorderAround(ExcelBorderStyle.Medium);
                //    firstCell.Style.Font.Size = 15;
                //    firstCell.Style.Font.Bold = true;
                //    firstCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //    firstCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //    //var pp = ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(dtSdt.Date, dtEdt.Date));

                //    //double o = 0;

                //    //foreach (DataRow row in pp.Rows)
                //    // {
                //    //     o += Convert.ToDouble(row["TIAMLate"].ToString());
                //    // }

                //    ws.Cells["A5"].LoadFromDataTable(ConvertToDataTable(App.EmployeeDatabase.GetSummaryReportsBetweenDate(dtSdt.Date, dtEdt.Date)), true);

                //    ws.Protection.IsProtected = true;
                //    ws.Protection.AllowSelectLockedCells = false;

                //    StorageFolder devices = Windows.Storage.KnownFolders.RemovableDevices;
                //    StorageFolder sdCard = (await devices.GetFoldersAsync()).FirstOrDefault();

                //    //if(sdCard != null)
                //    //{
                //    //    pck.SaveAs(new FileInfo(sdCard.Path+"Reports.xlsx"));
                //    //    await DisplayAlert("Yosh...", "Summary Report Exported", "Sure");
                //    //    await Navigation.PopToRootAsync();
                //    //}
                //    //else
                //    //{
                //    //    await DisplayAlert("Yosh...", "USB not found or recognized", "Sure");
                //    //}


                //}
            }
            else
            {
                //await DisplayAlert("Yosh...", "Invalid Selection", "Sure");
                await Navigation.PushAsync(new ViewReport { e = App.EmployeeDatabase.GetDetailedReportsBetweenDate(dtSdt.Date, dtEdt.Date), sdt = dtSdt.Date, edt = dtEdt.Date });
            }


            //ListToExcel(App.EmployeeDatabase.GetEmployeeTodayLogAsync());

            //ExcelPackage ExcelPkg = new ExcelPackage();
            //ExcelWorksheet wsSheet1 = ExcelPkg.Workbook.Worksheets.Add("Sheet1");
            //using (ExcelRange Rng = wsSheet1.Cells[2, 2, 2, 2])
            //{
            //    Rng.Value = "Welcome to Everyday be coding - tutorials for beginners";
            //    Rng.Style.Font.Size = 16;
            //    Rng.Style.Font.Bold = true;
            //    Rng.Style.Font.Italic = true;
            //}
            ////wsSheet1.Cells.LoadFromDataTable(, true, OfficeOpenXml.Table.TableStyles.Light8);
            //wsSheet1.Protection.IsProtected = false;
            //wsSheet1.Protection.AllowSelectLockedCells = false;
            //ExcelPkg.SaveAs(new FileInfo(@"E:/Report.xlsx"));

            //using (FileStream aFile = new FileStream(@"E:\asdf.xlsx", FileMode.Create))
            //{
            //    aFile.Seek(0, SeekOrigin.Begin);
            //    ExcelPkg.SaveAs(aFile);
            //    aFile.Close();
            //}

            //// See here - I can still work with the spread sheet.
            //var worksheet = ExcelPkg.Workbook.Worksheets.Single();

        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public void ListToExcel<T>(List<T> query)
        {
            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    //Create the worksheet
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Result");

            //    //get our column headings
            //    var t = typeof(T);
            //    var Headings = t.GetProperties();
            //    for (int i = 0; i < Headings.Count(); i++)
            //    {

            //        ws.Cells[1, i + 1].Value = Headings[i].Name;
            //    }

            //    //populate our Data
            //    if (query.Count() > 0)
            //    {
            //        ws.Cells["A2"].LoadFromCollection(query);
            //    }

            //    //Format the header
            //    using (ExcelRange rng = ws.Cells["A1:BZ1"])
            //    {
            //        rng.Style.Font.Bold = true;
            //        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;   //Set Pattern for the background to Solid
            //        rng.Style.Fill.BackgroundColor.SetColor(Xamarin.Forms.Color.Red);  //Set color to dark blue
            //        rng.Style.Font.Color.SetColor(Xamarin.Forms.Color.White);
            //    }

            //    ws.Protection.IsProtected = true;
            //    ws.Protection.AllowSelectLockedCells = false;

            //    //Write it back to the client
            //    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    //Response.AddHeader("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
            //    //Response.BinaryWrite(pck.GetAsByteArray());
            //    //Response.End();
            //    pck.SaveAs(new FileInfo("Reports.xlsx"));
            //}
        }
        public void OOP()
        {
            //using (var package = new ExcelPackage())
            //{
            //    ExcelWorksheet sheet = package.Workbook.Worksheets.Add("MySheet");

            //    // Setting & getting values
            //    ExcelRange firstCell = sheet.Cells[1, 1];
            //    firstCell.Value = "will it work?";
            //    sheet.Cells["A2"].Formula = "CONCATENATE(A1,\" ... Of course it will!\")";

            //    // Numbers
            //    var moneyCell = sheet.Cells["A3"];
            //    moneyCell.Style.Numberformat.Format = "$#,##0.00";
            //    moneyCell.Value = 15.25M;

            //    // Easily write any Enumerable to a sheet
            //    // In this case: All Excel functions implemented by EPPlus

            //    var funcs = package.Workbook.FormulaParserManager.GetImplementedFunctions()
            //        .Select(x => new { FunctionName = x.Key, TypeName = x.Value.GetType().FullName });

            //    sheet.Cells["A4"].LoadFromCollection(funcs, true);

            //    // Styling cells
            //    var someCells = sheet.Cells["A1,A4:B4"];
            //    someCells.Style.Font.Bold = true;
            //    someCells.Style.Font.Color.SetColor(Xamarin.Forms.Color.AliceBlue);
            //    someCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    someCells.Style.Fill.BackgroundColor.SetColor(Xamarin.Forms.Color.LavenderBlush);
                
            //    //sheet.Cells.AutoFitColumns();
            //    package.SaveAs(new FileInfo("Reports.xlsx"));
            //}
        }

        #endregion 

        #region Operations
        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        private async void BtnShutdown_Clicked(object sender, EventArgs e)
        {
            bool a = await DisplayAlert("Reminder..", "Shutdown the device?", "Yes", "No");
            if (a)
            {
                //Shutdown Device
                // Shutdowns the device immediately:
                await DisplayAlert("Reminder..", "Shuting Down","Yes");

                //ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(3));
            }
            else
            {
                await Navigation.PopToRootAsync();
                await DisplayAlert("Alert..", "Shutdown Cancelled", "Yes");
            }
        }

        private async void BtnQuit_Clicked(object sender, EventArgs e)
        {
            bool o = await DisplayAlert("Alert...", "Close the application ?", "Yes", "No");
            if (o)
            {
                Application.Current.Quit();
            }
        }

        private async void BtnClearRecords_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ConfirmationPopup() { ConfirmMessage = "DELETE EXPORTED" , ConfirmEnum = Enums.ConfirmDeleteEnums.Exported });
            //bool o = await DisplayAlert("Alert...", "This will clear all records ?", "Yes", "No");
            //if (o)
            //{
            //  await  App.EmployeeDatabase.ResetAllTimelogs();
            //}
        }



        #endregion

        #region Import 

        private async void BtnImport_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopup(), true);

            try
            {

                await Navigation.PushAsync(new ViewImportedEmployee { ee = await App.EmployeeDatabase.GetSQLEmployeeAsync()/*, r = await App.EmployeeDatabase.GetSQLEmployeeAsyncRaw()*/ });

            }
            catch (Exception ex) {

                await DisplayAlert("Baka", ex.Message, "Yaro");
                await DisplayAlert("Baka", "No Connection Found", "Yaro");
            }
            finally
            {
                await PopupNavigation.Instance.PopAsync(true);
            }
        }

        private void BtnEmpType_Clicked(object sender, EventArgs e)
        {
            empType = 1;

            btnEmpType.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            //btnSummary.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
        }
        #endregion

        #region IP
        private async void BtnSaveIP_Clicked(object sender, EventArgs e)
        {
            if (txtPort.Text != null && txtPort.Text != string.Empty && txtIP.Text != null && txtIP.Text != string.Empty)
            {
                await App.EmployeeDatabase.SaveIPConfiguration(txtIP.Text, txtPort.Text);
                var a = await App.EmployeeDatabase.GetLatestConfig();
                if(a == null)
                {

                }
                else
                {
                    await DisplayAlert("Nice...", a.ConfigIP + "" + a.ConfigPort, "Okay");
                }
                await DisplayAlert("Nice...", "IP Saved", "Okay");

            }
            else
            {
                await DisplayAlert("Alert...", "Enrty must not be null", "Okay");
            }
        }

        //private async void BtnImport_Clicked_1(object sender, EventArgs e)
        //{
        //    bool oo = await DisplayAlert("Hey", "This will delete the application data and add a new one \r\n\r\n Continue?", "Alright", "Cancel That");

        //    if (oo && txtIP != null && txtIP.Text != string.Empty)
        //    {
        //        await App.EmployeeDatabase.SaveIPConfiguration(txtIP.Text, txtPort.Text);
        //        //App.K5OrderingDatabase.Port = txtPort.Text;
        //        //App.K5OrderingDatabase.IP = txtIP.Text;
        //        IPConfig ipconf = await App.EmployeeDatabase.GetLatestConfig();

        //        try
        //        {

        //            if (await App.OrderDatabase.SaveAllImportedDataToSQLite() == 0)
        //            {
        //                await App.OrderDatabase.SaveLogPref(3);
        //                await DisplayAlert("Hey", "All Data Successfully Imported", "Alright");
        //            }
        //            else
        //            {

        //                await DisplayAlert("Hey", "Data has not been added", "Alright");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            await DisplayAlert("Baka..", ex.Message, "Taskowarimashta");

        //        }
        //        finally
        //        {
        //            await PopupNavigation.Instance.PopAsync(true);
        //            App.OrderDatabase.AdditionalLoadInfo = "";
        //        }

        //    }
        //    else
        //    {
        //        await DisplayAlert("Hey", "Operation Canceled | Complete Entry", "Alright");
        //    }
        //}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = await App.EmployeeDatabase.GetLatestConfig();
        }

        private void BtnTIN_Clicked(object sender, EventArgs e)
        {
            //App.EmployeeDatabase.SavePref(1,txtDevCode.Text);
            btnTIN.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
            btnOUT.BackgroundColor = Xamarin.Forms.Color.FloralWhite;

        }

        private void BtnOUT_Clicked(object sender, EventArgs e)
        {
            //App.EmployeeDatabase.SavePref(2,txtDevCode.Text);

            btnTIN.BackgroundColor = Xamarin.Forms.Color.FloralWhite;
            btnOUT.BackgroundColor = Xamarin.Forms.Color.PaleVioletRed;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            if(txtDevCode.Text == string.Empty)
            {
                await DisplayAlert("Alert...", "Entry must not be null", "Okay");
            }
            else
            {
                if (btnTIN.BackgroundColor == Xamarin.Forms.Color.PaleVioletRed)
                {
                   await App.EmployeeDatabase.SavePref(1, txtDevCode.Text, pickExpTime.Time);
                }
                else
                {
                   await App.EmployeeDatabase.SavePref(2, txtDevCode.Text, pickExpTime.Time);
                }
               
                await DisplayAlert("Alert...", "Config Saved", "Okay");
            }

        }
        #endregion

      
    }
}
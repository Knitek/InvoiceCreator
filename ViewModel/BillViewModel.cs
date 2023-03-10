using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceCreator.Model;
using InvoiceCreator.Controls;
using ToolsLib;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;
using System.Windows;

namespace InvoiceCreator.ViewModel
{
    class BillViewModel : INotifyPropertyChanged
    {
        BillData billData { get; set; }
        string statusText { get; set; }
        string lastFileName { get; set; }

        public ResourceDictionary Resources { get; set; }

        public BillData BillData
        {
            get { return billData; }
            set
            {
                billData = value;
                RaisePropertyChanged("BillData");
            }
        }
        public string StatusText
        {
            get { return statusText; }
            set { statusText = value;RaisePropertyChanged("StatusText"); }
        }
        public string LastFileName
        {
            get { return lastFileName; }
            set { lastFileName = value;RaisePropertyChanged("LastFileName"); }
        }
        public Action ClearStatusAction { get; set; }

        public CommandBase LoadDefaultCommand { get; set; }
        public CommandBase OpenDefaultDirectoryCommand { get; set; }
        public CommandBase GeneratePDFCommand { get; set; }
        public CommandBase SaveCommand { get; set; }
        public CommandBase SaveAsCommand { get; set; }
        public CommandBase OpenCommand { get; set; }
        public CommandBase NextInvoiceNowCommand { get; set; }
        public CommandBase NextInvoiceNextMonthCommand { get; set; }
        public CommandBase AboutWindowCommand { get; set; }
        public CommandBase ExitCommand { get; set; }

        public BillViewModel()
        {
            BillData = new BillData();
            ClearStatusAction = new Action(async () => { await Task.Delay(TimeSpan.FromSeconds(2.5)); StatusText = ""; });

            LoadDefaultCommand = new CommandBase(LoadDefault);
            OpenDefaultDirectoryCommand = new CommandBase(OpenDefaultDirectory);
            GeneratePDFCommand = new CommandBase(GeneratePDF);
            SaveCommand = new CommandBase(Save);
            SaveAsCommand = new CommandBase(SaveAs);
            OpenCommand = new CommandBase(Open);

            NextInvoiceNowCommand = new CommandBase(NextInvoiceNow);
            NextInvoiceNextMonthCommand = new CommandBase(NextInvoiceNextMonth);

            AboutWindowCommand = new CommandBase(AboutWindow);
            ExitCommand = new CommandBase(Exit);
        }

        private void LoadDefault()
        {
            string dir = System.IO.Path.GetDirectoryName(ToolsLib.Tools.ReadAppSetting("defaultDataDirectory"));
            if(!System.IO.Directory.Exists(dir))
            {
                MessageBox.Show("Directory not exists\r\n" + dir);
                return;
            }
            var tmpData = ToolsLib.Tools.Deserialize<BillData>(System.IO.Path.Combine(dir, "default.xml"));
            if(tmpData!=null)
            {
                BillData = null;
                BillData = tmpData;
                LastFileName = System.IO.Path.GetFileName("default.xml");
                StatusText = "Loaded: " + LastFileName;
                Task.Factory.StartNew(ClearStatusAction);
            }
        }
        private void OpenDefaultDirectory()
        {
            string dir = ToolsLib.Tools.ReadAppSettingPath("defaultDataDirectory");
            if (!System.IO.Directory.Exists(dir))
            {
                MessageBox.Show("Directory not exists\r\n" + dir);
                return;
            }
            System.Diagnostics.Process.Start(dir);
        }
        
        private void NextInvoiceNow()
        {
            DateTime currentDate = DateTime.Now;
            var result = EditFunctions.InvocieDateController.NextInvoiceByNow(BillData, currentDate);
            if(string.IsNullOrWhiteSpace(result.errorMessage) is false)
            {
                MessageBox.Show(result.errorMessage);
                return;
            }
            else
            {
                if(result.outBillData == null)
                {
                    MessageBox.Show("Unexpected error in edit function");
                    return;
                }
                BillData.BillNumber = result.outBillData.BillNumber;
                BillData.SaleDate = result.outBillData.SaleDate;
                BillData.IssueDate = result.outBillData.IssueDate;
                BillData.PaymentDate = result.outBillData.PaymentDate;
            }
        }
        private void NextInvoiceNextMonth()
        {
            var result = EditFunctions.InvocieDateController.NextInvoiceByEndOfNextMonth(BillData);
            if (String.IsNullOrEmpty(result.errorMessage) is false)
            {
                MessageBox.Show(result.errorMessage);
                return;
            }
            else
            {
                if (result.outBillData == null)
                {
                    MessageBox.Show("Unexpected error in edit function");
                    return;
                }
                BillData.BillNumber = result.outBillData.BillNumber;
                BillData.IssueDate = result.outBillData.IssueDate;
                BillData.SaleDate = result.outBillData.SaleDate;
                BillData.PaymentDate = result.outBillData.PaymentDate;
            }
        }
        
        private void GeneratePDF()
        {            
            string result = PDFFileGenerator.Generate(BillData);
            if(string.IsNullOrWhiteSpace(result) is false)
            {
                string filename = System.IO.Path.GetFileName(result);
                MessageBox.Show("File created:" + filename);
            }
            Save();
        }
        private void Save()
        {
            if (BillData == null) { MessageBox.Show("No data"); return; }
            if(string.IsNullOrWhiteSpace(BillData.BillNumber) is true)
            {
                MessageBox.Show(Resources["invNoEmpty"].ToString());
                return;
            }
            string filename = BillData.BillNumber.Replace(@"/","_").Replace(@"\","_") + ".xml";
            string path = System.IO.Path.Combine(Tools.ReadAppSettingPath("defaultDataDirectory"), filename);
            Tools.Serialize<BillData>(BillData, path);
            StatusText = "Saved " + filename;
            LastFileName = path;
            Task.Factory.StartNew(ClearStatusAction);
        }
        private void SaveAs()
        {
            try
            {
                if (BillData == null ) { MessageBox.Show("No data"); return; }

                string dir = Tools.ReadAppSettingPath("defaultDataDirectory");

                string filename = BillData.BillNumber.Replace(@"/", "_").Replace(@"\", "_") + ".xml";
                var dialog = new System.Windows.Forms.SaveFileDialog();
                dialog.InitialDirectory  = dir; 
                dialog.FileName = filename;
                dialog.DefaultExt = "xml";
                dialog.Filter = "XML (*.xml)|*.xml|All files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Tools.Serialize<BillData>(BillData, dialog.FileName);
                    LastFileName = dialog.FileName;
                    StatusText = "Saved: " + System.IO.Path.GetFileName(LastFileName);
                    Task.Factory.StartNew(ClearStatusAction);
                }
            }
            catch (Exception exc)
            {
                Tools.ExceptionLogAndShow(exc, "SaveAsCommand");
            }
        }
        private void Open()
        {
            string selectedFile = string.Empty;
            try
            {
                using (var dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    var tmp = System.Environment.CurrentDirectory.Remove(System.Environment.CurrentDirectory.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
                    tmp = System.IO.Path.Combine(tmp, "Data" + System.IO.Path.DirectorySeparatorChar);
                    dialog.DefaultExt = "xml";
                    dialog.Filter = "XML (*.xml)|*.xml|All files (*.*)|*.*";
                    dialog.InitialDirectory = tmp;

                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        selectedFile = dialog.FileName;
                    }
                }
                if (selectedFile == string.Empty) return;
                if(System.IO.Path.GetExtension(selectedFile).ToLower() != ".xml")
                {
                    MessageBox.Show("Can't open file type of \""+ System.IO.Path.GetExtension(selectedFile) + "\"");
                    return;
                }
                BillData = null;
                BillData = Tools.Deserialize<BillData>(selectedFile);
                LastFileName = selectedFile;
                StatusText = "Loaded: " + System.IO.Path.GetFileName(LastFileName);
                Task.Factory.StartNew(ClearStatusAction);
            }
            catch (Exception exc)
            {
                Tools.ExceptionLogAndShow(exc, "Open");
            }
        }
        private void AboutWindow()
        {
            ToolsLib.Wpf.AboutWindow aboutWindow = new ToolsLib.Wpf.AboutWindow("Invoice creator", "20180817 v1.0", "Application for creating simple Bill documents in this moment, and invoice in future");
        }
        private void Exit()
        {
            App.Current.MainWindow.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceCreator.Model;
using InvoiceCreator.Controls;
using System.Windows.Forms;
using ToolsLib;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Drawing.Layout;

namespace InvoiceCreator.ViewModel
{
    class BillViewModel : INotifyPropertyChanged
    {
        BillData billData { get; set; }
        string statusText { get; set; }
        string lastFileName { get; set; }

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
        public CommandBase NextMonthCommand { get; set; }
        public CommandBase NextInvoiceNoCommand { get; set; }
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

            NextMonthCommand = new CommandBase(NextMonth);
            NextInvoiceNoCommand = new CommandBase(NextInvoiceNO);

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
        private void NextMonth()
        {
            var tmpPaymentDate = DateTime.Parse(BillData.PaymentDate);
            tmpPaymentDate = tmpPaymentDate.AddMonths(1);
            tmpPaymentDate = tmpPaymentDate.AddDays(14 - tmpPaymentDate.Day);

            var tmpSaleDate = DateTime.Parse(BillData.SaleDate);
            tmpSaleDate = tmpSaleDate.AddMonths(1);
            tmpSaleDate = tmpSaleDate.AddDays(DateTime.DaysInMonth(tmpSaleDate.Year, tmpSaleDate.Month)-tmpSaleDate.Day);

            BillData.PaymentDate = tmpPaymentDate.ToString("dd.MM.yyyy");
            BillData.SaleDate = tmpSaleDate.ToString("dd.MM.yyyy");
            BillData.IssueDate = tmpSaleDate.ToString("dd.MM.yyyy");
        }
        private void NextInvoiceNO()
        {
            (string no, string errorMessage) = Model.InvocieDateController.NextInvoiceNO(BillData.BillNumber);
            if (errorMessage.Length > 0)
                MessageBox.Show(errorMessage);
            else if (no.Length > 0)
                BillData.BillNumber = no;
                
        }
        private void GeneratePDF()
        {
            var invoice = BillData;
            PdfDocument document = new PdfDocument();
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Times New Roman", 12, XFontStyle.Regular, options);
            XFont bfont = new XFont("Times New Roman", 13, XFontStyle.Bold, options);
            XTextFormatter tf = new XTextFormatter(gfx);
            XTextFormatter tfcenter = new XTextFormatter(gfx);
            tfcenter.Alignment = XParagraphAlignment.Center;
            XRect rect = new XRect();
            //header
            rect = new XRect(50, 40, 100, 20);
            tf.DrawString("RACHUNEK", bfont, XBrushes.Black, rect);
            rect = new XRect(50, 60, 100, 20);
            tf.DrawString("Nr " + invoice.BillNumber + "\r\nORYGINAŁ", font, XBrushes.Black, rect);

            //items
            double itemsStartY = 340;
            double itemsColumnYSep = itemsStartY + 20;
            gfx.DrawLine(XPens.Black, 45, itemsColumnYSep - 4, 550, itemsColumnYSep - 4);
            string tmp = string.Empty;
            int height = (invoice.Items.Count * 20) + 20;
            rect = new XRect(40, itemsStartY, 40, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += i.ToString() + "\r\n"; }
            tfcenter.DrawString("L.p.", font, XBrushes.Black, rect);
            rect = new XRect(50, itemsColumnYSep, 40, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(80, itemsStartY, 200, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].Name + "\r\n"; }
            tfcenter.DrawString("Nazwa", font, XBrushes.Black, rect);
            rect = new XRect(80, itemsColumnYSep, 200, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(280, itemsStartY, 40, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].PKWiU + "\r\n"; }
            tfcenter.DrawString("PKWiU", font, XBrushes.Black, rect);
            rect = new XRect(280, itemsColumnYSep, 40, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(320, itemsStartY, 30, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].Unit + "\r\n"; }
            tfcenter.DrawString("j.m.", font, XBrushes.Black, rect);
            rect = new XRect(320, itemsColumnYSep, 30, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(350, itemsStartY, 30, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].Count + "\r\n"; }
            tfcenter.DrawString("Ilość", font, XBrushes.Black, rect);
            rect = new XRect(350, itemsColumnYSep, 30, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(380, itemsStartY, 110, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].UnitPrice + "\r\n"; }
            tfcenter.DrawString("Cena jednostkowa", font, XBrushes.Black, rect);
            rect = new XRect(380, itemsColumnYSep, 110, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            rect = new XRect(490, itemsStartY, 70, height);
            tmp = string.Empty;
            for (int i = 1; i < invoice.Items.Count + 1; i++) { tmp += invoice.Items[i - 1].Value + "\r\n"; }
            tfcenter.DrawString("Wartość", font, XBrushes.Black, rect);
            rect = new XRect(490, itemsColumnYSep, 70, height);
            tfcenter.DrawString(tmp, font, XBrushes.Black, rect);

            //addresses
            rect = new XRect(50, 180, 250, 30);
            tf.DrawString("Sprzedawca", bfont, XBrushes.Black, rect);
            rect = new XRect(50, 200, 250, 220);
            tf.DrawString("Nazwa:\r\n" + ((invoice.VendorNIP != "") ? "NIP:\r\n" : "") + "Adres:\r\n", font, XBrushes.Black, rect);
            rect = new XRect(100, 200, 250, 220);
            tf.DrawString(invoice.VendorData, font, XBrushes.Black, rect);


            rect = new XRect(300, 180, 250, 200);
            tf.DrawString("Nabywca", bfont, XBrushes.Black, rect);
            rect = new XRect(300, 200, 210, 220);
            tf.DrawString("Nazwa:\r\n" + ((invoice.CustomerNIP != "") ? "NIP:\r\n" : "") + "Adres:\r\n", font, XBrushes.Black, rect);
            rect = new XRect(350, 200, 210, 220);
            tf.DrawString(invoice.CustomerData, font, XBrushes.Black, rect);

            //seller details
            rect = new XRect(50, 260, 280, 200);
            tf.DrawString("Bank:\r\nKonto:\r\nEmail:\r\nTelefon:", font, XBrushes.Black, rect);
            rect = new XRect(100, 260, 280, 200);
            tf.DrawString(invoice.SellerDetails, font, XBrushes.Black, rect);

            //dates
            rect = new XRect(330, 50, 200, 200);
            tf.DrawString("Miejsce wystawienia:\r\nData wystawienia:\r\nData sprzedaży:\r\nTermin płatności:\r\nForma płatności:\r\n", font, XBrushes.Black, rect);
            rect = new XRect(440, 50, 200, 200);
            tf.DrawString(invoice.DatesValues, font, XBrushes.Black, rect);

            //comments
            rect = new XRect(40, 520, 200, 200);
            if (invoice.Comment != "")
                tf.DrawString(invoice.Comment, font, XBrushes.Black, rect);

            //total amount to pay
            rect = new XRect(370, height + 380, 200, 200);
            tf.DrawString(invoice.PaymentAmount, bfont, XBrushes.Black, rect);
            rect = new XRect(370, height + 400, 200, 200);
            tf.DrawString(invoice.PaymentDetails, font, XBrushes.Black, rect);


            string pdfFilename = invoice.BillNumber.Replace(@"\", "_").Replace(@"/", "_") + ".pdf";
            string dir = Tools.ReadAppSettingPath("defaultDataDirectory");
            //document.Save(System.IO.Path.Combine(dir,pdfFilename));
           
            //Save();
            //if (System.IO.File.Exists(System.IO.Path.Combine(dir,pdfFilename)))
            //{
            //    MessageBox.Show(pdfFilename + " was created.", "Succes");
            //}
            PDFFileGenerator.Generate(invoice);
            System.Diagnostics.Process.Start(dir);
        }
        private void Save()
        {
            if (BillData == null) { MessageBox.Show("No data"); return; }
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
                
                dialog.InitialDirectory = tmp;

                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        selectedFile = dialog.FileName;
                    }
                }
                if (selectedFile == string.Empty) return;
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

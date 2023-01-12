using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolsLib;

namespace InvoiceCreator.Model
{
    static class PDFFileGeneratorV1
    {
        public static (bool isSuccess, string resultText) Generate(BillData invoice)
        {
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
            document.Save(System.IO.Path.Combine(dir, pdfFilename));
            if (System.IO.File.Exists(System.IO.Path.Combine(dir, pdfFilename)))            
                return (true,pdfFilename + " was created succesfuly.");
            else
                return (false,"Error");
        }

    }
}

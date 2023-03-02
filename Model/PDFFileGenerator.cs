using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using InvoiceCreator.Model;
using System.IO;
using ToolsLib;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace InvoiceCreator.Model
{
    class PDFFileGenerator
    {
        public static string Generate(BillData model)
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            FontFactory.Register(@"C:\Windows\Fonts\times.ttf");
            Font B7 = FontFactory.GetFont("Times New Roman", BaseFont.IDENTITY_H, true, 7, Font.NORMAL, BaseColor.BLACK);
            Font B8 = FontFactory.GetFont("Times New Roman",BaseFont.IDENTITY_H,true, 9, Font.NORMAL, BaseColor.BLACK);
            Font BB8 = FontFactory.GetFont("Times New Roman", BaseFont.IDENTITY_H, true, 9, Font.BOLD, BaseColor.BLACK);
            Font BB9 = FontFactory.GetFont("Times New Roman", BaseFont.IDENTITY_H, true, 10, Font.BOLD, BaseColor.BLACK);
            Font BB12 = FontFactory.GetFont("Times New Roman", BaseFont.IDENTITY_H, true, 12, Font.BOLD, BaseColor.BLACK);
            MemoryStream myMemoryStream = new MemoryStream();
            Document doc = new Document(PageSize.A4, 20f, 20f, 20f, 20f);
            PdfWriter myPDFWriter = PdfWriter.GetInstance(doc, myMemoryStream);
            doc.Open();
            PdfContentByte cb = new PdfContentByte(myPDFWriter);
            doc.NewPage();

            PdfPTable firstpdfrow = new PdfPTable(new float[] { 7f, 4f }) { WidthPercentage = 100f, SpacingAfter = 10f };
            //podstawowe informacje o fakturze
            PdfPTable invTable = new PdfPTable(1) {SpacingBefore=10f, TotalWidth=120f, SpacingAfter = 10f, HorizontalAlignment = Element.ALIGN_LEFT };
            invTable.AddCell(new PdfPCell(new Phrase("Faktura", BB12)) { Border = 0 });
            invTable.AddCell(new PdfPCell(new Phrase("Nr " + model.BillNumber, BB9)) { Border = 0 });
            invTable.AddCell(new PdfPCell(new Phrase("ORYGINAŁ", BB9)) { Border = 0 });

            //daty i miejse faktury
            PdfPTable placeTable = new PdfPTable(new float[] { 6f, 5f }) {SpacingAfter = 10f,  HorizontalAlignment = Element.ALIGN_RIGHT };
            placeTable.Rows.AddRange(new List<PdfPRow>()
            {
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Miejsce wystawienia:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.PlaceOfIssue,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Data wystawienia:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.IssueDate,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Data sprzedaży:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.SaleDate,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Termin płatności:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.PaymentDate,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Forma płatności:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.PaymentForm,B8)){Border = 0},
                }),
            });

            firstpdfrow.AddCell(new PdfPCell(invTable) { PaddingLeft= 40f, Border = 0, PaddingTop = 20f });
            firstpdfrow.AddCell(new PdfPCell(placeTable) { Border = 0, PaddingRight = 40f, PaddingTop = 30f });
            doc.Add(firstpdfrow);

            PdfPTable secondpdfrow = new PdfPTable(new float[] { 5f, 5f }) { WidthPercentage = 100f, SpacingAfter = 10f };
            PdfPTable vendorTable = new PdfPTable(new float[] { 2f, 8f }) { WidthPercentage = 40f, HorizontalAlignment = Element.ALIGN_LEFT };
            vendorTable.Rows.AddRange(new List<PdfPRow>()
            {
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Sprzedawca",BB9)){Border = 0},
                    new PdfPCell(new Phrase(Chunk.NEWLINE)){Border = 0,Colspan = 1},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Nazwa:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorName,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("NIP:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorNIP,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Adres:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorAddress,B8)){Border = 0},
                }),
            });
            
            PdfPTable customerTable = new PdfPTable(new float[] { 2f, 8f }) { WidthPercentage = 40f, HorizontalAlignment = Element.ALIGN_LEFT };
            customerTable.Rows.AddRange(new List<PdfPRow>()
            {
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Nabywca",BB9)){Border = 0},
                    new PdfPCell(new Phrase(Chunk.NEWLINE)){Border = 0,Colspan = 1},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Nazwa:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.CustomerName,B8)){Border = 0},
                }),  
            });
            if (!string.IsNullOrWhiteSpace(model.CustomerNIP))
                customerTable.Rows.Add(new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("NIP:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.CustomerNIP,B8)){Border = 0},
                }));
            customerTable.Rows.Add(new PdfPRow(new PdfPCell[]
            {
                new PdfPCell(new Phrase("Adres:",B8)){Border = 0},
                new PdfPCell(new Phrase(model.CustomerAddress,B8)){Border = 0},
            }));

            secondpdfrow.AddCell(new PdfPCell(vendorTable) { Border = 0, PaddingLeft = 15f });
            secondpdfrow.AddCell(new PdfPCell(customerTable) { Border = 0 });
            doc.Add(secondpdfrow);
            PdfPTable thirdRow = new PdfPTable(new float[] { 5f, 5f }) { WidthPercentage = 100f, SpacingAfter = 20f };
            PdfPTable payInfo = new PdfPTable(new float[] { 2f, 8f }) { WidthPercentage = 40f, HorizontalAlignment = Element.ALIGN_LEFT };
            if (!string.IsNullOrWhiteSpace(model.VendorAccountNumber))
                payInfo.Rows.AddRange(new List<PdfPRow>(){
                    new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Bank",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorBankName,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Konto:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorAccountNumber,B8)){Border = 0},
                }), });
            payInfo.Rows.AddRange(new List<PdfPRow>()
            {                
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Email:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorEmail,B8)){Border = 0},
                }),
                new PdfPRow(new PdfPCell[]
                {
                    new PdfPCell(new Phrase("Telefon:",B8)){Border = 0},
                    new PdfPCell(new Phrase(model.VendorPhone,B8)){Border = 0},
                }),
            });
            thirdRow.AddCell(new PdfPCell(payInfo) { BorderWidth = 0,PaddingLeft=15f });
            thirdRow.AddCell(new PdfPCell() { BorderWidth = 0 });
            doc.Add(thirdRow);
            //lista produktów faktury
            //PdfPTable itemTable = new PdfPTable(new float[] { 1.5f, 10f, 4f, 4f, 2f, 3f, 4f, 3f, 4f, 4f }) { WidthPercentage=95f, HorizontalAlignment = Element.ALIGN_CENTER };
            bool noPKWiU = false;
            if (model.Items.All(x => string.IsNullOrWhiteSpace(x.PKWiU))) noPKWiU = true;
            PdfPTable itemTable = null;
            if(noPKWiU)
                itemTable = new PdfPTable(new float[] { 1.5f, 13f, 3f, 3f, 5f, 5f}) { WidthPercentage=95f, HorizontalAlignment = Element.ALIGN_CENTER };
            else
                itemTable = new PdfPTable(new float[] { 1.5f, 10f, 3f, 3f, 3f, 5f, 5f }) { WidthPercentage = 95f, HorizontalAlignment = Element.ALIGN_CENTER };

            List<PdfPCell> headers = new List<PdfPCell>()
            {
                new PdfPCell(new Phrase("L.p.", B8)){HorizontalAlignment = Element.ALIGN_RIGHT } ,
                new PdfPCell(new Phrase("Nazwa", B8)){HorizontalAlignment = Element.ALIGN_LEFT } ,                
            };
            if (!noPKWiU)
                headers.Add(new PdfPCell(new Phrase("PKWiU", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            headers.AddRange(new List<PdfPCell>()
            {
                new PdfPCell(new Phrase("JM", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT },
                new PdfPCell(new Phrase("Ilość", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT },
                
                //new PdfPCell(new Phrase("Wartość \nnetto [zł]", B8)) {HorizontalAlignment = Element.ALIGN_RIGHT },
                //new PdfPCell(new Phrase("VAT\n[%]", B8)) {HorizontalAlignment = Element.ALIGN_RIGHT },
                //new PdfPCell(new Phrase("Wartość\nVAT [zł]", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT },
               // new PdfPCell(new Phrase("Wartość\nbrutto [zł]", B8)) {HorizontalAlignment = Element.ALIGN_RIGHT },
                new PdfPCell(new Phrase("Cena jednostkowa [zł]", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT },
                new PdfPCell(new Phrase("Wartość [zł]", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT },
            });
                //new PdfPCell(new Phrase("Cena netto \n[zł]", B8)) {HorizontalAlignment = Element.ALIGN_RIGHT },
                
            //headers.ForEach(x => { x.BorderWidthTop = 0; x.BorderWidthBottom = 0; });
            itemTable.Rows.Add(new PdfPRow(headers.ToArray()));
            int counter = 1;
            foreach(var item in model.Items)
            {
                bool last = counter == model.Items.Count;
                List<PdfPCell> values = new List<PdfPCell>();
                values.Add(new PdfPCell(new Phrase(counter.ToString(), B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                values.Add(new PdfPCell(new Phrase(item.Name, B8)) { HorizontalAlignment = Element.ALIGN_LEFT });
                if(!noPKWiU)
                values.Add(new PdfPCell(new Phrase(item.PKWiU, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //values.Add(new PdfPCell(new Phrase(item.UnitPrice, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                values.Add(new PdfPCell(new Phrase(item.Unit, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                values.Add(new PdfPCell(new Phrase(item.Count, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                
                values.Add(new PdfPCell(new Phrase(item.UnitPrice, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //values.Add(new PdfPCell(new Phrase("ZW", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //values.Add(new PdfPCell(new Phrase("0,00", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                values.Add(new PdfPCell(new Phrase(item.UnitPrice, B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //if (last) values.ForEach(x => x.BorderWidthBottom = 0);
                itemTable.Rows.Add( new PdfPRow(values.ToArray()));
                counter++;
            }
            var sumRow = new List<PdfPCell>();
           // sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0,HorizontalAlignment = Element.ALIGN_RIGHT });
            //sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            //sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase("", B8)) { BorderWidth = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase("Razem", B8)) { HorizontalAlignment = Element.ALIGN_RIGHT } );
            sumRow.Add(new PdfPCell(new Phrase(model.Items.Count.ToString(), B8)) {  HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase(model.Items.Sum(x=>decimal.Parse(x.UnitPrice)).ToString(),B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //sumRow.Add(new PdfPCell(new Phrase("",B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //sumRow.Add(new PdfPCell(new Phrase("0,00",B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            sumRow.Add(new PdfPCell(new Phrase(model.Items.Sum(x=>decimal.Parse(x.UnitPrice)).ToString(),B8)) { HorizontalAlignment = Element.ALIGN_RIGHT });
            itemTable.Rows.Add(new PdfPRow(sumRow.ToArray()));            
            doc.Add(itemTable);

            PdfPTable additionalInfo = new PdfPTable(new float[] { 1f, 8f }) { WidthPercentage = 95f, HorizontalAlignment = Element.ALIGN_CENTER, SpacingBefore =20f};
            additionalInfo.Rows.Add(new PdfPRow(new PdfPCell[]{
                new PdfPCell(new Phrase("Uwagi:", B8)) { BorderWidthLeft = 0, PaddingLeft = 10f, BorderColor = BaseColor.LIGHT_GRAY},
                new PdfPCell(new Phrase(model.Comments,B8)) { BorderWidthRight = 0,BorderWidthLeft = 0 , BorderColor = BaseColor.LIGHT_GRAY}
            }));
            additionalInfo.Rows.Add(new PdfPRow(new PdfPCell[]{
                new PdfPCell(new Phrase("Do zapłaty:", B8)) { BorderWidthLeft = 0, PaddingLeft = 10f,BorderWidthTop = 0, BorderColor = BaseColor.LIGHT_GRAY},
                new PdfPCell(new Phrase(model.Items.Sum(x=>decimal.Parse(x.UnitPrice)).ToString()+"zł",B8)) { BorderWidthRight = 0 ,BorderWidthLeft = 0,BorderWidthTop = 0, BorderColor = BaseColor.LIGHT_GRAY}
            }));
            additionalInfo.Rows.Add(new PdfPRow(new PdfPCell[]{
                new PdfPCell(new Phrase("Słownie:", B8)) { BorderWidthLeft = 0, PaddingLeft = 10f,BorderWidthTop = 0, BorderColor = BaseColor.LIGHT_GRAY},
                new PdfPCell(new Phrase(model.AmountInWords,B8)) { BorderWidthRight = 0,BorderWidthLeft = 0,BorderWidthTop = 0 , BorderColor = BaseColor.LIGHT_GRAY}
            }));
            
            doc.Add(additionalInfo);
            PdfPTable signatures =  new PdfPTable(new float[] { 1.5f,5f,2f,5f,1.5f}) { WidthPercentage = 90f, SpacingBefore = 40f, HorizontalAlignment = Element.ALIGN_CENTER };
            PdfPTable sellerSignature = new PdfPTable(1) {TotalWidth = 30f,  HorizontalAlignment = Element.ALIGN_CENTER };
            sellerSignature.AddCell(new PdfPCell(new Phrase("Rafał Kurc", BB9)) { PaddingLeft = 10f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER});
            sellerSignature.AddCell(new PdfPCell(new Phrase("Podpis osoby uprawnionej do wystawienia faktury", B7)) { PaddingLeft = 10f, Border = 0, BorderWidthTop = 1f,
                BorderColorTop = BaseColor.LIGHT_GRAY,  HorizontalAlignment = Element.ALIGN_CENTER });
            PdfPTable buyerSignature = new PdfPTable(1) { TotalWidth = 30f, HorizontalAlignment = Element.ALIGN_CENTER };
            buyerSignature.AddCell(new PdfPCell(new Phrase("   ", BB9)) { PaddingLeft = 10f, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER});
            buyerSignature.AddCell(new PdfPCell(new Phrase("Podpis osoby uprawnionej do odbioru faktury", B7)) { PaddingLeft = 10f, Border = 0, BorderWidthTop = 1f,
                BorderColorTop = BaseColor.LIGHT_GRAY,  HorizontalAlignment = Element.ALIGN_CENTER });

            signatures.AddCell(new PdfPCell(new Phrase(Chunk.NEWLINE)) { Border = 0 });
            signatures.AddCell(new PdfPCell(sellerSignature) { Border = 0 });
            signatures.AddCell(new PdfPCell(new Phrase(Chunk.NEWLINE)) { Border = 0 });
            signatures.AddCell(new PdfPCell(buyerSignature) { Border = 0});
            signatures.AddCell(new PdfPCell(new Phrase(Chunk.NEWLINE)) { Border = 0 });
            doc.Add(signatures);
            doc.Close();
            string pdfFilename = model.BillNumber.Replace(@"\", "_").Replace(@"/", "_") + ".pdf";
            string dir = Tools.ReadAppSettingPath("defaultDataDirectory");
            string pathToSave = Path.Combine(dir, pdfFilename);
            byte[] content = myMemoryStream.ToArray();

            try
            {
                using (FileStream fs = File.Create(pathToSave))
                {
                    fs.Write(content, 0, (int)content.Length);
                }
            }
            catch
            {
                using (FileStream fs = File.Create(Path.Combine(Path.GetDirectoryName(pathToSave), Guid.NewGuid().ToString().Substring(0, 8) + Path.GetFileName(pathToSave))))
                {
                    fs.Write(content, 0, (int)content.Length);
                }
            }
            if (System.IO.File.Exists(pathToSave))
                return pathToSave;
            else
                return string.Empty;
        }
     }
}

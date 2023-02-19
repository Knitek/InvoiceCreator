using InvoiceCreator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceCreator.EditFunctions
{
    public static class InvocieDateController
    {
        /// <summary>
        /// Generates next invoice number
        /// </summary>
        /// <param name="invNumber">String invoice number for incrementation</param>
        /// <returns>New invoice number if can generate and error message if it is inpossible</returns>
        public static (string outInvNumber, string errorMessage) NextInvoiceNO(string invNumber, DateTime baseDate)
        {
            if (invNumber.Count(x => x.Equals('/')) != 3)
                return ("", "Unrecognized invoice format. {FV/YYYY/MM/NO}");

            string[] segments = invNumber.Split('/');
            //year segment
            bool newYear = false;
            if (int.TryParse(segments[1], out int invNoYear))
            {
                if (baseDate.Year != invNoYear)
                {
                    newYear = true;
                    segments[1] = baseDate.Year.ToString();
                }
            }
            else
                return ("", "Invoice year is not a valid number.");
            //month segment
            if (int.TryParse(segments[2], out int invMonth))
            {
                if (baseDate.Month != invMonth)
                {
                    segments[2] = baseDate.ToString("MM");
                }
            }
            else
                return ("", "Invoice month is not a valid number.");
            //no segment
            if (int.TryParse(segments[3], out int invNo))
                segments[3] = newYear ? "1" : (++invNo).ToString();
            else
                return ("","Invoice NO is not a valid number.");

            return (string.Join("/", segments), "");
        }
        /// <summary>
        /// Incerements payment and sale date by full month
        /// </summary>
        /// <param name="inPaymentDate"></param>
        /// <param name="inSaleDate"></param>
        /// <returns></returns>
        public static (string paymentDate, string saleDate, string errorMessage) NextMonth(string inPaymentDate, string inSaleDate)
        {
            DateTime tmpPaymentDate;
            if (DateTime.TryParse(inPaymentDate, out tmpPaymentDate) is false)
                return ("", "", "Invalid payment date format");
            DateTime tmpSaleDate;
            if(DateTime.TryParse(inSaleDate,out tmpSaleDate) is false)
                return ("", "", "Invalid sale date format");          

            tmpPaymentDate = tmpPaymentDate.AddMonths(1);
            tmpPaymentDate = tmpPaymentDate.AddDays(14 - tmpPaymentDate.Day);

            tmpSaleDate = tmpSaleDate.AddMonths(1);            
            tmpSaleDate = tmpSaleDate.AddDays(DateTime.DaysInMonth(tmpSaleDate.Year, tmpSaleDate.Month)-tmpSaleDate.Day);

            return (tmpPaymentDate.ToString("dd.MM.yyyy"), tmpSaleDate.ToString("dd.MM.yyyy"), "");
        }
        /// <summary>
        /// Incerements payment and sale date by full month
        /// </summary>
        /// <param name="inPaymentDate"></param>
        /// <param name="inSaleDate"></param>
        /// <returns></returns>
        public static (string paymentDate, string saleDate, string errorMessage) NextEndOfMonth(string inPaymentDate, string inSaleDate)
        {
            DateTime tmpPaymentDate;
            if (DateTime.TryParse(inPaymentDate, out tmpPaymentDate) is false)
                return ("", "", "Invalid payment date format");
            DateTime tmpSaleDate;
            if (DateTime.TryParse(inSaleDate, out tmpSaleDate) is false)
                return ("", "", "Invalid sale date format");

            tmpPaymentDate = tmpPaymentDate.AddMonths(1);
            tmpPaymentDate = tmpPaymentDate.AddDays(14 - tmpPaymentDate.Day);

            tmpSaleDate = tmpSaleDate.AddMonths(1);
            tmpSaleDate = tmpSaleDate.AddDays(DateTime.DaysInMonth(tmpSaleDate.Year, tmpSaleDate.Month) - tmpSaleDate.Day);

            return (tmpPaymentDate.ToString("dd.MM.yyyy"), tmpSaleDate.ToString("dd.MM.yyyy"), "");
        }
        public static (BillData outBillData, string errorMessage) NextInvoiceByMonth(BillData inBillData)
        {

            BillData tmpBillData = new BillData();
            return (tmpBillData, "");
        }
        /// <summary>
        /// Sets date to current date and payment date as (+14 Days) and generates next invoice number based on current date
        /// </summary>
        /// <param name="inBillData"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public static (BillData outBillData, string errorMessage) NextInvoiceByNow(BillData inBillData, DateTime currentDate)
        {
            DateTime tmpSaleDate = currentDate;
            DateTime tmpPaymentDate = tmpSaleDate.AddDays(14);
            var result = NextInvoiceNO(inBillData.BillNumber, currentDate);
            if (string.IsNullOrWhiteSpace(result.errorMessage) is false)
                return (null, result.errorMessage);
            BillData tmpBillData = new BillData()
            {
                BillNumber = result.outInvNumber,
                SaleDate = tmpSaleDate.ToString("dd.MM.yyyy"),
                IssueDate = tmpSaleDate.ToString("dd.MM.yyyy"),
                PaymentDate = tmpPaymentDate.ToString("dd.MM.yyyy"),
            };
            return (tmpBillData,"");
        }
    }    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceCreator.Model
{
    public static class InvocieDateController
    {
        /// <summary>
        /// Generates next invoice number
        /// </summary>
        /// <param name="invNumber">String invoice number for incrementation</param>
        /// <returns>New invoice number if can generate and error message if it is inpossible</returns>
        public static (string,string) NextInvoiceNO(string invNumber)
        {
            if (invNumber.Count(x => x.Equals('/')) != 3)
                return ("","Unrecognized invoice format. {FV/YYYY/MM/NO}");            

            string[] segments = invNumber.Split('/');
            bool newYear = false;
            if (int.TryParse(segments[1], out int invNoYear))
            {
                if (DateTime.Now.Year != invNoYear)
                {
                    newYear = true;
                    segments[1] = DateTime.Now.Year.ToString();
                }
            }
            else
                return ("","Invoice year is not a valid number.");

            if (int.TryParse(segments[2], out int invMonth))
            {
                if (DateTime.Now.Month != invMonth)
                {
                    segments[2] = DateTime.Now.ToString("MM");
                }
            }
            else
                return("","Invoice month is not a valid number.");

            if (int.TryParse(segments[3], out int invNo))
                segments[3] = newYear ? "1" : (++invNo).ToString();
            else
                return("Invoice NO is not a valid number.","");

            return (string.Join("/", segments),"");
        }
    }
}

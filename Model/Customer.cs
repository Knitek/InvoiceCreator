using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InvoiceCreator.Model
{
    class Customer
    {
        public string Name { get; set; }
        public string NIP { get; set; }
        public string Address { get; set; }
        public string AddressInline { get {  return Address.Replace(System.Environment.NewLine,", "); } }
        [XmlIgnore]
        public string GetHash
        {
            get 
            {
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(Name + NIP + Address);
                return Hash.CreateSHA256(bytes).ToString();
            }
        }
    }
}

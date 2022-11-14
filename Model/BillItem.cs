using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceCreator.Model
{
    public class BillItem
    {
        public string Name { get; set; }
        public string PKWiU { get; set; }
        public string Count { get; set; }
        public string Unit { get; set; }
        public string UnitPrice { get; set; }
        public string Value { get; set; }
    }
}

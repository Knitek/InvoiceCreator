using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InvoiceCreator.Model
{
    public class BillData : INotifyPropertyChanged
    {
        private string saleDate { get; set; }
        private string issueDate { get; set; }
        private string paymentDate { get; set; }
        private string billNumber { get; set; }        

        public string VendorName { get; set; }
        public string VendorNIP { get; set; }
        public string VendorAddress { get; set; }
        public string VendorEmail { get; set; }
        public string VendorPhone { get; set; }

        public string CustomerName { get; set; }
        public string CustomerNIP { get; set; }
        public string CustomerAddress { get; set; }

        public ObservableCollection<BillItem> Items { get; set; }

        public string SaleDate
        {
            get
            {
                return saleDate;
            }
            set
            {
                if (value != saleDate)
                {
                    saleDate = value;
                    RaisePropertyChanged("SaleDate");
                }
            }
        }
        public string IssueDate
        {
            get
            {
                return issueDate;
            }
            set
            {
                if (value != issueDate)
                {
                    issueDate = value;
                    RaisePropertyChanged("IssueDate");
                }
            }
        }
        public string PaymentDate
        {
            get
            {
                return paymentDate;
            }
            set
            {
                if (value != paymentDate)
                {
                    paymentDate = value;
                    RaisePropertyChanged("PaymentDate");
                }
            }
        }
        public string BillNumber
        {
            get
            {
                return billNumber;
            }
            set
            {
                if(value != billNumber)
                {
                    billNumber = value;
                    RaisePropertyChanged("BillNumber");
                }
            }
        }
        //public string IssueDate { get; set; }
        //public string PaymentDate { get; set; }
        //public string PaymentDate { get; set; }

        public string PlaceOfIssue { get; set; }
        public string ToPay { get; set; }
        public string PaymentForm { get; set; }
        public string AmountInWords { get; set; }
        public string VendorBankName { get; set; }
        public string VendorAccountNumber { get; set; }

        public string Comments { get; set; }

        [XmlIgnore]
        public string VendorData { get { return $"{VendorName}\r\n" + ((VendorNIP != "") ? $"{VendorNIP}\r\n" : "") + VendorAddress; } }
        [XmlIgnore]
        public string CustomerData { get { return $"{CustomerName}\r\n" + ((CustomerNIP != "") ? $"{CustomerNIP}\r\n" : "") + CustomerAddress; } }

        [XmlIgnore]
        public string DatesValues { get { return $"{PlaceOfIssue}\r\n{IssueDate}\r\n{SaleDate}\r\n{PaymentDate}\r\n{PaymentForm}"; } }
        [XmlIgnore]
        public string SellerDetails { get { return $"{VendorBankName}\r\n{VendorAccountNumber}\r\n{VendorEmail}\r\n{VendorPhone}"; } }
        [XmlIgnore]
        public string Comment { get { return (Comments.Length > 0 ? ($"Uwagi: {Comments}") : ""); } }
        [XmlIgnore]
        public string PaymentAmount { get { return $"Do zapłaty: {ToPay}"; } }
        [XmlIgnore]
        public string PaymentDetails { get { return $"Słownie: {AmountInWords}\r\n"; } }

        public BillData()
        {
            this.Items = new ObservableCollection<BillItem>();
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

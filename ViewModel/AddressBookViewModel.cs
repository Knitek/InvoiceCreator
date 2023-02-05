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
using InvoiceCreator.View;
using System.Collections.ObjectModel;

namespace InvoiceCreator.ViewModel
{
    class AddressBookViewModel : INotifyPropertyChanged
    {
        ObservableCollection<Customer> customers { get; set; }

        public ObservableCollection<Customer> Customers
        {
            get
            {
                return customers;
            }
            set
            {
                customers = value;
                RaisePropertyChanged("Customers");
            }
        }
        
        public CommandBase AddressBookCommand { get; set; }

        public AddressBookViewModel()
        {
            customers = new ObservableCollection<Customer>()
            {
                new Customer(){Name = "name1", NIP= "23", Address = "234 address test 1"},
                new Customer(){Name = "name2", NIP= "323", Address = "234 ad3254dress" +System.Environment.NewLine+
                " test 324"}
            };
        }
        private void OpenAddressBook()
        {
            AddressBookView addressBookView = new AddressBookView();
            addressBookView.Show();
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InvoiceCreator.Model;
using InvoiceCreator.ViewModel;

namespace InvoiceCreator.View
{
    /// <summary>
    /// Interaction logic for AddressBookView.xaml
    /// </summary>
    public partial class AddressBookView : Window
    {
        AddressBookViewModel viewModel = new AddressBookViewModel();
        public AddressBookView()
        {
            
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

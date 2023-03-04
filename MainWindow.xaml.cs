using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InvoiceCreator.Controls;
using InvoiceCreator.ViewModel;

namespace InvoiceCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BillViewModel billViewModel { get; set; }
        public MainWindow()
        {
            SetLanguageDictionary();
            InitializeComponent();
            billViewModel = new BillViewModel();
            billViewModel.Resources = this.Resources;
            this.DataContext = billViewModel;
        }
        private void SetLanguageDictionary()
        {
            ResourceDictionary defaultRes = new ResourceDictionary()
            {
                Source = new Uri("..\\Resources\\StringResources.xaml", UriKind.Relative),
            };
            this.Resources.MergedDictionaries.Add(defaultRes);
        
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml",UriKind.Relative);
                    break; 
                case "pl-PL":
                    dict.Source = new Uri("..\\Resources\\StringResources.pl.xaml",UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\StringResources.xaml",UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}

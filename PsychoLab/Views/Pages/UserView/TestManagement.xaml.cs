using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for TestManagement.xaml
    /// </summary>
    public partial class TestManagement : Page
    {
        public List<Client> listClients { get; set; }

        public TestManagement()
        {
            InitializeComponent();
            listClients = AppData.db.Clients.ToList();
            cmbListClient.ItemsSource = listClients.Select(item => item.GetData).ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

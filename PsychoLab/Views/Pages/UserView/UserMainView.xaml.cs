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

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for UserMainView.xaml
    /// </summary>
    public partial class UserMainView : Page
    {
        public string FullName { get; set; }
        public UserMainView(string fullName)
        {
            InitializeComponent();
            FullName = fullName;
            this.DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblCurrentUser.Content = FullName;
        }

        private void btnManageClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageClientView());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnTesting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TestManagement());
        }

        private void btnManagementTests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagementTestsView());
        }
    }
}

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
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.AdminView
{
    /// <summary>
    /// Interaction logic for AdminMainView.xaml
    /// </summary>
    public partial class AdminMainView : Page
    {
        public User user { get; set; }
        public AdminMainView(User user)
        {
            InitializeComponent();
            this.user = user;
            this.DataContext = this;
            currentUser.Content = user.FullName;
        }

        private void btnManageUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageUsersPage(user));
        }
    }
}

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

namespace PsychoLab.Views.Pages.AdminView
{
    /// <summary>
    /// Interaction logic for ManageUsersPage.xaml
    /// </summary>
    public partial class ManageUsersPage : Page
    {
        public ManageUsersPage()
        {
            InitializeComponent();
            DataLoad();
        }
        private void DataLoad()
        {
            dataUserListView.ItemsSource = AppData.db.Users.ToList();
        }
        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

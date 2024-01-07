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
using PsychoLab.Views.Windows;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ManagementTestsView.xaml
    /// </summary>
    public partial class ManagementTestsView : Page
    {
        public ManagementTestsView()
        {
            InitializeComponent();
        }

        private void btnAddTest_Click(object sender, RoutedEventArgs e)
        {
            AddTestWindow window = new AddTestWindow();
            window.ShowDialog();
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TestViewList.ItemsSource = AppData.db.PsychologicalTests.ToList();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (PsychologicalTest)TestViewList.SelectedItem;
            if(selectedItem != null)
            {
                ManageTestWindow manageTestWindow = new ManageTestWindow(selectedItem.TestID);
                manageTestWindow.ShowDialog();
            }
        }
    }
}

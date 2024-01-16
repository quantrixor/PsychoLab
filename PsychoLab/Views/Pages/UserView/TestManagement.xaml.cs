using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for TestManagement.xaml
    /// </summary>
    public partial class TestManagement : Page
    {
        public Client Client { get; set; }
        public Session Session { get; set; }
        public TestManagement(Client client, Session session)
        {
            InitializeComponent();
            LoadClients();
            LoadTests();
            Client = client;
            Session = session;
            if(Client.ClientID != 0)
            {
                cmbListClient.SelectedItem = Client;
            }
        }
        private void LoadClients()
        {
            // Загрузка клиентов из базы данных
            cmbListClient.ItemsSource = AppData.db.Clients.ToList();
        }

        private void LoadTests()
        {
            // Загрузка тестов из базы данных
            cmbListTest.ItemsSource = AppData.db.PsychologicalTests.ToList();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnStartTesting_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = cmbListClient.SelectedItem as Client;
            var selectedTest = cmbListTest.SelectedItem as PsychologicalTest;

            if (selectedClient == null || selectedTest == null)
            {
                MessageBox.Show("Необходимо выбрать клиента и тест.");
                return;
            }

            // Открытие окна тестирования
            NavigationService.Navigate(new TestPassingView(selectedClient.ClientID, selectedTest.TestID, Session));

        }
    }
}

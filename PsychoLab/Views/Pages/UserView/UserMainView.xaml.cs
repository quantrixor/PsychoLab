using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for UserMainView.xaml
    /// </summary>
    public partial class UserMainView : Page
    {
        // Объявили сущность пользователя, чтобы из предыдущего вью принять объект пользователя
        // Это нам поможет определить какой именно пользователь прошел аутентификацию
        public User User { get; set; }
        public UserMainView(User user)
        {
            InitializeComponent();
            User = user;
            this.DataContext = this;
        }
        // Загрзука данных
        // Определяем авторизованного пользователя
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblCurrentUser.Content = User.FullName;
        }
        // Переход на страницу управления клиентами
        private void btnManageClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageClientView());
        }
        // Выход на страницу аутентификации
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        // Переход на страницу управления тестированием пациентов
        private void btnTesting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TestManagement());
        }
        // Переход на страницу управления тестами
        private void btnManagementTests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagementTestsView());
        }
        // Переход на страницу управления сеансами
        private void btnManageSession_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageSessionView());
        }
        // Переход на страницу управления расписанием
        private void btnManageSchedulen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageScheduleView());
        }
    }
}

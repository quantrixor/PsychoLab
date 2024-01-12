using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PsychoLab.Context;
using PsychoLab.Model;
using PsychoLab.Views.Windows.AdminWindows;

namespace PsychoLab.Views.Pages.AdminView
{
    /// <summary>
    /// Interaction logic for ManageUsersPage.xaml
    /// </summary>
    public partial class ManageUsersPage : Page
    {
        public User user { get; set; }
        public User selectedUser = null;
        public ManageUsersPage(User user)
        {
            InitializeComponent();
            this.user = user;
            DataContext = this;
            DataLoad();
        }

        private void DataLoad()
        {
            dataUserListView.ItemsSource = AppData.db.Users.ToList();
        }


        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dataUserListView.ItemsSource = AppData.db.Users.Where(item => item.FirstName.Contains(txbSearch.Text) ||
            item.LastName.Contains(txbSearch.Text) || item.MiddleName.Contains(txbSearch.Text) || item.Username.Contains(txbSearch.Text)
            || item.Email.Contains(txbSearch.Text)).ToList(); ;
        }

        private void btnRegistrationUser_Click(object sender, RoutedEventArgs e)
        {
            UserDataManageWindow userWindow = new UserDataManageWindow(new User());
            userWindow.ShowDialog();

        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            selectedUser = (User)dataUserListView.SelectedItem;
            if (selectedUser != null)
            {
                UserDataManageWindow userWindow = new UserDataManageWindow(selectedUser);
                userWindow.ShowDialog();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataLoad();
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedUser = dataUserListView.SelectedItem as User;
                if (selectedUser != null)
                {
                    if (selectedUser.UserID == user.UserID)
                    {
                        MessageBox.Show($"Невозможно удалить выбранного пользователя {selectedUser.Username}, так как его сессия активна!",
                            "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (MessageBox.Show("Вы действительно хотите удалить данные выбранного пользователя? Учтите, что данные потеряются навсегда.", "Подтвердите действие.", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        AppData.db.Users.Remove(selectedUser);
                        AppData.db.SaveChanges();
                        Page_Loaded(null, null);
                        MessageBox.Show("Пользователь успешно удален из базы данных.", "Операция прошла успешно.", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Отображение сообщения об ошибке с внутренним исключением
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                MessageBox.Show($"{ex.Message}\nInner exception: {innerExceptionMessage}", ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        // Обновление данных
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Page_Loaded(null, null);
        }
    }
}

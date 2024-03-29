﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            currentUserFirstname.Content = User.FirstName;
            currentUserLastname.Content = User.LastName;
            currentUserEmail.Content = User.Email;
            LoadImageFromDatabase(User);
        }
        private void LoadImageFromDatabase(User user)
        {
            if (user.PicUser != null && user.PicUser.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(user.PicUser))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();

                    Dispatcher.Invoke(() => {
                        picCurrentUser.Source = image;
                    });
                }
            }
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
            NavigationService.Navigate(new TestManagement(new Client(), new Session()));
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

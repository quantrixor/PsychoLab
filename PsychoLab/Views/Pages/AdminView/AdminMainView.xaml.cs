﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using PsychoLab.Model;
using PsychoLab.Views.Pages.UserView;
using PsychoLab.Views.Windows;

namespace PsychoLab.Views.Pages.AdminView
{
    /// <summary>
    /// Interaction logic for AdminMainView.xaml
    /// </summary>
    public partial class AdminMainView : Page
    {
        public User User { get; set; }
        public AdminMainView(User user)
        {
            InitializeComponent();
            this.User = user;
            this.DataContext = this;
            currentUserFirstname.Content = user.FirstName;
            currentUserLastname.Content = user.LastName;
            currentUserEmail.Content = user.Email;
            LoadImageFromDatabase(user);
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

        private void btnManageUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageUsersPage(User));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnManageTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManagementTestsView());
        }
    }
}

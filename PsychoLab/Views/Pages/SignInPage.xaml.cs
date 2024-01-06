using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PsychoLab.Context;
using PsychoLab.Model;
using PsychoLab.Views.Pages.AdminView;
using PsychoLab.Views.Pages.UserView;

namespace PsychoLab.Views.Pages
{
    /// <summary>
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // Происходит аутентификация пользователя
            try
            {
                switch(AuthenticateUser(txbUsername.Text, psbPassword.Password).role)
                {
                    case "Администратор":
                        NavigationService.Navigate(new AdminMainView());
                        break;
                    case "Психолог":
                        NavigationService.Navigate(new UserMainView());
                        break;
                    case null:
                        MessageBox.Show("Пользователь с такими данными не найден! Пожалуйста, убедитесь, что вводите данные правильно и повторите попытку.",
                            "Пользователь не найден.", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
            }
        }

        // Метод аутентификации пользователя по логину и паролю и определению роли
        public (User user, string role) AuthenticateUser(string username, string password)
        {
            // Найти пользователя по имени пользователя и паролю
            var user = AppData.db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                var userRoles = user.Roles.Select(r => r.RoleName).ToList();
                string role = userRoles.FirstOrDefault();

                return (user, role);
            }

            return (null, null);
        }
    }
}

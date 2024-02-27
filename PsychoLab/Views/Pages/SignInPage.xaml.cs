using System;
using System.Collections.Generic;
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
        // Метод аутентификации пользователя по логину и паролю и определению роли
        private (User user, string role) AuthenticateUser(string username, string password)
        {
            // Поиск пользователя по имени и паролю
            var user = AppData.db.Users
                            .Include(u => u.Roles) // Загрузка ролей
                            .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Получение роли пользователя. Предполагается, что у пользователя только одна роль.
                var role = user.Roles.Select(r => r.RoleName).FirstOrDefault(); // Первая роль или null, если ролей нет
                return (user, role);
            }
            return (null, null);
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            // Происходит аутентификация пользователя
            try
            {
                var (user, role) = AuthenticateUser(txbUsername.Text, psbPassword.Password);
                if (user != null)
                {
                    switch (role) // Используйте переменную role для свитча
                    {
                        case "Administrator":
                            NavigationService.Navigate(new AdminMainView(user));
                            break;
                        case "Psychologist":
                            NavigationService.Navigate(new UserMainView(user)); // Используйте user.FullName
                            break;
                        default:
                            MessageBox.Show("Пользователь с такими данными не найден! Пожалуйста, убедитесь, что вы вводите данные правильно и повторите попытку.",
                                "Пользователь не найден.", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void GuideButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start($@"{Environment.CurrentDirectory}\GuideDocs\Guide.pdf");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Windows;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Windows.AdminWindows
{
    /// <summary>
    /// Interaction logic for UserDataManageWindow.xaml
    /// </summary>
    public partial class UserDataManageWindow : Window
    {
        public User user { get; set; }
        public UserDataManageWindow(User user)
        {
            InitializeComponent();
            this.user = user ?? new User();

            cmbRole.ItemsSource = AppData.db.Roles.ToList();
            DataContext = this;
            if (this.user.UserID > 0) // Проверка, что у пользователя есть ID
            {
                // Загрузка данных пользователя в форму, если он существует
                txbLastname.Text = this.user.LastName;
                txbFirstname.Text = this.user.FirstName;
                txbMiddlename.Text = this.user.MiddleName;
                txbUsername.Text = this.user.Username;
                txbEmail.Text = this.user.Email;
                txbPassword.Text = this.user.Password;

                if (this.user.Roles != null && this.user.Roles.Any())
                {
                    var userRole = this.user.Roles.FirstOrDefault(); // Получаем первую роль пользователя
                    cmbRole.SelectedItem = userRole; // Устанавливаем выбранную роль в ComboBox
                }
            }
        }
        public bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            bool isValid = Regex.IsMatch(email, pattern);

            return isValid;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbUsername.Text) || string.IsNullOrWhiteSpace(txbPassword.Text) ||
                string.IsNullOrWhiteSpace(txbFirstname.Text) || string.IsNullOrWhiteSpace(txbLastname.Text) ||
                string.IsNullOrWhiteSpace(txbEmail.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поля ввода данных пользователя.", "Недопустимое значение!",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                if(AppData.db.Users.Count(item => item.Username == txbUsername.Text || item.Email == txbEmail.Text) > 0)
                {
                    MessageBox.Show($"Пользователь {txbUsername} уже существует в базы данных.", "Ошибка!", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if(ValidateEmail(txbEmail.Text) == false)
                {
                    MessageBox.Show("Некорректный формат электронной почты!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Прежде чем добавлять пользователя, получаем выбранную роль из этого же контекста
                var selectedRole = (Role)cmbRole.SelectedItem;
                var roleInCurrentContext = AppData.db.Roles.Find(selectedRole.RoleID);

                if (roleInCurrentContext != null)
                {
                    var user = new User
                    {
                        Username = txbUsername.Text,
                        FirstName = txbFirstname.Text,
                        LastName = txbLastname.Text,
                        MiddleName = txbMiddlename.Text,
                        Email = txbEmail.Text,
                        Password = txbPassword.Text,
                        Roles = new HashSet<Role>() { roleInCurrentContext }
                    };

                    AppData.db.Users.Add(user);
                    AppData.db.SaveChanges();
                    MessageBox.Show("Пользователь зарегестрирован в базе данных.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Отображение сообщения об ошибке с внутренним исключением
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                MessageBox.Show($"{ex.Message}\nInner exception: {innerExceptionMessage}", ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

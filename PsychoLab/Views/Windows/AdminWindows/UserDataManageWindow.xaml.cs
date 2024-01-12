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
            // Проверка на заполненность полей
            if (string.IsNullOrWhiteSpace(txbUsername.Text) || string.IsNullOrWhiteSpace(txbPassword.Text) ||
                string.IsNullOrWhiteSpace(txbFirstname.Text) || string.IsNullOrWhiteSpace(txbLastname.Text) ||
                string.IsNullOrWhiteSpace(txbEmail.Text) || !ValidateEmail(txbEmail.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поля ввода данных пользователя и убедитесь, что формат электронной почты корректен.", "Недопустимое значение!",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Если пользователь уже существует (редактирование)
                if (this.user.UserID > 0)
                {
                    // Обновляем данные пользователя
                    var userToUpdate = AppData.db.Users.Find(this.user.UserID);
                    if (userToUpdate != null)
                    {
                        userToUpdate.Username = txbUsername.Text;
                        userToUpdate.FirstName = txbFirstname.Text;
                        userToUpdate.LastName = txbLastname.Text;
                        userToUpdate.MiddleName = txbMiddlename.Text;
                        userToUpdate.Email = txbEmail.Text;
                        userToUpdate.Password = txbPassword.Text;

                        // Обновление ролей пользователя
                        userToUpdate.Roles.Clear();
                        var selectedRole = cmbRole.SelectedItem as Role;
                        if (selectedRole != null)
                        {
                            userToUpdate.Roles.Add(selectedRole);
                        }

                        AppData.db.SaveChanges();
                        MessageBox.Show("Данные пользователя обновлены.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    // Создание нового пользователя (добавление)
                    var newUser = new User
                    {
                        Username = txbUsername.Text,
                        FirstName = txbFirstname.Text,
                        LastName = txbLastname.Text,
                        MiddleName = txbMiddlename.Text,
                        Email = txbEmail.Text,
                        Password = txbPassword.Text,
                        Roles = new HashSet<Role>()
                    };

                    var selectedRole = cmbRole.SelectedItem as Role;
                    if (selectedRole != null)
                    {
                        newUser.Roles.Add(selectedRole);
                    }

                    AppData.db.Users.Add(newUser);
                    AppData.db.SaveChanges();
                    MessageBox.Show("Пользователь зарегистрирован в базе данных.", "Операция прошла успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close();
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

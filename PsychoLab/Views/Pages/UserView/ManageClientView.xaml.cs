using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ManageClientView.xaml
    /// </summary>
    public partial class ManageClientView : Page
    {
        public Client client { get; set; }
        public List<Gender> ListGenders { get; set; }
        public ManageClientView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListGenders = AppData.db.Genders.ToList();
            cmbGender.ItemsSource = ListGenders.Select(item => item.Title).ToList();
            GetClients();
        }

        private void btnSaveData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!AreClientDetailsValid())
                {
                    return; // Проверка деталей клиента и вывод соответствующих сообщений об ошибке
                }

                if (client == null)
                {
                    client = new Client();
                }

                if (IsClientDuplicate(client))
                {
                    MessageBox.Show("Клиент с таким адресом электронной почты или номером телефона уже существует.", "Дублирующийся клиент", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SaveClient(client); // Обновляем данные клиента
                SaveOrUpdateClient(client); // Сохраняем или обновляем клиента в базе данных
                MessageBox.Show("Данные клиента успешно сохранены.", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                GetClients();
                ClearTextBox();
            }
            catch (DbEntityValidationException ex)
            {
                DisplayValidationErrors(ex);
            }
        }
        public bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            bool isValid = Regex.IsMatch(email, pattern);

            return isValid;
        }
        private void GetClients()
        {
            listDataClient.ItemsSource = AppData.db.Clients.ToList();
        }
        private bool AreClientDetailsValid()
        {
            if (string.IsNullOrWhiteSpace(txbFirstname.Text)
                || string.IsNullOrWhiteSpace(txbLastname.Text)
                || string.IsNullOrWhiteSpace(txbEmail.Text)
                || string.IsNullOrWhiteSpace(txbPhone.Text)
                || cmbGender.SelectedItem == null
                || string.IsNullOrWhiteSpace(txbMiddlename.Text)
                || ptDateOfBirth.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!ValidateEmail(txbEmail.Text))
            {
                MessageBox.Show("Формат электронной почты недействителен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Возможно, добавить здесь дополнительные проверки...

            return true; // Все проверки пройдены
        }

        private bool IsClientDuplicate(Client client)
        {
            // Проверяем, существует ли уже клиент с таким же Email или Phone, исключая текущего клиента из поиска
            return AppData.db.Clients.Any(c => (c.Email == txbEmail.Text || c.Phone == txbPhone.Text) && c.ClientID != client.ClientID);
        }

        private void DisplayValidationErrors(DbEntityValidationException ex)
        {
            var errorMessages = new StringBuilder();

            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    errorMessages.AppendLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                }
            }

            MessageBox.Show(errorMessages.ToString(), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SaveClient(Client client)
        {

            client.FirstName = txbFirstname.Text;
            client.LastName = txbLastname.Text;
            client.MiddleName = txbMiddlename.Text;
            client.Email = txbEmail.Text;
            client.Phone = txbPhone.Text;
            client.DateOfBirth = ptDateOfBirth.SelectedDate.HasValue ? ptDateOfBirth.SelectedDate.Value.Date : (DateTime?)null;

            var selectedGender = cmbGender.SelectedItem as string;
            var gender = AppData.db.Genders.FirstOrDefault(g => g.Title == selectedGender);
            if (gender != null)
            {
                client.Gender = gender; // Устанавливаем связанный объект Gender, а не IDGender
            }

            // Установка дат создания или обновления
            DateTime currentTime = DateTime.UtcNow;
            if (client.ClientID == 0)
            {
                client.CreatedAt = currentTime;
                client.CreatedBy = GetCurrentUserId();
            }
            else
            {
                client.UpdatedAt = currentTime;
                client.UpdatedBy = GetCurrentUserId();
            }
        }

        private void SaveOrUpdateClient(Client client)
        {
            // Теперь этот метод отвечает за добавление нового клиента или обновление существующего в базе данных
            if (client.ClientID == 0)
            {
                AppData.db.Clients.Add(client); // Добавляем нового клиента, если его ID равен 0
            }
            else
            {
                AppData.db.Entry(client).State = EntityState.Modified; // Обновляем существующего клиента, если у него уже есть ID
            }

            AppData.db.SaveChanges(); // Сохраняем изменения в базе данных
        }

        private int GetCurrentUserId()
        {
            return 1;
        }

        private void ClearTextBox()
        {
            txbEmail.Text = "";
            txbFirstname.Text = "";
            txbLastname.Text = "";
            txbMiddlename.Text = "";
            ptDateOfBirth.SelectedDate = null;
            txbPhone.Text = "";
            cmbGender.Text = "";
            client = null;

        }
        private void DeleteClient(Client client)
        {
            if (client == null)
            {
                MessageBox.Show("Клиентский объект имеет значение null.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого клиента?", "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    AppData.db.Clients.Remove(client);
                    AppData.db.SaveChanges();
                    MessageBox.Show("Данные клиента успешно удалены.", "Удалено", MessageBoxButton.OK, MessageBoxImage.Information);
                    GetClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting client: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private List<Client> SearchClient(string keyword)
        {
            List<Client> searchResults = new List<Client>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.ToLower();

                searchResults = AppData.db.Clients
                    .Where(client =>
                        client.FirstName.ToLower().Contains(search) ||
                        client.LastName.ToLower().Contains(search) ||
                        client.Email.ToLower().Contains(search) ||
                        client.Phone.ToLower().Contains(search))
                    .ToList();
            }
            else
            {
                searchResults = AppData.db.Clients.ToList();
            }

            return searchResults;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            client = listDataClient.SelectedItem as Client;
            if (client != null)
            {
                txbEmail.Text = client.Email;
                txbFirstname.Text = client.FirstName;
                txbMiddlename.Text = client.MiddleName;
                txbLastname.Text = client.LastName;
                txbPhone.Text = client.Phone;
                ptDateOfBirth.SelectedDate = client.DateOfBirth;

                // Установка выбранного пола клиента в ComboBox
                cmbGender.SelectedItem = client.Gender?.Title;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            client = (Client)listDataClient.SelectedItem;
            DeleteClient(client);
        }

        private void txbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            listDataClient.ItemsSource = SearchClient(txbSearch.Text);
        }

        private void txbPhone_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;
            string text = textBox.Text + e.Text;

            if (text.Length == 1)
            {
                textBox.Text = "+7 (";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 8)
            {
                textBox.Text += ") ";
                textBox.SelectionStart = textBox.Text.Length;
            }
            else if (text.Length == 13 || text.Length == 16)
            {
                textBox.Text += "-";
                textBox.SelectionStart = textBox.Text.Length;
            }

            if (textBox.Text.Length >= 18)
            {
                e.Handled = true;
            }
        }

        private void txbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.SelectionStart >= 18)
            {
                textBox.SelectionStart = 18;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}

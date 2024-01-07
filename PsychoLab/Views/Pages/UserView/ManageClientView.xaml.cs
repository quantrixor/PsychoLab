using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
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
                if (string.IsNullOrWhiteSpace(txbFirstname.Text)
               || string.IsNullOrWhiteSpace(txbLastname.Text)
               || string.IsNullOrWhiteSpace(txbEmail.Text)
               || string.IsNullOrWhiteSpace(txbPhone.Text)
               || string.IsNullOrWhiteSpace(cmbGender.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (client == null)
                {
                    client = new Client();
                    SaveClient(client);
                    AppData.db.Clients.Add(client);
                }
                else
                {
                    SaveClient(client);
                }

                AppData.db.SaveChanges();
                MessageBox.Show("Данные клиента успешно сохранены.", "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                GetClients();
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
        private void GetClients()
        {
            listDataClient.ItemsSource = AppData.db.Clients.ToList();
        }

        private void SaveClient(Client client)
        {
           
            client.FirstName = txbFirstname.Text;
            client.LastName = txbLastname.Text;
            client.Email = txbEmail.Text;
            client.Phone = txbPhone.Text;

            var selectedGenderTitle = cmbGender.Text;
            var gender = AppData.db.Genders.FirstOrDefault(item => item.Title == selectedGenderTitle);
            if (gender != null)
            {
                client.Gender = gender;
            }
            else
            {
                MessageBox.Show("Выбранный пол не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            client = (Client)listDataClient.SelectedItem;
            if (client != null)
            {
                txbEmail.Text = client.Email;
                txbFirstname.Text = client.FirstName;
                txbLastname.Text = client.LastName;
                txbPhone.Text = client.Phone;


                cmbGender.Text = client.Gender?.Title;
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
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DocumentFormat.OpenXml.Drawing;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : Page
    {
        public Session Session { get; set; }
        public ScheduleView(Session session)
        {
            InitializeComponent();
            Session = session;
            clSessionDate.SelectedDate = DateTime.Today;
            cmbListClient.ItemsSource = AppData.db.Clients.ToList();
            if(Session.SessionID != 0)
            {
                clSessionDate.SelectedDate = Session.SessionDate;
                txbStartTime.Text = Session.StartTime.ToString();
                txbSessionNote.Text = Session.SessionNote;
                cmbListClient.SelectedItem = Session.Client;
            }
        }


        private bool IsNullTextBox()
        {
             if(string.IsNullOrWhiteSpace(txbSessionNote.Text) ||
                string.IsNullOrWhiteSpace(txbStartTime.Text) ||
                clSessionDate.SelectedDate == null || string.IsNullOrWhiteSpace(cmbListClient.Text))
            {
                return true;
            }
             else 
                return false;
        }

        private void btnSaveSession_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNullTextBox())
                {
                    MessageBox.Show("Все поля должны быть заполнены.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                txbStartTime_TextChanged(null, null);
                string startTimeText = txbStartTime.Text;
                TimeSpan startTime;
                bool isValidTime = TimeSpan.TryParse(startTimeText, out startTime);
                if (!isValidTime)
                {
                    MessageBox.Show("Вы ввели неверный формат времени.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (Session.SessionID == 0)
                {
                    Session = new Session()
                    {
                        SessionDate = clSessionDate.SelectedDate.Value,
                        StartTime = startTime,
                        Client = cmbListClient.SelectedItem as Client,
                        SessionNote = txbSessionNote.Text,
                        CreatedAt = DateTime.Today,
                        UpdatedAt = DateTime.Today
                    };
                    AppData.db.Sessions.Add(Session);
                }
                else
                {
                    Session.SessionDate = clSessionDate.SelectedDate.Value;
                    Session.StartTime = startTime;
                    Session.Client = cmbListClient.SelectedItem as Client;
                    Session.SessionNote = txbSessionNote.Text;
                }
                AppData.db.SaveChanges();
                MessageBox.Show("Сеанс успешно добавлен!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(Session.SessionID == 0)
                cmbListClient.ItemsSource = AppData.db.Clients.ToList();
            else
            {
                cmbListClient.SelectedItem = Session.Client;
            }
        }

        private void txbStartTime_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }

            var textBox = sender as TextBox;
            // Автоматически добавляем двоеточие
            if (textBox != null && textBox.Text.Length == 2 && !textBox.Text.Contains(":"))
            {
                textBox.Text += ":";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void txbStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text.Length == 5)
            {
                int hours, minutes;
                if (int.TryParse(textBox.Text.Substring(0, 2), out hours) &&
                    int.TryParse(textBox.Text.Substring(3, 2), out minutes))
                {
                    if (hours > 23 || minutes > 59)
                    {
                        // Обработка ошибочного времени
                        MessageBox.Show("Некорректное время. Введите время в формате ЧЧ:ММ.");
                        textBox.Clear();
                    }
                }
            }
        }
    }
}

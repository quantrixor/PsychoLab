using System;
using System.Windows;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Windows
{
    /// <summary>
    /// Interaction logic for SessionNoteAddWindow.xaml
    /// </summary>
    public partial class SessionNoteAddWindow : Window
    {
        public Session Session { get; set; }
        public SessionNoteAddWindow(Session session)
        {
            InitializeComponent();
            Session = session;
            this.DataContext = this;
        }

        private void btnSaveNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txbSessionNote.Text))
                {
                    MessageBox.Show("Заполните все поля!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                Session.SessionNote = txbSessionNote.Text;
                AppData.db.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Заметка добавлена.", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

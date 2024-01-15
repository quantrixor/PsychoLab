using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ManageScheduleView.xaml
    /// </summary>
    public partial class ManageScheduleView : Page
    {
        public ManageScheduleView()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnAddSchedul_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ScheduleView(new Session()));
        }

        private void btnEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            var selectedSession = listViewSchedule.SelectedItem as Session;
            if (selectedSession != null)
            {
                NavigationService.Navigate(new ScheduleView(selectedSession));
            }
        }
        public IEnumerable<Session> GetUpcomingSessions()
        {
            try
            {
                var currentDateTime = DateTime.UtcNow; // или DateTime.Now, в зависимости от того, как вы сохраняете время в БД
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;


                var upcomingSessions = AppData.db.Sessions
                                              .Where(s => s.SessionDate > currentDate ||
                                                          (s.SessionDate == currentDate && s.StartTime > currentTime))
                                              .ToList();
                return upcomingSessions;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        private void btnDeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedSession = listViewSchedule.SelectedItem as Session;
                if (selectedSession != null)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить этот сеанс?", "Внимание!",
                        MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        AppData.db.Sessions.Remove(selectedSession);
                        AppData.db.SaveChanges();
                        MessageBox.Show("Данные удалены успешно.", "Удалено.", MessageBoxButton.OK, MessageBoxImage.Information);
                        Page_Loaded(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            listViewSchedule.ItemsSource = GetUpcomingSessions();
        }
    }
}

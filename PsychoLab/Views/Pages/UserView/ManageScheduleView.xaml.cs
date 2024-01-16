using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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
                var currentDateTime = DateTime.UtcNow;
                var currentDate = currentDateTime.Date;
                var currentTime = currentDateTime.TimeOfDay;


                var upcomingSessions = AppData.db.Sessions
                                              .Where(s => s.SessionDate > currentDate ||
                                                          (s.SessionDate == currentDate && s.StartTime > currentTime)).Where(s => (bool)!s.IsTestCompleted)
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
        // Метод для вызова перед началом тестирования
        public bool ConfirmTestStart(Session session)
        {
            var sessionStartDateTime = session.SessionDate.Add(session.StartTime);
            if (DateTime.Now < sessionStartDateTime)
            {
                // Предупреждение, что тестирование начинается раньше запланированного времени
                MessageBoxResult result = MessageBox.Show("Тестирование запланировано на " + sessionStartDateTime.ToString() + ". Вы действительно хотите начать тестирование сейчас ? ", "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return result == MessageBoxResult.OK;
            }
            return true; // Если текущее время соответствует или больше времени начала, начинаем без предупреждения
        }
        private void StartTesting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = listViewSchedule.SelectedItem as Session;
                if (selectedItem != null)
                {
                    if (ConfirmTestStart(selectedItem))
                    {
                        NavigationService.Navigate(new TestManagement(selectedItem.Client, selectedItem));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

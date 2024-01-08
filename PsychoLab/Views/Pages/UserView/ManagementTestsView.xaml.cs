using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using PsychoLab.Context;
using PsychoLab.Model;
using PsychoLab.Views.Windows;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ManagementTestsView.xaml
    /// </summary>
    public partial class ManagementTestsView : Page
    {
        public ManagementTestsView()
        {
            InitializeComponent();
        }

        private void btnAddTest_Click(object sender, RoutedEventArgs e)
        {
            AddTestWindow window = new AddTestWindow();
            window.ShowDialog();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TestViewList.ItemsSource = AppData.db.PsychologicalTests.ToList();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (PsychologicalTest)TestViewList.SelectedItem;
            if(selectedItem != null)
            {
                ManageTestWindow manageTestWindow = new ManageTestWindow(selectedItem.TestID);
                manageTestWindow.ShowDialog();
            }
        }
        private void DeleteTest(int testId)
        {
            try
            {
                // Найти тест по ID
                var testToDelete = AppData.db.PsychologicalTests
                .Include("TestQuestions.TestAnswers")
                .FirstOrDefault(t => t.TestID == testId);


                if (testToDelete == null)
                {
                    MessageBox.Show("Тест не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Удаление всех связанных ответов и вопросов
                foreach (var question in testToDelete.TestQuestions.ToList())
                {
                    AppData.db.TestAnswers.RemoveRange(question.TestAnswers);
                }

                AppData.db.TestQuestions.RemoveRange(testToDelete.TestQuestions);

                // Удаление самого теста
                AppData.db.PsychologicalTests.Remove(testToDelete);

                // Сохранение изменений в базе данных
                AppData.db.SaveChanges();

                MessageBox.Show("Тест и все связанные вопросы и ответы были удалены.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке удаления теста: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить тест и все вопросы с ответами? Данным будут удалены без возможности восстановления.", "Внимание!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
            var selectedItem = (PsychologicalTest)TestViewList.SelectedItem;
            if (selectedItem != null)
            {
                DeleteTest(selectedItem.TestID);
                Page_Loaded(null, null);
            }
        }
    }
}

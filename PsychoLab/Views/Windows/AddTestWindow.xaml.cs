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
using System.Windows.Shapes;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Windows
{
    /// <summary>
    /// Interaction logic for AddTestWindow.xaml
    /// </summary>
    public partial class AddTestWindow : Window
    {
        public AddTestWindow()
        {
            InitializeComponent();
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            var testName = TestNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(testName))
            {
                MessageBox.Show("Пожалуйста, введите название теста.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning); ;
                return;
            }
            if(AppData.db.PsychologicalTests.Count(item => item.TestName == TestNameTextBox.Text) > 0)
            {
                MessageBox.Show($"Тест с {testName} уже существует.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var test = new PsychologicalTest { TestName = testName };
            AppData.db.PsychologicalTests.Add(test);
            AppData.db.SaveChanges();

            MessageBox.Show($"Тест '{testName}' добавлен.", "Успешно.", MessageBoxButton.OK, MessageBoxImage.Information); ;

            var manageTestWindow = new ManageTestWindow(test.TestID);
            manageTestWindow.Show();

            this.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for TestPassingView.xaml
    /// </summary>
    public partial class TestPassingView : Page
    {
        private readonly int _clientId;
        private readonly int _testId;
        private ObservableCollection<TestQuestion> _questions;
        private ObservableCollection<TestAnswer> _answers;
        private int _currentQuestionIndex;
        private Session Session { get; set; }
        public TestPassingView(int clientId, int testId)
        {
            InitializeComponent();
            _clientId = clientId;
            _testId = testId;
            _currentQuestionIndex = 0;

            LoadQuestions();
            UpdateQuestionDisplay();
            Session = new Session();
            var client = AppData.db.Clients.Find(_clientId);
            if (client == null)
            {
                // Обработка ситуации, когда клиент не найден
                MessageBox.Show("Клиент не найден.");
                return;
            }

            // Создаем сессию и присваиваем найденного клиента
            Session = new Session
            {
                Client = client,
                StartTime = DateTime.Now.TimeOfDay,
                SessionDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                CreatedBy = clientId
            };

            AppData.db.Sessions.Add(Session);
            AppData.db.SaveChanges();
        }
        private void LoadQuestions()
        {
            // Загружаем вопросы теста
            _questions = new ObservableCollection<TestQuestion>(AppData.db.TestQuestions
                .Where(q => q.PsychologicalTest.TestID == _testId)
                .ToList());
        }

        private void UpdateQuestionDisplay()
        {
            if (_questions.Count > 0)
            {
                var currentQuestion = _questions[_currentQuestionIndex];
                CurrentQuestionTextBlock.Text = currentQuestion.QuestionText;

                AnswersListBox.Items.Clear();
                foreach (var answer in currentQuestion.TestAnswers)
                {
                    var radioButton = new RadioButton
                    {
                        Content = answer.AnswerText,
                        Margin = new Thickness(5),
                        Tag = answer.AnswerID
                    };

                    radioButton.Checked += RadioButton_Checked;

                    AnswersListBox.Items.Add(radioButton);
                }
            }
        }

        private Dictionary<int, int> _selectedAnswers = new Dictionary<int, int>();

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                int questionId = _questions[_currentQuestionIndex].QuestionID;
                int selectedAnswerId = (int)radioButton.Tag;
                _selectedAnswers[questionId] = selectedAnswerId;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                UpdateQuestionDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            int currentQuestionId = _questions[_currentQuestionIndex].QuestionID;
            if (!_selectedAnswers.ContainsKey(currentQuestionId))
            {
                MessageBox.Show("Please answer the question before proceeding.");
                return;
            }

            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                UpdateQuestionDisplay();
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Определяем клиента в базе данных
                Client client = AppData.db.Clients.Find(_clientId);
                // Определяем тест
                PsychologicalTest psychologicalTest = AppData.db.PsychologicalTests.Find(_testId);

                if (client != null && psychologicalTest != null)
                {

                    foreach (var entry in _selectedAnswers)
                    {
                        int questionId = entry.Key;
                        int answerId = entry.Value;

                        TestResult testResult = new TestResult
                        {
                            Session = Session,
                            TestAnswer = AppData.db.TestAnswers.Find(answerId),
                            TestQuestion = AppData.db.TestQuestions.Find(questionId),
                            PsychologicalTest = psychologicalTest,
                        };

                        AppData.db.TestResults.Add(testResult);
                        TimeSpan timeSpan = DateTime.Now.TimeOfDay;
                        Session.EndTime = timeSpan;

                    }

                    // Сохранение данных в базу данных
                    AppData.db.SaveChanges();
                }
                MessageBox.Show("Ваши результаты сохранены!", "Тест завершен.", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    var childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}

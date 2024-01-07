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
    /// Interaction logic for ManageTestWindow.xaml
    /// </summary>
    public partial class ManageTestWindow : Window
    {
        private readonly int _testId;
        public ManageTestWindow(int testId)
        {
            InitializeComponent(); 
            _testId = testId;
            GetData();
        }
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            var questionText = QuestionTextBox.Text.Trim();
            if (string.IsNullOrEmpty(questionText))
            {
                MessageBox.Show("Please enter a question text.");
                return;
            }

            var test = AppData.db.PsychologicalTests.Find(_testId); // Предполагая, что _testId уже задан
            if (test == null)
            {
                MessageBox.Show("Test not found.");
                return;
            }

            var question = new TestQuestion { QuestionText = questionText, PsychologicalTest = test };
            AppData.db.TestQuestions.Add(question);
            AppData.db.SaveChanges();

            MessageBox.Show("Question has been added.");
            GetData();
        }
        private void AddAnswerOption_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuestion = QuestionsListView.SelectedItem as TestQuestion;
            if (selectedQuestion == null)
            {
                MessageBox.Show("Please select a question.");
                return;
            }

            var answerText = AnswerTextBox.Text.Trim();
            if (string.IsNullOrEmpty(answerText))
            {
                MessageBox.Show("Please enter an answer text.");
                return;
            }

            var answer = new TestAnswer { AnswerText = answerText, IsCorrect = null, TestQuestion = selectedQuestion };
            AppData.db.TestAnswers.Add(answer);
            AppData.db.SaveChanges();

            MessageBox.Show("Answer option has been added.");
            AnswersListView.ItemsSource = AppData.db.TestAnswers
                    .Where(a => a.TestQuestion.QuestionID == selectedQuestion.QuestionID)
                    .ToList();
        }

        private void GetData()
        {
            // Для нового теста инициализируем пустой список вопросов
            if (AppData.db.PsychologicalTests.Find(_testId) == null)
            {
                QuestionsListView.ItemsSource = new List<TestQuestion>();
            }
            else
            {
                // Если тест уже существует, загружаем его вопросы
                QuestionsListView.ItemsSource = AppData.db.TestQuestions
    .           Where(q => q.PsychologicalTest.TestID == _testId)
    .           ToList();
            }
        }
        private void QuestionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный вопрос
            var selectedQuestion = QuestionsListView.SelectedItem as TestQuestion;
            if (selectedQuestion != null)
            {
                // Загружаем ответы для выбранного вопроса
                AnswersListView.ItemsSource = AppData.db.TestAnswers
                    .Where(a => a.TestQuestion.QuestionID == selectedQuestion.QuestionID)
                    .ToList();
            }
            else
            {
                AnswersListView.ItemsSource = null;
            }
        }

    }
}

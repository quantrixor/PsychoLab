using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PsychoLab.Context;
using PsychoLab.Model;

namespace PsychoLab.Views.Windows
{
    /// <summary>
    /// Interaction logic for ManageTestWindow.xaml
    /// </summary>
    public partial class ManageTestWindow : Window
    {
        private TestQuestion selectedQuestion { get; set; }
        private TestAnswer selectedAnswer { get; set; }

        private readonly int _testId;
        public ManageTestWindow(int testId)
        {
            InitializeComponent(); 
            _testId = testId;
            GetData();
        }
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            if(selectedQuestion != null)
            {
                if (QuestionTextBox.Text == "")
                    return;
                selectedQuestion.QuestionText = QuestionTextBox.Text;
                AppData.db.SaveChanges();
                GetData();
                QuestionTextBox.Clear();
                MessageBox.Show("Данные вопроса успешно сохранены.", "Успешно.", MessageBoxButton.OK, MessageBoxImage.Information);
                selectedQuestion = null;
                return;
            }
            var questionText = QuestionTextBox.Text.Trim();
            if (string.IsNullOrEmpty(questionText))
            {
                MessageBox.Show("Пожалуйста, введите текст вопроса.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var test = AppData.db.PsychologicalTests.Find(_testId);
            if (test == null)
            {
                MessageBox.Show("Тест ненайден.");
                return;
            }

            var question = new TestQuestion { QuestionText = questionText, PsychologicalTest = test };
            AppData.db.TestQuestions.Add(question);
            AppData.db.SaveChanges();

            MessageBox.Show("Вопрос добавлен.", "Успешно.", MessageBoxButton.OK, MessageBoxImage.Information);
            GetData();
            QuestionTextBox.Text = "";
        }
        private void AddAnswerOption_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuestion = QuestionsListView.SelectedItem as TestQuestion;
            if(selectedAnswer!= null && selectedQuestion != null)
            {
                if (AnswerTextBox.Text == "")
                    return;
                selectedAnswer.AnswerText = AnswerTextBox.Text;
                AppData.db.SaveChanges();
                AnswersListView.ItemsSource = AppData.db.TestAnswers
                    .Where(a => a.TestQuestion.QuestionID == selectedQuestion.QuestionID)
                    .ToList();
                AnswerTextBox.Clear();
                MessageBox.Show("Данные ответа успешно сохранены.", "Успешно.", MessageBoxButton.OK, MessageBoxImage.Information);
                selectedAnswer = null;
                return;
            }
            if (selectedQuestion == null)
            {
                MessageBox.Show("Пожалуйста, выберите вопрос из списка вопросов.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var answerText = AnswerTextBox.Text.Trim();
            if (string.IsNullOrEmpty(answerText))
            {
                MessageBox.Show("Пожалуйста, выберите ответ из списка ответов.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var answer = new TestAnswer { AnswerText = answerText, IsCorrect = null, TestQuestion = selectedQuestion };
            AppData.db.TestAnswers.Add(answer);
            AppData.db.SaveChanges();

            MessageBox.Show("Вариант ответа успешно добавлен.", "Успешно.", MessageBoxButton.OK, MessageBoxImage.Information);
            AnswersListView.ItemsSource = AppData.db.TestAnswers
                    .Where(a => a.TestQuestion.QuestionID == selectedQuestion.QuestionID)
                    .ToList();
            AnswerTextBox.Clear();
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
        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                selectedQuestion = QuestionsListView.SelectedItem as TestQuestion;
                if (selectedQuestion != null)
                    QuestionTextBox.Text = selectedQuestion.QuestionText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var question = button.DataContext as TestQuestion;
                AppData.db.TestAnswers.RemoveRange(question.TestAnswers);
                AppData.db.TestQuestions.Remove(question);
                AppData.db.SaveChanges();
                GetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            
        }
        private void EditAnswer_Click(object sender, RoutedEventArgs e)
        {
            var selectedQuestion = QuestionsListView.SelectedItem as TestQuestion;
            if (selectedQuestion == null)
                return;
            selectedAnswer = AnswersListView.SelectedItem as TestAnswer;
            if (selectedAnswer != null)
                AnswerTextBox.Text = selectedAnswer.AnswerText;
        }
        private void DeleteAnswer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var answer = button.DataContext as TestAnswer;

                if (answer == null) return;

                AppData.db.TestAnswers.Remove(answer);
                AppData.db.SaveChanges();
                // Обновить список ответов
                QuestionsListView_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении ответа: {ex.Message}");
            }
        }
    }
}

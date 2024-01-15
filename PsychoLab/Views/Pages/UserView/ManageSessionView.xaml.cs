using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using PsychoLab.Context;
using PsychoLab.Model;
using System;
using PsychoLab.Views.Windows;
using System.Collections.Generic;

namespace PsychoLab.Views.Pages.UserView
{
    /// <summary>
    /// Interaction logic for ManageSessionView.xaml
    /// </summary>
    public partial class ManageSessionView : Page
    {
        public ManageSessionView()
        {
            InitializeComponent();
        }

        private void DataLoad()
        {
            listViewSessionData.ItemsSource = GetPastSessions();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public IEnumerable<Session> GetPastSessions()
        {
            // Получаем текущую дату и время в UTC, если данные в базе в UTC, иначе используйте DateTime.Now
            var currentDateTime = DateTime.UtcNow;

            // Подготавливаем текущую дату и время для сравнения
            var currentDate = currentDateTime.Date;
            var currentTime = currentDateTime.TimeOfDay;


            // Извлекаем сеансы, у которых дата меньше текущей, или дата совпадает, но время окончания меньше текущего времени
            var pastSessions = AppData.db.Sessions
                .Where(s => s.SessionDate < currentDate ||
                            (s.SessionDate == currentDate && s.EndTime < currentTime))
                .ToList();

            return pastSessions;

        }


        private void ListViewSessionData_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gridView = listView.View as GridView;
            var actualWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // Вычитаем ширину полосы прокрутки
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                if (gridView.Columns[i].Header.ToString() != "Заметка") // За исключением колонки "Заметка"
                {
                    gridView.Columns[i].Width = actualWidth / (gridView.Columns.Count - 1); // Распределяем ширину равномерно между всеми колонками, кроме последней
                }
                else
                {
                    gridView.Columns[i].Width = actualWidth - (actualWidth / (gridView.Columns.Count - 1)) * (gridView.Columns.Count - 2); // Последней колонке отдаём все оставшееся место
                }
            }
        }
        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var session = button.DataContext as Session; // Убедитесь, что DataContext содержит объект Session

            // Строим название файла
            string fileName = $"{session.Client.FullName} результаты тестирования от {session.SessionDate:dd.MM.yyyy}.docx";

            // Подготавливаем данные для экспорта
            var testResults = AppData.db.TestResults
                                .Where(tr => tr.Session.SessionID == session.SessionID)
                                .ToList();

            // Создаем документ Word
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Заголовок
                var titleParagraph = body.AppendChild(new Paragraph());
                titleParagraph.AppendChild(new Run(new Text($"Пациент {session.Client.FullName}, тестирование от {session.SessionDate:dd.MM.yyyy}")));

                // Время начала и конца теста
                titleParagraph.AppendChild(new Run(new Text($"Начало теста - {session.StartTime.ToString("hh\\:mm\\:ss")}")));
                titleParagraph.AppendChild(new Run(new Text($"Конец - {session.EndTime.ToString("hh\\:mm\\:ss")}")));



                // Перебираем результаты теста и добавляем их в документ
                foreach (var result in testResults)
                {
                    // Добавляем вопрос
                    var questionParagraph = body.AppendChild(new Paragraph(new Run(new Text($"Вопрос {result.TestQuestion.QuestionID}: {result.TestQuestion.QuestionText}"))));
                    questionParagraph.AppendChild(new Run(new Break())); // Разрыв строки после вопроса

                    // Добавляем ответ
                    var answerParagraph = body.AppendChild(new Paragraph(new Run(new Text($"Ответ: {result.TestAnswer.AnswerText}"))));
                    answerParagraph.AppendChild(new Run(new Break())); // Разрыв строки после ответа

                    // Добавляем пустой абзац для пространства между вопросами и ответами
                    body.AppendChild(new Paragraph(new Run(new Text(""))));
                }

            }

            MessageBox.Show($"Данные о тестах успешно экспортированы в файл '{fileName}'.", "Данные успешно выгружены.", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddNotes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedSession = listViewSessionData.SelectedItem as Session;
                if (selectedSession != null)
                {
                    SessionNoteAddWindow sessionNote = new SessionNoteAddWindow(selectedSession);
                    sessionNote.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Выберите сеанс, чтобы добавить заметку.", "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataLoad();
        }

        private void btnDataUpdate_Click(object sender, RoutedEventArgs e)
        {
            DataLoad();
        }
    }
}

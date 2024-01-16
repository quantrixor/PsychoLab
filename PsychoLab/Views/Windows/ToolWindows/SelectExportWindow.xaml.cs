using System;
using System.Linq;
using System.Windows;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph; // Псевдоним для Paragraph из OpenXml
using WRun = DocumentFormat.OpenXml.Wordprocessing.Run; // Псевдоним для Run из OpenXml
using DocumentFormat.OpenXml;
using PsychoLab.Context;
using PsychoLab.Model;
using System.ComponentModel;
using System.Windows.Controls;



namespace PsychoLab.Views.Windows.ToolWindows
{
    /// <summary>
    /// Interaction logic for SelectExportWindow.xaml
    /// </summary>
    public partial class SelectExportWindow : Window
    {
        private Client Client { get; set; }
        public SelectExportWindow(Client client)
        {
            InitializeComponent();
            Client = client;
        }

        public void CreateClientSessionReport(Client client, BackgroundWorker worker)
        {

            if (client == null)
            {
                // Обработка ошибки, если клиент не найден
                return;
            }

            var sessions = AppData.db.Sessions.Where(s => s.Client.ClientID == client.ClientID).ToList();
            int totalOperations = sessions.Count() * 2; // Example: calculate total steps for progress
            int completedOperations = 0;


            using (WordprocessingDocument document = WordprocessingDocument.Create($"ClientSessionReport.docx", WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());    // Добавление информации о клиенте
                AddTextToBody(body, $"ID клиента: {client.ClientID}");
                AddTextToBody(body, $"ФИО клиента: {client.FirstName} {client.LastName}");
                AddTextToBody(body, $"Почта и телефон: {client.Email}, {client.Phone}");
                AddTextToBody(body, $"Дата рождения: {client.DateOfBirth:d}");
                AddTextToBody(body, $"Дата создания клиента: {client.CreatedAt:d}");

                // Добавление информации о сеансах и тестах
                foreach (var session in sessions)
                {
                    AddTextToBody(body, $"Дата сеанса: {session.SessionDate:d}");
                    AddTextToBody(body, $"Время начала: {session.StartTime}");
                    AddTextToBody(body, $"Время окончания: {session.EndTime}");
                    AddTextToBody(body, $"Заметка сеанса: {session.SessionNote}");

                    var testResults = session.TestResults;
                    if (testResults.Any())
                    {
                        var firstResult = testResults.First();
                        AddTextToBody(body, $"Название теста: {firstResult.PsychologicalTest.TestName}");

                        foreach (var result in testResults)
                        {
                            AddTextToBody(body, $"Вопрос: {result.TestQuestion.QuestionText}");
                            AddTextToBody(body, $"Ответ: {result.TestAnswer.AnswerText}");
                        }
                    }
                    int percentComplete = (int)((double)completedOperations / totalOperations * 100);
                    worker.ReportProgress(percentComplete);
                }

                // Сохранение документа
                mainPart.Document.Save();
            }


        }
        private void AddTextToBody(Body body, string text)
        {
            WParagraph para = body.AppendChild(new WParagraph());
            WRun run = para.AppendChild(new WRun());
            run.AppendChild(new Text(text));
        }
        private volatile bool isCompleted = false;
        public void StartCreateClientSessionReport(Client client)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (sender, e) =>
            {
                try
                {
                    var progress = new Progress<int>(percent =>
                    {
                        // Сообщайте о ходе выполнения только в том случае, если операция не завершена.
                        if (!isCompleted)
                        {
                            worker.ReportProgress(percent);
                        }
                    });

                    CreateClientSessionReport(client, worker);
                }
                catch (Exception ex)
                {
                    e.Result = ex; // Сохранить исключение для события RunWorkerCompleted.
                }
            };

            worker.ProgressChanged += (sender, e) =>
            {
                progressText.Text = $"Progress: {e.ProgressPercentage}%";
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                // Проверить, не привела ли операция к ошибке
                if (e.Error != null)
                {
                    // Обновить TextBlock, чтобы отобразить сообщение об ошибке.
                    progressText.Text = $"Error: {e.Error.Message}";
                    MessageBox.Show($"An error occurred: {e.Error.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Обновить TextBlock, чтобы показать завершение.
                    progressText.Text = "Report creation completed!";
                    MessageBox.Show("Report creation completed!");
                }
                ExportToWord.IsEnabled = true;
            };

            worker.RunWorkerAsync();
        }
        // Экспортируем данные в формат Word
        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            StartCreateClientSessionReport(Client);
        }

        // Экспортируем данные в формат Excel
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        // Экспортируем данные в формат JSON
        private void ExportToJson_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

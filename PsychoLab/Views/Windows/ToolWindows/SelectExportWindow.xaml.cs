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
using OfficeOpenXml;
using System.IO;
using System.IO.Packaging;


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

        public void CreateClientSessionReport(Client client, BackgroundWorker worker, string filePath = null)
        {

            if (client == null)
            {
                // Обработка ошибки, если клиент не найден
                return;
            }

            var sessions = AppData.db.Sessions.Where(s => s.Client.ClientID == client.ClientID).ToList();
            int totalOperations = sessions.Count(); // Пример: подсчитать общее количество шагов для прогресса
            int completedOperations = 0;


            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
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
                MessageBox.Show($"Файл сохранен в: {filePath}", "Экспорт в Word", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }
        private void AddTextToBody(Body body, string text)
        {
            WParagraph para = body.AppendChild(new WParagraph());
            WRun run = para.AppendChild(new WRun());
            run.AppendChild(new Text(text));
        }
        
        private volatile bool isCompleted = false;
        public void StartCreateClientSessionReport(Client client, string type, string filePath = null)
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
                    if (type == "Word")
                        CreateClientSessionReport(client, worker, filePath);
                    else if (type == "Excel")
                        CreateClientExcelReport(client, worker, filePath);
                    else
                        return;
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
                    progressText.Text = "Создание отчета завершено!";
                    MessageBox.Show("Создание отчета завершено!", "Уведомление.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                ExportToWord.IsEnabled = true;
            };

            worker.RunWorkerAsync();
        }

        // Экспортируем данные в формат Word
        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "ClientSessionReport"; // FileName по умолчанию
            saveFileDialog.DefaultExt = ".docx"; // Расширение по умолчанию
            saveFileDialog.Filter = "Word documents (.docx)|*.docx"; // Фильтр по расширению

            // Открыть файловое окно проводника
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                // Получить выбранное имя файла и начать процесс экспорта.
                string filename = saveFileDialog.FileName;
                StartCreateClientSessionReport(Client, "Word", filename);
            }
        }
        public void CreateClientExcelReport(Client client, BackgroundWorker worker, string filePath)
        {
            if (client == null)
            {
                // Если клиент не найден, вернуться
                return;
            }

            var sessions = AppData.db.Sessions.Where(s => s.Client.ClientID == client.ClientID).ToList();
            int totalOperations = sessions.Count(); // For progress calculation
            int completedOperations = 0;

            using (ExcelPackage package = new ExcelPackage())
            {
                // Создать эксель документ
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Client Data");

                // Добавить заголовки
                worksheet.Cells["A1"].Value = "Client ID";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Email";
                worksheet.Cells["D1"].Value = "Phone";
                worksheet.Cells["E1"].Value = "Date of Birth";
                worksheet.Cells["F1"].Value = "Client Since";
                worksheet.Cells["G1"].Value = "Session Date";
                worksheet.Cells["H1"].Value = "Start Time";
                worksheet.Cells["I1"].Value = "End Time";
                worksheet.Cells["J1"].Value = "Session Note";
                worksheet.Cells["K1"].Value = "Test Name";
                worksheet.Cells["L1"].Value = "Question";
                worksheet.Cells["M1"].Value = "Answer";

                int row = 2;

                foreach (var session in sessions)
                {
                    // Информация о клиенте и сеансе должна записываться один раз за сеанс.
                    worksheet.Cells[row, 1].Value = client.ClientID;
                    worksheet.Cells[row, 2].Value = client.FirstName + " " + client.LastName;
                    worksheet.Cells[row, 3].Value = client.Email;
                    worksheet.Cells[row, 4].Value = client.Phone;
                    worksheet.Cells[row, 5].Value = client.DateOfBirth?.ToString("d") ?? "N/A";
                    worksheet.Cells[row, 6].Value = client.CreatedAt.ToString("d");
                    worksheet.Cells[row, 7].Value = session.SessionDate.ToString("d");
                    worksheet.Cells[row, 8].Value = session.StartTime;
                    worksheet.Cells[row, 9].Value = session.EndTime;
                    worksheet.Cells[row, 10].Value = session.SessionNote;

                    var testResults = session.TestResults;
                    if (testResults.Any())
                    {
                        // Здесь записываем только информацию о результатах теста, а не статическую информацию о клиенте/сеансе.
                        foreach (var result in testResults)
                        {
                            worksheet.Cells[row, 11].Value = result.PsychologicalTest.TestName;
                            worksheet.Cells[row, 12].Value = result.TestQuestion.QuestionText;
                            worksheet.Cells[row, 13].Value = result.TestAnswer.AnswerText;
                            row++; // Переход к следующей строке для получения следующего результата теста
                        }
                    }
                    else
                    {
                        row++; // Переход к следующей строке для следующего сеанса, если результатов теста нет.
                    }

                    completedOperations++;
                    int percentComplete = (int)((double)completedOperations / totalOperations * 100);
                    worker.ReportProgress(percentComplete);
                }


                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
                MessageBox.Show($"Файл сохранен в: {filePath}", "Экспорт в Excel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        // Экспортируем данные в формат Excel
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = $"ClientSessionReport_{Client.ClientID}"; // Default file name
            saveFileDialog.DefaultExt = ".xlsx"; // Default file extension
            saveFileDialog.Filter = "Excel documents (.xlsx)|*.xlsx";
            bool? resultDialog = saveFileDialog.ShowDialog();

            if (resultDialog == true)
            {
                // Получить выбранное имя файла и начать процесс экспорта.
                string filename = saveFileDialog.FileName;
                StartCreateClientSessionReport(Client, "Excel", filename);
            }
        }

        // Экспортируем данные в формат JSON
        private void ExportToJson_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данная функция находится в разработке.", "В разработке!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

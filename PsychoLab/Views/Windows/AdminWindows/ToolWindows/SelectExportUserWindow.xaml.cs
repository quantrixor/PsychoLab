using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PsychoLab.Context;
using PsychoLab.Model;
using Microsoft.Win32;
using OfficeOpenXml;

namespace PsychoLab.Views.Windows.AdminWindows.ToolWindows
{
    /// <summary>
    /// Interaction logic for SelectExportUserWindow.xaml
    /// </summary>
    public partial class SelectExportUserWindow : Window
    {
        private List<User> _users { get; set; }
        public SelectExportUserWindow()
        {
            InitializeComponent();
            _users = AppData.db.Users.ToList();
        }
        //$"Word Document (*.docx)|*.docx";
        public string ChooseSaveLocation(string formatFile, string filterFile)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "UserData";
            saveFileDialog.Filter = filterFile;
            saveFileDialog.DefaultExt = formatFile;
            saveFileDialog.AddExtension = true;

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                return saveFileDialog.FileName;
            }
            else
            {
                return null; // Пользователь отменил операцию
            }
        }
        private async void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            string filePath = ChooseSaveLocation("docx", "Word Document (*.docx)|*.docx");
            if(filePath != null)
                await Task.Run(() => ExportUsersToWord(_users, filePath));
        }
        public void ExportUsersToWord(List<User> users, string filePath)
        {
            try
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());
                    int totalUsers = users.Count;

                    foreach (User user in users)
                    {
                        // Добавление данных пользователя
                        AddUserDataToDocument(mainPart.Document.Body, user);

                        // Добавление интервала (пустых строк) после каждого пользователя
                        for (int i = 0; i < 2; i++) // Можно изменить количество пустых строк
                        {
                            mainPart.Document.Body.AppendChild(new Paragraph(new Run(new Text(""))));
                        }
                    }
                }
                MessageBox.Show("Данные успешно экспортировны в Word.", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void AddUserDataToDocument(Body body, User user)
        {
            body.AppendChild(CreateParagraph($"ID: {user.UserID}"));
            body.AppendChild(CreateParagraph($"Имя: {user.FirstName}"));
            body.AppendChild(CreateParagraph($"Фамилия: {user.LastName}"));
            body.AppendChild(CreateParagraph($"Отчество: {user.MiddleName}"));
            body.AppendChild(CreateParagraph($"Почта: {user.Email}"));
            body.AppendChild(CreateParagraph($"Дата создания: {user.CreatedAt.ToString("dd.MM.yyyy HH:mm")}"));
            body.AppendChild(CreateParagraph($"Дата обновления: {(user.UpdatedAt.HasValue ? user.UpdatedAt.Value.ToString("dd.MM.yyyy HH:mm") : "Не обновлялось")}"));
        }

        private Paragraph CreateParagraph(string text)
        {
            Paragraph paragraph = new Paragraph();
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
            return paragraph;
        }

        public void ExportUsersToExcel(List<User> users, string filePath)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // Добавление нового листа в Excel файл
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Users");

                // Заголовки столбцов
                string[] columnHeaders = { "ID", "Имя", "Фамилия", "Отчество", "Почта", "Дата создания", "Дата обновления" };
                for (int i = 0; i < columnHeaders.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = columnHeaders[i];
                }

                int row = 2; // Стартовая строка для данных пользователей
                foreach (User user in users)
                {
                    // Заполнение ячеек данными пользователя
                    worksheet.Cells[row, 1].Value = user.UserID;
                    worksheet.Cells[row, 2].Value = user.FirstName;
                    worksheet.Cells[row, 3].Value = user.LastName;
                    worksheet.Cells[row, 4].Value = user.MiddleName;
                    worksheet.Cells[row, 5].Value = user.Email;
                    worksheet.Cells[row, 6].Value = user.CreatedAt.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[row, 7].Value = user.UpdatedAt.HasValue ? user.UpdatedAt.Value.ToString("dd.MM.yyyy HH:mm") : "Не обновлялось";

                    row++;
                }

                // Автоматическое изменение ширины столбцов по содержимому
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Сохранение Excel файла
                FileInfo fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);
            }

            MessageBox.Show("Данные успешно экспортированы в Excel.", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private async void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            string filePath = ChooseSaveLocation("xlsx", "Excel documents (.xlsx)|*.xlsx");
            if (filePath != null)
                await Task.Run(() => ExportUsersToExcel(_users, filePath));
        }
    }
}

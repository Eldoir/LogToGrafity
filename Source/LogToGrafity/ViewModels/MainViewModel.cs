using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LogToGrafity
{
    class MainViewModel
    {
        public MainViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFile);
            _rowParser = new RowParser();
            _rowAnalyzer = new RowAnalyzer();
        }

        public ICommand OpenFileCommand { get; }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() != true)
                return;

            Result<(string Eps1Content, string Eps2Content)> result = ParseFile(openFileDialog.FileName);
            if (result.IsFailure)
            {
                ShowError(result.Message);
                return;
            }

            ShowSuccess("Now select a folder in which files eps1.csv and eps2.csv will be saved.");

            System.Windows.Forms.FolderBrowserDialog dialog = new()
            {
                RootFolder = System.Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            string eps1FileName = "eps1.csv";
            string eps2FileName = "eps2.csv";
            string directoryPath = dialog.SelectedPath;

            while (DirectoryContains(directoryPath, eps1FileName) || DirectoryContains(directoryPath, eps1FileName))
            {
                if (MessageBox.Show(
                    $"{eps1FileName} or {eps2FileName} already exist. Do you want to overwrite them?",
                    "Overwrite?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return;

                    directoryPath = dialog.SelectedPath;
                }
                else break;
            }

            File.WriteAllText(Path.Join(directoryPath, eps1FileName), result.Value.Eps1Content);
            File.WriteAllText(Path.Join(directoryPath, eps2FileName), result.Value.Eps2Content);
            ShowSuccess("Saved!");
        }

        private static bool DirectoryContains(string directoryPath, string fileName)
        {
            return File.Exists(Path.Join(directoryPath, fileName));
        }

        private static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private Result<(string Eps1Content, string Eps2Content)> ParseFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length < 4)
                return Result.Fail<(string, string)>(
                    "File must be at least 4 lines (2 header lines, 1 line for column names, and at least 1 line of data)");

            List<Row> rows = new();

            for (int i = 3; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                    continue;

                Result<Row> rowResult = _rowParser.Parse(lines[i]);
                if (rowResult.IsFailure)
                    return Result.Fail<(string, string)>(rowResult.Message);

                rows.Add(rowResult.Value);
            }

            (EpsContainer Eps1, EpsContainer Eps2) = _rowAnalyzer.Analyze(rows);

            return Result.Ok((ToString(Eps1), ToString(Eps2)));
        }

        private static string ToString(EpsContainer eps)
        {
            StringBuilder builder = new();

            // Write column names
            builder.AppendLine(string.Join(" ", new[] { "Freq" }.Concat(eps.ColumnNames)));

            // Write rows
            foreach (string freq in eps.Frequencies)
            {
                builder.AppendLine(string.Join(" ", new[] { freq }.Concat(eps.GetValues(freq))));
            }

            return builder.ToString();
        }

        private readonly IRowParser _rowParser;
        private readonly IRowAnalyzer _rowAnalyzer;
    }
}

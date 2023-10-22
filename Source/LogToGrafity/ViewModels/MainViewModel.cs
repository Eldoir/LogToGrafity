using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LogToGrafity
{
    public class MainViewModel : DataModelBase,
        IFileDragEnterHandler,
        IFileDragLeaveHandler,
        IFileDropHandler
    {
        public MainViewModel()
        {
            OpenFileCommand = new DelegateCommand(OpenFile);
            ConvertCommand = new DelegateCommand(ConvertFiles);

            _rowParser = new RowParser();
        }

        public ICommand OpenFileCommand { get; }
        public ICommand ConvertCommand { get; }

        public DragNDropState DragNDropState
        {
            get => _dragNDropState;
            private set => SetValue(ref _dragNDropState, value);
        }
        private DragNDropState _dragNDropState;

        public bool AreColumnsTemperature { get; set; } = true;

        public ObservableCollection<LogFileViewModel> LogFiles => _logFiles;
        private readonly ObservableCollection<LogFileViewModel> _logFiles = new();

        #region Drag'n'drop handlers

        public bool OnDragEnter(string[] filepaths)
        {
            bool accepts = filepaths.Any(IsCorrectFormat);
            DragNDropState = accepts ? DragNDropState.Accept : DragNDropState.Reject;
            return accepts;
        }

        public void OnDragLeave(string[] filepaths)
        {
            DragNDropState = DragNDropState.Idle;
        }

        public void OnDrop(string[] filepaths)
        {
            foreach (string filepath in filepaths)
            {
                if (SupportedFormats.Supports(filepath))
                {
                    AddFile(filepath);
                }
                else
                {
                    ShowWarning($"File {filepath} is not a log file. It will be ignored.");
                }
            }

            OnDragLeave(filepaths);
        }

        #endregion

        private static bool IsCorrectFormat(string filePath)
        {
            return SupportedFormats.Supports(filePath);
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = SupportedFormats.ToFilter(),
                Multiselect = true,
            };
            if (openFileDialog.ShowDialog() != true)
                return;

            foreach (string fileName in openFileDialog.FileNames)
            {
                AddFile(fileName);
            }
        }

        private void ConvertFiles()
        {
            string directoryPath = string.Empty;
            bool allSuccess = true;

            foreach (LogFileViewModel logFile in LogFiles)
            {
                if (!File.Exists(logFile.FilePath))
                {
                    ShowWarning($"File {logFile.FilePath} no longer exists. It will be ignored.");
                    continue;
                }

                Result<(string Eps1Content, string Eps2Content)> result = ParseFile(logFile.FilePath);
                if (result.IsFailure)
                {
                    allSuccess = false;
                    ShowError(result.Message);
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(logFile.FilePath);
                string eps1FileName = $"{fileName}_eps1.csv";
                string eps2FileName = $"{fileName}_eps2.csv";

                if (string.IsNullOrEmpty(directoryPath))
                {
                    ShowSuccess("Select a folder where converted files will be saved.");

                    System.Windows.Forms.FolderBrowserDialog dialog = new()
                    {
                        RootFolder = Environment.SpecialFolder.Desktop,
                        ShowNewFolderButton = true
                    };
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return;

                    directoryPath = dialog.SelectedPath;

                    while (DirectoryContains(directoryPath, eps1FileName) || DirectoryContains(directoryPath, eps2FileName))
                    {
                        // TODO: fix this messagebox appearing only if the files from the FIRST source already exist
                        if (MessageBox.Show(
                            $"Files already exist at that location. Do you want to overwrite them?",
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
                }

                File.WriteAllText(Path.Join(directoryPath, eps1FileName), result.Value.Eps1Content);
                File.WriteAllText(Path.Join(directoryPath, eps2FileName), result.Value.Eps2Content);
            }

            if (allSuccess)
            {
                ShowSuccess($"Files saved under {directoryPath}.");
            }

            #region Local methods

            static bool DirectoryContains(string directoryPath, string fileName)
            {
                return File.Exists(Path.Join(directoryPath, fileName));
            }

            #endregion
        }

        private void AddFile(string filePath)
        {
            if (!LogFiles.Any(f => f.FilePath == filePath))
            {
                LogFileViewModel logFile = new(filePath);
                logFile.OnRemoved += OnRemoveFile;
                LogFiles.Add(logFile);
                RaisePropertyChanged(nameof(LogFiles));
            }
        }

        private void OnRemoveFile(object? sender, EventArgs args)
        {
            if (sender is not LogFileViewModel logFile)
                return;

            logFile.OnRemoved -= OnRemoveFile;
            LogFiles.Remove(logFile);
            RaisePropertyChanged(nameof(LogFiles));
        }

        private static void ShowSuccess(string message)
            => ShowMessage(message, "Success", MessageBoxImage.Information);

        private static void ShowWarning(string message)
            => ShowMessage(message, "Warning", MessageBoxImage.Warning);

        private static void ShowError(string message)
            => ShowMessage(message, "Error", MessageBoxImage.Error);

        private static void ShowMessage(string message, string caption, MessageBoxImage image)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, image);
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

            RowAnalyzer rowAnalyzer = new(EpsContainerStringConverterFactory.Create(AreColumnsTemperature));
            return Result.Ok(rowAnalyzer.Analyze(rows));
        }

        private readonly IRowParser _rowParser;

        private static readonly FileFormatContainer SupportedFormats = new(
            new FileFormat[] { new("Log files", ".log"), new("Text files", ".txt") });
    }
}

using System;
using System.Windows.Input;

namespace LogToGrafity
{
    public class LogFileViewModel
    {
        public LogFileViewModel(string filePath)
        {
            FilePath = filePath;
            RemoveCommand = new DelegateCommand(Remove);
        }

        public string FilePath { get; }
        public ICommand RemoveCommand { get; }
        public event EventHandler? OnRemoved;

        private void Remove()
        {
            OnRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}

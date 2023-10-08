using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogToGrafity
{
    public abstract class DataModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        public bool SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (property is not null && property.Equals(value) || property is null && value is null)
                return false;

            property = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tampleworks.WindowsApplicationBlock.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies derived classes when a property changes.
        /// </summary>
        /// <param name="propertyName">PropertyName</param>
        protected void OnPropertyChanged(string propertyName)
        {
            Debug.Assert(propertyName != null);
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCallerPropertyChanged([CallerMemberName]string propertyName = null)
            => OnPropertyChanged(propertyName);

        /// <summary>
        /// Notifies derived classes when a property changes.
        /// </summary>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Debug.Assert(e != null);
            if (e == null) throw new ArgumentNullException(nameof(e));

            if (PropertyChanged == null) return;

            PropertyChanged(this, e);
        }
    }
}

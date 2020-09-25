using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BitActionSwitch.Editor.Mvvm
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        private readonly List<bool> errors = new List<bool>();
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            onChanged?.Invoke();
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) => this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) => this.PropertyChanged?.Invoke(this, args);

        protected bool HasError => this.errors.Any(x => x);
        
        protected virtual bool SetError(bool isError)
        {
            this.errors.Add(isError);
            return isError;
        }

        protected virtual void ClearErrors()
        {
            this.errors.Clear();
        }
    }
}
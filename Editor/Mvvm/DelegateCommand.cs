using System;
using System.Reflection;
using System.Windows.Input;

namespace BitActionSwitch.Editor.Mvvm
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Action executeMethod;
        private readonly Func<bool> canExecuteMethod;

        public DelegateCommand(Action executeMethod) : this(executeMethod, () => true) { }
        
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
            
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }
        
        public bool CanExecute()
        {
            return this.canExecuteMethod();
        }

        public void Execute()
        {
            this.executeMethod();
        }
        
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute();
        }
        void ICommand.Execute(object parameter)
        {
            this.Execute();
        }

        public event EventHandler CanExecuteChanged;
    }
    
    public sealed class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> executeMethod;
        private readonly Func<T, bool> canExecuteMethod;

        public DelegateCommand(Action<T> executeMethod) : this(executeMethod, (o) => true) { }
        
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod));
            
            var genericTypeInfo = typeof(T).GetTypeInfo();
            
            if (genericTypeInfo.IsValueType)
            {
                if (!genericTypeInfo.IsGenericType || !typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo()))
                {
                    throw new InvalidCastException();
                }
            }
            
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }
        
        public bool CanExecute(T parameter)
        {
            return this.canExecuteMethod(parameter);
        }

        public void Execute(T parameter)
        {
            this.executeMethod(parameter);
        }
        
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute((T)parameter);
        }
        void ICommand.Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameInterface
{
    public class DelegateCommand<T> : ICommand
    {

        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute) : this(execute, () => true) { }

        public DelegateCommand(Action<T> execute, Func<bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            this._execute((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return this._canExecute();
        }

    }
}

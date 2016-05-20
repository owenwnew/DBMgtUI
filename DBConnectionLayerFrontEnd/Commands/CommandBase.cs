using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using DBConnectionLayerFrontEnd.ViewModel;
using System.ComponentModel;


namespace DBConnectionLayerFrontEnd.Commands
{
    internal class CommandBase : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        //cast a initiation
        public CommandBase(Action<object> executeDelegate) : this(executeDelegate, null)
        {

        }

        //constructor
        public CommandBase(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            if (executeDelegate == null)
                throw new ArgumentNullException("executeDelegate");
            _execute = executeDelegate;
            _canExecute = canExecuteDelegate;
        }



        //constructor used in tutorial
        //private FrontEndViewModel _viewModel;
        //public CommandBase(FrontEndViewModel viewModel)
        //{

        //    _viewModel = viewModel;

        //}

        //ICommand interface Members
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);

            //equivalent as
            // if(canExecute == null)
            //{ return ture}
            //else
            // {return _canExecute(obj)}
            //
        }


        //expand the property of CanExecuteChanged method of Eventhandler
        //add and remove the query of Command action by using CommandManager
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {

                CommandManager.RequerySuggested -= value;
            }

        }



        //on Execute go to propertychanged 
        public void Execute(object parameter)
        {
            _execute(parameter);
        }



    }
}

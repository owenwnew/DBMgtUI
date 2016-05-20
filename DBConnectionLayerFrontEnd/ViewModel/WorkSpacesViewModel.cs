using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DBConnectionLayerFrontEnd.Commands;

namespace DBConnectionLayerFrontEnd.ViewModel
{
    public abstract class WorkSpacesViewModel:ViewModelBase
    {
        CommandBase _closeCommand;

        protected WorkSpacesViewModel()
        {

        }
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new CommandBase(Parallel => this.OnRequestClose());
                return _closeCommand;
                
            }
        }


        public event EventHandler RequestClose;
        public void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}

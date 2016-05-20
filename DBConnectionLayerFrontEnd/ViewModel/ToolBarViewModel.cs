using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DBConnectionLayerFrontEnd.Commands;

namespace DBConnectionLayerFrontEnd.ViewModel
{
    public class ToolBarViewModel:ViewModelBase
    {
        public ToolBarViewModel(string displayName, ICommand command)
        {
            if (command == null)
                throw new ArgumentException("command");
            base.DisplayName = displayName;
            this.Command = command;
        }
        public ICommand Command { get; protected set; }
    }
}

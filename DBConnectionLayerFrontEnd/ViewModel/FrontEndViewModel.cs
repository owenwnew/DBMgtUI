using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using DBConnectionLayerFrontEnd.Commands;
using DBConnectionLayer;
using DBConnectionLayerFrontEnd.Resource;
namespace DBConnectionLayerFrontEnd.ViewModel
{
    public class FrontEndViewModel : WorkSpacesViewModel
    {

        ReadOnlyCollection<ToolBarViewModel> _toolBarCommands;
        ObservableCollection<WorkSpacesViewModel> _workSpaces;
        CommandBase _connectToDB;
        ConnectToMongoDB _connectedMongo = new ConnectToMongoDB();

        public FrontEndViewModel()
        {
            connectToDB();
        }

        public ReadOnlyCollection<ToolBarViewModel> ToolBarCommands
        {
            get
            {
                if(_toolBarCommands ==null)
                {
                    List<ToolBarViewModel> tbcmds = this.CreateToolBarCommands();
                    _toolBarCommands = new ReadOnlyCollection<ToolBarViewModel>(tbcmds); 
                }
                return _toolBarCommands;
            }


        }

        List<ToolBarViewModel> CreateToolBarCommands()
        {
            return new List<ToolBarViewModel> {
                new ToolBarViewModel(DirStrings.FrontEnd_Customer_Management, new CommandBase(param => this.openCustomerMgtWorkSpace())),
                new ToolBarViewModel(DirStrings.FrontEnd_Order_Management, new CommandBase(param => this.openOrderMgtWorkSpace()))


            };
        }

        public ObservableCollection<WorkSpacesViewModel> WorkSpaces
        {
            get
            {
                if(_workSpaces==null)
                {
                    _workSpaces = new ObservableCollection<WorkSpacesViewModel>();
                    _workSpaces.CollectionChanged += this.OnWorkSpacesChanged;
                }
                return _workSpaces;
            }

        }
        
        void OnWorkSpacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkSpaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkSpaceRequestClose;
        }

        void OnWorkSpaceRequestClose(object sender, EventArgs e)
        {
            WorkSpacesViewModel workspace = sender as WorkSpacesViewModel;
            workspace.Dispose();
            this.WorkSpaces.Remove(workspace);
        }

        public void openCustomerMgtWorkSpace()
        {
            CustomerMgtViewModel customerMgtWorkSpace = this.WorkSpaces.Where(w => w.DisplayName == "Customer Management").FirstOrDefault() as CustomerMgtViewModel;
            if(customerMgtWorkSpace == null)
            {
                customerMgtWorkSpace = new CustomerMgtViewModel();
                this.WorkSpaces.Add(customerMgtWorkSpace);
            }
            this.SetActiveWorkspace(customerMgtWorkSpace);
        }

        public void openOrderMgtWorkSpace()
        {
            //OrderMgtViewModel orderMgtWorkSpace = this.WorkSpaces.FirstOrDefault(vm => vm is OrderMgtViewModel) as OrderMgtViewModel;
            OrderMgtViewModel orderMgtWorkSpace = this.WorkSpaces.Where(w => w.DisplayName == "Order Management").FirstOrDefault() as OrderMgtViewModel;
            if (orderMgtWorkSpace == null)
            {
                orderMgtWorkSpace = new OrderMgtViewModel();
                this.WorkSpaces.Add(orderMgtWorkSpace);
            }
            this.SetActiveWorkspace(orderMgtWorkSpace);
        }

        void SetActiveWorkspace(WorkSpacesViewModel workspace)
        {
            Debug.Assert(this.WorkSpaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.WorkSpaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        public void connectToDB()
        {
            _connectedMongo.MongoDBConnection();
        }



        #region Icommands

        public ICommand ConnectToDB
        {
            get
            {
                if (_connectToDB == null)
                {
                    //commandBase(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
                    //this means commandBase takes 2 object parameters to create constructor
                    //first it will see if this command can be executed by going to CanUpdate
                    //if it cannot execute, it will disable the button
                    //once it gets a true boolean value, it will then proceed to execute
                    //if it can execute: then go to action boject which is updateTextOnCommand()
                    //_updateCommand = new CommandBase(param => this.UpdateTextOnCommand(), Param => this.CanUpdate);
                    _connectToDB = new CommandBase(param => this.connectToDB());
                }
                return _connectToDB;
            }


        }


        //public ICommand

        #endregion

        #region INotification interface
        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged(string PropertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged; //this is the samething as
        //    //handler = new event PropertyChangedEventHandler PropertyChanged

        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(PropertyName));
        //    }


        //}

        #endregion
    }
}

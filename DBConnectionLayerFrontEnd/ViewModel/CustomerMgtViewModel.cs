using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DBConnectionLayerFrontEnd.View;
using DBConnectionLayerFrontEnd.ViewModel;
using System.Windows.Data;
using System.Windows.Input;
using MongoDB.Bson;
using MongoDB.Driver;
using DBConnectionLayerFrontEnd.Commands;
using DBConnectionLayerFrontEnd.Model;
using DBConnectionLayer;

namespace DBConnectionLayerFrontEnd.ViewModel
{
    public class CustomerMgtViewModel:WorkSpacesViewModel
    {
        ObservableCollection<WorkSpacesViewModel> _workspaces;
        CommandBase _connectToDB;
        CommandBase _insertToDB;
        ConnectToMongoDB _connectedMongo = new ConnectToMongoDB();
        CustomerMgtModel customerDocumentModel = new CustomerMgtModel();
        public CustomerMgtViewModel()
        {

        }

        public void insertToDB()
        {
            var findResult = _connectedMongo.findDocument("CustomerMgtCollection", "CustomerName", "Owen");

            try
            {
                var _document = new BsonDocument {
                { "CustomerName" , customerName},
                { "Company", companyName},
                { "PrimaryPhone", primaryPhone },
                { "SecondaryPhone", secondaryPhone},
                { "EmailAddress", emailAddress },
                { "CompanyLocation",  companyLocation},
                {"Comments",comments }
            };


                _connectedMongo.insertDocumentToDB(_document, "CustomerMgtCollection");

                ActionResult = "Customer Added to DB";
                OnPropertyChanged("ActionResult");
            }
            catch
            {
                
            }
        }

        public void clearActionResult()
        {
            ActionResult = "";
            OnPropertyChanged(ActionResult);
        }

        #region properties
        #region CustomerMgtModelProperties
        public string customerName
        {
            get
            {
                return customerDocumentModel.customerName;
            }
            set
            {
                customerDocumentModel.customerName = value;
                clearActionResult();
            }


        }

        public string companyName
        {
            get
            {
                return customerDocumentModel.companyName;
            }
            set
            {
                customerDocumentModel.companyName = value;
                if (value == null)
                    customerDocumentModel.companyName = "";
                clearActionResult();
            }


        }
        public string primaryPhone
        {
            get
            {
                return customerDocumentModel.primaryPhone;
            }
            set
            {
                customerDocumentModel.primaryPhone = value;
                if (value == null)
                    customerDocumentModel.primaryPhone = "";
                clearActionResult();
            }


        }
        public string secondaryPhone
        {
            get
            {
                return customerDocumentModel.secondaryPhone;
            }
            set
            {
                customerDocumentModel.secondaryPhone = value;
                if (value == null)
                    customerDocumentModel.secondaryPhone = "";
                clearActionResult();
            }


        }
        public string emailAddress
        {
            get
            {
                return customerDocumentModel.emailAddress;
            }
            set
            {
                customerDocumentModel.emailAddress = value;
                if (value == null)
                    customerDocumentModel.emailAddress = "";
                clearActionResult();
            }


        }
        public string companyLocation
        {
            get
            {
                return customerDocumentModel.companyLocation;
            }
            set
            {
                customerDocumentModel.companyLocation = value;
                if (value == null)
                    customerDocumentModel.companyLocation = "";
                clearActionResult();
            }


        }
        public string comments
        {
            get
            {
                return customerDocumentModel.comments;
            }
            set
            {
                customerDocumentModel.comments = value;
                if (value == null)
                    customerDocumentModel.comments = "";
                clearActionResult();
            }


        }
        #endregion

        public string ActionResult {
            get;
            set;
        }

        #endregion

        #region Icommands

        public ICommand InsertToDB
        {
            get
            {
                if (_insertToDB == null)
                {
                    //commandBase(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
                    //this means commandBase takes 2 object parameters to create constructor
                    //first it will see if this command can be executed by going to CanUpdate
                    //if it cannot execute, it will disable the button
                    //once it gets a true boolean value, it will then proceed to execute
                    //if it can execute: then go to action boject which is updateTextOnCommand()
                    //_updateCommand = new CommandBase(param => this.UpdateTextOnCommand(), Param => this.CanUpdate);
                    _insertToDB = new CommandBase(param => this.insertToDB());
                }
                return _insertToDB;
            }


        }

        #endregion  

        #region mini workspace

        public ObservableCollection<WorkSpacesViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkSpacesViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }

                return _workspaces;
            }
        }

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkSpacesViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkSpacesViewModel workspace = sender as WorkSpacesViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        void SetActiveWorkspace(WorkSpacesViewModel workspace)
        {
            //Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        #endregion

    }
}

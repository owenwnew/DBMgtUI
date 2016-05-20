using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DBConnectionLayerFrontEnd.Commands;
using DBConnectionLayerFrontEnd.View;
using DBConnectionLayerFrontEnd.Model;


namespace DBConnectionLayerFrontEnd.ViewModel
{
    public class InvoiceItemListViewModel : WorkSpacesViewModel
    {
        public InvoiceItemListViewModel()
        {
            this.DisplayName = "Invoice List";
        }


        public ObservableCollection<InvoiceItemListModel> InvoiceList { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnectionLayerFrontEnd.Model
{
    public class InvoiceItemListModel
    {
        //string _invoicedItem;
        string _quantity;
        string _unitPrice;
        string _totalPrice;
        string _itemCatagory;
        string _paymentOption;
        string _item;
        string _description;

        public InvoiceItemListModel(string Item, string Description, string Quantity, string UnitPrice, string TotalPrice)
        {
            _item = Item;
            _description = Description;
            _quantity = Quantity;
            _unitPrice = UnitPrice;
            _totalPrice = TotalPrice;
            
            //_itemCatagory = ItemCatagory;
            //_paymentOption = PaymenOption;
        }

        public InvoiceItemListModel()
        {

        }

       
        public string item { get { return _item; } set { _item = value; } }
        public string description { get { return _description; } set { _description = value; } }
        public string quantity { get { return _quantity; } set { _quantity = value; } }
        public string unitPrice { get { return _unitPrice; } set { _unitPrice = value; } }
        public string totalPrice { get { return _totalPrice; } set { _totalPrice = value; } }
        //public string invoicedItem { get { return _invoicedItem; } set { _invoicedItem = value; } }
        //public string itemCatagory { get { return _itemCatagory; } set { _itemCatagory = value; } }
        //public string paymentOption { get { return _paymentOption; } set { _paymentOption = value; } }
        //public string paymentOption { get { return _paymentOption; } set { _paymentOption = value; } }

    }
}

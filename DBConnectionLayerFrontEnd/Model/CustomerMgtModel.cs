using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnectionLayerFrontEnd.Model
{
    public class CustomerMgtModel
    {
        string _customerName;
        string _companyName;
        string _primaryPhone;
        string _secondaryPhone;
        string _emailAddress;
        string _companyLocation;
        string _comments;


        public CustomerMgtModel(string CustomerName, string CompanyName, string PrimaryPhone, string SecondaryPhone, string EmailAddress, string CompanyLocation, string Comments)
        {
             _customerName = CustomerName;
             _companyName = CompanyName;
             _primaryPhone = PrimaryPhone;
             _secondaryPhone = SecondaryPhone;
             _emailAddress = EmailAddress;
             _companyLocation = CompanyLocation;
             _comments = Comments;


        }

        public CustomerMgtModel()
        {

        }

        public string customerName { get { return _customerName; } set { _customerName = value; } }
        public string companyName { get { return _companyName; } set { _companyName = value; } }
        public string primaryPhone { get { return _primaryPhone; } set { _primaryPhone = value; } }
        public string secondaryPhone { get { return _secondaryPhone; } set { _secondaryPhone = value; } }
        public string emailAddress{ get { return _emailAddress; } set { _emailAddress = value; } }
        public string companyLocation { get { return _companyLocation; } set { _companyLocation = value; } }
        public string comments { get { return _comments; } set { _comments = value; } }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnectionLayerFrontEnd.Model
{
    public class CustomerMgtModel
    {
        public CustomerMgtModel()
        {

        }

        public string customerName { get; set; }
        public string companyName { get; set; }
        public string primaryPhone { get; set; }
        public string secondaryPhone { get; set; }
        public string emailAddress{ get; set; }
        public string companyLocation { get; set; }
        public string comments { get; set; }

    }
}

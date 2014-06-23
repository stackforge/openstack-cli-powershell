using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Client.Powershell.Utility
{
    public class ServiceProviderAttribute : Attribute
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ServiceProviderAttribute(string name)
        {
            _name = name;  
        }
    }
}
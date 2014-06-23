using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Client.Powershell.Utility
{

    public enum CoreServices
    {
        Identity,
        ObjectStorage,
        ImageManagement,
        BlockStorage,
        Compute
    }


    public class ServiceMap
    {
        private string _source;
        private string _target;

        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

    }
}

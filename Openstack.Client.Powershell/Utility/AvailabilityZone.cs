/* ============================================================================
Copyright 2014 Hewlett Packard

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
============================================================================ */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openstack.Client.Powershell.Utility
{
    public class AvailabilityZone
    {
        private string _name;
        private bool _isDefault = false;
        private string _shellForegroundColor = "Green";
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string ShellForegroundColor
        {
            get { return _shellForegroundColor; }
            set { _shellForegroundColor = value; }
        }

        public bool IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }
        

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}

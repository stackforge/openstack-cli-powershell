// /* ============================================================================
// Copyright 2014 Hewlett Packard
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ============================================================================ */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Client.Powershell.Utility
{
    public class ServiceProvider 
    {
        private string _name;
        private bool _isDefault = false;
        private string _authenticationServiceURI;
        private string _defaultTenantId;
        private bool _isDirty = false;
        private List<CredentialElement> _credentialElements = new List<CredentialElement>();
        private string _configFilePath;

        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set { _configFilePath = value; }
        }

        public List<CredentialElement> CredentialElements
        {
            get { return _credentialElements; }
            set { _credentialElements = value; }
        }


        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public string DefaultTenantId
        {
            get { return _defaultTenantId; }
            set
            {
                _defaultTenantId = value; 
                _isDirty = true;
            }
        }

        public string AuthenticationServiceURI
        {
            get { return _authenticationServiceURI; }
            set { _authenticationServiceURI = value; _isDirty = true; }
        }

        public bool IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; _isDirty = true; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; _isDirty = true; }
        }
    }
}

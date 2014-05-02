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
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Utility
{
    public class RegistrationResponse 
    {
        private IOpenStackCredential _credentials;
        private ServiceProvider _provider;

        public ServiceProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public RegistrationResponse()
        {   }

        public RegistrationResponse(IOpenStackCredential credential, ServiceProvider provider)
        {
            _credentials = credential;
            _provider    = provider;
        }
        public IOpenStackCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }
    }
}

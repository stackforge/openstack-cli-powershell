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
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Utility
{
    [ServiceProviderAttribute("Default")]
    public class OpenstackCoreRegistrationManager : RegistrationManager
    {
        public override RegistrationResponse Register(ServiceProvider serviceProvider)
        {
            this.ValidateCredentialElements(ref serviceProvider);

            string authenticationEndpoint   = serviceProvider.CredentialElements.Where(ce => ce.Key == "AuthenticationServiceURI").Single().Value;
            string userName                 = serviceProvider.CredentialElements.Where(ce => ce.Key == "Username").Single().Value;
            string password                 = serviceProvider.CredentialElements.Where(ce => ce.Key == "Password").Single().Value;
            string tenantId                 = serviceProvider.CredentialElements.Where(ce => ce.Key == "DefaultTenantId").Single().Value;
            IOpenStackCredential credential = new OpenStackCredential(new Uri (authenticationEndpoint), userName, password, tenantId);

            return new RegistrationResponse(credential, serviceProvider);
        }

        private SecureString GetSecureString(string password)
        {
            SecureString securePassword = new SecureString();
            password.ToCharArray().ToList().ForEach(securePassword.AppendChar);
            return SecureStringHelper.ConvertToSecureString(password);

        }
    }
}


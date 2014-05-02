using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Identity;

namespace HP.OpenStack.Client.Powershell.Utility
{
  
    public class HPRegistrationManager : RegistrationManager
    {
        public override RegistrationResponse Register(ServiceProvider serviceProvider)
        {
            this.ValidateCredentialElements(ref serviceProvider);

            string authenticationEndpoint = serviceProvider.CredentialElements.Where(ce => ce.Key == "AuthenticationServiceURI").Single().Value;
            string userName               = serviceProvider.CredentialElements.Where(ce => ce.Key == "Username").Single().Value;
            string password               = serviceProvider.CredentialElements.Where(ce => ce.Key == "Password").Single().Value;
            string tenantId               = serviceProvider.CredentialElements.Where(ce => ce.Key == "DefaultTenantId").Single().Value;

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

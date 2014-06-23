using System;
using System.Linq;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Identity;

namespace AcmeInc.OpenStack.Client.Powershell.Utility
{
    [ServiceProviderAttribute("AcmeInc")]
    public class AcmeRegistrationManager : RegistrationManager
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
    }
}

///* ============================================================================
//Copyright 2014 Hewlett Packard

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//============================================================================ */
using System;
using System.Management.Automation;
using Openstack.Client.Powershell.Providers.Common;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Openstack;
using System.Xml.Linq;
using System.Collections.Generic;
using Openstack.Client.Powershell.Utility;
using System.Linq;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "SP", SupportsShouldProcess = true)]
    //[RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class GetServiceProvidersCmdlet : BasePSCmdlet
    {       
        #region Parameters
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            List<ServiceProvider> serviceProviders     = new List<ServiceProvider>();
            XDocument doc                              = XDocument.Load(this.ConfigFilePath);
            IEnumerable<XElement> serviceProviderNodes = doc.Descendants("ServiceProvider");
            
            foreach (XElement element in serviceProviderNodes)
            {
                ServiceProvider provider = new ServiceProvider();
                provider.AuthenticationServiceURI = element.Elements().Where(e => e.Attribute("key").Value == "AuthenticationServiceURI").Attributes("value").Single().Value;
                provider.DefaultTenantId          = element.Elements().Where(e => e.Attribute("key").Value == "DefaultTenantId").Attributes("value").Single().Value;
                provider.Username                 = element.Elements().Where(e => e.Attribute("key").Value == "Username").Attributes("value").Single().Value;
                provider.IsDefault                = Convert.ToBoolean(element.Attribute("isDefault").Value);
                provider.Name                     = element.Attribute("name").Value;

                serviceProviders.Add(provider);                
            }
            this.WriteObject(serviceProviders);
        }
        #endregion
    }
}

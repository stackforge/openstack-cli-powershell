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
using System.Management.Automation;
using Openstack.Objects.Domain.Admin;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
     [Cmdlet(VerbsCommon.Set, "Credentials", SupportsShouldProcess = true)]
    public class SetCredentialsCmdlet : BaseAuthenticationCmdlet
    {
         private string _accessKey;
         private string _secretKey;
         private string _tenantId;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "sc5", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("ak")]
        [ValidateNotNullOrEmpty]
         public string AccessKey
        {
            get { return _accessKey; }
            set { _accessKey = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "sc5", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("sk")]
        [ValidateNotNullOrEmpty]
        public string SecretKey
        {
            get { return _secretKey; }
            set { _secretKey = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "sc5", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("t")]
        [ValidateNotNullOrEmpty]
        public string TenantId
        {
            get { return _tenantId; }
            set { _tenantId = value; }
        }        
        #endregion
        #region Methods
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            AuthenticationRequest request = new AuthenticationRequest(new Credentials(_accessKey, _secretKey), _tenantId);
            this.InitializeSession(request);

            // Show the User the new ServiceCatalog that we just received..

            this.WriteServices();

            // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

            if (this.Context.ServiceCatalog.DoesServiceExist("OS-Storage"))
            {
                //this.WriteContainers();
            }
        }
        #endregion
    }
}

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
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Security;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.DataAccess;
using Openstack.Objects.DataAccess.Security;

namespace Openstack.Client.Powershell.Cmdlets.Security
{
    [Cmdlet(VerbsCommon.Remove, "SecurityGroup", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveSecurityGroupCmdlet : BasePSCmdlet
    {
        private string _securityGroupId;
     
        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "RemoveSecurityGroup", ValueFromPipelineByPropertyName = true, HelpMessage = "ww")]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        public string SecurityGroupId
        {
            get { return _securityGroupId; }
            set { _securityGroupId = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string id = this.TranslateQuickPickNumber(this.SecurityGroupId);
            ISecurityRepository repository = this.RepositoryFactory.CreateSecurityRepository();
           
            SecurityGroup group            = repository.GetSecurityGroup(id);

            if (group.Name != "default")
            {
                repository.DeleteSecurityGroup(id);
                this.UpdateCache<SecurityGroupsUIContainer>();   
            }
            else
            {                
                Console.WriteLine("");
                Console.WriteLine("Invalid SecurityGroupId : Unable to delete the Default Security Group.");
                Console.WriteLine("");
            }
        }
        #endregion
    }
        
}

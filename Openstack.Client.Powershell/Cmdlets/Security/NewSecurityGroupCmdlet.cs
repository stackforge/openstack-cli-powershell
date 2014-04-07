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
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Security;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;

namespace Openstack.Client.Powershell.Cmdlets.Security
{
    [Cmdlet(VerbsCommon.New, "SecurityGroup", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class NewSecurityGroupCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _description;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "NewSecurityGroup", ValueFromPipelineByPropertyName = true, HelpMessage = "ww")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewSecurityGroup", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "www")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
            NewSecurityGroup newGroup = new NewSecurityGroup();
            newGroup.Description      = this.Description;
            newGroup.Name             = this.Name;

            SecurityGroup securityGroup = this.RepositoryFactory.CreateSecurityRepository().SaveSecurityGroup(newGroup);
            this.UpdateCache<SecurityGroupsUIContainer>();
        }
        #endregion
    }
        
}

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
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Compute;
using System.Linq;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Common;
using System.Collections;
using Openstack.Objects.Domain.Security;

namespace Openstack.Client.Powershell.Cmdlets.Security
{
    [Cmdlet(VerbsCommon.Remove, "Rule", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveRuleCmdlet : BasePSCmdlet
    {
        private string _securityGroupRuleId;

        #region Parameters 
//=========================================================================================
/// <summary>
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "RemoveRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        public string SecurityGroupRuleId
        {
            get { return _securityGroupRuleId; }
            set { _securityGroupRuleId = value; }
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
            this.RepositoryFactory.CreateSecurityRepository().DeleteSecurityRule(this.SecurityGroupRuleId);
            this.UpdateCache<SecurityGroupUIContainer>();
        }
        #endregion
    }
}



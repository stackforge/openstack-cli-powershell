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
    [Cmdlet(VerbsCommon.Add, "Rule", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class AddRuleCmdlet : BasePSCmdlet
    {
        private string _direction;
        private string _etherType;
        private string _portRangeMax;
        private string _portRangeMin;
        private string _protocol;
        private string _remoteGroupId;
        private string _remoteIPPrefix;
        private string _securityGroupId;
      
        #region Parameters 
//=========================================================================================
/// <summary>
/// 
//add-rule -ipr "iprTest" -IP "100.0.0.0" -tp "80" -fp "81" -sg "2302"
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, Mandatory = true, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public string Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
//=========================================================================================
/// <summary>
/// 
//  add-rule -ipr "iprTest" -IP "tcp" -tp "81" -fp "80"
/// </summary>
//=========================================================================================
        [Parameter(Position = 4, Mandatory = false, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("et")]
        [ValidateNotNullOrEmpty]
        public string EtherType
        {
            get { return _etherType; }
            set { _etherType = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("max")]
        [ValidateNotNullOrEmpty]
        public string PortRangeMax
        {
            get { return _portRangeMax; }
            set { _portRangeMax = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("min")]
        [ValidateNotNullOrEmpty]
        public string PortRangeMin
        {
            get { return _portRangeMin; }
            set { _portRangeMin = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, Mandatory = true, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("p")]
        [ValidateNotNullOrEmpty]
        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }
//=========================================================================================
/// <summary>
/// 
//add-rule -ipr "iprTest" -IP "100.0.0.0" -tp "80" -fp "81" -sg "2302"
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, Mandatory = true, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("sid")]
        [ValidateNotNullOrEmpty]
        public string SecurityGroupId
        {
            get { return _securityGroupId; }
            set { _securityGroupId = value; }
        }
//=========================================================================================
/// <summary>
/// 
//add-rule -ipr "iprTest" -IP "100.0.0.0" -tp "80" -fp "81" -sg "2302"
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("rgid")]
        [ValidateNotNullOrEmpty]
        public string RemoteGroupId
        {
            get { return _remoteGroupId; }
            set { _remoteGroupId = value; }
        }
//=========================================================================================
/// <summary>
/// 
//add-rule -ipr "iprTest" -IP "100.0.0.0" -tp "80" -fp "81" -sg "2302"
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "AddRule", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("ripp")]
        [ValidateNotNullOrEmpty]
        public string RemoteIPPrefix
        {
            get { return _remoteIPPrefix; }
            set { _remoteIPPrefix = value; }
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
            NewSecurityGroupRule rule = new NewSecurityGroupRule();
            rule.PortRangeMin         = this.PortRangeMin;
            rule.PortRangeMax         = this.PortRangeMax;
            rule.Protocol             = this.Protocol;
            rule.SecurityGroupId      = this.SecurityGroupId;
            rule.RemoteGroupId        = this.RemoteGroupId;
            rule.RemoteIPPrefix       = this.RemoteIPPrefix;
            rule.EtherType            = this.EtherType;
            rule.Direction            = this.Direction;

            SecurityGroupRule result = this.RepositoryFactory.CreateSecurityRepository().SaveSecurityRule(rule);
            this.UpdateCache<SecurityGroupUIContainer>();
        }
        #endregion
    }
}


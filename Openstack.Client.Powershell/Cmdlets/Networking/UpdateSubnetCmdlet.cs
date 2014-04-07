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
using System.Management.Automation;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet("Update", "Subnet", SupportsShouldProcess = true)]
    public class UpdateSubnetCmdlet : BasePSCmdlet
    {
        private string _gatewayIP;
        private string _name;
        private string _subnetId;

//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 0, ParameterSetName = "UpdateSubnet", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("id")]
        public string SubnetId
        {
            get { return _subnetId; }
            set { _subnetId = value; }
        }
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 2, ParameterSetName = "UpdateSubnet", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("egw")]
        public string GatewayId
        {
            get { return _gatewayIP; }
            set { _gatewayIP = value; }
        }
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 1, ParameterSetName = "UpdateSubnet", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        protected override void ProcessRecord()
        {
            UpdateSubnet updateSubnet = new UpdateSubnet();
            updateSubnet.Name         = this.Name;
            updateSubnet.GatewayIP    = this.GatewayId;

            this.RepositoryFactory.CreateSubnetRepository().UpdateSubnet(_subnetId, updateSubnet);
        }
    }
}

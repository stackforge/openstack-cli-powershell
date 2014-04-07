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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet(VerbsCommon.New, "FloatingIP", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class NewFloatingIPCmdlet : BasePSCmdlet
    {
        private string _networkId;
        private string _portId;
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "NewFloatingIP", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("pid")]      
        public string PortId
        {
            get { return _portId; }
            set { _portId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewFloatingIP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("nid")]      
        public string NetworkId
        {
            get { return _networkId; }
            set { _networkId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            NewFloatingIP newFloatingIP     = new NewFloatingIP();
            newFloatingIP.FloatingNetworkId = this.NetworkId;
            newFloatingIP.PortId            = this.PortId;

            this.RepositoryFactory.CreateFloatingIPRepository().SaveFloatingIP(newFloatingIP);        
        }
    }
}

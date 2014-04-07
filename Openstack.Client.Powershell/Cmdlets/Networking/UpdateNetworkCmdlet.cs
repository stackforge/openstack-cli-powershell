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
    [Cmdlet("Update", "Network", SupportsShouldProcess = true)]
    public class UpdateNetworkCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _networkId;
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "UpdateNetwork", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("id")]
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
        [Parameter(Position = 1, ParameterSetName = "UpdateNetwork", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================   
        protected override void ProcessRecord()
        {
            string id                   = this.TranslateQuickPickNumber(this.NetworkId);
            UpdateNetwork updateNetwork = new UpdateNetwork();
            updateNetwork.Name          = this.Name;

            this.RepositoryFactory.CreateNetworkRepository().UpdateNetwork(id, updateNetwork);
            this.UpdateCache();
        }
    }
}

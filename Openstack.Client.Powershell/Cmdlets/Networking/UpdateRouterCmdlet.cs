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
    [Cmdlet("Update", "Router", SupportsShouldProcess = true)]
    public class UpdateRouterCmdlet : BasePSCmdlet
    {
        private string _networkId;
        private string _routerId;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================     
        [Parameter(Position = 0, ParameterSetName = "UpdateRouter", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("id")]
        public string RouterId
        {
            get { return _routerId; }
            set { _routerId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 1, ParameterSetName = "UpdateRouter", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
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
            string id                                  = this.TranslateQuickPickNumber(this.RouterId);
            UpdateRouter updateRouter                  = new UpdateRouter();
            //updateRouter.ExternalGatewayInfo.NetworkId = this.NetworkId;

            this.RepositoryFactory.CreateRouterRepository().UpdateRouter(id, updateRouter);
        }
    }
}

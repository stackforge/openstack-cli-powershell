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
    [Cmdlet("New", "Port", SupportsShouldProcess = true)]
    public class NewPortCmdlet : BasePSCmdlet
    {
        private string _name;
        private SwitchParameter _admin_state_up = true;
        private string _networkId;
        private string _deviceId;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "NewPort", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
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
        [Parameter(Position = 1, ParameterSetName = "NewPort", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
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
        [Parameter(Position = 3, ParameterSetName = "NewPort", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("asu")]

        public SwitchParameter AdminStateUp
        {
            get { return _admin_state_up; }
            set { _admin_state_up = value; }
        }
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 2, ParameterSetName = "NewPort", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("did")]
        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }
        #endregion
        #region Methods
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        protected override void ProcessRecord()
         {
            NewPort newPort      = new NewPort();
            newPort.AdminStateUp = this.AdminStateUp;
            newPort.DeviceId     = this.DeviceId;
            newPort.Name         = this.Name;
            newPort.NetworkId    = this.NetworkId;
            
            this.RepositoryFactory.CreatePortRepository().SavePort(newPort);

         }
        #endregion
    }
}

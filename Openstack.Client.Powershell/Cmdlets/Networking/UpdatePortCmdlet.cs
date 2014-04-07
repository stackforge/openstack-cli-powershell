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
    [Cmdlet("Update", "Port", SupportsShouldProcess = true)]
    public class UpdatePortCmdlet : BasePSCmdlet
    {
        private string _deviceId;
        private string _portId;

        #region Properties
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 0, ParameterSetName = "UpdatePort", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("id")]
        public string PortId
        {
            get { return _portId; }
            set { _portId = value; }
        }
//============================================================================
/// <summary>
/// 
/// </summary>
//============================================================================
        [Parameter(Position = 1, ParameterSetName = "UpdatePort", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
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
            UpdatePort updatePort = new UpdatePort();
            updatePort.DeviceId   = this.DeviceId;
                        
            this.RepositoryFactory.CreatePortRepository().UpdatePort(this.PortId, updatePort);
         }
        #endregion
    }
}

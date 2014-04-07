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
using Openstack.Objects.Domain.Networking;
using System;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.DataAccess.Networking;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Security
{
    [Cmdlet("UnAssign", "IP", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class UnAssignIPCmdlet : BasePSCmdlet
    {
        private string _serverId;
        private string _ip;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("ip")]
        [ValidateNotNullOrEmpty]
        public string IpAddress
        {
            get { return _ip; }
            set { _ip = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("sid")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
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
            IFloatingIPRepository repository = new FloatingIPRepository(this.Context.GetRepositoryContext("compute"));
            UnAssignIPAction assignment      = new UnAssignIPAction();
            assignment.ServerId              = this.ServerId;
            assignment.Ip                    = this.IpAddress;

            repository.UnAssignIP(assignment);

            Console.WriteLine("");
            Console.WriteLine("Floating IP Address " + this.IpAddress + " has been disassociated with Server : " + assignment.ServerId);
            Console.WriteLine("");
            this.UpdateCache<ServerUIContainer>();
        }
        #endregion
    }
}


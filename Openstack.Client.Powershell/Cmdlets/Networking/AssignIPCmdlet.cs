﻿/* ============================================================================
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

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet("Assign", "IP", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class AssignIPCmdlet : BasePSCmdlet
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
        [Alias("s")]
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
            if (!this.ServerId.Contains("."))
            {
                IFloatingIPRepository repository = new FloatingIPRepository(this.Context.GetRepositoryContext("compute"));

                AssignIPAction assignment = new AssignIPAction();
                assignment.ServerId       = this.ServerId;
                assignment.Ip             = this.IpAddress;

                repository.AssignIP(assignment);

                Console.WriteLine("");
                Console.WriteLine("Floating IP Address " + this.IpAddress + " now assigned to Server : " + assignment.ServerId);
                Console.WriteLine("");
                this.UpdateCache();
            }
            else
            {
                InvalidOperationException ex = new InvalidOperationException("Please check the supplied parameters. IP addresses are not allowed in place of Server IDs.", null);
                WriteError(new ErrorRecord(ex, "0", ErrorCategory.InvalidArgument, null));
            }
        }
        #endregion
    }
}


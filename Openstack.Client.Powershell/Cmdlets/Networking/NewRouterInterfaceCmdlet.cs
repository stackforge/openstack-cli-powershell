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
    [Cmdlet("New", "RouterInterface", SupportsShouldProcess = true)]
    public class NewRouterInterfaceCmdlet : BasePSCmdlet
    {
        private string _subnetId;
        private string _portId;
        private string _routerId;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "NewRouterInterface", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("rid")]
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
        [Parameter(Position = 1, ParameterSetName = "NewRouterInterface", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("sid")]
        public string SubnetId
        {
            get { return _subnetId; }
            set { _subnetId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 2, ParameterSetName = "NewRouterInterface", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("pid")]
        public string PortId
        {
            get { return _portId; }
            set { _portId = value; }
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
            NewRouterInterface newRouterInterface = new NewRouterInterface();
            newRouterInterface.PortId             = this.PortId;
            newRouterInterface.SubnetId           = this._subnetId;
            newRouterInterface.RouterId           = this.RouterId;

            NewRouterInterface response  = this.RepositoryFactory.CreateRouterRepository().SaveRouterInterface(newRouterInterface);
            if (response.SubnetId != null)
            {
                Console.WriteLine("");
                Console.WriteLine("New Router Interface created for Subnet Id " + response.SubnetId);
                Console.WriteLine("");
            }
            else if (response.PortId != null)
            {
                Console.WriteLine("");
                Console.WriteLine("New Router Interface created for Port Id " + response.PortId);
                Console.WriteLine("");
            }
        }
        #endregion

    }
}

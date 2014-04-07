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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet("New", "Router", SupportsShouldProcess = true)]
    public class NewRouterCmdlet : BasePSCmdlet
    {
        private bool _adminStateUp = true;
        private string _name;
        private string _extGatewayNetworkId;       

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 2, ParameterSetName = "NewRouter", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("egw")]
        public string ExternalGateway
        {
            get { return _extGatewayNetworkId; }
            set { _extGatewayNetworkId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "NewRouter", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
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
        [Parameter(Position = 1, ParameterSetName = "NewRouter", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("asu")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter AdminStateUp
        {
            get { return _adminStateUp; }
            set { _adminStateUp = value; }
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
            NewRouter newRouter     = new NewRouter();
            newRouter.Name          = this.Name;
            newRouter.AdminStateUp  = this.AdminStateUp;

            if (this.ExternalGateway != null) {
                newRouter.ExternalGateway.NetworkId = this.ExternalGateway;
            }

            this.RepositoryFactory.CreateRouterRepository().SaveRouter(newRouter);
        }
        #endregion

    }
}

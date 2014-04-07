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
using Openstack.Objects.Domain.Compute;
using System.Linq;
using Openstack.Objects.Domain.Compute.Servers;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet("Update", "Server", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class UpdateServerCmdlet : BasePSCmdlet
    {
        private string _serverId;
        private string _name;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "UpdateServer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("n")]
        [ValidateNotNullOrEmpty]
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
        [Parameter(Position = 0, ParameterSetName = "UpdateServer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("id")]
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
        private void UpdateName(string name)
        {
            BaseUIContainer server = ((CommonDriveInfo)this.Drive).CurrentContainer;
            server.Name            = name;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            Openstack.Objects.Domain.Compute.Server server = new Openstack.Objects.Domain.Compute.Server();
            if (this.ServerId != null)
            {
                server.Id   = this.ServerId;
                server.Name = this.Name;

                this.RepositoryFactory.CreateServerRepository().UpdateServer(server);
                this.UpdateName(this.Name);
                this.UpdateCache<ServersUIContainer>();     
            }          
        }
        #endregion
    }
}



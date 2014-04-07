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
using Openstack.Objects.Domain.Compute.Servers;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects;
using Openstack.Client.Powershell.Providers.Compute;
using System;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet("Reboot", "Server", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RebootServerCmdlet : BasePSCmdlet
    {
        private ServerRebootType _type = ServerRebootType.SOFT;
        private string _serverId;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "RebootServer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "RebootServer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("t")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("SOFT", "HARD")]
        public ServerRebootType Type
        {
            get { return _type; }
            set { _type = value; }
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
            string id = this.TranslateQuickPickNumber(this.ServerId);

            RebootAction action = new RebootAction();
            action.RebootType   = this.Type;

            if (this.ServerId != null)
            {
                action.ServerId = id;
                Console.WriteLine("");
                Console.WriteLine("Rebooting Server " + id);
                Console.WriteLine("");
                this.RepositoryFactory.CreateServerRepository().Reboot(action);
            }
            else
            {
                BaseUIContainer currentContainer = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;

                if (currentContainer.Name == "Metadata")
                {
                    ServerUIContainer serverContainer = currentContainer.Parent as ServerUIContainer;

                    if (serverContainer != null) 
                    {       
                        action.ServerId = serverContainer.Entity.Id;
                        this.RepositoryFactory.CreateServerRepository().Reboot(action);
                    }
                }
                else
                {
                    ServerUIContainer serverContainer = currentContainer as ServerUIContainer;
                    if (serverContainer != null)
                    {
                        action.ServerId = serverContainer.Entity.Id;
                        this.RepositoryFactory.CreateServerRepository().Reboot(action);
                        this.UpdateCache<ServerUIContainer>();   
                    }                   
                }
            }
        }
        #endregion
    }
}

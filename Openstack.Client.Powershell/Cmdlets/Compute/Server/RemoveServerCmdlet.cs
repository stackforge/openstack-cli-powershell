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
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using System;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet(VerbsCommon.Remove, "Server", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveServerCmdlet : BasePSCmdlet
    {
        private string _serverId;
        private SwitchParameter _force = false;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 1, ParameterSetName = "RemoveServer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("all")]
        public SwitchParameter RemoveAll
        {
            get { return _force; }
            set { _force = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "RemoveServer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        private void RemoveAllServers()
        {
            List<Openstack.Objects.Domain.Compute.Server> servers = this.RepositoryFactory.CreateServerRepository().GetServers();
            Console.WriteLine("");

            foreach(Openstack.Objects.Domain.Compute.Server server in servers)
            {
                if (server.Name != "RGWinLarge")
                {
                    Console.WriteLine("Removing Server : " + server.Name);
                    this.RepositoryFactory.CreateServerRepository().DeleteServer(server.Id);
                }
            }
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string id = this.TranslateQuickPickNumber(this.ServerId);
        
            if (_force)
            {
                Collection<ChoiceDescription> choices = new Collection<ChoiceDescription>();
                choices.Add(new ChoiceDescription("Y", "Yes"));
                choices.Add(new ChoiceDescription("N", "No"));

                Console.WriteLine("");
                if (this.Host.UI.PromptForChoice("Confirm Action", "You are about to remove all active Server instances. Are you sure about this?", choices, 0) == 0)
                    this.RemoveAllServers();
            }
            else
            {
                if (this.ServerId != null)
                {
                    this.RepositoryFactory.CreateServerRepository().DeleteServer(id);
                   this.UpdateCache();
                }
            }
                       
        }
        #endregion
    }
}


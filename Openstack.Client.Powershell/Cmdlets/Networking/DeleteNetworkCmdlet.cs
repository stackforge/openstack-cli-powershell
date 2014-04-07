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
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.DataAccess.Networking;
using Openstack.Objects.Domain;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet(VerbsCommon.Remove, "Network", SupportsShouldProcess = true)]
    //[RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.)]
    public class DeleteNetworkCmdlet : BasePSCmdlet
    {
        private string _networkId;
        private SwitchParameter _removeAll = false;
        private SwitchParameter _force     = true;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "DeleteNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("id")]
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
        [Parameter(Position = 1, ParameterSetName = "DeleteNetwork2", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("all")]
        public SwitchParameter RemoveAll
        {
            get { return _removeAll; }
            set { _removeAll = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 2, ParameterSetName = "DeleteNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("f")]
        public SwitchParameter Force
        {
            get { return _force; }
            set { _force = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
        private void BuilderEvent(object sender, CreateNetworkEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void RemoveAllNetworks()
        {
            INetworkRepository repository = this.RepositoryFactory.CreateNetworkRepository();
            List<Network> networks        = repository.GetNetworks();
        
            repository.Changed += new Openstack.Objects.DataAccess.Networking.NetworkRepository.CreateNetworkEventHandler(BuilderEvent);
            foreach (BaseEntity entity in this.CurrentContainer.Entities)
            {
                if (entity.Name != "Ext-Net")
                {
                    Console.WriteLine("");
                    Console.WriteLine("Removing Network : " + entity.Name);
                    Console.WriteLine("");
                    repository.DeleteNetwork(entity.Id, _force);
                }
            }
            Console.WriteLine("");
            repository.Changed -= new Openstack.Objects.DataAccess.Networking.NetworkRepository.CreateNetworkEventHandler(BuilderEvent);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {

            if (_networkId == null && _removeAll == false)
            {
                Console.WriteLine("Error : ");
            }
            else
            {
                if (_removeAll)
                {
                    if (this.UserConfirmsDeleteAction("Networks"))
                    {                        
                        this.RemoveAllNetworks();
                        this.UpdateCache();
                    }
                }
                else
                {
                    string id = this.TranslateQuickPickNumber(this.NetworkId);
                    this.RepositoryFactory.CreateNetworkRepository().DeleteNetwork(id, _force);
                    this.UpdateCache();
                }
            }
            
        }

    }
}

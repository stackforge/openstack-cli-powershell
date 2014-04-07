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
using System.Collections.Generic;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Providers.Networking
{
   // [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class NetworksUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public NetworksUIContainer() {  }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public NetworksUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path)
                              : base(parentContainer, name, description, path)
        {
            this.ObjectType =  Common.ObjectType.Container;
        }

        #region Methods
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            this.LoadEntities();
            this.LoadContainers();
        }        
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
//=========================================================================================================
        private void LoadEntities() 
        {
            List<Network> networks = this.RepositoryFactory.CreateNetworkRepository().GetNetworks();

            if (networks != null && networks.Count > 0)
            {
                this.SetUIContainers<NetworkUIContainer>(networks);
                this.Entities = networks;
            }
            else
            {
                this.Containers.Clear();
                this.Entities.Clear();
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private void LoadContainers()
        {
            this.AddContainer(this.CreateContainer<SubnetsUIContainer>("Subnets", "All Subnets assocated with the Account.", "Subnets"));
            this.AddContainer(this.CreateContainer<PortsUIContainer>("Ports", "All Ports associated with the Account", "Ports"));
            this.AddContainer(this.CreateContainer<FloatingIPsUIContainer>("FloatingIPs", "All Floating IPs associated with the Account", "FloatingIPs"));
            this.AddContainer(this.CreateContainer<RoutersUIContainer>("Routers", "All Routers associated with the Account", "Routers")); 
        }   
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//=========================================================================================================
        public override BaseUIContainer CreateContainer(string id)
        {
            return null;
        }
        #endregion
    }
}

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
using System.Text;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.DataAccess.Networking;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Providers.Networking
{
    public class NetworkUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            this.LoadNetworkDetails();
            this.LoadContainers();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadPortUIContainers()
        {
            string networkId = this.Entity.Id;
            BaseUIContainer container = (BaseUIContainer)this.CreateContainer<PortsUIContainer>("Ports", "Ports associated with this Network.", this.Parent.Path + @"\Port");
          
            if (this.Entity != null) 
            {
                IPortRepository repository = this.RepositoryFactory.CreatePortRepository();
                List<Port> allports        = repository.GetPorts();
                List<Port> ports           = new List<Port>();
                                
                 foreach (Port port in allports)  {                            
                    if (port.NetworkId == networkId)
                        ports.Add(port);                    
                 }

                ((PortsUIContainer)container).Load(ports);
                this.Containers.Add(container);           
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadFloatingIPUIContainers()
        {
            string networkId          = this.Entity.Id;
            BaseUIContainer container = (BaseUIContainer)this.CreateContainer<FloatingIPsUIContainer>("FloatingIPs", "Floating IPs associated with this Network.", this.Parent.Path + @"\FloatingIP");

            if (this.Entity != null)
            {
                IFloatingIPRepository repository = this.RepositoryFactory.CreateFloatingIPRepository();
                List<FloatingIP> allfloatingIPs  = repository.GetFloatingIPs();
                List<FloatingIP> floatingIPs     = new List<FloatingIP>();

                foreach (FloatingIP floatingIP in allfloatingIPs)
                {
                    if (floatingIP.FloatingNetworkId == networkId)
                        floatingIPs.Add(floatingIP);
                }

                ((FloatingIPsUIContainer)container).Load(floatingIPs);
                this.Containers.Add(container);
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadSubnetUIContainers()
        {
            BaseUIContainer container = (BaseUIContainer)this.CreateContainer<SubnetsUIContainer>("Subnets", "Subnets associated with this Network.", this.Parent.Path + @"\Subnet");
            if (this.Entity != null)
            {
                ISubnetRepository repository = this.RepositoryFactory.CreateSubnetRepository();
                List<Subnet> subnets = new List<Subnet>();

                foreach (string subnetListElement in ((Network)this.Entity).Subnets)
                {
                    Subnet subnet = repository.GetSubnet(subnetListElement);
                    if (subnet != null)
                        subnets.Add(subnet);
                }

                ((SubnetsUIContainer)container).Load(subnets);
                this.Containers.Add(container);
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
            this.Containers.Clear();
            this.LoadSubnetUIContainers();
            this.LoadPortUIContainers();
            this.LoadFloatingIPUIContainers();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadNetworkDetails()
        {
            Network network = (Network)this.Entity;
            this.Entity = this.RepositoryFactory.CreateNetworkRepository().GetNetwork(network.Id);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void WriteNetworkDetails(Network network)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id                    : " + network.Id);
            Console.WriteLine("  Name                  : " + network.Name);
            Console.WriteLine("  Status                : " + network.Status);
            Console.WriteLine("  Admin State Up        : " + network.AdminStateUp);
            Console.WriteLine("  Shared                : " + network.Shared);
            Console.WriteLine("  Is External           : " + network.IsExternal);
            Console.WriteLine("  Port Security Enabled : " + network.PortSecurityEnabled);
            Console.WriteLine("  Tenant Id             : " + network.TenantId);            
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Network network = (Network)this.Entity;
            this.WriteHeader("Network Details");
            this.WriteNetworkDetails(network);
            Console.WriteLine();
        }
    }
}

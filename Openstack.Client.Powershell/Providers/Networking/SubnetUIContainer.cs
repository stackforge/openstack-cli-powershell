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
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Providers.Networking
{
    public class SubnetUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public SubnetUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
            : base(parentContainer, name, description, path)
        {           
            this.ObjectType = Common.ObjectType.Entity;            
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public SubnetUIContainer() { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="subnet"></param>
//=========================================================================================================
        private void WriteSubnetDetails(Subnet subnet)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id            : " + subnet.Id);
            Console.WriteLine("  Gateway IP    : " + subnet.GatewayIP);
            Console.WriteLine("  IP Version    : " + subnet.IPVersion);
            Console.WriteLine("  Cidr          : " + subnet.Cidr);
            Console.WriteLine("  DHCP Enabled  : " + subnet.EnableDHCP);
            Console.WriteLine("  Network Id    : " + subnet.NetworkId);
            Console.WriteLine("  Tenant Id     : " + subnet.TenantId);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="subnet"></param>
//=========================================================================================================
        private void WriteAllocationPools(Subnet subnet)
        {
            this.WriteHeader("Available Subnet Allocation Pools");
            foreach (AllocationPool pool in subnet.AllocationPools)
            {
                 Console.WriteLine("  Start : " + pool.Start);
                 Console.WriteLine("  End   : " + pool.End);
                 Console.WriteLine("");
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Subnet subnet = (Subnet)this.Entity;
            this.WriteHeader("Subnet Details");
            this.WriteSubnetDetails(subnet);
            this.WriteAllocationPools(subnet);
            Console.WriteLine();
        }
    }
}

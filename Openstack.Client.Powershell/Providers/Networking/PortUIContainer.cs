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
    public class PortUIContainer : BaseUIContainer
    {
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public override void Load()
        {
            
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="port"></param>
//==================================================================================================
        private void WriteFixedIPs(Port port)
        {
            this.WriteHeader("Assigned Fixed IPs");
            foreach (FixedIP fixedIP in port.FixedIPs)
            {
                Console.WriteLine("  IP Address    : " + fixedIP.IpAddress);
                Console.WriteLine("  Subnet Id     : " + fixedIP.SubnetId);
                Console.WriteLine("");
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public override void WriteEntityDetails()
        {
            Port port = (Port)this.Entity;
            this.WriteHeader("Port Details");
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id                    : " + port.Id);
            Console.WriteLine("  Device Id             : " + port.DeviceId);
            Console.WriteLine("  Device Owner          : " + port.DeviceOwner);
            Console.WriteLine("  MAC Address           : " + port.MacAddress);
            Console.WriteLine("  Status                : " + port.Status);
            Console.WriteLine("  Network Id            : " + port.NetworkId);
            Console.WriteLine("  Admin State Up        : " + port.AdminStateUp);
            Console.WriteLine("  Port Security Enabled : " + port.PortSecurityEnabled);
            Console.WriteLine("  Binding Type          : " + port.BindingType);
          
            this.WriteFixedIPs(port);
        }
    }
}

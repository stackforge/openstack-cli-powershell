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
    public class FloatingIPUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public FloatingIPUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
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
        public FloatingIPUIContainer() { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
           
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            FloatingIP floatingIP = (FloatingIP)this.Entity;
            this.WriteHeader("Floating IP Details");
            this.WriteFloatingIPDetails(floatingIP);          
            Console.WriteLine();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="subnet"></param>
//=========================================================================================================
        private void WriteFloatingIPDetails(FloatingIP floatingIP)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id               : " + floatingIP.Id);
            Console.WriteLine("  Fixed Address    : " + floatingIP.FixedIPAddress);
            Console.WriteLine("  Floating Address : " + floatingIP.FloatingIPAddress);
            Console.WriteLine("  Port Id          : " + floatingIP.PortId);
            Console.WriteLine("  Router Id        : " + floatingIP.RouterID);
            Console.WriteLine("  Tenant Id        : " + floatingIP.TenantId);            
        }
    }
}
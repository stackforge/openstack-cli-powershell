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
using Openstack.Objects.Domain.Compute;
using Openstack.Client.Powershell.Providers.Common;

namespace Openstack.Client.Powershell.Providers.Compute
{
    public class FlavorUIContainer : BaseUIContainer
    {
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
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Flavor flavor = (Flavor)this.Entity;
            this.WriteHeader("Flavor Details");
            this.WriteFlavorDetails(flavor);           
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteFlavorDetails(Flavor flavor)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("Id        : " + flavor.Id);
            Console.WriteLine("Name      : " + flavor.Name);
            Console.WriteLine("Disk      : " + flavor.Disk);
            Console.WriteLine("Ram       : " + flavor.Ram);
            Console.WriteLine("Status    : " + flavor.Status);
            Console.WriteLine("RxtxCap   : " + flavor.RxtxCap);
            Console.WriteLine("RxtxQuota : " + flavor.RxtxQuota);
            Console.WriteLine("Swap      : " + flavor.Swap);
            Console.WriteLine("Vcpus     : " + flavor.Vcpus);
            Console.WriteLine("");

        }
    }
}

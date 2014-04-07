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
using Openstack.Client.Powershell.Cmdlets.Common;
using System.Management.Automation;
using Openstack.Objects.Domain;
using Openstack.Client.Powershell.Providers.Common;

namespace Openstack.Client.Powershell.Cmdlets.Storage.CDN
{
    [Cmdlet(VerbsCommon.Get, "CDN", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.CDN)]
    public class GetCDNCmdlet : BasePSCmdlet
    {
//=========================================================================================
/// <summary>
/// The main driver..
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            List<StorageContainer> containers = this.RepositoryFactory.CreateCDNRepository().GetContainers();

            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The following Storage Containers are CDN enabled");          
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(""); 


            this.WriteObject(containers);
        }
    }
}

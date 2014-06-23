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
using System.Management.Automation;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Get", "Zone", SupportsShouldProcess = true)]
    public class GetZoneCmdlet : BasePSCmdlet
    {        
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {                             
            
            string path = @"C:\Users\plummert\Documents\WindowsPowerShell\Modules\AcmeInc\ServiceProvider.xml";

           ConfigurationManager manager = new ConfigurationManager();
           manager.WriteServiceProvider(path);
           
        AvailabilityZone zone = this.Context.CurrentServiceProvider.AvailabilityZones.Where(z => z.IsDefault == true).Single();

            if (zone != null)  {
                Console.WriteLine("");
                Console.WriteLine("Current Availability Zone is " + zone.Name);
                Console.WriteLine("");
            }
        }
    }
}

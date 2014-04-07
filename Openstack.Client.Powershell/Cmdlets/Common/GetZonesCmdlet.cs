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
using System.Xml.Linq;
using System.Xml.XPath;
using Openstack.Objects.Domain.Admin;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Get", "Zones", SupportsShouldProcess = true)]
    public class GetZonesCmdlet : BasePSCmdlet
    {
        private SwitchParameter _verbose = false;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Mandatory = false, ParameterSetName = "gc0", ValueFromPipelineByPropertyName = true, HelpMessage = "Prints extended information for each service.")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Verbose2
        {
            get { return _verbose; }
            set { _verbose = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowVerboseOutput(IEnumerable<XElement> zoneKeyNode)
        {
            foreach (XElement element in zoneKeyNode)
            {
                this.WriteHeaderSection("Zone : " + element.Attribute("name").Value);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                Console.WriteLine("Zone Id                : " + element.Attribute("id").Value);
                Console.WriteLine("Zone Name              : " + element.Attribute("name").Value);
                Console.WriteLine("Shell Foreground Color : " + element.Attribute("shellForegroundColor").Value);
                Console.WriteLine("");
                Console.WriteLine("The following Services are available from this Availability Zone");
                Console.WriteLine("----------------------------------------------------------------");
                this.WriteObject(this.Context.ServiceCatalog.GetAZServices(element.Attribute("name").Value));
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowNonVerboseOutput(IEnumerable<XElement> zoneKeyNode)
        {
            Console.WriteLine("");
            Console.WriteLine("Current Availability Zones include ");
            Console.WriteLine("");
            foreach (XElement element in zoneKeyNode)
            {
                Zone zone                 = new Zone();
                zone.Name                 = element.Attribute("name").Value;
                zone.ShellForegroundColor = element.Attribute("shellForegroundColor").Value;
                zone.Id                   = element.Attribute("id").Value;

                if (element.Attribute("isDefault").Value == "True")
                    zone.IsDefault = true;
                else
                    zone.IsDefault = false;

                this.WriteObject(zone);
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
            string configFilePath             = this.ConfigFilePath; ;
            XDocument doc                     = XDocument.Load(configFilePath);
            IEnumerable<XElement> zoneKeyNode = doc.Descendants("AvailabilityZone");

            if (_verbose)
            {
                this.ShowVerboseOutput(zoneKeyNode);
            }
            else
            {
                this.ShowNonVerboseOutput(zoneKeyNode);                
            }
        }
    }
}

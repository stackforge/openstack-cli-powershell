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
using OpenStack.Client.Powershell.Cmdlets.Common;
using System.Management.Automation;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenStack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Get", "Zones", SupportsShouldProcess = true)]
    public class GetZonesCmdlet : BasePSCmdlet
    {
        //private SwitchParameter _verbose = true;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Mandatory = false, ParameterSetName = "gz", ValueFromPipelineByPropertyName = true, HelpMessage = "Prints extended information for each service.")]
        //[Alias("v")]
        //[ValidateNotNullOrEmpty]
        //public SwitchParameter Verbose
        //{
        //    get { return _verbose; }
        //    set { _verbose = value; }
        //}
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowVerboseOutput()
        {
            foreach (AvailabilityZone zone in this.Context.CurrentServiceProvider.AvailabilityZones)
            {
                this.WriteHeaderSection("Zone : " + zone.Name);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), zone.ShellForegroundColor);
                Console.WriteLine("Zone Id                : " + zone.Id);
                Console.WriteLine("Zone Name              : " + zone.Name);
                Console.WriteLine("Shell Foreground Color : " + zone.ShellForegroundColor);
                Console.WriteLine("");
                Console.WriteLine("The following Services are available from this Availability Zone");
                Console.WriteLine("----------------------------------------------------------------");
                
                this.WriteObject(this.Context.ServiceCatalog.GetServicesInAvailabilityZone(zone.Name));
                Console.WriteLine("");
            }
            Console.WriteLine("");         
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowNonVerboseOutput()
        {
             foreach (AvailabilityZone zone in this.Context.CurrentServiceProvider.AvailabilityZones)
            {
                this.WriteHeaderSection("Zone : " + zone.Name);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), zone.ShellForegroundColor);
                Console.WriteLine("Zone Id                : " + zone.Id);
                Console.WriteLine("Zone Name              : " + zone.Name);
                Console.WriteLine("Shell Foreground Color : " + zone.ShellForegroundColor);
                Console.WriteLine("");              
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
            this.ShowVerboseOutput();

            //if (_verbose)
            //{
            //    this.ShowVerboseOutput();
            //}
            //else
            //{
            //    this.ShowNonVerboseOutput();
            //}
        }
    }
}

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
using System.Collections;
using OpenStack.Client.Powershell.Cmdlets.Common;
using System.IO;
using System.Xml;
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "Catalog", SupportsShouldProcess = true)]
    public class GetCatalogCmdlet : BasePSCmdlet
    {
        private SwitchParameter _verbose        = false;
 
        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Mandatory = false, ParameterSetName = "gc0", ValueFromPipelineByPropertyName = true, HelpMessage = "Prints information for the current service in use.")]
        //[Alias("cs")]
        //[ValidateNotNullOrEmpty]
        //public SwitchParameter CurrentService
        //{
        //    get { return _currentService; }
        //    set { _currentService = value; }
        //}
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
        #endregion
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private void WriteSection(string headerText)
        {
            WriteObject(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(headerText);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="endpoint"></param>
//=======================================================================================================
        private void PrintEndpoint(OpenStackServiceEndpoint endpoint)
        {
            Console.WriteLine("Region       : " + endpoint.Region);
            Console.WriteLine("Public URL   : " + endpoint.PublicUri.ToString());
            //Console.WriteLine("Internal URL : " + endpoint..InternalURL);
            //Console.WriteLine("Admin URL    : " + endpoint.AdminURL);
            Console.WriteLine("Version      : " + endpoint.Version);
            //Console.WriteLine("Version Info : " + endpoint.Version.InfoURL);
            //Console.WriteLine("Version List : " + endpoint.Version.ListURL);
            Console.WriteLine();
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="service"></param>
//=======================================================================================================
        private void PrintServiceVerbose(OpenStackServiceDefinition service)
        {
            Console.WriteLine("");
            this.WriteSection ("Service : " + service.Name);
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("General");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Name          : " + service.Name);
            Console.WriteLine("Type          : " + service.Type);
          
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine("Associated Endpoints");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;

            foreach (OpenStackServiceEndpoint endpoint in service.Endpoints)
            {
                this.PrintEndpoint(endpoint);
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void PrintServiceCatalog()
        {
            WriteObject("");
            this.WriteSection("You have access to the following Services ");
            WriteObject("");

            foreach (OpenStackServiceDefinition service in this.Context.ServiceCatalog)
            {
                if (!_verbose)
                    WriteObject(service);
                else
                    PrintServiceVerbose(service);
            }
            WriteObject("");
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            this.PrintServiceCatalog();
        }
    }
}

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
using OpenStack.Client.Powershell.Cmdlets.Common;
using OpenStack.Client.Powershell.Utility;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Set", "Zone", SupportsShouldProcess = true)]
    public class SetZoneCmdlet : BasePSCmdlet
    {
        private string _Zone;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "SetZone", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("z")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("1", "2", "3", "4", "5")]
        public string Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        } 
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {            
            if (this.Drive.Provider.Name != "OpenStack-Cloud" && this.Drive.Provider.Name != "Object Storage") {
                ShowError();
            }
            else
            {
                // Mark down the new default Region and reload the config file..

                XElement zoneKeyNode = SaveZone();
                this.Context.CurrentRegion = zoneKeyNode.Attribute("name").Value;
                this.Context.Settings.Load(this.ConfigFilePath);

                ConfigurationManager manager = new ConfigurationManager();
                manager.Load();

                this.SetUI(zoneKeyNode);
                this.ChanegDirectory();
                this.UpdateCache();

                // Now show the User what he is getting for this action..

                var abilities = new AccountCapabilities(this.SessionState, this.Context, this.CoreClient , this);
                abilities.WriteServices();
                abilities.WriteContainers();
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void ShowError()
        {
            ConsoleColor oldColor   = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("");
            Console.WriteLine("Setting the Availability Zone requires you to be connected to the OpenStack: drive or a valid Storage Container. Please map to one of these drive types and reissue the command.");
            Console.WriteLine("");
            Console.ForegroundColor = oldColor;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="zoneKeyNode"></param>
//=======================================================================================================
        private void SetUI(XElement zoneKeyNode)
        {
            this.Context.Forecolor  = zoneKeyNode.Attribute("shellForegroundColor").Value;
            Console.ForegroundColor =(ConsoleColor) Enum.Parse(typeof (ConsoleColor), zoneKeyNode.Attribute("shellForegroundColor").Value);
            Console.WriteLine("");
            Console.WriteLine("New Availability Zone " + zoneKeyNode.Attribute("name").Value + " selected.");
            Console.WriteLine("");
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void ChanegDirectory()
        {
            if (this.Drive.Provider.Name == "Object Storage")
               this.SessionState.InvokeCommand.InvokeScript(@"cd\");
            else
                this.SessionState.InvokeCommand.InvokeScript("cd c:");
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=======================================================================================================
        private XElement SaveZone()
        {
            string configFilePath    = this.ConfigFilePath;
            XDocument doc            = XDocument.Load(configFilePath);
            XElement zoneKeyNode     = doc.XPathSelectElement("//AvailabilityZone[@id='" + _Zone + "']");
            XElement defaultZoneNode = doc.XPathSelectElement("//AvailabilityZone[@isDefault='True']");

            defaultZoneNode.SetAttributeValue("isDefault", "False");
            zoneKeyNode.SetAttributeValue("isDefault", "True");
            doc.Save(configFilePath);
            return zoneKeyNode;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void WriteServices(string availabilityZone)
        {
            this.WriteHeader("This Availability Zone has the following services available.");
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            WriteObject(this.Context.ServiceCatalog.GetServicesInAvailabilityZone(availabilityZone));
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void WriteHeader(string message)
        {
            // Write out the commands header information first..

            WriteObject("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("===================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("===================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            WriteObject(" ");
        }
        #endregion
    }
}

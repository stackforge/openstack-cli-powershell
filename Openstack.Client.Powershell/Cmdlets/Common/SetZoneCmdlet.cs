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
using Openstack.Client.Powershell.Cmdlets.Common;
using System.Management.Automation;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using Openstack.Objects.Domain;
using Openstack.Client.Powershell.Providers.Storage;
using System.Collections.ObjectModel;
using Openstack.Common.Properties;
using Openstack.Objects.DataAccess;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Set", "Zone", SupportsShouldProcess = true)]
    public class SetZoneCmdlet : BasePSCmdlet
    {
        private string _Zone;

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
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (this.Drive.Provider.Name != "OS-Cloud" && this.Drive.Provider.Name != "OS-Storage")
            {
                ConsoleColor oldColor   = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("");
                Console.WriteLine("Setting the Availability Zone requires you to be connected to the OpenStack: drive or a valid Storage Container. Please map to one of these drive types and reissue the command.");
                Console.WriteLine("");
                Console.ForegroundColor = oldColor;
            }
            else
            {
                string configFilePath    = this.ConfigFilePath; 
                XDocument doc            = XDocument.Load(configFilePath);
                XElement zoneKeyNode     = doc.XPathSelectElement("//AvailabilityZone[@id='" + _Zone + "']");
                XElement defaultZoneNode = doc.XPathSelectElement("//AvailabilityZone[@isDefault='True']");

                defaultZoneNode.SetAttributeValue("isDefault", "False");
                zoneKeyNode.SetAttributeValue("isDefault", "True");
                doc.Save(configFilePath);
                this.Settings.Load(configFilePath);

                this.Context.Forecolor = zoneKeyNode.Attribute("shellForegroundColor").Value;
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), zoneKeyNode.Attribute("shellForegroundColor").Value);
                Console.WriteLine("");
                Console.WriteLine("New Availability Zone " + zoneKeyNode.Attribute("name").Value + " selected.");
                Console.WriteLine("");


                if (this.Drive.Name == "OpenStack")
                   this.SessionState.InvokeCommand.InvokeScript(@"cd\");
                else
                    this.SessionState.InvokeCommand.InvokeScript("cd c:");


                this.UpdateCache();
                this.WriteServices(zoneKeyNode.Attribute("name").Value);
                this.WriteContainers();
            }
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
            WriteObject(this.Context.ServiceCatalog.GetAZServices(availabilityZone));
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private System.Collections.ObjectModel.Collection<PSDriveInfo> GetAvailableDrives(Settings settings, ProviderInfo providerInfo)
        {
            List<StorageContainer> storageContainers = null;
            OSDriveParameters parameters = new OSDriveParameters();

            if (this.Settings != null)
            {
                parameters.Settings = this.Settings;
            }
            else
            {
                parameters.Settings = settings;
            }

            try
            {
                IContainerRepository storageContainerRepository = this.RepositoryFactory.CreateContainerRepository();
                storageContainers = storageContainerRepository.GetStorageContainers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

            // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

            try
            {
                if (storageContainers.Count > 0)
                {
                    foreach (StorageContainer storageContainer in storageContainers)
                    {
                        PSDriveInfo driveInfo = new PSDriveInfo(storageContainer.Name, providerInfo, "/", "Root folder for your storageContainer", null);
                        OSDriveInfo kvsDriveInfo = new OSDriveInfo(driveInfo, parameters, this.Context);
                        drives.Add(kvsDriveInfo);
                    }
                }
                else
                {
                    PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                    return new Collection<PSDriveInfo>   
                        {   
                        new OSDriveInfo(driveInfo, parameters, this.Context)   
                        };
                }
            }
            catch (Exception)
            {

            }

            return drives;
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
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void WriteContainers()
        {
            List<string> invalidDriveNames  = new List<string>();
            OSDriveParameters parameters  = new OSDriveParameters();
            HPOSNavigationProvider provider = new HPOSNavigationProvider();
            Collection<PSDriveInfo> drives  = this.GetAvailableDrives(this.Settings, this.SessionState.Provider.GetOne("OS-Storage"));

            if (drives != null)
            {
                this.WriteHeader("Storage Containers available in this Region include");

                // Remove the old Users drives first..

                Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("OS-Storage");
                foreach (PSDriveInfo deadDrive in deadDrives)
                {
                    this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
                }

                foreach (PSDriveInfo drive in drives)
                {
                    if (drive.Name != string.Empty)
                    {
                        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                        WriteObject("Storage Container : [" + drive.Name + "] now available.");
                    }

                    try
                    {
                        this.SessionState.Drive.New(drive, "local");
                    }
                    catch (PSArgumentException ex)
                    {
                        if (drive.Name != string.Empty)
                            invalidDriveNames.Add(drive.Name);
                    }

                }
                //WriteObject("");
            }
            else
            {
                // No storageContainers exist for the new credentials so make some up...

                PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                this.SessionState.Drive.New(new OSDriveInfo(driveInfo, parameters, this.Context), "local");
            }

            if (invalidDriveNames.Count > 0)
            {
                WriteObject("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteObject("=================================================================");
                Console.ForegroundColor = ConsoleColor.Red;
                WriteObject("Error : A subset of your Containers could not be bound to this");
                WriteObject("session due to naming conflicts with the naming standards required");
                WriteObject("for Powershell drives. These containers are listed below.");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteObject("=================================================================");
                Console.ForegroundColor = ConsoleColor.Green;
                WriteObject(" ");

                foreach (string name in invalidDriveNames)
                {
                    WriteObject(name);
                    WriteObject(" ");
                }
                //WriteObject(" ");
            }
        }
    }
}

//* ============================================================================
//Copyright 2014 Hewlett Packard
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//============================================================================ */
using System.Management.Automation;
using OpenStack.Client.Powershell.Utility;
using System.Linq;
using OpenStack.Identity;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using OpenStack.Client.Powershell.Providers.Storage;
using OpenStack.Storage;
using System.Threading.Tasks;
using System;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Set, "SP", SupportsShouldProcess = true)]
    public class SetServiceProviderCmdlet : BasePSCmdlet
    {
        private string _name;
        private SwitchParameter _setDefault = false;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "SetSP", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter SetDefault
        {
            get { return _setDefault; }
            set { _setDefault = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "SetSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
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
            ConfigurationManager manager = new ConfigurationManager();
            manager.Load();
            
            ServiceProvider provider = manager.GetServiceProvider(this.Name);
            provider.IsDefault       = this.SetDefault;

            this.WriteObject("");
            this.WriteObject(" - Connecting to OpenStack Provider " + this.Name);

            this.InitialzeServiceProvider(provider);           
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private bool DoesRequireCredentials()
        {
            return true;   
        }       
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private void InitialzeServiceProvider(ServiceProvider provider)
        {
            if (this.SetDefault)
            {
                ConfigurationManager configManager = new ConfigurationManager();
                configManager.Load();
                configManager.SetDefaultServiceProvider(provider.Name);
            }
         
            this.WriteObject(" - Loading Service Provider extensions ");
            ExtensionManager manager = new ExtensionManager(this.SessionState, this.Context);
            manager.LoadExtension(provider);
            this.ShowAccountState();     
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void ShowAccountState()
        {
            // Show the User the new ServiceCatalog that we just received..

            this.WriteObject(" - Success!");
            this.WriteObject("");
            this.WriteServices();

            // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

            if (this.Context.ServiceCatalog.Exists("Object Storage"))
            {
                this.WriteContainers();
            }      
        }
//=======================================================================================================
/// <summary>
/// Removes all currently registered drives..
/// </summary>
//=======================================================================================================
        private void RemoveDrives()
        {
            // Remove the old Users drives first..

            Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("Object Storage");
            foreach (PSDriveInfo deadDrive in deadDrives)
            {
                this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected void WriteServices()
        {
            WriteObject("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("=================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(" Binding to new Account. New service catalog is as follows.");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("=================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            WriteObject(" ");

            foreach (OpenStackServiceDefinition service in this.Context.ServiceCatalog)
            {               
                WriteObject(service);
            }
            WriteObject("");
        } 
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected void WriteContainers()
        {
            List<string> invalidDriveNames = new List<string>();
            OSDriveParameters parameters   = new OSDriveParameters();

            // Write out the commands header information first..

            WriteObject("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("===================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(" Object Storage Service available. Remapping to the following drives.");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("===================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            WriteObject(" ");

            HPOSNavigationProvider provider = new HPOSNavigationProvider();
            Collection<PSDriveInfo> drives = this.GetAvailableDrives(this.Context.Settings, this.SessionState.Provider.GetOne("Object Storage"));

            if (drives != null)
            {
                this.RemoveDrives();

                foreach (PSDriveInfo drive in drives)
                {
                    if (drive.Name != string.Empty)
                    {
                        WriteObject(" Storage Container : [" + drive.Name + "] now available.");
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
                    catch (Exception) { }

                }
                WriteObject("");
            }
            else
            {
                // No storageContainers exist for the new credentials so make some up...

                //PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                //this.SessionState.Drive.New(new OSDriveInfo(driveInfo, parameters, this.Context), "local");
            }

            if (invalidDriveNames.Count > 0)
            {
                WriteObject("");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteObject("=================================================================");
                Console.ForegroundColor = ConsoleColor.Red;
                WriteObject(" Error : A subset of your Containers could not be bound to this");
                WriteObject(" session due to naming conflicts with the naming standards required");
                WriteObject(" for Powershell drives. These containers are listed below.");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteObject("=================================================================");
                Console.ForegroundColor = ConsoleColor.Green;
                WriteObject(" ");

                foreach (string name in invalidDriveNames)
                {
                    WriteObject(name);
                    WriteObject(" ");
                }
                WriteObject(" ");
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private System.Collections.ObjectModel.Collection<PSDriveInfo> GetAvailableDrives(Settings settings, ProviderInfo providerInfo) //, string configFilePath)
        {
            IEnumerable<StorageContainer> storageContainers = null;
            OSDriveParameters parameters = new OSDriveParameters();

            if (this.Context.Settings != null)
            {
                parameters.Settings = this.Context.Settings;
            }
            else
            {
                parameters.Settings = settings;
            }

            try
            {
                Task <StorageAccount> getStorageAccountTask = this.CoreClient.CreateServiceClient<IStorageServiceClient>().GetStorageAccount();
                getStorageAccountTask.Wait();
                StorageAccount result = getStorageAccountTask.Result;
                storageContainers = result.Containers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

            // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

            try
            {
                string publicStoreUrl = this.Context.ServiceCatalog.GetPublicEndpoint("Object Storage", "region-a.geo-1").ToString();

                if (storageContainers.Count() > 0)
                {
                    foreach (StorageContainer storageContainer in storageContainers)
                    {
                        PSDriveInfo driveInfo = new PSDriveInfo(storageContainer.Name, providerInfo, "/", "Root folder for your storageContainer", null);
                        OpenStackPSDriveInfo kvsDriveInfo = new OpenStackPSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl);
                        try
                        {
                            drives.Add(kvsDriveInfo);
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                    return new Collection<PSDriveInfo>   
                        {   
                        new OpenStackPSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl)   
                        };
                }
            }
            catch (Exception ex)
            {
                int g = 7;
            }

            return drives;
        }
        #endregion
    }
}

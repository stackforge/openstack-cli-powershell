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
using System.Management.Automation;
using System.Xml.Linq;
using System.Linq;
using OpenStack;
using System.Xml.XPath;
using OpenStack.Client.Powershell.Providers.Common;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Identity;
using OpenStack.Client.Powershell.Providers.Storage;
using OpenStack.Storage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Set, "Config", SupportsShouldProcess = true)]
    public class SetConfigCmdlet : BasePSCmdlet
    {
        private string _key;
        private string _value;
        private string _configFilePath =   @"C:\Users\tplummer\Source\Repos\OpenStack-NewCLI\Rackspace.Client.Powershell\Deployment\Rackspace.config";
        private SwitchParameter _reset = false;
        private string _oldAccessKey;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Position = 1, Mandatory = false, ParameterSetName = "sc3", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        //[Alias("resetcfg")]
        //[ValidateNotNullOrEmpty]
        //public SwitchParameter Reset
        //{
        //    get { return _reset; }
        //    set { _reset = value; }
        //}
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "sc4", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public string ConfigFilePathKey
        {
            get { return _configFilePath; }
            set { _configFilePath = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Position = 1, Mandatory = true, ParameterSetName = "sc", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        //[Alias("k")]
        //[ValidateNotNullOrEmpty]
        //public string Key
        //{
        //    get { return _key; }
        //    set { _key = value; }
        //}
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Position = 2, Mandatory = true, ParameterSetName = "sc", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        //[Alias("v")]
        //[ValidateNotNullOrEmpty]
        //public string Value
        //{
        //    get { return _value; }
        //    set { _value = value; }
        //}
        #endregion
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
            WriteObject("Binding to new Account. New service catalog is as follows.");
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
//=================================================zx======================================================
        private void LoadConfigFile()
        {
            ConfigurationManager configManager = new ConfigurationManager();
            configManager.Load(this.ConfigFilePathKey);
            ServiceProvider provider = configManager.GetDefaultServiceProvider();  
            
            ExtensionManager loader = new ExtensionManager(this.SessionState, this.Context);
           
            loader.LoadCore(provider);
            loader.LoadExtension(provider);

            // Show the User the new ServiceCatalog that we just received..

            this.WriteServices();

             // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

            if (this.Context.ServiceCatalog.Exists("Object Storage"))
            {
                this.WriteContainers(_configFilePath);
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
            List<StorageContainer> storageContainers = null;
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
                Task<IEnumerable<StorageContainer>> getContainersTask = this.CoreClient.CreateServiceClient<IStorageServiceClient>().ListStorageContainers();
                getContainersTask.Wait();
                storageContainers = getContainersTask.Result.ToList<StorageContainer>();               
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

                if (storageContainers.Count > 0)
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
        protected void WriteContainers(string configFilePath)
        {
            List<string> invalidDriveNames = new List<string>();
            OSDriveParameters parameters = new OSDriveParameters();

            // Write out the commands header information first..
            
            WriteObject("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("===================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject("Object Storage Service available. Remapping to the following drives.");
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
                WriteObject(" ");
            }
        }
//======================================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="address"></param>
/// <returns></returns>
//======================================================================================================================
        private string GetContainerName(string url)
        {
            string[] elements = url.Split('/');
            return elements[elements.Length - 1];
        }
//======================================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="url"></param>
/// <returns></returns>
//======================================================================================================================
        private string GetDNSPortion(string url)
        {
            string[] elements = url.Split('/');
            return elements[2];
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        //private void LoadConfigFile(string configFilePath)
        //{           
        //    this.Settings         = Settings.LoadConfig(configFilePath);
        //    this.Context.Settings = this.Settings;
        //    this.Context          = this.Context;
                    
        //    // We need to ensure that the Users new identity doesn't alter the list bof available storageContainers. If so, just deal with it..

        //    if (_oldAccessKey != this.Settings.Username)
        //    {
        //        this.InitializeSession(this.Settings);
               
        //        // Show the User the new ServiceCatalog that we just received..

        //        this.WriteServices();

        //        // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

        //        if (this.Context.ServiceCatalog.DoesServiceExist("OS-Storage"))
        //        {
        //            this.WriteContainers(_configFilePath);
        //        }

        //        if (this.Drive.Name == "OpenStack")
        //        {
        //            this.SessionState.InvokeCommand.InvokeScript(@"cd\");
        //            ((CommonDriveInfo)this.Drive).CurrentContainer.Load();
        //        }
                
        //        this.SessionState.PSVariable.Set(new PSVariable("ConfigPath", configFilePath));
                
        //        //Context tempContext = (Context)this.SessionState.PSVariable.GetValue("Context", null);
        //        //this.UpdateCache(tempContext);
        //    }
        //}
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            this.LoadConfigFile();


            //if (_reset)
            //{
            //    _configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";
            //    this.LoadConfigFile();
            //}
            //else
            //{
            //    if (_configFilePath != null)
            //        this.LoadConfigFile(_configFilePath);
            //    else
            //        this.Settings[_key] = _value;
            //}
        }
    }
}

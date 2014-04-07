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
using Openstack.Common.Properties;
using System.Collections.ObjectModel;
using Openstack.Client.Powershell.Providers.Storage;
using System.Collections.Generic;
using Openstack.Objects.Domain;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain.Admin;
using Openstack.Objects.DataAccess.Security;
using Openstack.Objects.Utility;

namespace Openstack.Client.Powershell.Cmdlets.Common
{  
    public class BaseAuthenticationCmdlet : BasePSCmdlet
    {
        private string _key;
        private string _value;
        private SwitchParameter _reset = false;
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        protected void InitializeSession(AuthenticationRequest request, Settings settings = null)
        {
            Context context  = new Context();         

            if (request != null)
            {
                KeystoneAuthProvider authProvider = new KeystoneAuthProvider();
                AuthenticationResponse response   = authProvider.Authenticate(request);

                context.ServiceCatalog = response.ServiceCatalog;
                if (settings == null)
                    context.Settings = Settings.Default;
                else
                    context.Settings = settings;
                context.AccessToken    = response.Token;

                this.SessionState.PSVariable.Set(new PSVariable("Context", context));
                this.SessionState.PSVariable.Set(new PSVariable("BaseRepositoryFactory", new BaseRepositoryFactory(context)));
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        protected void InitializeSession(Settings settings)
        {
            AuthenticationRequest request = new AuthenticationRequest(new Credentials(settings.Username, settings.Password), settings.DefaultTenantId);
            this.InitializeSession(request, settings);            
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private System.Collections.ObjectModel.Collection<PSDriveInfo> GetAvailableDrives(Settings settings, ProviderInfo providerInfo, string configFilePath)
        {
            List<StorageContainer> storageContainers = null;
            OSDriveParameters parameters           = new OSDriveParameters();

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
                storageContainers                               = storageContainerRepository.GetStorageContainers(configFilePath);
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
                        PSDriveInfo driveInfo      = new PSDriveInfo(storageContainer.Name, providerInfo, "/", "Root folder for your storageContainer", null);
                        OSDriveInfo kvsDriveInfo = new OSDriveInfo(driveInfo, parameters, this.Context);
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
                        new OSDriveInfo(driveInfo, parameters, this.Context)   
                        };
                }
            }
            catch (Exception)
            {

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

            Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("OS-Storage");
            foreach (PSDriveInfo deadDrive in deadDrives)
            {
                this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
            }
        }
////=======================================================================================================
///// <summary>
///// removes only the currently registered shared drives..
///// </summary>
////=======================================================================================================
//        private void RemoveSharedDrives()
//        {
//            Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("OS-Storage");
//            foreach (OSDriveInfo deadDrive in deadDrives)
//            {
//                if (deadDrive.SharePath != null)
//                   this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
//            }
//        }      
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
            Collection<PSDriveInfo> drives = this.GetAvailableDrives(this.Settings, this.SessionState.Provider.GetOne("OS-Storage"), configFilePath);

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
                WriteObject(" ");
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
            WriteObject("Binding to new Account. New service catalog is as follows.");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("=================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            WriteObject(" ");

            foreach (Service service in this.Context.ServiceCatalog)
            {
                WriteObject(service);
            }
            WriteObject("");
        }  
    }
}

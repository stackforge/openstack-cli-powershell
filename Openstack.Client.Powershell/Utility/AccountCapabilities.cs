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
using System.Collections.ObjectModel;
using System.Management.Automation;
using OpenStack.Client.Powershell.Providers.Storage;
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Utility
{
    public class AccountCapabilities
    {
        private SessionState _session;
        private Context _context;
        private IOpenStackClient _coreClient;
        private Cmdlet _cmdlet;
        
        #region Properties

        public Cmdlet Cmdlet
        {
            get { return _cmdlet; }
            set { _cmdlet = value; }
        }

        public IOpenStackClient CoreClient
        {
            get { return _coreClient; }
            set { _coreClient = value; }
        }
    
        private Context Context
        {
            get { return _context; }
            set { _context = value; }
        }
        private SessionState SessionState
        {
            get { return _session; }
            set { _session = value; }
        } 
        #endregion
        #region Methods
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="session"></param>
/// <param name="context"></param>
/// <param name="coreClient"></param>
/// <param name="cmd"></param>
//=======================================================================================================
        public AccountCapabilities(SessionState session, Context context, IOpenStackClient coreClient, Cmdlet cmd)
        {
            _session    = session;
            _context    = context;
            _coreClient = coreClient;
            _cmdlet     = cmd;
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
        public void WriteContainers()
        {
            var invalidDriveNames = new List<string>();
            var parameters        = new ObjectStorageDriveParameters();
            var provider          = new ObjectStorageNavigationProvider();
            var converter         = new ObjectStorageDriveConverter(this.Context, _session.Drive.Current.Provider, this.CoreClient);
            var drives            = converter.ConvertContainers();
         
            if (drives != null)
            {
                this.WriteHeader("Storage Containers available in this AZ include");

                // Remove the old Users drives first..

                Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("Object Storage");
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

                PSDriveInfo driveInfo = new PSDriveInfo("HPOS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                this.SessionState.Drive.New(new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, this.Context.ServiceCatalog.GetPublicEndpoint("object-store", this.Context.CurrentRegion)), "local");
            }

            if (invalidDriveNames.Count > 0) {
                ShowNameConflictError(invalidDriveNames);
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="invalidDriveNames"></param>
//=======================================================================================================
        private void ShowNameConflictError(List<string> invalidDriveNames)
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
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="text"></param>
//=======================================================================================================
        private void WriteObject(string text)
        {
            this.Cmdlet.WriteObject(text);
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public void WriteServices()
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

            foreach (OpenStackServiceDefinition service in this.Context.ServiceCatalog.GetServicesInAvailabilityZone(this.Context.CurrentRegion))
            {
                this.Cmdlet.WriteObject(service);
            }
            WriteObject("");
        }
        #endregion
    }
}
         
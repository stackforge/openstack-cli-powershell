
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
using System.Management.Automation.Provider;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenStack.Client.Powershell.Utility;
using System.Linq;
using Openstack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Providers.Common
{
    public class BaseNavigationCmdletProvider : NavigationCmdletProvider  
    {
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected OpenStackClient Client
        {
            get
            {
                return (OpenStackClient)this.SessionState.PSVariable.Get("CoreClient").Value;
            }
            set
            {
                this.SessionState.PSVariable.Set(new PSVariable("CoreClient", value));
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        protected Settings Settings
        {
            get
            {
                return this.Context.Settings;
            }            
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        protected Context Context
        {
            get
            {
                return (Context)this.SessionState.PSVariable.GetValue("Context", null);
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private bool IsContextInitialized()
        {
            if (this.SessionState.PSVariable.GetValue("Context", null) == null) {
                return false;
            }
            else
            {
                return true;
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        protected string ConfigFilePath
        {
            get
            {
                try
                {
                    return (string)this.SessionState.PSVariable.Get("ConfigPath").Value;
                }
                catch (Exception)
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config";
                }
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private void SetZoneColor(ServiceProvider provider)
        {
            AvailabilityZone defZone           = provider.AvailabilityZones.Where(z => z.IsDefault == true).SingleOrDefault();    
            Console.ForegroundColor            = (ConsoleColor)Enum.Parse(typeof(ConsoleColor),  defZone.ShellForegroundColor);
            this.Host.UI.RawUI.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor),  defZone.ShellForegroundColor);
            this.Context.Forecolor             = defZone.ShellForegroundColor;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        protected void InitializeSession()
        {
            ConfigurationManager configManager = new ConfigurationManager();
            ExtensionManager loader            = new ExtensionManager(this.SessionState, this.Context);
            configManager.Load();

            ServiceProvider provider = configManager.GetDefaultServiceProvider();           

            if (provider.Name == String.Empty && provider.IsDefault == true)
            {
                // Technically Core is already loaded (you're in it :) but this signs in for you to the ServiceProvider selected..
                // This is just used in the case where it's the Users first time loading the CLI..

                loader.LoadCore(provider);
            }
            else
            {
                // Load any extensions that were supplied by the ServiceProvider...

                loader.LoadExtension(provider);
            }

            this.SetZoneColor(provider);            
      }
        #region Implementation of DriveCmdletProvider    
//==================================================================================================
/// <summary>
/// Removes an Item from the store..
/// </summary>
/// <param name="path"></param>
//==================================================================================================
        protected override void ClearItem(string path)
        {
            base.ClearItem(path);
        }
//==================================================================================================
/// <summary>
/// Called when the user decides to delete a KVSDrive.
/// </summary>
/// <param name="drive"></param>
/// <returns></returns>
//==================================================================================================
        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, drive));
                return null;
            }
            return drive;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <param name="returnContainers"></param>
//==================================================================================================
        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            WriteItemObject(path, path, true);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override string GetChildName(string path)
        {
             return base.GetChildName(path);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override bool ItemExists(string path)
        {         
            return true;
        }
        #endregion    
//==================================================================================================
/// <summary>
/// This test should not verify the existance of the item at the path. 
/// It should only perform syntactic and semantic validation of the 
/// path. For instance, for the file system provider, that path should
/// be canonicalized, syntactically verified, and ensure that the path
/// does not refer to a device.
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override bool IsValidPath(string path)
        {
            return true;
        }
    }
}

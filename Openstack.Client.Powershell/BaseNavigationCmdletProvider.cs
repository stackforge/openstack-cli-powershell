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
using Openstack;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Openstack.Client.Powershell.Providers.Storage;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using Openstack.Client.Powershell.Utility;
using Openstack.Storage;
using System.Security;
using System.Linq;
using Openstack.Identity;


namespace Openstack.Client.Powershell.Providers.Common
{
    public class BaseNavigationCmdletProvider : NavigationCmdletProvider  
    {
        
        OpenstackClient _client; // = new OpenstackClient(credential, CancellationToken.None);

        
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected OpenstackClient Client
        {
            get
            {                
                return (OpenstackClient)this.SessionState.PSVariable.Get("Client").Value;
            }
            set
            {
                this.SessionState.PSVariable.Set(new PSVariable("Client", value));
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
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";
                }
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private void SetZoneColor()
        {
            string configFilePath              = this.ConfigFilePath;
            XDocument doc                      = XDocument.Load(configFilePath);
            XElement defaultZoneNode           = doc.XPathSelectElement("//AvailabilityZone[@isDefault='True']");
            Console.ForegroundColor            = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), defaultZoneNode.Attribute("shellForegroundColor").Value);
            this.Host.UI.RawUI.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), defaultZoneNode.Attribute("shellForegroundColor").Value);
            this.Context.Forecolor             = defaultZoneNode.Attribute("shellForegroundColor").Value;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        protected void InitializeSession()
        {            
            CredentialManager manager       = new CredentialManager(false);
            IOpenstackCredential credential = manager.GetCredentials(false);                   

            // Connect to the Service Provider..
            
            var client        = new OpenstackClient(credential, CancellationToken.None);
            var connectTask   = client.Connect();
            connectTask.Wait();
            
            // Setup the environment based on what came back from Auth..

            Context context           = new Context();
            context.ServiceCatalog    = credential.ServiceCatalog;
            context.Settings          = Settings.Default;
            context.ProductName       = "Openstack-WinCLI";
            context.Version           = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.SessionState.PSVariable.Set(new PSVariable("Context", context));
            this.SessionState.PSVariable.Set(new PSVariable("Client", client));           
            this.SetZoneColor();
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

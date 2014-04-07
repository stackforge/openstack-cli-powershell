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
using Openstack.Objects.DataAccess;
using Openstack.Common.Properties;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using Openstack.Client.Powershell.Providers.Storage;
using Openstack.Objects.Utility;
using Openstack.Objects.Domain.Admin;
using Openstack.Objects.DataAccess.Security;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Openstack.Client.Powershell.Providers.Common
{
    public class BaseNavigationCmdletProvider : NavigationCmdletProvider  
    {
        static BaseRepositoryFactory _repositoryFactory;
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected BaseRepositoryFactory RepositoryFactory
        {
            get
            {
                return (BaseRepositoryFactory)this.SessionState.PSVariable.Get("BaseRepositoryFactory").Value;
            }
            set
            {
                this.SessionState.PSVariable.Set(new PSVariable("BaseRepositoryFactory", value));
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
            if (!IsContextInitialized())
            {
                Context context               = new Context();
                CredentialManager manager     = new CredentialManager(false);
                AuthenticationRequest request = manager.BuildAuthenticationRequest();

                if (request != null)
                {
                    KeystoneAuthProvider authProvider = new KeystoneAuthProvider();
                    AuthenticationResponse response   = authProvider.Authenticate(request);

                    context.ServiceCatalog    = response.ServiceCatalog;
                    context.Settings          = Settings.Default;
       
                    context.AccessToken       = response.Token;
                    context.ProductName       = "Openstack-WinCLI";
                    context.Version           = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    this.SessionState.PSVariable.Set(new PSVariable("Context", context));
                    this.SessionState.PSVariable.Set(new PSVariable("BaseRepositoryFactory", new BaseRepositoryFactory(context)));
                    this.SetZoneColor();

                    string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    //string currentVersion = "1.4.0.0";
                    UpdateManager updateManager = new UpdateManager(this.Context, currentVersion, this.RepositoryFactory);
                    updateManager.ProcessUpdateCheck();
                }
            }
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
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="graph"></param>
/// <param name="path"></param>
//==================================================================================================
        protected void WriteJSON<T> (T graph, string path)
        {
            MemoryStream stream            = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(stream, graph);

            string retValue = Encoding.Default.GetString(stream.ToArray());
            WriteItemObject(retValue, path, false);
            WriteItemObject("", path, false);         
        }
//==================================================================================================
/// <summary>
/// Writes out the files represented as StorageObjects for the supplied path.
/// </summary>
//==================================================================================================
        protected void WriteXML<T>(T graph, string path)
        {
            XmlTextWriter xtw        = null;
            MemoryStream stream      = new MemoryStream();
            StringBuilder builder    = new StringBuilder();
            XmlDocument document     = new XmlDocument();
            StringWriter writer      = null;
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            try
            {
                serializer.Serialize(stream, graph);
                stream.Position = 0;
                document.Load(stream);

                writer         = new StringWriter(builder);
                xtw            = new XmlTextWriter(writer);
                xtw.Formatting = Formatting.Indented;

                document.WriteTo(xtw);
            }
            finally
            {
                xtw.Close();
            }
            WriteItemObject(builder.ToString(), path, false);
            WriteItemObject("", path, false);
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
/// 
/// </summary>
//==================================================================================================
        protected ResponseFormat ResponseFormat
        {
            get
            {
                try
                {
                    return (ResponseFormat)this.SessionState.PSVariable.Get("ResponseFormat").Value;
                }
                catch (Exception)
                {
                    PSVariable variable = new PSVariable("ResponseFormat");
                    variable.Value = ResponseFormat.data;

                    this.SessionState.PSVariable.Set(variable);
                    return ResponseFormat.data;
                }
            }
        }      
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

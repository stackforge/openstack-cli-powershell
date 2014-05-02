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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml.XPath;
using System;
using System.Management.Automation;
using System.Reflection;
using System.Security.Policy;
using OpenStack.Client.Powershell.Utility;
using System.Linq;
using OpenStack.Identity;
using System.Threading;

namespace OpenStack.Client.Powershell.Utility
{
    public sealed partial class Settings : Hashtable
    {
        private static Settings defaultInstance = new Settings();
        private static string _configFilePath;
        private XDocument _document;

        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set { _configFilePath = value; }
        }
        
        #region Ctors      
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public Settings(Hashtable settings)
        {
            this.Load(settings);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public Settings(bool isEmpty)
        {
           
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public Settings()
        {
            this.Reset();
        }
        #endregion           
        #region File Transfer Settings..
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public bool UseCleanLargeFileCopies
        {
            get
            {
                return Convert.ToBoolean(this["UseCleanLargeFileCopies"]);
            }
            set
            {
                this["UseCleanLargeFileCopies"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public int MaxSegmentCopyRetries
        {
            get
            {
                return Convert.ToInt32(this["MaxSegmentCopyRetries"]);
            }
            set
            {
                this["MaxSegmentCopyRetries"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string LargeFileSize
        {
            get
            {
                return (string)this["largeFileSize"];
            }
            set
            {
                this["largeFileSize"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string DefaultSegmentNumber
        {
            get
            {
                return (string)this["defSegmentNumber"];
            }
            set
            {
                this["defSegmentNumber"] = value;
            }
        }
        #endregion
        #region General Operations
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public bool IsMocked
        {
            get
            {
                return ((bool)(this["IsMocked"]));
            }
            set
            {
                this["IsMocked"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public bool PasteGetURIResultsToClipboard
        {
            get
            {
                return Convert.ToBoolean(this["PasteGetURIResultsToClipboard"]);
            }
            set
            {
                this["PasteGetURIResultsToClipboard"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string ReleaseNotesURI
        {
            get
            {
                return (string)this["ReleaseNotesURI"];
            }
            set
            {
                this["ReleaseNotesURI"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string NewReleaseFolder
        {
            get
            {
                return (string)this["NewReleaseFolder"];
            }
            set
            {
                this["NewReleaseFolder"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string DefaultZone
        {
            get
            {
                return (string)this["DefaultZone"];
            }
            set
            {
                this["DefaultZone"] = value;
            }
        }
        #endregion
        #region Connecting to a Server Settings
        //==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string CredentialListPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OS\CredentialList.xml";
            }         
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public bool EnableCredentialTracking
        {
            get
            {
                return Convert.ToBoolean(this["EnableCredentialTracking"]);
            }
            set
            {
                this["EnableCredentialTracking"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string SSHClientPath
        {
            get
            {
                return (string)this["SSHClientPath"];
            }
            set
            {
                this["SSHClientPath"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string KeyStoragePath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }         
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string LogReadAttemptIntervalInMilliSeconds
        {
            get
            {
                return (string)this["LogReadAttemptIntervalInMilliSeconds"];
            }
            set
            {
                this["LogReadAttemptIntervalInMilliSeconds"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string LogReadAttemptsMax
        {
            get
            {
                return (string)this["LogReadAttemptsMax"];
            }
            set
            {
                this["LogReadAttemptsMax"] = value;
            }
        }
        #endregion
        #region Testing
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestFilePath
        {
            get
            {
                return ((string)(this["TestFilePath"]));
            }
            set
            {
                this["TestFilePath"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestImageId
        {
            get
            {
                return ((string)(this["TestImageId"]));
            }
            set
            {
                this["TestImageId"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestServerName
        {
            get
            {
                return ((string)(this["TestServerName"]));
            }
            set
            {
                this["TestServerName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestNetworkName
        {
            get
            {
                return ((string)(this["TestNetworkName"]));
            }
            set
            {
                this["TestNetworkName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestPortName
        {
            get
            {
                return ((string)(this["TestPortName"]));
            }
            set
            {
                this["TestPortName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestRouterName
        {
            get
            {
                return ((string)(this["TestRouterName"]));
            }
            set
            {
                this["TestRouterName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestSecurityGroupName
        {
            get
            {
                return ((string)(this["TestSecurityGroupName"]));
            }
            set
            {
                this["TestSecurityGroupName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestSnapshotName
        {
            get
            {
                return ((string)(this["TestSnapshotName"]));
            }
            set
            {
                this["TestSnapshotName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestSubnetName
        {
            get
            {
                return ((string)(this["TestSubnetName"]));
            }
            set
            {
                this["TestSubnetName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestVolumeName
        {
            get
            {
                return ((string)(this["TestVolumeName"]));
            }
            set
            {
                this["TestVolumeName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestFlavorId
        {
            get
            {
                return ((string)(this["TestFlavorId"]));
            }
            set
            {
                this["TestFlavorId"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string LocalTestDirectory
        {
            get
            {
                return (string)this["LocalTestDirectory"];
            }
            set
            {
                this["LocalTestDirectory"] = value;
            }
        }

//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public int HttpTimeoutInterval
        {
            get
            {
                return Convert.ToInt32(this["HttpTimeoutInterval"]);
            }
            set
            {
                this["HttpTimeoutInterval"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestStorageContainerSecondary
        {
            get
            {
                return (string)this["TestStorageContainerSecondaryName"];
            }
            set
            {
                this["TestStorageContainerSecondaryName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestStorageContainerName
        {
            get
            {
                return (string)this["TestStorageContainerName"];
            }
            set
            {
                this["TestStorageContainerName"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string TestVolumeIdUnattached
        {
            get
            {
                return (string)this["TestVolumeIdUnattached"];
            }
            set
            {
                this["TestVolumeIdUnattached"] = value;
            }
        }
        #endregion
        #region Authentication
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string DefaultTenantId
        {
            get
            {
                return ((string)(this["DefaultTenantId"]));
            }
            set
            {
                this["DefaultTenantId"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string Username
        {
            get
            {
                return (string)this["Username"];
            }
            set
            {
                this["Username"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string Host
        {
            get
            {
                return ((string)(this["Host"]));
            }
            set
            {
                this["Host"] = value;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public string AuthenticationServiceURI
        {
            get
            {
                return (string)this["AuthenticationServiceURI"];
            }
            set
            {
                this["AuthenticationServiceURI"] = value;
            }
        }
#endregion    
        #region Methods
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public void Reset()
        {
            if (File.Exists("OpenStack.config"))
            {
                this.Load("OpenStack.config");
            }
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config"))
            {
                string configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config";
                this.Load(configFilePath);
            }
            else
            {
                throw new InvalidOperationException("Unable to locate OpenStack.config file.");
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="settings"></param>
//==================================================================================================
        public void Load (Hashtable settings)
        {
            foreach (DictionaryEntry entry in settings) {
                this.Add(entry.Key, entry.Value);
            }
        }
//==================================================================================================
/// <summary>
/// Fully Qualified path to a config file
/// </summary>
/// <param name="settings"></param>
//==================================================================================================
        public static Settings LoadConfig(string configFilePath)
        {
            try
            {
                Settings settings  = new Settings(true);
                XDocument document = XDocument.Load(configFilePath);
               _configFilePath = configFilePath;

                foreach (XElement element in document.Descendants("add"))
                {
                    string key   = element.Attribute(XName.Get("key")).Value;
                    string value = element.Attribute(XName.Get("value")).Value;
                    settings.Add(key, value);
                }
                return settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
//==================================================================================================
/// <summary>
/// Fully Qualified path to a config file 
/// </summary>
/// <param name="settings"></param>
//==================================================================================================
        public void Load(string configFilePath)
        {
            try
            {               
                XDocument document = XDocument.Load(configFilePath);
                _configFilePath = configFilePath;
                this.Clear();
                foreach (XElement element in document.Descendants("add"))
                {
                    string key   = element.Attribute(XName.Get("key")).Value;
                    string value = element.Attribute(XName.Get("value")).Value;
                    try
                    {
                        this.Add(key, value);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }           
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private string GetConfigPath()
        {
            //if (_configFilePath != null)
            //    return _configFilePath;

            if (File.Exists("OpenStack.config"))
            {
                return this.GetType().Assembly.Location;
            }
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config"))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config";
            }
            else
            {
                throw new InvalidOperationException("Unable to locate OpenStack.config file.");
            }
        }

        #endregion
    }
}


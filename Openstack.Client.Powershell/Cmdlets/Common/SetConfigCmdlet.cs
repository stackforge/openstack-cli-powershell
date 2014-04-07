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
using Openstack.Common.Properties;
using Openstack.Objects.Domain;
using System.Xml.XPath;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Utility;
using Openstack.Client.Powershell.Providers.Common;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Set, "Config", SupportsShouldProcess = true)]
    public class SetConfigCmdlet : BaseAuthenticationCmdlet
    {
        private string _key;
        private string _value;
        private string _configFilePath = null;
        private SwitchParameter _reset = false;
        private string _oldAccessKey;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "sc3", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("resetcfg")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Reset
        {
            get { return _reset; }
            set { _reset = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "sc4", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
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
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "sc", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("k")]
        [ValidateNotNullOrEmpty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, Mandatory = true, ParameterSetName = "sc", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void LoadConfigFile()
        {           
            this.InitializeSession(Settings.Default);

            // Show the User the new ServiceCatalog that we just received..

            this.WriteServices();

            // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

            if (this.Context.ServiceCatalog.DoesServiceExist("OS-Storage"))
            {
                this.WriteContainers(_configFilePath);
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
        private void LoadConfigFile(string configFilePath)
        {           
            this.Settings         = Settings.LoadConfig(configFilePath);
            this.Context.Settings = this.Settings;
            this.Context          = this.Context;
                    
            // We need to ensure that the Users new identity doesn't alter the list bof available storageContainers. If so, just deal with it..

            if (_oldAccessKey != this.Settings.Username)
            {
                this.InitializeSession(this.Settings);
               
                // Show the User the new ServiceCatalog that we just received..

                this.WriteServices();

                // If ObjectStorage is among those new Services, show the new Container set bound to those credentials..

                if (this.Context.ServiceCatalog.DoesServiceExist("OS-Storage"))
                {
                    this.WriteContainers(_configFilePath);
                }

                if (this.Drive.Name == "OpenStack")
                {
                    this.SessionState.InvokeCommand.InvokeScript(@"cd\");
                    ((CommonDriveInfo)this.Drive).CurrentContainer.Load();
                }
                
                this.SessionState.PSVariable.Set(new PSVariable("ConfigPath", configFilePath));
                
                //Context tempContext = (Context)this.SessionState.PSVariable.GetValue("Context", null);
                //this.UpdateCache(tempContext);
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            if (_reset)
            {
                _configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";
                this.LoadConfigFile();
            }
            else
            {
                if (_configFilePath != null)
                    this.LoadConfigFile(_configFilePath);
                else
                    this.Settings[_key] = _value;
            }
        }
    }
}

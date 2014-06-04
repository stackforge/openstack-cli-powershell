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
using System.Management.Automation;
using OpenStack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Set, "Config", SupportsShouldProcess = true)]
    public class SetConfigCmdlet : BasePSCmdlet
    {
        //private string _configFilePath =   @"C:\Users\tplummer\Source\Repos\OpenStack-NewCLI\Rackspace.Client.Powershell\Deployment\Rackspace.config";
        private string _configFilePath = null;
        private SwitchParameter _reset = false;

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
        #endregion
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=================================================zx======================================================
        private void LoadConfigFile()
        {
            var configManager = new ConfigurationManager();
            configManager.Load(this.ConfigFilePathKey);
            ServiceProvider provider = configManager.GetDefaultServiceProvider();  
            
            var loader = new ExtensionManager(this.SessionState, this.Context);
           
            loader.LoadCore(provider);
            loader.LoadExtension(provider);

            // Show the User the new ServiceCatalog that we just received..

            var capabilities = new AccountCapabilities(this.SessionState, this.Context, this.CoreClient, this);
            capabilities.WriteServices();
            capabilities.WriteContainers();
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            this.LoadConfigFile();
        }
    }
}

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
            var manager = new ConfigurationManager();
            manager.Load();
            
            var provider = manager.GetServiceProvider(this.Name);
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
                var configManager = new ConfigurationManager();
                configManager.Load();
                configManager.SetDefaultServiceProvider(provider.Name);
            }
         
            this.WriteObject(" - Loading Service Provider extensions ");
            var manager = new ExtensionManager(this.SessionState, this.Context);
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

            // This is a hack for sure.. Need to move Zones into Vendor specific section.

    this.Context.CurrentRegion = "region-a.geo-1";

            var capabilities = new AccountCapabilities(this.SessionState, this.Context, this.CoreClient, this);
            capabilities.WriteServices();
            capabilities.WriteContainers();
        }
        #endregion
    }
}

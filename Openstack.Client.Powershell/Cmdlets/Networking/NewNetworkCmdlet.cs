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
using System.Management.Automation;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.DataAccess.Networking;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet("New", "Network", SupportsShouldProcess = true)]
    public class NewNetworkCmdlet : BasePSCmdlet
    {
        private bool _adminStateUp = true;
        private bool _force = true;
        private string _name;
        private string _cidr =  "11.0.3.0/24";

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "NewNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 3, ParameterSetName = "NewNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("cidr")]
        public string CidrValue
        {
            get { return _cidr; }
            set { _cidr = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//========================================================================================= 
        [Parameter(Position = 1, ParameterSetName = "NewNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("asu")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter AdminStateUp
        {
            get { return _adminStateUp; }
            set { _adminStateUp = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//========================================================================================= 
        [Parameter(Position = 2, ParameterSetName = "NewNetwork", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("f")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Force
        {
            get { return _force; }
            set { _force = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
        private void BuilderEvent(object sender, CreateNetworkEventArgs e)
        {
            Console.WriteLine("- " + e.Message);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//========================================================================================= 
        protected override void ProcessRecord()
        {
            Console.WriteLine("");
            NewNetwork newNetwork   = new NewNetwork();
            newNetwork.AdminStateUp = _adminStateUp;
            newNetwork.Name         = _name;

            if (this.CidrValue == null) {
                newNetwork.Cidr = "11.0.3.0/24";
            }
            else{
                newNetwork.Cidr = this.CidrValue;
            }

            INetworkRepository repository = this.RepositoryFactory.CreateNetworkRepository();
            repository.Changed += new Openstack.Objects.DataAccess.Networking.NetworkRepository.CreateNetworkEventHandler(BuilderEvent);
            repository.SaveNetwork(newNetwork, this.Force);
            repository.Changed -= new Openstack.Objects.DataAccess.Networking.NetworkRepository.CreateNetworkEventHandler(BuilderEvent);
            Console.WriteLine("  Network Build Complete!");
            Console.WriteLine("");
        }
        #endregion
    }
}

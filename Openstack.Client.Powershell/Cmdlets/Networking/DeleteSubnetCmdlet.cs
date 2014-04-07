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
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.DataAccess.Networking;
using Openstack.Objects.Domain;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet(VerbsCommon.Remove, "Subnet", SupportsShouldProcess = true)]
    //[RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class DeleteSubnetCmdlet : BasePSCmdlet
    {
        private string _subnetId;
        private SwitchParameter _force = false;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "DeleteSubnet", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("id")]
        public string SubnetId
        {
            get { return _subnetId; }
            set { _subnetId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 1, ParameterSetName = "DeleteSubnet2", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("all")]
        public SwitchParameter RemoveAll
        {
            get { return _force; }
            set { _force = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void RemoveAllSubnets()
        {
            ISubnetRepository repository = this.RepositoryFactory.CreateSubnetRepository();
            Console.WriteLine("");

            foreach (BaseEntity entity in this.CurrentContainer.Entities)
            {
                if (entity.Name != null)
                    Console.WriteLine("Removing Subnet : " + entity.Name);
                else
                    Console.WriteLine("Removing Subnet : " + entity.Id);

                repository.DeleteSubnet(entity.Id);
            }
            Console.WriteLine("");       
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string id = this.TranslateQuickPickNumber(this.SubnetId);

            if (_force && this.UserConfirmsDeleteAction("Subnets"))
            {
                    this.RemoveAllSubnets();
                    this.UpdateCache();               
                }
            else
            {
                this.RepositoryFactory.CreateSubnetRepository().DeleteSubnet(id);
                this.UpdateCache();
            }
        }
    }
}

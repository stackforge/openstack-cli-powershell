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
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Compute;
using System.Linq;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Common;
using System.Collections;
using System.Collections.Generic;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet(VerbsCommon.Remove, "Metadata", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveMetadataCmdletd : BasePSCmdlet
    {
        private string _key;
        private string _serverId;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "RemoveMetadata", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("id")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "RemoveMetadata", ValueFromPipelineByPropertyName = true, HelpMessage = "sdfgh")]
        [Alias("k")]
        [ValidateNotNullOrEmpty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="key"></param>
//=========================================================================================
        private void RemoveElement(string key)
        {
            IList elements = ((CommonDriveInfo)this.Drive).CurrentContainer.Entities;
            MetaDataElement mdElement = null;

            foreach (MetaDataElement element in elements)
            {
                if (element.Key == this.Key)
                {
                    mdElement = element;
                }
            }

            elements.Remove(mdElement);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (this.ServerId != null)
            {
                this.RepositoryFactory.CreateServerRepository().DeleteMetadata(this.ServerId, this.Key);
                this.RemoveElement(this.Key);
            }
            else
            {
                BaseUIContainer currentContainer = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;

                if (currentContainer.Name == "Metadata")
                {
                    ServerUIContainer serverContainer = currentContainer.Parent as ServerUIContainer;

                    if (serverContainer != null) {
                        this.RepositoryFactory.CreateServerRepository().DeleteMetadata(serverContainer.Entity.Id, this.Key);
                        this.RemoveElement(this.Key);
                    }
                }
                else {
                    this.RepositoryFactory.CreateServerRepository().DeleteMetadata(currentContainer.Entity.Id, this.Key);
                    this.RemoveElement(this.Key);
                }
            }
        }
    }

    }
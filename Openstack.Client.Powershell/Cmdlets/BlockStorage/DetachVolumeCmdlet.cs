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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.BlockStorage;
using Openstack.Client.Powershell.Providers.BlockStorage;
using Openstack.Client.Powershell.Providers.Security;

namespace Openstack.Client.Powershell.Cmdlets.BlockStorage
{ 
    [Cmdlet("Detach", "Volume", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.BlockStorage)]
    public class DetachVolumeCmdlet : BasePSCmdlet
    {
        private string _serverId;
        private string _volumeId;

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "DettachVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("s")]
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
        [Parameter(Position = 1, ParameterSetName = "DettachVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public string VolumeId
        {
            get { return _volumeId; }
            set { _volumeId = value; }
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
            this.RepositoryFactory.CreateVolumeRepository().DetachVolume(this.VolumeId, this.ServerId);
            this.UpdateCache<AttachmentsUIContainer>();
            this.UpdateCache<VolumesUIContainer>();
        }
        #endregion
    }
}





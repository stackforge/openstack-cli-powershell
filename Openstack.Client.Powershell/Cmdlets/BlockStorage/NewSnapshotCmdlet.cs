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
using Openstack.Client.Powershell.Providers.Security;
using System;

namespace Openstack.Client.Powershell.Cmdlets.BlockStorage
{ 
    [Cmdlet(VerbsCommon.New, "Snapshot", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.BlockStorage)]
    public class NewSnapshotCmdlet : BasePSCmdlet
    {
        private string _name;
        private SwitchParameter _force = false;
        private string _description;
        private string _volumeId;

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewSnapshot", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("n")]
        [ValidateNotNullOrEmpty]
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
        [Parameter(Position = 1, ParameterSetName = "NewSnapshot", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 2, ParameterSetName = "NewSnapshot", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public string VolumeId
        {
            get { return _volumeId; }
            set { _volumeId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 4, ParameterSetName = "NewSnapshot", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("f")]
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
//=========================================================================================
        private bool IsVolumeAvailable(string volumeId)
        {
            Volume volume = this.RepositoryFactory.CreateVolumeRepository().GetVolume(volumeId);
            if (volume.Status == "in-use")
                return false;
            else
                return true;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (this.IsVolumeAvailable(this.VolumeId))
            {
                NewSnapshot snapshot = new NewSnapshot();
                snapshot.Description = this.Description;
                snapshot.Name        = this.Name;
                snapshot.VolumeId    = this.VolumeId;
                snapshot.Force       = this.Force;

                this.RepositoryFactory.CreateSnapshotRepository().SaveSnapshot(snapshot);
                this.UpdateCache<SnapshotsUIContainer>();
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("The specified Volume is already in use.");
                Console.WriteLine("");
            }
        }
        #endregion
    }
}





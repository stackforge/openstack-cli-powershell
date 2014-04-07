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
    [Cmdlet("Backup", "Volume", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.BlockStorage)]
    public class BackupVolumeCmdlet : BasePSCmdlet
    {
        private NewVolumeBackup _newBackup = new NewVolumeBackup();

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, ParameterSetName = "BackupVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("c")]
        [ValidateNotNullOrEmpty]
        public string Container
        {
            get { return _newBackup.Container; }
            set { _newBackup.Container = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, ParameterSetName = "BackupVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public string Description
        {
            get { return _newBackup.Description; }
            set { _newBackup.Container = value; }
        }       

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "BackupVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public string VolumeId
        {
            get { return _newBackup.VolumeId;}
            set { _newBackup.VolumeId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "BackupVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _newBackup.Name; }
            set { _newBackup.Name = value; }
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
            NewVolumeBackup backup = new NewVolumeBackup();

            backup.VolumeId        = this.TranslateQuickPickNumber(_newBackup.VolumeId);

            this.RepositoryFactory.CreateVolumeRepository().SaveVolumeBackup(backup);
            this.UpdateCache<VolumesUIContainer>();
        }
        #endregion
    }
}





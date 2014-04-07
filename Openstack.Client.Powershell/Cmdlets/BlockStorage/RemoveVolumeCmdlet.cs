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
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.Domain.BlockStorage;

namespace Openstack.Client.Powershell.Cmdlets.BlockStorage
{
    [Cmdlet(VerbsCommon.Remove, "Volume", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveVolumeCmdlet : BasePSCmdlet
    {
        private string _volumeId;
        private SwitchParameter _force = false;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================  
        [Parameter(Position = 1, ParameterSetName = "RemoveVolume2", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(Position = 0, ParameterSetName = "RemoveVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "a")]
        [Alias("v")]
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
           string id = this.TranslateQuickPickNumber(this.VolumeId);

           if (_force == true && this.UserConfirmsDeleteAction("Volumes")) {
               this.RemoveAllVolumes();
            }
           else
           {
               this.RepositoryFactory.CreateVolumeRepository().DeleteVolume(id);
               this.UpdateCache<VolumesUIContainer>();
           }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void RemoveAllVolumes()
        {
            List<Volume> volumes = this.RepositoryFactory.CreateVolumeRepository().GetVolumes();
            Console.WriteLine("");

            foreach (Volume volume in volumes)
            {
               
                Console.WriteLine("Removing Volume : " + volume.Name);
                this.RepositoryFactory.CreateVolumeRepository().DeleteVolume(volume.Id);
               
            }
            Console.WriteLine("");
        }
        #endregion
    }
}




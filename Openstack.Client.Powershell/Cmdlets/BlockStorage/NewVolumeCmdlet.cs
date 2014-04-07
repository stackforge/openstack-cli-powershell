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

namespace Openstack.Client.Powershell.Cmdlets.BlockStorage
{ 
    [Cmdlet(VerbsCommon.New, "Volume", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.BlockStorage)]
    public class NewVolumeCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _size;
        private string _description;
        private string[] _metadata;
        private NewVolume vol = new NewVolume();
        private string _availabilityZone;
        private string _sourceVolid;

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 4, ParameterSetName = "NewVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("az")]
        [ValidateNotNullOrEmpty]
        public string AvailabilityZone
        {
            get { return _availabilityZone; }
            set { _availabilityZone = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, ParameterSetName = "NewVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("svid")]
        [ValidateNotNullOrEmpty]
        public string SourceVolumeId
        {
            get { return _sourceVolid; }
            set { _sourceVolid = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(Position = 1, ParameterSetName = "NewVolume", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(Position = 2, ParameterSetName = "NewVolume", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "NewVolume", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("md")]
        [ValidateNotNullOrEmpty]
        public string[] MetaData
        {
            get { return _metadata; }
            set { _metadata = value; }
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
            NewVolume volume        = new NewVolume();
            volume.Description      = this.Description;
            volume.Name             = this.Name;
            volume.SourceVolid      = this.SourceVolumeId;
            volume.AvailabilityZone = this.AvailabilityZone;

            if (this.SourceVolumeId == null)
                volume.Size = this.Size;
            else
                volume.Size = null;


            if (_metadata != null && _metadata.Length > 0)
            {
                foreach (string kv in _metadata)
                {
                  char[] seperator                     = { '|' };
                  string[] temp                        = kv.Split(seperator);
                  volume.Metadata.Add(temp[0], temp[1]);                        
                }
            }

            this.RepositoryFactory.CreateVolumeRepository().SaveVolume(volume);
            this.UpdateCache<VolumesUIContainer>();
        }
        #endregion
    }
}



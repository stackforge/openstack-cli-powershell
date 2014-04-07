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
using Openstack.Client.Powershell.Providers.Security;

namespace Openstack.Client.Powershell.Cmdlets.BlockStorage
{
    [Cmdlet(VerbsCommon.Remove, "Snapshot", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveSnapshotCmdlet : BasePSCmdlet
    {
        private string _snapshotId;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "RemoveSnapshot", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "a")]
        [Alias("s")]
        public string SnapshotId
        {
            get { return _snapshotId; }
            set { _snapshotId = value; }
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
            if (this.UserConfirmsDeleteAction("Snapshots"))
            {
                this.RepositoryFactory.CreateSnapshotRepository().DeleteSnapshot(this.SnapshotId);
                this.UpdateCache<SnapshotsUIContainer>();
            }
        }
        #endregion
    }
}




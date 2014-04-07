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
using System.Management.Automation;
using Openstack.Objects.DataAccess;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain;
using System.IO;
using Openstack.Client.Powershell.Providers.Storage;
using Openstack.Common;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Storage;
using System.Collections.Generic;

namespace Openstack.Client.Powershell.Cmdlets.Containers
{
    [Cmdlet(VerbsCommon.Set, "Scope", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class SetScopeCmdlet : BasePSCmdlet
    {
        private string _containerName;
        private ContainerScope _scope;
        private string _permission;
        private string[] _users;

        #region Parameters
//=========================================================================================
/// <summary>
/// The Container name.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "scp1", Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The Container name.")]
        [Alias("cn")]
        [ValidateNotNullOrEmpty]
        public string ContainerName
        {
            get { return _containerName; } 
            set { _containerName = value; }
        }
//=========================================================================================
/// <summary>
/// The Container name.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "scp2", Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = @"The Container name.")]
        [Alias("c")]
        [ValidateNotNullOrEmpty]       
        public string Name
        {
            get { return _containerName; }
            set { _containerName = value; }
        }
//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "scp2", Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Containers Scope.")]
        [ValidateSet("Public", "Private")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public ContainerScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
//=========================================================================================
/// <summary>
///
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "scp1", Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "c")]
        [Alias("u")]
        [ValidateNotNullOrEmpty]
        public string[] Users
        {
            get { return _users; }
            set { _users = value; }
        }
//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "scp1", Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "v")]
        [ValidateSet("Read", "Write", "ReadWrite")]
        [Alias("p")]
        [ValidateNotNullOrEmpty]
        public string Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }
        #endregion 
        #region Methods
//========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="permission"></param>
/// <returns></returns>
//========================================================================================
        private ContainerPermissions GetPermission(string permission)
        {
            switch (permission)
            {
                case ("Read") :
                    return ContainerPermissions.PublicRead;

                case ("ReadWrite"):
                    return ContainerPermissions.PublicReadWrite;

                case ("Write"):
                    return ContainerPermissions.PublicWrite;

                default :
                    return ContainerPermissions.PublicRead;
            }
        }
//========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="users"></param>
/// <returns></returns>
//========================================================================================
        private List<string> GetUsers(string[] users)
        {
            List<string> userList = new List<string>();
            foreach (string user in users) {
                userList.Add("*:" + user);
            }
            return userList;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowUrl()
        {
            StoragePath path = this.CreateStoragePath(String.Empty); 
            path.Volume      = _containerName;
            string uri       = path.AbsoluteURI.Remove(path.AbsoluteURI.LastIndexOf('/'));

            if (this.Settings.PasteGetURIResultsToClipboard)
                OutClipboard.SetText(uri);

            WriteObject("");
            WriteObject("Shared Container located at : " + uri);
            WriteObject(""); 
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            IContainerRepository repository = this.RepositoryFactory.CreateContainerRepository();

            if (this.ParameterSetName == "scp2")
            {                
                repository.SetScope(_containerName, _scope);
            }
            else
            {
                ContainerACL acl = new ContainerACL();
                acl.Permission   = this.GetPermission(_permission);
                acl.Users        = this.GetUsers(_users);
                repository.SetScope(_containerName, acl);
                this.ShowUrl();
            }
        }
        #endregion
    }
}

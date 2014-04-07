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
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain;
using System.IO;
using Openstack.Client.Powershell.Providers.Storage;
using System.Diagnostics;
using Openstack.Common;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Linq;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;

namespace Openstack.Client.Powershell.Cmdlets.Storage.CDN
{
    [Cmdlet(VerbsCommon.Remove, "CDN", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.CDN)]
    public class RemoveCDNCmdlet : BasePSCmdlet
    {
        private string _containerName;
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Mandatory = true, ParameterSetName = "EnableCDN", ValueFromPipelineByPropertyName = true, HelpMessage = "The Name of the Container to enable for CDN access.")]
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        public string ContainerName
        {
            get { return _containerName; }
            set { _containerName = value; }
        }
//=========================================================================================
/// <summary>
/// The main driver..
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            this.RepositoryFactory.CreateCDNRepository().DeleteContainer(this.ContainerName);
        }
    }
}


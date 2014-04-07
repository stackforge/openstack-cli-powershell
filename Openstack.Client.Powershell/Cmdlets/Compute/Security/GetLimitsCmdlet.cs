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
using Openstack.Objects.Domain.Networking;
using System;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.DataAccess.Security;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.DataAccess.Compute;


namespace Openstack.Client.Powershell.Cmdlets.Compute.Security
{
    [Cmdlet("Get", "Limits", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class GetLimitsCmdlet : BasePSCmdlet
    {
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            Limits limits = this.RepositoryFactory.CreateServerRepository().GetLimits();
            Console.WriteLine("");
            Console.WriteLine("The following Limits are in place for this account.");
            Console.WriteLine("");
            Console.WriteLine("Limit Category : Absolute");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Max Server Metadata Elements : " + limits.limits.AbsoluteLimits.maxServerMeta);
            Console.WriteLine("Max Personality              : " + limits.limits.AbsoluteLimits.maxPersonality);
            Console.WriteLine("Max Image Metadata Elements  : " + limits.limits.AbsoluteLimits.maxImageMeta);
            Console.WriteLine("Max Personality Size         : " + limits.limits.AbsoluteLimits.maxPersonalitySize);
            Console.WriteLine("Max Security Group Rules     : " + limits.limits.AbsoluteLimits.maxSecurityGroupRules);
            Console.WriteLine("Max Security Groups          : " + limits.limits.AbsoluteLimits.maxSecurityGroups);
            Console.WriteLine("Max Total Instances          : " + limits.limits.AbsoluteLimits.maxTotalInstances);
            Console.WriteLine("Max Total RAM Size           : " + limits.limits.AbsoluteLimits.maxTotalRAMSize);
            Console.WriteLine("");
        }
        #endregion
    }
}


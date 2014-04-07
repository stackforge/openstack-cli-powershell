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
using System.Linq;
using System.Text;
using System.Collections;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.Domain;
using Openstack.Objects.Utility;
using Openstack.Objects.DataAccess;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Providers.Security
{
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class SecurityUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public SecurityUIContainer() { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            this.LoadContainers();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private void  LoadContainers()
        {            
            this.Containers.Clear();
            this.Containers.Add(this.CreateContainer<SecurityGroupsUIContainer>("SecurityGroups", "Container objects used for security group rules", @"Security\SecurityGroups"));
            this.Containers.Add(this.CreateContainer<KeyPairsUIContainer>("KeyPairs", "A place to manage public/private keys used to create secure connections to compute instances.", @"Security\KeyPairs"));
        }       
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public SecurityUIContainer(BaseUIContainer parentContainer, string name, string description, string path, Context context, BaseRepositoryFactory repository)
            : base(parentContainer, name, description, path)
        {
            this.Context           = context;
            this.RepositoryFactory = repository;
            
            this.LoadContainers();
            this.ObjectType = Client.Powershell.Providers.Common.ObjectType.Entity;
        }
    }
}

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

namespace Openstack.Client.Powershell.Providers.Database
{
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class DatabaseUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public DatabaseUIContainer() { }
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
            this.Containers.Add(this.CreateContainer<VolumesUIContainer>("Instances", "Manage Database Instances across the Account..", @"Data\Instances"));
            this.AddContainer(this.CreateContainer<FlavorsUIContainer>("Flavors", "Manage additional hardware configurations for Database Servers.", @"Data\Flavors"));
            this.Containers.Add(this.CreateContainer<SnapshotsUIContainer>("Snapshots", "Container for storing and managing Database Instance Snapshots. ", @"Data\Snapshots"));
            this.Containers.Add(this.CreateContainer<SnapshotsUIContainer>("SecurityGroups", "Container for storing and managing Database Instance Security Groups. ", @"Data\SecurityGroups"));
        }       
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public DatabaseUIContainer(BaseUIContainer parentContainer, string name, string description, string path, Context context, BaseRepositoryFactory repository)
            : base(parentContainer, name, description, path)
        {
            this.Context           = context;
            this.RepositoryFactory = repository;
            
            this.LoadContainers();
            this.ObjectType = Client.Powershell.Providers.Common.ObjectType.Entity;
        }
    }
}


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
using Openstack.Client.Powershell.Providers.Database;
using Openstack.Client.Powershell.Providers.Networking;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Administration.Domain
{
    public static class CurrentAccountUIContainer
    {
        public static AccountUIContainer CurrentAccount = null;
    }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
    public class AccountUIContainer : BaseUIContainer
    {
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
          
            this.AddContainer(this.CreateContainer<ServersUIContainer>      ("Servers", "A place to manage Servers across the Account.", "Servers"));
            this.AddContainer(this.CreateContainer<FlavorsUIContainer>      ("Flavors", "Manage additional hardware configurations for Servers.", "Flavors"));
            this.AddContainer(this.CreateContainer<ImagesUIContainer>       ("Images", "Manage collections of files used to create or rebuild Servers.", "Images"));
            this.AddContainer(this.CreateContainer<SecurityUIContainer>     ("Security", "Manage Key Pair and Security Group Rules here.", "Security"));
            //this.AddContainer(this.CreateContainer<FloatingIPUIContainer>   ("Networking", "A place to manage  dynamic IP addresses assigned to your Servers.", "Networking"));
            this.AddContainer(this.CreateContainer<BlockStorageUIContainer> ("BlockStorage", "A place to manage Block Storage Volumes and Snapshots.", "BlockStorage"));
            this.AddContainer(this.CreateContainer<NetworksUIContainer>     ("Networks", "Manage networks, subnets, ports and routers.", "Networks"));
           // this.AddContainer(this.CreateContainer<DatabaseUIContainer>    ("Data", "Database Services", "Data"));
        }       
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public AccountUIContainer(BaseUIContainer parentContainer, string name, string description, string path, Context context, BaseRepositoryFactory repository)
            : base(parentContainer, name, description, path)
        {
            this.Context           = context;
            this.RepositoryFactory = repository;
            
            this.LoadContainers();
            this.ObjectType = Client.Powershell.Providers.Common.ObjectType.Entity;
        }
    }
}

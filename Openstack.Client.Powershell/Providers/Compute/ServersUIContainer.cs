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
using System.Collections.Generic;
using Openstack.Administration.Domain;
using System.Collections;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain;
using System;
using System.Collections.ObjectModel;
using Openstack.Objects;
using System.Xml.Serialization;
using System.IO;

namespace Openstack.Client.Powershell.Providers.Compute
{
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class ServersUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public ServersUIContainer() {  }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public ServersUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path)
                              : base(parentContainer, name, description, path)
        {
            this.ObjectType =  Common.ObjectType.Container;
        }

        #region Methods
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            this.LoadEntities();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
//=========================================================================================================
        private void LoadEntities() 
        {           
           IList servers = this.RepositoryFactory.CreateServerRepository().GetServers(null, null, null, null, ServerStatus.Unknown);
           if (servers != null && servers.Count > 0)
           {
               this.SetUIContainers<ServerUIContainer>(servers);
               this.Entities = servers;
           }
           else
           {
               this.Containers.Clear();
               this.Entities.Clear();
           }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//=========================================================================================================
        public override BaseUIContainer CreateContainer(string id)
        {
            return null;
        }
        #endregion
    }
}

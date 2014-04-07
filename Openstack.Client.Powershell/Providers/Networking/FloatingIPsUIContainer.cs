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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Networking;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Providers.Networking
{
    public class FloatingIPsUIContainer : BaseUIContainer
    {
        private bool _hasLoaded = false;

//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public FloatingIPsUIContainer() {  }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public FloatingIPsUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path)
                              : base(parentContainer, name, description, path)
        {
            this.ObjectType =  Common.ObjectType.Container;
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            if (!_hasLoaded)
            {
                this.LoadEntities();
            }
        }
//=================================================================================================
/// <summary>
/// 
/// </summary>
//=================================================================================================
        public void Load(List<FloatingIP> floatingIPs)
        {
            this.Containers.Clear();
            this.SetUIContainers<FloatingIPUIContainer>(floatingIPs);
            this.Entities = floatingIPs;
            _hasLoaded = true;
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
            List<FloatingIP> floatingIPs = this.RepositoryFactory.CreateFloatingIPRepository().GetFloatingIPs();

            if (floatingIPs != null && floatingIPs.Count > 0)
            {
                this.SetUIContainers<FloatingIPUIContainer>(floatingIPs);
                this.Entities = floatingIPs;
            }
            else
            {
                this.Containers.Clear();
                this.Entities.Clear();
            }
        }
    }
}

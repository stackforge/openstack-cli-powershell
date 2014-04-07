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
using Openstack.Objects.DataAccess.Networking;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Providers.Networking
{
    public class PortsUIContainer : BaseUIContainer
    {
        private bool _hasLoaded = false;
//=================================================================================================
/// <summary>
/// 
/// </summary>
//=================================================================================================
        public override void Load()
        {
            if (!_hasLoaded)
            {
                this.Containers.Clear();
                IPortRepository repository = this.RepositoryFactory.CreatePortRepository();
                this.Entities = repository.GetPorts();
                this.SetUIContainers<PortUIContainer>(this.Entities);
            }
        }
//=================================================================================================
/// <summary>
/// 
/// </summary>
//=================================================================================================
        public void Load(List<Port> ports)
        {
            this.Containers.Clear();
            this.SetUIContainers<PortUIContainer>(ports);
            this.Entities = ports;
            _hasLoaded = true;
        }
    }
}

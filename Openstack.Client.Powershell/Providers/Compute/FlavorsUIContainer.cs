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
using Openstack.Client.Powershell.Providers.Common;
using System.Collections;
using System.Collections.Generic;
using Openstack.Objects.Domain.Compute;

namespace Openstack.Client.Powershell.Providers.Compute
{
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class FlavorsUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================       
        public FlavorsUIContainer(BaseUIContainer parentContainer,
                                 string name,
                                 string description,
                                 string path)
                                 : base(parentContainer, name, description, path)
        {
            this.ObjectType = ObjectType.Container;
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public FlavorsUIContainer()
        {}
        #region Methods       
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
//=========================================================================================================
        public override void  Load()
          {             
              List<Flavor> flavors = this.RepositoryFactory.CreateFlavorRepository().GetFlavors(null, null);
              flavors.Sort();
              this.SetUIContainers<FlavorUIContainer>(flavors);
              this.Entities = flavors;           
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

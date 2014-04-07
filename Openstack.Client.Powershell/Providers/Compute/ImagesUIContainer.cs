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

namespace Openstack.Client.Powershell.Providers.Compute
{
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class ImagesUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================       
        public ImagesUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path)
                              : base(parentContainer, name, description, path)
        {
            this.ObjectType = ObjectType.Container;
        }
        public ImagesUIContainer()
        {}
        #region Methods
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
//=========================================================================================================
        public override void Load()
        {         
           IList images = this.RepositoryFactory.CreateImageRepository().GetImages(null, null, null, null, null);
           this.SetUIContainers<ImageUIContainer>(images);
           this.Entities = images;
        }
        #endregion
    }
}

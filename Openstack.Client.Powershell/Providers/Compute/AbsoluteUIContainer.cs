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
    public class AbsoluteLimitsUIContainer : BaseUIContainer
    {
        private List<BaseEntity> _absoluteLimits;

//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public AbsoluteLimitsUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path,
                              List<BaseEntity> absoluteLimits)
                              : base(parentContainer, name, description, path)
        {
            _absoluteLimits = absoluteLimits;
            this.ObjectType = ObjectType.Container;
        }

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
        #region Properties
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        //public List<AbsoluteLimit> AbsoluteLimits
        //{
        //    get { return _absoluteLimits; }
        //    set { _absoluteLimits = value; }
        //}
        #endregion
    }
}

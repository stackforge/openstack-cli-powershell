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
using Openstack.Objects.Domain.Compute;

namespace Openstack.Client.Powershell.Providers.Compute
{
    public class MetaDataUIContainer : BaseUIContainer 
    {
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="elements"></param>
//==================================================================================================
        public void Load(System.Collections.Generic.Dictionary<string, string> elements)
        {
            List<MetaDataElement> metadataElements = new List<MetaDataElement>();
           
            foreach (KeyValuePair<string, string> element in elements) {
                if (element.Key != String.Empty)
                    metadataElements.Add(new MetaDataElement(element.Key, element.Value));
            }
            this.Entities = metadataElements;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public override void Load()
        {
           
        }
        public override void WriteEntityDetails()
        {
            
            base.WriteEntityDetails();
        }
    }
}

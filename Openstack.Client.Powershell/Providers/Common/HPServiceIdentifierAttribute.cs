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
using Openstack.Objects.Domain.Admin;

namespace Openstack.Client.Powershell.Providers.Common
{
     [System.AttributeUsage(System.AttributeTargets.Class )
]    public class RequiredServiceIdentifierAttribute : System.Attribute
    {
        private string _serviceName;
        private Services _services;

        public Services Services
        {
            get { return _services; }
            set { _services = value; }
        }

        public RequiredServiceIdentifierAttribute(Services service)
        {
            _services = service;
        }

        public RequiredServiceIdentifierAttribute(string serviceName)
        {
            _serviceName = serviceName;
        }

        public string ServiceName
        {
            get
            {
                return _serviceName;
            }
        }
    }
}

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
using System.Xml.Serialization;

namespace Openstack.Client.Powershell.Utility
{

    public enum StorageOperationType
    {
        IntraServer = 0,
        ServerToLocal = 1,
        LocalToServer = 2
    }

    public enum PathType
    {
        Local = 1,   // Local is implicitly always fully qualified..
        Remote = 2,
        UnknownPathType = 3
    }

    public enum CannedPermissionTypes
    {
        [XmlEnum(Name = "private")]
        Private,
        [XmlEnum(Name = "public-read")]
        PublicRead,
        [XmlEnum(Name = "public-read-write")]
        PublicReadWrite,
        [XmlEnum(Name = "authenticated-read")]
        AuthenticatedRead,
        [XmlEnum(Name = "authenticated-read-write")]
        AuthenticatedReadWrite,
        [XmlEnum(Name = "storageContainer-owner-read")]
        storageContainerOwnerRead,
        [XmlEnum(Name = "storageContainer-owner-full-control")]
        storageContainerOwnerFullcontrol,
        Unspecified
    }

    public enum ContainerScope
    {
        Public,
        Private

    }
}



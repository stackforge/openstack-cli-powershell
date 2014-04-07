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
using Openstack.Objects.Domain.BlockStorage;

namespace Openstack.Client.Powershell.Providers.Security
{
    public class SnapshotUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
           // this.Entities = ((Image)this.Entity).MetaData;
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Snapshot snapshot = (Snapshot)this.Entity;

            this.WriteHeader("Snapshot Details");
            this.WriteSnapshotDetails(snapshot);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteVolumeMetadata(Volume volume)
        {
            //this.WriteMetadata(volume.MetaData);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteSnapshotDetails(Snapshot snapshot)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("Id              : " + snapshot.Id);
            Console.WriteLine("Name            : " + snapshot.DisplayName);
            Console.WriteLine("Description     : " + snapshot.Description);
            Console.WriteLine("Created On      : " + snapshot.CreationDate);
            Console.WriteLine("Status          : " + snapshot.Status);
            Console.WriteLine("Volume Id       : " + snapshot.VolumeId);
            Console.WriteLine("");  
        }
    }
}


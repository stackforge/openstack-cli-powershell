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
using Openstack.Client.Powershell.Providers.BlockStorage;

namespace Openstack.Client.Powershell.Providers.Security
{
    public class VolumeBackupUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private void LoadContainers()
        {}
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {}
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            VolumeBackup backup = (VolumeBackup)this.Entity;

            this.WriteHeader("Volume Backup Details");
            this.WriteVolumeBackupDetails(backup);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteVolumeBackupDetails(VolumeBackup backup)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("Id                 : " + backup.Id);
            Console.WriteLine("Name               : " + backup.Name);
            Console.WriteLine("Description        : " + backup.Description);
            Console.WriteLine("Size               : " + backup.Size);
            Console.WriteLine("Availability Zone  : " + backup.AvailabilityZone);
            Console.WriteLine("Container          : " + backup.Container);
            Console.WriteLine("Object Count       : " + backup.ObjectCount);
            Console.WriteLine("Created On         : " + backup.CreatedDate);
            Console.WriteLine("Status             : " + backup.Status);
            Console.WriteLine("Volume Id          : " + backup.VolumeId);       
            Console.WriteLine("");        
        }
    }
}

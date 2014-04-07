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
    public class VolumeUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            //this.LoadContainers();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private void LoadContainers()
        {
            this.Containers.Clear();
            AttachmentsUIContainer sgContainer = (AttachmentsUIContainer)this.CreateContainer<AttachmentsUIContainer>("Attachments", "Manage this Volumes attachments.", this.Parent.Path + @"\Attachments");
            this.Containers.Add(sgContainer);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Volume volume = (Volume)this.Entity;

            this.WriteHeader("Volume Details");
            this.WriteVolumeDetails(volume);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteVolumeMetadata(Volume volume)
        {
            int maxLength           = volume.Metadata.Max(m => m.Key.Length);
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            foreach (KeyValuePair<string, string> element in volume.Metadata)
            {
                Console.WriteLine(element.Key.PadRight(maxLength) + " : " + element.Value.Trim());
            }
            Console.WriteLine();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteVolumeDetails(Volume volume)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("Id                 : " + volume.Id);
            Console.WriteLine("Name               : " + volume.Name);
            Console.WriteLine("Description        : " + volume.Description);
            Console.WriteLine("Size               : " + volume.Size);
            Console.WriteLine("Created On         : " + volume.CreationDate);
            Console.WriteLine("Status             : " + volume.Status);
            Console.WriteLine("Server Attached To : " + volume.AttachedTo);
            Console.WriteLine("Device             : " + volume.Device);
            Console.WriteLine("");           

            if (volume.Metadata.Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Volume Metadata");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("");
                WriteVolumeMetadata(volume);
            }
        }
    }
}

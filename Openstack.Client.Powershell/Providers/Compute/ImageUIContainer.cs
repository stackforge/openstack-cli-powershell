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
    public class ImageUIContainer : BaseUIContainer
    {
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load(){ }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Image image = (Image)this.Entity;

            this.WriteHeader("Image Details");
            this.WriteImageDetails(image);
            this.WriteHeader("Image meta-data is as follows.", ConsoleColor.White);
            this.WriteImageMetadata(image);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteImageMetadata(Image image)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            this.WriteMetadata(image.MetaData);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteImageDetails(Image image)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("Id              : " + image.Id);
            Console.WriteLine("Name            : " + image.Name);
            Console.WriteLine("Created On      : " + image.CreatedDate);
            Console.WriteLine("Last Updated On : " + image.LastModified);
            Console.WriteLine("Status          : " + image.Status);
            Console.WriteLine("Progress        : " + image.Progress);
        }
    }
}

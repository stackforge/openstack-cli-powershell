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
using System.Linq;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using System.Collections.Generic;
using Openstack.Client.Powershell.Providers.BlockStorage;
using Openstack.Objects.Domain.Security;
using Openstack.Objects.Domain.Compute.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.DataAccess.Security;

namespace Openstack.Client.Powershell.Providers.Compute
{
    public class ServerUIContainer : BaseUIContainer
    {
        private Image _image;
        private Flavor _flavor;
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public ServerUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
            : base(parentContainer, name, description, path)
        {
            this.LoadContainers();
            this.ObjectType = Common.ObjectType.Entity;            
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public ServerUIContainer() { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {            
            this.LoadServerDetails();
            this.LoadContainers();
            this.LoadFlavorDetails();
            this.LoadImageDetails();            
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadServerDetails()
        {
            Server server = (Server)this.Entity;
            this.Entity   = this.RepositoryFactory.CreateServerRepository().GetServer(server.Id);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadFlavorDetails()
        {
            Server temp     = ((Server)this.Entity);
            string flavorId = ((EntityRef)temp.Flavor).Id;
            _flavor         = this.RepositoryFactory.CreateFlavorRepository().GetFlavor(flavorId);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void LoadImageDetails()
        {
            Server temp    = ((Server)this.Entity);
            string imageId = ((EntityRef)temp.Image).Id;
            _image         = this.RepositoryFactory.CreateImageRepository().GetImage(imageId);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private List<SecurityGroup> GetResolvedSecurityGroups(List<EntityRef> groups)
        {
            List<SecurityGroup> secGroups = this.RepositoryFactory.CreateSecurityRepository().GetSecurityGroups();
            try
            {
                var innerJoinQuery =
                from agroup in groups
                join secGroup in secGroups on agroup.Name equals secGroup.Name
                select new SecurityGroup()
                {
                    Id = secGroup.Id,
                    Description = secGroup.Description,
                    Name = secGroup.Name,
                    TenantId = secGroup.TenantId,
                    Rules = secGroup.Rules
                };
                return innerJoinQuery.ToList<SecurityGroup>();
            }
            catch (Exception) { }
            return null;    
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
            BaseUIContainer container = (BaseUIContainer)this.CreateContainer<MetaDataUIContainer>("Metadata", "Metadata about the Server.", this.Parent.Path + @"\Metadata");
            if (this.Entity != null)
            {
                ((MetaDataUIContainer)container).Load(((Server)this.Entity).MetaData);
            }

            this.Containers.Add(container);

            SecurityGroupsUIContainer sgContainer = (SecurityGroupsUIContainer)this.CreateContainer<SecurityGroupsUIContainer>("SecurityGroups", "Manage this Servers security rules.", this.Parent.Path + @"\SecurityGroups");
            sgContainer.IsServerGroups = true;
            sgContainer.LoadEntities(this.GetResolvedSecurityGroups(((Server)this.Entity).SecurityGroups));
            this.Containers.Add(sgContainer);

            try
            {
                AttachmentsUIContainer aContainer = (AttachmentsUIContainer)this.CreateContainer<AttachmentsUIContainer>("Volumes", "List the Volumes attached to this Server", this.Parent.Path + @"\Volumes");
                aContainer.LoadEntities(((Server)this.Entity));
                //if (aContainer != null && aContainer.Entities != null)
                if (aContainer != null)
                    this.Containers.Add(aContainer);
            }
            catch (Exception ex) { int h = 8; }

            ServerLogUIContainer logContainer = (ServerLogUIContainer)this.CreateContainer<ServerLogUIContainer>("EventLog", "EventLog", this.Parent.Path + @"\Log");
            this.Containers.Add(logContainer);
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void WriteNetworkDetails()
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine();
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("   Address : " + ((Server)this.Entity).IpAddress);
            Console.WriteLine();

            //IList<IPAddress> addresses = ((Server)this.Entity).Addresses.Private;
            //if (addresses != null)
            //{
            //    foreach (IPAddress address in addresses)
            //    {
            //        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            //        Console.WriteLine();
            //        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            //        Console.WriteLine("   Address : " + address.Addr);
            //        Console.WriteLine("   Version : " + address.Version);
            //        Console.WriteLine();
            //    }
            //}
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        private void WriteServerDetails(Server server)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id          : " + server.Id);
            Console.WriteLine("  Name        : " + server.Name);
            Console.WriteLine("  UUID        : " + server.UUID);
            Console.WriteLine("  Host Id     : " + server.HostId);
            Console.WriteLine("  Status      : " + server.Status);
            Console.WriteLine("  Progress    : " + server.Progress);
            Console.WriteLine("  Created On  : " + server.CreationDate);
            Console.WriteLine("  Last Update : " + server.LastUpdatedDate);
            Console.WriteLine("  Key         : " + server.KeyName);
            
            TimeSpan span = DateTime.Now - server.CreationDate;
            Console.WriteLine("  Age         : " + Convert.ToString(span.Days) + " day(s)");
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        private void WriteImageDetails(Image image)
        {
            if (image != null)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                Console.WriteLine("  Id            : " + image.Id);
                Console.WriteLine("  Name          : " + image.Name);
                Console.WriteLine("  Last Modified : " + image.LastModified);
                Console.WriteLine("  Created On    : " + image.CreatedDate);
                Console.WriteLine("  Status        : " + image.Status);
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="flavor"></param>
//=========================================================================================================
        private void WriteFlavorDetails(Flavor flavor)
        {
            if (flavor != null)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                Console.WriteLine("  Id     : " + flavor.Id);
                Console.WriteLine("  Name   : " + flavor.Name);
                Console.WriteLine("  RAM    : " + flavor.Ram);
                Console.WriteLine("  Disk   : " + flavor.Disk);
                Console.WriteLine("  Status : " + flavor.Status);
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            Server server = (Server)this.Entity;

            this.WriteHeader("Server Details");
            this.WriteServerDetails(server);
            this.WriteHeader("Image Information.", ConsoleColor.White);
            this.WriteImageDetails(_image);
            this.WriteHeader("Flavor Details", ConsoleColor.White);
            this.WriteFlavorDetails(_flavor);
            this.WriteHeader("Assigned Network Addresses", ConsoleColor.White);
            this.WriteNetworkDetails();           
            Console.WriteLine();            
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//=========================================================================================================
        public override bool IsEntityId(string id)
        {
            if (id == ((Server)this.Entity).Id)
                return true;
            else
                return false;
        }
    }
}

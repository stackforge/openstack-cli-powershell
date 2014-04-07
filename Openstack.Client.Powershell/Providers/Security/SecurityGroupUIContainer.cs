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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain.Security;

namespace Openstack.Client.Powershell.Providers.Security
{
    public class SecurityGroupUIContainer : BaseUIContainer
    {        
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public SecurityGroupUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
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
        public SecurityGroupUIContainer() { }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {            
            this.Entity = this.RepositoryFactory.CreateSecurityRepository().GetSecurityGroup(this.Id);
        }
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
        private void WriteGroupDetails(SecurityGroup group)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("  Id          : " + group.Id);
            Console.WriteLine("  Name        : " + group.Name);
            Console.WriteLine("  Tenant Id   : " + group.TenantId);
            Console.WriteLine("  Description : " + group.Description);           
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="group"></param>
//=========================================================================================================
        private void WriteSecurityRules(SecurityGroup group)
        {
            foreach (SecurityGroupRule rule in ((SecurityGroup)this.Entity).Rules)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                Console.WriteLine("");
                Console.WriteLine("  Rule Id          : " + rule.Id);
                Console.WriteLine("  IP Protocol      : " + rule.Protocol);
                Console.WriteLine("  Direction        : " + rule.Direction);
                Console.WriteLine("  Range Min        : " + rule.PortRangeMin);
                Console.WriteLine("  Range Max        : " + rule.PortRangeMax);
                Console.WriteLine("  Ether Type       : " + rule.EtherType);
                Console.WriteLine("  Security Group   : " + rule.SecurityGroupId);
                Console.WriteLine("  Remote Group     : " + rule.RemoteGroupId);
                Console.WriteLine("  Remote IP Prefix : " + rule.RemoteIPPrefix);
                Console.WriteLine("  Tenant Id        : " + rule.TenantId);
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
            SecurityGroup group = (SecurityGroup)this.Entity;

            this.WriteHeader("Security Group Details");
            this.WriteGroupDetails(group);
            this.WriteHeader("Security Rules", ConsoleColor.White);

            this.WriteSecurityRules(group);
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

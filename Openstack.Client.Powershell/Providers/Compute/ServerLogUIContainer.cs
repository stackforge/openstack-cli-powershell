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
using Openstack.Objects.Domain.Compute.Servers;
using Openstack.Objects.Domain.Compute.Servers.Actions;

namespace Openstack.Client.Powershell.Providers.Compute
{
    public class ServerLogUIContainer : BaseUIContainer
    {
        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void Load()
        {
            GetServerLogAction action = new GetServerLogAction();
            action.ServerId           = this.Parent.Entity.Id;
            this.Entity               = (Log)this.RepositoryFactory.CreateServerRepository().GetServerLog(action);
            
            this.WriteHeader("Log Entries");

            Log log                                 = (Log)this.Entity;
            System.Text.RegularExpressions.Regex ex = new System.Text.RegularExpressions.Regex(@"\\n");
            string[] buffer                         = ex.Split(log.Content);

            Console.WriteLine();
            foreach (string line in buffer)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void WriteEntityDetails()
        {
        
            this.WriteHeader("Log Entries");
            Console.Write(((Log)this.Entity).Content);
        }
    }
}

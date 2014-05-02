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
using System.Threading.Tasks;
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Utility
{
    public abstract class RegistrationManager
    {
       // public abstract ServiceProviderInfo CreateCredential(Dictionary<string, string> configValues);
        public abstract RegistrationResponse Register(ServiceProvider serviceProvider);
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="elements"></param>
//==================================================================================================
        protected void ValidateCredentialElements (ref ServiceProvider serviceProvider)
        {
            // If there are any CredentialElements with empty values, get them now from the User..
            
            List<CredentialElement> elements = serviceProvider.CredentialElements;
            if (elements.Where(e => e.Value == String.Empty).Any())
            {
                this.WriteGetCredentialsHeader();

                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].IsMandatory && elements[i].Value == String.Empty)  {
                        elements[i].Value = this.PromptForCredentialElement(elements[i]);                          
                    }
                }                               
            }

            // Also check that the ServiceProvider name was supplied..

            if (serviceProvider.Name == String.Empty)
            {
                Console.WriteLine("");
                Console.WriteLine("Service Provider Name :");
                Console.WriteLine("");
                serviceProvider.Name  = Console.ReadLine();
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private string PromptForCredentialElement(CredentialElement element)
        {
            Console.WriteLine("");
            Console.WriteLine(element.DisplayName + " :");            
            Console.WriteLine("");
            return Console.ReadLine();
        }
//==================================================================================================
/// <summary>
/// Writes out header information to be used during the process that prompts for new credentials.
/// </summary>
//==================================================================================================
        private void WriteGetCredentialsHeader()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("We've noticed that you haven't supplied any credentials yet. To continue we need to get your Username, ");
            Console.WriteLine("Password, and the Tenant Id provided to you during the sign up process. If you haven't signed up");
            Console.WriteLine("for any services yet, just go to https://console.OpenStack.com for details on how to get started today!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ");
        }
    }
}

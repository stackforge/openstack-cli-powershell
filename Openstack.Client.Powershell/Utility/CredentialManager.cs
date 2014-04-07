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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Security;
using Openstack.Identity;
using Openstack.Client.Powershell.Utility;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Openstack.Client.Powershell.Utility
{   

    public class ServiceProviderInfo
    {
        public string Name;
        public IOpenstackCredential Credential;
        public string ServbiceProviderUrl;
    }

    public class CredentialManager
    {
        private bool _supressPrompt = false;

//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public CredentialManager(bool supressPrompt = false)
        {
            _supressPrompt = supressPrompt;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=======================================================================================================
        private Boolean DoesRequireCredentials(string accountIdNode, string accessKeyNode, string identityServiceURI, string tenantId)
        {
            if (accountIdNode == String.Empty || accessKeyNode == String.Empty || identityServiceURI == String.Empty || tenantId == String.Empty)
                return true;
            else return false;




            //// First make sure the User info exist..

            //System.Collections.Generic.IEnumerable<XAttribute> attributes = accountIdNode.Attributes();
            //foreach (XAttribute attribute in attributes)
            //{
            //    if (attribute.Name == "value" && attribute.Value == string.Empty)
            //    {
            //        return true;
            //    }
            //}

            //// Now check to ensure the auth key exist

            //System.Collections.Generic.IEnumerable<XAttribute> attributes2 = accessKeyNode.Attributes();
            //foreach (XAttribute attribute in attributes2)
            //{
            //    if (attribute.Name == "value" && attribute.Value == string.Empty)
            //    {
            //        return true;
            //    }
            //}

            //return false;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="accessKey"></param>
/// <param name="accountId"></param>
/// <returns></returns>
//==================================================================================================
        private bool VerifyCredentials(string accessKey, string accountId)
        {
            //RequestHeaders headers = BaseRepositoryFactory.GetSecurityHeaders(accountId, accessKey);

            //if (headers != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private static string ReadPassword()
        {
            string password     = "";
            ConsoleKeyInfo info = Console.ReadKey(true);

            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }

                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters

                        password = password.Substring(0, password.Length - 1);

                        // get the location of the cursor

                        int pos = Console.CursorLeft;

                        // move the cursor to the left by one character

                        Console.SetCursorPosition(pos - 1, Console.CursorTop);

                        // replace it with space

                        Console.Write(" ");

                        // move the cursor to the left by one character again

                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            // add a new line because user pressed enter at the end of their password

            Console.WriteLine();
            return password;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="secstrPassword"></param>
/// <returns></returns>
//==================================================================================================
        private string convertToUNSecureString(SecureString secstrPassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secstrPassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="value"></param>
/// <returns></returns>
//==================================================================================================
        private SecureString ConvertToSecureString(string value)
        {
            // Instantiate the secure string.
            SecureString securePwd = new SecureString();
            var secureStr = new SecureString();
            if (value.Length > 0)
                {
                    foreach (var c in value.ToCharArray()) secureStr.AppendChar(c);
                }
            else return null;
            return secureStr;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private ServiceProviderInfo PromptForCredentials()
        {
            ServiceProviderInfo info = new ServiceProviderInfo();

            Console.WriteLine("User Name  : ");
            string username = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Password   : ");
            string password = CredentialManager.ReadPassword();
            Console.WriteLine("");  
            Console.WriteLine("Tenant Name : ");
            string tenantName = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Service Provider Identity Service Url : ");
            string identityServiceUrl = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Finally supply a name for this Profile.");
            string name = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("");
            info.Credential =  new OpenstackCredential(new Uri(identityServiceUrl), username, this.ConvertToSecureString(password), tenantName);
            info.ServbiceProviderUrl = identityServiceUrl;
            info.Name = name;

            return info;
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
            Console.WriteLine("for any services yet, just go to https://console.Openstack.com for details on how to get started today!");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ");
        }
//==================================================================================================
/// <summary>
/// Writes out an error message when invalid credentials are supplied..
/// </summary>
//==================================================================================================
        private void WriteBadCredentialsMsg()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("The credentials that you have supplied could not be verified. Please try again. If you continue to have ");
            Console.WriteLine("problems please feel free to contact our support staff at https://console.Openstack.com");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ");
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public IOpenstackCredential GetCredentials(bool badCredentialsSupplied = false)
        {
            ServiceProviderInfo info;
            IOpenstackCredential credential;

            string configFilePath        = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";
            XDocument doc                = XDocument.Load(configFilePath);
            XElement serviceProviderNode = doc.Descendants("ServiceProvider").Where(a => a.Attribute("isDefault").Value == "True").FirstOrDefault();
            XElement username            = serviceProviderNode.Elements().Where(u => u.Name == "add" && u.Attribute("key").Value == "Username").Single();        
            XElement password            = serviceProviderNode.Elements().Where(u => u.Name == "add" && u.Attribute("key").Value == "Password").Single();
            XElement tenantId            = serviceProviderNode.Elements().Where(u => u.Name == "add" && u.Attribute("key").Value == "DefaultTenantId").Single();
            XElement identityServiceUrl  = serviceProviderNode.Elements().Where(u => u.Name == "add" && u.Attribute("key").Value == "AuthenticationServiceURI").Single(); 
            XAttribute name              = serviceProviderNode.Attribute("name");

            if (this.DoesRequireCredentials(username.Attribute("value").Value, password.Attribute("value").Value, identityServiceUrl.Attribute("value").Value, tenantId.Attribute("value").Value))
            {
                if (!badCredentialsSupplied){
                    this.WriteGetCredentialsHeader();
                }

                if (_supressPrompt == false) {
                     info = this.PromptForCredentials();
                     credential = info.Credential;
                }
                else
                {
                    // The test client would be an example of where we need to supress the prompt for credentials. Here we just raise an exception instead..

                    throw new SecurityException("You must supply a valid Access Key and Secret Key in the CLI.config file to start unit testing.");
                }

                while (!this.VerifyCredentials(username.Value, password.Value))
                {
                    Console.Clear();
                    this.WriteBadCredentialsMsg();
                    info = this.PromptForCredentials();
                    credential = info.Credential;
                }

                username.SetAttributeValue("value", credential.UserName);
                password.SetAttributeValue("value", this.convertToUNSecureString(credential.Password));
                tenantId.SetAttributeValue("value", credential.TenantId);
                identityServiceUrl.SetAttributeValue("value", credential.AuthenticationEndpoint.AbsoluteUri);
                name.SetValue(info.Name);
               
                Console.WriteLine("   ==> Verifying and Storing Credentials..");
                doc.Save(configFilePath);
                             
                return credential;
            }

            // We didn't need to prompt for credentials from the user so just send back what we found in the config file..

            SecureString securePassword = new SecureString();
            password.Attribute("value").Value.ToCharArray().ToList().ForEach(securePassword.AppendChar);
            string h = this.convertToUNSecureString(securePassword);
            IOpenstackCredential newCredential = new OpenstackCredential(new Uri(identityServiceUrl.Attribute("value").Value), username.Attribute("value").Value, securePassword, tenantId.Attribute("value").Value);

            return newCredential;
        }
    }
}

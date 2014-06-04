//* ============================================================================
//Copyright 2014 Hewlett Packard

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//============================================================================ */
using System;
using System.Management.Automation;
using System.Reflection;
using System.Security.Policy;
using OpenStack.Client.Powershell.Utility;
using System.Linq;
using OpenStack.Identity;
using System.Threading;
using Openstack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Utility
{
    public class ExtensionManager
    {
        private SessionState _session;
        private Context _context;

        #region Properties
        public Context Context
        {
            get { return _context; }
            set { _context = value; }
        }
        public SessionState Session
        {
            get { return _session; }
            set { _session = value; }
        }
        #endregion
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="session"></param>
/// <param name="context"></param>
//==================================================================================================
        public ExtensionManager(SessionState session, Context context)
        {
            _session = session;
            _context = context;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
/// <returns></returns>
//==================================================================================================
        private Settings GetSettings(ServiceProvider provider)
        {
            if (provider.ConfigFilePath == null)
            {
                Settings.Default.Reset();
                return Settings.Default;
            }
            else
            {
                return Settings.LoadConfig(provider.ConfigFilePath);
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="credential"></param>
/// <param name="context"></param>
/// <param name="client"></param>
//==================================================================================================
        private void SetSessionState(IOpenStackCredential credential, IOpenStackClient client, ServiceProvider provider)
        {
            // Setup the environment based on what came back from Auth..

            Context context = new Context();
            context.ServiceCatalog = credential.ServiceCatalog;
            context.Settings = this.GetSettings(provider);
            context.ProductName = "OpenStack-WinCLI";
            context.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            context.CurrentServiceProvider = provider;
            context.CurrentRegion = provider.AvailabilityZones.Where(z => z.IsDefault == true).Single().Name;

            this.Session.PSVariable.Set(new PSVariable("Context", context));
            this.Session.PSVariable.Set(new PSVariable("CoreClient", client));
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private RegistrationManager GetRegistrationManager(string serviceProviderName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Iterate through all Assemblies in Current AppDomain that contain a RegistrationManager class..
            
            foreach (Assembly assembly in assemblies) 
            {
                Type[] types = assembly.GetTypes().Where(b => b != null && b.BaseType != null && b.BaseType.UnderlyingSystemType.FullName == "OpenStack.Client.Powershell.Utility.RegistrationManager").ToArray<Type>();
                                
                // Now that we found one, make sure that it's the one we're looking for..
   
                foreach (Type type in types)   
                {                  
                  MemberInfo info = type;
                  foreach (object attribute in info.GetCustomAttributes(true))
                  {
                      ServiceProviderAttribute identifier = attribute as ServiceProviderAttribute;
                      
                      if (identifier != null && identifier.Name == serviceProviderName) {
                          return (RegistrationManager)Activator.CreateInstance(type, null);
                      }
                  }
               }  
            }
            return new OpenstackCoreRegistrationManager();
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        public void LoadCore(ServiceProvider provider)
        {
            //provider.Name = "Default";
            OpenstackCoreRegistrationManager manager = new OpenstackCoreRegistrationManager();
            RegistrationResponse response = manager.Register(provider);

            // Connect to the Service Provider..

            var client = OpenStackClientFactory.CreateClient<OpenStackClient>(response.Credentials, CancellationToken.None, String.Empty);
            var connectTask = client.Connect();
            connectTask.Wait();

            this.SetSessionState(response.Credentials, client, provider);

            ConfigurationManager configManager = new ConfigurationManager();
            configManager.WriteServiceProvider(response.Provider, true);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public void LoadExtension(ServiceProvider targetProvider)
        {
            string moduleName               = targetProvider.Name;
            AppDomain currentDomain         = AppDomain.CurrentDomain;
            Evidence asEvidence             = currentDomain.Evidence;
            IOpenStackClient client         = null;
            IOpenStackCredential credential = null;

            // Load up the Default Config file...

            ConfigurationManager configManager = new ConfigurationManager();
            configManager.Load();

            // Load the specified module into the Current AppDomain...

            this.Session.InvokeCommand.InvokeScript("Import-Module " + moduleName + " -DisableNameChecking");

            // Register the Module if it isn't already and grab the resulting credentials for Auth..

            RegistrationManager manager = this.GetRegistrationManager(moduleName);

            //if (manager.GetType().Name != "OpenstackCoreRegistrationManager")
            //{
                RegistrationResponse response = manager.Register(configManager.GetServiceProvider(moduleName));
                credential                    = response.Credentials;

                // Authenticate and create our Extension Client and stash it....

                client = OpenStackClientFactory.CreateClient<OpenStackClient>(credential);
                var connectTask = client.Connect();
                connectTask.Wait();

                // Save the credentials if there are any changes (The RegistrationManager may have prompted for missing Credentials)

                configManager.WriteServiceProvider(response.Provider);
            //}

            // Store Context and Client so that all PS-Providers and Cmdlets have access to it..

            this.SetSessionState(credential, client, targetProvider);
        }
    }
}

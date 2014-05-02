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
          if (provider.ConfigFilePath == null) {
              Settings.Default.Reset();
              return Settings.Default;
          }
          else {
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

          Context context        = new Context();
          context.ServiceCatalog = credential.ServiceCatalog;
          context.Settings       = this.GetSettings(provider);
          context.ProductName    = "OpenStack-WinCLI";
          context.Version        = Assembly.GetExecutingAssembly().GetName().Version.ToString();

          this.Session.PSVariable.Set(new PSVariable("Context", context));
          this.Session.PSVariable.Set(new PSVariable("CoreClient", client));         
      }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
      public void LoadCore(ServiceProvider provider)
      {
          OpenstackCoreRegistrationManager manager = new OpenstackCoreRegistrationManager();
          RegistrationResponse response            = manager.Register(provider);
          
          // Connect to the Service Provider..

          
          var client = OpenStackClientFactory.CreateClient<OpenStackClient>(response.Credentials, CancellationToken.None, null);
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
            string moduleName        = targetProvider.Name;
            AppDomain currentDomain  = AppDomain.CurrentDomain;
            Evidence asEvidence      = currentDomain.Evidence;
            
            ConfigurationManager configManager = new ConfigurationManager();
            configManager.Load();

            // Load the specified module. We take a before and after snapshot of assemblies loaded in the current AppDomain.
            // The targetList represents only what's been added...

            Assembly[] originalList = currentDomain.GetAssemblies();
            this.Session.InvokeCommand.InvokeScript("Import-Module " + moduleName + " -DisableNameChecking");
            Assembly[] modifiedList = currentDomain.GetAssemblies();
            var targetList = modifiedList.Except(originalList);

            foreach (Assembly assembly in targetList)
            {
                // Grab a Type instance for the first occurrence of a subtyped RegistrationManager. Create a new instance from that System.Type so that we can
                // let them supply the IOpenStackCredential instance...

                Type registrationManagerType    = assembly.GetTypes().Where(b => b.BaseType.UnderlyingSystemType.FullName == "OpenStack.Client.Powershell.Utility.RegistrationManager").First();
                RegistrationManager manager     = (RegistrationManager)Activator.CreateInstance(registrationManagerType, null);
                RegistrationResponse response   = manager.Register(configManager.GetServiceProvider(moduleName));
                IOpenStackCredential credential = response.Credentials;
             
                // Create our Extension Client and stash it....

                var client = OpenStackClientFactory.CreateClient<OpenStackClient>(credential);
                var connectTask = client.Connect();
                connectTask.Wait();

                // Save the credentials ..

                configManager.WriteServiceProvider(response.Provider);

                // Store Context and Client so that all PS-Providers and Cmdlets have access to it..

                this.SetSessionState(credential, client, targetProvider);
            }
        }
    }
}

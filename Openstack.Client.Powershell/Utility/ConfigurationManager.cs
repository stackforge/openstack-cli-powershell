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
using System.Xml.Linq;
using System.Collections;
using System.IO;
using System.Xml.XPath;

namespace OpenStack.Client.Powershell.Utility
{
    public class ConfigurationManager
    {
        private bool _isLoaded = false;
        private XDocument _document = new XDocument();

        public XDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }
//====================================================================================
/// <summary>
/// 
/// </summary>
//====================================================================================
        public ConfigurationManager()
        { }
//====================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="value"></param>
//=========================================================================================
        private XElement CreateAddElement(CredentialElement credentialElement)
        {
            XElement element = new XElement("add");
            element.SetAttributeValue("key", credentialElement.Key);
            element.SetAttributeValue("value", credentialElement.Value);
            element.SetAttributeValue("displayName", credentialElement.DisplayName);
            element.SetAttributeValue("isMandatory", credentialElement.IsMandatory);

            return element;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="provider"></param>
/// <returns></returns>
//==================================================================================================
        private ServiceProvider ResolveServiceProviderCredentials(ServiceProvider provider)
        {
            if (!this.IsLoaded) throw new InvalidOperationException("Instance must be Loaded first..");

            CredentialElement configFilePathCredentialElement = provider.CredentialElements.Where(ce => ce.Key == "ConfigFilePath").SingleOrDefault();
            if (configFilePathCredentialElement != null)
            {
                // Need to get credentials from the linked in Config File..

                ConfigurationManager manager = new ConfigurationManager();
                manager.Load(configFilePathCredentialElement.Value);
                provider.CredentialElements.Remove(provider.CredentialElements.Where(ce => ce.Key == "ConfigFilePath").Single());
                return manager.GetServiceProvider(provider.Name);
            }
            else
            {
                return provider;
            }
        } 
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="moduleName"></param>
//=========================================================================================
        public void SetDefaultServiceProvider(string providerName)
        {
            if (!this.IsLoaded) throw new InvalidOperationException("Instance must be Loaded first..");
                       
            XElement defaultProvider = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("isDefault").Value == "true").Single();
            defaultProvider.Attribute("isDefault").SetValue("false");
            XElement serviceProviderNode = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == providerName).Single();
            serviceProviderNode.Attribute("isDefault").SetValue("true");
            this.Document.Save(this.GetFullConfigPath());
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        public ServiceProvider GetDefaultServiceProvider()
        {
            if (!this.IsLoaded) throw new InvalidOperationException("Instance must be Loaded first..");

            XElement serviceProviderNode = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("isDefault").Value == "true").Single();
            return this.GetServiceProvider(serviceProviderNode.Attribute("name").Value);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public void Load()
        {
            _document = new XDocument(); ;
            _document = XDocument.Load(this.GetFullConfigPath());
            _isLoaded = true;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="configFilePath"></param>
//=========================================================================================
        public void Load(string configFilePath)
        {         
            _document = new XDocument();
            _document = XDocument.Load(configFilePath);
            _isLoaded = true;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private string GetFullConfigPath()
        {
            if (File.Exists("OpenStack.config"))
            {
                return this.GetType().Assembly.Location;
            }
            else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config"))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config";
            }
            else
            {
                throw new InvalidOperationException("Unable to locate OpenStack.config file.");
            }
        }
 //=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <returns></returns>
//=========================================================================================
        public ServiceProvider GetServiceProvider(string name)
        {
            if (!this.IsLoaded) throw new InvalidOperationException("Instance must be Loaded first..");
                       
            XElement serviceProviderNode     = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == name).Single();
            var provider                     = new ServiceProvider();        
            provider.Name                    = serviceProviderNode.Attribute("name").Value;
            provider.IsDefault               = Convert.ToBoolean(serviceProviderNode.Attribute("isDefault").Value);

            // The ServiceProvider in the Primary config file is pointing to a Vender specific config so reolve that first..

            foreach (XElement xElement in serviceProviderNode.Elements())
            {               
                CredentialElement element = new CredentialElement();
                element.Key               = xElement.Attribute("key").Value;
                element.Value             = xElement.Attribute("value").Value;

                 if (xElement.Attribute("key").Value == "AuthenticationServiceURI")               
                    provider.AuthenticationServiceURI = xElement.Attribute("value").Value;                   
               
                if (xElement.Attribute("key").Value == "isDefault")              
                    provider.IsDefault = Convert.ToBoolean(xElement.Attribute("value").Value);
             
                try
                {
                    element.IsMandatory = Convert.ToBoolean(xElement.Attribute("isMandatory").Value);
                    element.DisplayName = xElement.Attribute("displayName").Value;
                    //element.HelpText          = xElement.Attribute("helpText").Value;
                }
                catch (Exception) { }

                if (element.Key == "ConfigFilePath")
                {  
                    provider.CredentialElements.Add(element);
                    ServiceProvider resolvedProvider = this.ResolveServiceProviderCredentials(provider);
                    resolvedProvider.ConfigFilePath = element.Value;
                    return resolvedProvider;
                }
                else
                    provider.CredentialElements.Add(element);
            }
            return provider;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        public IEnumerable<ServiceProvider> GetServiceProviders()
        {
            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            foreach (XElement element in this.Document.Descendants("ServiceProvider"))  
            {
                ServiceProvider newProvider = this.GetServiceProvider(element.Attribute("name").Value);
                newProvider.IsDefault = Convert.ToBoolean(element.Attribute("isDefault").Value);
                serviceProviders.Add(newProvider);
            }
            
            return serviceProviders;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="providerName"></param>
//=========================================================================================
        public void RemoveInitialServiceProvider(ref XDocument _document)
       {
           try
           {
               XElement oldServiceProvider = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == String.Empty).Single();
               if (oldServiceProvider != null)
                   oldServiceProvider.Remove();
           }
           catch (InvalidOperationException ex) { }
       }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="doc"></param>
/// <param name="isDefault"></param>
//=========================================================================================
        private void RemoveDefaultServiceProvider(ref XDocument doc)
        {
            try
            {
                XElement serviceProviderNode = doc.Descendants("ServiceProvider").Where(sp => sp.Attribute("isDefault").Value == "True").Single();
                serviceProviderNode.Attribute("isDefault").SetValue("false");
            }
            catch (Exception) { }
        }
//=========================================================================================
/// <summary>
/// Flush and fill style..
/// </summary>
//=========================================================================================
        public void WriteServiceProvider(ServiceProvider serviceProvider, bool removeInitialServiceProvider = false)
        {
            IEnumerable<XElement> serviceProviderNodes = this.Document.Descendants("ServiceProvider");
            XElement spElement                         = new XElement("ServiceProvider", 
                                                                      new XAttribute("name", serviceProvider.Name), 
                                                                      new XAttribute("isDefault", serviceProvider.IsDefault));

            // Get rid of the old ServiceProvider first ...

            try
            {
                XElement oldServiceProvider = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == serviceProvider.Name).Single();
                if (oldServiceProvider != null)
                    oldServiceProvider.Remove();
            }
            catch (InvalidOperationException ex) { }
           
            
            // If this new Provider is set = default, remove the default flag from the previous one..

            if (serviceProvider.IsDefault) {
                RemoveDefaultServiceProvider(ref _document);
            }
            
            // Create CredentialElement instances for all Key\Value (Add elements).. 
            
            foreach (CredentialElement element in serviceProvider.CredentialElements) {
                spElement.Add(this.CreateAddElement(element));
            }

            if (removeInitialServiceProvider == true)
                this.RemoveInitialServiceProvider(ref _document);

            this.Document.XPathSelectElement("configuration/appSettings/IdentityServices").Add(spElement);
            this.Document.Save(this.GetFullConfigPath());
       }
    }
}

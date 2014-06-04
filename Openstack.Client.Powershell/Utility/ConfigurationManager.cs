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
using Openstack.Client.Powershell.Utility;
using System.Xml.Schema;
using System.Xml;

namespace OpenStack.Client.Powershell.Utility
{
    public class ValidationResult
    {
        private bool _hasErrors = false;
        private List<string> _errorList = new List<string>();

        public List<string> Errors
        {
            get { return _errorList; }
            set { _errorList = value; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            set { _hasErrors = value; }
        }
        
    }

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

            XElement serviceProviderNode = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("isDefault").Value == "true").FirstOrDefault();
            return this.GetServiceProvider(serviceProviderNode.Attribute("name").Value);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private ValidationResult ValidateDocument(XDocument document)
        {   
            ValidationResult result = new ValidationResult();

            // First we check the structural integrity of the document via XSD..

            //string xsdMarkup = this.GetSchema();
            //XmlSchemaSet schemas = new XmlSchemaSet();
            //schemas.Add("", XmlReader.Create(new StringReader(xsdMarkup)));

            //document.Validate(schemas, (o, e) =>
            //{
            //    result.Errors.Add(e.Message);
            //    result.HasErrors = true;
            //});
            
            // Next, ensure that a default Service Provider exist..            
            
            var serviceProviders    = this.Document.Descendants("ServiceProvider");
            bool hasDefaultServiceProvider = serviceProviders.Where(sp => sp.Attributes("isDefault").Single().Value.ToUpper() == "TRUE").Any();
            if (!hasDefaultServiceProvider)  {
                result.HasErrors = true;
                result.Errors.Add("No Default Service Provider found.");
            }

            // Now check that each Service Provider has at least one default AZ.

            foreach (XElement provider in serviceProviders) {             
                if (!provider.Descendants("AvailabilityZones").Descendants().Where(d => d.Attributes("isDefault").Single().Value.ToUpper() == "TRUE").Any()) {
                    result.HasErrors = true;
                    result.Errors.Add("The Service Provider " + provider.Attributes("name").Single().Value + " doesn not have a default Availability Zone/Region");
                }             
            }

            // Make sure that we only have 1 default Service Provider....

             if (serviceProviders.Where(d => d.Attributes("isDefault").Single().Value.ToUpper() == "TRUE").Count() > 1) {
                    result.HasErrors = true;
                    result.Errors.Add("Only one Service Provider can be marked as the default.");
                 }

            // Check that each Service Provider has a unique Name. (how can we get linqs Distinct to work with XElement when we can't
            // force that type to implement IComparable<T> ????

            var table = new Hashtable();
            foreach (XElement provider in serviceProviders)
            {
                string name = provider.Attributes("name").Single().Value;
                try {
                    table.Add(name, String.Empty);
                }
                catch (Exception ex)
                {
                    result.HasErrors = true;
                    result.Errors.Add("Please ensure that all Service Providers have a unique name assigned to them.");
                    return result;
                }
            }             

            return result;           
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private InvalidDataException CreateLoadException(ValidationResult result)
        {           
            int count  = 0;
            string msg = "The config file is invalid for the following reasons => ";
            
            foreach (string message in result.Errors) {
                msg = msg + message;               
            }
            return new InvalidDataException(msg);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public void Load(bool requiresValidation = true)
        {
            ValidationResult result = null;
            _document               = new XDocument(); ;
            _document               = XDocument.Load(this.GetFullConfigPath());

            if (requiresValidation)
            {
                result = this.ValidateDocument(_document);

                if (result.HasErrors) {
                    throw this.CreateLoadException(result);
                }
            }
            _isLoaded = true;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="configFilePath"></param>
//=========================================================================================
        public void Load(string configFilePath, bool requiresValidation = true)
        {      
             ValidationResult result = null;
            _document                = new XDocument();
            _document                = XDocument.Load(configFilePath);

            if (requiresValidation)
            {
                result = this.ValidateDocument(_document);

                if (result.HasErrors){
                    throw this.CreateLoadException(result);
                }
            }

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
/// <param name="serviceProviderNode"></param>
/// <returns></returns>
//=========================================================================================
        private IEnumerable<AvailabilityZone> GetAvailabilityZones(XElement serviceProviderNode)
        {
            List<AvailabilityZone> zones = new List<AvailabilityZone>();

            if (serviceProviderNode.HasElements && serviceProviderNode.Element("AvailabilityZones") != null && serviceProviderNode.Element("AvailabilityZones").Descendants().Count() > 0)
            {
                foreach (XElement az in serviceProviderNode.Element("AvailabilityZones").Descendants()) {

                    AvailabilityZone zone     = new AvailabilityZone();
                    zone.Name                 = az.Attribute("name").Value;
                    zone.ShellForegroundColor = az.Attribute("shellForegroundColor").Value;
                    zone.IsDefault            = Convert.ToBoolean(az.Attribute("isDefault").Value);
                    zone.Id                   = az.Attribute("id").Value;
                    zones.Add(zone);
                }

                return zones;
            }
            else
            {
                return null;
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
            provider.AvailabilityZones       = this.GetAvailabilityZones(serviceProviderNode);
            provider.ServiceMaps             = this.GetServiceMaps(serviceProviderNode);

            // The ServiceProvider in the Primary config file is pointing to a Vender specific config so reolve that first..

            foreach (XElement xElement in serviceProviderNode.Elements())
            {
                if (xElement.Name == "add")
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
            }           
           
            return provider;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="serviceProviderNode"></param>
/// <returns></returns>
//=========================================================================================
        private ServiceMaps GetServiceMaps(XElement serviceProviderNode)
        {
            ServiceMaps serviceMaps = new ServiceMaps();
            var serviceMapsDoc = serviceProviderNode.Descendants("ServiceMaps").Descendants("ServiceMap");

            foreach (XElement map in serviceMapsDoc) {
                
                ServiceMap newMap = new ServiceMap();
                newMap.Source     = map.Attributes("source").Single().Value;
                newMap.Target     = map.Attributes("target").Single().Value;

                serviceMaps.Add(newMap);
            }
            return serviceMaps;
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
            XElement oldServiceProvider = null;

            if (serviceProvider.ConfigFilePath != null)
                this.Load(serviceProvider.ConfigFilePath);
            else
                this.Load();
            
            XElement availabilityZones = null;
            XElement serviceMaps       = null;
            IEnumerable<XElement> serviceProviderNodes = this.Document.Descendants("ServiceProvider");
            XElement spElement                         = new XElement("ServiceProvider", 
                                                                      new XAttribute("name", serviceProvider.Name), 
                                                                      new XAttribute("isDefault", serviceProvider.IsDefault));
            // Get rid of the old ServiceProvider first ...

            try
            {
                if (this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == serviceProvider.Name).Count() > 0)  {
                    oldServiceProvider = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == serviceProvider.Name).Single();
                }
                else {
                    oldServiceProvider = this.Document.Descendants("ServiceProvider").Where(sp => sp.Attribute("name").Value == String.Empty).Single();
                }
                
                // Preserve contained elements so we can place them back with the new provider..

                availabilityZones = oldServiceProvider.Element("AvailabilityZones");
                serviceMaps       = oldServiceProvider.Element("ServiceMaps");

                if (oldServiceProvider != null)
                    oldServiceProvider.Remove();
            }
            catch (InvalidOperationException ex) { }
                       
            // If this new Provider is set = default, remove the default flag from the previous one..

            if (serviceProvider.IsDefault) {
                RemoveDefaultServiceProvider(ref _document);
            }
            
            // Create CredentialElement instances for all Key\Value (Add elements).. 

            spElement.Add(availabilityZones);
            spElement.Add(serviceMaps);


            foreach (CredentialElement element in serviceProvider.CredentialElements) {
                spElement.Add(this.CreateAddElement(element));
            }

            if (removeInitialServiceProvider == true)
                this.RemoveInitialServiceProvider(ref _document);

            if (this.Document != null)
            {
                this.Document.XPathSelectElement("configuration/appSettings/IdentityServices").Add(spElement);
                if (serviceProvider.ConfigFilePath == null)
                   this.Document.Save(this.GetFullConfigPath());
                else
                    this.Document.Save(serviceProvider.ConfigFilePath);
            }
        }

        private string GetSchema()
        {
            return @"<?xml version='1.0' encoding='utf-8'?>
            <xs:schema attributeFormDefault='unqualified' elementFormDefault='qualified' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
              <xs:element name='configuration'>
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name='appSettings'>
                      <xs:complexType>
                        <xs:sequence>
                          <xs:element name='Testing'>
                            <xs:complexType>
                              <xs:sequence>
                                <xs:element maxOccurs='unbounded' name='add'>
                                  <xs:complexType>
                                    <xs:attribute name='key' type='xs:string' use='required' />
                                    <xs:attribute name='value' type='xs:string' use='required' />
                                  </xs:complexType>
                                </xs:element>
                              </xs:sequence>
                            </xs:complexType>
                          </xs:element>
                          <xs:element name='StorageManagement'>
                            <xs:complexType>
                              <xs:sequence>
                                <xs:element maxOccurs='unbounded' name='add'>
                                  <xs:complexType>
                                    <xs:attribute name='key' type='xs:string' use='required' />
                                    <xs:attribute name='value' type='xs:string' use='required' />
                                  </xs:complexType>
                                </xs:element>                
                              </xs:sequence>
                            </xs:complexType>
                          </xs:element>
                          <xs:element name='IdentityServices'>
                            <xs:complexType>
                              <xs:sequence>
                                <xs:element name='ServiceProvider'>
                                  <xs:complexType>
                                    <xs:sequence>
                                      <xs:element minOccurs='1' maxOccurs='unbounded' name='add'>
                                        <xs:complexType>
                                          <xs:attribute name='key' type='xs:string' use='required' />
                                          <xs:attribute name='value' type='xs:string' use='required' />
                                          <xs:attribute name='displayName' type='xs:string' use='required' />
                                          <xs:attribute name='helpText' type='xs:string' use='required' />
                                          <xs:attribute name='isMandatory' type='xs:string' use='required' />
                                        </xs:complexType>
                                      </xs:element>
                                      <xs:element name='AvailabilityZones'>
                                        <xs:complexType>
                                          <xs:sequence>
                                            <xs:element minOccurs ='1'  maxOccurs='unbounded' name='AvailabilityZone'>
                                              <xs:complexType>
                                                <xs:attribute name='id' type='xs:unsignedByte' use='required' />
                                                <xs:attribute name='isDefault' type='xs:string' use='required' />
                                                <xs:attribute name='name' type='xs:string' use='required' />
                                                <xs:attribute name='shellForegroundColor' type='xs:string' use='required' />
                                              </xs:complexType>
                                            </xs:element>
                                          </xs:sequence>
                                        </xs:complexType>
                                      </xs:element>
                                    </xs:sequence>
                                    <xs:attribute name='name' type='xs:string' use='required' />
                                    <xs:attribute name='isDefault' type='xs:boolean' use='required' />
                                  </xs:complexType>
                                </xs:element>
                              </xs:sequence>
                            </xs:complexType>
                          </xs:element>
                          <xs:element name='ComputeServices'>
                            <xs:complexType>
                              <xs:sequence>
                                <xs:element maxOccurs='unbounded' name='add'>
                                  <xs:complexType>
                                    <xs:attribute name='key' type='xs:string' use='required' />
                                    <xs:attribute name='value' type='xs:string' use='required' />
                                  </xs:complexType>
                                </xs:element>
                              </xs:sequence>
                            </xs:complexType>
                          </xs:element>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:schema>";
         
        }
    }
}

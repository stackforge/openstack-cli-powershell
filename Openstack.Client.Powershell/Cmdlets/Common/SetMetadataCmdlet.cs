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
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using System;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain.Admin;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain;
using System.Collections;
using Openstack.Client.Powershell.Providers.Security;
using System.Linq;
using Openstack.Client.Powershell.Providers.Common;
using System.Collections.Generic;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Storage.CDN
{
    [Cmdlet(VerbsCommon.Set, "Metadata", SupportsShouldProcess = true)]  
    public class SetMetadataCmdlet2 : BasePSCmdlet
    {
        private string _cdnContainerName;
        private string _sourcePath;
        private string _objectStorageContainerName;      
        private string _serverId;
        private string[] _extendedProperties;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter( ParameterSetName = "containerMetadata", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("c")]
        [ValidateNotNullOrEmpty]
        public string ObjectStorageContainerName
        {
            get { return _objectStorageContainerName; }
            set { _objectStorageContainerName = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("md")]
        [ValidateNotNullOrEmpty]
        public string[] ExtendedProperties
        {
            get { return _extendedProperties; }
            set { _extendedProperties = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "serverMetadata", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "objectMetadata", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("sp")]
        [ValidateNotNullOrEmpty]
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Mandatory = false, ParameterSetName = "cdnMetadata", ValueFromPipelineByPropertyName = true, HelpMessage = "The Name of the Container to enable for CDN access.")]
        [Alias("cdn")]
        [ValidateNotNullOrEmpty]
        public string CDNContainerName
        {
            get { return _cdnContainerName; }
            set { _cdnContainerName = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessCDNMetadata()
        {
            if (this.Context.ServiceCatalog.DoesServiceExist(Services.CDN) == false)
                Console.WriteLine("You don't have access to CDN services under this account. For information on signing up for CDN access please go to http://Openstack.com/.");
            else
            {
                List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
                if (this.ExtendedProperties != null && this.ExtendedProperties.Count() > 0)
                {
                    foreach (string kv in this.ExtendedProperties)
                    {
                        char[] seperator                     = { '|' };
                        string[] temp                        = kv.Split(seperator);
                        KeyValuePair<string, string> element = new KeyValuePair<string, string>(temp[0], temp[1]);

                        metadata.Add(element);
                    }
                    this.RepositoryFactory.CreateCDNRepository().SetMetaData(this.CDNContainerName, metadata);
                }
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessObjectMetadata()
        {
            IStorageObjectRepository repository = this.RepositoryFactory.CreateStorageObjectRepository();
            StoragePath storagePath             = this.CreateStoragePath(_sourcePath);
            StorageObject storageObject         = new StorageObject();
            storageObject.FileName              = storagePath.FileName;

            storageObject.ExtendedProperties.AddEntries(this.ExtendedProperties);
            repository.SetMetaData(storagePath.AbsoluteURI, storageObject.ExtendedProperties);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="key"></param>
//=========================================================================================
        private void AddElements(MetaData metadata)
        {
            IList elements = ((CommonDriveInfo)this.Drive).CurrentContainer.Entities;

            foreach (KeyValuePair<string, string> element in metadata)
            {
                MetaDataElement newElement = new MetaDataElement();
                newElement.Key             = element.Key;
                newElement.Value           = element.Value;

                elements.Add(newElement);
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="keyValuePairs"></param>
/// <returns></returns>
//=========================================================================================
        public MetaData ReformatMetadata(string[] keyValuePairs)
        {
            MetaData metadata = new MetaData();

            if (keyValuePairs != null && keyValuePairs.Count() > 0)
            {
                foreach (string kv in keyValuePairs)
                {
                    char[] seperator        = { '|' };
                    string[] temp           = kv.Split(seperator);
                    MetaDataElement element = new MetaDataElement();
                    element.Key             = temp[0];
                    element.Value           = temp[1];

                    metadata.Add(temp[0], temp[1]);
                }
                return metadata;
            }
            return null;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessServerMetadata()
        {
            if (this.Context.ServiceCatalog.DoesServiceExist(Services.Compute) == false)
                Console.WriteLine("You don't have access to Compute services under this account. For information on signing up for this please go to http://Openstack.com/.");
            else
            {
                MetaData md = null;
                if (this.ServerId != null)
                {
                    md = this.ReformatMetadata(this.ExtendedProperties);
                    this.RepositoryFactory.CreateServerRepository().SetMetadata(md, this.ServerId);
                    this.UpdateCache();
                }
                else
                {
                    BaseUIContainer currentContainer = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;

                    if (currentContainer.Name == "Metadata")
                    {
                        ServerUIContainer serverContainer = currentContainer.Parent as ServerUIContainer;

                        if (serverContainer != null)
                        {
                            md = this.ReformatMetadata(this.ExtendedProperties);
                            this.RepositoryFactory.CreateServerRepository().SetMetadata(md, serverContainer.Entity.Id);
                            this.UpdateCache();
                        }
                    }
                    else
                    {
                        md = this.ReformatMetadata(this.ExtendedProperties);
                        this.RepositoryFactory.CreateServerRepository().SetMetadata(md, currentContainer.Entity.Id);
                        this.UpdateCache();
                    }
                }
            }
        }
////=========================================================================================
///// <summary>
/////
///// </summary>
////=========================================================================================
        private void ProcessContainerMetadata()
        {
            if (this.Context.ServiceCatalog.DoesServiceExist(Services.ObjectStorage) == false)
                Console.WriteLine("You don't have access to Object Storage services under this account. For information on signing up for this please go to http://Openstack.com/.");
            else
            {
                MetaData md = null;
                if (this.ObjectStorageContainerName != null)
                {
                    List<KeyValuePair<string, string>> exProps = new List<KeyValuePair<string, string>>();
                    if (this.ExtendedProperties != null && this.ExtendedProperties.Count() > 0)
                    {
                        foreach (string kv in this.ExtendedProperties)
                        {
                            char[] seperator = { '|' };
                            string[] temp    = kv.Split(seperator);
                            exProps.AddEntry(temp[0], temp[1]);
                        }
                    }

                    md = this.ReformatMetadata(this.ExtendedProperties);
                    this.RepositoryFactory.CreateContainerRepository().SetMetaData(_objectStorageContainerName, exProps); 
                    this.UpdateCache();
                }
                else
                {

                }
            }
        }
//=========================================================================================
/// <summary>
/// The main driver..
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            switch (this.ParameterSetName)
            {
                case ("serverMetadata"):
                    this.ProcessServerMetadata();
                    break;               

                case ("cdnMetadata"):

                    this.ProcessCDNMetadata();
                    break;

                case ("objectMetadata"):
                    this.ProcessObjectMetadata();
                    break;

                case ("containerMetadata"):
                    this.ProcessContainerMetadata();
                    break;
                   
            }
        }

       
    }
}


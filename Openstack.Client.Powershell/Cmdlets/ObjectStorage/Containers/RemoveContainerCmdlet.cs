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
using System.Management.Automation;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.DataAccess;
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using Openstack.Common.Properties;
using Openstack.Client.Powershell.Providers.Common;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Openstack.Client.Powershell.Cmdlets.Containers
{
    [Cmdlet(VerbsCommon.Remove, "Container", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class RemoveContainerCmdlet : BasePSCmdlet
    {
        private string _name;
        private SwitchParameter _forceDelete = false;
        private SwitchParameter _reset = false;
        private bool _removeCDN = false;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "RemoveContainer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("cdn")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter RemoveCDN
        {
            get { return _removeCDN; }
            set { _removeCDN = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "RemoveContainer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "RemoveContainer", ValueFromPipelineByPropertyName = true, HelpMessage = "s")]
        [Alias("fd")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ForceDelete
        {
            get { return _forceDelete; }
            set { _forceDelete = value; }
        }
        #endregion
        #region Methods
//======================================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name">Name of the Container to trash</param>
//======================================================================================================================
        private bool DeleteContainerContents(StorageContainer storageContainer)
        {
            List<StorageObject> containerItems = null;
            IStorageObjectRepository repository = this.RepositoryFactory.CreateStorageObjectRepository();

            try
            {
                containerItems = repository.GetStorageObjects(storageContainer.Name, true);
            }
            catch (Exception ex)
            {
                return false;
            }

            // Define our Action delegate which will delete the element in the list..

            if (containerItems != null)
            {
                Action<StorageObject> deleteStorageObjectAction = delegate(StorageObject storageObject)
                {
                    StoragePath targetPath = this.CreateStoragePath(storageObject.Key);
                    repository.DeleteStorageObject(targetPath.AbsoluteURI);
                };

                // Remove all Files contained within the storageContainer...

                if (containerItems != null)
                {
                    containerItems.ForEach(deleteStorageObjectAction);
                    return true;
                }
            }
            return true;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void DeleteAllContainers()
        {
            IContainerRepository repository = this.RepositoryFactory.CreateContainerRepository();
            List<StorageContainer> containers = repository.GetStorageContainers();

            foreach (StorageContainer container in containers)
            {
                try
                {
                    // repository.DeleteContainer(container);
                }
                catch (Exception) { }
            }
        }
//======================================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="address"></param>
/// <returns></returns>
//======================================================================================================================
        private string GetContainerName(string url)
        {
            string[] elements = url.Split('/');
            return elements[elements.Length - 1];
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private bool RemoveSharedContainer()
        {
            bool entryRemoved                = false;
            List<StorageContainer> results   = new List<StorageContainer>();
            string configFilePath            = this.ConfigFilePath;
            XDocument doc                    = XDocument.Load(configFilePath);
            IEnumerable<XElement> containers = doc.XPathSelectElements("//SharedContainer");

            foreach (XElement element in containers)
            {
                string sharedPath = (string)element.Attribute("url");
                if (this.GetContainerName(sharedPath) == this.Name)
                {
                    element.Remove();
                    entryRemoved = true;
                }
            }
            doc.Save(configFilePath);
            this.SessionState.Drive.Remove(this.Name, true, "local");
            if (this.Name == this.SessionState.Drive.Current.Name)
                this.SessionState.InvokeCommand.InvokeScript("cd c:");

            return entryRemoved;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (RemoveSharedContainer() == true) return;

            Collection<ChoiceDescription> choices = new Collection<ChoiceDescription>();
            choices.Add(new ChoiceDescription("Y", "Yes"));
            choices.Add(new ChoiceDescription("N", "No"));

            if (_forceDelete == false)
            {
                if (this.Host.UI.PromptForChoice("Confirm Action", "You are about to delete an entire Container. Are you sure about this?", choices, 0) == 0)
                {
                    IContainerRepository repository = this.RepositoryFactory.CreateContainerRepository();
                    StorageContainer storageContainer = new StorageContainer();
                    storageContainer.Name = _name;

                    try
                    {
                        repository.DeleteContainer(storageContainer);
                        if (_removeCDN)
                        {
                            this.RepositoryFactory.CreateCDNRepository().DeleteContainer(_name);
                        }
                    }
                    catch (Exception ex)
                    {
                        // The container has content and the operation is in conflict. Destroy all contents then retry (the User knows what's up).

                        if (ex.Message == "Unknown Repository Error")
                        {
                            if (DeleteContainerContents(storageContainer))
                            {
                                repository.DeleteContainer(storageContainer);
                                if (_removeCDN)
                                {
                                    this.RepositoryFactory.CreateCDNRepository().DeleteContainer(_name);
                                }
                            }
                        }
                    }
                    try
                    {
                        this.SessionState.Drive.Remove(storageContainer.Name, true, "local");
                        if (storageContainer.Name == this.SessionState.Drive.Current.Name)
                            this.SessionState.InvokeCommand.InvokeScript("cd c:");
                    }
                    catch (DriveNotFoundException ex) { }
                }
                else
                {
                    return;
                }
            }
            else
            {
                IContainerRepository repository = this.RepositoryFactory.CreateContainerRepository();
                StorageContainer storageContainer = new StorageContainer();
                storageContainer.Name = _name;

                try
                {
                    repository.DeleteContainer(storageContainer);
                }
                catch (Exception ex)
                {
                    // The container has content and the operation is in conflict. Destroy all contents then retry (the User knows what's up).

                    if (ex.Message == "Unknown Repository Error")
                    {
                        if (DeleteContainerContents(storageContainer))
                        {
                            repository.DeleteContainer(storageContainer);
                            if (_removeCDN)
                            {
                                this.RepositoryFactory.CreateCDNRepository().DeleteContainer(_name);
                            }
                        }
                    }

                }
            }
        #endregion
        }
    }
}

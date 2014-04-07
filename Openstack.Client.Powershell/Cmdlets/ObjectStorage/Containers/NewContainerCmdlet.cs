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
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain;
using Openstack.Client.Powershell.Providers.Storage;
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using Openstack.Client.Powershell.Cmdlets.Common;
using System;
using Openstack.Client.Powershell.Providers.Common;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.IO;

namespace Openstack.Client.Powershell.Cmdlets.Containers
{ 
    [Cmdlet(VerbsCommon.New, "Container", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class NewContainerCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _sharedContainerURL = null;
        private bool _forceSave = false;
        private bool _createCDN = false;

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "SaveSharedStorageContainer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("url")]
        [ValidateNotNullOrEmpty]
        public string SharedContainerURL
        {
            get { return _sharedContainerURL; }
            set { _sharedContainerURL = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "SavestorageContainer", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(ParameterSetName = "SavestorageContainer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("cdn")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter CreateCDN
        {
            get { return _createCDN; }
            set { _createCDN = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "SavestorageContainer", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("f")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Force
        {
            get { return _forceSave; }
            set { _forceSave = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void CreateStorageContainer(StorageContainer storageContainer)
        {
            IContainerRepository repository = this.RepositoryFactory.CreateContainerRepository();
            bool skipAdd = false;

            // These should be transacted somehow..

            repository.SaveContainer(storageContainer);

            Collection<PSDriveInfo> drives = this.SessionState.Drive.GetAllForProvider("OS-Storage");
            foreach (PSDriveInfo drive in drives)
            {
                if (drive.Name == storageContainer.Name)
                {
                    skipAdd = true;
                }
            }
            if (!skipAdd)
            {
                PSDriveInfo psDriveInfo             = new PSDriveInfo(storageContainer.Name, this.GetStorageProvider(drives), "/", "", null);
                OSDriveParameters driveParameters = new OSDriveParameters();
                driveParameters.Settings            = this.Settings;

                this.SessionState.Drive.New(new OSDriveInfo(psDriveInfo, driveParameters, this.Context), "local");
            }                 
          
            this.WriteObject(" ");
            this.WriteObject("Storage Container " + _name + " created successfully.");
            this.WriteObject(" ");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private bool IsDuplicateContainer(string container)
        {
            List<StorageContainer> containers = this.RepositoryFactory.CreateContainerRepository().GetStorageContainers();
            return containers.Where(cn => cn.Name == container).Any();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void SaveNewContainer()
        {
            StorageContainer storageContainer = new StorageContainer();
            storageContainer.Name = _name;

            if (IsDuplicateContainer(this.Name))
            {
                this.WriteObject(" ");
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                Console.WriteLine("Storage Container " + _name + " already exist.");
                Console.WriteLine("");
                return;
            }

            if (storageContainer.ValidateBasicRequirements() == false)
            {
                this.WriteObject(" ");
                this.WriteObject("Storage Container " + _name + " has failed basic validation rules. Please ensure that the name doesn't include a forward slash, single or double quote character and is less than 255 characters in length.");
                return;
            }
            else if (storageContainer.ValidateExtendedRequirements() == false)
            {

                // Check to see if the Container already exist.. Or not      maybe    it does    z

                if (_forceSave)
                {
                    this.CreateStorageContainer(storageContainer);
                    if (_createCDN)
                    {
                        this.RepositoryFactory.CreateCDNRepository().SaveContainer(_name);
                    }
                }
                else
                {
                    Collection<ChoiceDescription> choices = new Collection<ChoiceDescription>();
                    choices.Add(new ChoiceDescription("Y", "Yes"));
                    choices.Add(new ChoiceDescription("N", "No"));

                    if (this.Host.UI.PromptForChoice("Confirm Action", "Specified Storage Container name is not a valid virtalhost name, continue anyway?", choices, 0) == 0)
                    {
                        this.CreateStorageContainer(storageContainer);
                        if (_createCDN)
                        {
                            this.RepositoryFactory.CreateCDNRepository().SaveContainer(_name);
                        }
                    }
                }
            }
            else
            {
                this.CreateStorageContainer(storageContainer);
                if (_createCDN)
                {
                    this.RepositoryFactory.CreateCDNRepository().SaveContainer(_name);
                }

            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void CreateSharedDrive(string drivePath)
        {
            string driveName                    = Path.GetFileName(drivePath);
            Collection<PSDriveInfo> drives      = this.SessionState.Drive.GetAllForProvider("OS-Storage");
            PSDriveInfo psDriveInfo             = new PSDriveInfo(driveName, this.GetStorageProvider(drives), "/", "", null);
            OSDriveParameters driveParameters = new OSDriveParameters();
            driveParameters.Settings            = this.Settings;
            OSDriveInfo newDrive              = new OSDriveInfo(psDriveInfo, driveParameters, this.Context);
            newDrive.SharePath                  = drivePath;

            this.SessionState.Drive.New(newDrive, "local");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void SaveSharedContainer()
        {
            if (this.IsUrlValid(this.SharedContainerURL))
            {
                string configFilePath = this.ConfigFilePath;
                XDocument doc         = XDocument.Load(configFilePath);
                XElement newDrive     = new XElement("SharedContainer");
                newDrive.Add(new XAttribute("url", this.SharedContainerURL));

                doc.Element("configuration").Element("appSettings").Element("StorageManagement").Element("SharedContainers").Add(newDrive);
                doc.Save(configFilePath);
                this.CreateSharedDrive(this.SharedContainerURL);
            }
            else
            {
                Console.WriteLine("Invalid URL supplied");
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private ProviderInfo GetStorageProvider(Collection<PSDriveInfo> drives)
        {
            foreach (PSDriveInfo drive in drives)
            {
                if (drive.Provider.Name == "OS-Storage")
                {
                    return drive.Provider;
                }
            }
            return null;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="smtpHost"></param>
/// <returns></returns>
//=========================================================================================
        private bool IsUrlValid(string smtpHost)
        {
            return true;
            bool br = false;
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(smtpHost);
                br = true;
            }
            catch (SocketException se)
            {
                br = false;
            }
            return br;
        }
//=========================================================================================
/// <summary>
/// qqqqqqqqqqq
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (_sharedContainerURL != null)
            {
                this.SaveSharedContainer();
            }
            else
            {
                this.SaveNewContainer();
            }
        }
        #endregion
    }
}


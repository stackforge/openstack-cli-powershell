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
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Collections.ObjectModel;
using OpenStack.Storage;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Client.Powershell.Providers.Common;
using System.Management.Automation.Runspaces;
using OpenStack;
using System.Threading.Tasks;
using System.Threading;
using OpenStack.Client.Powershell.Providers.ObjectStorage;


namespace OpenStack.Client.Powershell.Providers.Storage 
{
    [CmdletProvider("Object Storage", ProviderCapabilities.ExpandWildcards)]
    public class ObjectStorageNavigationProvider : BaseNavigationCmdletProvider
    { 
        const string cDelimiter    = @"\";
        const string cFolderMarker = "folder.txt";
 
        private class FolderStatistics
        {
            public long TotalFilesFound   = 0;
            public long TotalBytes = 0;
            public long TotalFoldersFound = 0;
        }
        private IStorageServiceClient _storageClient;


        private IStorageServiceClient StorageClient
        {
            get 
            {
                IOpenStackClient client = (IOpenStackClient)this.SessionState.PSVariable.GetValue("CoreClient", null);
                return client.CreateServiceClient<IStorageServiceClient>();
            }

        }

        #region Implementation of DriveCmdletProvider    
//==================================================================================================
/// <summary>
/// Removes an Item from the store..
/// </summary>
/// <param name="path"></param>
//==================================================================================================
        protected override void ClearItem(string path)
        {
            base.ClearItem(path);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private string StorageServiceURL
        {
            get
            {
                if (this.Drive == null || this.Drive.SharePath == null)
                    return this.Context.ServiceCatalog.GetPublicEndpoint("Object Storage", "region-a.geo-1").ToString();
                    //return this.Context.ServiceCatalog.GetPublicEndpoint("object-store", null).ToString();             //.GetService("object-store").Url;
                else
                    return this.Drive.SharePath;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        public System.Collections.ObjectModel.Collection<PSDriveInfo> GetAvailableDrives(Settings settings, ProviderInfo providerInfo)
        {
            IEnumerable<StorageContainer> storageContainers = null;
            ObjectStorageDriveParameters parameters                    = new ObjectStorageDriveParameters();

            if (this.Settings != null)
            {
                parameters.Settings = this.Settings;
            }
            else
            {
                parameters.Settings = settings;
            }
   
            try
            {           
                // Get a list of Storage Containers...

                Task<IEnumerable<StorageContainer>>  getContainersTask = this.Client.CreateServiceClient<IStorageServiceClient>().ListStorageContainers();
                getContainersTask.Wait();
                storageContainers = getContainersTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }          

            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

            // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

            try
            {               
                if (storageContainers.Count() > 0)
                {
                    foreach (StorageContainer storageContainer in storageContainers)
                    {
                        PSDriveInfo driveInfo             = new PSDriveInfo(storageContainer.Name, providerInfo, "/", "Root folder for your storageContainer", null);
                        parameters.Settings               = this.Settings;
                        ObjectStoragePSDriveInfo kvsDriveInfo = new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, this.StorageServiceURL);
                        //kvsDriveInfo.SharePath            = storageContainer.SharePath;
                        drives.Add(kvsDriveInfo);
                    }
                }
                else
                {
                    PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.ProviderInfo, "/", "Root folder for your storageContainer", null);
                    return new Collection<PSDriveInfo>   
                        {   
                        new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, this.StorageServiceURL)   
                        };
                }
            }
            catch (Exception)
            {

            }

            return drives;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private void SetDefaultDrive()
        {
            Runspace runSpace = RunspaceFactory.CreateRunspace();
            runSpace.Open();
            Pipeline pipeline = runSpace.CreatePipeline();
            Command setDefaultDriveCmd = new Command("Set-Location OS-Init:");
            pipeline.Commands.Add(setDefaultDriveCmd);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private Collection<PSDriveInfo> CreateDefaultDrive(ObjectStorageDriveParameters parameters)
        {
            WriteDebug("No Storage Containers found, initializing defaults.");
            PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.ProviderInfo, "/", "Root folder for your Container", null);
          
            return new Collection<PSDriveInfo>   
                        {                      
                        new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, this.StorageServiceURL)   
                        };
        }
//==================================================================================================
/// <summary>
/// This is called when the CLIManifest.psd1 is registered with the Import-Module cmdlet.
/// </summary>
/// <returns></returns>
//==================================================================================================
        protected override System.Collections.ObjectModel.Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            //Thread.Sleep(new TimeSpan(0, 0, 0, 10, 0));

            this.InitializeSession();

            if (this.Context.ServiceCatalog.Exists(this.ProviderInfo.Name))
            {
                IEnumerable<StorageContainer> storageContainers = null;
                ObjectStorageDriveParameters parameters         = new ObjectStorageDriveParameters();
                var storageServiceClient                        = this.Client.CreateServiceClient<IStorageServiceClient>();                
                Task<StorageAccount> accountTask                = storageServiceClient.GetStorageAccount();
                
                accountTask.Wait();
                StorageAccount account         = accountTask.Result;
                storageContainers              = account.Containers;
                Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

                // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

                try
                {
                    if (storageContainers != null)
                    {
                        if (storageContainers.Count() == 1 && storageContainers.First().Name == string.Empty)
                        {
                            return this.CreateDefaultDrive(parameters);
                        }
                        parameters.Settings = this.Settings;

                        foreach (StorageContainer storageContainer in storageContainers)
                        {
                            PSDriveInfo driveInfo             = new PSDriveInfo(storageContainer.Name, this.ProviderInfo, "/", "Root folder for your storageContainer", null);
                            ObjectStoragePSDriveInfo kvsDriveInfo = new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, this.StorageServiceURL);
                            //kvsDriveInfo.SharePath            = storageContainer.SharePath;
                            drives.Add(kvsDriveInfo);
                        }
                    }
                    else
                    {
                        return this.CreateDefaultDrive(parameters);
                    }
                }
                catch (Exception ex)
                {
                    WriteDebug("Exception : HPOSNavigationProvider.InitializeDefaultDrives");
                    WriteDebug(ex.Message);
                    this.WriteError(new ErrorRecord(ex, "1", ErrorCategory.InvalidArgument, this));
                }
                return drives;
            }
            else
            {
                return null;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public bool ContainsWildcard(string path)
        {            
            if (path.Contains("?") || path.Contains("*") || path.Contains("[") || path.Contains("]"))
                return true;
            else
                return false;           
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private string GetContainerName(string path)
        {
            return this.Drive.Name;

            if (path == "//")
            {
                return this.Drive.Name;
            }
            return null;
        }
//==================================================================================================
/// <summary>
/// Retrieves a StorageObject with a fully qualified path from the User.
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private IEnumerable<StorageItemViewModel> GetStorageObjects(string path)
        {
            List<StorageItemViewModel> modelViewItems = new List<StorageItemViewModel>();
            Task<StorageFolder> listStorageObjectTask;
            string folderName = null;

            if (path != @"\" && !path.EndsWith(@"\"))
                path = path + @"\";

            StoragePath storagePath      = this.CreateStoragePath(path);
            var client = this.Client.CreateServiceClient<IStorageServiceClient>();                     
            
            if (path == "\\") {
                folderName = "/";             
            }
            else {                
                folderName =  path.Replace(@"\", "/").TrimStart('/');                
            }

           listStorageObjectTask = client.GetStorageFolder(this.GetContainerName(path), folderName);                
           listStorageObjectTask.Wait();
           foreach (StorageItem item in listStorageObjectTask.Result.Objects.Union<StorageItem>(listStorageObjectTask.Result.Folders))
           {
               StorageItemViewModel modelView = new StorageItemViewModel(item);
               modelViewItems.Add(modelView);
           }
           return modelViewItems;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private void WriteData(IEnumerable<StorageItemViewModel> storageItemViewModel, string path, WildcardPattern pattern = null)
        {
            FolderStatistics statistics = new FolderStatistics();
            bool doesExist              = false;

            if (storageItemViewModel.Count() > 0 || storageItemViewModel == null)
            {
                // Write out any files that we find within this particular folder..

                statistics = this.WriteFiles(storageItemViewModel, path);
                doesExist = true;
            }

            //List<StorageItem> folders = storageObjects.Where(so => so.Name.EndsWith(@"\")).ToList<StorageItem>(); 

            List<StorageItemViewModel> folders = storageItemViewModel.Where(so => so.Type == "Folder").ToList<StorageItemViewModel>(); 

           // List<StorageObject> folders = storageObjects.Where(so => so.StorageObjectType == StorageObjectType.Folder).ToList<StorageObject>(); 

            if (folders != null)
            {
                // Now we'll write out any decendant folders found within the current directory..

                this.WriteFolders(folders, ref statistics);
                doesExist = true;
            }

            // Finally write out the summary folder stats section of the command..

            if (doesExist)
            {
                this.WriteFolderStatistics(statistics);
            }
            else
            {
                Console.WriteLine("No files or folders found.");
                Console.WriteLine("");
            }
        }
//==================================================================================================
/// <summary>
/// Writes out the files represented as StorageObjects for the supplied path.
/// </summary>
//==================================================================================================
        private FolderStatistics WriteFiles(IEnumerable<StorageItemViewModel> storageObjects, string path)
        {            
            FolderStatistics statistics = new FolderStatistics();           

            if (storageObjects != null && storageObjects.ToList() != null)
            {
                storageObjects.ToList().ForEach(item =>
                {                 
                    if (item.Type == "File")
                    {
                        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                        WriteItemObject(item, item.Name, false);
                        statistics.TotalFilesFound++;
                        //statistics.TotalBytes = statistics.TotalBytes + item.Length;
                    }                    
                });
            }
            return statistics;         
        }
//==================================================================================================
/// <summary>
/// Writes out a list of supplied Folders and returns stats on them as well.
/// </summary>
//==================================================================================================
        private void WriteFolders(List<StorageItemViewModel> folders, ref FolderStatistics statistics)
        {
            if (folders != null)
            {
                // Write out each folder we find in the current directory..

                foreach (StorageItemViewModel obj in folders)
                {
                    string fixedString   = obj.Name.TrimEnd('/');
                    string[] folderNames = fixedString.Split('/');
                    string folderName    = folderNames[folderNames.Count() - 1];
                    //obj.Name             = folderName;

                    if (folderName != "")
                    {
                        WriteItemObject(obj, folderName, false);
                        statistics.TotalFoldersFound++;
                    }
                }
            }
            else
            {
                statistics.TotalFoldersFound = 0;
            }
        }
//==================================================================================================
/// <summary>
/// Writes out summary information on a given folder.
/// </summary>
//==================================================================================================
        private void WriteFolderStatistics(FolderStatistics statistics)
        {
            if (statistics.TotalFoldersFound > 0 || statistics.TotalFilesFound > 0)
            WriteItemObject ("----------------------------------------------------------------------------------------------", cDelimiter, false);
            WriteItemObject(" ", cDelimiter, false);
            WriteItemObject("Total # Folders found     : " + Convert.ToString(statistics.TotalFoldersFound) , cDelimiter, false);
            WriteItemObject("Total # Items Found       : " + Convert.ToString(statistics.TotalFilesFound) , cDelimiter, false);
            WriteItemObject("Total # Bytes in Folder   : " + Convert.ToString(statistics.TotalBytes), cDelimiter, false);
           // WriteItemObject("Total Cost of Folder      : " + "$" + Convert.ToString(statistics.TotalBytes * 2), cDelimiter, false);
            WriteItemObject(" ", cDelimiter, false);
        }
//==================================================================================================
/// <summary>
/// Writes out header information to be used during listing operations.
/// </summary>
//==================================================================================================
        private void WriteHeader()
        {
             // Write out the commands header information first..
                        
            WriteItemObject(" ", cDelimiter, false);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteItemObject("==============================================================================================", cDelimiter, false);
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteItemObject("Storage Container : " + this.Drive.Name, cDelimiter, false);
            WriteItemObject("Directory of      : " + this.Drive.Name + " " + cDelimiter + this.Drive.CurrentLocation, cDelimiter, false);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteItemObject("==============================================================================================", cDelimiter, false);
            Console.ForegroundColor = ConsoleColor.Green;
            WriteItemObject(" ", cDelimiter, false);        
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private bool CheckDefaultDrive(bool suppressMsg)
        {
            if (this.Drive.Name == "OS-Init")
            {
                if (!suppressMsg)
                {
                    Console.WriteLine(" ", cDelimiter, false);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("==============================================================================================", cDelimiter, false);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You are currently connected to the default Container. This drive is used to boot-strap the");
                    Console.WriteLine("Container creation prcoess. The only valid command that you can issue from here is the");
                    Console.WriteLine("New-Container command. After the Container is created successfully, issue CD yourcontainer:");
                    Console.WriteLine("This will bind to that container exposing the full functionality of OpenStack Object Storage. ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("==============================================================================================", cDelimiter, false);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" ", cDelimiter, false);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        protected override object GetItemDynamicParameters(string path)
        {
            return base.GetItemDynamicParameters(path);
        }

        protected override void SetItem(string path, object value)
        {
            base.SetItem(path, value);
        }
        protected override string[] ExpandPath(string path)
        {
            return base.ExpandPath(path);
        }
        protected override string GetParentPath(string path, string root)
        {
            return base.GetParentPath(path, root);
        }
        protected override object GetChildNamesDynamicParameters(string path)
        {
            return base.GetChildNamesDynamicParameters(path);
        }
        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            return base.GetChildItemsDynamicParameters(path, recurse);
        }
        protected override object ClearItemDynamicParameters(string path)
        {
            return base.ClearItemDynamicParameters(path);
        }
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            return base.NormalizeRelativePath(path, basePath);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override bool IsItemContainer(string path)
        {       
            if (path == @"\")
            {
                if (this.CheckDefaultDrive(false)) return true;
            }
            else
            {
                StoragePath storagePath = null;

                if (path.Contains(":"))
                {
                    storagePath = new StoragePath(this.StorageServiceURL + path);
                }
                else
                {
                    storagePath = new StoragePath(this.StorageServiceURL, this.Drive.Name, path.Substring(1) + "/");
                }

                IStorageServiceClient storageService = this.Client.CreateServiceClient<IStorageServiceClient>(); 
                Task<StorageObject> getStorageObjectTask = storageService.GetStorageObject(storagePath.Volume, storagePath.ResourcePath);                            //(storagePath.AbsoluteURI);
                getStorageObjectTask.Wait();
                StorageObject sObject = getStorageObjectTask.Result;
                
                if (sObject != null)
                {                   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
//==================================================================================================
/// <summary>
/// Displays all StorageObjects for a given folder..
/// </summary>
/// <param name="path"></param>
/// <param name="recurse"></param>
//==================================================================================================
        protected override void GetChildItems(string path, bool recurse)
        {
            // Write out the commands header information first..

            if (this.CheckDefaultDrive(true)) return;

            this.WriteHeader();
            this.WriteData(this.GetStorageObjects(path).ToList<StorageItemViewModel>(), path);            
        }
//==================================================================================================
/// <summary>
/// Called when the user decides to delete a KVSDrive.
/// </summary>
/// <param name="drive"></param>
/// <returns></returns>
//==================================================================================================
        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, drive));
                return null;
            }
            return drive;
        }
//==================================================================================================
/// <summary>
/// Called by the PS runtime when a NewDrive is required..
/// </summary>
/// <param name="drive"></param>
/// <returns></returns>
//==================================================================================================
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive == null) {
                WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, drive));
                return null;
            }

            if (drive.Root == null) {
                WriteError(new ErrorRecord(new ArgumentNullException("drive.Root"), "NullRoot", ErrorCategory.InvalidArgument, drive));
                return null;
            }

            if (drive is ObjectStoragePSDriveInfo) {
                return drive;
            }

            var driveParams = this.DynamicParameters as ObjectStorageDriveParameters;
            return new ObjectStoragePSDriveInfo(drive, driveParams, this.Context, this.StorageServiceURL);
        }
//==================================================================================================
/// <summary>
/// Called by the PS runtime when 
/// </summary>
/// <returns></returns>
//==================================================================================================
        protected override object NewDriveDynamicParameters()
        {
            // The KVSDriveParameters instance will pull its values from the config file when created..

            return new ObjectStorageDriveParameters();
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private string GetLeafFolder(string path)
        {
            string[] elements = path.Replace(@"\", "/").Split('/');
            return elements[elements.Count() - 2];
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <param name="itemTypeName"></param>
/// <param name="newItemValue"></param>
//==================================================================================================
        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            //// Validate the folder name first..

            //if (path.Contains("%") || path.Contains("."))
            //{
            //    Console.WriteLine("");
            //    Console.WriteLine("Folder names cannot contain % or . characters" );
            //    Console.WriteLine("");
            //}
            //else
            //{
            //    if (!path.EndsWith("/"))
            //    {
            //        path = path + "/";
            //    }

            //    StoragePath newFolderPath = this.CreateStoragePath(path);
            //    this.RepositoryFactory.CreateStorageObjectRepository().MakeFolder(newFolderPath.AbsoluteURI);
            //}
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private string ExtractPattern(string path)
        {
            string[] elements = path.Split('\\');
            return elements[elements.Length - 1];
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
//==================================================================================================
        protected override void GetItem(string path)
        {
            // Write out the commands header information first..

            if (this.CheckDefaultDrive(true)) return;
            this.WriteHeader();
            IEnumerable<StorageItemViewModel> storageObjects = this.GetStorageObjects(path);



            this.WriteData(storageObjects.ToList<StorageItemViewModel>(), path);           
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override bool ItemExists(string path)
        {
            if (path == "//")
            {
                if (this.CheckDefaultDrive(true))
                {
                    return false;
                }
                else
                {
                    return base.ItemExists(path);
                }
            }
            return base.ItemExists(path);
        }
        #endregion
        #region Properties   
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private ObjectStoragePSDriveInfo Drive
        {
            get
            {
                return this.PSDriveInfo as ObjectStoragePSDriveInfo;
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
//=========================================================================================
        private StoragePath CreateStoragePath(string path)
        {
            return this.Drive.CreateStoragePath(path);
        }
        #endregion   
//==================================================================================================
/// <summary>
/// This test should not verify the existance of the item at the path. 
/// It should only perform syntactic and semantic validation of the 
/// path. For instance, for the file system provider, that path should
/// be canonicalized, syntactically verified, and ensure that the path
/// does not refer to a device.
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override bool IsValidPath(string path)
        {
            return true;
        }
    }
}

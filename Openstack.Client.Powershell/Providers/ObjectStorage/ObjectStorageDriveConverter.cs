using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Openstack.Client.Powershell.Utility;
using OpenStack;
using OpenStack.Client.Powershell.Providers.Storage;
using OpenStack.Client.Powershell.Utility;
using OpenStack.Storage;

namespace OpenStack.Client.Powershell.Providers.Storage
{
    /// <summary>
    /// This class is responsible for Converting the Users current set of Storage Containers into PSDrives..
    /// </summary>
    public class ObjectStorageDriveConverter
    {
        private Context _context;
        private ProviderInfo _providerInfo;
        private IOpenStackClient _client;
        private SessionState _sessionState;
        
        #region Properties
        private SessionState SessionState
        {
            get { return _sessionState; }
            set { _sessionState = value; }
        }
        private IOpenStackClient CoreClient
        {
          get { return _client; }
          set { _client = value; }
        }

        private Context Context
        {
            get { return _context; }
        }

        private ProviderInfo ProviderInfo
        {
          get { return _providerInfo; }
          set { _providerInfo = value; }
        }
        #endregion
        #region Ctors
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="providerInfo"></param>
/// <param name="client"></param>
//==================================================================================================
        public ObjectStorageDriveConverter(Context context, ProviderInfo providerInfo, IOpenStackClient client)
        {
            _context      = context;
            _providerInfo = providerInfo;
            _client       = client;
        }
        #endregion
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="service"></param>
/// <returns></returns>
//==================================================================================================
        protected T CreateServiceClient<T>(CoreServices service) where T : IOpenStackServiceClient
        {
            ServiceProvider provider = this.Context.CurrentServiceProvider;
            return this.CoreClient.CreateServiceClientByName<T>(provider.ServiceMaps.TranslateServiceName(service));
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        public System.Collections.ObjectModel.Collection<PSDriveInfo> ConvertContainers()
        { 
            IEnumerable<StorageContainer> storageContainers = null;
            var parameters = new ObjectStorageDriveParameters();

            if (this.Context != null && this.Context.Settings != null) {
                parameters.Settings = this.Context.Settings;
            }           
            else {
                throw new NullReferenceException("Context and/or Settings");
            }

            try
            {
                this.CoreClient.SetRegion(this.Context.CurrentRegion);
                Task<StorageAccount> getAccountTask = this.CreateServiceClient<IStorageServiceClient>(CoreServices.ObjectStorage).GetStorageAccount();
                getAccountTask.Wait();
                storageContainers = getAccountTask.Result.Containers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

            // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

            try
            {
                string serviceName    = this.Context.CurrentServiceProvider.ServiceMaps.TranslateServiceName(CoreServices.ObjectStorage);
                string publicStoreUrl = this.Context.ServiceCatalog.GetPublicEndpoint(serviceName, this.Context.CurrentRegion).ToString();

                if (storageContainers.Count() > 0)
                {
                    foreach (StorageContainer storageContainer in storageContainers)
                    {
                        PSDriveInfo driveInfo = new PSDriveInfo(storageContainer.Name, this.ProviderInfo, "/", "Root folder for your storageContainer", null);
                        ObjectStoragePSDriveInfo kvsDriveInfo = new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl);
                        try
                        {
                            drives.Add(kvsDriveInfo);
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
                    return new Collection<PSDriveInfo>   
                        {   
                        new ObjectStoragePSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl)   
                        };
                }
            }
            catch (Exception ex) {}

            return drives;
        }
//=======================================================================================================
/// <summary>
/// Removes all currently registered drives..
/// </summary>
//=======================================================================================================
        private void RemoveDrives()
        {
            // Remove the old Users drives first..

            Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("Object Storage");
            foreach (PSDriveInfo deadDrive in deadDrives)
            {
                this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
            }
        }
    }
}

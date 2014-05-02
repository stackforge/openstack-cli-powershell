//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Management.Automation;
//using System.Text;
//using System.Threading.Tasks;
//using OpenStack.Client.Powershell.Providers.Storage;
//using OpenStack.Identity;
//using OpenStack.Storage;

//namespace OpenStack.Client.Powershell.Utility
//{
//    public class AccountCapabilities
//    {
//        private SessionState _session;
//        private Context _context;

//        #region Properties
//        public Context Context
//        {
//            get { return _context; }
//            set { _context = value; }
//        }
//        public SessionState Session
//        {
//            get { return _session; }
//            set { _session = value; }
//        }

//        public AccountCapabilities(SessionState session, Context context)
//        {
//            _session = session;
//            _context = context;
//        }
//        //=======================================================================================================
//        /// <summary>
//        /// 
//        /// </summary>
//        //=======================================================================================================
//        public void WriteServices()
//        {
//            this.Session.InvokeCommand.InvokeScript("Hey");

//            //WriteObject("");
//            //Console.ForegroundColor = ConsoleColor.DarkGray;
//            //WriteObject("=================================================================");
//            //Console.ForegroundColor = ConsoleColor.Yellow;
//            //WriteObject("Binding to new Account. New service catalog is as follows.");
//            //Console.ForegroundColor = ConsoleColor.DarkGray;
//            //WriteObject("=================================================================");
//            //Console.ForegroundColor = ConsoleColor.Green;
//            //WriteObject(" ");

//            //foreach (OpenstackServiceDefinition service in this.Context.ServiceCatalog)
//            //{
//            //    WriteObject(service);
//            //}
//            //WriteObject("");
//        }
//        //==================================================================================================
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        //==================================================================================================
//        private System.Collections.ObjectModel.Collection<PSDriveInfo> GetAvailableDrives(Settings settings, ProviderInfo providerInfo) //, string configFilePath)
//        {
//            List<StorageContainer> storageContainers = null;
//            OSDriveParameters parameters = new OSDriveParameters();

//            if (this.Context.Settings != null)
//            {
//                parameters.Settings = this.Context.Settings;
//            }
//            else
//            {
//                parameters.Settings = settings;
//            }

//            try
//            {
//                Task<IEnumerable<StorageContainer>> getContainersTask = this.CoreClient.CreateServiceClient<IStorageServiceClient>().ListStorageContainers();
//                getContainersTask.Wait();
//                storageContainers = getContainersTask.Result.ToList<StorageContainer>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex);
//            }

//            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();

//            // For every storageContainer that the User has access to, create a Drive that he can mount within Powershell..

//            try
//            {
//                string publicStoreUrl = this.Context.ServiceCatalog.GetPublicEndpoint("Object Storage", "region-a.geo-1").ToString();

//                if (storageContainers.Count > 0)
//                {
//                    foreach (StorageContainer storageContainer in storageContainers)
//                    {
//                        PSDriveInfo driveInfo = new PSDriveInfo(storageContainer.Name, providerInfo, "/", "Root folder for your storageContainer", null);
//                        OpenStackPSDriveInfo kvsDriveInfo = new OpenStackPSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl);
//                        try
//                        {
//                            drives.Add(kvsDriveInfo);
//                        }
//                        catch (Exception) { }
//                    }
//                }
//                else
//                {
//                    PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
//                    return new Collection<PSDriveInfo>   
//                        {   
//                        new OpenStackPSDriveInfo(driveInfo, parameters, this.Context, publicStoreUrl)   
//                        };
//                }
//            }
//            catch (Exception ex)
//            {
//                int g = 7;
//            }

//            return drives;
//        }
//        //=======================================================================================================
//        /// <summary>
//        /// Removes all currently registered drives..
//        /// </summary>
//        //=======================================================================================================
//        private void RemoveDrives()
//        {
//            // Remove the old Users drives first..

//            Collection<PSDriveInfo> deadDrives = this.SessionState.Drive.GetAllForProvider("Object Storage");
//            foreach (PSDriveInfo deadDrive in deadDrives)
//            {
//                this.SessionState.Drive.Remove(deadDrive.Name, true, "local");
//            }
//        }
//        //=======================================================================================================
//        /// <summary>
//        /// 
//        /// </summary>
//        //=======================================================================================================
//        public void WriteContainers(string configFilePath)
//        {
//            List<string> invalidDriveNames = new List<string>();
//            OSDriveParameters parameters = new OSDriveParameters();

//            // Write out the commands header information first..

//            WriteObject("");
//            Console.ForegroundColor = ConsoleColor.DarkGray;
//            WriteObject("===================================================================");
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            WriteObject("Object Storage Service available. Remapping to the following drives.");
//            Console.ForegroundColor = ConsoleColor.DarkGray;
//            WriteObject("===================================================================");
//            Console.ForegroundColor = ConsoleColor.Green;
//            WriteObject(" ");

//            HPOSNavigationProvider provider = new HPOSNavigationProvider();
//            Collection<PSDriveInfo> drives = this.GetAvailableDrives(this.Context.Settings, this.SessionState.Provider.GetOne("Object Storage"));

//            if (drives != null)
//            {
//                this.RemoveDrives();

//                foreach (PSDriveInfo drive in drives)
//                {
//                    if (drive.Name != string.Empty)
//                    {
//                        WriteObject("Storage Container : [" + drive.Name + "] now available.");
//                    }

//                    try
//                    {
//                        this.SessionState.Drive.New(drive, "local");
//                    }
//                    catch (PSArgumentException ex)
//                    {
//                        if (drive.Name != string.Empty)
//                            invalidDriveNames.Add(drive.Name);
//                    }
//                    catch (Exception) { }

//                }
//                WriteObject("");
//            }
//            else
//            {
//                // No storageContainers exist for the new credentials so make some up...

//                //PSDriveInfo driveInfo = new PSDriveInfo("OS-Init", this.SessionState.Drive.Current.Provider, "/", "Root folder for your storageContainer", null);
//                //this.SessionState.Drive.New(new OSDriveInfo(driveInfo, parameters, this.Context), "local");
//            }

//            if (invalidDriveNames.Count > 0)
//            {
//                WriteObject("");
//                Console.ForegroundColor = ConsoleColor.DarkGray;
//                WriteObject("=================================================================");
//                Console.ForegroundColor = ConsoleColor.Red;
//                WriteObject("Error : A subset of your Containers could not be bound to this");
//                WriteObject("session due to naming conflicts with the naming standards required");
//                WriteObject("for Powershell drives. These containers are listed below.");
//                Console.ForegroundColor = ConsoleColor.DarkGray;
//                WriteObject("=================================================================");
//                Console.ForegroundColor = ConsoleColor.Green;
//                WriteObject(" ");

//                foreach (string name in invalidDriveNames)
//                {
//                    WriteObject(name);
//                    WriteObject(" ");
//                }
//                WriteObject(" ");
//            }
//        }
//    }
//}
//        #endregion
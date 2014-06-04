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
using System.Collections.Generic;
using System.Management.Automation;
using System.Collections;
using OpenStack.Client.Powershell.Utility;
using System;
using System.IO;
using System.Linq;
using OpenStack.Storage;
using Openstack.Client.Powershell.Utility;


namespace OpenStack.Client.Powershell.Providers.Storage
{
    public class ObjectStoragePSDriveInfo : PSDriveInfo 
    {
        public const string cDelimiter = "/";

        private ObjectStorageDriveParameters _parameters = null;
        private IStorageServiceClient _storageClient;       
        private List<string> _pathCache = new List<string>();
        private bool _isQueueOn = false;
        private Settings _settings = null;
        private Context _context;
        private string _sharePath;
        private string _storageServiceUrl;

        #region Ctors
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="driveInfo"></param>
//==================================================================================================
        public ObjectStoragePSDriveInfo(PSDriveInfo driveInfo, ObjectStorageDriveParameters parameters, Context context , string storageServiceUrl): base(driveInfo)   
        {            
            _parameters        = parameters;
            _context           = context;
            _storageServiceUrl = storageServiceUrl;

            if (parameters != null)
               _settings = parameters.Settings;
        }   
        #endregion
        #region Methods  
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private string GetFirstPathElement(string path)
        {
            int delimeterPosition;

            if (path.StartsWith(@"\"))
            {
                delimeterPosition = path.IndexOf(@"\", 1, StringComparison.Ordinal);
                if (delimeterPosition != -1)
                    return path.Substring(0, delimeterPosition);
                else
                    return null;
            }
            else
            {
                delimeterPosition = path.IndexOf("/", 0, StringComparison.Ordinal);
                if (delimeterPosition != -1)
                    return path.Substring(0, delimeterPosition);
                else
                {
                    delimeterPosition = path.IndexOf(@"\", 0, StringComparison.Ordinal);
                    if (delimeterPosition != -1)
                        return path.Substring(0, delimeterPosition);
                    else
                        return null;
                }
             }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private bool IsOpenStackDrive(string name)
        {
            if (name != null)
            {
                if (name.Contains(":"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }

        public string SharePath
        {
            get { return _sharePath; }
            set { _sharePath = value; }
        }
        private IStorageServiceClient StorageClient
        {
            get { return _storageClient; }
            set { _storageClient = value; }
        }
        
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private bool ContainsHPOSDrive(string name)
        {
            foreach (PSDriveInfo drive in this.Provider.Drives)
            {
                if (drive.Provider.Name == "Object Storage" && drive.Name.Contains(name))
                    return true;
            }
            return false;
        }
////=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=========================================================================================
        private bool IsLocalPath(string path)
        {
            // Temporaily reverse the delimiter for this check...

            string temp            = path.Replace(@"/", @"\");
            List<DriveInfo> drives = DriveInfo.GetDrives().ToList<DriveInfo>();

            drives.DefaultIfEmpty(null);
            if (drives.Where(d => temp.ToUpper().Contains(d.Name)).FirstOrDefault() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private bool IsFullyQualifiedPath (string path)
        {
            string firstPathElement = GetFirstPathElement(path);
            
            if (IsOpenStackDrive(firstPathElement))
            {
                return true;
            }
            else if (IsLocalPath(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private string FormatPath (string path)
        {
            string currentLocation = null;

            // Strip out any leading or trailing delimiters..

            if (path.StartsWith(@"\") || path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            // Reverse the delimiter and strip out the current location. We do this because some paths will come from the provider
            // in which PS has already supplied the fully qualified path, yet some paths will come from cmdlets (like CopyItem)
            // that receive paths from the user. These paths may or may not contain the current location so we strip it out reguardless
            // then read it back...

            path            = path.Replace(@"\", "/");
            currentLocation = this.CurrentLocation.Replace(@"\", "/");

            if (currentLocation != "") 
            {
                path = path.Replace(currentLocation + "/", string.Empty);
            }
            path = path.Replace(this.Name + "/", string.Empty);
            return path;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private StoragePath ProcessRootPath(string path)
        {
            path = this.FormatPath(path);

            if (this.IsLocalPath(path))
            {
                // Nothing to do to a pure local path. Just return it wrapped up..

                return new StoragePath(path);
            }
            else
            {              
                string firstElement = this.GetFirstPathElement(path);

                if (this.IsOpenStackDrive(firstElement))
                {
                    // If the path supplied already contains a storageContainer name, strip it out and pass it in as the volume name..

                    path = path.Replace(firstElement, string.Empty);

                    return new StoragePath(this.StorageServiceUrl);                 
             
                }
                else
                {
                    // If the path supplied lacks a storageContainer name, take the current one..

                    return new StoragePath(this.StorageServiceUrl, this.Name, path.Replace(@"\", "/"));
                }
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private StoragePath ProcessNonRootPath (string path)
        {
            string publicURL = this.StorageServiceUrl;

            if (this.IsFullyQualifiedPath(path) == true)
            {
                return new StoragePath(this.StorageServiceUrl + "/" + path.Replace(@"\", "/").Replace(":", "/"));
            }
            else
            { 
                path = this.FormatPath(path);
            
                if (path.EndsWith(cDelimiter))
                {
                    // We're dealing with a folder path here..

                    return new StoragePath(publicURL, this.Name, this.CurrentLocation.Replace(@"\", "/") + cDelimiter + path);                  

                }
                else
                {
                    // We're dealing with an object path here..

                    path = (publicURL + cDelimiter + this.Name + cDelimiter + this.CurrentLocation.Replace(@"\", "/") + cDelimiter + path);
                    return new StoragePath(path);
                }
            }          
     }
 //==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sourcePath"></param>
/// <returns></returns>
//==================================================================================================
        public StoragePath CreateStoragePath(string path)
        {            
            if (this.IsLocalPath(path))
            {
                return new StoragePath(path);
            }
            else
            {
                // Check for a root path supplied first..

                if (this.CurrentLocation == string.Empty)
                {
                    return this.ProcessRootPath(path);
                }
                else
                {
                  return this.ProcessNonRootPath(path);
                }
            }
            return null;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        public Hashtable GetParameters()
        {
            return null;
         }
        #endregion     
        #region Properties
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private string StorageServiceUrl
        {
            get
            {
                if (this.SharePath == null)
                {
                    string serviceName = this._context.CurrentServiceProvider.ServiceMaps.TranslateServiceName(CoreServices.ObjectStorage);
                    return this._context.ServiceCatalog.GetPublicEndpoint(serviceName, _context.CurrentRegion).ToString();
                    
                }
                else
                    return this.SharePath;
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        public List<string> PathCache
        {
            get { return _pathCache; }
            set { _pathCache = value; }
        }
        #endregion
    }
}

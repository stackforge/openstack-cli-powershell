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
using System.IO;
using OpenStack;
using OpenStack.Client.Powershell.Utility;
using System.Collections.ObjectModel;
using OpenStack.Common;
using System.Diagnostics.Contracts;
using System.Web;

namespace OpenStack.Client.Powershell.Utility
{
    public class StoragePath
    {
        const string cFolderMarker = "folder.txt";
        private string _storageManagementURI;
        private string _path;
        private PathType _pathType;
        private string _volume;
        private string _fileName;

//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="drive">The KvsDriveInfo instance</param>
/// <param name="path">The path to the resource</param>
//=======================================================================================================
        public StoragePath(string dnsPortion, string volume, string path)
        {
            Contract.Requires(dnsPortion != null);
            Contract.Requires(volume != null);
            Contract.Requires(path != null);

            string absoluteURI;
            if (volume != "")
                absoluteURI = dnsPortion + "/" + volume + "/" + path;
            else
                absoluteURI = dnsPortion + "/" + path;
                        
            _pathType = this.GetPathType(absoluteURI);

            if (_pathType == PathType.Remote)
            {
                _storageManagementURI = this.GetPathSubstring(absoluteURI, 2);
                string resourcePath   = volume + "/" + path;
                _volume               = volume;
                _path                 = path;
                _fileName             = System.IO.Path.GetFileName(_path);
            }
            else
            {
                _volume   = volume;
                _path     = path;
                _fileName = System.IO.Path.GetFileName(_path);
            }
            _volume = volume;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="drive">The KvsDriveInfo instance</param>
/// <param name="path">The path to the resource</param>
//=======================================================================================================
        public StoragePath(string absoluteURI)
        {
            this.Initialize(absoluteURI);
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="absoluteURI"></param>
//=======================================================================================================
        private void Initialize(string absoluteURI)
        {
            Contract.Requires(absoluteURI != null);

            _pathType = this.GetPathType(absoluteURI);

            if (_pathType == PathType.Remote)
            {
                _storageManagementURI = this.GetPathSubstring(absoluteURI, 2);
                string resourcePath   = absoluteURI.Replace(_storageManagementURI, String.Empty).Substring(1);
                _volume               = StoragePath.GetFirstPathElement(resourcePath);
                _path                 = absoluteURI.Replace(_storageManagementURI + "/" + _volume, String.Empty).Substring(1);
                _fileName             = System.IO.Path.GetFileName(_path);
            }
            else
            {
                _volume   = StoragePath.GetFirstPathElement(absoluteURI);
                _path     = absoluteURI.Replace(_volume, String.Empty).Substring(1);
                _fileName = System.IO.Path.GetFileName(_path);
            }
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
            path = path.TrimEnd('/');
            string[] elements = path.Split('/');
            return elements[elements.Length - 1];
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public bool IsRoot
        {
            get
            {
                if (this.ContainsWildcard)

                    if (this.GetDirectoryName().Replace(this.ExtractPattern(this.Path), String.Empty) == String.Empty)
                        return true;
                    else
                        return false;
                return false;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public bool ContainsWildcard
        {
            get
            {
                if (this.Path.Contains("?") || this.Path.Contains("*") || this.Path.Contains("[") || this.Path.Contains("]"))
                    return true;
                else
                    return false;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <returns></returns>
//=======================================================================================================
        private string CleanVolumeName(string name)
        {
            string storageContainerName = name.Replace("/", "");
            int index         = storageContainerName.IndexOf(":");

            if (index != -1)
                storageContainerName = storageContainerName.Remove(index, 1);

            return storageContainerName.Replace(@"\", "").Trim();
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private string CleanPath(string path)
        {
            string tempPath = path.Replace(@"\", "/");

            if (path.StartsWith("/"))
            {
                tempPath = tempPath.Substring(1);
            }

            // If this is not a folder path, extract out the filename..

            if (!tempPath.EndsWith("/"))
            {
                _fileName = System.IO.Path.GetFileName(tempPath);
            }
            return tempPath;
        }
        #region Properties
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public PathType PathType
        {
            get { return _pathType; }
            //set { _pathType = value; }
        }
        #endregion
        #region Methods
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=======================================================================================================
        public string GetDirectoryName()
        {
            return System.IO.Path.GetDirectoryName(this.Path);
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private string GetPathSubstring(string element, int offset)
        {
            bool isSecure = false;

            if (element.StartsWith("https://"))
            {
                element = element.Replace("https://", string.Empty);
                isSecure = true;
            }
            else
            {
                element = element.Replace("http://", string.Empty);
            }
            string [] elements = element.Split('/');
            string result = null;

            for (int i = 0; i <= offset; ++i) {
                result = result + "/" + elements[i];                            
            }

            if (isSecure)
            {
                return "https:/" + result;
            }
            else
            {
                return "http:/" + result;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        public static string GetFirstPathElement(string element)
        {
            if (element != string.Empty)
            {
                if (element.StartsWith("/"))
                {
                    int delimeterPosition = element.IndexOf("/", 1, StringComparison.Ordinal);
                    if (delimeterPosition != -1)
                        return element.Substring(0, delimeterPosition);
                    else
                        return null;
                }
                else
                {
                    int delimeterPosition = element.IndexOf("/", 0, StringComparison.Ordinal);
                    if (delimeterPosition != -1)
                        return element.Substring(0, delimeterPosition);
                    else
                    {
                        // No / delimiters exist. This is a local file path with \ as the delimiter so parse with that in mind..

                        int delimeterPosition2 = element.IndexOf(@"\", 0, StringComparison.Ordinal);
                        if (delimeterPosition2 != -1)
                            return element.Substring(0, delimeterPosition2);

                    }
                    return null;
                }
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
//=========================================================================================
        public bool IsFolderPathOnly
        {
            get
            {
                if (_path.EndsWith("/") || _path.EndsWith(@"\") || _path == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
////=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="resourcePath"></param>
/// <returns></returns>
//=========================================================================================
        private bool IsLocalPath(string volume)
        {
            // Temporaily reverse the delimiter for this check...

            List<DriveInfo> drives = DriveInfo.GetDrives().ToList<DriveInfo>();

            drives.DefaultIfEmpty(null);
            if (drives.Where(d => volume.ToUpper().Contains(d.Name)).FirstOrDefault() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//=======================================================================================================
        private PathType GetPathType(string absoluteURI)
        {
            if (absoluteURI.StartsWith("http:") || absoluteURI.StartsWith("https:"))
                return PathType.Remote;
            else
                return PathType.Local;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public string BasePath
        {
            get
            {
                string[] absolutePath = this.AbsoluteURI.Split('/');
                List<string> basePath = new List<string>();

                //for (int i = 0; i < 3; i++) {
                for (int i = 0; i <= absolutePath.Length - 1; i++)
                {
                    basePath.Add(absolutePath[i]);
                }

                return String.Join("/", basePath);
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public string ContainerPath
        {
            get
            {
                return _storageManagementURI + "/" + _volume + "/";
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>-
/// <returns></returns>
//=======================================================================================================
        public string FileName
        {
            get
            {
                if (this.IsFolderPathOnly)
                    return string.Empty;
                else
                    return _fileName;
            }
            set
            {
                string[] tempPath = _path.Split('/');
                tempPath[tempPath.Length - 1] = value;
                _path = String.Join("/", tempPath);
                _fileName = value;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public string Volume
        {
            get
            {
               return _volume;
            }
            set
            {
                _volume = value;
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public string LeafFolder
        {
            get
            {
                string tempPath = null;
                if (this.Path.EndsWith("/") || this.Path.EndsWith(@"\"))
                {
                    tempPath = this.Path.Substring(0, this.Path.Count() - 1 );
                }
                else
                {
                    tempPath = this.Path;
                }

                string[] elements = tempPath.Replace(@"\", "/").Split('/');

                if (elements.Count() == 1)
                {
                    return elements[0];
                }
                else if (elements.Count() == 0)
                {
                    return "/";
                }
                else if (elements.Count() > 1)
                {
                    string leaf = elements[elements.Count() - 1];
                    return leaf + "/";
                }
                return "";
            }
        }
//=======================================================================================================
/// <summary>
/// This is the address to the Object starting AFTER the storageContainer name. If this points to an object, that
/// object name will still be in this path..
/// </summary>
//=======================================================================================================
        public string Path
        {
            get
            {
                if (_path.StartsWith(@"\") || _path.StartsWith("/"))
                {
                    return _path.Substring(1);
                    //return _path;
                }
                else
                {
                    return _path;
                }
            }
        }
//=======================================================================================================
/// <summary>
/// Volume + Path
/// </summary>
//=======================================================================================================
        public string ResourcePath
        {
            get
            {
                if (_pathType == PathType.Remote)

                    if (this.Path == "/")
                    {
                        //return (this.Volume + "/").Replace(@"\", "/");
                        //return ("/").Replace(@"\", "/");
                        return String.Empty;
                    }
                    else
                    {
                        //return (this.Volume + "/" + this.Path).Replace(@"\", "/").Replace("//", "/");                        
                        return (this.Path).Replace(@"\", "/").Replace("//", "/");                        
                        //return this.Path.Replace("/", String.Empty);
                    }
                else
                    //return this.Volume + @"\" + this.Path;
                    return this.Path;
            }
        }
//=======================================================================================================
/// <summary>
/// DNS Address + Volume + Path
/// </summary>
//=======================================================================================================
        public string AbsoluteURI
        {
            get
            {
                if (_pathType == PathType.Remote)
                {
                    //return (Settings.Default.StorageManagementURI + "/" + this.ResourcePath).Replace(@"\", "/").Replace(" ", "_");

                    if (this.ResourcePath.StartsWith("/"))
                        return (_storageManagementURI +  this.ResourcePath).Replace(@"\", "/");
                    else
                        return (_storageManagementURI + "/" + this.ResourcePath).Replace(@"\", "/");
                }
                else
                {
                    return (this.Volume + @"\" + this.Path).Replace(@"\", "/");
                }
            }
        }
//=======================================================================================================
/// <summary>
/// Returns the fully qualified URI in string format.
/// </summary>
/// <returns></returns>
//=======================================================================================================
        public override string ToString()
        {
            return this.AbsoluteURI;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=======================================================================================================
        public Uri ToUri()
        {
            string uri;

            if (this.ResourcePath.StartsWith("/"))
            {
                uri = _storageManagementURI + this.ResourcePath;
                return new Uri(uri);
            }
            else
            {
                uri = _storageManagementURI + "/" + this.ResourcePath;
                return new Uri(uri);
            }
        }
        #endregion
    }
}

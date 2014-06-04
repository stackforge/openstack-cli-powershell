///* ============================================================================
//Copyright 2014 Hewlett Packard

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//============================================================================ */
//using System.Management.Automation;
//using OpenStack;
//using OpenStack.Client.Powershell.Providers.Storage;
//using System;
//using OpenStack.Client.Powershell.Providers.Common;
//using System.IO;
//using OpenStack.Client.Powershell.Utility;
//using OpenStack.Storage;
//using System.Net.Http;

//namespace OpenStack.Client.Powershell.Cmdlets.Common
//{
//    [Cmdlet(VerbsCommon.Copy, "Item", SupportsShouldProcess = true)]
//    //[RequiredServiceIdentifierAttribute(OpenStack.Objects.Domain.Admin.Services.ObjectStorage)]
//    public class CopyItemCmdlet : BasePSCmdlet
//    {
//        public const string cDelimiter = "/";
//        const string cFolderMarker = "folder.txt";
//        private string _sourcePath;
//        private string _targetPath;
//        private SwitchParameter _recursive;
//        private long _bytesCopied = 0;
//        private int _filesCopied = 0;
//        private long _totalBytesCopied = 0;
//        private int _totalFilesCopied = 0;

//        #region Ctors
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        public CopyItemCmdlet()
//        { }
//        #endregion
//        #region Parameters
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        [Parameter(Mandatory = false, ParameterSetName = "aa", ValueFromPipelineByPropertyName = true, HelpMessage = "oih")]
//        [Alias("recurse")]
//        [ValidateNotNullOrEmpty]
//        public SwitchParameter Recursive
//        {
//            get { return _recursive; }
//            set { _recursive = value; }
//        }
////=========================================================================================
///// <summary>
///// The location of the file to copy.
///// </summary>
////=========================================================================================
//        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "aa", ValueFromPipeline = true, HelpMessage = "Help Text")]
//        [ValidateNotNullOrEmpty]
//        public string SourcePath
//        {
//            get { return _sourcePath; }
//            set { _sourcePath = value; }
//        }
////=========================================================================================
///// <summary>
///// The destination of the file to copy.
///// </summary>
////=========================================================================================
//        [Parameter(Position = 2, Mandatory = false, ValueFromPipeline = true, ParameterSetName = "aa", ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
//        [ValidateNotNullOrEmpty]
//        public string TargetPath
//        {
//            get { return _targetPath; }
//            set
//            {
//                if (value.StartsWith(@"/") || value.StartsWith(@"\"))
//                {
//                    _targetPath = value.Substring(1, _targetPath.Length - 1);
//                }
//                else
//                {
//                    _targetPath = value;
//                }
//            }
//        }
//        #endregion
//        #region Properties
////==================================================================================================
///// <summary>
///// 
///// </summary>
////==================================================================================================
//        private string StorageServiceURL
//        {
//            get
//            {
//                if (((ObjectStoragePSDriveInfo)this.Drive).SharePath == null)
//                    return this.Context.ServiceCatalog.GetPublicEndpoint("Object Storage", String.Empty);
//                else
//                    //return ((OSDriveInfo)this.Drive).SharePath;
//                    return ((ObjectStoragePSDriveInfo)this.Drive).SharePath.Replace(this.Drive.Name, string.Empty);
//            }
//        }
//        #endregion
//        #region Methods
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        private void PrintTotals()
//        {
//            Console.ForegroundColor = ConsoleColor.Gray;
//            Console.WriteLine("");
//            Console.WriteLine("--------------------------------------");
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine("Operation Summary");
//            Console.ForegroundColor = ConsoleColor.Gray;
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("");
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine("Total Files Copied : " + Convert.ToString(_totalFilesCopied));
//            Console.WriteLine("Total Bytes Copied : " + Convert.ToString(_totalBytesCopied));
//            Console.WriteLine("");
//            Console.ForegroundColor = ConsoleColor.Green;

//        }
////=================================================================================================
///// <summary>
///// 
///// </summary>
///// <param ---name="path"></param>
///// <returns></returns>
////=================================================================================================
//        private bool IsFolderPath(string path)
//        {
//            if (path.EndsWith(@"\") || path.EndsWith("/"))
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
////=================================================================================================
///// <summary>
///// 
///// </summary>
///// <param name="sourcePath"></param>
///// <param name="targetPath"></param>
///// <returns></returns>
////=================================================================================================
//        private StoragePath CreateValidTargetPath(StoragePath sourcePath, string targetPath)
//        {
//            if (targetPath == null && sourcePath != null)
//            {
//                // The user has supplied a source but no target at all (we should use the current folder)

//                return new StoragePath(this.StorageServiceURL + cDelimiter + this.Drive.Name + cDelimiter + this.Drive.CurrentLocation + cDelimiter + sourcePath.FileName);
//            }
//            else if (System.IO.Path.GetFileName(targetPath) == "" && this.IsFolderPath(targetPath) == true)
//            {
//                // The user supplied a target path but no filename (we will use the Source filename)

//                return this.CreateStoragePath(targetPath + sourcePath.FileName);
//            }
//            else
//            {
//                // The user has supplied a target path and target filename so create the StoragePath against that...

//                return this.CreateStoragePath(targetPath);
//            }
//        }
////=========================================================================================
///// <summary>
///// 
///// </summary>
///// <param name="sender"></param>
///// <param name="e"></param>
////=========================================================================================
//        //private void ListChanged(object sender, CopyOperationInfoEventArgs e)
//        //{
//        //    //if (e.ExceptionMessage == null)
//        //    //{
//        //    //    if (e.Filename != null || e.Filename != string.Empty)
//        //    //    {
//        //    //        Console.WriteLine("Copying file " + e.Filename);
//        //    //        _bytesCopied = _bytesCopied + e.BytesCopied;
//        //    //        ++_filesCopied;
//        //    //        _totalBytesCopied = _totalBytesCopied + e.BytesCopied;
//        //    //        ++_totalFilesCopied;
//        //    //    }
//        //    //}
//        //    //else
//        //    //{
//        //    //    Console.ForegroundColor = ConsoleColor.Red;
//        //    //    Console.WriteLine("Error : " + e.ExceptionMessage);
//        //    //    Console.ForegroundColor = ConsoleColor.Green;
//        //    //}
//        //}
////================================================================================
///// <summary>
///// 
///// </summary>
///// <param name="path"></param>
///// <returns></returns>
////================================================================================
//        private FileStream GetFileStream(StoragePath path)
//        {
//            if (!path.Volume.EndsWith(@"\") && !path.Path.StartsWith(@"\"))
//            {
//                return new FileStream(path.Volume + @"\" + path.Path, FileMode.Open);
//            }
//            else
//            {
//                return new FileStream(path.Volume + path.Path, FileMode.Open);
//            }
//        }
////=================================================================================================
///// <summary>
///// Direct the operation based on the types of paths supplied for both target and source locations.
///// </summary>
////=================================================================================================
//        private void ProcessNonQueuedCopy()
//        {
//            StoragePath sourcePath        = this.CreateStoragePath(this.SourcePath);
//            StoragePath targetPath        = this.CreateValidTargetPath(sourcePath, this.TargetPath);
//            IStorageServiceClient service = this.CoreClient.CreateServiceClient<IStorageServiceClient>();
//            string containerName          = Path.GetFileNameWithoutExtension(this.SourcePath);


//            if (sourcePath.PathType == PathType.Local && targetPath.PathType == PathType.Remote) {
//                service.CreateStorageObject(containerName, targetPath.ResourcePath, null, this.GetFileStream(sourcePath));
//            }



//            //StoragePath sourcePath = this.CreateStoragePath(this.SourcePath);
//            //StoragePath targetPath = this.CreateValidTargetPath(sourcePath, this.TargetPath);
//            //IStorageObjectRepository repository = this.RepositoryFactory.CreateStorageObjectRepository();


//            //if (sourcePath.PathType == OpenStack.Common.PathType.Local && targetPath.PathType == OpenStack.Common.PathType.Remote)
//            //{
//            //    long lastSegment = repository.GetLastSegmentId(targetPath);
//            //    if (lastSegment != 0)
//            //    {
//            //        Console.WriteLine("");
//            //        Console.WriteLine(" You've already uploaded a portion of this file.");
//            //        Console.WriteLine(" Would you like to resume your previous upload? [Y/N]");
//            //        ConsoleKeyInfo response = Console.ReadKey();
//            //        if (response.Key != ConsoleKey.Y)
//            //        {
//            //            repository.DeleteStorageObject(targetPath.AbsoluteURI + @"_temp\");
//            //        }
//            //    }
//            //}

//            //((StorageObjectRepository)repository).Changed += new StorageObjectRepository.CopyOperationCompleteEventHandler(ListChanged);
//            //Console.WriteLine("");
//            //repository.Copy(sourcePath.AbsoluteURI, targetPath.AbsoluteURI, true);
//            //this.PrintTotals();
//            //((StorageObjectRepository)repository).Changed -= new StorageObjectRepository.CopyOperationCompleteEventHandler(ListChanged);
//        }
////=========================================================================================
///// <summary>
///// The main driver..
///// </summary>
////=========================================================================================
//        protected override void ProcessRecord()
//        {
//            ObjectStoragePSDriveInfo drive = this.SessionState.Drive.Current as ObjectStoragePSDriveInfo;
//            this.ProcessNonQueuedCopy();
//        }
//        #endregion
//    }
//}

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
//using System;
//using System.Management.Automation;
//using Openstack.Objects.DataAccess;
//using Openstack.Objects.Domain;
//using Openstack.Client.Powershell.Providers.Storage;
//using Openstack.Common;
//using System.Management.Automation.Host;
//using System.Collections.ObjectModel;
//using System.Diagnostics.Contracts;
//using Openstack.Client.Powershell.Providers.Common;

//namespace Openstack.Client.Powershell.Cmdlets.Common
//{
//    [Cmdlet(VerbsCommon.Remove, "Item", SupportsShouldProcess = true)]
//    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
//    public class RemoveItemCmdlet : BasePSCmdlet
//    {
//        public const string cDelimiter = "/";
//        private string _targetPath;

//        #region Parameters
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
//        [ValidateNotNullOrEmpty]
//        [Alias("t")]
//        public string TargetPath
//        {
//            get { return _targetPath; }
//            set { _targetPath = value; }
//        }
//        #endregion
//        #region Methods
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        private void DeleteFolder(StoragePath targetPath, bool recurse)
//        {
//            Contract.Requires(targetPath.IsFolderPathOnly);
            
//            string confirmationMsg;
//            IStorageObjectRepository repository = null;
//            OSDriveInfo kvsDrive                = null;
//            Collection<ChoiceDescription> choices;

//            #if (DEBUG)
            
//                // We can't prompt for confirmation as this jacks up the unit test..

//                repository = this.RepositoryFactory.CreateStorageObjectRepository();
//                repository.DeleteFolder(targetPath.AbsoluteURI + "/");
//                kvsDrive = (OSDriveInfo)this.SessionState.Drive.Current;

//                if (kvsDrive.PathCache.Exists(p => p == targetPath.Path))
//                {
//                    kvsDrive.PathCache.Remove(targetPath.Path);
//                }
//                return;
         
//            #endif

//            choices = new Collection<ChoiceDescription>();
//            choices.Add(new ChoiceDescription("Y", "Yes"));
//            choices.Add(new ChoiceDescription("N", "No"));

//            if (recurse) confirmationMsg = "You are about to delete an entire folder and all of its decendant folders. Are you sure about this?";
//            else confirmationMsg         =  "You are about to delete a folder and its Storage Objects Are you sure about this?";

//            if (this.Host.UI.PromptForChoice("Confirmation Required." , confirmationMsg, choices, 0) == 0)
//            {
//                repository = this.RepositoryFactory.CreateStorageObjectRepository();
//                repository.DeleteFolder(targetPath.AbsoluteURI);

//                kvsDrive = (OSDriveInfo)this.SessionState.Drive.Current;

//                if (kvsDrive.PathCache.Exists(p => p == targetPath.Path))
//                {
//                    kvsDrive.PathCache.Remove(targetPath.Path);
//                }
//            }
//        }
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        protected override void ProcessRecord()
//        {
//            Contract.Requires(this.Drive != null);
//            StoragePath targetPath = this.CreateStoragePath(this.TargetPath);

//            if (targetPath.IsFolderPathOnly)
//            {
//                this.DeleteFolder(targetPath, true);
//            }
//            else
//            {
//                try
//                {
//                    this.RepositoryFactory.CreateStorageObjectRepository().DeleteStorageObject(targetPath.AbsoluteURI);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("");
//                    Console.WriteLine(ex.Message);
//                    Console.WriteLine("");
//                }
//            }
//        }
//      }
//        #endregion
//    }


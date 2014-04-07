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
using Openstack.Objects.Utility;
using System;
using Openstack.Objects.DataAccess.Storage;
using System.IO;

namespace Openstack.Migrations
{
    public enum StorageProvider
    {
        SkyDrive = 0,
        DropBox = 1
    }

    public class LocalStoreMigration
    {      
        private Session _session;
        public delegate void CopyOperationEventHandler(object sender, CopyOperationInfoEventArgs e);
        public event CopyOperationEventHandler Changed;

////=========================================================================================
///// <summary>
///// 
///// </summary>
///// <param name="e"></param>
////=========================================================================================
        private void OnChanged(CopyOperationInfoEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
        private void Repository_Changed(object sender, CopyOperationInfoEventArgs e)
        {
            this.OnChanged(e);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="accessKeyID"></param>
/// <param name="secretAccessKeyID"></param>
//=========================================================================================
        public LocalStoreMigration()
        {          
            _session = Session.CreateSession();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void CreateContainer(string name)
        {
            _session.Factory.CreateContainerRepository().SaveContainer(new Objects.Domain.StorageContainer(name));
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="localStorePath"></param>
/// <returns></returns>
//=========================================================================================
        private DirectoryInfo[] GetRootDecendants(string localStorePath)
        {
            DirectoryInfo info = new DirectoryInfo(localStorePath);
            return info.GetDirectories("*.*", SearchOption.AllDirectories);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void MigrateDrive(StorageProvider provider, string containerName)
        {
            string localStorePath = null;

            if (provider == StorageProvider.DropBox)
                localStorePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Dropbox\";
            else
                localStorePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Skydrive\";
                       
            string targetPath                  = _session.Context.GetRepositoryContext("object-store").ServiceDescription.Url + "/" + containerName + "/";
            StorageObjectRepository repository = new StorageObjectRepository(_session.Context.GetRepositoryContext("object-store"));

            // Create the target Container..
            
            this.CreateContainer(containerName);

            repository.Changed += new StorageObjectRepository.CopyOperationCompleteEventHandler(Repository_Changed);
            
            DirectoryInfo[] dirs = this.GetRootDecendants(localStorePath);
            foreach (DirectoryInfo direcory in dirs) {
                repository.Copy(localStorePath + direcory.Name + @"\", targetPath, true);
            }

            repository.Changed -= new StorageObjectRepository.CopyOperationCompleteEventHandler(Repository_Changed);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public void MigrateLocalStore(StorageProvider provider)
        {
            this.MigrateLocalStore(provider, provider.ToString()); 
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public void MigrateLocalStore(StorageProvider provider, string containerName)
        {
            MigrateDrive(provider, containerName);
        }
    }
}

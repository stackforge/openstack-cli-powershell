
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
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Collections.ObjectModel;
using System.IO;
using Openstack.Administration.Domain;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using System.Diagnostics.Contracts;
using System.Linq;
using Openstack.Objects.Domain;
using System.Management.Automation.Runspaces;

namespace Openstack.Client.Powershell.Providers.Security
{
    public enum ContainerAxis
    {
        Ancestor   = 0,
        Descendant = 1,
        Neutral    = 3,
        Root       = 4,
        NotFound   = 5
    }

    [CmdletProvider("OS-Cloud", ProviderCapabilities.None)]
    public class CommonServiceProvider : BaseNavigationCmdletProvider
    {      
        #region Provider LifeCycle Support
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        protected override System.Collections.ObjectModel.Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            this.InitializeSession();

            // Check to see if this provider is in the list of returned Services in the Catalog (indicating access is granted)

            PSDriveInfo driveInfo = new PSDriveInfo("OpenStack", this.ProviderInfo, "/", "Openstack Services Provider", null);
            Collection<PSDriveInfo> drives = new Collection<PSDriveInfo>();
            drives.Add(driveInfo);
            return drives;
        }
//==================================================================================================      
/// <summary>
/// 
/// </summary>
/// <param name="providerInfo"></param>
/// <returns></returns>
//==================================================================================================      
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            return base.Start(providerInfo);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="drive"></param>
/// <returns></returns>
//==================================================================================================
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            WriteDebug("Enter : CloudServiceProvider.NewDrive");
            if (drive == null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("drive"), "NullDrive", ErrorCategory.InvalidArgument, drive));
                return null;
            }

            if (drive.Root == null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("drive.Root"), "NullRoot", ErrorCategory.InvalidArgument, drive));
                return null;
            }

            if (drive is CommonDriveInfo)
            {
                return drive;
            }

            var driveParams = this.DynamicParameters as CommonDriveParameters;
            WriteDebug("Exit : CloudServiceProvider.NewDrive");
            return new CommonDriveInfo(drive, driveParams);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        protected override object NewDriveDynamicParameters()
        {
            return new CommonDriveParameters();
        }
        #endregion
        #region Basic Operations (LS, CD xxx, CD.. etc)
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <param name="recurse"></param>
//==================================================================================================
        protected override void GetChildItems(string path, bool recurse)
        {
            WriteContainer(this.Drive.CurrentContainer);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private bool IsParentPath(string path)
        {
            if (Path.GetFileNameWithoutExtension(path) == Path.GetFileNameWithoutExtension(this.Drive.CurrentContainer.Parent.Path))
                return true;
            else
                return false;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//==================================================================================================
        private void SetRoot()
        {
            string defaultTenantId = "TENANTAC1001";
            BaseUIContainer root   = new AccountUIContainer(null, "Account", "The Users assigned Account", @"\\", this.Context, this.RepositoryFactory);
            Account fake           = new Account();
            fake.Name              = "TestAccount";
            fake.Id                = defaultTenantId;
            root.Entity            = fake;

            CurrentAccountUIContainer.CurrentAccount = (AccountUIContainer)root;
            this.Drive.CurrentContainer = root;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="lastPathEntry"></param>
/// <returns></returns>
//==================================================================================================
        private bool IsMovingToContainer(string lastPathEntry)
        {
            bool isMovingToContainer = false;

            if (this.Drive.CurrentContainer.Name == lastPathEntry) isMovingToContainer = true;

            foreach (BaseUIContainer container in this.Drive.CurrentContainer.Containers)
            {
                if (container.Id == lastPathEntry) isMovingToContainer = true;
            }

            return isMovingToContainer;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private ContainerAxis CheckContainerAxis(string path)
        {
            // Do our Root and Neutrality Checks (by Neutral I mean the User has issued LS on the same entity so no movement along the axis).

            if (path == "\\") return ContainerAxis.Root;

            string lastPathEntry = Path.GetFileNameWithoutExtension(path);

            // See if the User is moving back up to the Parent node of the Current Container.           

            if ((this.Drive.CurrentContainer != null && this.Drive.CurrentContainer.Parent != null && lastPathEntry == this.Drive.CurrentContainer.Parent.Id)) return ContainerAxis.Ancestor;
            if (this.IsNamedAncestor(path)) return ContainerAxis.Ancestor;

            // Before we get started verifying the decendant move, make sure the next node is populated with something first..

            if (this.Drive.CurrentContainer != null && this.Drive.CurrentContainer.Containers.Count == 0)
                this.Drive.CurrentContainer.Load();           

            // Finally see if the User is moving downward into a decendant BaseUIContainer from the current one, if not the input supplied is invalid.

            if (lastPathEntry == this.Drive.CurrentContainer.Id) return ContainerAxis.Descendant;

            if (this.IsMovingToContainer(lastPathEntry)) return ContainerAxis.Descendant;
            else
                return ContainerAxis.NotFound;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private bool IsNamedAncestor(string path)
        {
            string lastPathEntry = Path.GetFileNameWithoutExtension(path);

            if (Path.GetFileNameWithoutExtension(path).StartsWith("-") || path == "\\")
            {
                return false;
            }
            else if (this.Drive.CurrentContainer.Parent != null && this.Drive.CurrentContainer.Parent.Name == lastPathEntry)
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
        private string TranslateQuickPickNumber(string path)
        {
            BaseUIContainer result = null;
            int number             = 0;

            if (Int32.TryParse(Path.GetFileName(path), out number))
            {
                if (path == "\\" + this.Drive.CurrentLocation)
                {
                    return path.Replace(Path.GetFileName(path), this.Drive.CurrentContainer.Entity.Id);
                }
                else if (path.Length < this.Drive.CurrentLocation.Length)
                {
                    result = this.Drive.CurrentContainer.Parent;
                }
                else
                {
                    try
                    {
                        result = this.Drive.CurrentContainer.Containers.Where(q => q.Entity.QuickPickNumber == number).FirstOrDefault<BaseUIContainer>();
                    }
                    catch (NullReferenceException ex)
                    {
                        return null;
                    }
                }
                
                
                if (result != null)
                    return path.Replace(Path.GetFileName(path), result.Id);
                else return null;
            }
            else return null;
          
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        private bool IsQuickPickNumber(string path)
        {   
            int test = 0;
            return Int32.TryParse(Path.GetFileName(path), out test);
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
            if (this.IsQuickPickNumber(path))
            {
                string testPath = this.TranslateQuickPickNumber(path);
                if (testPath == null && this.Drive.CurrentContainer.Entity != null)
                {                   
                    path = path.Replace(Path.GetFileName(path), this.Drive.CurrentContainer.Entity.Id);
                }
                else {
                    path = testPath;
                }
            }

            switch (this.CheckContainerAxis(path))
            {
                case (ContainerAxis.Ancestor):

                    // The User has issued "cd .." so we're moving up the Ancestor axis...

                    this.Drive.CurrentContainer = this.Drive.CurrentContainer.Parent;
                    this.SessionState.PSVariable.Set("CurrentContainer", this.Drive.CurrentContainer);
                    return true;

                case (ContainerAxis.Descendant):

                    // The User has moved down the decendant axis either into a Container representing a single Entity or one that represents
                    // a full blown association of entities.

                    this.SetDescendant(path);
                    break;

                case (ContainerAxis.Neutral):

                    // The User has issued LS one the same Container so no movement on the axis..

                    return true;

                case (ContainerAxis.Root):

                    // The User has issued cd\ bringing them all the way back to Root..

                    this.SetRoot();
                    return true;

                case (ContainerAxis.NotFound):

                    return false;
            }

            return true;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private bool SetDescendant(string path)
        {
            if (this.Drive.CurrentContainer != null && this.Drive.CurrentContainer.Containers != null)
            {
                // To clarify, we're looking for the value of the last delimited entry in the path..

                string lastPathEntry = Path.GetFileNameWithoutExtension(path);

                if (lastPathEntry != "" && lastPathEntry != this.Drive.CurrentContainer.Id)
                {
                    BaseUIContainer container = null;
                    container = this.Drive.CurrentContainer.GetContainer(lastPathEntry);
                    this.SessionState.PSVariable.Set("CurrentContainer", container);

                    if (container != null)
                    {
                        // NAVIGATION SUCCESS : The commands value can be matched to an available association so set that node 
                        // as the current container\position..

                        this.Drive.CurrentContainer = container;
                        this.SessionState.PSVariable.Set("CurrentContainer", container);
                        return true;
                    }
                    else
                    {
                        // SHOW ENTITY DETAILS : Check to see if the key supplied by the User matches any Entity that is being managed 
                        // by the current Container. Here, the User has moved (CD) into an Entity and has issued an LS command.

                        if (this.Drive.CurrentContainer.Entity != null && this.Drive.CurrentContainer.IsEntityId(lastPathEntry)) return true;

                        // The User tried to view the details of some entity yet we havn't pulled that Entity into the Container yet.

                        BaseUIContainer entityContainer = this.Drive.CurrentContainer.CreateContainer(lastPathEntry);

                        if (entityContainer != null)
                        {
                            // The Read operation on that Entity succeeded so set that Container as current..

                            this.Drive.CurrentContainer = entityContainer;
                            this.SessionState.PSVariable.Set("CurrentContainer", container);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (this.Drive.CurrentContainer != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="containers"></param>
//==================================================================================================
        private void WriteDecendantContainers(BaseUIContainer container)
        {
            Contract.Assert(container != null);

            if (container.Containers != null && container.Containers.Where(c => c.ObjectType == ObjectType.Container).Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteItemObject("==========================================================================================================", container.Path, false);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(container.Name + " " + "Associations");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteItemObject("==========================================================================================================", container.Path, false);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);


                foreach (BaseUIContainer testContainer in container.Containers)
                {
                    if (testContainer.ObjectType == ObjectType.Container)
                    {
                        WriteItemObject(testContainer, container.Path, false);
                    }
                }
                //container.Containers.ForEach(c => WriteItemObject(c, c.Path, true));                
                WriteItemObject("", container.Path, false);
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="entities"></param>
//==================================================================================================
        private void WriteEntities(BaseUIContainer container)
        {
            //if (container.Entities.Count == 0)
               //container.Load();

            if (container.Entities != null && container.Entities.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteItemObject("", container.Path, false);
                WriteItemObject("==========================================================================================================", container.Path, false);
                Console.ForegroundColor = ConsoleColor.Yellow;
                WriteItemObject("The following " + container.Name + " " + "are available.", container.Path, false);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteItemObject("==========================================================================================================", container.Path, false);
                WriteItemObject("", container.Path, false);
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

                int count = 0;
                foreach (object obj in container.Entities)
                {
                    this.Host.UI.RawUI.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
                  
                    BaseEntity tempObj = obj as BaseEntity;
                    if (tempObj != null)
                    {
                        tempObj.QuickPickNumber = count;
                        WriteItemObject(tempObj, this.Drive.CurrentLocation, false);
                    }
                    else
                    {
                        WriteItemObject(obj, this.Drive.CurrentLocation, false);
                    }
                    ++count;
                }

                WriteItemObject("", this.Drive.CurrentLocation, false);
                WriteItemObject(Convert.ToString(count) + " " + container.Name + "(s) found", this.Drive.CurrentLocation, false);
                WriteItemObject("", this.Drive.CurrentLocation, false);
            }
            else
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="container"></param>
//==================================================================================================
        private void WriteEntity(BaseUIContainer container)
        {
            if (container.ObjectType == ObjectType.Entity)
            {
                container.WriteEntityDetails();
            }
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="container"></param>
//==================================================================================================
        private void WriteUIContainer(BaseUIContainer container)
        {            
            container.Load();

            this.WriteDecendantContainers(container);
            this.WriteEntities(container);
            this.WriteEntity(container);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="container"></param>
//==================================================================================================
        private void WriteEntityContainer(BaseEntityUIContainer container)
        { }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="container"></param>
//==================================================================================================
        private void WriteContainer(BaseUIContainer container)
        {
            this.WriteUIContainer(container);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <param name="returnContainers"></param>
//==================================================================================================
        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            WriteItemObject(path, path, true);
        }
//==================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="path"></param>
/// <returns></returns>
//==================================================================================================
        protected override string GetChildName(string path)
        {
            return base.GetChildName(path);
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
            return true;
        }
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
        #endregion
        #region Properties
//==================================================================================================
/// <summary>
/// 
/// </summary>
//==================================================================================================
        private CommonDriveInfo Drive
        {
            get
            {
                return this.PSDriveInfo as CommonDriveInfo;
            }
        }
        #endregion
    }
}

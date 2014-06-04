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
using System.Text;
using System.Management.Automation;
using OpenStack.Client.Powershell.Utility;
using System.IO;
using OpenStack.Common;
using System.Xml;
using System.Xml.Serialization;
using OpenStack.Client.Powershell.Providers.Storage;
using OpenStack.Client.Powershell.Providers.Common;
using System.Linq;
using System.Collections.ObjectModel;
using System.Management.Automation.Host;
using System.Threading;
using Openstack.Client.Powershell.Utility;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    public class BasePSCmdlet : PSCmdlet
    {
        #region Properties
        ////=========================================================================================
        ///// <summary>
        ///// 
        ///// </summary>
        ////=========================================================================================
        //protected BaseUIContainer CurrentContainer
        //{
        //    get
        //    {
        //        CommonDriveInfo tempDrive = this.Drive as CommonDriveInfo;
        //        if (tempDrive != null)
        //        {
        //            return tempDrive.CurrentContainer as BaseUIContainer;
        //        }
        //        else return null;
        //    }
        //}
        //=========================================================================================
        /// <summary>
        /// Exposes the currently mapped Drive. Belongs in base class???
        /// </summary>
        //=========================================================================================
        protected PSDriveInfo Drive
        {
            get
            {
                return this.SessionState.Drive.Current;
            }
        }
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        //=========================================================================================
        protected void WriteHeaderSection(string headerText)
        {
            WriteObject(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(headerText);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
        }
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
        //==================================================================================================
        protected IOpenStackClient CoreClient
        {
            get
            {
                return (IOpenStackClient)this.SessionState.PSVariable.GetValue("CoreClient", null);
            }
            set
            {
                this.SessionState.PSVariable.Set(new PSVariable("CoreClient", value));
            }
        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //==================================================================================================
         protected override void StopProcessing()
        {
            CancellationTokenSource source = (CancellationTokenSource)this.SessionState.PSVariable.Get("CancellationTokenSource").Value;
            source.Cancel();
        }
      //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        protected Context Context
        {
            get
            {
                return (Context)this.SessionState.PSVariable.GetValue("Context", null);
            }
            set
            {
                this.SessionState.PSVariable.Set(new PSVariable("Context", value));
            }
        }
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //=========================================================================================
        protected string ConfigFilePath
        {
            get
            {               
              return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\OpenStack.config";
            }
        }
        #endregion
        #region Methods
        ////==================================================================================================
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        ////==================================================================================================
        //protected string TranslateQuickPickNumber(string path)
        //{
        //    CommonDriveInfo drive = this.Drive as CommonDriveInfo;
        //    if (drive != null)
        //    {
        //        BaseUIContainer result = null;
        //        int number = 0;

        //        if (Int32.TryParse(Path.GetFileName(path), out number))
        //        {
        //            if (path == "\\" + this.Drive.CurrentLocation)
        //            {
        //                return path.Replace(Path.GetFileName(path), drive.CurrentContainer.Entity.Id);
        //            }
        //            //else if (path.Length < this.Drive.CurrentLocation.Length)
        //            //{
        //            //    result = drive.CurrentContainer.Parent;
        //            //}
        //            else
        //            {
        //                result = drive.CurrentContainer.Containers.Where(q => q.Entity.QuickPickNumber == number).FirstOrDefault<BaseUIContainer>();
        //            }
        //        }
        //        else
        //        {
        //            return path;
        //        }

        //        if (result != null)
        //            return path.Replace(Path.GetFileName(path), result.Id);
        //        else return null;
        //    }
        //    else return null;
        //}
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        protected override void BeginProcessing()
        {
            //if (this.Drive.Name != "OpenStack" && this.Drive.Provider.Name != "OS-Storage")
            //{
            //    ErrorRecord err = new ErrorRecord(new InvalidOperationException("You must be attached to an ObjectStorage Container or the OpenStack drive to execute an OpenStack Cloud cmdlet."), "0", ErrorCategory.InvalidOperation, this);
            //    this.ThrowTerminatingError(err);
            //}

            //bool isAuthorized = false;
            //Type type = this.GetType();
            //object[] metadata = type.GetCustomAttributes(false);
            //bool foundattribute = false;

            //foreach (object attribute in metadata)
            //{
            //    RequiredServiceIdentifierAttribute identifier = attribute as RequiredServiceIdentifierAttribute;

            //    if (identifier != null)
            //    {
            //        if (this.Context.ServiceCatalog.Exists(identifier.Services) != null)
            //            isAuthorized = true;
            //    }
            //}

            //if (isAuthorized == false && foundattribute == false) return;

            //if (!isAuthorized)
            //    this.ThrowTerminatingError(new ErrorRecord(new InvalidOperationException("You're not current authorized to use this service. Please go to https://www.OpenStack.com/ for more information on signing up for this service."), "aa", ErrorCategory.InvalidOperation, this));
        }        
        #endregion
        #region Methods
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //=========================================================================================
      
        protected bool UserConfirmsDeleteAction(string entity)
        {
            Collection<ChoiceDescription> choices = new Collection<ChoiceDescription>();
            choices.Add(new ChoiceDescription("Y", "Yes"));
            choices.Add(new ChoiceDescription("N", "No"));

            if (this.Host.UI.PromptForChoice("Confirm Action", "You are about to delete all " + entity + " in the current container. Are you sure about this?", choices, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        //=========================================================================================
        protected void UpdateCache(Context context)
        {
            //CommonDriveInfo tempDrive = this.Drive as CommonDriveInfo;
            //if (tempDrive != null)
            //{
            //    BaseUIContainer container = tempDrive.CurrentContainer as BaseUIContainer;
            //    container.Context = context;

            //    if (container != null)
            //    {
            //        try
            //        {
            //            container.Load();
            //        }
            //        catch (InvalidOperationException) { }
            //        if (container.Parent != null)
            //            container.Parent.Load();
            //    }
            //}
        }
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        //=========================================================================================
        protected void UpdateCache()
        {
            //CommonDriveInfo tempDrive = this.Drive as CommonDriveInfo;
            //if (tempDrive != null)
            //{
            //    BaseUIContainer container = tempDrive.CurrentContainer as BaseUIContainer;

            //    if (container != null)
            //    {
            //        try
            //        {
            //            container.Load();
            //        }
            //        catch (InvalidOperationException) { }
            //        if (container.Parent != null)
            //            container.Parent.Load();
            //    }
            //}
        }
        //=========================================================================================
        /// <summary>
        /// Updates the cache if the current UIContainer manages the supplied type.
        /// </summary>
        //=========================================================================================
        //protected void UpdateCache<T>() where T : BaseUIContainer
        //{
        //    T container = ((CommonDriveInfo)this.Drive).CurrentContainer as T;

        //    if (container != null)
        //    {
        //        container.Load();
        //    }
        //    else
        //    {
        //        T parentContainer = ((CommonDriveInfo)this.Drive).CurrentContainer.Parent as T;
        //        if (parentContainer != null)
        //        {
        //            parentContainer.Load();
        //        }
        //    }
        //}
        //=========================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        //=========================================================================================
        protected StoragePath CreateStoragePath(string path)
        {
            return ((ObjectStoragePSDriveInfo)this.Drive).CreateStoragePath(path);
        }
        #endregion
    }
}

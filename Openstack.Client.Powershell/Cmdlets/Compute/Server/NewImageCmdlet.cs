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
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Compute;
using System.Linq;
using Openstack.Objects.Domain.Compute.Servers;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Compute;

namespace  Openstack.Client.Powershell.Cmdlets.Compute.Server
{ 
    [Cmdlet(VerbsCommon.New, "Image", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class NewImageCmdlet : BasePSCmdlet
    {
        private string _name;
        private string[] _metadata;
        private string _serverId;
       
        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "NewImage", ValueFromPipelineByPropertyName = true, HelpMessage = "cfgn")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewImage", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("n")]        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "NewImage", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("md")]
        [ValidateNotNullOrEmpty]
        public string[] MetaData
        {
            get { return _metadata; }
            set { _metadata = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="keyValuePairs"></param>
/// <returns></returns>
//=========================================================================================
        public MetaData AddEntries(string[] keyValuePairs)
        {
            MetaData metadata = new MetaData();

            if (keyValuePairs != null && keyValuePairs.Count() > 0)
            {
                foreach (string kv in keyValuePairs)
                {
                    char[] seperator        = { '|' };
                    string[] temp           = kv.Split(seperator);
                    MetaDataElement element = new MetaDataElement();
                    element.Key             = temp[0];
                    element.Value           = temp[1];

                    metadata.Add(temp[0], temp[1]);
                }
                return metadata;
            }
            return null;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="keyValuePairs"></param>
/// <returns></returns>
//=========================================================================================
        public MetaData ReformatMetadata(string[] keyValuePairs)
        {
            MetaData metadata = new MetaData();

            if (keyValuePairs != null && keyValuePairs.Count() > 0)
            {
                foreach (string kv in keyValuePairs)
                {
                    char[] seperator        = { '|' };
                    string[] temp           = kv.Split(seperator);
                    MetaDataElement element = new MetaDataElement();
                    element.Key             = temp[0];
                    element.Value           = temp[1];

                    metadata.Add(temp[0], temp[1]);
                }
                return metadata;
            }
            return null;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        { 
            MetaData md    = null;
            md             = this.ReformatMetadata(this.MetaData);   
            NewImage image = new NewImage();
            image.MetaData = md;
            image.Name     = this.Name;

            if (this.ServerId != null)
            {  
                image.ServerId = this.ServerId;
                this.RepositoryFactory.CreateImageRepository().SaveImage(image);
                this.UpdateCache<ImageUIContainer>();             
            }
            else
            {
                BaseUIContainer currentContainer = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;
          
                if (currentContainer.Name == "Metadata")
                {
                    ServerUIContainer serverContainer = currentContainer.Parent as ServerUIContainer;

                    if (serverContainer != null) 
                    {
                        image.ServerId = serverContainer.Entity.Id;
                        this.RepositoryFactory.CreateImageRepository().SaveImage(image);
                        this.UpdateCache<ImagesUIContainer>();   
                    }
                }
                else {

                    image.ServerId = currentContainer.Entity.Id;
                    this.RepositoryFactory.CreateImageRepository().SaveImage(image);
                    this.UpdateCache<ImagesUIContainer>();   
                }
            }            
        }
        #endregion
    }
}



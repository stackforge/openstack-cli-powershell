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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Security;
using System.Collections.Generic;
using System;
using Openstack.Objects.DataAccess.Compute;
using System.Threading;

namespace  Openstack.Client.Powershell.Cmdlets.Compute.Server
{ 
    [Cmdlet(VerbsCommon.New, "Server", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class NewServerCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _imageId;
        private string _flavorId;
        private string _password;
        private string _accessIPv4;
        private string _accessIPv6;
        private string _keyName;
        private string[] _metadata;
        private string[] _securityGroups;
        private bool _useWizard = false;
        private string[] _networksIds;
        private string _availabilityZone;       

        #region Parameters 
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewServerPS", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
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
        [Parameter(Position = 1, ParameterSetName = "NewServerPS", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "A valid reference to an Image.")]
        [Alias("i")]
        [ValidateNotNullOrEmpty]
        public string ImageRef
        {
            get { return _imageId; }
            set { _imageId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, ParameterSetName = "NewServerPS", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "A reference to a valid Flavor.Image")]
        [Alias("f")]
        [ValidateNotNullOrEmpty]
        public string FlavorRef
        {
            get { return _flavorId; }
            set { _flavorId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, ParameterSetName = "NewServerPS", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "ipv4 address for remote access.")]
        [Alias("ip4")]
        public string AccessIPv4
        {
            get { return _accessIPv4; }
            set { _accessIPv4 = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 4, ParameterSetName = "NewServerPS", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "ipv6 address for remote access.")]
        [Alias("ip6")]
        public string AccessIPv6
        {
            get { return _accessIPv6; }
            set { _accessIPv6 = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 5, Mandatory = false, ParameterSetName = "NewServerPS", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("md")]
        [ValidateNotNullOrEmpty]
        public string[] MetaData
        {
            get { return _metadata; }
            set { _metadata = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 6, ParameterSetName = "NewServerPS", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("k")]
        [ValidateNotNullOrEmpty]
        public string KeyName
        {
            get { return _keyName; }
            set { _keyName = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 7, Mandatory = false, ParameterSetName = "NewServerPS", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("sg")]
        [ValidateNotNullOrEmpty]
        public string[] SecurityGroups
        {
            get { return _securityGroups; }
            set { _securityGroups = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 8, Mandatory = true, ParameterSetName = "NewServerPS", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("nid")]
        [ValidateNotNullOrEmpty]
        public string[] NetworksIds
        {
            get { return _networksIds; }
            set { _networksIds = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 9, ParameterSetName = "NewServerPS", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("az")]
        public string AvailabilityZone
        {
            get { return _availabilityZone; }
            set { _availabilityZone = value; }
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
//=========================================================================================
        private List<SecurityGroupAssignment> GetSecurityGroups()
        {
            List<SecurityGroupAssignment> assignments = new List<SecurityGroupAssignment>();

            if (_securityGroups != null)
            {
                foreach (string sg in _securityGroups)
                {
                    assignments.Add(new SecurityGroupAssignment(sg));
                }
            }

            return assignments;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private bool IsWindowsImage(string imageId)
        {
            Image image = this.RepositoryFactory.CreateImageRepository().GetImage(imageId);
            return image.IsWindowsImage;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private List<NetworkId> GetNetworkIDs()
        {
            if (this.NetworksIds != null)
            {
                List<NetworkId> ids = new List<NetworkId>();
                foreach (string id in this.NetworksIds)
                {
                    NetworkId uuid = new NetworkId(id);
                    ids.Add(uuid);
                }
                return ids;
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
        private void SaveServer()
        {
            NewServer server        = new NewServer();
            server.Name             = this.Name;
            server.ImageRef         = this.ImageRef;
            server.FlavorRef        = this.FlavorRef;
            server.AccessIPv6       = this.AccessIPv6;
            server.AccessIPv4       = this.AccessIPv4;
            server.MetaData         = this.AddEntries(this.MetaData);
            server.KeyName          = this.KeyName;
            server.SecurityGroups   = this.GetSecurityGroups();
            server.Networks         = this.GetNetworkIDs();
            server.AvailabilityZone = this.AvailabilityZone;

            if (IsWindowsImage(this.ImageRef))
            {
                WindowsInstanceBuilder builder = new WindowsInstanceBuilder(this.RepositoryFactory, this.Settings);
                builder.Changed += new Openstack.Objects.DataAccess.Compute.WindowsInstanceBuilder.CreateInstanceEventHandler(BuilderEvent);
                builder.CreateInstance(server);
                builder.Changed -= new Openstack.Objects.DataAccess.Compute.WindowsInstanceBuilder.CreateInstanceEventHandler(BuilderEvent);
                this.UpdateCache();
            }
            else
            {
                NonWindowsInstanceBuilder nonWIBuilder = new NonWindowsInstanceBuilder(this.RepositoryFactory, this.Settings);
                nonWIBuilder.Changed += new Openstack.Objects.DataAccess.Compute.NonWindowsInstanceBuilder.CreateInstanceEventHandler(BuilderEvent);
                nonWIBuilder.CreateInstance(server, _keyName);
                nonWIBuilder.Changed -= new Openstack.Objects.DataAccess.Compute.NonWindowsInstanceBuilder.CreateInstanceEventHandler(BuilderEvent);
                this.UpdateCache();
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
        private void BuilderEvent(object sender, CreateInstanceEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private NewServer ShowNewServerWizard()
        {
            return null;
        }     
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            this.SaveServer();
        }
        #endregion
    }
}


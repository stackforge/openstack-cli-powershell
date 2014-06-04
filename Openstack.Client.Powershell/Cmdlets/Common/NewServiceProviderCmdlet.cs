//* ============================================================================
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
using System.Management.Automation;
using System.Xml.Linq;
using System.Collections.Generic;
using OpenStack.Client.Powershell.Utility;
using System.Xml.XPath;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.New, "SP", SupportsShouldProcess = true)]
    //[RequiredServiceIdentifierAttribute(OpenStack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class NewServiceProvidersCmdlet : BasePSCmdlet
    {

        private string _name = "";
        private bool _isDefault = false;
        private string _authenticationServiceURI = "";
        private string _username = "";
        private string _password = "";
        private string _defTenantId = "";
        
        #region Parameters

        [Parameter(Position = 0, ParameterSetName = "NewSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Parameter(Position = 1, ParameterSetName = "NewSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("url")]
        public string AuthenticationServiceURI
        {
            get { return _authenticationServiceURI; }
            set { _authenticationServiceURI = value; }
        }
        [Parameter(Position = 2, ParameterSetName = "NewSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("un")]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        [Parameter(Position = 3, ParameterSetName = "NewSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("p")]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        [Parameter(Position = 4, ParameterSetName = "NewSP", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("t")]
        public string DefTenantId
        {
            get { return _defTenantId; }
            set { _defTenantId = value; }
        }
        [Parameter(Position = 4, ParameterSetName = "NewSP", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [Alias("d")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter IsDefault
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }

        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="value"></param>
//=========================================================================================
        private XElement CreateAddElement(string name, string value)
        {
            XElement element = new XElement("add");
            element.SetAttributeValue("key", name);
            element.SetAttributeValue("value", value);
            return element;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string isDef = "False";
            if (IsDefault) {
                isDef = "True";                
            }
            else {
                isDef = "False";
            }

            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            XDocument doc                          = XDocument.Load(this.ConfigFilePath);
            XElement element                       = new XElement("ServiceProvider", new XAttribute("name", this.Name), new XAttribute("isDefault", isDef));

            element.Add(this.CreateAddElement("AuthenticationServiceURI", this.AuthenticationServiceURI));
            element.Add(this.CreateAddElement("Username", this.Username));
            element.Add(this.CreateAddElement("Password", this.Password));
            element.Add(this.CreateAddElement("DefaultTenantId", this.DefTenantId));

            doc.XPathSelectElement("configuration/appSettings/IdentityServices").Add(element);
            doc.Save(this.ConfigFilePath);
            this.WriteObject("");
            this.WriteObject("New Serviced Provider " + this.Name + " created!");
            this.WriteObject("");
        }
        #endregion
    }
}


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
using Openstack.Objects.DataAccess.Security;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Domain.Compute.Servers.Actions;
using System;
using Openstack.Objects.DataAccess.Compute;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet(VerbsCommon.Get, "Password", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class GetPasswordCmdlet : BasePSCmdlet
    {
        private string _administratorPassword;
        private string _serverId;
        private string _keyName;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "GetPassword", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(Position = 0, ParameterSetName = "GetPassword", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("s")]
        [ValidateNotNullOrEmpty]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================
        private string GetServerId()
        {
            BaseUIContainer currentContainer = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;

            if (currentContainer.Name == "Metadata")
            {
                ServerUIContainer serverContainer = currentContainer.Parent as ServerUIContainer;
                if (serverContainer != null)
                    return serverContainer.Entity.Id;
            }
            else
            {
                return currentContainer.Entity.Id;
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
            if (this.ServerId == null)
                this.ServerId = this.GetServerId();

            IServerRepository repository = this.RepositoryFactory.CreateServerRepository();              
            GetServerLogAction action    = new GetServerLogAction();
            action.ServerId              = this.ServerId;
            Log log                      = repository.GetServerLog(action);

            if (log != null && log.Content.Length > 14)
            {
                Console.WriteLine("");
                Console.WriteLine("Log detected!");
                string pw = log.ExtractAdminPassword(this.Settings.KeyStoragePath + @"\OS\" + _keyName + ".pem");
                Console.WriteLine("Administrator Password : " + pw);
                Console.WriteLine("");

                if (this.Settings.EnableCredentialTracking == true)
                {
                    CredentialListManager manager = new CredentialListManager(this.Settings);
                    manager.SetPassword(this.ServerId, pw);
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Unfortunately we couldn't retrieve the Instance Log. If this situation persist, we recommend that you delete and recreate the instance.");
                Console.WriteLine("");
            }
        }
        #endregion
    }
}

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
using Openstack.Objects.Domain.Compute.Servers;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.DataAccess.Security;
using System;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet(VerbsCommon.Reset, "Password", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class ResetPasswordCmdlet : BasePSCmdlet
    {
        private string _administratorPassword;
        private string _serverId;      

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "ResetPassword", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("id")]
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
        [Parameter(Position = 0, ParameterSetName = "ResetPassword", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("p")]
        [ValidateNotNullOrEmpty]
        public string AdministratorPassword
        {
            get { return _administratorPassword; }
            set { _administratorPassword = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ChangeWindowsImagePW()
        {
            CredentialListManager manager = new CredentialListManager(this.Settings);
            if (this.Settings.EnableCredentialTracking == true)
            {
                manager.SetPassword(this.ServerId, this.AdministratorPassword);
                Console.WriteLine("");
                Console.WriteLine("Password Updated.");
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("You're attempting to update the password for a Windows Image but EnableCredentialTracking is currently turned off. To turn this on please use the Get-config and Set-config cmdlets. Operation failed.");
                Console.WriteLine("");
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ChangeNonWindowsImagePW()
        {
            ChangePasswordAction action  = new ChangePasswordAction();
            action.AdministratorPassword = this.AdministratorPassword;
            action.ServerId              = this.ServerId;

            this.RepositoryFactory.CreateServerRepository().ChangePassword(action);
            Console.WriteLine("");
            Console.WriteLine("Password Changed.");
            Console.WriteLine("");
        }
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
//=========================================================================================
        protected override void ProcessRecord()
        {
            if (this.ServerId == null)
                this.ServerId = this.GetServerId();

            Openstack.Objects.Domain.Compute.Server server = this.RepositoryFactory.CreateServerRepository().GetServer(this.ServerId);
            
            if (this.IsWindowsImage(server.Image.Id)) {
                ChangeWindowsImagePW();
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Invalid Server : The Server you supplied is not a based on a Windows image. We currently only support key based authentication for non-Windows images.");
                Console.WriteLine("");
            }         
        }
        #endregion
    }
}

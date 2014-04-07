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
using Openstack.Objects.DataAccess.Compute;
using Openstack.Objects.DataAccess.Security;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.Domain.Compute.Servers.Actions;
using Openstack.Objects.Domain.Compute.Servers;
using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet("Connect", "Server", SupportsShouldProcess = true)]
    public class ConnectServerCmdletd : BasePSCmdlet
    {
        private string _serverId;
        private string _keypairName = null;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "ConnectServerPS", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("s")]
        public string ServerId
        {
            get
            { 
                if (_serverId != null)
                    return _serverId; 
                else
                    {
                        BaseUIContainer currentContainer  = this.SessionState.PSVariable.Get("CurrentContainer").Value as BaseUIContainer;               
                        ServerUIContainer serverContainer = currentContainer as ServerUIContainer;
                
                        if (serverContainer != null) {
                            return serverContainer.Entity.Id;
                        }               
                    }
                return null;
            
            }
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
        private string GetAdminPassword(string keyname)
        {
            GetServerLogAction action = new GetServerLogAction();
            action.ServerId = this.ServerId;
            Log log = this.RepositoryFactory.CreateServerRepository().GetServerLog(action);

            if (log != null)
            {
                Console.WriteLine("");
                string pw = log.ExtractAdminPassword(this.Settings.KeyStoragePath + keyname + ".pem");
                Console.WriteLine("Administrator Password : " + pw);
                return pw;
            }
            else
                return null;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ShowCredentialsNotFoundMsg()
        {
            Console.WriteLine("");
            Console.WriteLine("Credentials not found. You'll have to manually enter your Administrator credentials for this instance.");
            Console.WriteLine("If you want to continue to use Automatic Credential Tracking for this instance make sure that you update");
            Console.WriteLine("our records with the new password. To do this, use the Reset-Password cmdlet.");
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private void WriteSection(string headerText)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("==============================================================================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(headerText);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("==============================================================================================================================================");
            Console.ForegroundColor = oldColor;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//===================================================================================K=====
        private void ShowKeyBasedLoginInstructions(string keyname)
        {
            if (keyname == null) keyname = "YourKeyfile";

            this.WriteSection("Unable to find a suitable Putty key file!");
            Console.WriteLine("1. Make sure that you have downloaded the latest version of Putty and PuttyGen for Windows.");
            Console.WriteLine("2. Tell us where Putty.exe exist by setting the Config file entry SSHClientPath (use Set-Config)");
            Console.WriteLine("3. Tell PuttyGen to Load an existing key file. This file will be located at " + this.Settings.KeyStoragePath + @"\OS\" + keyname + ".pem");
            Console.WriteLine("4. Convert the .pem file to the .ppk format that Putty.exe understands by clicking the Save Private Key button.");
            Console.WriteLine("5. Save the .ppk file as " + keyname + ".ppk");
            Console.WriteLine("6. Launch the SSH session with the Connect-Server cmdlet");
            Console.WriteLine("7. HINT : If you use the same keypair name for each Server you create you can skip steps 1-5 after the steps have been completed once.");
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private bool LaunchPasswordBasedSSHSession(Openstack.Objects.Domain.Compute.Server server)
        {
            CredentialListManager manager = new CredentialListManager(this.Settings);
            SSHClient sshClient           = new SSHClient(this.Settings);
            sshClient.Username            = "ubuntu";
            sshClient.Address             = server.Addresses.Private[1].Addr;
            sshClient.Password            = manager.GetPassword(server.Id);

            if (sshClient.Password == null) {
                return false;
            }
            else {
                sshClient.LaunchClient();
                return true;
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private bool IsSSHClientConfigured()
        {
            if (this.Settings.SSHClientPath == string.Empty)
                return false;
            else
                return File.Exists(this.Settings.SSHClientPath);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private bool DoesPPKExist(string keyName)
        {          
            return File.Exists(this.Settings.KeyStoragePath + @"\OS\" + keyName + ".ppk");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void SaveSSHClientLocation(string path)
        {
            string configFilePath   = this.ConfigFilePath;
            XDocument doc           = XDocument.Load(configFilePath);
            XElement clientPathNode = doc.XPathSelectElement("//add[@key='SSHClientPath']");

            clientPathNode.SetAttributeValue("value", path);
            doc.Save(configFilePath);
            this.Settings.SSHClientPath = path;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ConfigureSSHClient()
        {
            Console.WriteLine("");
            Console.WriteLine("You're trying to establish a remote session with a non-Windows based server. This requires a fully qualified path to the SSH client (putty.exe)");
            Console.Write("SSH Client Path : ");
             string path = Console.ReadLine();

            if (File.Exists(path))
            {
                Console.WriteLine("");
                Console.WriteLine("Putty.exe found! Saving location for future sessions..");
                Console.WriteLine("");
                this.SaveSSHClientLocation(path);
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Putty.exe not found! Invalid path supplied.");
                Console.WriteLine("");
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void LaunchSSHSession(Openstack.Objects.Domain.Compute.Server server)
        {
            if (!IsSSHClientConfigured()) {
                this.ConfigureSSHClient();
            }

            if (server.KeyName != null && this.DoesPPKExist(server.KeyName))
            {
                SSHClient sshClient   = new SSHClient(this.Settings);
                sshClient.Username    = "ubuntu";
                sshClient.KeypairName = server.KeyName;
                sshClient.Address     = server.Addresses.Private[1].Addr;

                sshClient.LaunchClient();
            }
            else
            {               
                this.ShowKeyBasedLoginInstructions(server.KeyName);
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void LaunchWindowsSession(Openstack.Objects.Domain.Compute.Server server)
        {
            CredentialListManager manager = new CredentialListManager(this.Settings);
            RDPClient client              = new RDPClient();
            client.Address                = server.Addresses.Private[1].Addr;
            client.Username               = "Administrator";

            if (this.Settings.EnableCredentialTracking == true)
            {
                client.Password = manager.GetPassword(server.Id);
                if (client.Password == null)
                {
                    this.ShowCredentialsNotFoundMsg();
                    return;
                }
            }
            client.LaunchClient();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string id = this.TranslateQuickPickNumber(this.ServerId);
            if (id != null)
            {
                Openstack.Objects.Domain.Compute.Server server = this.RepositoryFactory.CreateServerRepository().GetServer(id);
                if (server != null)
                {
                    if (IsWindowsImage(server.Image.Id))
                    {
                        this.LaunchWindowsSession(server);
                    }
                    else
                    {
                        this.LaunchSSHSession(server);
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Server not found.");
                    Console.WriteLine("");
                }
            }           
        }
        #endregion
    }
}


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
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Security;
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Openstack.Objects.Domain.Server;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Network
{
    [Cmdlet(VerbsCommon.New, "KeyPair", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class NewKeyPairCmdlet : BasePSCmdlet
    {
        private string _name;
        private string _filename = null;
        private SwitchParameter _copyToClipboard = false;   
   
        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        //[Parameter(Position = 2, ParameterSetName = "NewKeypair", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "a")]
        //[Alias("c")]
        //public SwitchParameter CopyToClipboard
        //{
        //    get { return _copyToClipboard; }
        //    set { _copyToClipboard = value; }
        //}
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "NewKeypair", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "a")]
        [Alias("n")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void StoreKey(string privateKey)
        {

        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ExportKey(string privateKey, string filename)
        {
           // Save the key locally first..
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(privateKey);

            using (StreamWriter outfile = new StreamWriter(this.Settings.KeyStoragePath + @"\OS\" + filename, false))
            {
                outfile.Write(builder.ToString());
            }

            // Now export with PuttyGen

            //Process.Start(this.Settings.PuttuGenPath, this.Settings.KeyStoragePath + @"\TempKey.key -O private -o " + this.Settings.KeyStoragePath + "testKey.ppk" + "-q");
            //int y = 9;
           
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {          
            NewKeyPair nkp          = new NewKeyPair();
            nkp.Name                = this.Name;
            KeyPair keypair         = this.RepositoryFactory.CreateKeyPairRepository().SaveKeyPair(nkp);
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);

            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("New keypair created.");
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), this.Context.Forecolor);
            Console.WriteLine("");
            Console.WriteLine("User Id     : " + keypair.UserId);
            Console.WriteLine("Name        : " + keypair.Name);
            Console.WriteLine("Private Key : ");
            Console.WriteLine("");
            Console.WriteLine(keypair.PrivateKey);
            Console.WriteLine("");
            Console.WriteLine("Public Key  : ");
            Console.WriteLine("");
            Console.WriteLine(keypair.PublicKey);
            Console.WriteLine("");
            Console.WriteLine("Fingerprint : " + keypair.Fingerprint);
            Console.WriteLine("");
                  
            this.ExportKey(keypair.PrivateKey, keypair.Name + ".pem");

            if (_copyToClipboard)
            {
                OutClipboard.SetText(keypair.PrivateKey);
            }

            Console.WriteLine("Exporting Private Key.");
            Console.WriteLine("Storing Key.");
            Console.WriteLine("Key Generation and storage complete.");
            Console.WriteLine("");
            this.UpdateCache<KeyPairsUIContainer>();
        }
        #endregion
    }
}

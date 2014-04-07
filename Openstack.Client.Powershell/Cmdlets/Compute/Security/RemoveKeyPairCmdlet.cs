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
using System;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Client.Powershell.Providers.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Network
{
    [Cmdlet(VerbsCommon.Remove, "KeyPair", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class RemoveKeyPairCmdlet : BasePSCmdlet
    {
        private string _name;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "RemoveKeyPair", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "a")]
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
        protected override void ProcessRecord()
        {
            try
            {
                this.RepositoryFactory.CreateKeyPairRepository().DeleteKeyPair(this.Name);
                Console.WriteLine("");
                Console.WriteLine("Keypair " + this.Name + " removed.");
                Console.WriteLine("");
                this.UpdateCache<KeyPairsUIContainer>();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error : Keypair " + this.Name + " not found.");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
        #endregion
    }
}

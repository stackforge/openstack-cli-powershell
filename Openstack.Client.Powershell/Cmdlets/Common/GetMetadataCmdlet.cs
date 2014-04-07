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
using System.Collections.Generic;
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using System;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain.Admin;

namespace Openstack.Client.Powershell.Cmdlets.Storage.CDN
{
    [Cmdlet(VerbsCommon.Get, "Metadata", SupportsShouldProcess = true)]    
    public class GetNMetadataCmdlet2 : BasePSCmdlet
    {
        private string _cdnContainerName;
        private string _sourcePath;
        private string _objectStorageContainerName;
        private string _serverId;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "serverMetadata", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
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
        [Parameter(ParameterSetName = "containerMetadata", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("cn")]
        [ValidateNotNullOrEmpty]
        public string ObjectStorageContainerName
        {
            get { return _objectStorageContainerName; }
            set { _objectStorageContainerName = value; }
        }
//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(Position = 0 , ParameterSetName = "objectMetadata",  Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("sp")]
        [ValidateNotNullOrEmpty]
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Mandatory = false, ParameterSetName = "cdnMetadata", ValueFromPipelineByPropertyName = true, HelpMessage = "The Name of the Container to enable for CDN access.")]
        [Alias("cdn")]
        [ValidateNotNullOrEmpty]
        public string CDNContainerName
        {
            get { return _cdnContainerName; }
            set { _cdnContainerName = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private void WriteSection(string headerText)
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
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="kvps"></param>
//=========================================================================================
        private void WriteKVPs(List<KeyValuePair<string, string>> kvps, string displayName)
        {
            if (kvps.Count > 0)
            {
                WriteObject("");
                this.WriteSection("Meta-Data for " + displayName + " is as follows.");
                WriteObject("");
                foreach (KeyValuePair<string, string> kvp in kvps)
                {
                    WriteObject("Key   = " + kvp.Key.Replace("X-", string.Empty));
                    WriteObject("Value = " + Convert.ToString(kvp.Value));
                    WriteObject("---------------------------------");
                }
                WriteObject("");
            }
            else
            {
                WriteObject("");
                Console.WriteLine("No meta-data found for the supplied resource name.");
                WriteObject("");
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessCDNMetadata()
        {
            if (this.Context.ServiceCatalog.DoesServiceExist(Services.CDN) == false) {
                Console.WriteLine("You don't have access to CDN services under this account. For information on signing up for CDN access please go to http://Openstack.com/.");
            }
            WriteKVPs(RepositoryFactory.CreateCDNRepository().GetMetaData(this.CDNContainerName), this.CDNContainerName);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessObjectMetadata()
        {
            if (this.Drive.Provider.Name != "OS-Storage")
                Console.WriteLine("You must be using the Object Storage Provider to use this cmdlet with the supplied parameters. To use this provider mount to a ObjectStorage container first. To see a list of available Containers issue the Get-PSDrive command.");
            else
                WriteKVPs(RepositoryFactory.CreateStorageObjectRepository().GetMetaData(this.CreateStoragePath(this.SourcePath).AbsoluteURI), this.SourcePath);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessContainerMetadata()
        {
            if (this.Drive.Provider.Name != "OS-Storage")
                Console.WriteLine("You must be using the Object Storage Provider to use this cmdlet with the supplied parameters. To use this provider mount to a ObjectStorage container first. To see a list of available Containers issue the Get-PSDrive command.");
            else
                WriteKVPs(RepositoryFactory.CreateContainerRepository().GetMetaData(_objectStorageContainerName), _objectStorageContainerName);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void ProcessServerMetadata()
        {
            if (this.Context.ServiceCatalog.DoesServiceExist(Services.Compute) == false)
                Console.WriteLine("You don't have access to Compute services under this account. For information on signing up for this please go to http://Openstack.com/.");
            else
            {
                Server server = RepositoryFactory.CreateServerRepository().GetServer(this.ServerId);
                //WriteKVPs(server.MetaData.ToKeypairs(), server.Name);
            }
        }
//=========================================================================================
/// <summary>
/// The main driver..
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            switch (this.ParameterSetName)
            {
                case ("serverMetadata"):
                    this.ProcessServerMetadata();
                    break;

                case ("containerMetadata"):
                    this.ProcessContainerMetadata();
                    break;

                case ("cdnMetadata"):

                    this.ProcessCDNMetadata();
                    break;

                case ("objectMetadata"):
                    this.ProcessObjectMetadata();
                    break;
            }
        }
    }
}


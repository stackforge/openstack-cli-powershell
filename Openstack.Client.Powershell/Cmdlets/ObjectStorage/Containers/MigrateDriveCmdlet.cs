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
using Openstack.Objects.DataAccess.Storage;
using Openstack.Migrations;
using System;
using Openstack.Client.Powershell.Providers.Storage;
using Openstack.Client.Powershell.Providers.Common;

namespace Openstack.Client.Powershell.Cmdlets.Containers
{
    [Cmdlet("Migrate", "Drive",  SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class MigrateDriveCmdlet : BasePSCmdlet
    {
        private string _accessKeyID;
        private string _secretAccessKeyID;
        private long _bytesCopied = 0;
        private int _filesCopied = 0;
        private long _totalBytesCopied = 0;
        private int _totalFilesCopied = 0;
        private string[] _buckets;
        private string _provider;
       
        #region Parameters

//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "localStore", Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = " ")]
        [ValidateSet("Skydrive", "Dropbox", "S3")]
        [Alias("p")]
        [ValidateNotNullOrEmpty]
        public string Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "localStore", ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("ak")]
        [ValidateNotNullOrEmpty]
        public string AccessKeyId
        {
            get { return _accessKeyID; }
            set { _accessKeyID = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 2, ParameterSetName = "localStore", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("sk")]
        [ValidateNotNullOrEmpty]
        public string SecretAccessKeyId
        {
            get { return _secretAccessKeyID; }
            set { _secretAccessKeyID = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "localStore", ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text")]
        [Alias("b")]
        [ValidateNotNullOrEmpty]
        public string[] Buckets
        {
            get { return _buckets; }
            set { _buckets = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void PrintTotals()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("");
            Console.WriteLine("--------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation Summary");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Total Files Copied : " + Convert.ToString(_totalFilesCopied));
            Console.WriteLine("Total Bytes Copied : " + Convert.ToString(_totalBytesCopied));
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;

        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void MigrateS3Drive()
        {
            //AWSMigration migration = new AWSMigration("AKIAJ6SAONGOGCUKSONA", "0Hi00F7zlFwGi8a45qRhGfW2Btf+FAioZhfD+99K");
            AWSMigration migration = new AWSMigration(_accessKeyID, _secretAccessKeyID);
            if (_buckets == null) _buckets = migration.GetBuckets();

            migration.Changed += new Openstack.Migrations.AWSMigration.CopyOperationEventHandler(ListChanged);
            migration.ContainerCreated += new Openstack.Migrations.AWSMigration.CreateContainerOperationEventHandler(OnCreateContainer);

            Console.WriteLine("");
            foreach (string bucketName in _buckets)
            {
                _bytesCopied = 0;
                _filesCopied = 0;

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("");
                Console.WriteLine("--------------------------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Processing Bucket : " + bucketName);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Green;
                migration.MigrateBucket(bucketName);
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Files Copied : " + Convert.ToString(_filesCopied));
                Console.WriteLine("Bytes Copied : " + Convert.ToString(_bytesCopied));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("");
            }

            this.PrintTotals();
            migration.Changed -= new Openstack.Migrations.AWSMigration.CopyOperationEventHandler(ListChanged);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void MigrateLocalDrive()
        {
            LocalStoreMigration migration = new LocalStoreMigration();

            migration.Changed += new Openstack.Migrations.LocalStoreMigration.CopyOperationEventHandler(ListChanged);
            Console.WriteLine("");
           
            _bytesCopied = 0;
            _filesCopied = 0;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("");
            Console.WriteLine("--------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Migrating local " + _provider + " store.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;

            if (_provider == "Dropbox")
                migration.MigrateLocalStore(StorageProvider.DropBox);
            else
                migration.MigrateLocalStore(StorageProvider.SkyDrive);
            
            this.PrintTotals();
            migration.Changed -= new Openstack.Migrations.LocalStoreMigration.CopyOperationEventHandler(ListChanged);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
        private void OnCreateContainer(object sender, CreateContainerOperationInfoEventArgs e)
        {
            PSDriveInfo psDriveInfo             = new PSDriveInfo(e.ContainerName, this.Drive.Provider, "/", "", null);
            OSDriveParameters driveParameters = new OSDriveParameters();
            driveParameters.Settings            = this.Settings;
            try
            {
                this.SessionState.Drive.New(new OSDriveInfo(psDriveInfo, driveParameters, this.Context), "local");
            }
            catch (SessionStateException ex) { }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
//=========================================================================================
      private void ListChanged(object sender, CopyOperationInfoEventArgs e) 
      {
          if (e.ExceptionMessage == null)
          {
              if (e.Filename != null || e.Filename != string.Empty)
              {
                  Console.WriteLine("Copying file " + e.Filename);
                  _bytesCopied = _bytesCopied + e.BytesCopied;
                  ++_filesCopied;
                  _totalBytesCopied = _totalBytesCopied + e.BytesCopied;
                  ++_totalFilesCopied;
              }
          }
          else
          {
              Console.ForegroundColor = ConsoleColor.Red;
              Console.WriteLine("Error : " + e.ExceptionMessage);
              Console.ForegroundColor = ConsoleColor.Green;
          }
      }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
      protected override void ProcessRecord()
      {
          if (_provider == "S3")
          {
              if (_accessKeyID == null || _secretAccessKeyID == null)
              {
                  Console.ForegroundColor = ConsoleColor.Red;
                  Console.WriteLine("Please supply both Secret key and Access key parameters to migrate your S3 Buckets.");
                  Console.ForegroundColor = ConsoleColor.Green;
                  this.MigrateS3Drive();
              }
              else
              {
                  this.MigrateS3Drive();
              }
          }
          else
          {
              this.MigrateLocalDrive();
          }
      }
        #endregion
    }
    }

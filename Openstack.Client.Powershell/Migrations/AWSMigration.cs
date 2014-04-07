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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Openstack.Objects.DataAccess;
using Openstack.Objects.Utility;
using System.IO;
using Openstack.Objects.Domain;
using Openstack.Objects.DataAccess.Storage;

namespace Openstack.Migrations
{
    public class AWSMigration
    {
        private IAmazonS3 _client;
        private Session _session;
        public delegate void CopyOperationEventHandler(object sender, CopyOperationInfoEventArgs e);
        public delegate void CreateContainerOperationEventHandler(object sender, CreateContainerOperationInfoEventArgs e);
        public event CopyOperationEventHandler Changed;
        public event CreateContainerOperationEventHandler ContainerCreated;    
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="e"></param>
//=========================================================================================
        private void OnChanged(CopyOperationInfoEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="e"></param>
//=========================================================================================
        private void OnCreateContainer(CreateContainerOperationInfoEventArgs e)
        {
            if (ContainerCreated != null)
                ContainerCreated(this, e);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="accessKeyID"></param
/// <param name="secretAccessKeyID"></param>
//=========================================================================================
        public AWSMigration(string accessKeyID, string secretAccessKeyID)
        {
            _client  = Amazon.AWSClientFactory.CreateAmazonS3Client(accessKeyID, secretAccessKeyID);
            _session = Session.CreateSession();           
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private void CreateContainer(string name)
        {
           _session.Factory.CreateContainerRepository().SaveContainer(new Objects.Domain.StorageContainer(name));
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public string[] GetBuckets()
        {
            return _client.ListBuckets().Buckets.Select(b => b.BucketName).ToArray<string>();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="s3Object"></param>
/// <param name="bucketName"></param>
//=========================================================================================
        private void CopyObject(S3Object s3Object, string bucketName)
        {
            IStorageObjectRepository repository = _session.Factory.CreateStorageObjectRepository();

            if (s3Object.Key.EndsWith("/") || s3Object.Size == 0)
            {
                StoragePath path = new StoragePath(_session.Context.GetRepositoryContext("object-store").ServiceDescription.Url, bucketName, s3Object.Key);
               repository.MakeFolder(path.AbsoluteURI);              
            }
            else
            {
                // Grab the file from the S3 store...

                GetObjectRequest goRequest = new GetObjectRequest();
                goRequest.BucketName = bucketName;
                goRequest.Key = s3Object.Key;
                GetObjectResponse goResponse = _client.GetObject(goRequest);               
              
                // Save to the newly created OpenStack Container..

                StorageObject sObject = new StorageObject();
                sObject.Load(goResponse.ResponseStream);
                StoragePath path = new StoragePath(_session.Context.GetRepositoryContext("object-store").ServiceDescription.Url, bucketName, s3Object.Key);
                _session.Factory.CreateStorageObjectRepository().Copy(sObject, path.AbsoluteURI, true);

                this.OnChanged(new CopyOperationInfoEventArgs(Path.GetFileName(s3Object.Key), s3Object.Size));
            }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="bucketName"></param>
/// <param name="accessKeyID"></param>
/// <param name="secretAccessKeyID"></param>
//=========================================================================================
        public void MigrateBucket(string sourceBucketName)
        {           
            ListObjectsRequest request = new ListObjectsRequest();
            request                    = new ListObjectsRequest();
            request.BucketName         = sourceBucketName;

            // Create the target Storage Object Container first in Swift..
           
            this.CreateContainer(sourceBucketName);
            this.OnCreateContainer(new CreateContainerOperationInfoEventArgs(sourceBucketName));
          
            do
            {
                ListObjectsResponse response = _client.ListObjects(request);

                foreach (S3Object s3Object in response.S3Objects) {
                    this.CopyObject(s3Object, sourceBucketName);
                }

                if (response.IsTruncated) {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);
        }
    }
}

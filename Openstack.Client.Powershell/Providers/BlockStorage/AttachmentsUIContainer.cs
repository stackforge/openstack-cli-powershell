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
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain.BlockStorage;
using System.Linq;
using Openstack.Client.Powershell.Providers.Security;

namespace Openstack.Client.Powershell.Providers.BlockStorage
{
    [RequiredServiceIdentifierAttribute("Openstack-ComputeDrive")]
    public class AttachmentsUIContainer : BaseUIContainer
    {
        private Server _server;
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================       
        public AttachmentsUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path)
                              : base(parentContainer, name, description, path)
        {
            this.ObjectType = ObjectType.Container;
        }
        public AttachmentsUIContainer()
        {}
        #region Methods
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="securityGroups"></param>
//=========================================================================================================
        public void LoadEntities(Server server)
        {
            _server = server;
            List<VolumeAttachment> attachments = this.RepositoryFactory.CreateVolumeRepository().GetServerVolumes(server.Id);

            if (attachments != null && attachments.Count() > 0)
            {
                List<Volume> volumes = this.RepositoryFactory.CreateVolumeRepository().GetVolumes();

                var innerJoinQuery =
                from attachment in attachments
                join volume in volumes on attachment.VolumeId equals volume.Id
                select new Volume()
                {
                    Id               = volume.Id,
                    Description      = volume.Description,
                    Name             = volume.Name,
                    Size             = volume.Size,
                    AvailabilityZone = volume.AvailabilityZone,
                    Device           = attachment.Device,
                    AttachedTo       = volume.AttachedTo,
                    Status           = volume.Status,
                    CreationDate     = volume.CreationDate
                };

                List<Volume> allVolumes = innerJoinQuery.ToList<Volume>();

                this.SetUIContainers<VolumeUIContainer>(allVolumes);
                this.Entities = allVolumes;
            }
            else
            {
                this.Entities = null;
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
//=========================================================================================================
        public override void  Load()
        {
            this.LoadEntities(_server);
        }
        #endregion
    }
}




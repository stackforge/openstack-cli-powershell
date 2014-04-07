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
using Openstack.Administration.Domain;
using System.Collections;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Objects.Domain.Compute;
using Openstack.Objects.Domain;

namespace Openstack.Client.Powershell.Providers.Compute
{
    public class RatesUIContainer : BaseUIContainer
    {
        private List<BaseEntity>  _rates;
       
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//=========================================================================================================
        public RatesUIContainer(BaseUIContainer parentContainer, 
                              string name, 
                              string description,
                              string path,
                             List<BaseEntity> rates)
                              : base(parentContainer, name, description, path)
        {
            _rates          = rates;
            this.ObjectType = ObjectType.Container;
        }

        #region Methods
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private BaseUIContainer MockEntity(string id)
        {
            Server server1 = new Server();
            server1.Name = "server1";
            server1.Id = "11";
            ServerUIContainer container = new ServerUIContainer(this, id, "server test", this.Path + "\\" + server1.Id);
            container.Entity = server1;

            return container;
          
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================================
        public override void  Load()
        {
            _entities = this.MockEntities();
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=========================================================================================================
        private List<BaseEntity> MockEntities()
        {
            Server server1 = new Server();
            server1.Name = "server1";
            server1.Id = "11";

            Server server2 = new Server();
            server2.Name = "server2";
            server2.Id = "44";


            Server server3 = new Server();
            server3.Name = "server3";
            server3.Id = "99";

            Server server4 = new Server();
            server4.Name = "server4";
            server4.Id = "23";

            Server server5 = new Server();
            server5.Name = "server5";
            server5.Id = "87";

            List<BaseEntity> servers = new List<BaseEntity>();
            servers.Add(server1);
            servers.Add(server2);
            servers.Add(server3);
            servers.Add(server4);
            servers.Add(server5);

            return servers;

        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//=========================================================================================================
        public override BaseUIContainer CreateContainer(string id)
        {
            return this.MockEntity(id);

            //string tenantId            = ((Tenant)CurrentTenantContainer.CurrentTenant.Entity).Id;
            //IUserRepository repository = RepositoryFactory.CreateUserRepository(ResponseFormat.xml);
            //User user                  = repository.GetUser(id, tenantId);

            //if (user != null)
            //{
            //    UserContainer container = new UserContainer(this, user.AccountId, "test descr", this.BuildChildPath(user.AccountId));
            //    container.Entity        = user;

            //    return container;
            //}
            //else
            //{
            //    return null;
            //}        
        }
        #endregion
        #region Properties
        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //=========================================================================================================
        public List<BaseEntity> Rates
        {
            get { return _rates; }
            set { _rates = value; }
        }
        #endregion
    }
}

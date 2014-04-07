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
using System.Linq;
using Openstack.Common.Properties;
using Openstack.Objects.Domain;
using System;
using Openstack.Objects.Utility;
using Openstack.Objects.DataAccess;
using System.Collections.ObjectModel;
using System.Collections;
using Openstack.Objects.Domain.Compute;

namespace Openstack.Client.Powershell.Providers.Common
{
    public class GenericUIContainer : BaseEntityUIContainer { }

    public enum ObjectType
    {
        Entity    = 1,
        Container = 2
    }
    public abstract class BaseUIContainer
    {
        private string _id;
        private string _name;
        private string _description;
        private string _path = @"\";
        private ObjectType _objectType = ObjectType.Container;
        protected List<BaseUIContainer> _containers = new List<BaseUIContainer>();
        protected IList _entities = new List<BaseEntity>();
        private BaseEntity _entity;
        private BaseUIContainer _parentContainer;
        private bool _isContainerListInitialized = false;
        private Context _context;
        private BaseRepositoryFactory _repositoryFactory;
        private string _displayName = null;
             
        #region Ctors
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public BaseUIContainer()
        {

        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="settings"></param>
/// <param name="parentContainer"></param>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
//================================================================================
        public BaseUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
        {
            _description = description;
            _parentContainer = parentContainer;
            _name = name;
            _path = path;
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="settings"></param>
/// <param name="parentContainer"></param>
/// <param name="name"></param>
/// <param name="path"></param>
//================================================================================
        public BaseUIContainer(string displayName, BaseUIContainer parentContainer, string name, string path)
        {
            _parentContainer = parentContainer;
            _name            = name;
            _path            = path;
            _displayName     = displayName;
        }
        #endregion
        #region Properties
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public BaseRepositoryFactory RepositoryFactory
        {
            get { return _repositoryFactory; }
            set { _repositoryFactory = value; }
        } 
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public Context Context
        {
            get { return _context; }
            set 
            {
             _context = value;
             if (_repositoryFactory != null && _repositoryFactory.Context != null)
                 _repositoryFactory.Context = value;
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//================================================================================
        protected bool IsMocked
        {
            get
            {
                if (Settings.Default.IsMocked == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public IList Entities
        {
            get { return _entities; }
            set
            {
                _entities = value;
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public BaseEntity Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public List<BaseUIContainer> Containers
        {
            get { return _containers; }
            //set { _containers = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public ObjectType ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public BaseUIContainer Parent
        {
            get { return _parentContainer; }
            set { _parentContainer = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public string Path
        {
            get { return _path; }
            set { _path = value; }
          
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
        #region Methods
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="container"></param>
//================================================================================
        protected void AddContainer(BaseUIContainer container)
        {
            if (container != null) {
                this.Containers.Add(container);
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <returns></returns>
//================================================================================
        protected string BuildChildPath(string name)
        {
            return this.Path + "\\" + name;
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
//================================================================================
        public virtual BaseUIContainer CreateContainer(string id)
        {
            return null;
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="container"></param>
/// <returns></returns>
//================================================================================
        private bool IsAuthorized<T>(T container)
        {
            Type type         = container.GetType();
            object[] metadata = type.GetCustomAttributes(false);

            foreach (object attribute in metadata)
            {
                RequiredServiceIdentifierAttribute identifier = attribute as RequiredServiceIdentifierAttribute;

                if (identifier != null)
                    if (this.Context.ServiceCatalog.DoesServiceExist(identifier.ServiceName))
                        return true;
                    else return false;
            }           
            
            return true;
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
/// <returns></returns>
//================================================================================
        protected BaseUIContainer SetContainer<T>(BaseUIContainer container, string name, string description, string path, string displayName = null) where T : BaseUIContainer
        {
            if (this.IsAuthorized<BaseUIContainer>(container))
            {
                container.Name              = name;
                container.Description       = description;
                container.Context           = this.Context;
                container.RepositoryFactory = this.RepositoryFactory;
                container.Parent            = this;
                container.Path              = path;
                container.DisplayName       = displayName;
                container.Id                = name;

                return container;
            }
            else
            {
                return null;
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="path"></param>
/// <returns></returns>
//================================================================================
        protected BaseUIContainer CreateContainer<T>(string name, string description, string path, string displayName = null) where T : BaseUIContainer
        {
            T container = Activator.CreateInstance<T>();

            if (this.IsAuthorized<T>(container))
            {
                container.Name              = name;
                container.Description       = description;
                container.Context           = this.Context;
                container.RepositoryFactory = this.RepositoryFactory;
                container.Parent            = this;
                container.Path              = path;
                container.DisplayName       = displayName;
                container.Id                = name;

                return container;
            }
            else
            {
                return null;
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
//================================================================================
        public virtual BaseUIContainer CreateEntityContainer(BaseEntity entity)
        {
            BaseEntityUIContainer container = new BaseEntityUIContainer();
            container.Entity                = entity;

            return container; 
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="containerName"></param>
/// <returns></returns>
//================================================================================
        public BaseUIContainer GetContainer(string containerName)
        {
            return _containers.Where<BaseUIContainer>(c => c.Id == containerName).SingleOrDefault<BaseUIContainer>();
        }

        public virtual void WriteEntityDetails() { return; }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="entities"></param>
//================================================================================
        protected void SetUIContainers<T>(Collection<BaseEntity> entities) where T : BaseUIContainer
        {
            List<T> entityContainers = new List<T>();
            foreach (BaseEntity entity in entities)
            {
                T entityContainer = (T)Activator.CreateInstance(typeof(T));

                entityContainer.Entity            = entity;
                entityContainer.Name              = entity.Name;
                entityContainer.ObjectType        = Common.ObjectType.Entity;
                entityContainer.Parent            = this;
                entityContainer.RepositoryFactory = this.RepositoryFactory;
                entityContainer.Context           = this.Context;
                entityContainer.Id                = entity.Id;

                entityContainers.Add(entityContainer);
            }

            this.Containers.Clear();

            int count = 0;
            foreach (BaseUIContainer bc in entityContainers)
            {               
                bc.Path = @"\" + bc.Parent.Path + @"\-" + Convert.ToString(count);
                this.Containers.Add(bc);
                ++count;
            }
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="entities"></param>
//=========================================================================================================
        protected void SetUIContainers<T>(IList entities) where T : BaseUIContainer
        {
            if (entities != null && entities.Count > 0)
            {
                List<T> entityContainers = new List<T>();
                foreach (BaseEntity entity in entities)
                {
                    T entityContainer = (T)Activator.CreateInstance(typeof(T));

                    entityContainer.Entity            = entity;
                    entityContainer.Name              = entity.Name;
                    entityContainer.ObjectType        = Common.ObjectType.Entity;
                    entityContainer.Parent            = this;
                    entityContainer.RepositoryFactory = this.RepositoryFactory;
                    entityContainer.Context           = this.Context;
                    entityContainer.Id                = entity.Id;

                    entityContainers.Add(entityContainer);
                }

                this.Containers.Clear();

                int count = 0;
                foreach (BaseUIContainer bc in entityContainers)
                {                    
                    bc.Path = @"\" + bc.Parent.Path + @"\-" + Convert.ToString(count);
                    this.Containers.Add(bc);
                    ++count;
                }
            }
        }
        public abstract void Load();
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
//================================================================================
        public virtual bool IsEntityId(string id)
        {
            if (this.Id == id) return true; else return false;
        }
//=========================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="image"></param>
//=========================================================================================================
        protected void WriteMetadata(MetaData metadata)
        {
            if (metadata != null)
            {
                int maxLength = metadata.Max(m => m.Key.Length);

                foreach (KeyValuePair<string, string> element in metadata)
                {
                    if (element.Value != null && element.Key != null)
                       Console.WriteLine(element.Key.PadRight(maxLength) + " : " + element.Value.Trim());
                }
                Console.WriteLine();
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="path"></param>
//================================================================================
        protected void WriteHeader(string msg, ConsoleColor textColor = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = ConsoleColor.White;    
            Console.WriteLine("");
            Console.WriteLine(msg );
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
        }
        #endregion
    }
}

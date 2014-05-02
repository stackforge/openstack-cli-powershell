using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStack.Storage;

namespace OpenStack.Client.Powershell.Providers.ObjectStorage
{
    public class StorageItemViewModel 
    {
        private DateTime _lastModifiedDate;
        private long _size;
        private string _name;
        private string _type;

        public StorageItemViewModel(StorageItem item)
        {
            StorageObject storageObject = item as StorageObject;

            if (storageObject != null)
            {
                this.Name             = storageObject.Name;
                this.Size             = storageObject.Length;
                this.Type             = "File";
                this.LastModifiedDate = storageObject.LastModified;
            }
            else
            {
                StorageFolder storageFolder = item as StorageFolder;
                if (storageFolder != null)
                {
                    this.Name = storageFolder.Name;
                    this.Size = 0;
                    this.Type = "Folder";
                }
            }
        }
        public DateTime LastModifiedDate 
        {
            get { return _lastModifiedDate; }
            set { _lastModifiedDate = value; }
        }
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}

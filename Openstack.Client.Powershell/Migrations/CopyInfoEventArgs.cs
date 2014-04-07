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
namespace Openstack.Migrations
{
    public class CopyInfoEventArgs
    {
        private long _BytesCopied = 0;
        private string _filename;
        private string _exception;

        public string ExceptionMessage
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public CopyInfoEventArgs(string filename, long bytesCopied)
        {
            _filename = filename;
            _BytesCopied = bytesCopied;
        }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public long BytesCopied
        {
            get { return _BytesCopied; }
            set { _BytesCopied = value; }
        }
    }
}

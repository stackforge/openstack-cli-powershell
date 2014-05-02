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
using System.Threading.Tasks;

namespace OpenStack.Client.Powershell.Utility
{
    public class CredentialElement
    {
        private string _key;
        private string _value;
        private string _displayName;
        private string _helpText;
        private bool _isMandatory = true;

        public CredentialElement(string key)
        {
            _key = key;          
        }

        public CredentialElement()
        {

        }

        public bool IsMandatory
        {
            get { return _isMandatory; }
            set { _isMandatory = value; }
        }

        public string HelpText
        {
            get { return _helpText; }
            set { _helpText = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }
}

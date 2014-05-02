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
using OpenStack.Common;
using OpenStack;
using System.Security;
using System.Reflection;
using OpenStack.Identity;

namespace OpenStack.Client.Powershell.Utility
{
    public class Context
    {
        private IOpenStackServiceCatalog _serviceCatalog;
        private Settings _settings;
        //private Token _accessToken;
        private string _productName = "OpenstackDotNetAPI";
        private string _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string _forecolor = "Green";
        
        #region Ctors
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public Context()
        {
            _settings = Settings.Default;
        }
        #endregion
        #region Properties
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public string Forecolor
        {
            get { return _forecolor; }
            set { _forecolor = value; }
        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }
        ////==================================================================================================
        ///// <summary>
        ///// 
        ///// </summary>
        ////==================================================================================================
        //        public Token AccessToken
        //        {
        //            get { return _accessToken; }
        //            set { _accessToken = value; }
        //        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        public IOpenStackServiceCatalog ServiceCatalog
        {
            get { return _serviceCatalog; }
            set { _serviceCatalog = value; }
        }
        #endregion
    }
}

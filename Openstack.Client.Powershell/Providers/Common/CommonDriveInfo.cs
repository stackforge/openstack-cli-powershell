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
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using OpenStack.Client.Powershell.Providers.Common;

namespace OpenStack.Client.Powershell.Providers.Common
{
    public class CommonDriveInfo : PSDriveInfo
    {
        private CommonDriveParameters _parameters = null;
        //private BaseUIContainer _currentContainer = null;

        #region Ctors
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driveInfo"></param>
        //==================================================================================================
        public CommonDriveInfo(PSDriveInfo driveInfo, CommonDriveParameters parameters)
            : base(driveInfo)
        {
            _parameters = parameters;
        }
        #endregion
        #region Methods
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //==================================================================================================
        public Hashtable GetParameters()
        {
            return null;
        }
        #endregion
        #region Properties
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        //public BaseUIContainer CurrentTenant
        //{
        //    get { return _currentContainer; }
        //    set
        //    {
        //        _currentContainer = value;
        //        this.CurrentLocation = _currentContainer.Path;
        //    }
        //}
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        //==================================================================================================
        //public BaseUIContainer CurrentContainer
        //{
        //    get { return _currentContainer; }
        //    set
        //    {
        //        _currentContainer = value;
        //        this.CurrentLocation = _currentContainer.Path;
        //    }
        //}
        #endregion
    }
}

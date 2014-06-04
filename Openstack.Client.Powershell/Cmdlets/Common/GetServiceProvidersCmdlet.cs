///* ============================================================================
//Copyright 2014 Hewlett Packard

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//============================================================================ */
using System;
using System.Management.Automation;
using OpenStack.Client.Powershell.Providers.Common;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using OpenStack;
using System.Xml.Linq;
using System.Collections.Generic;
using OpenStack.Client.Powershell.Utility;
using System.Linq;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "SP", SupportsShouldProcess = true)]
    //[RequiredServiceIdentifierAttribute(OpenStack.Objects.Domain.Admin.Services.ObjectStorage)]
    public class GetServiceProvidersCmdlet : BasePSCmdlet
    {       
        #region Methods

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            var manager = new ConfigurationManager();
            manager.Load();
            
            this.WriteObject("");
            this.WriteObject("Current Service Provider : " + this.Context.CurrentServiceProvider.Name);
            this.WriteObject("-----------------------------------------");
            this.WriteObject(manager.GetServiceProviders());
        }
        #endregion
    }
}

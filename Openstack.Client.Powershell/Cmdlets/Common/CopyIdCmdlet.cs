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
using System.Management.Automation;
using System.Collections;
using Openstack.Client.Powershell.Cmdlets.Common;
using System.IO;
using System.Xml;
using Openstack.Client.Powershell.Providers.Common;
using System.Linq;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Copy, "Id", SupportsShouldProcess = true)]
    public class CopyIdCmdlet : BasePSCmdlet
    {
        private int _quickPickNumber;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "CopyIdCmdlet", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the Server.")]
        [Alias("qp")]
        public int QuickPickNumber
        {
            get { return _quickPickNumber; }
            set { _quickPickNumber = value; }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {            
            int number             = 0;
            CommonDriveInfo drive  = this.Drive as CommonDriveInfo;
            BaseUIContainer result = drive.CurrentContainer.Containers.Where(q => q.Entity.QuickPickNumber == _quickPickNumber).FirstOrDefault<BaseUIContainer>();

            WriteObject("");
            this.WriteObject("Id " + result.Entity.Id + " copied to clipboard.");
            WriteObject("");
            OutClipboard.SetText(result.Entity.Id);          
        }
    }
}

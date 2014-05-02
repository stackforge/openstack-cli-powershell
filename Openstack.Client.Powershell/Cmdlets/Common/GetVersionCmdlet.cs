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
using OpenStack.Client.Powershell.Cmdlets.Common;
using System.Reflection;
using OpenStack.Client.Powershell.Providers.Common;

namespace OpenStack.Client.Powershell.Cmdlets.GroupManagement
{
    [Cmdlet(VerbsCommon.Get, "Version", SupportsShouldProcess = true)]
    public class GetVersionCmdlet : BasePSCmdlet
    {
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            Console.WriteLine(" ");
            Console.WriteLine("Assembly Location : " + Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Product Version   : " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("CLR Version       : " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
            Console.WriteLine(" ");
        }
    }
}

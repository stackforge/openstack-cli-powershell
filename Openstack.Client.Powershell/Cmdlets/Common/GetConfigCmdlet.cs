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
using OpenStack.Client.Powershell.Cmdlets.Common;
using System.IO;
using System.Xml;

namespace OpenStack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "Config", SupportsShouldProcess = true)]
    public class GetConfigCmdlet : BasePSCmdlet
    {
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private void WriteSection(string headerText)
        {
            WriteObject(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteObject(headerText);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteObject("==============================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            string configFilePath = this.ConfigFilePath;

            WriteObject("");
            this.WriteSection("Current Session Settings are as follows. ");
            WriteObject("");
            this.WriteObject("Configuration File located at " + configFilePath);
            WriteObject("");

            foreach (DictionaryEntry setting in this.Context.Settings)
            {              
                if (((string)setting.Key) == "SecretKey" || ((string)setting.Key)== "AccessKey")
                {
                    DictionaryEntry entry = new DictionaryEntry();
                    entry.Value           = "***********";
                    entry.Key             = setting.Key;
                    WriteObject(entry);
                }
                else
                {
                    WriteObject(setting);
                }
            }
            WriteObject("");
        }
    }
}

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
using Openstack.Client.Powershell.Cmdlets.Common;
using System.Management.Automation;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet("Set", "ZoneColor", SupportsShouldProcess = true)]
    public class SetZoneColorCmdlet : BasePSCmdlet
    {
        private string _zone;
        private string _color;

//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, ParameterSetName = "SetZoneColor", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("c")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("Black", "Blue", "Cyan", "DarkBlue", "DarkCyan", "DarkGray", "DarkGreen", "DarkMagenta", "DarkRed", "DarkYellow", "Gray", "Green", "Magenta", "Red", "White", "Yellow")]
        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "SetZoneColor", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("z")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("1", "2", "3", "4")]
        public string Zone
        {
            get { return _zone; }
            set { _zone = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            string configFilePath  = this.ConfigFilePath;
            XDocument doc          = XDocument.Load(configFilePath);
            XElement zoneKeyNode   = doc.XPathSelectElement("//AvailabilityZone[@id='" + _zone + "']");
  
            zoneKeyNode.SetAttributeValue("shellForegroundColor", _color);
            doc.Save(configFilePath);

            Console.WriteLine("");
            Console.Write(zoneKeyNode.Attribute("name").Value + " now assigned to ");

            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor   = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), zoneKeyNode.Attribute("shellForegroundColor").Value);

            Console.Write(_color + ".");
            Console.ForegroundColor = currentColor;
            Console.WriteLine("");
            Console.WriteLine("");
        }
    }
}

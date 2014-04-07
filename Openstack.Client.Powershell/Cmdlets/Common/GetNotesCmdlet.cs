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
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;

namespace Openstack.Client.Powershell.Cmdlets.Common
{
    [Cmdlet(VerbsCommon.Get, "Notes", SupportsShouldProcess = true)]
    public class GetNotesCmdlet : BasePSCmdlet
    {
        private bool _showAllNotes = false;
        private string _version;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "sn", ValueFromPipelineByPropertyName = true, HelpMessage = "Show all release notes for this product.")]
        [Alias("all")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ShowAllNotes
        {
            get { return _showAllNotes; }
            set { _showAllNotes = value; }
        }
//=========================================================================================
/// <summary>
/// The location of the file to set permissions on.
/// </summary>
//=========================================================================================
        [Parameter(ParameterSetName = "sn", Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Show release notes for a specific version of the CLI.")]
        [Alias("v")]
        [ValidateNotNullOrEmpty]
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }
        #endregion
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="releaseNode"></param>
//=======================================================================================================
        private void PrintRelease (XElement releaseNode)
        {
            Console.WriteLine("-------------------------------------------------------- ");
            Console.WriteLine("Version : " + releaseNode.Attribute(XName.Get("version")).Value);
            Console.WriteLine("Notes   :");

            XElement notes = (from xml2 in releaseNode.Descendants()
                             select xml2).FirstOrDefault();
            Console.WriteLine(notes.Value);
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        public void PrintVersionNotes(XDocument releaseNotes)
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            XElement releaseNode = (from xml2 in releaseNotes.Descendants("Release")
                                    where xml2.Attribute(XName.Get("version")).Value == _version
                                    select xml2).FirstOrDefault();

            this.PrintRelease(releaseNode);
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void PrintAllNotes(XDocument releaseNotes)
        {
            Console.WriteLine(" ");
            foreach (XElement element in releaseNotes.Descendants("Release"))
            {
                this.PrintRelease(element);                
            }
            Console.WriteLine(" ");
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void PrintCurrentRelease(XDocument releaseNotes)
        {
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //XElement releaseNode  = (from xml2 in releaseNotes.Descendants("Release")
            //                         where xml2.Attribute(XName.Get("version")).Value == currentVersion
            //                         select xml2).FirstOrDefault();

            XElement notes = releaseNotes.XPathSelectElement("//Release[@version='" + currentVersion + "']");
            if (notes != null)
                this.PrintRelease(notes);
            else
            {
                Console.WriteLine("");
                Console.WriteLine("No release notes exist for the current version.");
                Console.WriteLine("");
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        protected override void ProcessRecord()
        {
            XDocument releaseNotes = XDocument.Load(this.Settings.ReleaseNotesURI);
          
            if (_version == null && _showAllNotes == false)
            {
                this.PrintCurrentRelease(releaseNotes);
            }
            else if (_showAllNotes)
            {
                this.PrintAllNotes(releaseNotes);
            }
            else if (_version != null)
            {
                this.PrintVersionNotes(releaseNotes);
            }
        }
    }
}

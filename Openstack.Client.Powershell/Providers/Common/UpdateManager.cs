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
using System.Linq;
using System.Xml.Linq;
using Openstack.Common.Properties;
using System.Xml.XPath;
using Openstack.Objects.Utility;
using Openstack.Objects.DataAccess;
using System.IO;

namespace Openstack.Client.Powershell.Providers.Common
{
    public class UpdateManager
    {
        private Settings _settings;
        private string _currentVersion;
        private Context _context;
        private BaseRepositoryFactory _factory = null;
               
//================================================================================
/// <summary>
/// 
/// </summary>
//================================================================================
        public UpdateManager(Context context, string currentVersion, BaseRepositoryFactory factory)
        {
            _settings        = context.Settings;
            _currentVersion  = currentVersion;
            _context         = context;
            _factory         = new BaseRepositoryFactory(context);
        }
//================================================================================
// <summary>
 
// </summary>
//================================================================================
        public BaseRepositoryFactory Factory
        {
            get { return _factory; }
            set { _factory = value; }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="manifest"></param>
/// <returns></returns>
//================================================================================
        private bool RequiresUpdate(XDocument manifest)
        {
            try
            {
                XElement releaseNode = (from xml2 in manifest.Descendants("Release")
                                        where xml2.Attribute(XName.Get("IsCurrentRelease")).Value == "True"
                                        select xml2).FirstOrDefault();

                if (releaseNode.Attribute(XName.Get("version")).Value == _currentVersion)
                    return false;
                else
                    return true;
            }
            catch (Exception ex) { return false; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        protected void WriteHeaderSection(string headerText)
        {
            Console.WriteLine(" ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("========================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(headerText);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("========================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="url"></param>
/// <returns></returns>
//=========================================================================================
        private string ReplaceToken(string url)
        {
            string[] elements = url.Split('/');
            elements[4]       = "AUTH_2485a207-71a4-4429-9e24-f7bf49e207fc";
            return String.Join("/", elements);
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private string CreateValidSourcePath(string sourcePath)
        {
            int pos    = sourcePath.IndexOf(':');
            sourcePath = sourcePath.Remove(pos, 1);
            return (this.ReplaceToken(this._context.ServiceCatalog.GetService("object-store").Url) + "/" + sourcePath).Replace(@"\", "/");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
//=========================================================================================
        private void GetLatestRelease(XDocument manifest)
        {
            IStorageObjectRepository repository                 = this.Factory.CreateStorageObjectRepository();
            ((BaseRepository)repository).Context.AccessToken.Id = null;

            XElement releaseNode                = (from xml2 in manifest.Descendants("Release")
                                                   where xml2.Attribute(XName.Get("IsCurrentRelease")).Value == "True"
                                                   select xml2).FirstOrDefault();

            string sourcePath = this.CreateValidSourcePath(releaseNode.Attribute(XName.Get("InstallPath")).Value);
            
            repository.Copy(sourcePath, _settings.NewReleaseFolder + Path.GetFileName(sourcePath), false);
            Console.Write(@"   ==> Download of " + _settings.NewReleaseFolder + Path.GetFileName(sourcePath) +  " was successful.");
            Console.WriteLine(" ");
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//=======================================================================================================
        private string PromptForCredentials()
        {         
            Console.WriteLine("Please supply a downloads folder : ");
            string path =  Console.ReadLine();
            if (!path.EndsWith(@"\")) path = path + @"\";

            if (!path.EndsWith(@"\")) path = path + @"\";

            while (!Directory.Exists(path)) 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("");
                Console.WriteLine("The supplied path does not exist. Please enter a valid path : ");
                Console.ForegroundColor = ConsoleColor.Green;
                path = Console.ReadLine();
            }           
            return path;
        }
//=======================================================================================================
/// <summary>
/// Writes out header information to be used during the process that prompts for new credentials.
/// </summary>
//=======================================================================================================
        private void WriteGetReleasePathHeader()
        {
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("We've noticed that you haven't supplied a location that we can download new releases to. When a new ");
            Console.WriteLine("Windows CLI version is released, you will be shown the release notes for that version and be asked ");
            Console.WriteLine("to download the latest install package. This component will be sent to that location");     
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("=======================================================================================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ");
        }     
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void UpdateCheck(XDocument manifest)
        {
               string configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";

            if (this.RequiresUpdate(manifest))
            {
                this.WriteHeaderSection("New updates are available..");
                Console.WriteLine(" ");
                this.PrintReleaseNotes(manifest);
                Console.WriteLine(" ");
                Console.Write(@"Would you like to download the latest release? (Y\N)");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.KeyChar == 'Y' || key.KeyChar == 'y')
                {
                    _settings.Load(configFilePath);
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                    Console.WriteLine(@"   ==> Copying latest release to " + _settings.NewReleaseFolder);
                    this.GetLatestRelease(manifest);
                }
                else
                {
                    Console.WriteLine(" ");
                    Console.WriteLine(" ");
                }
            }
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="releaseNode"></param>
//=======================================================================================================
        private void PrintReleaseNotes(XDocument manifest)
        {
            XElement releaseNode = (from xml2 in manifest.Descendants("Release")
                                    where xml2.Attribute(XName.Get("IsCurrentRelease")).Value == "True"
                                    select xml2).FirstOrDefault();

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
/// <returns></returns>
//=======================================================================================================
        private bool DoesRequireReleasePath(XElement releasePathNode)
        {
            System.Collections.Generic.IEnumerable<XAttribute> attributes = releasePathNode.Attributes();
            foreach (XAttribute attribute in attributes)
            {
                if (attribute.Name == "value" && attribute.Value == string.Empty)
                {
                    return true;
                }
            }
            return false;
        }
//=======================================================================================================
/// <summary>
/// 
/// </summary>
//=======================================================================================================
        private void CheckReleasePath()
        {
            string configFilePath    = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + @"OS\CLI.config";
            XDocument doc            = XDocument.Load(configFilePath);
            XElement releasePathNode = doc.XPathSelectElement("//add[@key='NewReleaseFolder']");

            if (this.DoesRequireReleasePath(releasePathNode))
            {
                this.WriteGetReleasePathHeader();
                string path = this.PromptForCredentials();
                releasePathNode.SetAttributeValue("value", path);
                doc.Save(configFilePath);
            }
        }
//================================================================================
/// <summary>
/// 
/// </summary>
/// <returns></returns>
//================================================================================
        public void ProcessUpdateCheck()
        {
            return;
            XDocument manifest = XDocument.Load(_settings.ReleaseNotesURI);

            // Make sure that we have a supplied Downloads path from the user to send the release to..

            this.CheckReleasePath();

            // Check for and handle the new update..

            this.UpdateCheck(manifest);          
        }
    }
}

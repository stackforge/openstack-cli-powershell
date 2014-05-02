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
//using System;
//using System.Management.Automation;
//using OpenStack.Client.Powershell.Providers.Common;
//using System.Security.Cryptography;
//using System.Text;
//using System.Web;
//using OpenStack.Objects.Domain;

//namespace OpenStack.Client.Powershell.Cmdlets.Common
//{
//    [Cmdlet(VerbsCommon.Get, "URI", SupportsShouldProcess = true)]
//    [RequiredServiceIdentifierAttribute(OpenStack.Objects.Domain.Admin.Services.ObjectStorage)]
//    public class GetURICmdlet : BasePSCmdlet
//    {
//        private string _sourcePath;
//        private int _daysValid     = 0;
//        private int _secondsValid = 0;

//        #region Parameters
////=========================================================================================
///// <summary>
///// The location of the file to set permissions on.
///// </summary>
////=========================================================================================
//        [Parameter(Position = 0, ParameterSetName = "qA",  Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text is here")]
//        [Alias("s")]
//        [ValidateNotNullOrEmpty]
//        public string SourcePath
//        {
//            get { return _sourcePath; }
//            set { _sourcePath = value; }
//        }
////=========================================================================================
///// <summary>
///// The location of the file to set permissions on.
///// </summary>
////=========================================================================================
//        [Parameter(Position = 1, ParameterSetName = "qA",  Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text is here")]
//        [Alias("dv")]
//        [ValidateNotNullOrEmpty]
//        public int DaysValid
//        {
//            get { return _daysValid; }
//            set { _daysValid = value; }
//        }
////=========================================================================================
///// <summary>
///// The location of the file to set permissions on.
///// </summary>
////=========================================================================================
//        [Parameter(Position = 2, ParameterSetName = "qA",  Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Help Text is here")]
//        [Alias("sv")]
//        [ValidateNotNullOrEmpty]
//        public int SecondsValid
//        {
//            get { return _secondsValid; }
//            set { _secondsValid = value; }
//        }
//        #endregion
//        #region Methods
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        private string ConvertSignatureToHex(string signature)
//        {
//            string hexaHash = "";
//            foreach (byte b in signature)
//            {
//                hexaHash += String.Format("{0:x2}", b);
//            }
//            return hexaHash; 
//        }
////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        private string GenerateTempUrl(string signatureString)
//        {            
//            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(this.Settings.Username));
//            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureString));
//            return BitConverter.ToString(hash).Replace("-", "").ToLower();
//        }
////=========================================================================================
///// <summary>
///// 
///// </summary>
///// <returns></returns>
////=========================================================================================
//        private string GetFormattedUri(StoragePath path)
//        {
//            string[] elements = this.CreateStoragePath(this.SourcePath).ToString().Split('/');
//            return String.Join("/", elements).Replace(elements[0] + "/" + elements[1] + "/" + elements[2], string.Empty);                
//        }
////=========================================================================================
///// <summary>
///// 
///// </summary>
///// <returns></returns>
////=========================================================================================
//        public long GetEpochTime()
//        {
//            long baseTicks      = 621355968000000000;
//            long tickResolution = 10000000;
//            long epoch          = (DateTime.Now.ToUniversalTime().Ticks - baseTicks) / tickResolution;    
            
//            return epoch;
//        } 
////=========================================================================================
///// <summary>
///// 
///// </summary>
///// <returns></returns>
////=========================================================================================
//        private long GetExpirationInSeconds()
//        {
//            if (_daysValid != 0)
//            {
//                return GetEpochTime() + (86400 * _daysValid);
//            }
//            else if (_secondsValid != 0)
//            {
//                return GetEpochTime() + _secondsValid;  
//            }
//            return 0;
//        }

////=========================================================================================
///// <summary>
///// 
///// </summary>
////=========================================================================================
//        private void GetTempUrl()
//        {
//            string uri           = null;
//            long expiration      = this.GetExpirationInSeconds();
//            string totalSeconds  = Convert.ToString(expiration);
//            StoragePath fullPath = this.CreateStoragePath(this.SourcePath);
//            uri                  = this.GetFormattedUri(fullPath);            
//            string signedString  = this.GenerateTempUrl("GET" + "\n" + totalSeconds + "\n" + uri);
//            string signature     = HttpUtility.UrlEncode(this.Settings.DefaultTenantId + ":" + this.Settings.Username + ":" + signedString);
//            string tempUrl       = fullPath.BasePath  + "?temp_url_sig=" + signature + "&temp_url_expires=" + totalSeconds;

//            WriteObject("");
//            WriteObject("Object located at   : " + tempUrl);
//            WriteObject("Url Expiration Date : " + DateTime.Now.AddDays(_daysValid).ToShortDateString() + ". [" + _daysValid + @" day(s) \ " + expiration + " seconds.]");
//            WriteObject("");

//            if (this.Settings.PasteGetURIResultsToClipboard)
//                OutClipboard.SetText(tempUrl);
//        }
////=========================================================================================
///// <summary>
///// 1347472640
///// </summary>
////=========================================================================================
//        protected override void ProcessRecord()
//        {
//            if (_daysValid != 0 || _secondsValid != 0)
//            {
//                this.GetTempUrl();
//            }
//            else
//            {
//                string uri = this.CreateStoragePath(this.SourcePath).ToString();

//                if (this.Settings.PasteGetURIResultsToClipboard)
//                    OutClipboard.SetText(uri);

//                WriteObject("");
//                WriteObject("Object located at : " + uri);
//                WriteObject(""); 
//            }
//        }
//        #endregion
//    }
//}

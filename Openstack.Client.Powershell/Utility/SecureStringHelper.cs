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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenStack.Client.Powershell.Utility
{
    public static class SecureStringHelper
    {
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="secstrPassword"></param>
        /// <returns></returns>
        //==================================================================================================
        public static string convertToUNSecureString(SecureString secstrPassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secstrPassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
        //==================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //==================================================================================================
        public static SecureString ConvertToSecureString(string value)
        {
            // Instantiate the secure string.
            SecureString securePwd = new SecureString();
            var secureStr = new SecureString();
            if (value.Length > 0)
            {
                foreach (var c in value.ToCharArray()) secureStr.AppendChar(c);
            }
            else return null;
            return secureStr;
        }
    }
}

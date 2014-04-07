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
using System.Management.Automation;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Client.Powershell.Providers.Common;
using System;
using System.Threading;
using System.Net.NetworkInformation;
using System.Text;
using System.Collections.Generic;
using Openstack.Objects.Domain.Networking;
using Openstack.Objects.Domain.Compute;

namespace Openstack.Client.Powershell.Cmdlets.Compute.Server
{
    [Cmdlet("Ping", "Server", SupportsShouldProcess = true)]
    [RequiredServiceIdentifierAttribute(Openstack.Objects.Domain.Admin.Services.Compute)]
    public class PingServerCmdletd : BasePSCmdlet
    {
        private string _serverId;

        #region Parameters
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 0, ParameterSetName = "PingwServerPS", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("s")]
        public string ServerId
        {
            get { return _serverId; }
            set { _serverId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="address"></param>
//=========================================================================================
        private void PingServer(string address)
        {
            Console.WriteLine(""); 
            Console.WriteLine("Pinging Server : " + address);
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised, 
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted. 
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 12 seconds for a reply. 
            int timeout = 12000;

            // Set options for transmission: 
            // The data can go through 64 gateways or routers 
            // before it is destroyed, and the data packet 
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            Console.WriteLine("Time to live   : {0}", options.Ttl);
            Console.WriteLine("Don't fragment : {0}", options.DontFragment);

            // Send the ping asynchronously. 
            // Use the waiter as the user token. 
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(address, timeout, buffer, options, waiter);

            // Prevent this example application from ending. 
            // A real application should do something useful 
            // when possible.
            waiter.WaitOne();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private IList<IPAddress> GetServerIPAddresses(string serverId)
        {
            Openstack.Objects.Domain.Compute.Server server = this.RepositoryFactory.CreateServerRepository().GetServer(serverId);
            return server.Addresses.Private;
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            IList<IPAddress> addresses = this.GetServerIPAddresses(this.ServerId);
            Console.WriteLine("");
            Console.WriteLine(addresses.Count + " assigned IP addresses found. Ping results are as follows.");
            Console.WriteLine("");

            foreach (IPAddress address in addresses)
            {
                this.PingServer(address.Addr);
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        private static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user. 
            if (e.Cancelled)
            {
                Console.WriteLine("Ping canceled.");

                // Let the main thread resume.  
                // UserToken is the AutoResetEvent object that the main thread  
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user. 
            if (e.Error != null)
            {
                Console.WriteLine("Ping failed:");
                Console.WriteLine(e.Error.ToString());

                // Let the main thread resume. 
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        public static void DisplayReply(PingReply reply)
        {
            if (reply == null)
                return;

            Console.WriteLine("ping status    : {0}", reply.Status);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address        : {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time : {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live   : {0}", reply.Options.Ttl);
                Console.WriteLine("Don't fragment : {0}", reply.Options.DontFragment);
                Console.WriteLine("Buffer size    : {0}", reply.Buffer.Length);
            }
        }

        #endregion
    }
}


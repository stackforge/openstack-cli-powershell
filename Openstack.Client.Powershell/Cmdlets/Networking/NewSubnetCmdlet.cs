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
using System.Management.Automation;
using System.Text;
using Openstack.Client.Powershell.Cmdlets.Common;
using Openstack.Objects.Domain.Networking;

namespace Openstack.Client.Powershell.Cmdlets.Networking
{
    [Cmdlet("New", "Subnet", SupportsShouldProcess = true)]
    public class NewSubnetCmdlet : BasePSCmdlet
    {
        private string _networkId;
        private int _IPVersion;
        private string _cidr;
        private string[] _allocationPools;

        #region Properties
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 0, ParameterSetName = "NewSubnet", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("nid")]
        public string NetworkId
        {
            get { return _networkId; }
            set { _networkId = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 1, ParameterSetName = "NewSubnet", Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("ipv")]
        public int IPVersion
        {
            get { return _IPVersion; }
            set { _IPVersion = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================        
        [Parameter(Position = 2, ParameterSetName = "NewSubnet", Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Id of the Server.")]
        [Alias("c")]
        public string Cidr
        {
            get { return _cidr; }
            set { _cidr = value; }
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        [Parameter(Position = 3, Mandatory = false, ParameterSetName = "NewSubnet", ValueFromPipelineByPropertyName = true, HelpMessage = "Valid values include")]
        [Alias("a")]
        [ValidateNotNullOrEmpty]
        public string[] AllocationPools
        {
            get { return _allocationPools; }
            set { _allocationPools = value; }
        }
        #endregion
        #region Methods
//=========================================================================================
/// <summary>
/// 
/// </summary>
//=========================================================================================
        protected override void ProcessRecord()
        {
            NewSubnet newSubnet       = new NewSubnet();
            //newSubnet.AllocationPools = this.FormatAllocationPools();
            newSubnet.Cidr            = this.Cidr;
            newSubnet.IPversion       = this.IPVersion;
            newSubnet.NetworkId       = this.NetworkId;

            Subnet subnet = this.RepositoryFactory.CreateSubnetRepository().SaveSubnet(newSubnet);
            Console.WriteLine("");
            Console.WriteLine("New Subnet " + subnet.Id + " created.");
            Console.WriteLine("");
        }
//=========================================================================================
/// <summary>
/// 
/// </summary>
/// <param name="keyValuePairs"></param>
/// <returns></returns>
//=========================================================================================
        public List<AllocationPool> FormatAllocationPools()
        {
            if (_allocationPools != null)
            {
                List<AllocationPool> allocationPools = new List<AllocationPool>();
                char[] seperator = { '|' };

                foreach (string ap in _allocationPools)
                {
                    string[] temp = ap.Split(seperator);
                    AllocationPool allocationPool = new AllocationPool();
                    allocationPool.Start = temp[0];
                    allocationPool.End = temp[1];

                    allocationPools.Add(allocationPool);
                }
                return allocationPools;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}

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
using System.Text;
using System.Collections;
using Openstack.Client.Powershell.Providers.Common;
using Openstack.Client.Powershell.Providers.Security;
using Openstack.Objects.Domain.Compute;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using Openstack.Objects.Domain;

namespace Openstack.Client.Powershell.Providers.Compute
{    
    public class LimitsUIContainer : BaseUIContainer
    {
        private Limits _limits;

        public LimitsUIContainer()
        {

        }

        public override void Load()
        {
            
        }

        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //=========================================================================================================
        public void LoadContainers()
        {


            //XmlSerializer serializer = new XmlSerializer(typeof(Limits));


            //XDocument document = new XDocument();
            //document.lo
            
            
            //Limits limits = (Limits)serializer.Deserialize(new FileStream(@"c:\Projects\rates.xml", FileMode.Open, FileAccess.Read));
           // _limits = (Limits)serializer.Deserialize(new FileStream(@"c:\Projects\results.xml", FileMode.Open, FileAccess.Read));
            //int y = 7;





            //this.Containers.Add(new AbsoluteLimitsUIContainer(this, "Absolute", "Limits based on some monitoring value." , "/"));
           // this.Containers.Add(new RatesUIContainer(this, "Rates",             "Limits based on HTTP Verb in use.", "/"));


            //Limits limits = new Limits();
            //Rate rate1 = new Rate();
            //rate1.RegEx = "testreg";
            //rate1.Uri = "theURI";


            //limits.rates.Add(rate1);

            //XmlSerializer serializer = new XmlSerializer(typeof(Limits));
            //TextWriter tw = new StreamWriter(@"c:\Projects\results.xml");
            //serializer.Serialize(tw, limits);
            //tw.Close(); 




           

         


            //Limits limits = this.MockEntity();
            //RatesUIContainer ratesContainer = new RatesUIContainer(this, "Rates", "Rate limits are specified in terms of both a human-readable wild-card URI and a machine-processable regular expression.", this.BuildChildPath("Rates"));
            //ratesContainer.Rates = limits.Rates;
            //this.Containers.Add(ratesContainer);
            //AbsoluteLimitsUIContainer absoluteLimitsContainer = new AbsoluteLimitsUIContainer(this, "AbsoluteLimits", "Absolute Limits assigned to this account.", this.BuildChildPath("Rates"));
            //this.Containers.Add(absoluteLimitsContainer);
            
        }
        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //=========================================================================================================
        //private Limits MockEntity()
        //{
        //    Limits limits =  new Limits();
            
            
        //    AbsoluteLimit al1 = new AbsoluteLimit();
        //    al1.Name = "al1 test";
        //    al1.Value = 21;
        //    limits.AbsoluteLimits.Add(al1);

        //    AbsoluteLimit al2 = new AbsoluteLimit();
        //    al2.Name = "al2 test";
        //    al2.Value = 21;
        //    limits.AbsoluteLimits.Add(al2);

        //    AbsoluteLimit al3 = new AbsoluteLimit();
        //    al3.Name = "al3 test";
        //    al3.Value = 21;
        //    limits.AbsoluteLimits.Add(al3);

        //    AbsoluteLimit al4 = new AbsoluteLimit();
        //    al4.Name = "al4 test";
        //    al4.Value = 21;
        //    limits.AbsoluteLimits.Add(al4);


        //    Rate r1 = new Rate();
            
        //    limits.Rates.Add





        //}
        //=========================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="path"></param>
        //=========================================================================================================
        public LimitsUIContainer(BaseUIContainer parentContainer, string name, string description, string path)
            : base(parentContainer, name, description, path)
        {
            this.LoadContainers();
            this.ObjectType = ObjectType.Container;
        }
    }
}

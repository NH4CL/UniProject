using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.Member;

namespace LiveAzure.Tools.Tester
{
    class ModelTestFenghuang
    {
        public void MainTest()
        {
            // GeneralTest();
            MemberTest();
            // InitialiseDatabase.ImportRegion(@"C:\Temp\SystemRegion.xls");
        }
        public void MemberTest()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            MemberOrganization ResourceA = new MemberOrganization
            {

                Code="123456"
               
            };
            MemberOrganization ResourceB = new MemberOrganization
            {
                Code = "2345",
                CellPhone = "123456"
            };

            oLiveEntities.MemberOrganizations.Add(ResourceA);
            oLiveEntities.MemberOrganizations.Add(ResourceB);
            oLiveEntities.SaveChanges();

            oLiveEntities.Dispose();
            GC.Collect();
        }
    }
}

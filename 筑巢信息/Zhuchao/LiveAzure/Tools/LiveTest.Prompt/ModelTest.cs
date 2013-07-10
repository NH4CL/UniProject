using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models;

namespace LiveAzure.Tools.Tester
{
    public class ModelTest
    {
        public void MainTest()
        {
            MemberTest();        
        }

        public void MemberTest()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);




            oLiveEntities.Dispose();
            GC.Collect();
        }

        public void ProductTest()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);




            oLiveEntities.Dispose();
            GC.Collect();
        }
    }
}

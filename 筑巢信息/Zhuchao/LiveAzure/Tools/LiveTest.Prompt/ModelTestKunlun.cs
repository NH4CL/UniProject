using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Data.Common;
using System.Data.SqlClient;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;

namespace LiveAzure.Tools.Tester
{
    public class ModelTestKunlun
    {
        public void MainTest()
        {
            MemberTest6();
        }

        public void MemberTest()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            GeneralResource ResourceA = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.STRING,
                Code = "ResourceA",
                Culture = 1033,
                Matter = "ResourceTest"
            };

            oLiveEntities.GeneralResources.Add(ResourceA); 
            oLiveEntities.SaveChanges();

            oLiveEntities.Dispose();
            GC.Collect();
        }

        public void MemberTest2()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            GeneralResource ResourceB = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.STRING,
                Code = "ResourceB",
                Culture = 1033,
                Matter = "ResourceTest2"
            };

            GeneralResItem ResItemA = new GeneralResItem
            {
                Culture = 1033,
                Matter = "this is the resource's son",
                Cash = 3.08m,
                Resource = ResourceB
            };

            oLiveEntities.GeneralResItems.Add(ResItemA);
            oLiveEntities.SaveChanges();

            oLiveEntities.Dispose();
            GC.Collect();
        }

        public void MemberTest3()
        {
            LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            GeneralLargeItem LargeItemA = new GeneralLargeItem
            {
                //LargeObject = LargeObjectA,
                Culture = 1024,
                CLOB = "this is a test LargeItem",
                FileType = ".txt",
                Source = (byte)ModelEnum.LargeObjectSource.DATEBASE,
                ObjUrl = "/123/23/3"
            };

            GeneralLargeItem LargeItemB = new GeneralLargeItem
            {
                //LargeObject = LargeObjectB,
                Culture = 2011,
                CLOB = "this is the other test LargeItem",
                FileType = ".php",
                Source = (byte)ModelEnum.LargeObjectSource.NONE,
                ObjUrl = "/1abc"
            };

            GeneralLargeObject LargeObjectA = new GeneralLargeObject
            {
                Code = "LargeObjectA",
                Culture = 1033,
                CLOB = "this is a LargeObject CLOB",
                FileType = ".jpg",
                ObjUrl = "/zhuchao/123",
                LargeItems = new List<GeneralLargeItem> { LargeItemA, LargeItemB},
                Source = (byte)ModelEnum.LargeObjectSource.DATEBASE
            };

            oLiveEntities.GeneralLargeObjects.Add(LargeObjectA);
            oLiveEntities.SaveChanges();

            //Console.WriteLine(LargeItemA.Gid);
            //Console.WriteLine(LargeObjectA.Gid);

            oLiveEntities.Dispose();
            GC.Collect();
        }

        public void MemberTest4()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            var oResources = oGeneralEntities.GeneralLargeObjects.Include("LargeItems");
            foreach (GeneralLargeObject obj1 in oResources.ToList())
            {
                Console.WriteLine(obj1.CLOB);
                foreach (GeneralLargeItem obj2 in obj1.LargeItems)
                {
                    CultureInfo oCulture = new CultureInfo(obj2.Culture);
                    Console.WriteLine("    " + oCulture.NativeName + " " + obj2.CLOB);
                }
            }

        }

        public void MemberTest5()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            GeneralRegion RegionA = new GeneralRegion
            {
                Code = "RegionA",
                FullName = "RegionChinaA",
                ShortName = "A",
                Sorting = 1
            };

            GeneralRegion RegionB = new GeneralRegion
            {
                Code = "RegionB",
                FullName = "RegionChinaB",
                ShortName = "B",
                Sorting = 2,
                Parent = RegionA
            };

            oGeneralEntities.GeneralRegions.Add(RegionB);
            oGeneralEntities.SaveChanges();

            oGeneralEntities.Dispose();
            GC.Collect();
        }

        public void MemberTest6()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            GeneralMeasureUnit UnitA = new GeneralMeasureUnit
            {
                Utype = (byte)ModelEnum.MeasureUnit.PIECE,
                Code = "RMB",
                Name = new GeneralResource
                {
                    Matter = "RMB",
                    Culture = 2052,
                    ResourceItems = new List<GeneralResItem>
                    {
                        new GeneralResItem
                        {
                            Culture = 1033,
                            Matter = "USD"
                        },
                        new GeneralResItem
                        {
                            Culture = 1035,
                            Matter = "FR"
                        }
                    }
                }
            };

            oGeneralEntities.GeneralMeasureUnits.Add(UnitA);
            oGeneralEntities.SaveChanges();

            oGeneralEntities.Dispose();
            GC.Collect();
        }

        public void MemberTest7()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            var DefaultCurrency = oGeneralEntities.GeneralMeasureUnits.Include("UnitA");
            Console.WriteLine();
            
           
        }
    }
}

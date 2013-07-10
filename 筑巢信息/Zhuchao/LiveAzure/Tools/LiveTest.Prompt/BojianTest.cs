using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Diagnostics;
using LiveAzure.BLL;
using LiveAzure.BLL.Taobao;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models.Exchange;
using LiveAzure.Models.Order;
using LiveAzure.Models.Product;

namespace LiveTest.Prompt
{
    public class BojianTest : ITest
    {
        private class ShipList
        {
            public Guid? ShipID { get; set; }
            public int ShipWeight { get; set; }
        }

        public void MainTest()
        {
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            DataTransferBLL oTransfer = new DataTransferBLL(dbEntity);
            oTransfer.RunOnce(@"C:\developer\zhuchao\LiveAzure\Documents\初始数据.xls");
        }

        public void MainTest002()
        {
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            MemberOrganization org = (from o in dbEntity.MemberOrganizations
                                      where o.Code == "Zhuchao"
                                      select o).FirstOrDefault();
            MemberChannel chl = (from c in dbEntity.MemberChannels
                                 where c.Code == "Taobao01"
                                 select c).FirstOrDefault();

            OrderBLL oOrderBLL = new OrderBLL(dbEntity);
            oOrderBLL.GenerateOrderFromTaobao(org, chl);
        }

        public void MainTest005()
        {
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            ProductOnSale oOnSale = (from o in dbEntity.ProductOnSales
                                     where o.Code == "1509000248" && o.Channel.Code == "Taobao01"
                                     select o).FirstOrDefault();

            MemberOrgChannel oOrgChl = (from c in dbEntity.MemberOrgChannels.Include("Channel.ExtendType")
                                        where c.OrgID == oOnSale.OrgID && c.ChlID == oOnSale.ChlID
                                        select c).FirstOrDefault();
            if (oOrgChl.Channel.ExtendType.Code == "Taobao")
            {
                // 同步淘宝库存和价格
                LiveAzure.BLL.Taobao.ProductAPI oTopProduct = new LiveAzure.BLL.Taobao.ProductAPI(dbEntity, oOrgChl);
                oTopProduct.ProductUpdate(oOnSale);
            }
            else if (oOrgChl.Channel.ExtendType.Code == "yihaodian")
            {
                // TODO 同步一号店库存和价格
            }
            else if (oOrgChl.Channel.ExtendType.Code == "Paipai")
            {
                // TODO 同步拍拍库存和价格
            }
            else if (oOrgChl.Channel.ExtendType.Code == "Sina")
            {
                // TODO 同步新浪库存和价格
            }
            else if (oOrgChl.Channel.ExtendType.Code == "tg.com.cn")
            {
                // TODO 同步齐家库存和价格
            }



            // LogisticsAPI oLogistics = new LogisticsAPI(dbEntity, channel);
            // oLogistics.OnlineSend();
            // oLogistics.GetAreas();

            //TranscationAPI oTranscation = new TranscationAPI(dbEntity, channel);
            //oTranscation.DownloadOrders(DateTime.Now.AddDays(-5), DateTime.Now);
        }

        public void MakeTestOrder(LiveEntities dbEntity)
        {
            // 建一个测试订单（真实订单，运单号）
            OrderInformation oNewOrder = new OrderInformation
            {
                Organization = (from o in dbEntity.MemberOrganizations
                                where o.Code == "Zhuchao" && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                select o).FirstOrDefault(),
                Channel = (from c in dbEntity.MemberChannels
                           where c.Code == "Taobao01" && c.Otype == (byte)ModelEnum.OrganizationType.CHANNEL
                           select c).FirstOrDefault(),
                Warehouse = (from w in dbEntity.WarehouseInformations
                             where w.Code == "ZCWH01" && w.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE
                             select w).FirstOrDefault(),
                User = (from u in dbEntity.MemberUsers
                        where u.LoginName == "test"
                        select u).FirstOrDefault(),
                LinkCode = "92781947684462",
                Ostatus = (byte)ModelEnum.OrderStatus.DELIVERIED,
                PayStatus = (byte)ModelEnum.PayStatus.PAID,
                TransType = (byte)ModelEnum.TransType.SECURED,
                Currency = (from u in dbEntity.GeneralMeasureUnits
                            where u.Code == "¥" && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                            select u).FirstOrDefault(),
                Location = (from r in dbEntity.GeneralRegions
                            where r.Code == "110105"
                            select r).FirstOrDefault(),
            };
            OrderShipping oShipping = new OrderShipping
            {
                Order = oNewOrder,
                Shipper = (from s in dbEntity.ShippingInformations
                           where s.Code == "EMS" && s.Otype == (byte)ModelEnum.OrganizationType.SHIPPER
                           select s).FirstOrDefault(),
                Ostatus = (byte)ModelEnum.ShippingCheck.PASSED,
                Candidate = true
            };
            dbEntity.OrderShippings.Add(oShipping);
            dbEntity.SaveChanges();

            ExTaobaoDeliveryPending oPending = new ExTaobaoDeliveryPending
            {
                Order = oNewOrder,
                Dstatus = (byte)ModelEnum.TaobaoDeliveryStatus.WAIT_FOR_SEND,
                ShipID = oShipping.Gid,
                tid = oNewOrder.LinkCode,
                logistics = oShipping.Shipper.Code,
                out_sid = "EM812010333CS"
            };
            dbEntity.ExTaobaoDeliveryPendings.Add(oPending);
            dbEntity.SaveChanges();
        }

        public void ImportRegions()
        {
            Console.WriteLine("Start Test");
            
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            DataTransferBLL oTransfer = new DataTransferBLL(dbEntity);
            // 应先导入2007-12-31版，淘宝使用该版本
            // 然后导入2010-12-31版，这是最新版
            oTransfer.ImportChinaRegionsText(@"C:\Temp\ChinaRegions071231.txt");
            // 需要断开数据连接再执行，Why?
            oTransfer.ImportChinaRegionsText(@"C:\Temp\ChinaRegions101231.txt");

            MemberOrgChannel channel = (from c in dbEntity.MemberOrgChannels
                                        where c.RemoteUrl != null
                                        select c).FirstOrDefault();
            LogisticsAPI oLogisticsAPI = new LogisticsAPI(dbEntity, channel);
            oLogisticsAPI.GetAreas();
        }

        public void MainTest02()
        {
            Console.WriteLine("Start Test");
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            InitialiseDatabase.RunOnce();
            ExTaobaoOrder oExOrder = new ExTaobaoOrder
            {
                tid = 456
            };
            ExTaobaoOrdItem oExItem = new ExTaobaoOrdItem
            {
                num_iid = 345
            };
            oExOrder.TaobaoOrderItems.Add(oExItem);

            dbEntity.ExTaobaoOrders.Add(oExOrder);
            dbEntity.SaveChanges();

            MemberOrgChannel channel = (from c in dbEntity.MemberOrgChannels
                                        where c.RemoteUrl != null
                                        select c).FirstOrDefault();

            TranscationAPI oTransAPI = new TranscationAPI(dbEntity, channel);
            oTransAPI.DownloadOrders(DateTime.Now.AddDays(-1), DateTime.Now);
        }

        public void MainTest01()
        {
            Console.WriteLine("Start Test");
            InitialiseDatabase.RunOnce();
            //InitialiseDatabase.ImportRegionExcel(@"C:\Temp\SystemRegion.xls");

            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);


            Guid gUserID = (from u in dbEntity.MemberUsers
                            where u.LoginName == "test"
                            select u.Gid).FirstOrDefault();

            var oPrivilege1 = (from p in dbEntity.MemberPrivileges
                              where p.UserID == gUserID && p.Ptype == (byte)ModelEnum.UserPrivType.PROGRAM
                              select p).FirstOrDefault();
            var oProgram = (from p in dbEntity.GeneralPrograms.Include("ProgramNodes")
                            where p.Code == "ProgramIndex"
                            select p).FirstOrDefault();
            oPrivilege1.PrivilegeItems.Add(
                new MemberPrivItem
                {
                    RefID = oProgram.Gid
                });

            var oPrivilege2 = (from p in dbEntity.MemberPrivileges
                               where p.UserID == gUserID && p.Ptype == (byte)ModelEnum.UserPrivType.PROGRAM_NODE
                               select p).FirstOrDefault();
            if (oPrivilege2 == null)
            {
                oPrivilege2 = new MemberPrivilege
                {
                    Ptype = (byte)ModelEnum.UserPrivType.PROGRAM_NODE,
                    UserID = gUserID,
                    PrivilegeItems = new List<MemberPrivItem>
                    { 
                        new MemberPrivItem
                        {
                            RefID = oProgram.FindProgramNode("EnableEdit").Gid,
                            NodeCode = "EnableEdit",
                            NodeValue = "1"
                        }
                    }

                };
                dbEntity.MemberPrivileges.Add(oPrivilege2);
            }

            dbEntity.Entry(oPrivilege1).State = EntityState.Modified;
            dbEntity.SaveChanges();
        }

        public void TestOld()
        {
            LiveEntities dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);

            CultureInfo oCulture1 = new CultureInfo("zh-CN");
            CultureInfo oCulture2 = new CultureInfo("zh-TW");
            CultureInfo oCulture = new CultureInfo("ja-JP");
            decimal dm = 123894.56m;
            string s1 = oCulture.NumberFormat.CurrencySymbol + dm.ToString(oCulture.NumberFormat);
            Debug.WriteLine(s1);

            string symbolRMB = (new CultureInfo("zh-CN")).NumberFormat.CurrencySymbol;
            string symbolUSD = (new CultureInfo("en-US")).NumberFormat.CurrencySymbol;
            string symbolEUR = (new CultureInfo("fr-FR")).NumberFormat.CurrencySymbol;
            Guid? oUnitRMB = (from m in dbEntity.GeneralMeasureUnits
                              where m.Code == symbolRMB
                              select m.Gid).FirstOrDefault();
            Guid? oUnitUSD = (from m in dbEntity.GeneralMeasureUnits
                              where m.Code == symbolUSD
                              select m.Gid).FirstOrDefault();
            Guid? oUnitEUR = (from m in dbEntity.GeneralMeasureUnits
                              where m.Code == symbolEUR
                              select m.Gid).FirstOrDefault();

            GeneralResource oMoney = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.MONEY,
                Currency = oUnitRMB,
                Cash = 123.56m,
                ResourceItems = new List<GeneralResItem>
                {
                    new GeneralResItem { Currency = oUnitUSD, Cash = 34.67m },
                    new GeneralResItem { Currency = oUnitEUR, Cash = 31.69m }
                }
            };
            dbEntity.GeneralResources.Add(oMoney);
            dbEntity.SaveChanges();

            GeneralBLL oGeneralBLL = new GeneralBLL(dbEntity);
            string sm = oGeneralBLL.GetMoneyString(oMoney.Gid, oUnitRMB);


            string sMoneyCode1 = oMoney.GetCurrencyUnit(dbEntity).Code;
            string sMoneyCode2 = oMoney.GetCurrencyUnit(dbEntity, oUnitUSD.Value).Code;


            GeneralProgNode oProgNode = new GeneralProgNode
            {
                Program = dbEntity.GeneralPrograms.Where(p => p.Code == "ConfigIndex").FirstOrDefault(),
                Code = "EnableModify",
                Name = new GeneralResource
                {
                    Culture = 1033,
                    Matter = "Modify",
                    ResourceItems = new List<GeneralResItem>
                    {
                        new GeneralResItem { Culture = 2052, Matter = "允许修改" }
                    }
                },
                InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                Optional = new GeneralResource
                {
                    Culture = 1033,
                    Matter = "{0|No},{1|Yes}",
                    ResourceItems = new List<GeneralResItem>
                    {
                        new GeneralResItem { Culture = 2052, Matter = "{0|否},{1|是}" }
                    }
                }
            };
            dbEntity.GeneralProgNodes.Add(oProgNode);
            dbEntity.SaveChanges();

            MemberPrivilege oPrivilege = new MemberPrivilege
            {
                User = dbEntity.MemberUsers.Where(u => u.LoginName == "test").FirstOrDefault(),
                Ptype = (byte)ModelEnum.UserPrivType.PROGRAM_NODE,
                PrivilegeItems = new List<MemberPrivItem>
                {
                     new MemberPrivItem
                     {
                         RefID = oProgNode.Gid,
                         NodeCode = "EnableModify",
                         NodeValue = "1"
                     }
                }
            };
            dbEntity.MemberPrivileges.Add(oPrivilege);
            dbEntity.SaveChanges();


            Dictionary<string, string> oProgramNodes = new Dictionary<string, string>();
            oProgramNodes.Add("Supervisor", "1");
            if (oProgramNodes.ContainsKey("Supervisor") && (oProgramNodes["Supervisor"] == "1"))
            {
                Debug.WriteLine(oProgramNodes["Supervisor"]);
            }
            if (oProgramNodes.ContainsKey("Key") && (oProgramNodes["Key"] == "1"))
            {
                string s = oProgramNodes["Key"];
                Debug.WriteLine(s);
            }

            TimeSpan oTimeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            DateTimeOffset oLocalTime1 = new DateTimeOffset(2011, 8, 24, 13, 45, 24, oTimeSpan);

            DateTime oLocalTime2 = DateTime.Now;
            oLocalTime2 = DateTime.SpecifyKind(oLocalTime2, DateTimeKind.Utc);
            DateTimeOffset oLocalTime3 = oLocalTime2;
            DateTimeOffset oLocalTime4 = oLocalTime1;

            GeneralConfig oConfig1 = new GeneralConfig
            {
                Code = "Test11",
                DateValue = oLocalTime1
            };
            GeneralConfig oConfig2 = new GeneralConfig
            {
                Code = "Test21",
                DateValue = oLocalTime3
            };
            GeneralConfig oConfig3 = new GeneralConfig
            {
                Code = "Test31",
                DateValue = oLocalTime4
            };


            dbEntity.GeneralConfigs.Add(oConfig1);
            dbEntity.GeneralConfigs.Add(oConfig2);
            dbEntity.GeneralConfigs.Add(oConfig3);
            dbEntity.SaveChanges();

            GeneralTodoList oTodo2 = (from t in dbEntity.GeneralTodoLists
                                      where t.Gid == new Guid("3429BF77-1ECE-E011-A3A0-60EB69D65AE8")
                                      select t).FirstOrDefault();

            // string strSQL = String.Format("SELECT * FROM dbo.fn_FindTest('{0}')", Guid.Empty);
            // string strSQL = String.Format("EXECUTE sp_FindTest '{0}'", Guid.Empty);

            var fnFindTest = dbEntity.Database.SqlQuery<ShipList>("SELECT * FROM fn_FindTest({0})", Guid.Empty);

            foreach (ShipList fnItem in fnFindTest)
            {
                Debug.WriteLine(fnItem.ShipID);
                Debug.WriteLine(fnItem.ShipWeight);
            }

            Guid og = Guid.Empty;
            Guid? og1 = null;
            Guid? og2 = Guid.NewGuid();
            Guid? og3 = Guid.Empty;
            if (og == Guid.Empty)
                Debug.WriteLine(og);
            if (og1.HasValue)
                Debug.WriteLine(og1.Value);
            if (og2.HasValue)
                Debug.WriteLine(og2.Value);
            if (og3.HasValue)
                Debug.WriteLine(og3.Value);

            var list = from c in dbEntity.WarehouseInformations
                       where c.Deleted == false
                       select c.Parent;
            foreach (var v in list)
            {
                Debug.WriteLine(v.FullName.Matter);

                //Debug.WriteLine(v.Key);
                //foreach (var item in v)
                //{
                //    Debug.WriteLine(item.FullName.Matter);
                //}
                //if (v.FullName == null)
                //    Debug.WriteLine("is null");
                //foreach (var v2 in v.FullName.ResourceItems)
                //    Debug.WriteLine(v2.Matter);
            }


            EventBLL oEventBLL = new EventBLL(dbEntity);
            oEventBLL.WriteEvent("系统启动", ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());

            GeneralConfig oConfig = dbEntity.GeneralConfigs.Where(c => c.Code == "SessionName").FirstOrDefault();

            List<ListItem> oList2 = oConfig.SelectEnumList(typeof(ModelEnum.ActionSource), 2);
            string oListName = oConfig.SelectEnumName(typeof(ModelEnum.ActionSource), 2);

        }
    }
}

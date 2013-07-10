using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Diagnostics;
using System.Globalization;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models.Product;
using LiveAzure.Models.Warehouse;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace LiveAzure.Tools.Tester
{
    public class ModelTestBojian
    {
        public void MainTest()
        {
            GeneralTest();
            //LiveAzure.Models.General.InitialiseDatabase.RunOnce();
            // InitialiseDatabase.ImportRegion(@"C:\Temp\SystemRegion.xls");
            // TestReadRecord();
            // ErrorReportTrigger();
            // CallStoredProcedure();
            // ProductWarehouse();
        }

        public void ProductWarehouse()
        {
            using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
            {
                MemberOrganization oOrgan = oLiveEntities.MemberOrganizations.Where(o => o.Code == "Zhuchao").Single();
                MemberUser oUser = oLiveEntities.MemberUsers.Where(u => u.LoginName == "admin").Single();

                ProductInformation oProduct = new ProductInformation
                {
                    Organization = oOrgan,
                    Code = "6001002",
                    Name = new GeneralResource
                    {
                        Matter = "产品名称"
                    },
                    SkuItems = new List<ProductInfoItem>
                    {
                        new ProductInfoItem
                        {
                            Organization = oOrgan,
                            Code = "6001002001",
                            Barcode = "6001002001"
                        },
                        new ProductInfoItem
                        {
                            Organization = oOrgan,
                            Code = "6001002002",
                            Barcode = "6001002002"
                        }
                    }
                };

                oLiveEntities.ProductInformations.Add(oProduct);
                oLiveEntities.SaveChanges();
            }

        }

        public void CallStoredProcedure()
        {
            ArrayList sOutParm = new ArrayList();
            Hashtable htParmList = new Hashtable();
            using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
            {
                //oLiveEntities.Database.ExecuteSqlCommand("EXECUTE sp_Test {0}", "测试标题2");

                //oLiveEntities.Database.ExecuteSqlCommand("EXECUTE sp_Test @Title", new SqlParameter("Title", "测试标题3"));

                SqlParameter oFirstParam = new SqlParameter("Title", "测试标题6");
                SqlParameter oReturnParam = new SqlParameter("@ReturnInt", SqlDbType.Int);
                oReturnParam.Direction = ParameterDirection.Output;
                // oLiveEntities.Database.ExecuteSqlCommand("EXECUTE sp_Test @ReturnInt, @Title", oReturnParam, oFirstParam);

//CREATE PROCEDURE sp_Test (
//  -- @ReturnInt int OUTPUT,
//  @Title nvarchar (256)
//) AS
//BEGIN
//  DECLARE @ReturnValue int;
//  INSERT INTO GeneralErrorReport (UserID, Title) VALUES ('B6DCCAFF-C5B9-E011-86F0-60EB69D65AE8', @Title);
//  -- SET @ReturnInt = 109;
//  SET @ReturnValue = 1080;
//  SELECT @ReturnValue;
//  -- SELECT TOP 1 @ReturnValue;
//END;

//CREATE FUNCTION [dbo].[fn_FindTest] (
//  @OrgID uniqueidentifier )
//  RETURNS @FindKeys TABLE (ShipID uniqueidentifier, ShipWeight int) AS
//BEGIN
//  INSERT @FindKeys (ShipID, ShipWeight)
//    SELECT mo.Gid, mo.Otype FROM MemberOrganization mo;
//  RETURN
//END;                
                var o = oLiveEntities.Database.SqlQuery<int>("EXECUTE sp_Test @Title", oFirstParam).Single();

                // 执行函数
                //string strSQL1 = String.Format("SELECT * FROM dbo.fn_FindTest('{0}')", Guid.Empty);
                // 执行存储过程
                //string strSQL2 = String.Format("EXECUTE sp_FindTest '{0}'", Guid.Empty);

                //var fnFindTest = dbEntity.Database.SqlQuery<ShipList>(strSQL);
                //foreach (ShipList fnItem in fnFindTest)
                //{ }

                Console.WriteLine(o.ToString());
            }
        }

        public void ErrorReportTrigger()
        {
            using (var scope = new TransactionScope())
            {
                // 创建EF实体
                GeneralErrorReport oErrorReport;
                using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
                {
                    oErrorReport = new GeneralErrorReport
                    {
                        User = oLiveEntities.MemberUsers.Where(u => u.LoginName == "admin").Single(),
                        Title = "测试触发器产生代码2",
                        Keyword = "触发器",
                        Technology = "测试触发器产生代码"
                    };
                    oLiveEntities.GeneralErrorReports.Add(oErrorReport);
                    oLiveEntities.SaveChanges();
                }
                scope.Complete();
                Debug.WriteLine("Submit Completed");
            }
        }

        public void TestReadRecord()
        {
            using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
            {
                var oConfigList = oLiveEntities.GeneralConfigs.OrderBy(c => c.Gid).Skip(2).Take(2);
                foreach (GeneralConfig oConfig in oConfigList)
                {
                    Console.WriteLine("{0}, {1}, {2}", oConfig.Gid, oConfig.CreateTime, oConfig.Code);
                }
                var oErrorList = oLiveEntities.GeneralErrorReports.Where(e => e.Code == "CER000003");
                foreach (GeneralErrorReport oErrorReport in oErrorList)
                {
                    Console.WriteLine("{0}, {1}, {2}", oErrorReport.Gid.ToString("N"), oErrorReport.Code, oErrorReport.Title);
                }
            }
        }
        
        public void WarehouseTest()
        {
            //try
            //{
                // 创建数据库事务
                using (var scope = new TransactionScope())
                {
                    // 创建EF实体
                    using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
                    {
                        WarehouseInformation oWarehouse = new WarehouseInformation
                        {
                            Code = "Shanghai"
                        };
                        oLiveEntities.WarehouseInformations.Add(oWarehouse);
                        oLiveEntities.SaveChanges();
                    }
                    scope.Complete();
                    Debug.WriteLine("Submit Completed");
                }
            //}
            //catch { }
        }

        public void GeneralTest()
        {
            try
            {
                // 创建数据库事务
                using (var scope = new TransactionScope())
                {
                    // 创建EF实体
                    using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
                    {
                        GeneralResource oResource = new GeneralResource
                        {
                            Code = "Test",
                            Culture = 0,
                            Matter = "名称"
                        };
                        string culturename = oResource.CultureName;

                        CultureInfo oCultureCN = new CultureInfo("zh-CN");
                        CultureInfo oCultureGB = new CultureInfo("en-GB");
                        CultureInfo oCultureUS = new CultureInfo("en-US");
                        CultureInfo oCultureFR = new CultureInfo("fr-FR");
                        CultureInfo oCultureDE = new CultureInfo("de-DE");

                        GeneralMessageReceive oMessageReceive = new GeneralMessageReceive
                        {
                            SendFrom = "13816626660",
                            Matter = "Hello World",
                            GetFrom = "EUCP",
                            SentTime = new DateTime(2010, 2, 10, 12, 35, 09)
                        };

                        oLiveEntities.GeneralMessageReceives.Add(oMessageReceive);
                        // 此处，数据库中事实上没有保存
                        oLiveEntities.SaveChanges();

                        GeneralProgram oProgram = new GeneralProgram
                        {
                            Code = "Main"
                        };
                        GeneralProgNode oNode = new GeneralProgNode
                        {
                            Program = oProgram,
                            Code = "Test"
                        };
                        oLiveEntities.GeneralProgNodes.Add(oNode);
                        oLiveEntities.SaveChanges();
                    }
                    // 提交事务，数据库物理写入
                    scope.Complete();
                    Debug.WriteLine("Submit Completed");
                }
            }
            catch (TransactionAbortedException ex)
            {
                Console.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
            GC.Collect();
        }

        /// <summary>
        /// 支持数据库事务处理的更新程序段
        /// </summary>
        public void MemberTest()
        {
            try
            {
                // 创建数据库事务
                using (var scope = new TransactionScope())
                {
                    // 创建EF实体
                    using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
                    {
                        MemberChannel oOrgan1 = new MemberChannel
                        {
                            Code = "Channel_C"
                        };
                        MemberOrganization oOrgan2 = new MemberOrganization
                        {
                            Code = "Organ_C"
                        };
                        oLiveEntities.MemberChannels.Add(oOrgan1);
                        oLiveEntities.MemberOrganizations.Add(oOrgan2);
                        // 此处，数据库中事实上没有保存
                        oLiveEntities.SaveChanges();
                    }
                    // 提交事务，数据库物理写入
                    scope.Complete();
                    Debug.WriteLine("Submit Completed");
                }
            }
            catch (TransactionAbortedException ex)
            {
                Console.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
            GC.Collect();
        }
        private static void Test01()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            GeneralConfig oGeneralA1 = new GeneralConfig
            {
                Code = "F1",
                Culture = 2052,
                Ctype = (byte)ModelEnum.ConfigParamType.DECIMAL,
                IntValue = 0,
                DecValue = 123.3456m
            };
            GeneralConfig oGeneralA2 = new GeneralConfig
            {
                Code = "F2",
                Culture = 2052,
                Ctype = (byte)ModelEnum.ConfigParamType.DATETIME,
                DateValue = DateTimeOffset.Now
            };
            GeneralConfig oGeneralA = new GeneralConfig
            {
                Code = "F",
                Culture = 2052,
                Ctype = (byte)Models.ModelEnum.ConfigParamType.STRING,
                StrValue = "Root_A",
                ChildItems = new List<GeneralConfig> { oGeneralA1, oGeneralA2 }
            };
            GeneralConfig oGeneralA3 = new GeneralConfig
            {
                Code = "F3",
                Culture = 2052,
                Ctype = (byte)Models.ModelEnum.ConfigParamType.STRING,
                StrValue = "Root_A",
                Parent = oGeneralA
            };

            // oGeneralEntities.GeneralConfigs.Add(oGeneralA);
            // oGeneralEntities.GeneralConfigs.Add(oGeneralA1);
            // oGeneralEntities.GeneralConfigs.Add(oGeneralA2);
            oGeneralEntities.GeneralConfigs.Add(oGeneralA3);
            oGeneralEntities.SaveChanges();

            var oConfigs = oGeneralEntities.GeneralConfigs;
            foreach (GeneralConfig obj in oConfigs.ToList())
            {
                Console.WriteLine("GUID: {0}  Code: {1}, Decimal: {2}", obj.Gid.ToString("N"), obj.Code, obj.DecValue);
            }
            oGeneralEntities.Dispose();
            GC.Collect();
        }

        private static void Test02(string s)
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            // 指定Child方式
            GeneralResItem oResItemA1 = new GeneralResItem
            {
                Culture = 1033,
                Matter = "Product Name (En)"
            };
            GeneralResItem oResItemA2 = new GeneralResItem
            {
                Culture = 1036,
                Matter = "Product Name (Fr)"
            };
            GeneralResource oResourceA = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.STRING,
                Matter = "产品名称",
                ResourceItems = new List<GeneralResItem> { oResItemA1, oResItemA2 }
            };
            // 指定Parent方式
            GeneralResource oResourceB = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.MONEY,
                Matter = "产品名称"
            };
            GeneralResItem oResItemB1 = new GeneralResItem
            {
                Culture = 1033,
                Matter = "Product Name (En)",
                Resource = oResourceB
            };
            GeneralResItem oResItemB2 = new GeneralResItem
            {
                Culture = 1036,
                Matter = "Product Name (Fr)",
                Resource = oResourceB
            };

            oGeneralEntities.GeneralResources.Add(oResourceA);
            oGeneralEntities.GeneralResItems.Add(oResItemB1);
            oGeneralEntities.GeneralResItems.Add(oResItemB2);
            oGeneralEntities.SaveChanges();

            var oResources = oGeneralEntities.GeneralResources.Include("GeneralResItems");
            foreach (GeneralResource obj1 in oResources.ToList())
            {
                Console.WriteLine(obj1.Matter);
                foreach (GeneralResItem obj2 in obj1.ResourceItems)
                {
                    CultureInfo oCulture = new CultureInfo(obj2.Culture);
                    Console.WriteLine("    " + oCulture.NativeName + " " + obj2.Matter);
                }
            }
            oGeneralEntities.Dispose();
            GC.Collect();
        }

        private static void Test03()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            // 指定Child方式
            GeneralLargeItem oItemA1 = new GeneralLargeItem
            {
                Culture = 1033,
                CLOB = "hello, this is a long text (en-US)",
                Source = (byte)ModelEnum.LargeObjectSource.NONE
            };
            GeneralLargeItem oItemA2 = new GeneralLargeItem
            {
                Culture = 1036,
                CLOB = "hello, this is a long text (fr-FR)",
                Source = (byte)ModelEnum.LargeObjectSource.NONE
            };
            GeneralLargeObject oLargeA = new GeneralLargeObject
            {
                CLOB = "hello, there are parents",
                Source = (byte)ModelEnum.LargeObjectSource.DATEBASE,
                LargeItems = new List<GeneralLargeItem> { oItemA1, oItemA2 }
            };
            // 指定Parent方式
            GeneralLargeObject oLargeB = new GeneralLargeObject
            {
                CLOB = "hello, there are parents",
                Source = (byte)ModelEnum.LargeObjectSource.DATEBASE
            };
            GeneralLargeItem oItemB1 = new GeneralLargeItem
            {
                Culture = 1033,
                CLOB = "hello, this is a long text (en-US)",
                Source = (byte)ModelEnum.LargeObjectSource.NONE,
                LargeObject = oLargeB
            };
            GeneralLargeItem oItemB2 = new GeneralLargeItem
            {
                Culture = 1036,
                CLOB = "hello, this is a long text (fr-FR)",
                Source = (byte)ModelEnum.LargeObjectSource.NONE,
                LargeObject = oLargeB
            };

            oGeneralEntities.GeneralLargeObjects.Add(oLargeA);

            oGeneralEntities.GeneralLargeItems.Add(oItemB1);
            oGeneralEntities.GeneralLargeItems.Add(oItemB2);

            oGeneralEntities.SaveChanges();

            var oResources = oGeneralEntities.GeneralLargeObjects.Include("GeneralLargeItems");
            foreach (GeneralLargeObject obj1 in oResources.ToList())
            {
                Console.WriteLine(obj1.CLOB);
                foreach (GeneralLargeItem obj2 in obj1.LargeItems)
                {
                    CultureInfo oCulture = new CultureInfo(obj2.Culture);
                    Console.WriteLine("    " + oCulture.NativeName + " " + obj2.CLOB);
                }
            }
            oGeneralEntities.Dispose();
            GC.Collect();
        }

        private static void Test04()
        {
            LiveEntities oGeneralEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            MemberUser oUser = new MemberUser
            {
                Passcode = "hello world"
            };
            oGeneralEntities.Dispose();
            GC.Collect();
        }
    }
}

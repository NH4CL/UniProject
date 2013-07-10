using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Transactions;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Purchase;
using LiveAzure.Models.Warehouse;
using LiveAzure.Models.Product;
using LiveAzure.Models.Finance;
using LiveAzure.Models.Union;

namespace LiveAzure.BLL
{
    /// <summary>
    /// 数据导入导出模块
    /// </summary>
    public class DataTransferBLL : BaseBLL
    {
        public ProductBLL oProductBLL;
        public GeneralBLL oGeneralBLL;

        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public DataTransferBLL(LiveEntities entity) : base(entity) 
        {
            oProductBLL = new ProductBLL(entity);
            oGeneralBLL = new GeneralBLL(entity);
        }

        /// <summary>
        /// 系统初始化，导入Excel数据
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        public void RunOnce(string sExcelFile = "")
        {
            if (!String.IsNullOrEmpty(sExcelFile) && File.Exists(sExcelFile))
            {
                // Step 1: 导入配置参数
                ImportConfigs(sExcelFile, "配置参数");

                // Step 2: 导入标准分类
                ImportStandardCategories(sExcelFile, "标准分类");

                // Step 3: 导入主要计量单位，默认语言
                ImportMeasureUnits(sExcelFile, "计量单位");
                ImportCultureUnits(sExcelFile, "默认语言");

                // Step 4: 导入程序定义
                ImportPrograms(sExcelFile, "程序定义");

                // Step 5: 导入地区表
                ImportRegions(sExcelFile, "中国地区");  // 最新版本，淘宝使用2007-12-31版本
                ImportRegions(sExcelFile, "美国地区");

                // Setp 6: 导入组织、渠道、仓库、两个角色由触发器生成
                ImportOrganizations(sExcelFile, "组织定义");
                ImportOrganChannels(sExcelFile, "组织渠道");
                ImportOrganCultures(sExcelFile, "组织语言");

                // Step 7: 导入属性定义
                ImportOptional(sExcelFile, "属性定义");
                ImportOrgAttrib(sExcelFile, "组织属性");

                // Step 8: 导入角色
                ImportRoles(sExcelFile, "角色定义");

                // Step 9: 导入用户级别
                ImportUserLevel(sExcelFile, "用户级别");

                // Setp 10: 导入管理员角色，第一个管理员用户，第一个测试用户
                ImportUsers(sExcelFile, "用户定义");

                // Step 11: 导入私有分类
                ImportPrivateCategories(sExcelFile, "私有分类");

                // Step 12: 导入仓库货架，仓库支持的地区和承运商
                ImportWarehouseShelves(sExcelFile, "仓库货架");
                ImportWarehouseRegion(sExcelFile, "仓库地区");
                ImportWarehouseShipping(sExcelFile, "仓库承运商");

                // Step 13: 导入支付方式
                ImportPaymentType(sExcelFile, "支付方式");

                // Step 14: 导入联盟返点规则
                ImportUnion(sExcelFile, "联盟返点");

                // Step 15: 导入产品PU,SKU，模板和批量上架
                ImportProduct(sExcelFile, "商品定义");
                ImportProductOnSale(sExcelFile, "批量上架");

                // Step 16: 导入模板
                ImportMessageTemplate(sExcelFile, "消息模板");
                ImportShippingEnvelope(sExcelFile, "面单模板");

                // Step 17: 测试初始权限
                ImportPrivileges(sExcelFile, "用户权限");
            }
        }

        /// <summary>
        /// 导入和更新中国的地区表，可多次执行；数据来源于国家统计局网站
        /// http://www.stats.gov.cn/tjbz/xzqhdm/t20110726_402742468.htm
        /// </summary>
        /// <param name="sTextFile">文本文件格式代码 名称；必须为UTF-8格式 例如110000  北京市</param>
        [Obsolete]
        public void ImportChinaRegionsText(string sTextFile)
        {
            if (!File.Exists(sTextFile)) return;
            try
            {
                string sCountryCode = "CHN";                // 中国
                int nLevel = 0;
                var oCountry = (from r in dbEntity.GeneralRegions
                                where r.Code == sCountryCode
                                select r).FirstOrDefault();
                if (oCountry == null)
                {
                    oCountry = new GeneralRegion
                    {
                        Code = sCountryCode,
                        FullName = "中华人民共和国",
                        ShortName = "中国",
                        RegionLevel = nLevel
                    };
                    dbEntity.GeneralRegions.Add(oCountry);
                    dbEntity.SaveChanges();
                }
                // 全部删除
                dbEntity.Database.ExecuteSqlCommand("EXECUTE dbo.sp_ClearRegions {0}", oCountry.Gid);
                oCountry.Deleted = false;
                dbEntity.SaveChanges();
                // 更新所有节点
                StreamReader fsTextFile = new StreamReader(sTextFile, Encoding.Default);
                string sLastParent = sCountryCode;
                string sThisParent = sCountryCode;
                GeneralRegion oParent = oCountry;
                do
                {
                    string sLine = fsTextFile.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(sLine))
                    {
                        string sCode = sLine.Substring(0, 6);
                        string sName = sLine.Substring(6).Trim();
                        string sPrivance = sLine.Substring(0, 2);
                        string sCity = sLine.Substring(2, 2);
                        string sDistrict = sLine.Substring(4, 2);
                        if (sCity == "00")
                        {
                            sThisParent = sCountryCode;
                            nLevel = 1;
                        }
                        else if (sDistrict == "00")
                        {
                            sThisParent = sPrivance + "0000";
                            nLevel = 2;
                        }
                        else
                        {
                            sThisParent = sPrivance + sCity + "00";
                            nLevel = 3;
                        }
                        if (String.IsNullOrEmpty(sThisParent))
                        {
                            oParent = null;
                            sLastParent = "";
                        }
                        else if (sThisParent != sLastParent)
                        {
                            oParent = (from r in dbEntity.GeneralRegions
                                       where r.Code == sThisParent
                                       select r).FirstOrDefault();
                            sLastParent = sThisParent;
                        }
                        var oRegion = (from r in dbEntity.GeneralRegions
                                       where r.Code == sCode
                                       select r).FirstOrDefault();
                        if (oRegion == null)
                        {
                            oRegion = new GeneralRegion
                            {
                                Parent = oParent,
                                Code = sCode,
                                FullName = sName,
                                ShortName = sName,
                                RegionLevel = nLevel
                            };
                            dbEntity.GeneralRegions.Add(oRegion);
                        }
                        else
                        {
                            oRegion.Deleted = false;
                            oRegion.FullName = sName;
                            oRegion.ShortName = sName;
                        }
                        dbEntity.SaveChanges();
                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("{0} {1}", this.ToString(), sLine);
                    }
                } while (!fsTextFile.EndOfStream);
                fsTextFile.Dispose();
                fsTextFile.Close();
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入中国地区失败 {0} {1}", sTextFile, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            GC.Collect();
        }

        /// <summary>
        /// 导入系统配置GeneralConfig
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportConfigs(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colInt = oExcel.ExcelTable.Columns["整数"];
                DataColumn colDec = oExcel.ExcelTable.Columns["小数"];
                DataColumn colStr = oExcel.ExcelTable.Columns["字符"];
                DataColumn colDate = oExcel.ExcelTable.Columns["日期"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastParent = "";
                GeneralConfig oParent = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString();
                    string sParent = row[colParent].ToString();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    string sRemark = row[colRemark].ToString();

                    if (String.IsNullOrEmpty(sParent))
                    {
                        oParent = null;
                        sLastParent = "";
                    }
                    else if (sParent != sLastParent)
                    {
                        oParent = (from c in dbEntity.GeneralConfigs
                                   where c.Code == sParent
                                   select c).FirstOrDefault();
                        sLastParent = sParent;
                    }
                    var oConfig = (from c in dbEntity.GeneralConfigs
                                   where c.Code == sCode
                                   select c).FirstOrDefault();
                    if (oConfig == null)
                    {
                        // 只新建，不更新
                        oConfig = new GeneralConfig
                        {
                            Code = sCode,
                            Parent = oParent,
                            Ctype = nType,
                            Remark = sRemark
                        };
                        switch ((ModelEnum.ConfigParamType)nType)
                        {
                            case ModelEnum.ConfigParamType.INTEGER:
                                int nValue;
                                if (Int32.TryParse(row[colInt].ToString(), out nValue))
                                    oConfig.IntValue = nValue;
                                break;
                            case ModelEnum.ConfigParamType.DECIMAL:
                                decimal mValue;
                                if (Decimal.TryParse(row[colDec].ToString(), out mValue))
                                    oConfig.DecValue = mValue;
                                break;
                            case ModelEnum.ConfigParamType.STRING:
                                oConfig.StrValue = row[colStr].ToString();
                                break;
                            case ModelEnum.ConfigParamType.DATETIME:
                                DateTimeOffset dtDateTime;
                                if (DateTimeOffset.TryParse(row[colDate].ToString(), out dtDateTime))
                                    oConfig.DateValue = dtDateTime;
                                break;
                        }
                        dbEntity.GeneralConfigs.Add(oConfig);
                        dbEntity.SaveChanges();
                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                    }
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralConfig成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralConfig错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入标准分类
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        public void ImportStandardCategories(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                
                string sLastParent = "";
                GeneralStandardCategory oParent = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString();
                    string sParent = row[colParent].ToString();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sRemark = row[colRemark].ToString();

                    if (String.IsNullOrEmpty(sParent))
                    {
                        oParent = null;
                        sLastParent = "";
                    }
                    else if (sLastParent != sParent)
                    {
                        oParent = (from c in dbEntity.GeneralStandardCategorys
                                   where c.Code == sParent && c.Ctype == nType
                                   select c).FirstOrDefault();
                        sLastParent = sParent;
                    }
                    var oCategory = (from c in dbEntity.GeneralStandardCategorys
                                     where c.Code == sCode && c.Ctype == nType
                                     select c).FirstOrDefault();
                    if (oCategory == null)
                    {
                        oCategory = new GeneralStandardCategory { Code = sCode, Ctype = nType };
                        dbEntity.GeneralStandardCategorys.Add(oCategory);
                    }
                    oCategory.Parent = oParent;
                    if (oCategory.Name == null)
                        oCategory.Name = oName;
                    else
                        oCategory.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oCategory.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralStandardCategory成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralStandardCategory错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入计量单位
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet表名</param>
        public void ImportMeasureUnits(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sRemark = row[colRemark].ToString();
                    var oUnit = (from c in dbEntity.GeneralMeasureUnits
                                 where c.Code == sCode
                                 select c).FirstOrDefault();
                    if (oUnit == null)
                    {
                        oUnit = new GeneralMeasureUnit { Code = sCode };
                        dbEntity.GeneralMeasureUnits.Add(oUnit);
                    }
                    oUnit.Utype = nType;
                    if (oUnit.Name == null)
                        oUnit.Name = oName;
                    else
                        oUnit.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oUnit.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralMeasureUnit成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralMeasureUnit错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入默认语言及其单位
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        public void ImportCultureUnits(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCulture = oExcel.ExcelTable.Columns["语言"];
                DataColumn colPiece = oExcel.ExcelTable.Columns["计件"];
                DataColumn colWeight = oExcel.ExcelTable.Columns["重量"];
                DataColumn colVolumn = oExcel.ExcelTable.Columns["体积"];
                DataColumn colFluid = oExcel.ExcelTable.Columns["容积"];
                DataColumn colArea = oExcel.ExcelTable.Columns["面积"];
                DataColumn colLinear = oExcel.ExcelTable.Columns["长度"];
                DataColumn colCurrency = oExcel.ExcelTable.Columns["货币"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    int nCulture;
                    Int32.TryParse(row[colCulture].ToString(), out nCulture);
                    string sPiece = row[colPiece].ToString();
                    string sWeight = row[colWeight].ToString();
                    string sVolumn = row[colVolumn].ToString();
                    string sFluid = row[colFluid].ToString();
                    string sArea = row[colArea].ToString();
                    string sLinear = row[colLinear].ToString();
                    string sCurrency = row[colCurrency].ToString();
                    string sRemark = row[colRemark].ToString();

                    var oCulture = (from c in dbEntity.GeneralCultureUnits
                                    where c.Culture == nCulture
                                    select c).FirstOrDefault();
                    if (oCulture == null)
                    {
                        oCulture = new GeneralCultureUnit { Culture = nCulture };
                        dbEntity.GeneralCultureUnits.Add(oCulture);
                    }
                    oCulture.Piece = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sPiece).FirstOrDefault();
                    oCulture.Weight = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sWeight).FirstOrDefault();
                    oCulture.Volume = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sVolumn).FirstOrDefault();
                    oCulture.Fluid = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sFluid).FirstOrDefault();
                    oCulture.Area = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sArea).FirstOrDefault();
                    oCulture.Linear = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sLinear).FirstOrDefault();
                    oCulture.Currency = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sCurrency).FirstOrDefault();
                    oCulture.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), nCulture, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralCultureUnit成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralCultureUnit错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入组织定义
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        public void ImportOrganizations(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colOstatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colOtype = oExcel.ExcelTable.Columns["类型"];
                DataColumn colExCode = oExcel.ExcelTable.Columns["扩展代码"];
                DataColumn colExType = oExcel.ExcelTable.Columns["扩展类型"];
                DataColumn colExTypeCode = oExcel.ExcelTable.Columns["类型代码"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colTerminal = oExcel.ExcelTable.Columns["末级"];
                DataColumn colFullNameCN = oExcel.ExcelTable.Columns["中文完整名称"];
                DataColumn colFullNameUS = oExcel.ExcelTable.Columns["英文完整名称"];
                DataColumn colShortNameCN = oExcel.ExcelTable.Columns["中文简称"];
                DataColumn colShortNameUS = oExcel.ExcelTable.Columns["英文简称"];
                DataColumn colLocation = oExcel.ExcelTable.Columns["地区"];
                DataColumn colFullAddress = oExcel.ExcelTable.Columns["完整地址"];
                DataColumn colPostCode = oExcel.ExcelTable.Columns["邮政编码"];
                DataColumn colContact = oExcel.ExcelTable.Columns["联系人"];
                DataColumn colCellPhone = oExcel.ExcelTable.Columns["手机"];
                DataColumn colWorkPhone = oExcel.ExcelTable.Columns["电话"];
                DataColumn colWorkFax = oExcel.ExcelTable.Columns["传真"];
                DataColumn colEmail = oExcel.ExcelTable.Columns["电子邮件"];
                DataColumn colHomeUrl = oExcel.ExcelTable.Columns["主页"];
                DataColumn colBrief = oExcel.ExcelTable.Columns["简介"];
                DataColumn colIntroduction = oExcel.ExcelTable.Columns["介绍"];
                DataColumn colPuPolicy = oExcel.ExcelTable.Columns["PU编码规则"];
                DataColumn colSkuPolicy = oExcel.ExcelTable.Columns["SKU编码规则"];
                DataColumn colBarcodePolicy = oExcel.ExcelTable.Columns["条码编码规则"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString();
                    byte nStatus;
                    Byte.TryParse(row[colOstatus].ToString(), out nStatus);
                    byte nType;
                    Byte.TryParse(row[colOtype].ToString(), out nType);
                    string sExCode = row[colExCode].ToString();
                    
                    byte nExType;
                    Byte.TryParse(row[colExType].ToString(), out nExType);
                    string sExTypeCode = row[colExTypeCode].ToString();
                    var oExType = (from c in dbEntity.GeneralStandardCategorys
                                   where c.Ctype == nExType && c.Code == sExTypeCode
                                   select c).FirstOrDefault();

                    string sParent = row[colParent].ToString();
                    var oParent = (from m in dbEntity.MemberOrganizations
                                   where m.Code == sParent && m.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                   select m).FirstOrDefault();

                    bool bLeaf = row[colTerminal].ToString() == "1" ? true : false;
                    GeneralResource oFullName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colFullNameCN].ToString(), 1033, row[colFullNameUS].ToString());
                    GeneralResource oShortName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colShortNameCN].ToString(), 1033, row[colShortNameUS].ToString()); 
                    string sLocation = row[colLocation].ToString();
                    var oLocation = (from r in dbEntity.GeneralRegions
                                     where r.Code == sLocation
                                     select r).FirstOrDefault();

                    string sFullAddress = row[colFullAddress].ToString();
                    string sPostCode = row[colPostCode].ToString();
                    string sContact = row[colContact].ToString();
                    string sCellPhone = row[colCellPhone].ToString();
                    string sWorkPhone = row[colWorkPhone].ToString();
                    string sWorkFax = row[colWorkFax].ToString();
                    string sEmail = row[colEmail].ToString();
                    string sHomeUrl = row[colHomeUrl].ToString();
                    string sBrief = row[colBrief].ToString();
                    string sIntroduction = row[colIntroduction].ToString();
                    string sPuPolicy = row[colPuPolicy].ToString();
                    string sSkuPolicy = row[colSkuPolicy].ToString();
                    string sBarcodePolicy = row[colBarcodePolicy].ToString();
                    string sRemark = row[colRemark].ToString();

                    switch ((ModelEnum.OrganizationType)nType)
                    {
                        case ModelEnum.OrganizationType.CORPORATION:
                            var oOrgan = (from o in dbEntity.MemberOrganizations
                                          where o.Code == sCode && o.Otype == nType
                                          select o).FirstOrDefault();
                            if (oOrgan == null)
                            {
                                oOrgan = new MemberOrganization
                                {
                                    Code = sCode,
                                    Otype = nType
                                };
                                dbEntity.MemberOrganizations.Add(oOrgan);
                            }
                            oOrgan.Ostatus = nStatus;
                            oOrgan.ExCode = sExCode;
                            oOrgan.ExtendType = oExType;
                            oOrgan.Parent = oParent;
                            oOrgan.Terminal = bLeaf;
                            if (oOrgan.FullName == null)
                                oOrgan.FullName = oFullName;
                            else
                                oOrgan.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                            if (oOrgan.ShortName == null)
                                oOrgan.ShortName = oShortName;
                            else
                                oOrgan.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                            oOrgan.Location = oLocation;
                            oOrgan.FullAddress = sFullAddress;
                            oOrgan.PostCode = sPostCode;
                            oOrgan.Contact = sContact;
                            oOrgan.CellPhone = sCellPhone;
                            oOrgan.WorkPhone = sWorkPhone;
                            oOrgan.WorkFax = sWorkFax;
                            oOrgan.Email = sEmail;
                            oOrgan.HomeUrl = sHomeUrl;
                            oOrgan.Brief = sBrief;
                            if (!String.IsNullOrEmpty(sIntroduction))
                            {
                                if (oOrgan.Introduction == null)
                                    oOrgan.Introduction = new GeneralLargeObject(2052, sIntroduction);
                                else
                                    oOrgan.Introduction.SetLargeObject(2052, sIntroduction);
                            }
                            oOrgan.ProdCodePolicy = sPuPolicy;
                            oOrgan.SkuCodePolicy = sSkuPolicy;
                            oOrgan.BarcodePolicy = sBarcodePolicy;
                            oOrgan.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                        case ModelEnum.OrganizationType.CHANNEL:
                            var oChannel = (from o in dbEntity.MemberChannels
                                            where o.Code == sCode && o.Otype == nType
                                            select o).FirstOrDefault();
                            if (oChannel == null)
                            {
                                oChannel = new MemberChannel
                                {
                                    Code = sCode,
                                    Otype = nType
                                };
                                dbEntity.MemberChannels.Add(oChannel);
                            }
                            oChannel.Ostatus = nStatus;
                            oChannel.ExCode = sExCode;
                            oChannel.ExtendType = oExType;
                            oChannel.Parent = oParent;
                            oChannel.Terminal = bLeaf;
                            if (oChannel.FullName == null)
                                oChannel.FullName = oFullName;
                            else
                                oChannel.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                            if (oChannel.ShortName == null)
                                oChannel.ShortName = oShortName;
                            else
                                oChannel.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                            oChannel.Location = oLocation;
                            oChannel.FullAddress = sFullAddress;
                            oChannel.PostCode = sPostCode;
                            oChannel.Contact = sContact;
                            oChannel.CellPhone = sCellPhone;
                            oChannel.WorkPhone = sWorkPhone;
                            oChannel.WorkFax = sWorkFax;
                            oChannel.Email = sEmail;
                            oChannel.HomeUrl = sHomeUrl;
                            oChannel.Brief = sBrief;
                            if (!String.IsNullOrEmpty(sIntroduction))
                            {
                                if (oChannel.Introduction == null)
                                    oChannel.Introduction = new GeneralLargeObject(2052, sIntroduction);
                                else
                                    oChannel.Introduction.SetLargeObject(2052, sIntroduction);
                            }
                            oChannel.ProdCodePolicy = sPuPolicy;
                            oChannel.SkuCodePolicy = sSkuPolicy;
                            oChannel.BarcodePolicy = sBarcodePolicy;
                            oChannel.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                        case ModelEnum.OrganizationType.SHIPPER:
                            var oShipper = (from o in dbEntity.ShippingInformations
                                            where o.Code == sCode && o.Otype == nType
                                            select o).FirstOrDefault();
                            if (oShipper == null)
                            {
                                oShipper = new ShippingInformation
                                {
                                    Code = sCode,
                                    Otype = nType
                                };
                                dbEntity.ShippingInformations.Add(oShipper);
                            }
                            oShipper.Ostatus = nStatus;
                            oShipper.ExCode = sExCode;
                            oShipper.ExtendType = oExType;
                            oShipper.Parent = oParent;
                            oShipper.Terminal = bLeaf;
                            if (oShipper.FullName == null)
                                oShipper.FullName = oFullName;
                            else
                                oShipper.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                            if (oShipper.ShortName == null)
                                oShipper.ShortName = oShortName;
                            else
                                oShipper.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                            oShipper.Location = oLocation;
                            oShipper.FullAddress = sFullAddress;
                            oShipper.PostCode = sPostCode;
                            oShipper.Contact = sContact;
                            oShipper.CellPhone = sCellPhone;
                            oShipper.WorkPhone = sWorkPhone;
                            oShipper.WorkFax = sWorkFax;
                            oShipper.Email = sEmail;
                            oShipper.HomeUrl = sHomeUrl;
                            oShipper.Brief = sBrief;
                            if (!String.IsNullOrEmpty(sIntroduction))
                            {
                                if (oShipper.Introduction == null)
                                    oShipper.Introduction = new GeneralLargeObject(2052, sIntroduction);
                                else
                                    oShipper.Introduction.SetLargeObject(2052, sIntroduction);
                            }
                            oShipper.ProdCodePolicy = sPuPolicy;
                            oShipper.SkuCodePolicy = sSkuPolicy;
                            oShipper.BarcodePolicy = sBarcodePolicy;
                            oShipper.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                        case ModelEnum.OrganizationType.SUPPLIER:
                            var oSupplier = (from o in dbEntity.PurchaseSuppliers
                                             where o.Code == sCode && o.Otype == nType
                                             select o).FirstOrDefault();
                            if (oSupplier == null)
                            {
                                oSupplier = new PurchaseSupplier
                                {
                                    Code = sCode,
                                    Otype = nType
                                };
                                dbEntity.PurchaseSuppliers.Add(oSupplier);
                            }
                            oSupplier.Ostatus = nStatus;
                            oSupplier.ExCode = sExCode;
                            oSupplier.ExtendType = oExType;
                            oSupplier.Parent = oParent;
                            oSupplier.Terminal = bLeaf;
                            if (oSupplier.FullName == null)
                                oSupplier.FullName = oFullName;
                            else
                                oSupplier.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                            if (oSupplier.ShortName == null)
                                oSupplier.ShortName = oShortName;
                            else
                                oSupplier.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                            oSupplier.Location = oLocation;
                            oSupplier.FullAddress = sFullAddress;
                            oSupplier.PostCode = sPostCode;
                            oSupplier.Contact = sContact;
                            oSupplier.CellPhone = sCellPhone;
                            oSupplier.WorkPhone = sWorkPhone;
                            oSupplier.WorkFax = sWorkFax;
                            oSupplier.Email = sEmail;
                            oSupplier.HomeUrl = sHomeUrl;
                            oSupplier.Brief = sBrief;
                            if (!String.IsNullOrEmpty(sIntroduction))
                            {
                                if (oSupplier.Introduction == null)
                                    oSupplier.Introduction = new GeneralLargeObject(2052, sIntroduction);
                                else
                                    oSupplier.Introduction.SetLargeObject(2052, sIntroduction);
                            }
                            oSupplier.ProdCodePolicy = sPuPolicy;
                            oSupplier.SkuCodePolicy = sSkuPolicy;
                            oSupplier.BarcodePolicy = sBarcodePolicy;
                            oSupplier.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                        case ModelEnum.OrganizationType.WAREHOUSE:
                            var oWarehouse = (from o in dbEntity.WarehouseInformations
                                              where o.Code == sCode && o.Otype == nType
                                              select o).FirstOrDefault();
                            if (oWarehouse == null)
                            {
                                oWarehouse = new WarehouseInformation
                                {
                                    Code = sCode,
                                    Otype = nType
                                };
                                dbEntity.WarehouseInformations.Add(oWarehouse);
                            }
                            oWarehouse.Ostatus = nStatus;
                            oWarehouse.ExCode = sExCode;
                            oWarehouse.ExtendType = oExType;
                            oWarehouse.Parent = oParent;
                            oWarehouse.Terminal = bLeaf;
                            if (oWarehouse.FullName == null)
                                oWarehouse.FullName = oFullName;
                            else
                                oWarehouse.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                            if (oWarehouse.ShortName == null)
                                oWarehouse.ShortName = oShortName;
                            else
                                oWarehouse.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                            oWarehouse.Location = oLocation;
                            oWarehouse.FullAddress = sFullAddress;
                            oWarehouse.PostCode = sPostCode;
                            oWarehouse.Contact = sContact;
                            oWarehouse.CellPhone = sCellPhone;
                            oWarehouse.WorkPhone = sWorkPhone;
                            oWarehouse.WorkFax = sWorkFax;
                            oWarehouse.Email = sEmail;
                            oWarehouse.HomeUrl = sHomeUrl;
                            oWarehouse.Brief = sBrief;
                            if (!String.IsNullOrEmpty(sIntroduction))
                            {
                                if (oWarehouse.Introduction == null)
                                    oWarehouse.Introduction = new GeneralLargeObject(2052, sIntroduction);
                                else
                                    oWarehouse.Introduction.SetLargeObject(2052, sIntroduction);
                            }
                            oWarehouse.ProdCodePolicy = sPuPolicy;
                            oWarehouse.SkuCodePolicy = sSkuPolicy;
                            oWarehouse.BarcodePolicy = sBarcodePolicy;
                            oWarehouse.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                    }
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, oFullName.Matter);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberOrganization成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberOrganization错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入组织和渠道之间的关系
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportOrganChannels(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colWarehouse = oExcel.ExcelTable.Columns["仓库"];
                DataColumn colChannel = oExcel.ExcelTable.Columns["渠道"];
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colRemoteUrl = oExcel.ExcelTable.Columns["远程地址"];
                DataColumn colConfigKey = oExcel.ExcelTable.Columns["ConfigKey"];
                DataColumn colSecretKey = oExcel.ExcelTable.Columns["SecretKey"];
                DataColumn colSessionKey = oExcel.ExcelTable.Columns["SessionKey"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    Guid gOrgID = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o.Gid).FirstOrDefault();
                    string sWarehouse = row[colWarehouse].ToString();
                    if (!String.IsNullOrEmpty(sWarehouse))
                    {
                        var oWhID = (from w in dbEntity.WarehouseInformations
                                 where w.Code == sWarehouse && w.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE
                                       && w.aParent == gOrgID
                                 select w.Gid).FirstOrDefault();
                        if (oWhID != null) gOrgID = oWhID;
                    }
                    string sChannel = row[colChannel].ToString();
                    Guid gChlID = (from c in dbEntity.MemberChannels
                                    where c.Code == sChannel && c.Otype == (byte)ModelEnum.OrganizationType.CHANNEL
                                    select c.Gid).FirstOrDefault();
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    string sRemoteUrl = row[colRemoteUrl].ToString();
                    string sConfigKey = row[colConfigKey].ToString();
                    string sSecretKey = row[colSecretKey].ToString();
                    string sSessionKey = row[colSessionKey].ToString();
                    string sRemark = row[colRemark].ToString();

                    var oOrgChl = (from o in dbEntity.MemberOrgChannels
                                   where o.OrgID == gOrgID && o.ChlID == gChlID
                                   select o).FirstOrDefault();
                    if (oOrgChl == null)
                    {
                        oOrgChl = new MemberOrgChannel { OrgID = gOrgID, ChlID = gChlID };
                        dbEntity.MemberOrgChannels.Add(oOrgChl);
                    }
                    oOrgChl.Cstatus = nStatus;
                    oOrgChl.RemoteUrl = sRemoteUrl;
                    oOrgChl.ConfigKey = sConfigKey;
                    oOrgChl.SecretKey = sSecretKey;
                    oOrgChl.SessionKey = sSessionKey;
                    oOrgChl.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2} {3}", this.ToString(), sOrgan, sChannel, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberOrgChannel成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberOrgChannel错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入组织支持的语言
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportOrganCultures(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colCulture = oExcel.ExcelTable.Columns["语言"];
                DataColumn colCurrency = oExcel.ExcelTable.Columns["货币"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    Guid gCultureID = Guid.Empty;
                    if (!String.IsNullOrEmpty(row[colCulture].ToString()))
                    {
                        int nCulture = 0;
                        if (Int32.TryParse(row[colCulture].ToString(), out nCulture))
                        {
                            var oCulture = (from u in dbEntity.GeneralCultureUnits
                                            where u.Culture == nCulture
                                            select u).FirstOrDefault();
                            if (oCulture != null) gCultureID = oCulture.Gid;
                        }
                    }
                    Guid gCurrencyID = Guid.Empty;
                    if (!String.IsNullOrEmpty(row[colCurrency].ToString()))
                    {
                        string sCurrency = row[colCurrency].ToString();
                        var oCurrency = (from u in dbEntity.GeneralMeasureUnits
                                         where u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == sCurrency
                                         select u).FirstOrDefault();
                        if (oCurrency != null) gCurrencyID = oCurrency.Gid;
                    }
                    string sRemark = row[colRemark].ToString();

                    switch ((ModelEnum.CultureType)nType)
                    {
                        case ModelEnum.CultureType.LANGUAGE:
                            var oOrgCulture = (from o in dbEntity.MemberOrgCultures
                                   where o.OrgID == oOrgan.Gid && o.Ctype == nType && o.aCulture == gCultureID
                                   select o).FirstOrDefault();
                            if (oOrgCulture == null)
                            {
                                oOrgCulture = new MemberOrgCulture
                                {
                                    Organization = oOrgan,
                                    Ctype = nType,
                                    aCulture = gCultureID
                                };
                                dbEntity.MemberOrgCultures.Add(oOrgCulture);
                            }
                            oOrgCulture.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                        case ModelEnum.CultureType.CURRENCY:
                            var oOrgCurrency = (from o in dbEntity.MemberOrgCultures
                                   where o.OrgID == oOrgan.Gid && o.Ctype == nType && o.aCurrency == gCurrencyID
                                   select o).FirstOrDefault();
                            if (oOrgCurrency == null)
                            {
                                oOrgCurrency = new MemberOrgCulture
                                {
                                    Organization = oOrgan,
                                    Ctype = nType,
                                    aCurrency = gCurrencyID
                                };
                                dbEntity.MemberOrgCultures.Add(oOrgCurrency);
                            }
                            oOrgCurrency.Remark = sRemark;
                            dbEntity.SaveChanges();
                            break;
                    }
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2} {3}", this.ToString(), sOrgan, nType, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberOrgCulture成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberOrgCulture错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入用户角色
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportRoles(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastParent = "";
                MemberRole oParent = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    string sParent = row[colParent].ToString();
                    string sType = row[colType].ToString();
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sRemark = row[colRemark].ToString();

                    if (String.IsNullOrEmpty(sParent))
                    {
                        oParent = null;
                        sLastParent = "";
                    }
                    else if (sParent != sLastParent)
                    {
                        oParent = (from r in dbEntity.MemberRoles
                                   where r.OrgID == oOrgan.Gid && r.Code == sParent
                                   select r).FirstOrDefault();
                        sLastParent = sParent;
                    }
                    var oRole = (from r in dbEntity.MemberRoles
                                  where r.OrgID == oOrgan.Gid && r.Code == sCode
                                  select r).FirstOrDefault();
                    if (oRole == null)
                    {
                        oRole = new MemberRole { Code = sCode };
                        dbEntity.MemberRoles.Add(oRole);
                    }
                    oRole.Organization = oOrgan;
                    oRole.Parent = oParent;
                    oRole.RoleType = dbEntity.GeneralStandardCategorys.Where(c => c.Code == sType && c.Ctype == (byte)ModelEnum.StandardCategoryType.ROLE).FirstOrDefault();
                    if (oRole.Name == null)
                        oRole.Name = oName;
                    else
                        oRole.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oRole.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberRole成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberRole错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入用户级别
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportUserLevel(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colLevel = oExcel.ExcelTable.Columns["级别"];
                DataColumn colDiscount = oExcel.ExcelTable.Columns["折扣"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    byte nLevel;
                    Byte.TryParse(row[colLevel].ToString(), out nLevel);
                    decimal mDiscount = 1;
                    Decimal.TryParse(row[colDiscount].ToString(), out mDiscount);
                    string sRemark = row[colRemark].ToString();

                    var oLevel = (from l in dbEntity.MemberLevels
                                 where l.OrgID == oOrgan.Gid && l.Code == sCode
                                 select l).FirstOrDefault();
                    if (oLevel == null)
                    {
                        oLevel = new MemberLevel { Code = sCode };
                        dbEntity.MemberLevels.Add(oLevel);
                    }
                    oLevel.Organization = oOrgan;
                    if (oLevel.Name == null)
                        oLevel.Name = oName;
                    else
                        oLevel.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oLevel.Mlevel = nLevel;
                    oLevel.Discount = mDiscount;
                    oLevel.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberLevel成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberLevel错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportUsers(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colLoginName = oExcel.ExcelTable.Columns["登陆名"];
                DataColumn colOrgCode = oExcel.ExcelTable.Columns["组织"];
                DataColumn colRoleCode = oExcel.ExcelTable.Columns["角色"];
                DataColumn colChlCode = oExcel.ExcelTable.Columns["渠道"];
                DataColumn colManager = oExcel.ExcelTable.Columns["上级"];
                DataColumn colExCode = oExcel.ExcelTable.Columns["扩展码"];
                DataColumn colUstatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colUserLevel = oExcel.ExcelTable.Columns["级别"];
                DataColumn colNickName = oExcel.ExcelTable.Columns["昵称"];
                DataColumn colFirstName = oExcel.ExcelTable.Columns["名"];
                DataColumn colLastName = oExcel.ExcelTable.Columns["姓"];
                DataColumn colDisplayName = oExcel.ExcelTable.Columns["姓名"];
                DataColumn colPasscode = oExcel.ExcelTable.Columns["密码"];
                DataColumn colCulture = oExcel.ExcelTable.Columns["语言"];
                DataColumn colTitle = oExcel.ExcelTable.Columns["头衔"];
                DataColumn colGender = oExcel.ExcelTable.Columns["性别"];
                DataColumn colHeadPic = oExcel.ExcelTable.Columns["图标"];
                DataColumn colUserSign = oExcel.ExcelTable.Columns["签名"];
                DataColumn colBrief = oExcel.ExcelTable.Columns["简介"];
                DataColumn colBirthday = oExcel.ExcelTable.Columns["生日"];
                DataColumn colPhone = oExcel.ExcelTable.Columns["电话"];
                DataColumn colMobile = oExcel.ExcelTable.Columns["手机"];
                DataColumn colEmail = oExcel.ExcelTable.Columns["电子邮件"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sLoginName = row[colLoginName].ToString();
                    string sOrgCode = row[colOrgCode].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sRoleCode = row[colRoleCode].ToString();
                    string sChlCode = row[colChlCode].ToString();
                    string sManager = row[colManager].ToString();
                    string sExCode = row[colExCode].ToString();
                    byte nStatus;
                    Byte.TryParse(row[colUstatus].ToString(), out nStatus);
                    string sUserLevel = row[colUserLevel].ToString();
                    string sNickName = row[colNickName].ToString();
                    string sFirstName = row[colFirstName].ToString();
                    string sLastName = row[colLastName].ToString();
                    string sDisplayName = row[colDisplayName].ToString();
                    string sPasscode = row[colPasscode].ToString();
                    int nCulture;
                    Int32.TryParse(row[colCulture].ToString(), out nCulture);
                    string sTitle = row[colTitle].ToString();
                    byte nGender;
                    Byte.TryParse(row[colGender].ToString(), out nGender);
                    string sHeadPic = row[colHeadPic].ToString();
                    string sUserSign = row[colUserSign].ToString();
                    string sBrief = row[colBrief].ToString();
                    DateTimeOffset dtDateTime;
                    DateTimeOffset? dtBirthday = null;
                    if (DateTimeOffset.TryParse(row[colBirthday].ToString(), out dtDateTime))
                        dtBirthday = dtDateTime;
                    string sPhone = row[colPhone].ToString();
                    string sMobile = row[colMobile].ToString();
                    string sEmail = row[colEmail].ToString();
                    string sRemark = row[colRemark].ToString();

                    var oUser = (from u in dbEntity.MemberUsers
                                 where u.LoginName == sLoginName
                                 select u).FirstOrDefault();
                    if (oUser == null)
                    {
                        oUser = new MemberUser { LoginName = sLoginName };
                        dbEntity.MemberUsers.Add(oUser);
                    }
                    oUser.Organization = oOrgan;
                    oUser.Role = dbEntity.MemberRoles.Where(r => r.OrgID == oOrgan.Gid && r.Code == sRoleCode).FirstOrDefault();
                    oUser.Channel = dbEntity.MemberChannels.Where(c => c.Code == sChlCode && c.Otype == (byte)ModelEnum.OrganizationType.CHANNEL).FirstOrDefault();
                    oUser.Manager = dbEntity.MemberUsers.Where(u => u.LoginName == sManager).FirstOrDefault();
                    oUser.ExCode = sExCode;
                    oUser.Ustatus = nStatus;
                    oUser.UserLevel = dbEntity.MemberLevels.Where(l => l.OrgID == oOrgan.Gid && l.Code == sUserLevel).FirstOrDefault();
                    oUser.NickName = sNickName;
                    oUser.FirstName = sFirstName;
                    oUser.LastName = sLastName;
                    oUser.DisplayName = sDisplayName;
                    oUser.Passcode = sPasscode;
                    oUser.Culture = dbEntity.GeneralCultureUnits.Where(c => c.Culture == nCulture).FirstOrDefault();
                    oUser.Title= sTitle;
                    oUser.Gender = nGender;
                    oUser.HeadPic= sHeadPic;
                    oUser.UserSign = sUserSign;
                    oUser.Brief = sBrief;
                    oUser.Birthday = dtBirthday;
                    oUser.Email = sEmail;
                    oUser.Telephone = sPhone;
                    oUser.CellPhone = sMobile;
                    oUser.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sLoginName, sDisplayName);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberUser成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberUser错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入用户权限
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportPrivileges(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colLoginName = oExcel.ExcelTable.Columns["登陆名"];
                DataColumn colPrivType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colRefCode = oExcel.ExcelTable.Columns["授权代码"];
                DataColumn colNodeCode = oExcel.ExcelTable.Columns["节点代码"];
                DataColumn colNodeValue = oExcel.ExcelTable.Columns["节点值"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastUser = "";
                Guid gLastGuid = Guid.Empty;
                MemberUser oUser = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sLoginName = row[colLoginName].ToString();
                    byte nType;
                    Byte.TryParse(row[colPrivType].ToString(), out nType);
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    string sRefCode = row[colRefCode].ToString();
                    string sNodeCode = row[colNodeCode].ToString();
                    string sNodeValue = row[colNodeValue].ToString();
                    string sRemark = row[colRemark].ToString();

                    if (!String.IsNullOrEmpty(sLoginName))
                    {
                        oUser = (from u in dbEntity.MemberUsers
                                     where u.LoginName == sLoginName
                                     select u).FirstOrDefault();
                        sLastUser = sLoginName;
                    }
                    // 授权主表
                    var oPrivilege = (from p in dbEntity.MemberPrivileges
                                      where p.UserID == oUser.Gid && p.Ptype == nType
                                      select p).FirstOrDefault();
                    if (oPrivilege == null)
                    {
                        oPrivilege = new MemberPrivilege { User = oUser, Ptype = nType };
                        dbEntity.MemberPrivileges.Add(oPrivilege);
                    }
                    oPrivilege.Pstatus = nStatus;
                    dbEntity.SaveChanges();
                    // 授权项目表
                    Guid gRefID = Guid.Empty;
                    switch ((ModelEnum.UserPrivType)nType)
                    {
                        case ModelEnum.UserPrivType.PROGRAM:
                            var oProgram = (from p in dbEntity.GeneralPrograms
                                            where p.Code == sRefCode
                                            select p).FirstOrDefault();
                            gLastGuid = oProgram.Gid;
                            gRefID = oProgram.Gid;
                            break;
                        case ModelEnum.UserPrivType.PROGRAM_NODE:
                            var oProgNode = (from n in dbEntity.GeneralProgNodes
                                             where n.ProgID == gLastGuid && n.Code == sNodeCode
                                             select n).FirstOrDefault();
                            gRefID = oProgNode.Gid;
                            break;
                        case ModelEnum.UserPrivType.ORGANIZATION:
                            var oOrgan = (from o in dbEntity.MemberOrganizations
                                          where o.Code == sRefCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                          select o).FirstOrDefault();
                            gLastGuid = oOrgan.Gid;
                            gRefID = oOrgan.Gid;
                            break;
                        case ModelEnum.UserPrivType.CHANNEL:
                            var oChannel = (from o in dbEntity.MemberChannels
                                            where o.Code == sRefCode && o.Otype == (byte)ModelEnum.OrganizationType.CHANNEL
                                            select o).FirstOrDefault();
                            gLastGuid = oChannel.Gid;
                            gRefID = oChannel.Gid;
                            break;
                        case ModelEnum.UserPrivType.WAREHOUSE:
                            var oWarehouse = (from o in dbEntity.WarehouseInformations
                                            where o.Code == sRefCode && o.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE
                                            select o).FirstOrDefault();
                            gLastGuid = oWarehouse.Gid;
                            gRefID = oWarehouse.Gid;
                            break;
                        case ModelEnum.UserPrivType.PRODUCT_CATEGORY:
                            var oCategory1 = (from c in dbEntity.GeneralPrivateCategorys
                                             where c.Organization.Code == sRefCode
                                                   && c.Code == sNodeCode
                                                   && c.Ctype == (byte)ModelEnum.PrivateCategoryType.PRODUCT
                                             select c).FirstOrDefault();
                            gLastGuid = oCategory1.Gid;
                            gRefID = oCategory1.Gid;
                            break;
                        case ModelEnum.UserPrivType.SUPPLIER_CATEGORY:
                            var oCategory2 = (from c in dbEntity.GeneralPrivateCategorys
                                             where c.Organization.Code == sRefCode
                                                   && c.Code == sNodeCode
                                                   && c.Ctype == (byte)ModelEnum.PrivateCategoryType.SUPPLIER
                                             select c).FirstOrDefault();
                            gLastGuid = oCategory2.Gid;
                            gRefID = oCategory2.Gid;
                            break;
                    }
                    MemberPrivItem oPrivItem;
                    if (nType == (byte)ModelEnum.UserPrivType.PROGRAM_NODE)
                    {
                        oPrivItem = (from i in dbEntity.MemberPrivItems
                                     where i.PrivID == oPrivilege.Gid && i.RefID == gRefID && i.NodeCode == sNodeCode
                                     select i).FirstOrDefault();
                        if (oPrivItem == null)
                        {
                            oPrivItem = new MemberPrivItem { Privilege = oPrivilege, RefID = gRefID, NodeCode = sNodeCode };
                            dbEntity.MemberPrivItems.Add(oPrivItem);
                        }
                        oPrivItem.NodeValue = sNodeValue;
                    }
                    else
                    {
                        oPrivItem = (from i in dbEntity.MemberPrivItems
                                     where i.PrivID == oPrivilege.Gid && i.RefID == gRefID
                                     select i).FirstOrDefault();
                        if (oPrivItem == null)
                        {
                            oPrivItem = new MemberPrivItem { Privilege = oPrivilege, RefID = gRefID };
                            dbEntity.MemberPrivItems.Add(oPrivItem);
                        }
                    }
                    oPrivItem.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2} {3} {4}", this.ToString(), sLoginName, sRefCode, sNodeCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入MemberPrivilege成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberPrivilege错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入程序菜单和定义
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportPrograms(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                // 程序定义字段
                DataColumn colPgCode = oExcel.ExcelTable.Columns["程序代码"];
                DataColumn colPgParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colPgUrl = oExcel.ExcelTable.Columns["URL"];
                DataColumn colPgLeaf = oExcel.ExcelTable.Columns["叶子"];
                // 功能定义字段
                DataColumn colNdCode = oExcel.ExcelTable.Columns["功能代码"];
                DataColumn colNdOptCN = oExcel.ExcelTable.Columns["中文选项"];
                DataColumn colNdOptUS = oExcel.ExcelTable.Columns["英文选项"];
                DataColumn colNdMode = oExcel.ExcelTable.Columns["输入模式"];
                // 公共字段
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                // 导入
                GeneralProgram oLastProgram = null;
                string sLastParent = "";
                GeneralProgram oParent = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sPgCode = row[colPgCode].ToString();
                    string sPgParent = row[colPgParent].ToString();
                    string sPgUrl = row[colPgUrl].ToString();
                    bool bPgLeaf = row[colPgLeaf].ToString() == "1" ? true : false;

                    string sNdCode = row[colNdCode].ToString();
                    GeneralResource oNdOpt = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNdOptCN].ToString(), 1033, row[colNdOptUS].ToString());
                    byte nNdMode;
                    Byte.TryParse(row[colNdMode].ToString(), out nNdMode);

                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sRemark = row[colRemark].ToString();

                    if (!String.IsNullOrEmpty(sPgCode))
                    {
                        // 定义程序
                        if (String.IsNullOrEmpty(sPgParent))
                        {
                            oParent = null;
                            sLastParent = "";
                        }
                        else if (sPgParent != sLastParent)
                        {
                            oParent = (from p in dbEntity.GeneralPrograms
                                       where p.Code == sPgParent
                                       select p).FirstOrDefault();
                            sLastParent = sPgParent;
                        }
                        var oProgram = (from p in dbEntity.GeneralPrograms
                                        where p.Code == sPgCode
                                        select p).FirstOrDefault();
                        if (oProgram == null)
                        {
                            oProgram = new GeneralProgram { Code = sPgCode };
                            dbEntity.GeneralPrograms.Add(oProgram);
                        }
                        oProgram.Parent = oParent;
                        oProgram.ProgUrl = sPgUrl;
                        oProgram.Terminal = bPgLeaf;
                        if (oProgram.Name == null)
                            oProgram.Name = oName;
                        else
                            oProgram.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                        oProgram.Show = true;
                        oProgram.Remark = sRemark;
                        dbEntity.SaveChanges();

                        oLastProgram = oProgram;
                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("{0} {1} {2}", this.ToString(), sPgCode, sRemark);
                    }
                    else
                    {
                        // 定义功能点
                        var oProgNode = (from n in dbEntity.GeneralProgNodes
                                         where n.ProgID == oLastProgram.Gid && n.Code == sNdCode
                                         select n).FirstOrDefault();
                        if (oProgNode == null)
                        {
                            oProgNode = new GeneralProgNode { Program = oLastProgram, Code = sNdCode };
                            dbEntity.GeneralProgNodes.Add(oProgNode);
                        }
                        if (oProgNode.Name == null)
                            oProgNode.Name = oName;
                        else
                            oProgNode.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                        if (nNdMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
                        {
                            if (oProgNode.Optional == null)
                                oProgNode.Optional = oNdOpt;
                            else
                                oProgNode.Optional.SetResource(ModelEnum.ResourceType.STRING, oNdOpt);
                        }
                        oProgNode.InputMode = nNdMode;
                        oProgNode.Remark = sRemark;
                        dbEntity.SaveChanges();
                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("    {0} {1}", sNdCode, sRemark);
                    }
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralProgram/GeneralProgNode成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralProgram/GeneralProgNode错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入仓库货架
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportWarehouseShelves(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colWhCode = oExcel.ExcelTable.Columns["仓库"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colBarcode = oExcel.ExcelTable.Columns["条码"];
                DataColumn colReserved = oExcel.ExcelTable.Columns["保留"];
                DataColumn colName = oExcel.ExcelTable.Columns["名称"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgan = "";
                string sLastWarehouse = "";
                WarehouseInformation oWarehouse = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    string sWhCode = row[colWhCode].ToString();
                    string sCode = row[colCode].ToString();
                    string sBarcode = row[colBarcode].ToString();
                    bool bReserved = (row[colReserved].ToString() == "1") ? true : false;
                    string sName = row[colName].ToString();
                    string sRemark = row[colRemark].ToString();

                    if ((sWhCode != sLastWarehouse) || (sOrgCode != sLastOrgan))
                    {
                        oWarehouse = (from w in dbEntity.WarehouseInformations
                                      where w.Parent.Code == sOrgCode && w.Code == sWhCode
                                      select w).FirstOrDefault();
                        sLastOrgan = sOrgCode;
                        sLastWarehouse = sWhCode;
                    }
                    var oShelf = (from s in dbEntity.WarehouseShelves
                                  where s.WhID == oWarehouse.Gid && s.Code == sCode
                                  select s).FirstOrDefault();
                    if (oShelf == null)
                    {
                        oShelf = new WarehouseShelf { Warehouse = oWarehouse, Code = sCode };
                        dbEntity.WarehouseShelves.Add(oShelf);
                    }
                    if (String.IsNullOrEmpty(sBarcode))
                        oShelf.Barcode = sCode;
                    else
                        oShelf.Barcode = sBarcode;
                    oShelf.Reserved = bReserved;
                    oShelf.Name = sName;
                    oShelf.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入WarehouseShelf成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入WarehouseShelf错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入仓库支持的地区
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportWarehouseRegion(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colWhCode = oExcel.ExcelTable.Columns["仓库"];
                DataColumn colRegion = oExcel.ExcelTable.Columns["地区"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgan = "";
                string sLastWarehouse = "";
                WarehouseInformation oWarehouse = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    string sWhCode = row[colWhCode].ToString();
                    string sRegion = row[colRegion].ToString();
                    string sRemark = row[colRemark].ToString();

                    if ((sWhCode != sLastWarehouse) || (sOrgCode != sLastOrgan))
                    {
                        oWarehouse = (from w in dbEntity.WarehouseInformations
                                      where w.Parent.Code == sOrgCode && w.Code == sWhCode
                                      select w).FirstOrDefault();
                        sLastOrgan = sOrgCode;
                        sLastWarehouse = sWhCode;
                    }
                    var oRegion = (from r in dbEntity.GeneralRegions
                                   where r.Code == sRegion
                                   select r).FirstOrDefault();
                    if (oRegion != null)
                    {
                        var oWhRegion = (from r in dbEntity.WarehouseRegions
                                         where r.WhID == oWarehouse.Gid && r.RegionID == oRegion.Gid
                                         select r).FirstOrDefault();
                        if (oWhRegion == null)
                        {
                            oWhRegion = new WarehouseRegion { Warehouse = oWarehouse, Region = oRegion };
                            dbEntity.WarehouseRegions.Add(oWhRegion);
                        }
                        oWhRegion.Remark = sRemark;
                        dbEntity.SaveChanges();
                    }
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sRegion, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入WarehouseRegion成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入WarehouseRegion错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入仓库支持的承运商
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportWarehouseShipping(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colWhCode = oExcel.ExcelTable.Columns["仓库"];
                DataColumn colShipper = oExcel.ExcelTable.Columns["承运商"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgan = "";
                string sLastWarehouse = "";
                WarehouseInformation oWarehouse = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    string sWhCode = row[colWhCode].ToString();
                    string sShipper = row[colShipper].ToString();
                    string sRemark = row[colRemark].ToString();

                    if ((sWhCode != sLastWarehouse) || (sOrgCode != sLastOrgan))
                    {
                        oWarehouse = (from w in dbEntity.WarehouseInformations
                                      where w.Parent.Code == sOrgCode && w.Code == sWhCode
                                      select w).FirstOrDefault();
                        sLastOrgan = sOrgCode;
                        sLastWarehouse = sWhCode;
                    }
                    var oShipper = (from s in dbEntity.ShippingInformations
                                    where s.Parent.Code == sOrgCode && s.Code == sShipper
                                    select s).FirstOrDefault();
                    if (oShipper != null)
                    {
                        var oWhShipper = (from r in dbEntity.WarehouseShippings
                                       where r.WhID == oWarehouse.Gid && r.ShipID == oShipper.Gid
                                       select r).FirstOrDefault();
                        if (oWhShipper == null)
                        {
                            oWhShipper = new WarehouseShipping { Warehouse = oWarehouse, Shipper = oShipper };
                            dbEntity.WarehouseShippings.Add(oWhShipper);
                        }
                        oWhShipper.Remark = sRemark;
                        dbEntity.SaveChanges();
                    }
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sShipper, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入WarehouseShipping成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入WarehouseShipping错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 过时的算法：导入国家地区（中国）
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        [Obsolete]
        public void ImportChinaRegions(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colName = oExcel.ExcelTable.Columns["名称"];
                DataColumn colMap01 = oExcel.ExcelTable.Columns["映射01"];
                DataColumn colMap02 = oExcel.ExcelTable.Columns["映射02"];
                DataColumn colMap03 = oExcel.ExcelTable.Columns["映射03"];
                DataColumn colMap04 = oExcel.ExcelTable.Columns["映射04"];
                DataColumn colMap05 = oExcel.ExcelTable.Columns["映射05"];

                string sCountryCode = "CHN";                // 中国
                int nLevel = 0;
                var oCountry = (from r in dbEntity.GeneralRegions
                                where r.Code == sCountryCode
                                select r).FirstOrDefault();
                if (oCountry == null)
                {
                    oCountry = new GeneralRegion
                    {
                        Code = sCountryCode,
                        FullName = "中华人民共和国",
                        ShortName = "中国",
                        RegionLevel = nLevel
                    };
                    dbEntity.GeneralRegions.Add(oCountry);
                    dbEntity.SaveChanges();
                }
                // 全部删除
                dbEntity.Database.ExecuteSqlCommand("EXECUTE dbo.sp_ClearRegions {0}", oCountry.Gid);
                oCountry.Deleted = false;
                dbEntity.SaveChanges();

                string sLastParent = sCountryCode;
                string sThisParent = sCountryCode;
                GeneralRegion oParent = oCountry;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString().Trim();
                    string sName = row[colName].ToString().Trim();
                    string sMap01 = row[colMap01].ToString().Trim();
                    string sMap02 = row[colMap02].ToString().Trim();
                    string sMap03 = row[colMap03].ToString().Trim();
                    string sMap04 = row[colMap04].ToString().Trim();
                    string sMap05 = row[colMap05].ToString().Trim();
                    string sPrivance = sCode.Substring(0, 2);
                    string sCity = sCode.Substring(2, 2);
                    string sDistrict = sCode.Substring(4, 2);
                    if (sCity == "00")
                    {
                        sThisParent = sCountryCode;
                        nLevel = 1;
                    }
                    else if (sDistrict == "00")
                    {
                        sThisParent = sPrivance + "0000";
                        nLevel = 2;
                    }
                    else
                    {
                        sThisParent = sPrivance + sCity + "00";
                        nLevel = 3;
                    }
                    if (String.IsNullOrEmpty(sThisParent))
                    {
                        oParent = null;
                        sLastParent = "";
                    }
                    else if (sThisParent != sLastParent)
                    {
                        oParent = (from r in dbEntity.GeneralRegions
                                   where r.Code == sThisParent
                                   select r).FirstOrDefault();
                        sLastParent = sThisParent;
                    }
                    var oRegion = (from r in dbEntity.GeneralRegions
                                   where r.Code == sCode
                                   select r).FirstOrDefault();
                    if (oRegion == null)
                    {
                        oRegion = new GeneralRegion { Code = sCode };
                        dbEntity.GeneralRegions.Add(oRegion);
                    }
                    oRegion.Deleted = false;
                    oRegion.Parent = oParent;
                    oRegion.FullName = sName;
                    oRegion.ShortName = sName;
                    oRegion.Map01 = sMap01;
                    oRegion.Map02 = sMap02;
                    oRegion.Map03 = sMap03;
                    oRegion.Map04 = sMap04;
                    oRegion.Map05 = sMap05;
                    oRegion.RegionLevel = nLevel;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sName);
                }
                oEventBLL.WriteEvent(String.Format("导入国家地区(中国)成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入国家地区(中国)错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入国家地区（通用算法）
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        public void ImportRegions(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colFullName = oExcel.ExcelTable.Columns["名称"];
                DataColumn colShortName = oExcel.ExcelTable.Columns["简称"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colLevel = oExcel.ExcelTable.Columns["级别"];
                DataColumn colPostCode = oExcel.ExcelTable.Columns["邮编"];
                DataColumn colDeleted = oExcel.ExcelTable.Columns["删除"];
                DataColumn colMap01 = oExcel.ExcelTable.Columns["映射01"];
                DataColumn colMap02 = oExcel.ExcelTable.Columns["映射02"];
                DataColumn colMap03 = oExcel.ExcelTable.Columns["映射03"];
                DataColumn colMap04 = oExcel.ExcelTable.Columns["映射04"];
                DataColumn colMap05 = oExcel.ExcelTable.Columns["映射05"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                // 全部删除
                //dbEntity.Database.ExecuteSqlCommand("EXECUTE dbo.sp_ClearRegions {0}", oCountry.Gid);
                //dbEntity.SaveChanges();

                GeneralRegion oParent = null;
                string sLastParent = "";
                Guid? oParentID = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sCode = row[colCode].ToString().Trim();
                    string sFullName = row[colFullName].ToString().Trim();
                    string sShortName = row[colShortName].ToString().Trim();
                    string sParent = row[colParent].ToString().Trim();
                    int nLevel = 0;
                    Int32.TryParse(row[colLevel].ToString(), out nLevel);
                    string sPostCode = row[colPostCode].ToString().Trim();
                    bool bDeleted = (row[colDeleted].ToString() == "1") ? true : false;
                    string sMap01 = row[colMap01].ToString().Trim();
                    string sMap02 = row[colMap02].ToString().Trim();
                    string sMap03 = row[colMap03].ToString().Trim();
                    string sMap04 = row[colMap04].ToString().Trim();
                    string sMap05 = row[colMap05].ToString().Trim();
                    string sRemark = row[colRemark].ToString().Trim();
                    
                    if (sParent != sLastParent)
                    {
                        oParent = (from r in dbEntity.GeneralRegions
                                   where r.Code == sParent
                                   select r).FirstOrDefault();
                        sLastParent = sParent;
                        if (oParent == null)
                            oParentID = null;
                        else
                            oParentID = oParent.Gid;
                    }
                    var oRegion = (from r in dbEntity.GeneralRegions
                                   where r.aParent == oParentID && r.Code == sCode
                                   select r).FirstOrDefault();
                    if (oRegion == null)
                    {
                        oRegion = new GeneralRegion { Parent = oParent, Code = sCode };
                        dbEntity.GeneralRegions.Add(oRegion);
                    }
                    oRegion.FullName = sFullName;
                    oRegion.ShortName = sShortName;
                    oRegion.RegionLevel = nLevel;
                    oRegion.PostCode = sPostCode;
                    oRegion.Deleted = bDeleted;
                    oRegion.Map01 = sMap01;
                    oRegion.Map02 = sMap02;
                    oRegion.Map03 = sMap03;
                    oRegion.Map04 = sMap04;
                    oRegion.Map05 = sMap05;
                    oRegion.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sFullName);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralRegion国家地区成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralRegion国家地区错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入私有分类
        /// </summary>
        /// <param name="sExcelFile"></param>
        /// <param name="sSheetName"></param>
        public void ImportPrivateCategories(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colParent = oExcel.ExcelTable.Columns["上级"];
                DataColumn colType = oExcel.ExcelTable.Columns["类型"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colSort = oExcel.ExcelTable.Columns["排序"];
                DataColumn colUnitType = oExcel.ExcelTable.Columns["单位类型"];
                DataColumn colUnitCode = oExcel.ExcelTable.Columns["计量单位"];
                DataColumn colShow = oExcel.ExcelTable.Columns["是否显示"];
                DataColumn colGuarantee = oExcel.ExcelTable.Columns["显示保质期"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastParent = "";
                GeneralPrivateCategory oParent = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    string sParent = row[colParent].ToString();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    int nSort;
                    Int32.TryParse(row[colSort].ToString(), out nSort);
                    byte nUnitType;
                    Byte.TryParse(row[colUnitType].ToString(), out nUnitType);
                    string sUnitCode = row[colUnitCode].ToString();
                    bool bShow = row[colShow].ToString() == "1" ? true : false;
                    bool bGuarantee = row[colGuarantee].ToString() == "1" ? true : false;
                    string sRemark = row[colRemark].ToString();

                    if (String.IsNullOrEmpty(sParent))
                    {
                        oParent = null;
                        sLastParent = "";
                    }
                    else if (sParent != sLastParent)
                    {
                        oParent = (from c in dbEntity.GeneralPrivateCategorys
                                   where c.OrgID == oOrgan.Gid && c.Ctype == nType && c.Code == sParent
                                   select c).FirstOrDefault();
                        sLastParent = sParent;
                    }
                    var oCategory = (from c in dbEntity.GeneralPrivateCategorys
                                     where c.OrgID == oOrgan.Gid && c.Ctype == nType && c.Code == sCode
                                     select c).FirstOrDefault();
                    if (oCategory == null)
                    {
                        oCategory = new GeneralPrivateCategory { Organization = oOrgan, Ctype = nType, Code = sCode };
                        dbEntity.GeneralPrivateCategorys.Add(oCategory);
                    }
                    oCategory.Parent = oParent;
                    if (oCategory.Name == null)
                        oCategory.Name = oName;
                    else
                        oCategory.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oCategory.Sorting = nSort;
                    oCategory.StandardUnit = dbEntity.GeneralMeasureUnits.Where(u => u.Utype == nUnitType && u.Code == sUnitCode).FirstOrDefault();
                    oCategory.Show = bShow;
                    oCategory.ShowGuarantee = bGuarantee;
                    oCategory.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralPrivateCategory成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralPrivateCategory错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }
		
        /// <summary>
        /// 导入商品定义，包括PU和SKU
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportProduct(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colProdCode = oExcel.ExcelTable.Columns["PU代码"];
                DataColumn colSkuCode = oExcel.ExcelTable.Columns["SKU代码"];
                DataColumn colBarcode = oExcel.ExcelTable.Columns["条码"];
                DataColumn colExCode1 = oExcel.ExcelTable.Columns["自定义编码"];
                DataColumn colProdNameCN = oExcel.ExcelTable.Columns["PU中文名称"];
                DataColumn colSkuNameCN = oExcel.ExcelTable.Columns["SKU中文名称"];
                DataColumn colStdCat = oExcel.ExcelTable.Columns["标准分类"];
                DataColumn colPrvCat = oExcel.ExcelTable.Columns["私有分类"];
                DataColumn colBlock = oExcel.ExcelTable.Columns["拆单分组标志"];
                DataColumn colMode = oExcel.ExcelTable.Columns["产品模式"];
                DataColumn colUnitType = oExcel.ExcelTable.Columns["单位类型"];
                DataColumn colUnitCode = oExcel.ExcelTable.Columns["单位代码"];
                DataColumn colPercision = oExcel.ExcelTable.Columns["计量精度"];
                DataColumn colSpecification = oExcel.ExcelTable.Columns["规格"];
                DataColumn colProdPicture = oExcel.ExcelTable.Columns["主图路径"];
                DataColumn colBriefCN = oExcel.ExcelTable.Columns["简单描述"];
                DataColumn colIntroCN = oExcel.ExcelTable.Columns["详细描述"];
                DataColumn colMinQuantity = oExcel.ExcelTable.Columns["起订量"];
                DataColumn colCycle = oExcel.ExcelTable.Columns["生产周期"];
                DataColumn colGuarantee = oExcel.ExcelTable.Columns["保质期"];
                DataColumn colMarketPriceRMB = oExcel.ExcelTable.Columns["市场价￥"];
                DataColumn colSuggestPriceRMB = oExcel.ExcelTable.Columns["建议价￥"];
                DataColumn colLowestPriceRMB = oExcel.ExcelTable.Columns["最低价￥"];
                DataColumn colKeywords = oExcel.ExcelTable.Columns["关键词"];
                DataColumn colGrossWeight = oExcel.ExcelTable.Columns["毛重"];
                DataColumn colNetWeight = oExcel.ExcelTable.Columns["净重"];
                DataColumn colGrossVolume = oExcel.ExcelTable.Columns["毛体积"];
                DataColumn colNetVolume = oExcel.ExcelTable.Columns["净体积"];
                DataColumn colNetPiece = oExcel.ExcelTable.Columns["计件"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                ProductInformation oProduct = null;
                MemberOrganization oOrgan = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgan = row[colOrgan].ToString();
                    if (!String.IsNullOrEmpty(sOrgan))
                        oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgan && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sProdCode = row[colProdCode].ToString();
                    string sSkuCode = row[colSkuCode].ToString();
                    if (String.IsNullOrEmpty(sSkuCode)) continue;
                    string sBarcode = row[colBarcode].ToString();
                    string sExCode1 = row[colExCode1].ToString();
                    GeneralResource oProdName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colProdNameCN].ToString());
                    GeneralResource oSkuName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colSkuNameCN].ToString());
                    GeneralResource oFullName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colProdNameCN].ToString() + row[colSkuNameCN].ToString());
                    GeneralResource oShortName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colSkuNameCN].ToString());
                    string sStdCat = row[colStdCat].ToString();
                    string sPrvCat = row[colPrvCat].ToString();
                    byte nBlock;
                    Byte.TryParse(row[colBlock].ToString(), out nBlock);
                    byte nMode;
                    Byte.TryParse(row[colMode].ToString(), out nMode);
                    byte nUnitType;
                    Byte.TryParse(row[colUnitType].ToString(), out nUnitType);
                    string sUnitCode = row[colUnitCode].ToString();
                    byte nPercision;
                    Byte.TryParse(row[colPercision].ToString(), out nPercision);
                    GeneralResource oSpecification = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colSpecification].ToString());
                    string sProdPicture = row[colProdPicture].ToString();
                    GeneralResource oBrief = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colBriefCN].ToString());
                    GeneralLargeObject oIntro = new GeneralLargeObject(2052, row[colIntroCN].ToString());
                    decimal nMinQuantity;
                    Decimal.TryParse(row[colMinQuantity].ToString(), out nMinQuantity);
                    int nCycle;
                    Int32.TryParse(row[colCycle].ToString(), out nCycle);
                    int nGuarantee;
                    Int32.TryParse(row[colGuarantee].ToString(), out nGuarantee);
                    decimal nMarketPriceRMB;
                    Decimal.TryParse(row[colMarketPriceRMB].ToString(), out nMarketPriceRMB);
                    decimal nSuggestPriceRMB;
                    Decimal.TryParse(row[colSuggestPriceRMB].ToString(), out nSuggestPriceRMB);
                    decimal nLowestPriceRMB;
                    Decimal.TryParse(row[colLowestPriceRMB].ToString(), out nLowestPriceRMB);
                    string sKeywords = row[colKeywords].ToString();
                    decimal nGrossWeight;
                    Decimal.TryParse(row[colGrossWeight].ToString(), out nGrossWeight);
                    decimal nNetWeight;
                    Decimal.TryParse(row[colNetWeight].ToString(), out nNetWeight);
                    decimal nGrossVolume;
                    Decimal.TryParse(row[colGrossVolume].ToString(), out nGrossVolume);
                    decimal nNetVolume;
                    Decimal.TryParse(row[colNetVolume].ToString(), out nNetVolume);
                    int nNetPiece;
                    Int32.TryParse(row[colNetPiece].ToString(), out nNetPiece);
                    string sRemark = row[colRemark].ToString();

                    if (!String.IsNullOrEmpty(sProdCode))
                    {
                        // 导入产品主表
                        oProduct = (from p in dbEntity.ProductInformations
                                    where p.OrgID == oOrgan.Gid && p.Code == sProdCode
                                    select p).FirstOrDefault();
                        if (oProduct == null)
                        {
                            oProduct = new ProductInformation { Organization = oOrgan, Code = sProdCode };
                            dbEntity.ProductInformations.Add(oProduct);
                        }
                        if (oProduct.Name == null)
                            oProduct.Name = oProdName;
                        else
                            oProduct.Name.SetResource(ModelEnum.ResourceType.STRING, oProdName);
                        oProduct.StandardCategory = dbEntity.GeneralStandardCategorys.Where(c => c.Code == sStdCat && c.Ctype == (byte)ModelEnum.StandardCategoryType.PRODUCT).FirstOrDefault();
                        oProduct.Block = nBlock;
                        oProduct.Mode = nMode;
                        oProduct.Picture = sProdPicture;
                        if (oProduct.Brief == null)
                            oProduct.Brief = oBrief;
                        else
                            oProduct.Brief.SetResource(ModelEnum.ResourceType.STRING, oBrief);
                        if (oProduct.Matter == null)
                            oProduct.Matter = oIntro;
                        else
                            oProduct.Matter.SetLargeObject(oIntro);
                        oProduct.MinQuantity = nMinQuantity;
                        oProduct.ProductionCycle = nCycle;
                        oProduct.GuaranteeDays = nGuarantee;
                        oProduct.Keywords = sKeywords;
                        oProduct.Remark = sRemark;
                        dbEntity.SaveChanges();
                    }
                    // 导入产品SKU表
                    var oProdItem = (from i in dbEntity.ProductInfoItems
                                     where i.OrgID == oOrgan.Gid && i.Code == sSkuCode
                                     select i).FirstOrDefault();
                    if (oProdItem == null)
                    {
                        oProdItem = new ProductInfoItem { Organization = oOrgan, Code = sSkuCode };
                        dbEntity.ProductInfoItems.Add(oProdItem);
                    }
                    oProdItem.Product = oProduct;
                    oProdItem.Barcode = (String.IsNullOrEmpty(sBarcode)) ? sSkuCode : sBarcode;
                    oProdItem.CodeEx1 = sExCode1;
                    if (oProdItem.FullName == null)
                        oProdItem.FullName = oFullName;
                    else
                        oProdItem.FullName.SetResource(ModelEnum.ResourceType.STRING, oFullName);
                    if (oProdItem.ShortName == null)
                        oProdItem.ShortName = oShortName;
                    else
                        oProdItem.ShortName.SetResource(ModelEnum.ResourceType.STRING, oShortName);
                    oProdItem.StandardUnit = dbEntity.GeneralMeasureUnits.Where(u => u.Code == sUnitCode && u.Utype == nUnitType).FirstOrDefault();
                    if (oProdItem.Specification == null)
                        oProdItem.Specification = oSpecification;
                    else
                        oProdItem.Specification.SetResource(ModelEnum.ResourceType.STRING, oSpecification);
                    var oCurrency = (from u in dbEntity.GeneralMeasureUnits
                                     where u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == "¥"
                                     select u).FirstOrDefault();
                    if (oProdItem.MarketPrice == null)
                        oProdItem.MarketPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nMarketPriceRMB);
                    else
                        oProdItem.MarketPrice.SetResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nMarketPriceRMB);
                    if (oProdItem.SuggestPrice == null)
                        oProdItem.SuggestPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nSuggestPriceRMB);
                    else
                        oProdItem.SuggestPrice.SetResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nSuggestPriceRMB);
                    if (oProdItem.LowestPrice == null)
                        oProdItem.LowestPrice = new GeneralResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nLowestPriceRMB);
                    else
                        oProdItem.LowestPrice.SetResource(ModelEnum.ResourceType.MONEY, oCurrency.Gid, nLowestPriceRMB);
                    oProdItem.GrossWeight = nGrossWeight;
                    oProdItem.NetWeight = nNetWeight;
                    oProdItem.GrossVolume = nGrossVolume;
                    oProdItem.NetVolume = nNetVolume;
                    oProdItem.NetPiece = nNetPiece;
                    oProdItem.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2} {3} {4}", this.ToString(), sProdCode, oProdName.Matter, oSkuName.Matter, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入ProductInformation成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入ProductInformation错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入商品上架模板
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        [Obsolete]
        public void ImportOnTemplate(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colShipPolicy = oExcel.ExcelTable.Columns["运输策略"];
                DataColumn colPayPolicy = oExcel.ExcelTable.Columns["支付策略"];
                DataColumn colRelation = oExcel.ExcelTable.Columns["关联商品"];
                DataColumn colLevelDiscount = oExcel.ExcelTable.Columns["等级折扣"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrganCode = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrganCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sShipPolicy = row[colShipPolicy].ToString();
                    string sPayPolicy = row[colPayPolicy].ToString();
                    string sRelation = row[colRelation].ToString();
                    string sLevelDiscount = row[colLevelDiscount].ToString();
                    string sRemark = row[colRemark].ToString();

                    var oTemplate = (from t in dbEntity.ProductOnTemplates
                                     where t.OrgID == oOrgan.Gid && t.Code == sCode
                                     select t).FirstOrDefault();
                    if (oTemplate == null)
                    {
                        oTemplate = new ProductOnTemplate { Organization = oOrgan, Code = sCode };
                        dbEntity.ProductOnTemplates.Add(oTemplate);
                    }
                    if (oTemplate.Name == null)
                        oTemplate.Name = oName;
                    else
                        oTemplate.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oTemplate.ShipPolicy = sShipPolicy;
                    oTemplate.PayPolicy = sPayPolicy;
                    oTemplate.Relation = sRelation;
                    oTemplate.LevelDiscount = sLevelDiscount;
                    oTemplate.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入ProductOnTemplate成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入ProductOnTemplate错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 商品批量上架
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportProductOnSale(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colChannel = oExcel.ExcelTable.Columns["渠道"];
                DataColumn colTemplate = oExcel.ExcelTable.Columns["套用模板"];
                
                DataColumn colProdCode = oExcel.ExcelTable.Columns["PU代码"];
                DataColumn colSkuCode = oExcel.ExcelTable.Columns["SKU代码"];
                
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colUnitType = oExcel.ExcelTable.Columns["单位类型"];
                DataColumn colUnitCode = oExcel.ExcelTable.Columns["单位代码"];
                DataColumn colRatio = oExcel.ExcelTable.Columns["转换比率"];
                DataColumn colPercision = oExcel.ExcelTable.Columns["计量精度"];

                DataColumn colCurrency1 = oExcel.ExcelTable.Columns["货币1"];
                DataColumn colMarketPrice1 = oExcel.ExcelTable.Columns["市场价1"];
                DataColumn colSalePrice1 = oExcel.ExcelTable.Columns["销售价1"];

                DataColumn colCurrency2 = oExcel.ExcelTable.Columns["货币2"];
                DataColumn colMarketPrice2 = oExcel.ExcelTable.Columns["市场价2"];
                DataColumn colSalePrice2 = oExcel.ExcelTable.Columns["销售价2"];

                DataColumn colCurrency3 = oExcel.ExcelTable.Columns["货币3"];
                DataColumn colMarketPrice3 = oExcel.ExcelTable.Columns["市场价3"];
                DataColumn colSalePrice3 = oExcel.ExcelTable.Columns["销售价3"];

                DataColumn colCurrency4 = oExcel.ExcelTable.Columns["货币4"];
                DataColumn colMarketPrice4 = oExcel.ExcelTable.Columns["市场价4"];
                DataColumn colSalePrice4 = oExcel.ExcelTable.Columns["销售价4"];
                
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgCode = "";
                string sLastChannel = "";
                string sLastTemplate = "";
                string sLastProdCode = "";
                MemberOrganization oOrgan = null;
                MemberChannel oChannel = null;
                ProductOnTemplate oTemplate = null;
                ProductInformation oProduct = null;
                
                List<Object> oItemList = new List<Object>();
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    if (!String.IsNullOrEmpty(sOrgCode) && (sOrgCode != sLastOrgCode))
                    {
                        oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                        sLastOrgCode = sOrgCode;
                    }
                    string sChannel = row[colChannel].ToString();
                    if (!String.IsNullOrEmpty(sChannel) && (sChannel != sLastChannel))
                    {
                        oChannel = (from c in dbEntity.MemberChannels
                                    where c.Code == sChannel && c.Otype == (byte)ModelEnum.OrganizationType.CHANNEL
                                    select c).FirstOrDefault();
                        sLastChannel = sChannel;
                    }
                    string sTemplate = row[colTemplate].ToString();
                    if (!String.IsNullOrEmpty(sTemplate) && (sTemplate != sLastTemplate))
                    {
                        oTemplate = (from t in dbEntity.ProductOnTemplates
                                     where t.Deleted == false && t.OrgID == oOrgan.Gid && t.Code == sTemplate
                                     select t).FirstOrDefault();
                        sLastTemplate = sTemplate;
                    }
                    string sProdCode = row[colProdCode].ToString();
                    if (!String.IsNullOrEmpty(sProdCode) && (sProdCode != sLastProdCode))
                    {
                        if (oItemList.Count() > 0)
                        {
                            oProductBLL.ProductTemplateOnSale(oOrgan, oChannel, oTemplate, oProduct, oItemList);
                            //Test(oOrgan, oChannel, oTemplate, oProduct, oItemList);
                            oItemList.Clear();
                        }
                        oProduct = (from p in dbEntity.ProductInformations
                                    where p.Deleted == false && p.OrgID == oOrgan.Gid && p.Code == sProdCode
                                    select p).FirstOrDefault();
                        sLastProdCode = sProdCode;
                    }
                    string sSkuCode = row[colSkuCode].ToString();
                    var oInfoItem = (from i in dbEntity.ProductInfoItems
                                     where i.Deleted == false && i.ProdID == oProduct.Gid && i.Code == sSkuCode
                                     select i).FirstOrDefault();
                    
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    
                    byte nUnitType;
                    Byte.TryParse(row[colUnitType].ToString(), out nUnitType);
                    string sUnitCode = row[colUnitCode].ToString();
                    var oUnit = (from u in dbEntity.GeneralMeasureUnits
                                 where u.Deleted == false && u.Utype == nUnitType && u.Code == sUnitCode
                                 select u).FirstOrDefault();
                    
                    decimal mRatio;
                    Decimal.TryParse(row[colRatio].ToString(), out mRatio);
                    byte nPercision;
                    Byte.TryParse(row[colPercision].ToString(), out nPercision);

                    string sCurrency1 = row[colCurrency1].ToString();
                    decimal mMarketPrice1;
                    Decimal.TryParse(row[colMarketPrice1].ToString(), out mMarketPrice1);
                    decimal mSalePrice1;
                    Decimal.TryParse(row[colSalePrice1].ToString(), out mSalePrice1);

                    string sCurrency2 = row[colCurrency2].ToString();
                    decimal mMarketPrice2;
                    Decimal.TryParse(row[colMarketPrice2].ToString(), out mMarketPrice2);
                    decimal mSalePrice2;
                    Decimal.TryParse(row[colSalePrice2].ToString(), out mSalePrice2);

                    string sCurrency3 = row[colCurrency3].ToString();
                    decimal mMarketPrice3;
                    Decimal.TryParse(row[colMarketPrice3].ToString(), out mMarketPrice3);
                    decimal mSalePrice3;
                    Decimal.TryParse(row[colSalePrice3].ToString(), out mSalePrice3);

                    string sCurrency4 = row[colCurrency4].ToString();
                    decimal mMarketPrice4;
                    Decimal.TryParse(row[colMarketPrice4].ToString(), out mMarketPrice4);
                    decimal mSalePrice4;
                    Decimal.TryParse(row[colSalePrice4].ToString(), out mSalePrice4);

                    string sRemark = row[colRemark].ToString();
                    if ((oInfoItem != null) && (oUnit != null))
                    {
                        Dictionary<string, object> oParams = new Dictionary<string, object>();
                        oParams.Add("Status", nStatus);
                        oParams.Add("SkuItem", oInfoItem);
                        oParams.Add("Unit", oUnit);
                        oParams.Add("Ratio", mRatio);
                        oParams.Add("Percision", nPercision);

                        oParams.Add("Currency1", dbEntity.GeneralMeasureUnits.Where(u => u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == sCurrency1).FirstOrDefault());
                        oParams.Add("MarketPrice1", mMarketPrice1);
                        oParams.Add("SalePrice1", mSalePrice1);
                        
                        oParams.Add("Currency2", dbEntity.GeneralMeasureUnits.Where(u => u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == sCurrency2).FirstOrDefault());
                        oParams.Add("MarketPrice2", mMarketPrice2);
                        oParams.Add("SalePrice2", mSalePrice2);
                        
                        oParams.Add("Currency3", dbEntity.GeneralMeasureUnits.Where(u => u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == sCurrency3).FirstOrDefault());
                        oParams.Add("MarketPrice3", mMarketPrice3);
                        oParams.Add("SalePrice3", mSalePrice3);
                        
                        oParams.Add("Currency4", dbEntity.GeneralMeasureUnits.Where(u => u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Code == sCurrency4).FirstOrDefault());
                        oParams.Add("MarketPrice4", mMarketPrice4);
                        oParams.Add("SalePrice4", mSalePrice4);

                        oParams.Add("Remark", sRemark);
                        oItemList.Add(oParams);
                    }
                }
                oEventBLL.WriteEvent(String.Format("导入ProductOnSale成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入ProductOnSale错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }



        private void Test(MemberOrganization oOrgan, MemberChannel oChannel, ProductOnTemplate oTemplate,
            ProductInformation oProduct, List<Object> oItemList = null)
        {
            Debug.WriteLine(oOrgan.Code);
            Debug.WriteLine(oChannel.Code);
            Debug.WriteLine(oTemplate.Code);
            Debug.WriteLine(oProduct.Code + oProduct.Name.Matter);
            foreach (Dictionary<string, object> item in oItemList)
            {
                Debug.WriteLine(item["Status"].ToString());
                
                ProductInfoItem oInfoItem = (ProductInfoItem)item["SkuItem"];
                Debug.WriteLine(oInfoItem.Code + oInfoItem.FullName.Matter);
                
                GeneralMeasureUnit oUnit = (GeneralMeasureUnit)item["Unit"];
                Debug.WriteLine(oUnit.Code);

                GeneralMeasureUnit oCurrency1 = (GeneralMeasureUnit)item["Currency1"];
                if (oCurrency1 != null)
                    Debug.WriteLine(oCurrency1.Code);
            }
        }
        
        /// <summary>
        /// 导入支付方式
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportPaymentType(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colMatter = oExcel.ExcelTable.Columns["简单描述"];
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colSorting = oExcel.ExcelTable.Columns["排序"];
                DataColumn colIsCod = oExcel.ExcelTable.Columns["货到付款"];
                DataColumn colIsOnline = oExcel.ExcelTable.Columns["在线支付"];
                DataColumn colIsSecured = oExcel.ExcelTable.Columns["担保交易"];
                DataColumn colFee = oExcel.ExcelTable.Columns["手续费率"];
                DataColumn colConfig = oExcel.ExcelTable.Columns["配置密钥"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrganCode = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrganCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    string sMatter = row[colMatter].ToString();
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    int nSorting;
                    Int32.TryParse(row[colSorting].ToString(), out nSorting);
                    bool bIsCod = (row[colIsCod].ToString() == "1") ? true : false;
                    bool bIsOnline = (row[colIsOnline].ToString() == "1") ? true : false;
                    bool bIsSecured = (row[colIsSecured].ToString() == "1") ? true : false;
                    decimal mFee;
                    Decimal.TryParse(row[colFee].ToString(), out mFee);
                    string sConfig = row[colConfig].ToString();
                    string sRemark = row[colRemark].ToString();

                    var oPayType = (from t in dbEntity.FinancePayTypes
                                    where t.OrgID == oOrgan.Gid && t.Code == sCode
                                    select t).FirstOrDefault();
                    if (oPayType == null)
                    {
                        oPayType = new FinancePayType { Organization = oOrgan, Code = sCode };
                        dbEntity.FinancePayTypes.Add(oPayType);
                    }
                    if (oPayType.Name == null)
                        oPayType.Name = oName;
                    else
                        oPayType.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    oPayType.Matter = sMatter;
                    oPayType.Pstatus = nStatus;
                    oPayType.Sorting = nSorting;
                    oPayType.IsCod = bIsCod;
                    oPayType.IsOnline = bIsOnline;
                    oPayType.IsSecured = bIsSecured;
                    oPayType.Fee = mFee;
                    oPayType.Config = sConfig;
                    oPayType.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入FinancePayType成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入FinancePayType错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入消息模板
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportMessageTemplate(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colMatterCN = oExcel.ExcelTable.Columns["中文内容"];
                DataColumn colMatterUS = oExcel.ExcelTable.Columns["英文内容"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrganCode = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrganCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    GeneralLargeObject oMatter = new GeneralLargeObject(2052, row[colMatterCN].ToString(), 1033, row[colMatterUS].ToString());
                    string sRemark = row[colRemark].ToString();

                    var oTemplate = (from t in dbEntity.GeneralMessageTemplates
                                     where t.OrgID == oOrgan.Gid && t.Code == sCode
                                     select t).FirstOrDefault();
                    if (oTemplate == null)
                    {
                        oTemplate = new GeneralMessageTemplate { Organization = oOrgan, Code = sCode };
                        dbEntity.GeneralMessageTemplates.Add(oTemplate);
                    }
                    if (oTemplate.Name == null)
                        oTemplate.Name = oName;
                    else
                        oTemplate.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                    if (oTemplate.Matter == null)
                        oTemplate.Matter = oMatter;
                    else
                        oTemplate.Matter.SetLargeObject(oMatter);
                    oTemplate.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralMessageTemplate成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralMessageTemplate错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入联盟返点
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportUnion(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colChannel = oExcel.ExcelTable.Columns["渠道"];
                DataColumn colRole = oExcel.ExcelTable.Columns["角色"];
                DataColumn colBackLevel = oExcel.ExcelTable.Columns["向上层级"];
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colPercent1 = oExcel.ExcelTable.Columns["有上线比例"];
                DataColumn colPercent2 = oExcel.ExcelTable.Columns["无上线比例"];
                DataColumn colCashier = oExcel.ExcelTable.Columns["提现"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    var oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                    string sChannel = row[colChannel].ToString();
                    var oChannel = (from c in dbEntity.MemberChannels
                                    where c.Code == sChannel && c.Otype == (byte)ModelEnum.OrganizationType.CHANNEL
                                    select c).FirstOrDefault();
                    string sRole = row[colRole].ToString();
                    var oRole = (from r in dbEntity.MemberRoles
                                 where r.OrgID == oOrgan.Gid && r.Code == sRole
                                 select r).FirstOrDefault();
                    int nBackLevel;
                    Int32.TryParse(row[colBackLevel].ToString(), out nBackLevel);
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    decimal mPercent1;
                    Decimal.TryParse(row[colPercent1].ToString(), out mPercent1);
                    decimal mPercent2;
                    Decimal.TryParse(row[colPercent2].ToString(), out mPercent2);
                    bool bCashier = (row[colCashier].ToString() == "1") ? true : false;
                    string sRemark = row[colRemark].ToString();

                    var oUnion = (from u in dbEntity.UnionLevelPercents
                                  where u.OrgID == oOrgan.Gid && u.RoleID == oRole.Gid && u.BackLevel == nBackLevel
                                  select u).FirstOrDefault();
                    if (oUnion == null)
                    {
                        oUnion = new UnionLevelPercent { Organization = oOrgan, Role = oRole, BackLevel = nBackLevel };
                        dbEntity.UnionLevelPercents.Add(oUnion);
                    }
                    oUnion.Ustatus = nStatus;
                    oUnion.Percentage = mPercent1;
                    oUnion.PercentTop = mPercent2;
                    oUnion.Cashier = bCashier;
                    oUnion.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sOrgCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入UnionLevelPercent成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入UnionLevelPercent错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入属性定义
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportOptional(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colType = oExcel.ExcelTable.Columns["用途"];
                DataColumn colMainCode = oExcel.ExcelTable.Columns["属性代码"];
                DataColumn colOptCode = oExcel.ExcelTable.Columns["选项代码"];
                DataColumn colSorting = oExcel.ExcelTable.Columns["排序"];
                DataColumn colNameCN = oExcel.ExcelTable.Columns["中文名称"];
                DataColumn colNameUS = oExcel.ExcelTable.Columns["英文名称"];
                DataColumn colMode = oExcel.ExcelTable.Columns["输入模式"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgCode = "";
                MemberOrganization oOrgan = null;
                GeneralOptional oOption = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    byte nType;
                    Byte.TryParse(row[colType].ToString(), out nType);
                    string sMainCode = row[colMainCode].ToString();
                    string sOptCode = row[colOptCode].ToString();
                    int nSorting;
                    Int32.TryParse(row[colSorting].ToString(), out nSorting);
                    GeneralResource oName = new GeneralResource(ModelEnum.ResourceType.STRING, 2052, row[colNameCN].ToString(), 1033, row[colNameUS].ToString());
                    byte nMode;
                    Byte.TryParse(row[colMode].ToString(), out nMode);
                    string sRemark = row[colRemark].ToString();

                    if (!String.IsNullOrEmpty(sOrgCode) && (sOrgCode != sLastOrgCode))
                    {
                        oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                        sLastOrgCode = sOrgCode;
                    }
                    if (!String.IsNullOrEmpty(sMainCode))
                    {
                        oOption = (from o in dbEntity.GeneralOptionals
                                   where o.OrgID == oOrgan.Gid && o.Code == sMainCode
                                   select o).FirstOrDefault();
                        if (oOption == null)
                        {
                            oOption = new GeneralOptional { Organization = oOrgan, Code = sMainCode };
                            dbEntity.GeneralOptionals.Add(oOption);
                        }
                        oOption.Otype = nType;
                        oOption.Sorting = nSorting;
                        if (oOption.Name == null)
                            oOption.Name = oName;
                        else
                            oOption.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                        oOption.InputMode = nMode;
                        oOption.Remark = sRemark;
                    }
                    else
                    {
                        GeneralOptItem oOptItem = (from i in dbEntity.GeneralOptItems
                                                   where i.Deleted == false && i.OptID == oOption.Gid && i.Code == sOptCode
                                                   select i).FirstOrDefault();
                        if (oOptItem == null)
                        {
                            oOptItem = new GeneralOptItem { Optional = oOption, Code = sOptCode };
                            dbEntity.GeneralOptItems.Add(oOptItem);
                        }
                        oOptItem.Sorting = nSorting;
                        if (oOptItem.Name == null)
                            oOptItem.Name = oName;
                        else
                            oOptItem.Name.SetResource(ModelEnum.ResourceType.STRING, oName);
                        oOptItem.Remark = sRemark;
                    }
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sMainCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入GeneralOptional成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入GeneralOptional错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入组织属性
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportOrgAttrib(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colMainCode = oExcel.ExcelTable.Columns["属性代码"];
                DataColumn colOptCode = oExcel.ExcelTable.Columns["结果代码"];
                DataColumn colOptValue = oExcel.ExcelTable.Columns["结果值"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                string sLastOrgCode = "";
                MemberOrganization oOrgan = null;
                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrgCode = row[colOrgan].ToString();
                    string sMainCode = row[colMainCode].ToString();
                    string sOptCode = row[colOptCode].ToString();
                    string sOptValue = row[colOptValue].ToString();
                    string sRemark = row[colRemark].ToString();

                    if (!String.IsNullOrEmpty(sOrgCode) && (sOrgCode != sLastOrgCode))
                    {
                        oOrgan = (from o in dbEntity.MemberOrganizations
                                  where o.Code == sOrgCode && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                  select o).FirstOrDefault();
                        sLastOrgCode = sOrgCode;
                    }
                    var oOptional = (from o in dbEntity.GeneralOptionals
                                     where o.Deleted == false
                                           && o.OrgID == oOrgan.Gid && o.Code == sMainCode
                                     select o).FirstOrDefault();
                    if (oOptional != null)
                    {
                        GeneralOptItem oOptItem = null;
                        if (oOptional.InputMode == (byte)ModelEnum.OptionalInputMode.COMBOBOX)
                        {
                            oOptItem = (from i in dbEntity.GeneralOptItems
                                        where i.Deleted == false && i.Code == sOptCode
                                              && i.OptID == oOptional.Gid
                                        select i).FirstOrDefault();
                        }
                        var oOrgAttrib = (from a in dbEntity.MemberOrgAttributes
                                          where a.OrgID == oOrgan.Gid && a.OptID == oOptional.Gid
                                          select a).FirstOrDefault();
                        if (oOrgAttrib == null)
                        {
                            oOrgAttrib = new MemberOrgAttribute { Organization = oOrgan, Optional = oOptional };
                            dbEntity.MemberOrgAttributes.Add(oOrgAttrib);
                        }
                        oOrgAttrib.OptionalResult = oOptItem;
                        oOrgAttrib.Matter = sOptValue;
                        oOrgAttrib.Remark = sRemark;
                        dbEntity.SaveChanges();
                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("{0} {1} {2}", this.ToString(), sMainCode, sRemark);
                    }
                }
                oEventBLL.WriteEvent(String.Format("导入MemberOrgAttribute成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入MemberOrgAttribute错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }

        /// <summary>
        /// 导入面单模板
        /// </summary>
        /// <param name="sExcelFile">Excel文件名</param>
        /// <param name="sSheetName">Sheet名</param>
        public void ImportShippingEnvelope(string sExcelFile, string sSheetName)
        {
            try
            {
                ExcelData oExcel = new ExcelData(sExcelFile, sSheetName);
                DataColumn colOrgan = oExcel.ExcelTable.Columns["组织"];
                DataColumn colShipper = oExcel.ExcelTable.Columns["承运商"];
                DataColumn colCode = oExcel.ExcelTable.Columns["代码"];
                DataColumn colStatus = oExcel.ExcelTable.Columns["状态"];
                DataColumn colDescription = oExcel.ExcelTable.Columns["描述"];
                DataColumn colMatterCN = oExcel.ExcelTable.Columns["中文内容"];
                DataColumn colMatterUS = oExcel.ExcelTable.Columns["英文内容"];
                DataColumn colRemark = oExcel.ExcelTable.Columns["备注"];

                foreach (DataRow row in oExcel.ExcelTable.Rows)
                {
                    string sOrganCode = row[colOrgan].ToString();
                    string sShipper = row[colShipper].ToString();
                    var oShipping = (from s in dbEntity.ShippingInformations
                                     where s.Parent.Code == sOrganCode && s.Code == sShipper
                                           && s.Otype == (byte)ModelEnum.OrganizationType.SHIPPER
                                     select s).FirstOrDefault();
                    string sCode = row[colCode].ToString();
                    byte nStatus;
                    Byte.TryParse(row[colStatus].ToString(), out nStatus);
                    string sDescription = row[colDescription].ToString();
                    GeneralLargeObject oTemplate = new GeneralLargeObject(2052, row[colMatterCN].ToString(), 1033, row[colMatterUS].ToString());
                    string sRemark = row[colRemark].ToString();

                    var oEnvelope = (from e in dbEntity.ShippingEnvelopes
                                     where e.Deleted == false
                                           && e.ShipID == oShipping.Gid && e.Code == sCode
                                     select e).FirstOrDefault();
                    if (oEnvelope == null)
                    {
                        oEnvelope = new ShippingEnvelope { Shipping = oShipping, Code = sCode };
                        dbEntity.ShippingEnvelopes.Add(oEnvelope);
                    }
                    oEnvelope.Estatus = nStatus;
                    oEnvelope.Matter = sDescription;
                    if (oEnvelope.Template == null)
                        oEnvelope.Template = oTemplate;
                    else
                        oEnvelope.Template.SetLargeObject(oTemplate);
                    oTemplate.Remark = sRemark;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), sCode, sRemark);
                }
                oEventBLL.WriteEvent(String.Format("导入ImportShippingEnvelope成功: {0} {1}", sExcelFile, sSheetName),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("导入ImportShippingEnvelope错误: {0} {1} {2}", sExcelFile, sSheetName, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.SYSTEM, this.ToString());
            }
        }



    }
}

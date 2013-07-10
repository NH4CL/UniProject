using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using LiveAzure.Utility;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;
using System.Diagnostics;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Order;

namespace LiveAzure.Tools.Tester
{
    public partial class ModelTest_NEW
    {
        /// <summary>
        /// ProductOnItem
        /// </summary>
        public ProductOnItem ProductOnItemTest()
        {
            GeneralResource oResource = new GeneralResource { };
            oLiveEntities.GeneralResources.Add(oResource);

            MemberOrganization oOrg = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(oOrg);

            ProductInformation oProd = new ProductInformation
            {
                Code = GetRandCode(),
                Organization = oOrg,
                Name = oResource
            };
            oLiveEntities.ProductInformations.Add(oProd);

            MemberChannel mChl = new MemberChannel
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberChannels.Add(mChl);

            MemberOrgChannel oOChl = new MemberOrgChannel
            {
                Organization = oOrg,
                Channel = mChl
            };
            oLiveEntities.MemberOrgChannels.Add(oOChl);

            ProductOnSale onSale = new ProductOnSale
            {
                Code = GetRandCode(),
                Product = oProd,
                Channel = mChl,
                Name = oResource
            };
            oLiveEntities.ProductOnSales.Add(onSale);

            ProductInfoItem oSku = new ProductInfoItem
            {
                Code = GetRandCode(),
                Organization = oOrg,
                Product = oProd
            };
            oLiveEntities.ProductInfoItems.Add(oSku);

            ProductOnItem oOnSku = new ProductOnItem
            {
                OnSale = onSale,
                SkuItem = oSku
            };
            oLiveEntities.ProductOnItems.Add(oOnSku);
            oLiveEntities.SaveChanges();
            return oOnSku;
        }
        /// <summary>
        /// ProductOnTemplate测试
        /// </summary>
        public void ProductOnTemplateTest()
        {

            MemberOrganization org = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(org);

            ProductOnTemplate onTemp = new ProductOnTemplate
            {
                Code = GetRandCode(),
                Organization = org
            };
            oLiveEntities.ProductOnTemplates.Add(onTemp);
            oLiveEntities.SaveChanges();
        }

        public void ProductOnShippingTest()
        {
            ProductOnSale oProd = ProductOnSaleTest();
            ProductOnItem oSku = ProductOnItemTest();
            ShippingInformation shipper = new ShippingInformation
            {
                Code = GetRandCode()
            };
            oLiveEntities.ShippingInformations.Add(shipper);
            ProductOnShipping ship = new ProductOnShipping
            {
                OnSale = oProd,
                Shipper = shipper
            };
            oLiveEntities.ProductOnShippings.Add(ship);
            oLiveEntities.SaveChanges();
        }

        public void ProductOnAdjustTest()
        {
            ProductOnSale oSale = ProductOnSaleTest();
            ProductOnItem oSku = ProductOnItemTest();

            MemberOrganization org = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(org);

            GeneralResource res = new GeneralResource
            {
            };
            oLiveEntities.GeneralResources.Add(res);

            ProductInformation prod = new ProductInformation
            {
                Name = res,
                Organization = org,
                Code = GetRandCode()
            };
            oLiveEntities.ProductInformations.Add(prod);

            ProductInfoItem infoItem = new ProductInfoItem
            {
                Organization = org,
                Product = prod,
                Code = GetRandCode()
            };
            oLiveEntities.ProductInfoItems.Add(infoItem);
            oLiveEntities.SaveChanges();

            ProductOnAdjust oAdjust = new ProductOnAdjust
            {
                OnSaleID = oSale.Gid,
                OnSkuID = oSku.Gid
            };
            oLiveEntities.ProductOnAdjusts.Add(oAdjust);
            oLiveEntities.SaveChanges();
        }

        public void OrderItemTest()
        {
            ProductOnItem oSku = ProductOnItemTest();
            GeneralResource res = new GeneralResource
            {
            };
            oLiveEntities.GeneralResources.Add(res);

            MemberOrganization org = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(org);

            MemberRole role = new MemberRole
            {
                Name = res,
                Code = GetRandCode(),
                Organization = org
            };
            oLiveEntities.MemberRoles.Add(role);

            MemberChannel mChl = new MemberChannel
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberChannels.Add(mChl);

            MemberUser user = new MemberUser
            {
                Organization = org,
                Channel = mChl,
                Role = role,
                LoginName = "550884152@qq.com",
                Passcode = "5425425425",
                SaltKey = "343134"
            };
            oLiveEntities.MemberUsers.Add(user);

            OrderInformation oInfo = new OrderInformation
            {
                Organization = org,
                Channel = mChl,
                Code = GetRandCode(),
                User = user
            };
            oLiveEntities.OrderInformations.Add(oInfo);
            oLiveEntities.SaveChanges();

            OrderItem oItem = new OrderItem
            {
                OnSkuID = oSku.Gid,
                Order = oInfo
            };
            oLiveEntities.OrderItems.Add(oItem);

            oLiveEntities.SaveChanges();
        }

        //=====================================================================

        public void InsertTestData()
        {
            //res 表中添加角色
            GeneralResource resRole = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "General User",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resRole);
            //res 表中添加组织全名
            GeneralResource resOrg = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "筑巢信息科技有限公司",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resOrg);
            //res 表中添加组织简称
            GeneralResource resShort = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "筑巢",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resShort);
            //res 表中添加数据
            GeneralResource resData = new GeneralResource
            {
                Code = GetRandCode(),
                Culture = 2052,
                Matter = "A Test"
            };
            oLiveEntities.GeneralResources.Add(resData);
            GeneralResource resProg0 = new GeneralResource
            {
                Code = GetRandCode(),
                Culture = 2052,
                Matter = "Config"
            };
            oLiveEntities.GeneralResources.Add(resProg0);
            //res 表中添加程序定义
            GeneralResource resProg1 = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "generalProperty",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg1);
            //res 表中添加程序定义
            GeneralResource resProg2 = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "Units",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg2);
            //res 表中添加程序定义
            GeneralResource resProg3 = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "Classification",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg3);
            //res 表中添加程序定义
            GeneralResource resProg4 = new GeneralResource
            {
                Code = GetRandCode(),
                Matter = "Organization",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg4);
            //unit表中添加数据
            GeneralMeasureUnit oUnit = new GeneralMeasureUnit
            {
                Utype = 2,
                Code = GetRandCode(),
                Name = resData
            };
            oLiveEntities.GeneralMeasureUnits.Add(oUnit);
            //unit表中添加数据
            GeneralMeasureUnit oUnit0 = new GeneralMeasureUnit
            {
                Utype = 6,
                Code = GetRandCode(),
                Name = resData
            };
            oLiveEntities.GeneralMeasureUnits.Add(oUnit0);
            //文化表中添加数据
            GeneralCultureUnit culture = new GeneralCultureUnit
            {
                Piece = oUnit,
                Weight = oUnit0,
                Culture = 2052
            };
            oLiveEntities.GeneralCultureUnits.Add(culture);
            //组织表中添加数据
            MemberOrganization org = new MemberOrganization
            {
                Code = GetRandCode(),
                FullName = resOrg,
                ShortName = resShort,
                WorkPhone = "15121040098",
                HomeUrl = "http://www.zhuchao.com"
            };
            oLiveEntities.MemberOrganizations.Add(org);
            MemberOrganization chl = new MemberOrganization
            {
                Code = GetRandCode(),
                FullName = resOrg,
                ShortName = resShort,
                WorkPhone = "15121040098",
                HomeUrl = "http://www.zhuchao.com"
            };
            oLiveEntities.MemberOrganizations.Add(chl);
            //角色表中添加数据
            MemberRole role = new MemberRole
            {
                Name = resRole,
                Code = GetRandCode(),
                Organization = org
            };
            oLiveEntities.MemberRoles.Add(role);

            //新建渠道
            MemberChannel mChl = new MemberChannel
            {
                Code = GetRandCode(),
                CellPhone = "4525254254",
                Email = "7494379@qq.com",
            };
            oLiveEntities.MemberChannels.Add(mChl);

            MemberOrgChannel oChl = new MemberOrgChannel
            {
                Organization = org,
                Channel = mChl
            };
            oLiveEntities.MemberOrgChannels.Add(oChl);
            //添加用户
            MemberUser user = new MemberUser
            {
                Organization = org,
                Channel = mChl,
                Role = role,
                LoginName = GetRandCode() + "@qq.com",
                Passcode = GetRandCode()
            };
            oLiveEntities.MemberUsers.Add(user);
            //添加用户
            MemberUser user0 = new MemberUser
            {
                Organization = org,
                Channel = mChl,
                Role = role,
                LoginName = "test@163.com",
                Passcode = "123"
            };
            oLiveEntities.MemberUsers.Add(user0);
            //添加程序定义
            GeneralProgram prog = new GeneralProgram
            {
                Name = resProg0,
                Code = GetRandCode()
            };
            oLiveEntities.GeneralPrograms.Add(prog);
            //添加程序定义
            GeneralProgram prog0 = new GeneralProgram
            {
                Name = resProg1,
                Code = GetRandCode()
            };
            oLiveEntities.GeneralPrograms.Add(prog0);
            //添加程序定义
            GeneralProgram prog1 = new GeneralProgram
            {
                Name = resProg2,
                Code = GetRandCode(),
                Parent = prog
            };
            oLiveEntities.GeneralPrograms.Add(prog1);
            //添加程序定义
            GeneralProgram prog2 = new GeneralProgram
            {
                Name = resProg3,
                Code = GetRandCode(),
                Parent = prog
            };
            oLiveEntities.GeneralPrograms.Add(prog2);
            //添加程序定义
            GeneralProgram prog3 = new GeneralProgram
            {
                Name = resProg4,
                Code = GetRandCode(),
                Parent = prog
            };
            oLiveEntities.GeneralPrograms.Add(prog3);
            //添加程序定义
            GeneralProgram prog5 = new GeneralProgram
            {
                Name = resProg4,
                Code = GetRandCode(),
                Parent = prog1
            };
            oLiveEntities.GeneralPrograms.Add(prog5);
            //res 表中添加程序定义
            GeneralResource resnode = new GeneralResource
            {
                Code = "resnode",
                Matter = "功能节点1",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg1);

            GeneralResource resoptional = new GeneralResource
            {
                Code = "resnodeoption",
                Matter = "0|不显示，1|显示",
                Culture = 2052
            };
            oLiveEntities.GeneralResources.Add(resProg1);

            //添加程序功能定义
            GeneralProgNode progNode = new GeneralProgNode
            {
                Program = prog,
                Code = "prognode",
                Name = resnode,
                Optional = resoptional,
                InputMode = 1
            };
            oLiveEntities.GeneralProgNodes.Add(progNode);
            oLiveEntities.SaveChanges();
            //添加权限控制
            MemberPrivilege prol = new MemberPrivilege
            {
                User = user0,
                Ptype = 0
            };
            oLiveEntities.MemberPrivileges.Add(prol);
            //添加权限控制
            MemberPrivilege prol0 = new MemberPrivilege
            {
                User = user0,
                Ptype = 1
            };
            oLiveEntities.MemberPrivileges.Add(prol0);

            MemberPrivItem prinode = new MemberPrivItem
            {
                Privilege = prol0,
                RefID = progNode.Gid,
                NodeValue = "1"
            };
            oLiveEntities.MemberPrivItems.Add(prinode);
            //添加权限控制明细
            MemberPrivItem prolItem = new MemberPrivItem
            {
                Privilege = prol,
                RefID = prog.Gid
            };
            oLiveEntities.MemberPrivItems.Add(prolItem);
            //添加权限控制明细
            MemberPrivItem prolItem0 = new MemberPrivItem
            {
                Privilege = prol,
                RefID = prog0.Gid

            };
            oLiveEntities.MemberPrivItems.Add(prolItem0);
            //添加权限控制明细
            MemberPrivItem prolItem1 = new MemberPrivItem
            {
                Privilege = prol,
                RefID = prog1.Gid

            };
            oLiveEntities.MemberPrivItems.Add(prolItem1);
            //添加权限控制明细
            MemberPrivItem prolItem2 = new MemberPrivItem
            {
                Privilege = prol,
                RefID = prog2.Gid

            };
            oLiveEntities.MemberPrivItems.Add(prolItem2);

            oLiveEntities.SaveChanges();
        }
    }
}

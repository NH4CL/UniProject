using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using LiveAzure.Models;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models.Finance;
using LiveAzure.Models.Order;

namespace LiveAzure.Tools.Tester
{
    /// <summary>
    /// 刘鑫
    /// </summary>
    public partial class ModelTest_NEW
    {
        /// <summary>
        /// 测试ProductOnSale
        /// </summary>
        /// <returns>新建的ProductOnSale对象</returns>
        public ProductOnSale ProductOnSaleTest()
        {
            MemberOrganization oOrg = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(oOrg);
            GeneralResource oName = new GeneralResource { };
            oLiveEntities.GeneralResources.Add(oName);
            ProductInformation oProduct = new ProductInformation
            {
                Organization = oOrg,
                Code = GetRandCode(),
                Name = oName
            };
            oLiveEntities.ProductInformations.Add(oProduct);
            MemberChannel oChannel = new MemberChannel
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberChannels.Add(oChannel);
            GeneralResource oName2 = new GeneralResource { };
            oLiveEntities.GeneralResources.Add(oName2);
            ProductOnSale oProductOnSale = new ProductOnSale
            {
                Product = oProduct,
                Channel = oChannel,
                Name = oName2,
                Code = GetRandCode()
            };
            oLiveEntities.ProductOnSales.Add(oProductOnSale);
            oLiveEntities.SaveChanges();
            return oProductOnSale;
        }
        /// <summary>
        /// 测试ProductOnUnitPrice
        /// </summary>
        /// <returns>新建的ProductOnUnitPrice对象</returns>
        public ProductOnUnitPrice ProductOnUnitPriceTest()
        {
            MemberOrganization oOrg = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(oOrg);
            oLiveEntities.SaveChanges();
            GeneralResource oName = new GeneralResource { };
            oLiveEntities.GeneralResources.Add(oName);
            oLiveEntities.SaveChanges();
            ProductInformation oProduct = new ProductInformation
            {
                Organization = oOrg,
                Code = GetRandCode(),
                Name = oName
            };
            oLiveEntities.ProductInformations.Add(oProduct);
            oLiveEntities.SaveChanges();
            ProductInfoItem oProductInfoItem = new ProductInfoItem
            {
                Organization = oOrg,
                Product = oProduct,
                Code = GetRandCode()
            };
            oLiveEntities.ProductInfoItems.Add(oProductInfoItem);
            oLiveEntities.SaveChanges();
            ProductOnItem oPOI = new ProductOnItem
            {
                OnSale = ProductOnSaleTest(),
                SkuItem = oProductInfoItem
            };
            oLiveEntities.ProductOnItems.Add(oPOI);
            oLiveEntities.SaveChanges();
            GeneralMeasureUnit oGMU = new GeneralMeasureUnit
            {
                Code = GetRandCode()
            };
            oLiveEntities.GeneralMeasureUnits.Add(oGMU);
            oLiveEntities.SaveChanges();
            ProductOnUnitPrice oPUP = new ProductOnUnitPrice
            {
                OnSkuItem = oPOI,
                ShowUnit = oGMU
            };
            oLiveEntities.ProductOnUnitPrices.Add(oPUP);
            oLiveEntities.SaveChanges();
            return oPUP;
        }
        /// <summary>
        /// 测试ProductOnPayment
        /// </summary>
        /// <returns>新建的ProductOnPayment对象</returns>
        public ProductOnPayment ProductOnPaymentTest()
        {
            MemberOrganization oOrg = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(oOrg);

            FinancePayType oFinancePayType = new FinancePayType
            {
                Organization = oOrg,
                Code = GetRandCode()
            };
            oLiveEntities.FinancePayTypes.Add(oFinancePayType);
            ProductOnPayment oProductOnPayment = new ProductOnPayment
            {
                OnSale = ProductOnSaleTest(),
                PayType = oFinancePayType
            };
            oLiveEntities.ProductOnPayments.Add(oProductOnPayment);
            oLiveEntities.SaveChanges();
            return oProductOnPayment;
        }
        /// <summary>
        /// 测试ProductOnRelation
        /// </summary>
        /// <returns>新建的ProductOnRelation对象</returns>
        public ProductOnRelation ProductOnRelationTest()
        {
            ProductOnSale oOnSale = ProductOnSaleTest();
            ProductOnSale oOnRelation = ProductOnSaleTest();
            ProductOnRelation oProductOnRelation = new ProductOnRelation
            {
                OnSale = oOnSale,
                OnRelation = oOnRelation
            };
            oLiveEntities.ProductOnRelations.Add(oProductOnRelation);
            oLiveEntities.SaveChanges();
            return oProductOnRelation;
        }
        /// <summary>
        /// 测试OrderAttribute
        /// </summary>
        /// <returns>新建的OrderAttribute对象</returns>
        public OrderAttribute OrderAttributeTest()
        {
            MemberOrganization oOrg = new MemberOrganization
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberOrganizations.Add(oOrg);
            MemberChannel oChannel = new MemberChannel
            {
                Code = GetRandCode()
            };
            oLiveEntities.MemberChannels.Add(oChannel);
            GeneralResource oGeneralResource = new GeneralResource { };
            MemberRole oRole = new MemberRole
            {
                Organization = oOrg,
                Code = GetRandCode(),
                Name = oGeneralResource
            };
            oLiveEntities.MemberRoles.Add(oRole);
            MemberUser oUser = new MemberUser
            {
                Organization = oOrg,
                Role = oRole,
                Channel = oChannel,
                LoginName = GetRandCode(),
                Passcode = GetRandCode(),
                SaltKey = "88888888"
            };
            oLiveEntities.MemberUsers.Add(oUser);
            OrderInformation oOrderInformation = new OrderInformation
            {
                Organization = oOrg,
                Channel = oChannel,
                User = oUser,
                Code = GetRandCode()
            };
            oLiveEntities.OrderInformations.Add(oOrderInformation);
            GeneralOptional oOptional = new GeneralOptional
            {
                Organization = oOrg,
                Code = GetRandCode()
            };
            oLiveEntities.GeneralOptionals.Add(oOptional);
            OrderAttribute oAttribute = new OrderAttribute
            {
                Order = oOrderInformation,
                Optional = oOptional
            };
            oLiveEntities.OrderAttributes.Add(oAttribute);
            oLiveEntities.SaveChanges();
            return oAttribute;
        }
    }
}

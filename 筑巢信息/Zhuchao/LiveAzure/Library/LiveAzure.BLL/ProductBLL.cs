using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Product;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.General;
using LiveAzure.Models.Finance;

namespace LiveAzure.BLL
{
    public class ProductBLL : BaseBLL
    {
        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public ProductBLL(LiveEntities entity) : base(entity) { }

        public string ProductTemplateOnSale(MemberOrganization oOrgan, MemberChannel oChannel, ProductOnTemplate oTemplate,
            ProductInformation oProduct, List<Object> oItemList = null) 
        {
            //判断是否存在SKU列表，如果不存在SKU列表，则只添加产品上架相关的信息；
            //否则既添加产品上架信息，还要添加SKU上架表的信息。
            ProductOnSale oProductOnsale;
            Guid currentOnsaleGid;
            Guid productGid = oProduct.Gid;
            Guid onSaleChID = oChannel.Gid;
            Guid onSaleOrgID = oOrgan.Gid;
            //ProductInformation oProduct = dbEntity.ProductInformations.Include("SkuItems").Where(p => p.Gid == productGid && p.Deleted == false).FirstOrDefault();
            String productOnsaleCode = oProduct.Code;

            ProductOnSale productOnsale = dbEntity.ProductOnSales.Where(p => p.ProdID == productGid && p.ChlID == onSaleChID && p.Code == productOnsaleCode).FirstOrDefault();
            //为新上架商品
            if (productOnsale == null)
            {
                oProductOnsale = new ProductOnSale();
                #region 模板上架ProductOnsale表信息添加

                oProductOnsale.ProdID = productGid;
                oProductOnsale.OrgID = onSaleChID;
                oProductOnsale.ChlID = onSaleChID;
                oProductOnsale.Code = productOnsaleCode;
                oProductOnsale.Ostatus = 1;
                oProductOnsale.aName = (Guid)oProduct.aName;
                oProductOnsale.Mode = oProduct.Mode;
                //=====市场价以及销售价======

                //=========================
                //oProductOnsale.CanSplit = oProduct.Block;
                oProductOnsale.aBrief = oProduct.aBrief;
                oProductOnsale.aMatter = oProduct.aMatter;

                dbEntity.ProductOnSales.Add(oProductOnsale);
                dbEntity.SaveChanges();
                //当前上架商品的OnsaleID
                currentOnsaleGid = oProductOnsale.Gid;

                #endregion

                #region 模板上架ProductOnItems表信息添加
                Guid marketPriceGid;
                Guid salePriceGid;
                //保存SKU的信息，如果存在SKU
                if (oItemList != null)
                {
                    foreach (Dictionary<string, object> item in oItemList)
                    {
                        //保存进入ProductOnItem表
                        ProductInfoItem oInfoItem = (ProductInfoItem)item["SkuItem"];
                        ProductOnItem oNewProductItem = new ProductOnItem();
                        oNewProductItem.OnSaleID = currentOnsaleGid;
                        oNewProductItem.SkuID = oInfoItem.Gid;
                        oNewProductItem.aFullName = oInfoItem.aFullName;
                        oNewProductItem.aShortName = oInfoItem.aShortName;
                        dbEntity.ProductOnItems.Add(oNewProductItem);
                        dbEntity.SaveChanges();
                        //获取当前保存的OnSKUID
                        Guid currentOnSKUGid = oNewProductItem.Gid;
                        GeneralMeasureUnit oUnit = (GeneralMeasureUnit)item["Unit"];
                        Guid currentUnitGid = oUnit.Gid;
                        
                        GeneralMeasureUnit oCurrency1 = (GeneralMeasureUnit)item["Currency1"];
                        //第一个价格不存在则认为添加失败
                        if (oCurrency1 != null)
                        {
                            GeneralResource oMarketCash = new GeneralResource();
                            oMarketCash.Rtype = (byte)ModelEnum.ResourceType.MONEY;
                            oMarketCash.Currency = oCurrency1.Gid;
                            oMarketCash.Cash = (decimal)item["MarketPrice1"];
                            GeneralResource oSaleCash = new GeneralResource();
                            oSaleCash.Rtype = (byte)ModelEnum.ResourceType.MONEY;
                            oSaleCash.Currency = oCurrency1.Gid;
                            oSaleCash.Cash = (decimal)item["SalePrice1"];
                            dbEntity.GeneralResources.Add(oMarketCash);
                            dbEntity.GeneralResources.Add(oSaleCash);
                            dbEntity.SaveChanges();
                            marketPriceGid = oMarketCash.Gid;
                            salePriceGid = oSaleCash.Gid;
                            GeneralMeasureUnit oCurrency2 = (GeneralMeasureUnit)item["Currency2"];
                            if (oCurrency2 != null) 
                            {
                                GeneralResItem oMarketResItem1 = new GeneralResItem();
                                oMarketResItem1.ResID = marketPriceGid;
                                oMarketResItem1.Currency = oCurrency2.Gid;
                                oMarketResItem1.Cash = (decimal)item["MarketPrice2"];
                                GeneralResItem oSaleResItem1 = new GeneralResItem();
                                oSaleResItem1.ResID = salePriceGid;
                                oSaleResItem1.Currency = oCurrency2.Gid;
                                oSaleResItem1.Cash = (decimal)item["SalePrice2"];
                                dbEntity.GeneralResItems.Add(oMarketResItem1);
                                dbEntity.GeneralResItems.Add(oSaleResItem1);
                                dbEntity.SaveChanges();
                            }
                            GeneralMeasureUnit oCurrency3 = (GeneralMeasureUnit)item["Currency3"];
                            if (oCurrency3 != null)
                            {
                                GeneralResItem oMarketResItem2 = new GeneralResItem();
                                oMarketResItem2.ResID = marketPriceGid;
                                oMarketResItem2.Currency = oCurrency3.Gid;
                                oMarketResItem2.Cash = (decimal)item["MarketPrice3"];
                                GeneralResItem oSaleResItem2 = new GeneralResItem();
                                oSaleResItem2.ResID = salePriceGid;
                                oSaleResItem2.Currency = oCurrency3.Gid;
                                oSaleResItem2.Cash = (decimal)item["SalePrice3"];
                                dbEntity.GeneralResItems.Add(oMarketResItem2);
                                dbEntity.GeneralResItems.Add(oSaleResItem2);
                                dbEntity.SaveChanges();
                            }
                            GeneralMeasureUnit oCurrency4 = (GeneralMeasureUnit)item["Currency4"];
                            if (oCurrency3 != null)
                            {
                                GeneralResItem oMarketResItem3 = new GeneralResItem();
                                oMarketResItem3.ResID = marketPriceGid;
                                oMarketResItem3.Currency = oCurrency3.Gid;
                                oMarketResItem3.Cash = (decimal)item["MarketPrice4"];
                                GeneralResItem oSaleResItem3 = new GeneralResItem();
                                oSaleResItem3.ResID = salePriceGid;
                                oSaleResItem3.Currency = oCurrency3.Gid;
                                oSaleResItem3.Cash = (decimal)item["SalePrice4"];
                                dbEntity.GeneralResItems.Add(oMarketResItem3);
                                dbEntity.GeneralResItems.Add(oSaleResItem3);
                                dbEntity.SaveChanges();
                            }
                            //添加到UnitPrice表里
                            ProductOnUnitPrice oProductOnUnitPrice = new ProductOnUnitPrice();
                            oProductOnUnitPrice.OnSkuID = currentOnSKUGid;
                            oProductOnUnitPrice.aShowUnit = currentUnitGid;
                            oProductOnUnitPrice.aMarketPrice = marketPriceGid;
                            oProductOnUnitPrice.aSalePrice = salePriceGid;
                            oProductOnUnitPrice.UnitRatio = (decimal)item["Ratio"];
                            oProductOnUnitPrice.Percision = (byte)item["Percision"];
                            dbEntity.ProductOnUnitPrices.Add(oProductOnUnitPrice);
                            dbEntity.SaveChanges();
                        }                        

                    }
                }
                else
                {
                    for (int i = 0; i < oProduct.SkuItems.Count; i++)
                    {
                        ProductOnItem oNewProductItem = new ProductOnItem();
                        oNewProductItem.OnSaleID = currentOnsaleGid;
                        oNewProductItem.SkuID = oProduct.SkuItems.ElementAt(i).Gid;
                        oNewProductItem.aFullName = oProduct.SkuItems.ElementAt(i).aFullName;
                        oNewProductItem.aShortName = oProduct.SkuItems.ElementAt(i).aShortName;
                        dbEntity.ProductOnItems.Add(oNewProductItem);
                        dbEntity.SaveChanges();
                        //加入价套信息
                        Guid stdUnitGid = oProduct.SkuItems.ElementAt(i).StdUnit;
                        ProductOnUnitPrice oNewUnitPrice = new ProductOnUnitPrice();
                        oNewUnitPrice.OnSkuID = oNewProductItem.Gid;
                        oNewUnitPrice.aShowUnit = stdUnitGid;
                        GeneralResource oMarketCash = new GeneralResource();
                        oMarketCash.Rtype = (byte)ModelEnum.ResourceType.MONEY;
                        oMarketCash.Currency = oProduct.SkuItems.ElementAt(i).MarketPrice.Currency;
                        oMarketCash.Cash = oProduct.SkuItems.ElementAt(i).MarketPrice.Cash;
                        GeneralResource oSaleCash = new GeneralResource();
                        oSaleCash.Rtype = (byte)ModelEnum.ResourceType.MONEY;
                        oSaleCash.Currency = oProduct.SkuItems.ElementAt(i).SuggestPrice.Currency;
                        oSaleCash.Cash = oProduct.SkuItems.ElementAt(i).SuggestPrice.Cash;
                        dbEntity.GeneralResources.Add(oMarketCash);
                        dbEntity.GeneralResources.Add(oSaleCash);
                        dbEntity.SaveChanges();
                        marketPriceGid = oMarketCash.Gid;
                        salePriceGid = oSaleCash.Gid;
                        oNewUnitPrice.aMarketPrice = marketPriceGid;
                        oNewUnitPrice.aSalePrice = salePriceGid;
                        Guid oldMarketGid = oProduct.SkuItems.ElementAt(i).MarketPrice.Gid;
                        List<GeneralResItem> listMarketPrice = dbEntity.GeneralResItems.Where(p => p.ResID == oldMarketGid && p.Deleted == false).ToList();
                        for (int j = 0; j < listMarketPrice.Count; j++)
                        {
                            GeneralResItem newMarketPrice = new GeneralResItem();
                            newMarketPrice.ResID = marketPriceGid;
                            newMarketPrice.Currency = listMarketPrice.ElementAt(j).Currency;
                            newMarketPrice.Cash = listMarketPrice.ElementAt(j).Cash;
                            dbEntity.GeneralResItems.Add(newMarketPrice);
                            dbEntity.SaveChanges();
                        }
                        Guid oldSaleGid = oProduct.SkuItems.ElementAt(i).SuggestPrice.Gid;
                        List<GeneralResItem> listSalePrice = dbEntity.GeneralResItems.Where(p => p.ResID == oldSaleGid && p.Deleted == false).ToList();
                        for (int j = 0; j < listSalePrice.Count; j++)
                        {
                            GeneralResItem newSalePrice = new GeneralResItem();
                            newSalePrice.ResID = salePriceGid;
                            newSalePrice.Currency = listSalePrice.ElementAt(j).Currency;
                            newSalePrice.Cash = listSalePrice.ElementAt(j).Cash;
                            dbEntity.GeneralResItems.Add(newSalePrice);
                            dbEntity.SaveChanges();
                        }
                        dbEntity.ProductOnUnitPrices.Add(oNewUnitPrice);
                        dbEntity.SaveChanges();
                    }
                }

                #endregion

                #region 根据数据库中取出的模板对上架商品进行赋值

                Guid onSaleTemplateGid = oTemplate.Gid;
                ProductOnTemplate oCurrentTemplate = dbEntity.ProductOnTemplates.Where(p => p.Gid == onSaleTemplateGid && p.Deleted == false).FirstOrDefault();
                if (oCurrentTemplate != null)
                {
                    string strShipPolicy = oCurrentTemplate.ShipPolicy;
                    string strPayPolicy = oCurrentTemplate.PayPolicy;
                    string strRelation = oCurrentTemplate.Relation;
                    string strLevelDiscount = oCurrentTemplate.LevelDiscount;

                    //将承运商信息和地区信息存入数据库
                    Guid onSaleShippingGid = new Guid();
                    string[] shippingInfo = strShipPolicy.Split(';');
                    for (int i = 0; i < shippingInfo.Count(); i++)
                    {
                        string[] currentShipAndAreaInfo = shippingInfo[i].Split(':');
                        //承运商代码
                        string currentShipCode = currentShipAndAreaInfo[0].Split('|')[0];
                        //承运商权重
                        string currentShipWeight = currentShipAndAreaInfo[0].Split('|')[1];
                        //承运商计费方案
                        string currentShipSolution = currentShipAndAreaInfo[0].Split('|')[2];
                        //承运商地区信息
                        string[] currentShipAreaList = currentShipAndAreaInfo[1].Split(',');

                        //保存承运商信息
                        //如果承运商信息不为空，保存信息；否则不做操作。
                        if (currentShipAndAreaInfo[0] != "")
                        {
                            ProductOnShipping oNewProductOnShipping = new ProductOnShipping();
                            oNewProductOnShipping.OnSaleID = currentOnsaleGid;
                            byte bOrgType = (byte)ModelEnum.OrganizationType.SHIPPER;
                            //判断承运商信息是否在数据库中已删除
                            ShippingInformation oCurrentShippingInfomation = dbEntity.ShippingInformations.Where(p => p.Code == currentShipCode && p.aParent == onSaleOrgID && p.Otype == bOrgType && p.Deleted == false).FirstOrDefault();
                            if (oCurrentShippingInfomation != null)
                            {
                                Guid currentShippingID = oCurrentShippingInfomation.Gid;
                                oNewProductOnShipping.ShipID = currentShippingID;
                                oNewProductOnShipping.ShipWeight = Int32.Parse(currentShipWeight);
                                oNewProductOnShipping.Solution = byte.Parse(currentShipSolution);
                                dbEntity.ProductOnShippings.Add(oNewProductOnShipping);
                                dbEntity.SaveChanges();
                                onSaleShippingGid = oNewProductOnShipping.Gid;
                                //将对应的承运商的区域存入ProductOnShipArea表
                                //如果承运商地区信息不为空，保存信息；否则不做操作。
                                if (currentShipAreaList[0] != "")
                                {
                                    for (int j = 0; j < currentShipAreaList.Count(); j++)
                                    {
                                        Guid currentRegionGid = Guid.Parse(currentShipAreaList[j]);
                                        //判断数据库中地区是否存在
                                        GeneralRegion oRegion = dbEntity.GeneralRegions.Where(p => p.Gid == currentRegionGid && p.Deleted == false).FirstOrDefault();
                                        if (oRegion != null)
                                        {
                                            ProductOnShipArea oNewShipArea = new ProductOnShipArea();
                                            oNewShipArea.RegionID = currentRegionGid;
                                            oNewShipArea.OnShip = onSaleShippingGid;
                                            dbEntity.ProductOnShipAreas.Add(oNewShipArea);
                                            dbEntity.SaveChanges();
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //将相关的支付方式存入数据库
                    string[] paymentList = strPayPolicy.Split(';');
                    //如果有支付方式，则将支付方式写入数据库；否则不保存。
                    if (paymentList[0] != "")
                    {
                        for (int i = 0; i < paymentList.Count(); i++)
                        {
                            string strPaymentCode = paymentList[i];
                            //判断支付方式在数据库中是否存在
                            FinancePayType oPaytype = dbEntity.FinancePayTypes.Where(p => p.OrgID == onSaleOrgID && p.Code == strPaymentCode && p.Deleted == false).FirstOrDefault();
                            if (oPaytype != null)
                            {
                                Guid currentPaymentGid = oPaytype.Gid;
                                ProductOnPayment oProductOnPayment = new ProductOnPayment();
                                oProductOnPayment.OnSaleID = currentOnsaleGid;
                                oProductOnPayment.PayID = currentPaymentGid;
                                dbEntity.ProductOnPayments.Add(oProductOnPayment);
                                dbEntity.SaveChanges();
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    //将相关的商品信息存入数据库
                    string[] relationList = strRelation.Split(';');
                    //判断是否存在关联商品信息
                    if (relationList[0] != "")
                    {
                        for (int i = 0; i < relationList.Count(); i++)
                        {
                            Guid relationProductGid = Guid.Parse(relationList[i].Split('|')[0]);
                            byte relationType = byte.Parse(relationList[i].Split('|')[1]);
                            ProductOnSale oRelationProductOnSale = dbEntity.ProductOnSales.Where(p => p.Gid == relationProductGid && p.Deleted == false).FirstOrDefault();
                            if (oRelationProductOnSale != null)
                            {
                                ProductOnRelation oProductOnRelation = new ProductOnRelation();
                                oProductOnRelation.OnSaleID = currentOnsaleGid;
                                oProductOnRelation.aOnRelation = relationProductGid;
                                oProductOnRelation.Rtype = relationType;
                                dbEntity.ProductOnRelations.Add(oProductOnRelation);
                                dbEntity.SaveChanges();
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    //将相关的会员打折信息存入数据库
                    string[] levelDiscountList = strLevelDiscount.Split(';');
                    //判断是否存在会员打折信息
                    if (levelDiscountList[0] != "")
                    {
                        for (int i = 0; i < levelDiscountList.Count(); i++)
                        {
                            string strMemberCode = levelDiscountList[i].Split(':')[0];
                            decimal decDiscount = Decimal.Parse(levelDiscountList[i].Split(':')[1]);
                            MemberLevel oMemberLevel = dbEntity.MemberLevels.Where(p => p.Code == strMemberCode && p.Deleted == false).FirstOrDefault();
                            if (oMemberLevel != null)
                            {
                                Guid memberLevelGid = oMemberLevel.Gid;
                                ProductOnLevelDiscount oProductOnLevelDiscount = new ProductOnLevelDiscount();
                                oProductOnLevelDiscount.OnSaleID = currentOnsaleGid;
                                oProductOnLevelDiscount.aUserLevel = memberLevelGid;
                                oProductOnLevelDiscount.Discount = decDiscount;
                                dbEntity.ProductOnLevelDiscounts.Add(oProductOnLevelDiscount);
                                dbEntity.SaveChanges();
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                }

                #endregion

            }
            else
            {
                //存在上架的商品，同时还是可用状态，则提示用户不能重复上架
                if (productOnsale.Deleted == false)
                {
                    return "fail";
                }
                else
                {
                    //==================ToDo===================

                    //#region 恢复原来的上架信息

                    ////上架商品表信息恢复
                    //productOnsale.Deleted = false;
                    //currentOnsaleGid = productOnsale.Gid;

                    ////上架SKU信息恢复
                    //List<ProductOnItem> listProductOnItem = dbEntity.ProductOnItems.Where(p => p.OnSaleID == currentOnsaleGid).ToList();
                    //foreach (ProductOnItem productOnItem in listProductOnItem)
                    //{
                    //    productOnItem.Deleted = false;
                    //}

                    ////上架承运商信息恢复
                    //List<ProductOnShipping> listProductOnShipping = dbEntity.ProductOnShippings.Where(p => p.OnSaleID == currentOnsaleGid).ToList();
                    //foreach (ProductOnShipping productOnShipping in listProductOnShipping)
                    //{
                    //    productOnShipping.Deleted = false;
                    //    Guid deleteShipGid = productOnShipping.Gid;
                    //    //上架承运商地区信息恢复
                    //    List<ProductOnShipArea> listProductOnShipArea = dbEntity.ProductOnShipAreas.Where(p => p.OnShip == deleteShipGid).ToList();
                    //    foreach (ProductOnShipArea productShipArea in listProductOnShipArea)
                    //    {
                    //        productShipArea.Deleted = false;
                    //    }
                    //}

                    ////上架支付方式信息恢复
                    //List<ProductOnPayment> listProductOnPayment = dbEntity.ProductOnPayments.Where(p => p.OnSaleID == currentOnsaleGid).ToList();
                    //foreach (ProductOnPayment productOnPayment in listProductOnPayment)
                    //{
                    //    productOnPayment.Deleted = false;
                    //}

                    ////上架关联商品信息恢复
                    //List<ProductOnRelation> listProductOnRelation = dbEntity.ProductOnRelations.Where(p => p.OnSaleID == currentOnsaleGid).ToList();
                    //foreach (ProductOnRelation productOnRelation in listProductOnRelation)
                    //{
                    //    productOnRelation.Deleted = false;
                    //}

                    ////上架会员打折信息恢复
                    //List<ProductOnLevelDiscount> listProductOnLevelDiscount = dbEntity.ProductOnLevelDiscounts.Where(p => p.OnSaleID == currentOnsaleGid).ToList();
                    //foreach (ProductOnLevelDiscount productOnLevelDiscount in listProductOnLevelDiscount)
                    //{
                    //    productOnLevelDiscount.Deleted = false;
                    //}

                    //#endregion
                }
            }
            
            
            return "success";
        }

    }
}

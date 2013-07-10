using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Mall;
using LiveAzure.Models.Member;
using LiveAzure.Portal.Models;
using LiveAzure.Models.Product;
using LiveAzure.Models.General;
using LiveAzure.Models.Finance;
using LiveAzure.BLL;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Order;

namespace LiveAzure.Portal.Controllers
{
    public class OrderController : BaseController
    {
        //
        // GET: /Order/

        #region 购物车首页，需要列出所有购买的产品，根据组织分别结算

        public OrderBLL orderBll;
        //全局组织Gid
        private static Guid globalOrgGid = new Guid();
        //货币单位，人民币
        private static Guid globalCurrencyGid = new Guid();
        //用以保存订单信息的全局变量，进入购物车页面时用以保存费用信息
        private static OrderInformation oNewOrder = new OrderInformation();
        //最佳承运商的Guid
        private static Guid globalShipperGid = new Guid();
        //全局商品总价格
        private static decimal globalProductExacuteAmount = 0m;
        //用户使用券信息的列表, 券的Gid以及券剩余的金额
        private static Dictionary<Guid, decimal> globalCouponList = new Dictionary<Guid, decimal>();
        //全局订单商品信息列表
        private static Dictionary<Guid, OrderItem> globalOrderItemList = new Dictionary<Guid, OrderItem>();
        //全局订单商品总数
        private static decimal globalOrderItemPieces = 0m; 
        //是否开发票
        private static bool bInvoiceOrNot = false;
        //是否选择快递或物流，还是自提
        private static bool bUseShipper = false;
        
        /// <summary>
        /// 实例化OrderBLL对象
        /// </summary>
        public OrderController()
        {
            orderBll = new OrderBLL(dbEntity);
        }
        
        /// <summary>
        /// 进入页面时需要获取用户的Gid以及当前渠道的Gid
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            globalCurrencyGid = LiveSession.currencyID;
            var listMallCart = dbEntity.MallCarts.Include("OnSkuItem").Where(p=>p.ChlID == LiveSession.channelID && p.UserID == LiveSession.userID && p.Deleted == false).GroupBy(p => p.OrgID).ToList();
            string strOrgList = "";
            for (int i = 0; i < listMallCart.Count(); i++)
            {
                IGrouping<Guid, MallCart> listGroupMallCart = (IGrouping<Guid, MallCart>)listMallCart.ElementAt(i);
                Guid oOrganizaitonGid = (Guid)listGroupMallCart.Key;
                if (i == listMallCart.Count() - 1)
                {
                    strOrgList = strOrgList + oOrganizaitonGid.ToString();
                }
                else
                {
                    strOrgList = strOrgList + oOrganizaitonGid.ToString() + ",";
                }
            }
            ViewBag.strOrgList = strOrgList;
            return View();
        }
        /// <summary>
        /// 获取不同组织的商品信息
        /// </summary>
        /// <param name="currentOrgGid"></param>
        /// <returns></returns>
        public ActionResult MallCartTablePartial(Guid? currentOrgGid)
        {
            MallCartInfo oMallCartInfo = new MallCartInfo();
            if (currentOrgGid != null)
            {
                List<MallCart> listGroupMallCart = dbEntity.MallCarts.Include("OnSkuItem").Include("OnSale").Include("Organization").Where(p => p.OrgID == currentOrgGid && p.ChlID == LiveSession.channelID && p.UserID == LiveSession.userID && p.Deleted == false).ToList();
                Guid oOrganizationGuid = (Guid)currentOrgGid;
                
                //拼接页面model需要的信息
                MemberOrganization oMember = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Gid == oOrganizationGuid && p.Deleted == false).FirstOrDefault();
                oMallCartInfo.organizationName = oMember.FullName.GetResource(LiveSession.Culture);
                oMallCartInfo.orgGid = oOrganizationGuid;
                //获取产品总数，数量为总数，不是商品类型的数量
                decimal nProductCount = 0m;
                //获取金额，以及设置页面商品List
                decimal allSalePrice = 0;
                decimal allMarketPrice = 0;
                List<MallCartProduct> listMallProduct = new List<MallCartProduct>();
                for (int j = 0; j < listGroupMallCart.Count; j++)
                {
                    Guid onSkuGid = listGroupMallCart.ElementAt(j).OnSkuID;
                    ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem.OnSale").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == onSkuGid && p.Deleted == false).OrderByDescending(p => p.IsDefault).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                    MallCartProduct oMallCartProduct = new MallCartProduct();
                    oMallCartProduct.mallCartGid = listGroupMallCart.ElementAt(j).Gid;
                    //转换前的数量
                    oMallCartProduct.productCount = listGroupMallCart.ElementAt(j).SetQty;
                    //折扣，现在默认为1，不打折
                    oMallCartProduct.productDiscount = 1;
                    //套装模式
                    oMallCartProduct.productMode = (byte)listGroupMallCart.ElementAt(j).OnSale.Mode;
                    //套装数量
                    oMallCartProduct.productSetCount = listGroupMallCart.ElementAt(j).SetQty;
                    //标准计量单位下的数量
                    oMallCartProduct.productQuantity = listGroupMallCart.ElementAt(j).Quantity;
                    //转换后的实际默认计量单位的数量
                    if (oUnitPrice.UnitRatio == 0)
                    {
                        oUnitPrice.UnitRatio = 1;
                    }
                    decimal nPercent = Decimal.Parse("1" + new string('0', oUnitPrice.Percision));
                    oMallCartProduct.productFactCount = Math.Ceiling(listGroupMallCart.ElementAt(j).Quantity / oUnitPrice.UnitRatio * nPercent ) / nPercent;
                    //商品信息
                    oMallCartProduct.productName = listGroupMallCart.ElementAt(j).OnSkuItem.FullName.GetResource(LiveSession.Culture);
                    oMallCartProduct.productPicture = listGroupMallCart.ElementAt(j).OnSkuItem.OnSale.Picture;
                    //价格以及价格总计，总计按照实际从标准单位转换回来的数量计算
                    oMallCartProduct.productPriceSum = oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                    oMallCartProduct.productSalePrice = oUnitPrice.SalePrice.GetResource(globalCurrencyGid);
                    //计算合计
                    allMarketPrice = allMarketPrice + oUnitPrice.MarketPrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                    allSalePrice = allSalePrice + oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                    //对应商品的计量单位
                    oMallCartProduct.defaultUnit = oUnitPrice.ShowUnit.Name.GetResource(LiveSession.Culture);
                    oMallCartProduct.standardUnit = oUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.GetResource(LiveSession.Culture);
                    listMallProduct.Add(oMallCartProduct);
                    nProductCount = nProductCount + oMallCartProduct.productFactCount;
                }
                oMallCartInfo.productCount = nProductCount;
                oMallCartInfo.marketPriceSum = allMarketPrice;
                oMallCartInfo.salePriceSum = allSalePrice;
                oMallCartInfo.priceLower = allMarketPrice - allSalePrice;
                ViewBag.oMallCartProductList = listMallProduct;
            }

            return View(oMallCartInfo);
        }
        /// <summary>
        /// 去掉单个商品
        /// </summary>
        /// <returns></returns>
        public void DeleteSingleProduct(string deleteMallCartGid) 
        {
            string[] deleteGidList = deleteMallCartGid.Split('|');
            for(int i = 0; i<deleteGidList.Count();i++)
            {
                if (deleteGidList[i] != "")
                {
                    Guid deleteGid = Guid.Parse(deleteGidList[i]);
                    MallCart oDeleteMallCart = dbEntity.MallCarts.Where(p => p.Gid == deleteGid && p.Deleted == false).FirstOrDefault();
                    if (oDeleteMallCart != null)
                    {
                        oDeleteMallCart.Deleted = true;
                        dbEntity.SaveChanges();
                    }
                }
            }
        }
        /// <summary>
        /// 清空购物车
        /// </summary>
        public void DeleteAllProduct() 
        {
            List<MallCart> listMallCart = dbEntity.MallCarts.Where(p => p.Deleted == false && p.UserID == LiveSession.userID && p.ChlID == LiveSession.channelID).ToList();
            foreach (var item in listMallCart) 
            {
                item.Deleted = true;
            }
            dbEntity.SaveChanges();
        }
        /// <summary>
        /// 如果用户在前台添加购物车中的商品，需要对库存进行检验
        /// </summary>
        /// <param name="skuGid"></param>
        /// <param name="saleQty"></param>
        /// <returns></returns>
        public string CheckCanSaleQty(Guid skuGid, decimal saleQty) 
        {
            MallCart oMallCart = dbEntity.MallCarts.Include("Onsale").Include("OnSkuItem").Where(p => p.Gid == skuGid && p.Deleted == false).FirstOrDefault();
            if (oMallCart != null)
            {
                //判断上架商品是否允许超卖
                if (oMallCart.OnSkuItem.Overflow == true)
                {
                    return "success";
                }
                else 
                {
                    if (oMallCart.OnSkuItem.MaxQuantity < 0)
                    {
                        //按实际库存进行销售
                        //查找库存总账中SKU的数量
                        decimal canSaleQty = dbEntity.WarehouseLedgers.Where(p => p.SkuID == oMallCart.OnSkuItem.SkuID && p.Deleted == false).Max(p => p.CanSaleQty);
                        //如果所有仓库的可销售总和大于等于购物车中修改的数量，则返回success
                        if (canSaleQty >= saleQty)
                        {
                            return "success";
                        }
                        else
                        {
                            return "fail";
                        }
                    }
                    else 
                    {
                        //如果修改后的数字大于实际数量，则提示用户数量不足
                        if (saleQty > oMallCart.OnSkuItem.MaxQuantity)
                        {
                            return "fail";
                        }
                        else 
                        {
                            return "success";
                        }
                    }
                }
            }
            return "success";            
        }
        /// <summary>
        /// 获取购物车数量修改后的总数量以及价格的变化
        /// </summary>
        /// <param name="orgGid"></param>
        /// <returns></returns>
        public string GetMallCartPriceSum(Guid mallCartGid)
        {
            MallCartInfo oMallCartInfo = new MallCartInfo();
            MallCart oCurrentMallCart = dbEntity.MallCarts.Where(p => p.Gid == mallCartGid && p.Deleted == false).FirstOrDefault();
            //当前购物车所属的组织
            Guid oOrganizationGuid = (Guid)oCurrentMallCart.OrgID;
            List<MallCart> listGroupMallCart = dbEntity.MallCarts.Include("OnSkuItem").Include("OnSale").Include("Organization").Where(p => p.OrgID == oOrganizationGuid && p.ChlID == LiveSession.channelID && p.UserID == LiveSession.userID && p.Deleted == false).ToList();
            
            //拼接页面model需要的信息
            MemberOrganization oMember = dbEntity.MemberOrganizations.Include("FullName").Where(p => p.Gid == oOrganizationGuid && p.Deleted == false).FirstOrDefault();
            oMallCartInfo.organizationName = oMember.FullName.GetResource(LiveSession.Culture);
            oMallCartInfo.orgGid = oOrganizationGuid;
            //获取产品总数
            decimal nProductCount = 0m;
            //获取金额，以及设置页面商品List
            decimal allSalePrice = 0;
            decimal allMarketPrice = 0;
            List<MallCartProduct> listMallProduct = new List<MallCartProduct>();
            for (int j = 0; j < listGroupMallCart.Count; j++)
            {
                Guid onSkuGid = listGroupMallCart.ElementAt(j).OnSkuID;
                ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem.OnSale").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == onSkuGid && p.Deleted == false).OrderByDescending(p => p.IsDefault).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                MallCartProduct oMallCartProduct = new MallCartProduct();
                oMallCartProduct.mallCartGid = listGroupMallCart.ElementAt(j).Gid;
                //转换前的数量
                oMallCartProduct.productCount = listGroupMallCart.ElementAt(j).SetQty;
                //折扣，现在默认为1，不打折
                oMallCartProduct.productDiscount = 1;
                //套装模式
                oMallCartProduct.productMode = (byte)listGroupMallCart.ElementAt(j).OnSale.Mode;
                //套装数量
                oMallCartProduct.productSetCount = listGroupMallCart.ElementAt(j).SetQty;
                //标准计量单位下的数量
                oMallCartProduct.productQuantity = listGroupMallCart.ElementAt(j).Quantity;
                //转换后的实际默认计量单位的数量
                if (oUnitPrice.UnitRatio == 0)
                {
                    oUnitPrice.UnitRatio = 1;
                }
                decimal nPercent = Decimal.Parse("1" + new string('0', oUnitPrice.Percision));
                oMallCartProduct.productFactCount = Math.Ceiling(listGroupMallCart.ElementAt(j).Quantity / oUnitPrice.UnitRatio * nPercent) / nPercent;
                //商品信息
                oMallCartProduct.productName = listGroupMallCart.ElementAt(j).OnSkuItem.FullName.GetResource(LiveSession.Culture);
                oMallCartProduct.productPicture = listGroupMallCart.ElementAt(j).OnSkuItem.OnSale.Picture;
                //价格以及价格总计，总计按照实际从标准单位转换回来的数量计算
                oMallCartProduct.productPriceSum = oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                oMallCartProduct.productSalePrice = oUnitPrice.SalePrice.GetResource(globalCurrencyGid);
                //计算合计
                allMarketPrice = allMarketPrice + oUnitPrice.MarketPrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                allSalePrice = allSalePrice + oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                //对应商品的计量单位
                oMallCartProduct.defaultUnit = oUnitPrice.ShowUnit.Name.GetResource(LiveSession.Culture);
                oMallCartProduct.standardUnit = oUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.GetResource(LiveSession.Culture);
                listMallProduct.Add(oMallCartProduct);
                nProductCount = nProductCount + oMallCartProduct.productFactCount;
            }
            oMallCartInfo.productCount = nProductCount;
            oMallCartInfo.marketPriceSum = allMarketPrice;
            oMallCartInfo.salePriceSum = allSalePrice;
            oMallCartInfo.priceLower = allMarketPrice - allSalePrice;
            ViewBag.oMallCartProductList = listMallProduct;
            string backString = "商品数量总计：" + oMallCartInfo.productCount.ToString("#0.00") + "件 赠送积分总计：" + oMallCartInfo.backPoint + "分 购物金额小计 ￥" + oMallCartInfo.salePriceSum.ToString("#0.00")
                + "，比市场价 ￥" + oMallCartInfo.marketPriceSum.ToString("#0.00") + "节省了 ￥" + oMallCartInfo.priceLower.ToString("#0.00") + "元";
            return backString + "|" + oOrganizationGuid.ToString();
        }
        /// <summary>
        /// 用户在购物车列表页面更新商品数量
        /// </summary>
        /// <param name="mallCartGid"></param>
        /// <param name="changeNum"></param>
        /// <returns></returns>
        public string ChangeMallCartNum(Guid mallCartGid, decimal changeNum) 
        {
            MallCart oMallCart = dbEntity.MallCarts.Include("OnSkuItem").Include("OnSale").Include("Organization").Where(p => p.Gid == mallCartGid && p.Deleted == false).FirstOrDefault();
            if (oMallCart != null)
            {
                oMallCart.SetQty = changeNum;
                Guid onSkuGid = oMallCart.OnSkuID;
                ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem.OnSale").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == onSkuGid && p.Deleted == false).OrderByDescending(p => p.IsDefault).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                MallCartProduct oMallCartProduct = new MallCartProduct();
                oMallCartProduct.mallCartGid = oMallCart.Gid;
                //转换前的数量
                oMallCartProduct.productCount = oMallCart.SetQty;
                //折扣，现在默认为1，不打折
                oMallCartProduct.productDiscount = 1;
                //套装模式
                oMallCartProduct.productMode = (byte)oMallCart.OnSale.Mode;
                //套装数量
                oMallCartProduct.productSetCount = oMallCart.SetQty;
                //转换后的实际默认计量单位的数量
                if (oUnitPrice.UnitRatio == 0m)
                {
                    oUnitPrice.UnitRatio = 1m;
                }
                decimal nPercent = Decimal.Parse("1" + new string('0', oUnitPrice.Percision));
                oMallCart.Quantity = Math.Ceiling(changeNum * oUnitPrice.UnitRatio * nPercent) / nPercent;
                //标准计量单位下的数量
                oMallCartProduct.productQuantity = oMallCart.Quantity;
                //将修改后的数据存入数据库，套装数量以及转换后的数量
                dbEntity.SaveChanges();
                oMallCartProduct.productFactCount = Math.Ceiling(oMallCart.Quantity / oUnitPrice.UnitRatio * nPercent) / nPercent;
                //商品信息
                oMallCartProduct.productName = oMallCart.OnSkuItem.FullName.GetResource(LiveSession.Culture);
                oMallCartProduct.productPicture = oMallCart.OnSkuItem.OnSale.Picture;
                //价格以及价格总计，总计按照实际从标准单位转换回来的数量计算
                oMallCartProduct.productPriceSum = oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                oMallCartProduct.productSalePrice = oUnitPrice.SalePrice.GetResource(globalCurrencyGid);
                //对应商品的计量单位
                oMallCartProduct.defaultUnit = oUnitPrice.ShowUnit.Name.GetResource(LiveSession.Culture);
                oMallCartProduct.standardUnit = oUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.GetResource(LiveSession.Culture);

                return "实际数量：" + oMallCartProduct.productQuantity.ToString("#0.00") + " (" + oMallCartProduct.standardUnit + ") = " + oMallCartProduct.productFactCount.ToString("#0.00") + " (" + oMallCartProduct.defaultUnit + ")" + "|" + oMallCartProduct.productPriceSum.ToString("#0.00");
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region 根据不同组织的对商品进行结算
        /// <summary>
        /// 设置全局组织Gid 
        /// </summary>
        /// <param name="orgGid"></param>
        public void SetGlobleOrgGid(Guid orgGid) 
        {
            globalOrgGid = orgGid;
        }
        /// <summary>
        /// 设置是否使用快递或者物流
        /// </summary>
        /// <param name="bShipping"></param>
        public void SetUseShipping(bool bShipping)
        {
            bUseShipper = bShipping;
        }
        /// <summary>
        /// 进入购物车结算界面，区分不同的组织
        /// </summary>
        /// <returns></returns>
        public ActionResult Cart(Guid? memberAddGid)
        {
            //清空相关的全局变量
            globalCouponList = new Dictionary<Guid, decimal>();
            //清空全局商品的总执行价格
            globalProductExacuteAmount = 0m;
            //清空全局订单商品的列表
            globalOrderItemList = new Dictionary<Guid, OrderItem>();
            //清空全局订单商品的数量
            globalOrderItemPieces = 0m; 
            //需要判断是否需要新建订单
            oNewOrder = new OrderInformation();
            //承运商信息清空
            globalShipperGid = Guid.Empty;
            //将发票信息回复为默认
            bInvoiceOrNot = false;
            //默认不使用承运商（快递或物流）
            bUseShipper = false;

            List<MallCart> listMallCart = dbEntity.MallCarts.Include("Organization").Include("Channel").Include("OnSale").Include("OnSkuItem").Include("CartType").Where(p => p.OrgID == globalOrgGid && p.ChlID == LiveSession.channelID && p.UserID == LiveSession.userID && p.Deleted == false).ToList();
            if (listMallCart.Count > 0)
            {
                //收货人信息
                List<MemberAddress> oUserAddressList = dbEntity.MemberAddresses.Where(p => p.UserID == LiveSession.userID && p.Deleted == false).ToList();
                ViewBag.oAddressList = oUserAddressList;

                //交易类型信息
                List<SelectListItem> oTransList = new List<SelectListItem>();
                oTransList = GetSelectList(oNewOrder.TransTypeList);
                ViewBag.oTransList = oTransList;

                //支付信息
                List<FinancePayType> listPayment = new List<FinancePayType>();
                //取支付方式的交集
                for (int i = 0; i < listMallCart.Count; i++) 
                {
                    for (int j = 0; j < listMallCart.ElementAt(i).OnSale.OnPayments.Count; j++)
                    {
                        if (!listPayment.Contains(listMallCart.ElementAt(i).OnSale.OnPayments.ElementAt(j).PayType))
                        {
                            listPayment.Add(listMallCart.ElementAt(i).OnSale.OnPayments.ElementAt(j).PayType);
                        }
                    }
                }
                ViewBag.oPaymentList = listPayment;
                //券和积分的信息
                List<MemberPoint> listMemberCoupon = dbEntity.MemberPoints.Include("Promotion").Include("Coupon").Include("Currency").Where(p => p.Ptype == (byte)ModelEnum.PointType.COUPON && p.aCurrency == globalCurrencyGid && p.UserID == LiveSession.userID && p.OrgID == globalOrgGid && p.Deleted == false && p.Pstatus != (byte)ModelEnum.PointStatus.NONE && p.StartTime <= DateTimeOffset.Now && p.EndTime >= DateTimeOffset.Now && p.Balance > 0 || p.Remain > 0).ToList();
                List<MemberPoint> listArriveCoupon = new List<MemberPoint>();
                foreach (var couponItem in listMemberCoupon) 
                {               
                    listArriveCoupon.Add(couponItem);                   
                }
                List<MemberPoint> listMemberPoint = dbEntity.MemberPoints.Include("Promotion").Include("Coupon").Include("Currency").Where(p => p.Ptype == (byte)ModelEnum.PointType.POINT && p.Deleted == false && p.Pstatus != (byte)ModelEnum.PointStatus.NONE).ToList();
                if (listMemberPoint != null)
                {
                    ViewBag.pointList = listMemberPoint;
                }
                if (listArriveCoupon != null)
                {
                    ViewBag.arriveCouponList = listArriveCoupon;
                }

                //物流信息
                //独立页面

                //发票信息
                //从GeneralOptional表中取得，下拉框
                GeneralOptional invoiceOption = dbEntity.GeneralOptionals.Include("OptionalItems").Where(p => p.Deleted == false && p.Otype == (byte)ModelEnum.OptionalType.ORDER && p.Code == "Invoice").FirstOrDefault();
                List<SelectListItem> listInvoiceItem = new List<SelectListItem>();
                if (invoiceOption != null)
                {                    
                    for (int i = 0; i < invoiceOption.OptionalItems.Count; i++)
                    {
                        listInvoiceItem.Add(new SelectListItem { Text = invoiceOption.OptionalItems.ElementAt(i).Name.GetResource(LiveSession.Culture), Value = invoiceOption.OptionalItems.ElementAt(i).Gid.ToString() });
                    }
                }
                ViewBag.invoiceList = listInvoiceItem;

                //订单备注

                //商品清单
                List<MallCartProduct> listMallProduct = new List<MallCartProduct>();
                for (int j = 0; j < listMallCart.Count; j++)
                {
                    Guid onSkuGid = listMallCart.ElementAt(j).OnSkuID;
                    ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem.OnSale").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == onSkuGid && p.Deleted == false).OrderByDescending(p => p.IsDefault).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                    MallCartProduct oMallCartProduct = new MallCartProduct();
                    oMallCartProduct.mallCartGid = listMallCart.ElementAt(j).Gid;
                    //转换前的数量
                    oMallCartProduct.productCount = listMallCart.ElementAt(j).SetQty;
                    //折扣，现在默认为1，不打折
                    oMallCartProduct.productDiscount = 1;
                    //套装模式
                    oMallCartProduct.productMode = (byte)listMallCart.ElementAt(j).OnSale.Mode;
                    //套装数量，默认计量单位的数量
                    oMallCartProduct.productSetCount = listMallCart.ElementAt(j).SetQty;
                    //标准计量单位下的数量
                    oMallCartProduct.productQuantity = listMallCart.ElementAt(j).Quantity;
                    //转换后的实际默认计量单位的数量
                    if (oUnitPrice.Percision == 0)
                    {
                        oUnitPrice.Percision = 1;
                    }
                    decimal nPercent = Decimal.Parse("1" + new string('0', oUnitPrice.Percision));
                    oMallCartProduct.productFactCount = Math.Ceiling(listMallCart.ElementAt(j).Quantity / oUnitPrice.UnitRatio * nPercent) / nPercent;
                    //商品信息
                    oMallCartProduct.productName = listMallCart.ElementAt(j).OnSkuItem.FullName.GetResource(LiveSession.Culture);
                    oMallCartProduct.productPicture = listMallCart.ElementAt(j).OnSkuItem.OnSale.Picture;
                    //价格以及价格总计，总计按照实际从标准单位转换回来的数量计算
                    oMallCartProduct.productPriceSum = oUnitPrice.SalePrice.GetResource(globalCurrencyGid) * oMallCartProduct.productFactCount;
                    oMallCartProduct.productSalePrice = oUnitPrice.SalePrice.GetResource(globalCurrencyGid);
                    //对应商品的计量单位
                    oMallCartProduct.defaultUnit = oUnitPrice.ShowUnit.Name.GetResource(LiveSession.Culture);
                    oMallCartProduct.standardUnit = oUnitPrice.OnSkuItem.SkuItem.StandardUnit.Name.GetResource(LiveSession.Culture);
                    listMallProduct.Add(oMallCartProduct);
                    globalProductExacuteAmount = globalProductExacuteAmount + oMallCartProduct.productPriceSum;
                    //生成订单商品列表
                    OrderItem oNewOrderItem = new OrderItem();
                    oNewOrderItem.OnSkuID = onSkuGid;
                    oNewOrderItem.SkuID = listMallCart.ElementAt(j).OnSkuItem.SkuItem.Gid;
                    oNewOrderItem.Name = listMallCart.ElementAt(j).OnSkuItem.FullName.GetResource(LiveSession.Culture);
                    //标准计量单位的数量
                    oNewOrderItem.Quantity = listMallCart.ElementAt(j).Quantity;
                    oNewOrderItem.MarketPrice = oUnitPrice.MarketPrice.GetResource(globalCurrencyGid);
                    oNewOrderItem.SalePrice = oUnitPrice.SalePrice.GetResource(globalCurrencyGid);
                    oNewOrderItem.ExecutePrice = oMallCartProduct.productPriceSum / listMallCart.ElementAt(j).Quantity;
                    globalOrderItemList.Add(listMallCart.ElementAt(j).Gid, oNewOrderItem);
                    globalOrderItemPieces = globalOrderItemPieces + listMallCart.ElementAt(j).Quantity;
                }

                ViewBag.oProductItemList = listMallProduct;
                
            }
            oNewOrder.OrgID = globalOrgGid;
            oNewOrder.ChlID = LiveSession.channelID;
            oNewOrder.UserID = (Guid)LiveSession.userID;

            ViewBag.nCulture = LiveSession.Culture;
            return View();
        }
        /// <summary>
        /// 购物车的承运商信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CartShippingInfo(Guid? memberAddressGid) 
        {
            ViewBag.currentCulture = LiveSession.Culture;
            if (memberAddressGid != null) 
            {
                MemberAddress oMemberAddress = dbEntity.MemberAddresses.Where(p => p.Gid == memberAddressGid && p.Deleted == false).FirstOrDefault();
                if (oMemberAddress != null)
                {
                    //通过组织Gid，渠道Gid，用户Gid以及地区Gid来获得承运商信息
                    Guid bestShipperGid = orderBll.GetBestShipping(globalOrgGid, LiveSession.channelID, (Guid)LiveSession.userID, (Guid)oMemberAddress.aLocation);
                    //设定订单的收货人信息
                    oNewOrder.aLocation = (Guid)oMemberAddress.aLocation;
                    oNewOrder.Consignee = oMemberAddress.DisplayName;
                    oNewOrder.FullAddress = oMemberAddress.FullAddress;
                    oNewOrder.PostCode = oMemberAddress.PostCode;
                    oNewOrder.Telephone = oMemberAddress.HomePhone;
                    oNewOrder.Mobile = oMemberAddress.CellPhone;
                    oNewOrder.Email = oMemberAddress.Email;
                    if (!bestShipperGid.Equals(Guid.Empty))
                    {
                        ShippingInformation bestShippingInfo = dbEntity.ShippingInformations.Include("FullName").Where(p => p.Gid == bestShipperGid && p.Deleted == false).FirstOrDefault();
                        if (bestShippingInfo != null)
                        {
                            globalShipperGid = bestShipperGid;
                            ViewBag.bestShipper = bestShippingInfo;
                        }
                        else 
                        {
                            ViewBag.bestShipper = null;
                        }
                    }
                    else 
                    {
                        ViewBag.bestShipper = null;
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// 返回购物车结算时费用信息
        /// </summary>
        /// <returns></returns>
        public string CartFeeInfo(Guid? pointGid = null)
        {
            decimal currentShippingFee;
            if (oNewOrder.aLocation == null || oNewOrder.aLocation.Equals(Guid.Empty))
            {
                currentShippingFee = 0m;
            }
            else
            {
                //用户自提，则不需要承运商费用
                if (bUseShipper == true)
                {
                    //获取物流费
                    currentShippingFee = orderBll.GetCartShippingFee(globalCurrencyGid, globalOrgGid, LiveSession.channelID, (Guid)LiveSession.userID, (Guid)oNewOrder.aLocation);
                    //如果找不到承运商，价格为0
                    if (currentShippingFee == -1)
                    {
                        currentShippingFee = 0m;
                    }
                }
                else 
                {
                    currentShippingFee = 0m;
                }
            }
            oNewOrder.ShippingFee = currentShippingFee;
            //根据小计获得的价格
            oNewOrder.ExecuteAmount = globalProductExacuteAmount;
            //应付款为承运商费用加上商品的执行总价，减去用户使用券的钱
            oNewOrder.TotalFee = currentShippingFee + globalProductExacuteAmount;

            //获取券的信息
            //Todo，当前情况，券只能使用一张
            //如果存在券的信息，先判断是否为删除券
            if (pointGid != null)
            {
                //可以使用券信息
                MemberPoint oMemberPoint = dbEntity.MemberPoints.Include("Promotion").Include("Coupon").Where(p => p.Gid == pointGid && p.Deleted == false).FirstOrDefault();
               
                //当券中的余额足够支付应付款，则将应付款归0，并且将券的信息和余额添加入全局变量globalCouponList中。
                if (oMemberPoint.Balance >= oNewOrder.TotalFee)
                {
                    decimal couponBalance = oMemberPoint.Balance - oNewOrder.TotalFee;
                    //券支付的增加的钱为原来的应付款
                    oNewOrder.CouponPay = oNewOrder.TotalFee;
                    oNewOrder.TotalPaid = oNewOrder.CouponPay;
                    //将应付款置0
                    oNewOrder.OrderAmount = oNewOrder.TotalFee - oNewOrder.TotalPaid; 
                    //将券信息加入globalCouponList
                    //如果券是一次使用，则使用余额为0
                    if (oMemberPoint.OnceUse == true)
                    {
                        couponBalance = 0m;
                    }
                    globalCouponList.Clear();
                    globalCouponList.Add((Guid)pointGid, couponBalance);
                }
                else
                {
                    //增加的券支付为券的全额
                    oNewOrder.CouponPay = oMemberPoint.Balance;
                    oNewOrder.TotalPaid = oNewOrder.CouponPay;
                    //应付款减少当前券的可用余额
                    oNewOrder.OrderAmount = oNewOrder.TotalFee - oNewOrder.TotalPaid;
                    //将券信息加入globalCouponList
                    globalCouponList.Clear();
                    globalCouponList.Add((Guid)pointGid, 0m);
                }
            }
            
            return "实际应付价格：" + globalProductExacuteAmount.ToString("#0.00") + "(商品价格总计) + " + currentShippingFee.ToString("#0.00") + "(物流费用) - " + oNewOrder.CouponPay.ToString("#0.00") + "(券支付) = " + oNewOrder.OrderAmount.ToString("#0.00") + "(应付总价)";
        }        
        /// <summary>
        /// 用户确认订单
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public string SaveNewOrder(FormCollection formCollection) 
        {
            string strReturnInfo = "success";

            //检查收货人信息是否存在
            if (oNewOrder.aLocation == null || oNewOrder.aLocation.Equals(Guid.Empty))
            {
                strReturnInfo = "请选择收货人信息！";
                return strReturnInfo;
            }

            //检查承运商信息是否存在
            if (globalShipperGid == null || globalShipperGid.Equals(Guid.Empty))
            {
                strReturnInfo = "请选择承运商！";
                return strReturnInfo;
            }

            #region 提交订单的信息

            OrderInformation oConfirmOrder = new OrderInformation();
            //订单基本信息
            oConfirmOrder.OrgID = oNewOrder.OrgID;
            oConfirmOrder.ChlID = oNewOrder.ChlID;
            //计算出最佳仓库
            var bestWhID = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindBestWarehouse({0}, {1}, {2})", oNewOrder.OrgID, oNewOrder.ChlID, oNewOrder.aLocation).FirstOrDefault();
            oConfirmOrder.WhID = bestWhID;
            oConfirmOrder.UserID = oNewOrder.UserID;
            oConfirmOrder.DocVersion = 0;
            oConfirmOrder.aCurrency = globalCurrencyGid;
            //交易类型
            oConfirmOrder.TransType = byte.Parse(formCollection["TransType"]);
            //当前订单是货到付款，则不需要检查支付方式；否则检查用户是否选择支付方式
            if (oConfirmOrder.TransType != (byte)ModelEnum.TransType.COD)
            {
                if (oNewOrder.PayID == null)
                {
                    strReturnInfo = "请选择支付方式！";
                    return strReturnInfo;
                }
                else 
                {
                    oConfirmOrder.PayID = oNewOrder.PayID;
                }
            }
            oConfirmOrder.Pieces = globalOrderItemPieces;
            //计算商品销售总价
            decimal salePriceSum = 0m;
            for (int i = 0; i < globalOrderItemList.Count; i++)
            {
                salePriceSum = salePriceSum + globalOrderItemList.ElementAt(i).Value.SalePrice * globalOrderItemList.ElementAt(i).Value.Quantity;
            }

            #region 订单价格信息

            oConfirmOrder.SaleAmount = salePriceSum;
            //设定订单执行价的总和
            oConfirmOrder.ExecuteAmount = globalProductExacuteAmount;
            oConfirmOrder.ShippingFee = oNewOrder.ShippingFee;
            oConfirmOrder.TaxFee = oNewOrder.TaxFee;
            oConfirmOrder.Insurance = oNewOrder.Insurance;
            oConfirmOrder.PaymentFee = oNewOrder.PaymentFee;
            oConfirmOrder.PackingFee = oNewOrder.PackingFee;
            oConfirmOrder.ResidenceFee = oNewOrder.ResidenceFee;
            oConfirmOrder.LiftGateFee = oNewOrder.LiftGateFee;
            oConfirmOrder.InstallFee = oNewOrder.InstallFee;
            oConfirmOrder.OtherFee = oNewOrder.OtherFee;
            oConfirmOrder.TotalFee = oNewOrder.TotalFee;
            oConfirmOrder.UsePoint = oNewOrder.UsePoint;
            oConfirmOrder.PointPay = oNewOrder.PointPay;
            oConfirmOrder.CouponPay = oNewOrder.CouponPay;
            oConfirmOrder.BounsPay = oNewOrder.BounsPay;
            oConfirmOrder.MoneyPaid = oNewOrder.MoneyPaid;
            oConfirmOrder.Discount = oNewOrder.Discount;
            oConfirmOrder.TotalPaid = oNewOrder.TotalPaid;
            oConfirmOrder.OrderAmount = oNewOrder.OrderAmount;
            oConfirmOrder.Differ = oNewOrder.Differ;

            #endregion

            #region 收货人基本信息

            oConfirmOrder.Consignee = oNewOrder.Consignee;
            oConfirmOrder.aLocation = oNewOrder.aLocation;
            oConfirmOrder.FullAddress = oNewOrder.FullAddress;
            oConfirmOrder.PostCode = oNewOrder.PostCode;
            oConfirmOrder.Telephone = oNewOrder.Telephone;
            oConfirmOrder.Mobile = oNewOrder.Mobile;
            oConfirmOrder.Email = oNewOrder.Email;
            oConfirmOrder.BestDelivery = formCollection["OrderBestDelivery"];
            oConfirmOrder.PostComment = formCollection["OrderPosComment"];

            dbEntity.OrderInformations.Add(oConfirmOrder);
            dbEntity.SaveChanges();

            #endregion

            #endregion

            #region 保存订单商品信息
            //将全局订单商品的列表保存入数据库
            for (int i = 0; i < globalOrderItemList.Count; i++)
            {
                OrderItem oNewOrderItem = new OrderItem();
                oNewOrderItem.OrderID = oConfirmOrder.Gid;
                oNewOrderItem.OnSkuID = globalOrderItemList.ElementAt(i).Value.OnSkuID;
                oNewOrderItem.SkuID = globalOrderItemList.ElementAt(i).Value.SkuID;
                oNewOrderItem.Name = globalOrderItemList.ElementAt(i).Value.Name;
                oNewOrderItem.Quantity = globalOrderItemList.ElementAt(i).Value.Quantity;
                oNewOrderItem.MarketPrice = globalOrderItemList.ElementAt(i).Value.MarketPrice;
                oNewOrderItem.SalePrice = globalOrderItemList.ElementAt(i).Value.SalePrice;
                oNewOrderItem.ExecutePrice = globalOrderItemList.ElementAt(i).Value.ExecutePrice;
                dbEntity.OrderItems.Add(oNewOrderItem);
                //删除购物车
                Guid mallCartGid = globalOrderItemList.ElementAt(i).Key;
                MallCart oCurrrentMallCart = dbEntity.MallCarts.Where(p => p.Gid == mallCartGid && p.Deleted == false).FirstOrDefault();
                if (oCurrrentMallCart != null)
                {
                    oCurrrentMallCart.Deleted = true;
                }
                dbEntity.SaveChanges();
            }

            #endregion

            #region 保存订单承运商信息
            //如果使用承运商，则保存最佳承运商信息
            if (bUseShipper == true)
            {
                OrderShipping oNewOrderShipping = new OrderShipping();
                oNewOrderShipping.OrderID = oConfirmOrder.Gid;
                oNewOrderShipping.ShipID = globalShipperGid;
                dbEntity.OrderShippings.Add(oNewOrderShipping);
                dbEntity.SaveChanges();
            }
            
            #endregion

            #region 订单的属性

            #endregion

            #region 发票信息
            //有订单发票信息
            if (bInvoiceOrNot == true)
            {
                FinanceInvoice oNewFinanceInvoice = new FinanceInvoice();
                Guid optionItemGid = Guid.Parse(formCollection["InvoiceItem"]);
                GeneralOptItem oInvoiceItem = dbEntity.GeneralOptItems.Include("Name").Where(p => p.Gid == optionItemGid && p.Deleted == false).FirstOrDefault();
                if (oInvoiceItem != null)
                {
                    oNewFinanceInvoice.Matter = oInvoiceItem.Name.GetResource(LiveSession.Culture);
                }
                oNewFinanceInvoice.Title = formCollection["orderInvoice"];
                oNewFinanceInvoice.Amount = oConfirmOrder.OrderAmount;
                dbEntity.FinanceInvoices.Add(oNewFinanceInvoice);
                dbEntity.SaveChanges();
            }

            #endregion

            #region 将使用的券信息写入用户记录

            for (int i = 0; i < globalCouponList.Count; i++)
            {
                Guid pointGid = globalCouponList.ElementAt(i).Key;
                MemberPoint oMemberPoint = dbEntity.MemberPoints.Include("User").Include("Promotion").Include("Coupon").Where(p=>p.Gid == pointGid && p.Deleted==false).FirstOrDefault();
                if (oMemberPoint != null)
                {
                    decimal couponBalance = globalCouponList.ElementAt(i).Value;
                    MemberUsePoint oNewMemberUsePoint = new MemberUsePoint();
                    oNewMemberUsePoint.PointID = pointGid;
                    oNewMemberUsePoint.Pstatus = (byte)ModelEnum.PointUsed.USED;
                    oNewMemberUsePoint.RefType = (byte)ModelEnum.NoteType.ORDER;
                    oNewMemberUsePoint.RefID = oConfirmOrder.Gid;
                    //将原有的金额减去余额即为消费金额
                    oNewMemberUsePoint.Amount = oMemberPoint.Balance - couponBalance;
                    dbEntity.MemberUsePoints.Add(oNewMemberUsePoint);
                    //将积分表的余额变为当前余额
                    oMemberPoint.Balance = couponBalance;
                    //将状态变为已使用
                    oMemberPoint.Pstatus = (byte)ModelEnum.PointStatus.USED;
                    //当余额为0时，是否改变状态
                    //To do 不改变券的状态
                    dbEntity.SaveChanges();
                }
            }

            #endregion

            return strReturnInfo;
        }
        /// <summary>
        /// 设定支付方式
        /// </summary>
        /// <param name="payGid"></param>
        public void SetPayType(Guid payGid)
        {
            oNewOrder.PayID = payGid;
        }
        /// <summary>
        /// 设定发票信息
        /// </summary>
        /// <param name="bInvoice"></param>
        public void SetInvoice(bool bInvoice)
        {
            bInvoiceOrNot = bInvoice;
        }

        #endregion

        #region 添加新地址

        public ActionResult AddNewAddress()
        {
            return View();
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Diagnostics;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;
using LiveAzure.Models.Order;
using LiveAzure.Models.Mall;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Finance;

namespace LiveAzure.BLL
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-08-04         -->
    /// <summary>
    /// 订单BLL类
    /// </summary>
    public class OrderBLL : BaseBLL
    {
        // 承运商
        private class OneShipper
        {
            public Guid ShipID { get; set; }
            public int ShipWeight { get; set; }
        }

        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public OrderBLL(LiveEntities entity) : base(entity) { }

        /// <summary>
        /// 搜索所有支持的承运商
        /// </summary>
        /// <param name="orgID">组织ID</param>
        /// <param name="chlID">渠道ID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="location">地区ID=>GeneralRegion.Gid</param>
        /// <returns>承运商ID和权重</returns>
        /// <remarks>
        ///   CREATE FUNCTION fn_FindAllShippings (@OrgID uniqueidentifier, @ChlID uniqueidentifier, @UserID uniqueidentifier, @Location uniqueidentifier)
        ///     RETURNS @FindKeys TABLE (ShipID uniqueidentifier, ShipWeight int) AS
        /// </remarks>
        public Dictionary<Guid, int> GetAllShippings(Guid orgID, Guid chlID, Guid userID, Guid location)
        {
            Dictionary<Guid, int> oShippingList = new Dictionary<Guid, int>();      // 所有符合条件的承运商
            var fnFindShip = dbEntity.Database.SqlQuery<OneShipper>("SELECT ShipID, ShipWeight FROM dbo.fn_FindAllShippings({0}, {1}, {2}, {3})", orgID, chlID, userID, location);
            foreach (OneShipper fnItem in fnFindShip)
                oShippingList.Add(fnItem.ShipID, fnItem.ShipWeight);
            return oShippingList;
        }

        public List<GeneralRegion> GetFullRegions(Guid location)
        {
            List<GeneralRegion> oFullRegionList = new List<GeneralRegion>();
            List<Guid> oFullRegionGidList = dbEntity.Database.SqlQuery<Guid>("SELECT Gid FROM dbo.fn_FindFullRegions({0})", location).ToList();
            foreach (Guid FGid in oFullRegionGidList)
            {
                oFullRegionList.Add(dbEntity.GeneralRegions.Find(FGid));
            }
            return oFullRegionList;
        }

        /// <summary>
        /// 返回支持的承运商列表
        /// </summary>
        /// <param name="listOrderItem">商品列表</param>
        /// <param name="WhId">仓库ID</param>
        /// <param name="location">地区ID</param>
        /// <returns></returns>
        public List<ShippingInformation> GetSupportShippings(List<OrderItem> listOrderItem, Guid WhId, Guid location)
        {

            //var fnFindShip = dbEntity.Database.SqlQuery<OneShipper>("SELECT ShipID, ShipWeight FROM fn_FindOnSkuShippings({0}, {1}, {2}, {3}, {4})", orgID, chlID, whID, onSku, location);

            List<GeneralRegion> oFullRegionList = GetFullRegions(location);
            List<ShippingInformation> oShippingList = new List<ShippingInformation>();
            bool isFirst = true;
            foreach (OrderItem orderItem in listOrderItem)
            {
                //List<ShippingInformation> shippers = (from poi in dbEntity.ProductOnItems.AsEnumerable()
                //             join pos in dbEntity.ProductOnSales.AsEnumerable() on poi.OnSaleID equals pos.Gid
                //             join ps in dbEntity.ProductOnShippings.AsEnumerable() on pos.Gid equals ps.OnSaleID
                //             join psa in dbEntity.ProductOnShipAreas.AsEnumerable() on ps.Gid equals psa.OnShip
                //             join ws in dbEntity.WarehouseShippings.AsEnumerable() on ps.ShipID equals ws.ShipID
                //             join wr in dbEntity.WarehouseRegions.AsEnumerable() on ws.WhID equals wr.WhID
                //             join fr in oFullRegionList.AsEnumerable() on psa.RegionID equals fr.Gid
                //             where fr.Gid == wr.RegionID && poi.Gid == orderItem.OnSkuID && ws.WhID == WhId
                //             orderby ps.ShipWeight descending
                //             select ps.Shipper).ToList();
                List<OneShipper> listShippers = dbEntity.Database.SqlQuery<OneShipper>("SELECT ShipID, ShipWeight FROM dbo.fn_FindOnSkuShippings({0}, {1}, {2})", WhId, orderItem.OnSkuID, location).ToList();
                List<ShippingInformation> shippers = (from oneshipper in listShippers.AsEnumerable()
                                                         join shipper in dbEntity.ShippingInformations.AsEnumerable() on oneshipper.ShipID equals shipper.Gid
                                                         orderby oneshipper.ShipWeight descending
                                                         select shipper).ToList();
                if (isFirst == true)
                { 
                    isFirst = false;
                    oShippingList.InsertRange(0,shippers);
                    continue;
                }
                foreach (var shipper in oShippingList)
                {
                    if (!shippers.Contains(shipper))//取多次查询的交集
                    {
                        oShippingList.Remove(shipper);
                    }
                }
            }

            return oShippingList;
        }

        /// <summary>
        /// 搜索最佳承运商，用于计算运费
        /// </summary>
        /// <param name="orgID">组织ID</param>
        /// <param name="chlID">渠道ID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="location">地区ID=>GeneralRegion.Gid</param>
        /// <returns>最佳承运商</returns>
        /// <remarks>
        ///   CREATE FUNCTION fn_FindBestShipping (@OrgID uniqueidentifier, @ChlID uniqueidentifier, @UserID uniqueidentifier, @Location uniqueidentifier)
        ///     RETURNS uniqueidentifier AS
        /// </remarks>
        public Guid GetBestShipping(Guid orgID, Guid chlID, Guid userID, Guid location)
        {
            Guid oBestShip = Guid.Empty;
            var fnBestShip = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindBestShipping({0}, {1}, {2}, {3})", orgID, chlID, userID, location).FirstOrDefault();
            if (fnBestShip != null)
                Guid.TryParse(fnBestShip.ToString(), out oBestShip);
            return oBestShip;
        }

        /// <summary>
        /// 根据MallCart的内容查询预生成订单的运费
        /// </summary>
        /// <param name="currency">当前货币</param>
        /// <param name="orgID">组织</param>
        /// <param name="chlID">渠道</param>
        /// <param name="userID">用户</param>
        /// <param name="location">地区</param>
        /// <returns></returns>
        /// <remarks>
        ///   CREATE FUNCTION fn_CartShippingFee (@Currency uniqueidentifier, @OrgID uniqueidentifier, @ChlID uniqueidentifier, @UserID uniqueidentifier, @Location uniqueidentifier )
        ///     RETURNS money AS 
        /// </remarks>
        public decimal GetCartShippingFee(Guid currency, Guid orgID, Guid chlID, Guid userID, Guid location)
        {
            decimal mShipFee = 0m;
            var fnShippingFee = dbEntity.Database.SqlQuery<decimal>("SELECT dbo.fn_CartShippingFee({0}, {1}, {2}, {3}, {4})", currency, orgID, chlID, userID, location).FirstOrDefault();
            if (fnShippingFee != null)
                Decimal.TryParse(fnShippingFee.ToString(), out mShipFee);
            return mShipFee;
        }

        /// <summary>
        /// 根据订单明细，生成符合订单要求的承运商列表，并保存到OrderShipping表中
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>0成功</returns>
        public int PrepareOrderShippings(Guid orderID)
        {
            int nResult = 0;
            var spOrderShip = dbEntity.Database.SqlQuery<string>("EXECUTE sp_PrepareOrderShippings {0}", orderID).FirstOrDefault();
            if (spOrderShip != null)
                Int32.TryParse(spOrderShip.ToString(), out nResult);
            return nResult;
        }

        /// <summary>
        /// 根据订单明细，计算订单的运费
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>运费</returns>
        public decimal GetOrderShippingFee(Guid orderID)
        {
            decimal mShipFee = 0m;
            var fnShipFee = dbEntity.Database.SqlQuery<string>("SELECT dbo.fn_OrderShippingFee({0})", orderID).FirstOrDefault();
            if (fnShipFee != null)
                Decimal.TryParse(fnShipFee.ToString(), out mShipFee);
            return mShipFee;
        }

        /// <summary>
        /// 用户点击“提交”后
        /// 根据MallCart的内容生成订单及其全部附属内容
        /// 包括订单主表，订单项，配送方式，自定义属性等
        /// 直接保存，返回订单号
        /// 注意：SKU的增减等，直接操作MallCart表；地址直接保存到MemberAddress中
        ///       MallCart中同一个渠道，但SKU分属于多个组织，则应生成多个订单，分别结算
        /// </summary>
        /// <param name="currency">当前货币</param>
        /// <param name="orgID">组织</param>
        /// <param name="chlID">渠道</param>
        /// <param name="userID">用户</param>
        /// <param name="address">详细地址</param>
        /// <param name="parameters">其它参数说明，可空，不限顺序
        ///     账号余额/券支付：        "MemberPoint",  "MemberPoint.Gid;可用金额"
        ///     支付方式：               "PaymentType",  "FinancePayType.Gid"
        ///     配送方式 0物流 1自提：   "ShippingType", "0"
        ///     发票要求：               "Invoice",      "抬头;内容"
        ///     备注：                   "Remark",       "备注内容"
        ///     促销：                   "Promotion",    "内容待定"
        /// </param>
        /// <returns>订单号，0表示失败</returns>
        public string GenerateOrderFromCart(Guid currency, Guid orgID, Guid chlID, Guid userID, Guid address, Dictionary<string, object> parameters)
        {

            return null;
        }

        /// <summary>
        /// 从淘宝中间表生成系统订单
        /// </summary>
        /// <param name="organ"></param>
        /// <param name="channel"></param>
        /// <see cref=""/>
        /// <returns>0转换成功，数值表示失败计数</returns>
        public int GenerateOrderFromTaobao(MemberOrganization organ, MemberChannel channel)
        {
            string strUserPrefix = Utility.ConfigHelper.GlobalConst.GetSetting("TaobaoUserPrefix");
            int nFailCount = 0;       // 转换失败计数器
            string strFailTids = "";
            var oTbOrders = (from o in dbEntity.ExTaobaoOrders.Include("TaobaoOrderItems")
                             where o.Transfered == false && o.status == "WAIT_SELLER_SEND_GOODS"
                                   && o.OrgID == organ.Gid && o.ChlID == channel.Gid
                             select o).ToList();
            foreach (var oTrade in oTbOrders)
            {
                try
                {
                    // 创建数据库事务
                    using (var scope = new TransactionScope())
                    {
                        string strTid = oTrade.tid.ToString();
                        var oOrder = (from o in dbEntity.OrderInformations
                                      where o.OrgID == organ.Gid && o.ChlID == channel.Gid
                                            && o.LinkCode == strTid
                                      select o).FirstOrDefault();
                        bool bSuccess = false;
                        if (oOrder == null)
                        {
                            // 查询和新建用户
                            string strLoginName = strUserPrefix + oTrade.buyer_nick;
                            var oUser = (from u in dbEntity.MemberUsers
                                         where u.LoginName == strLoginName
                                         select u).FirstOrDefault();
                            if (oUser == null)
                            {
                                oUser = new MemberUser
                                {
                                    Organization = organ,
                                    Role = dbEntity.MemberRoles.Where(r => r.OrgID == organ.Gid && r.Code == "Public").FirstOrDefault(),
                                    Channel = channel,
                                    LoginName = strLoginName,
                                    Ustatus = (byte)ModelEnum.UserStatus.VALID,
                                    NickName = oTrade.buyer_nick,
                                    DisplayName = oTrade.buyer_nick,
                                    Passcode = String.IsNullOrEmpty(oTrade.receiver_mobile) ? oTrade.receiver_phone : oTrade.receiver_mobile,
                                    Culture = dbEntity.GeneralCultureUnits.Where(c => c.Culture == 2052).FirstOrDefault(),
                                    Email = oTrade.buyer_email
                                };
                            }

                            // 匹配地区
                            var oLocation = (from r in dbEntity.GeneralRegions
                                             where r.Code == "CHN"       // 中国
                                             select r).FirstOrDefault();
                            var oProvince = (from r in dbEntity.GeneralRegions
                                             where r.Parent.Code == "CHN"
                                                   && (r.ShortName == oTrade.receiver_state || r.Map01 == oTrade.receiver_state)
                                             select r).FirstOrDefault();
                            if (oProvince != null)
                            {
                                oLocation = oProvince;                   // 匹配到省
                                var oCity = (from r in dbEntity.GeneralRegions
                                             where r.aParent == oProvince.Gid
                                                   && (r.ShortName == oTrade.receiver_city || r.Map01 == oTrade.receiver_city)
                                             select r).FirstOrDefault();
                                if (oCity != null)
                                {
                                    oLocation = oCity;                   // 匹配到市
                                    var oDistrict = (from r in dbEntity.GeneralRegions
                                                     where r.aParent == oCity.Gid
                                                           && (r.ShortName == oTrade.receiver_district || r.Map01 == oTrade.receiver_district)
                                                     select r).FirstOrDefault();
                                    if (oDistrict != null)
                                        oLocation = oDistrict;           // 匹配到区
                                }
                            }

                            // 支付方式 -- 支付宝
                            var oPayType = (from p in dbEntity.FinancePayTypes
                                            where p.Deleted == false && p.OrgID == organ.Gid && p.Code == "alipay"
                                            select p).FirstOrDefault();
                            // 货币 -- 人民币
                            var oCurrency = (from u in dbEntity.GeneralMeasureUnits
                                             where u.Deleted == false && u.Code == "¥"
                                                   && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                                             select u).FirstOrDefault();

                            // 创建订单主表
                            oOrder = new OrderInformation
                            {
                                Organization = organ,
                                Channel = channel,
                                User = oUser,
                                LinkCode = strTid,
                                Ostatus = (byte)ModelEnum.OrderStatus.NONE,
                                TransType = (oTrade.type == "cod") ? (byte)ModelEnum.TransType.COD : (byte)ModelEnum.TransType.SECURED,
                                PayType = oPayType,
                                PayNote = oTrade.alipay_no,
                                // Pieces = oTrade.num,

                                Currency = oCurrency,
                                SaleAmount = Decimal.Parse(oTrade.total_fee),
                                // ExecuteAmount = Decimal.Parse(oTrade.payment) - Decimal.Parse(oTrade.post_fee),
                                ShippingFee = Decimal.Parse(oTrade.post_fee),
                                MoneyPaid = Decimal.Parse(oTrade.payment),
                                // Differ = 0,

                                PaidTime = new DateTimeOffset(oTrade.pay_time.Value, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)),
                                Consignee = oTrade.receiver_name,
                                Location = oLocation,
                                FullAddress = oTrade.receiver_state + oTrade.receiver_city + oTrade.receiver_district + oTrade.receiver_address,
                                PostCode = oTrade.receiver_zip,
                                Telephone = oTrade.receiver_phone,
                                Mobile = oTrade.receiver_mobile,
                                Email = oTrade.buyer_email,
                                PostComment = oTrade.buyer_memo,
                                LeaveWord = oTrade.seller_memo
                            };

                            // 子订单实付金额。精确到2位小数，单位:元。如:200.07，表示:200元7分。计算公式如下：payment = price * num + adjust_fee - discount_fee + post_fee
                            // (邮费，单笔子订单时子订单实付金额包含邮费，多笔子订单时不包含邮费)；
                            // 对于退款成功的子订单，由于主订单的优惠分摊金额，会造成该字段可能不为0.00元。建议使用退款前的实付金额减去退款单中的实际退款金额计算。
                            bool bSingleProduct = (oTrade.TaobaoOrderItems.Count() == 1);
                            decimal nTotalQty = 0m;      // 商品总数量，标准计量单位
                            decimal nItemAmount = 0m;    // 商品总金额

                            // 创建订单从表
                            foreach (var item in oTrade.TaobaoOrderItems)
                            {
                                // 先找OnSale.Code确定是否为pu-parts模式，SKU号对应不上时，必须判断转换失败
                                var oParts = (from s in dbEntity.ProductOnSales.Include("Product")
                                              where s.Deleted == false
                                                    && s.OrgID == organ.Gid && s.ChlID == channel.Gid
                                                    && s.Code == item.outer_sku_id
                                                    && s.Mode == (byte)ModelEnum.ProductMode.PU_PARTS
                                              orderby s.Ostatus descending
                                              select s).FirstOrDefault();
                                if (oParts == null)
                                {
                                    // PU-SKU 模式，只导入一个OnItem加入订单
                                    var oOnSku = (from s in dbEntity.ProductOnItems.Include("SkuItem").Include("OnSale")
                                                  where s.OnSale.Deleted == false && s.Deleted == false
                                                        && s.OnSale.Mode == (byte)ModelEnum.ProductMode.PU_SKU
                                                        && s.OnSale.OrgID == organ.Gid
                                                        && s.OnSale.ChlID == channel.Gid
                                                        && s.SkuItem.Code == item.outer_sku_id
                                                  orderby s.OnSale.Ostatus descending, s.OnSale.CreateTime descending
                                                  select s).FirstOrDefault();
                                    var oOnPrice = (from p in dbEntity.ProductOnUnitPrices.Include("MarketPrice").Include("SalePrice")
                                                    where p.Deleted == false
                                                          && p.OnSkuID == oOnSku.Gid       // oOnSku空，则直接到catch记录错误
                                                    orderby p.IsDefault descending, p.CreateTime descending
                                                    select p).FirstOrDefault();
                                    // 计量单位转换，进位法
                                    decimal nQuantity = Decimal.Parse(item.num.ToString());
                                    decimal nPercent = Decimal.Parse("1" + new string('0', oOnPrice.Percision));
                                    nQuantity = Math.Ceiling(nQuantity * oOnPrice.UnitRatio * nPercent) / nPercent;
                                    decimal nPayment = Decimal.Parse(item.payment);
                                    // 单笔子订单时子订单实付金额包含邮费，多笔子订单时不包含邮费
                                    if (bSingleProduct) nPayment -= oOrder.ShippingFee;
                                    OrderItem oOrderItem = new OrderItem
                                    {
                                        OnSkuItem = oOnSku,
                                        SkuItem = oOnSku.SkuItem,
                                        Name = item.title,
                                        Quantity = nQuantity,
                                        MarketPrice = oOnPrice.MarketPrice.GetResource(oOrder.Currency.Gid),
                                        SalePrice = oOnPrice.SalePrice.GetResource(oOrder.Currency.Gid),
                                        ExecutePrice = Math.Round(nPayment / nQuantity, 2),
                                        Remark = String.Format("淘宝：{0} | {1} | {2} | {3}", item.num, item.payment, item.discount_fee, item.adjust_fee)
                                    };
                                    // 统计与误差计算
                                    nTotalQty += nQuantity;
                                    nItemAmount += oOrderItem.ExecutePrice * nQuantity;
                                    oOrder.OrderItems.Add(oOrderItem);
                                }
                                else
                                {
                                    // PU-Parts模式，需要将所有OnItem加入订单，只第一个商品有价格，其他商品价格为零
                                    var oPartItems = (from i in dbEntity.ProductOnItems.Include("SkuItem").Include("OnSale")
                                                      where i.Deleted == false
                                                            && i.OnSaleID == oParts.Gid
                                                      select i).ToList();
                                    bool bIsFirst = true;
                                    decimal nQuantity = Decimal.Parse(item.num.ToString());
                                    decimal nPayment = Decimal.Parse(item.payment);
                                    // 单笔子订单时子订单实付金额包含邮费，多笔子订单时不包含邮费
                                    if (bSingleProduct) nPayment -= oOrder.ShippingFee;
                                    foreach (var oOnSku in oPartItems)
                                    {
                                        var oOnPrice = (from p in dbEntity.ProductOnUnitPrices.Include("MarketPrice").Include("SalePrice")
                                                        where p.Deleted == false && p.OnSkuID == oOnSku.Gid  // oOnSku空，则直接到catch记录错误
                                                        orderby p.IsDefault descending
                                                        select p).FirstOrDefault();
                                        // decimal nItemQty = oOnSku.SetQuantity * nQuantity;          // 不使用套装数量，使用转换算法
                                        // 计量单位转换，进位法
                                        decimal nPercent = Decimal.Parse("1" + new string('0', oOnPrice.Percision));
                                        decimal nItemQty = Math.Ceiling(nQuantity * oOnPrice.UnitRatio * nPercent) / nPercent;
                                        OrderItem oOrderItem = new OrderItem
                                        {
                                            OnSkuItem = oOnSku,
                                            SkuItem = oOnSku.SkuItem,
                                            Name = oOnSku.FullName.GetResource(2052),
                                            Quantity = nItemQty,
                                            MarketPrice = oOnPrice.MarketPrice.GetResource(oOrder.Currency.Gid),
                                            SalePrice = oOnPrice.SalePrice.GetResource(oOrder.Currency.Gid),
                                            ExecutePrice = (bIsFirst) ? Math.Round(nPayment / nItemQty, 2) : 0m,  // 第一个商品有价格，其他商品没有价格
                                            Remark = String.Format("淘宝：{0} | {1} | {2} | {3} | {4}", item.title, item.num, item.payment, item.discount_fee, item.adjust_fee)
                                        };
                                        // 统计与误差计算
                                        nTotalQty += nItemQty;
                                        nItemAmount += oOrderItem.ExecutePrice * nItemQty;
                                        oOrder.OrderItems.Add(oOrderItem);
                                        bIsFirst = false;
                                    }
                                }
                            }
                            oOrder.Pieces = nTotalQty;
                            oOrder.ExecuteAmount = nItemAmount;
                            oOrder.Differ = oOrder.MoneyPaid - oOrder.ShippingFee - nItemAmount;  // 误差
                            dbEntity.OrderInformations.Add(oOrder);
                            dbEntity.SaveChanges();

                            // 更新淘宝订单转换成功状态
                            oTrade.OrderID = oOrder.Gid;
                            oTrade.Transfered = true;
                            dbEntity.SaveChanges();

                            // 创建已收款记录
                            FinancePayment oFinance = new FinancePayment
                            {
                                Organization = organ,
                                PayTo = (byte)ModelEnum.PayDirection.TO_CORPBANK,
                                Pstatus = (byte)ModelEnum.PayStatus.PAID,
                                RefType = (byte)ModelEnum.NoteType.ORDER,
                                RefID = oOrder.Gid,
                                Reason = String.Format("支付宝：{0}", oTrade.alipay_no),
                                PayDate = new DateTimeOffset(oTrade.pay_time.Value, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)),
                                Currency = oOrder.Currency,
                                Amount = oOrder.MoneyPaid,
                                Remark = String.Format("淘宝订单：{0}", oTrade.tid)
                            };
                            dbEntity.FinancePayments.Add(oFinance);

                            // 索取发票
                            if (!String.IsNullOrEmpty(oTrade.invoice_name))
                            {
                                FinanceInvoice oInvoice = new FinanceInvoice
                                {
                                    OrderInfo = oOrder,
                                    Code = oOrder.Gid.ToString("N"),
                                    Title = oTrade.invoice_name,
                                    Amount = oOrder.MoneyPaid
                                };
                                dbEntity.FinanceInvoices.Add(oInvoice);
                            }
                            dbEntity.SaveChanges();
                            bSuccess = true;
                        }
                        else
                        {
                            // TODO Warning

                            nFailCount++;
                            strFailTids += String.Format("{0},", oTrade.tid);
                        }
                        scope.Complete();
                        if (bSuccess)
                        {
                            oEventBLL.WriteEvent(String.Format("淘宝订单转换成功: {0}, {1}", oOrder.Code, oTrade.tid),
                                ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
                        }
                    }
                }
                catch
                {
                    nFailCount++;
                    strFailTids += String.Format("{0},", oTrade.tid);
                }
            }
            if (nFailCount > 0)
            {
                oEventBLL.WriteEvent(String.Format("淘宝订单转换失败: {0}", strFailTids),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
            return nFailCount;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="oEdit"></param>
        public void ReturnMoney(OrderInformation oEdit)
        {
            Guid OrderId = oEdit.Gid;
            dbEntity.Database.SqlQuery<string>("EXECUTE sp_UpdatePointByOrder {0}", OrderId);
            List<MemberUsePoint> PointList = dbEntity.MemberUsePoints.Where(p => p.Deleted == false && p.RefType == (byte)ModelEnum.NoteType.ORDER && p.RefID == OrderId).ToList();
            foreach (MemberUsePoint item in PointList)
            {
                item.Deleted = true;
            }
        }

        //陆旻添加，将购买的商品加入购物车
        public int GenerateMallCartFromPortal(Guid orgGid, Guid chlGid, Guid onSkuGid, decimal setQuantity, Guid unitGid) 
        {            
            ProductOnItem oProductOnItem = dbEntity.ProductOnItems.Include("OnSale").Include("SkuItem").Include("OnSkuPrices").Where(p => p.Gid == onSkuGid && p.Deleted == false).FirstOrDefault();
            if (oProductOnItem != null)
            {
                //如果是Pu-SKU模式
                if (oProductOnItem.OnSale.Mode == (byte)ModelEnum.ProductMode.PU_SKU)
                {
                    MallCart oNewMallCart = new MallCart();
                    oNewMallCart.OrgID = orgGid;
                    oNewMallCart.ChlID = chlGid;
                    oNewMallCart.OnSaleID = oProductOnItem.OnSale.Gid;
                    oNewMallCart.SetQty = setQuantity;
                    ProductOnUnitPrice oPriceUnit = dbEntity.ProductOnUnitPrices.Where(p => p.OnSkuID == onSkuGid && p.aShowUnit == unitGid && p.Deleted == false).FirstOrDefault();
                    if (oPriceUnit != null)
                    {
                        //转换为标准计量单位
                        decimal nPercent = Decimal.Parse("1" + new string('0', oPriceUnit.Percision));
                        oNewMallCart.Quantity = Math.Ceiling(setQuantity * oPriceUnit.UnitRatio * nPercent) / nPercent;
                    }
                    else 
                    {
                        //页面报错
                        return 1;
                    }
                    dbEntity.MallCarts.Add(oNewMallCart);
                    dbEntity.SaveChanges();
                }
                else if (oProductOnItem.OnSale.Mode == (byte)ModelEnum.ProductMode.PU_PARTS) 
                {
                    foreach (var item in oProductOnItem.OnSale.OnSkuItems) 
                    {
                        MallCart oNewMallCart = new MallCart();
                        oNewMallCart.OrgID = orgGid;
                        oNewMallCart.ChlID = chlGid;
                        oNewMallCart.OnSaleID = oProductOnItem.OnSale.Gid;
                        oNewMallCart.SetQty = setQuantity;
                        ProductOnUnitPrice oPriceUnit = dbEntity.ProductOnUnitPrices.Where(p => p.OnSkuID == item.Gid && p.aShowUnit == unitGid && p.Deleted == false).FirstOrDefault();
                        if (oPriceUnit != null)
                        {
                            //转换为标准计量单位
                            decimal nPercent = Decimal.Parse("1" + new string('0', oPriceUnit.Percision));
                            oNewMallCart.Quantity = Math.Ceiling(setQuantity * oPriceUnit.UnitRatio * nPercent) / nPercent;
                        }
                        else
                        {
                            //页面报错
                            return 1;
                        }
                        dbEntity.MallCarts.Add(oNewMallCart);
                        dbEntity.SaveChanges();
                    }
                }
            }
            //操作成功
            return 0;
        }

    }
}

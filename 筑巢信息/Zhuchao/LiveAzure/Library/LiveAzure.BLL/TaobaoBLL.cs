using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Data;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using Top.Api.Domain;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Exchange;
using LiveAzure.Models.General;
using LiveAzure.Models.Product;

namespace LiveAzure.BLL.Taobao
{
    /// <summary>
    /// 淘宝测试
    /// </summary>
    public class TaobaoBLL : BaseAPI
    {
        public TaobaoBLL(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        public void TestListTrade()
        {
            // 沙箱测试  http://gw.api.tbsandbox.com/router/rest
            // 正式环境  http://gw.api.taobao.com/router/rest
            
            string url = "http://gw.api.taobao.com/router/rest";
            string appkey = "12176743";
            string appsecret = "a9e366dde6816c2866e4f60af62162ca";
            string appsession = "23767603b359d0623b84b6963b5507db8f6b3_1";

            ITopClient client = new DefaultTopClient(url, appkey, appsecret);
            TradesSoldGetRequest req = new TradesSoldGetRequest();
            req.Fields = "seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_rate, status, payment, discount_fee, adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee, pic_path, num_iid, num, price, cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone, orders";
            req.StartCreated = DateTime.Parse("2011-08-01 00:00:00");
            req.EndCreated = DateTime.Parse("2011-08-10 23:59:59");
            req.PageNo = 1L;
            req.PageSize = 100L;
            TradesSoldGetResponse response = client.Execute(req, appsession);
            foreach (Trade oTrade in response.Trades)
            {
                Debug.WriteLine("LiveAzure.BLL.TaobaoBLL: {0} {1} {2}", oTrade.Tid, oTrade.Status, oTrade.ReceiverName);
                foreach (Order oOrder in oTrade.Orders)
                {
                    Debug.WriteLine("    {0} {1} {2}", oOrder.SkuId, oOrder.OuterIid, oOrder.Price);
                }
            }
        }

    }

    /// <summary>
    /// 淘宝类的公用基类
    /// </summary>
    public class BaseAPI : BaseBLL
    {
        protected MemberOrgChannel oChannel;       // 组织和渠道
        protected ITopClient oTopClient;           // 淘宝接口
        protected string strAppSession;

        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        /// <remarks>
        ///   沙箱测试  http://gw.api.tbsandbox.com/router/rest
        ///   正式环境  http://gw.api.taobao.com/router/rest
        /// </remarks>
        public BaseAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity)
        {
            this.dbEntity = entity;
            this.oChannel = channel;
            this.oTopClient = new DefaultTopClient(channel.RemoteUrl, channel.ConfigKey, channel.SecretKey);
            this.strAppSession = channel.SessionKey;
        }
    }

    /// <summary>
    /// 淘宝-用户API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:1"/>
    public class UserAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public UserAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        /// <summary>
        /// 下载淘宝用户
        /// </summary>
        /// <param name="nicks">用户昵称，多个昵称用逗号分隔</param>
        public void GetUsers(string nicks)
        {
            try
            {
                UsersGetRequest req = new UsersGetRequest();
                req.Fields = "user_id, uid, nick, sex, buyer_credit, seller_credit, location, created, last_visit, birthday, type, has_more_pic, item_img_num, item_img_size, prop_img_num, prop_img_size, auto_repost, promoted_type, status, alipay_bind, consumer_protection, alipay_account, alipay_no, avatar, liangpin, sign_food_seller_promise, has_shop, is_lightning_consignment, vip_info, email, magazine_subscribe, vertical_market, online_gaming";
                req.Nicks = nicks;
                UsersGetResponse response = oTopClient.Execute(req, strAppSession);
                ProcessUser(response.Users);
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝下载用户失败: {0}, {1}", nicks, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 保存淘宝用户
        /// </summary>
        /// <param name="oUsers">淘宝用户</param>
        private void ProcessUser(List<User> oUsers)
        {
            // 保存中间表
            foreach (User oUser in oUsers)
            {
                ExTaobaoUser oExUser = (from u in dbEntity.ExTaobaoUsers
                                        where u.user_id == oUser.UserId
                                        select u).FirstOrDefault();
                if (oExUser == null)
                {
                    oExUser = new ExTaobaoUser();
                    #region 保存中间表
                    DateTime dtDateTime;
                    oExUser.user_id = oUser.UserId;
                    oExUser.uid = oUser.Uid;
                    oExUser.nick = oUser.Nick;
                    oExUser.sex = oUser.Sex;
                    if (oUser.BuyerCredit != null)
                        oExUser.BuyerCredit = new ExTaobaoUserCredit
                        {
                            level = oUser.BuyerCredit.Level,
                            score = oUser.BuyerCredit.Score,
                            total_num = oUser.BuyerCredit.TotalNum,
                            good_num = oUser.BuyerCredit.GoodNum
                        };
                    if (oUser.SellerCredit != null)
                        oExUser.SellerCredit = new ExTaobaoUserCredit
                        {
                            level = oUser.SellerCredit.Level,
                            score = oUser.SellerCredit.Score,
                            total_num = oUser.SellerCredit.TotalNum,
                            good_num = oUser.SellerCredit.GoodNum
                        };
                    //ExTaobaoLocation oLocation = new ExTaobaoLocation
                    //{
                    //    zip = oUser.Location.Zip,
                    //    address = oUser.Location.Address,
                    //    city = oUser.Location.City,
                    //    state = oUser.Location.State,
                    //    country = oUser.Location.Country,
                    //    district = oUser.Location.District
                    //};
                    if (DateTime.TryParseExact(oUser.Created, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExUser.created = dtDateTime;
                    if (DateTime.TryParseExact(oUser.LastVisit, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExUser.last_visit = dtDateTime;
                    if (DateTime.TryParseExact(oUser.Birthday, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExUser.birthday = dtDateTime;
                    oExUser.type = oUser.Type;
                    oExUser.has_more_pic = oUser.HasMorePic;
                    oExUser.item_img_num = oUser.ItemImgNum;
                    oExUser.item_img_size = oUser.ItemImgSize;
                    oExUser.prop_img_num = oUser.PropImgNum;
                    oExUser.prop_img_size = oUser.PropImgSize;
                    oExUser.auto_repost = oUser.AutoRepost;
                    oExUser.promoted_type = oUser.PromotedType;
                    oExUser.status = oUser.Status;
                    oExUser.alipay_bind = oUser.AlipayBind;
                    oExUser.consumer_protection = oUser.ConsumerProtection;
                    oExUser.alipay_account = oUser.AlipayAccount;
                    oExUser.alipay_no = oUser.AlipayNo;
                    oExUser.avatar = oUser.Avatar;
                    oExUser.liangpin = oUser.Liangpin;
                    oExUser.sign_food_seller_promise = oUser.SignFoodSellerPromise;
                    oExUser.has_shop = oUser.HasShop;
                    oExUser.is_lightning_consignment = oUser.IsLightningConsignment;
                    oExUser.vip_info = oUser.VipInfo;
                    oExUser.email = oUser.Email;
                    oExUser.magazine_subscribe = oUser.MagazineSubscribe;
                    oExUser.vertical_market = oUser.VerticalMarket;
                    oExUser.online_gaming = oUser.OnlineGaming;
                    #endregion
                    dbEntity.ExTaobaoUsers.Add(oExUser);
                    dbEntity.SaveChanges();
                    oEventBLL.WriteEvent(String.Format("淘宝下载用户: {0}, {1}", oUser.UserId, oUser.Nick),
                        ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
                }
                else
                {
                    // TODO 更新?
                }
            }
        }
    }

    /// <summary>
    /// 淘宝-产品API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:2"/>
    public class ProductAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public ProductAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        public void ProductsGet()
        {
            ItemsCustomGetRequest req = new ItemsCustomGetRequest();
            req.Fields = "detail_url, num_iid, title, nick, type, desc, skus, created, auction_point, property_alias, cid, seller_cids, props, input_pids, input_str, pic_url, num, valid_thru, list_time, delist_time, stuff_status, location, price, post_fee, express_fee, ems_fee, has_discount, freight_payer, has_invoice, has_warranty, has_showcase, modified, increment, approve_status, postage_id, product_id, item_imgs, prop_imgs, outer_id, is_virtual, is_taobao, is_ex, videos, is_3D, one_station, second_kill, auto_fill, violation";
            req.OuterId = "1509000248-1";
            ItemsCustomGetResponse response = oTopClient.Execute(req);
            foreach (var item in response.Items)
            {
                foreach (var sku in item.Skus)
                {
                    Debug.WriteLine("Sku: {0}, {1}, {2}, {3}", sku.NumIid, sku.SkuId, sku.OuterId, sku.Price);
                }
            }
        }

        /// <summary>
        /// 更新淘宝库存价格等信息
        /// </summary>
        /// <param name="onSale">上架PU</param>
        public void ProductUpdate(ProductOnSale onSale)
        {
            // 货币 -- 淘宝只支持人民币
            var oCurrency = (from u in dbEntity.GeneralMeasureUnits
                             where u.Deleted == false && u.Code == "¥"
                                   && u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                             select u).FirstOrDefault();
            decimal nPriceHigh = 0m;
            decimal nPriceLow = 0m;
            foreach (var oOnSku in onSale.OnSkuItems)
            {
                var oOnPrice = (from p in dbEntity.ProductOnUnitPrices.Include("MarketPrice").Include("SalePrice")
                                where p.Deleted == false && p.OnSkuID == oOnSku.Gid
                                orderby p.IsDefault descending, p.CreateTime descending
                                select p).FirstOrDefault();
                decimal nOnPrice = oOnPrice.SalePrice.GetResource(oCurrency.Gid);
                if (nOnPrice > nPriceHigh)
                    nPriceHigh = nOnPrice;
                if (nOnPrice < nPriceLow)
                    nPriceLow = nOnPrice;
            }

            WarehouseBLL oWarehouseBLL = new WarehouseBLL(this.dbEntity);
            foreach (var oOnSku in onSale.OnSkuItems)
            {
                string sSkuCode = oOnSku.SkuItem.Code;
                var oOnPrice = (from p in dbEntity.ProductOnUnitPrices.Include("MarketPrice").Include("SalePrice")
                                where p.Deleted == false && p.OnSkuID == oOnSku.Gid
                                orderby p.IsDefault descending, p.CreateTime descending
                                select p).FirstOrDefault();
                // 价格
                decimal nOnPrice = oOnPrice.SalePrice.GetResource(oCurrency.Gid);
                // 库存
                decimal nStockQty = oWarehouseBLL.GetCansaleQty(oOnSku.Gid, oOnPrice.aShowUnit);
                try
                {
                    // 更新淘宝SKU，包括库存和价格
                    SkusCustomGetRequest req = new SkusCustomGetRequest();
                    req.OuterId = sSkuCode;
                    req.Fields = "sku_id, num_iid, properties, quantity, price, outer_id, created, modified, status";
                    SkusCustomGetResponse response = oTopClient.Execute(req, strAppSession);
                    foreach (Sku oTopSku in response.Skus)
                    {
                        ItemSkuUpdateRequest reqsku = new ItemSkuUpdateRequest();
                        reqsku.NumIid = oTopSku.NumIid;
                        reqsku.Properties = oTopSku.Properties;
                        if (nStockQty > 0)
                            reqsku.Quantity = Int64.Parse(Math.Round(nStockQty).ToString());
                        reqsku.Price = nOnPrice.ToString("#0.00");
                        reqsku.OuterId = sSkuCode;
                        reqsku.ItemPrice = nPriceHigh.ToString("#0.00");
                        ItemSkuUpdateResponse ressku = oTopClient.Execute(reqsku, strAppSession);

                        if (Utility.ConfigHelper.GlobalConst.IsDebug)
                            Debug.WriteLine("{0} {1} {2}", this.ToString(), ressku.IsError, ressku.Sku.NumIid);
                    }
                    oEventBLL.WriteEvent(String.Format("淘宝更新SKU价格和库存"),
                        ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.EXCHANGE, this.ToString());
                }
                catch (Exception ex)
                {
                    oEventBLL.WriteEvent(String.Format("淘宝更新SKU价格和库存失败: {0}", ex.Message),
                        ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.EXCHANGE, this.ToString());
                }
            }
        }

    }

    /// <summary>
    /// 淘宝-类目属性API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:3"/>
    public class CategoryAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public CategoryAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

    }

    /// <summary>
    /// 淘宝-交易API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:5"/>
    public class TranscationAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="oChannel">操作的组织和渠道</param>
        public TranscationAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        /// <summary>
        /// 下载淘宝订单，并转换成Stage订单
        /// </summary>
        /// <param name="dtStartCreated">开始时间</param>
        /// <param name="dtEndCreated">结束数据</param>
        public void DownloadOrders(DateTime dtStartCreated, DateTime dtEndCreated)
        {
            try
            {
                TradesSoldGetRequest req = new TradesSoldGetRequest();
                req.Fields = "seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_rate, status, payment, discount_fee, adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee, pic_path, num_iid, num, price, cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone, orders";
                req.StartCreated = dtStartCreated;
                req.EndCreated = dtEndCreated;
                req.PageNo = 1L;
                req.PageSize = 10L;
                int nCount = 1;
                do
                {
                    TradesSoldGetResponse response = oTopClient.Execute(req, strAppSession);
                    foreach (Trade oTrade in response.Trades)
                    {
                        // 更新订单详情，对比字段如下
                        // taobao.trade.fullinfo.get: seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_rate,             status, payment, discount_fee, adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee,                                                    pic_path, num_iid, num, price,                  cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone, orders
                        // taobao.trades.sold.get:    seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_flag, buyer_rate, status, payment,               adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee, buyer_memo, seller_memo, alipay_no, buyer_message, pic_path, num_iid, num, price, buyer_alipay_no, cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone,         buyer_email, seller_flag, seller_alipay_no, seller_mobile, seller_phone, seller_name, seller_email, available_confirm_fee, has_post_fee, timeout_action_time, snapshot_url, trade_memo, is_3D, buyer_memo, buyer_email
                        TradeFullinfoGetRequest reqFullInfo = new TradeFullinfoGetRequest();
                        reqFullInfo.Fields = "seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_flag, buyer_rate, status, payment,               adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee, buyer_memo, seller_memo, alipay_no, buyer_message, pic_path, num_iid, num, price, buyer_alipay_no, cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone,         buyer_email, seller_flag, seller_alipay_no, seller_mobile, seller_phone, seller_name, seller_email, available_confirm_fee, has_post_fee, timeout_action_time, snapshot_url, trade_memo, is_3D, orders, promotion_details";
                        reqFullInfo.Tid = oTrade.Tid;
                        TradeFullinfoGetResponse resFullInfo = oTopClient.Execute(reqFullInfo, strAppSession);
                        if (!resFullInfo.IsError)
                        {
                            oTrade.BuyerRate = resFullInfo.Trade.BuyerRate;
                            oTrade.BuyerMemo = resFullInfo.Trade.BuyerMemo;
                            oTrade.SellerMemo = resFullInfo.Trade.SellerMemo;
                            oTrade.AlipayNo = resFullInfo.Trade.AlipayNo;
                            oTrade.BuyerMessage = resFullInfo.Trade.BuyerMessage;
                            oTrade.BuyerAlipayNo = resFullInfo.Trade.BuyerAlipayNo;
                            oTrade.BuyerEmail = resFullInfo.Trade.BuyerEmail;
                            oTrade.SellerFlag = resFullInfo.Trade.SellerFlag;
                            oTrade.SellerAlipayNo = resFullInfo.Trade.SellerAlipayNo;
                            oTrade.SellerMobile = resFullInfo.Trade.SellerMobile;
                            oTrade.SellerPhone = resFullInfo.Trade.SellerPhone;
                            oTrade.SellerName = resFullInfo.Trade.SellerName;
                            oTrade.SellerEmail = resFullInfo.Trade.SellerEmail;
                            oTrade.AvailableConfirmFee = resFullInfo.Trade.AvailableConfirmFee;
                            oTrade.HasPostFee = resFullInfo.Trade.HasPostFee;
                            oTrade.TimeoutActionTime = resFullInfo.Trade.TimeoutActionTime;
                            oTrade.SnapshotUrl = resFullInfo.Trade.SnapshotUrl;
                            oTrade.TradeMemo = resFullInfo.Trade.TradeMemo;
                            oTrade.Is3D = resFullInfo.Trade.Is3D;
                            Debug.WriteLine("{0} {1} 下载淘宝订单详情 {2}", this.ToString(), nCount++, oTrade.Tid);
                        }
                    }
                    if (response.TotalResults > req.PageNo * req.PageSize)
                        req.PageNo++;
                    else
                        req.PageNo = 0;
                    ProcessOrders(response.Trades);
                } while (req.PageNo > 0L);
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("下载淘宝订单失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 下载淘宝增量订单，并转换成Stage订单
        /// </summary>
        public void DownloadIncOrders()
        {
            try
            {
                TradesSoldIncrementGetRequest req = new TradesSoldIncrementGetRequest();
                req.Fields = "seller_nick, buyer_nick, title, type, created, tid, seller_rate, buyer_rate, status, payment, discount_fee, adjust_fee, post_fee, total_fee, pay_time, end_time, modified, consign_time, buyer_obtain_point_fee, point_fee, real_point_fee, received_payment, commission_fee, pic_path, num_iid, num, price, cod_fee, cod_status, shipping_type, receiver_name, receiver_state, receiver_city, receiver_district, receiver_address, receiver_zip, receiver_mobile, receiver_phone, orders";
                req.StartModified = DateTime.Now.AddHours(-20);
                req.EndModified = DateTime.Now;
                req.UseHasNext = true;
                req.PageNo = 1L;
                req.PageSize = 50L;
                bool bHasNext = false;
                do
                {
                    TradesSoldIncrementGetResponse response = oTopClient.Execute(req, strAppSession);
                    bHasNext = response.HasNext;
                    req.PageNo++;
                    ProcessOrders(response.Trades);
                } while (bHasNext);
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("下载淘宝订单失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 保存中间表
        /// </summary>
        /// <param name="oTrades">淘宝订单</param>
        private void ProcessOrders(List<Trade> oTrades)
        {
            // 保存中间表
            foreach (Trade oTrade in oTrades)
            {
                ExTaobaoOrder oExOrder = (from t in dbEntity.ExTaobaoOrders
                                          where t.tid == oTrade.Tid
                                          select t).FirstOrDefault();
                bool bChanged = false;
                string sLogMsg = String.Empty;
                if (oExOrder == null)
                {
                    oExOrder = new ExTaobaoOrder();
                    dbEntity.ExTaobaoOrders.Add(oExOrder);
                    bChanged = true;
                    sLogMsg = String.Format("下载淘宝订单: {0}", oTrade.Tid);
                }
                else if (oExOrder.status != oTrade.Status)
                {
                    // 更新标识，以订单状态为准
                    bChanged = true;
                    oExOrder.Changed = true;
                    sLogMsg = String.Format("更新淘宝订单: {0}", oTrade.Tid);
                }
                if (bChanged)
                {
                    #region 保存淘宝中间表
                    // 以下更新不包括部分详情，例如alipay_no，需要用获取订单详情接口重新下载
                    oExOrder.OrgID = this.oChannel.OrgID;
                    oExOrder.ChlID = this.oChannel.ChlID;
                    DateTime dtDateTime;
                    if (DateTime.TryParseExact(oTrade.EndTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.end_time = dtDateTime;
                    oExOrder.buyer_message = oTrade.BuyerMessage;
                    oExOrder.shipping_type = oTrade.ShippingType;
                    oExOrder.buyer_cod_fee = oTrade.BuyerCodFee;
                    oExOrder.seller_cod_fee = oTrade.SellerCodFee;
                    oExOrder.express_agency_fee = oTrade.ExpressAgencyFee;
                    oExOrder.alipay_warn_msg = oTrade.AlipayWarnMsg;
                    oExOrder.status = oTrade.Status;
                    oExOrder.buyer_memo = oTrade.BuyerMemo;
                    oExOrder.seller_memo = oTrade.SellerMemo;
                    if (DateTime.TryParseExact(oTrade.Modified, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.modified = dtDateTime;
                    oExOrder.buyer_flag = oTrade.BuyerFlag;
                    oExOrder.seller_flag = oTrade.SellerFlag;
                    oExOrder.trade_from = oTrade.TradeFrom;
                    oExOrder.seller_nick = oTrade.SellerNick;
                    oExOrder.buyer_nick = oTrade.BuyerNick;
                    oExOrder.title = oTrade.Title;
                    oExOrder.type = oTrade.Type;
                    if (DateTime.TryParseExact(oTrade.Created, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.created = dtDateTime;
                    oExOrder.iid = oTrade.Iid;
                    oExOrder.price = oTrade.Price;
                    oExOrder.pic_path = oTrade.PicPath;
                    oExOrder.num = oTrade.Num;
                    oExOrder.tid = oTrade.Tid;
                    oExOrder.alipay_no = oTrade.AlipayNo;
                    oExOrder.payment = oTrade.Payment;
                    oExOrder.discount_fee = oTrade.DiscountFee;
                    oExOrder.adjust_fee = oTrade.AdjustFee;
                    oExOrder.snapshot_url = oTrade.SnapshotUrl;
                    oExOrder.snapshot = oTrade.Snapshot;
                    oExOrder.seller_rate = oTrade.SellerRate;
                    oExOrder.buyer_rate = oTrade.BuyerRate;
                    oExOrder.trade_memo = oTrade.TradeMemo;
                    if (DateTime.TryParseExact(oTrade.PayTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.pay_time = dtDateTime;
                    oExOrder.buyer_obtain_point_fee = oTrade.BuyerObtainPointFee;
                    oExOrder.point_fee = oTrade.PointFee;
                    oExOrder.real_point_fee = oTrade.RealPointFee;
                    oExOrder.total_fee = oTrade.TotalFee;
                    oExOrder.post_fee = oTrade.PostFee;
                    oExOrder.buyer_alipay_no = oTrade.BuyerAlipayNo;
                    oExOrder.receiver_name = oTrade.ReceiverName;
                    oExOrder.receiver_state = oTrade.ReceiverState;
                    oExOrder.receiver_city = oTrade.ReceiverCity;
                    oExOrder.receiver_district = oTrade.ReceiverDistrict;
                    oExOrder.receiver_address = oTrade.ReceiverAddress;
                    oExOrder.receiver_zip = oTrade.ReceiverZip;
                    oExOrder.receiver_mobile = oTrade.ReceiverMobile;
                    oExOrder.receiver_phone = oTrade.ReceiverPhone;
                    if (DateTime.TryParseExact(oTrade.ConsignTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.consign_time = dtDateTime;
                    oExOrder.buyer_email = oTrade.BuyerEmail;
                    oExOrder.commission_fee = oTrade.CommissionFee;
                    oExOrder.seller_alipay_no = oTrade.SellerAlipayNo;
                    oExOrder.seller_mobile = oTrade.SellerMobile;
                    oExOrder.seller_phone = oTrade.SellerPhone;
                    oExOrder.seller_name = oTrade.SellerName;
                    oExOrder.seller_email = oTrade.SellerEmail;
                    oExOrder.available_confirm_fee = oTrade.AvailableConfirmFee;
                    oExOrder.has_post_fee = oTrade.HasPostFee;
                    oExOrder.received_payment = oTrade.ReceivedPayment;
                    oExOrder.cod_fee = oTrade.CodFee;
                    oExOrder.cod_status = oTrade.CodStatus;
                    if (DateTime.TryParseExact(oTrade.TimeoutActionTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                        oExOrder.timeout_action_time = dtDateTime;
                    oExOrder.is_3D = oTrade.Is3D;
                    oExOrder.num_iid = oTrade.NumIid;
                    oExOrder.promotion = oTrade.Promotion;
                    oExOrder.invoice_name = oTrade.InvoiceName;
                    oExOrder.alipay_url = oTrade.AlipayUrl;
                    // 保存订单SKU
                    foreach (Order oOrder in oTrade.Orders)
                    {
                        ExTaobaoOrdItem oExItem = new ExTaobaoOrdItem();
                        oExItem.total_fee = oOrder.TotalFee;
                        oExItem.discount_fee = oOrder.DiscountFee;
                        oExItem.adjust_fee = oOrder.AdjustFee;
                        oExItem.payment = oOrder.Payment;
                        if (DateTime.TryParseExact(oOrder.Modified, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                            oExItem.modified = dtDateTime;
                        oExItem.item_meal_id = oOrder.ItemMealId;
                        oExItem.status = oOrder.Status;
                        oExItem.refund_id = oOrder.RefundId;
                        oExItem.iid = oOrder.Iid;
                        oExItem.sku_id = oOrder.SkuId;
                        oExItem.sku_properties_name = oOrder.SkuPropertiesName;
                        oExItem.item_meal_name = oOrder.ItemMealName;
                        oExItem.num = oOrder.Num;
                        oExItem.title = oOrder.Title;
                        oExItem.price = oOrder.Price;
                        oExItem.pic_path = oOrder.PicPath;
                        oExItem.seller_nick = oOrder.SellerNick;
                        oExItem.buyer_nick = oOrder.BuyerNick;
                        oExItem.refund_status = oOrder.RefundStatus;
                        oExItem.oid = oOrder.Oid;
                        oExItem.outer_iid = oOrder.OuterIid;
                        oExItem.outer_sku_id = oOrder.OuterSkuId;
                        oExItem.snapshot_url = oOrder.SnapshotUrl;
                        oExItem.snapshot = oOrder.Snapshot;
                        if (DateTime.TryParseExact(oOrder.TimeoutActionTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                            oExItem.timeout_action_time = dtDateTime;
                        oExItem.buyer_rate = oOrder.BuyerRate;
                        oExItem.seller_rate = oOrder.SellerRate;
                        oExItem.seller_type = oOrder.SellerType;
                        oExItem.num_iid = oOrder.NumIid;
                        oExItem.cid = oOrder.Cid;
                        oExItem.is_oversold = oOrder.IsOversold;

                        oExOrder.TaobaoOrderItems.Add(oExItem);
                    }
                    #endregion
                    dbEntity.SaveChanges();
                    oEventBLL.WriteEvent(sLogMsg, ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
                }
            }
        }

        /// <summary>
        /// 查询和下载退款列表
        /// </summary>
        public void DownloadRefunds()
        {
            try
            {
                RefundsReceiveGetRequest req = new RefundsReceiveGetRequest();
                req.Fields = "refund_id, tid, title, buyer_nick, seller_nick, total_fee, status, created, refund_fee";
                req.Type = "fixed";
                req.StartModified = DateTime.Now.AddDays(-1);
                req.EndModified = DateTime.Now;
                req.PageNo = 1L;
                req.PageSize = 50L;
                do
                {
                    RefundsReceiveGetResponse response = oTopClient.Execute(req, strAppSession);
                    if (response.TotalResults > req.PageNo * req.PageSize)
                        req.PageNo++;
                    else
                        req.PageNo = 0;
                    foreach (Refund oRefund in response.Refunds)
                    {
                        ExTaobaoOrder oExOrder = (from t in dbEntity.ExTaobaoOrders
                                                  where t.tid == oRefund.Tid
                                                  select t).FirstOrDefault();
                        if (oExOrder != null)
                        {
                            ExTaobaoRefund oExRefund = (from r in dbEntity.ExTaobaoRefunds
                                                        where r.tid == oRefund.Tid
                                                        select r).FirstOrDefault();
                            if (oExRefund == null)
                            {
                                oExRefund = new ExTaobaoRefund();
                                #region 退款单中间表
                                DateTime dtDateTime;
                                oExRefund.TaobaoOrder = oExOrder;
                                oExRefund.shipping_type = oRefund.ShippingType;
                                oExRefund.refund_id = oRefund.RefundId;
                                oExRefund.tid = oRefund.Tid;
                                oExRefund.oid = oRefund.Oid;
                                oExRefund.alipay_no = oRefund.AlipayNo;
                                oExRefund.total_fee = oRefund.TotalFee;
                                oExRefund.buyer_nick = oRefund.BuyerNick;
                                oExRefund.seller_nick = oRefund.SellerNick;
                                if (DateTime.TryParseExact(oRefund.Created, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                                    oExRefund.created = dtDateTime;
                                if (DateTime.TryParseExact(oRefund.Modified, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                                    oExRefund.modified = dtDateTime;
                                oExRefund.order_status = oRefund.OrderStatus;
                                oExRefund.status = oRefund.Status;
                                oExRefund.good_status = oRefund.GoodStatus;
                                oExRefund.has_good_return = oRefund.HasGoodReturn;
                                oExRefund.refund_fee = oRefund.RefundFee;
                                oExRefund.payment = oRefund.Payment;
                                oExRefund.reason = oRefund.Reason;
                                oExRefund.desc = oRefund.Desc;
                                oExRefund.iid = oRefund.Iid;
                                oExRefund.title = oRefund.Title;
                                oExRefund.price = oRefund.Price;
                                oExRefund.num = oRefund.Num;
                                if (DateTime.TryParseExact(oRefund.GoodReturnTime, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                                    oExRefund.good_return_time = dtDateTime;
                                oExRefund.company_name = oRefund.CompanyName;
                                oExRefund.sid = oRefund.Sid;
                                oExRefund.address = oRefund.Address;
                                oExRefund.num_iid = oRefund.NumIid;

                                ExTaobaoRefundRemind oExRefundRemind = new ExTaobaoRefundRemind();
                                oExRefundRemind.remind_type = oRefund.RefundRemindTimeout.RemindType;
                                oExRefundRemind.exist_timeout = oRefund.RefundRemindTimeout.ExistTimeout;
                                if (DateTime.TryParseExact(oRefund.RefundRemindTimeout.Timeout, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.AssumeLocal, out dtDateTime))
                                    oExRefundRemind.timeout = dtDateTime;
                                oExRefund.TaobaoRefundReminds.Add(oExRefundRemind);
                                #endregion
                                dbEntity.ExTaobaoRefunds.Add(oExRefund);
                                dbEntity.SaveChanges();
                                oEventBLL.WriteEvent(String.Format("下载淘宝退款列表: {0}", oExOrder.tid), ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
                            }
                        }
                        else
                        {
                            oEventBLL.WriteEvent(String.Format("下载淘宝退款单失败: 淘宝订单没有正确下载 {0}", oRefund.Tid),
                                ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
                        }
                    }
                } while (req.PageNo > 0L);
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("下载淘宝退款单失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 关闭一个订单
        /// </summary>
        /// <param name="tid">淘宝交易ID</param>
        /// <param name="reason">关闭原因</param>
        public void CloseOrder(long tid, string reason)
        {
            try
            {
                TradeCloseRequest req = new TradeCloseRequest();
                req.Tid = tid;
                req.CloseReason = reason;
                TradeCloseResponse response = oTopClient.Execute(req, strAppSession);
                oEventBLL.WriteEvent(String.Format("淘宝订单关闭: {0}, {1}, {2}", response.Trade.Tid, response.Trade.Modified, reason),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝订单关闭失败: {0}, {1}", tid, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 延长交易收货时间
        /// </summary>
        /// <param name="tid">淘宝ID</param>
        /// <param name="days">延长交易收货天数</param>
        public void ReceiveTimeDelay(long tid, long days)
        {
            try
            {
                TradeReceivetimeDelayRequest req = new TradeReceivetimeDelayRequest();
                req.Tid = tid;
                req.Days = days;
                TradeReceivetimeDelayResponse response = oTopClient.Execute(req, strAppSession);
                oEventBLL.WriteEvent(String.Format("淘宝订单延长交易时间: {0}, {1}, {2}天", response.Trade.Tid, response.Trade.Modified, days),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝订单延长交易时间失败: {0}, {1}", tid, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 添加和修改订单备注
        /// </summary>
        /// <param name="tid">淘宝ID</param>
        /// <param name="memo">备注内容，最长1000字节</param>
        /// <param name="flag">交易备注旗帜，可选值为：0(灰色), 1(红色), 2(黄色), 3(绿色), 4(蓝色), 5(粉红色)，默认值为0</param>
        public void MemoUpdate(long tid, string memo, long flag = 0)
        {
            try
            {
                TradeMemoUpdateRequest req = new TradeMemoUpdateRequest();
                req.Tid = tid;
                req.Memo = memo;
                req.Flag = flag;
                TradeMemoUpdateResponse response = oTopClient.Execute(req, strAppSession);
                oEventBLL.WriteEvent(String.Format("淘宝订单修改备注: {0}, {1}, {2}", response.Trade.Tid, response.Trade.Modified, memo),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝订单修改备注失败: {0}, {1}", tid, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }

        /// <summary>
        /// 更改交易的收货地址
        /// </summary>
        /// <param name="tid">淘宝ID</param>
        /// <param name="receiver_name">收货人全名。最大长度为50个字节</param>
        /// <param name="receiver_phone">固定电话。最大长度为30个字节</param>
        /// <param name="receiver_mobile">移动电话。最大长度为30个字节</param>
        /// <param name="receiver_state">省份。最大长度为32个字节</param>
        /// <param name="receiver_city">城市。最大长度为32个字节</param>
        /// <param name="receiver_district">区/县。最大长度为32个字节</param>
        /// <param name="receiver_address">收货地址。最大长度为228个字节</param>
        /// <param name="receiver_zip">邮政编码。必须由6个数字组成</param>
        public void ShippingAddressUpdate(long tid, string receiver_name, string receiver_phone, string receiver_mobile,
            string receiver_state, string receiver_city, string receiver_district, string receiver_address, string receiver_zip)
        {
            try
            {
                TradeShippingaddressUpdateRequest req = new TradeShippingaddressUpdateRequest();
                req.Tid = tid;
                req.ReceiverName = receiver_name;
                req.ReceiverPhone = receiver_phone;
                req.ReceiverMobile = receiver_mobile;
                req.ReceiverState = receiver_state;
                req.ReceiverCity = receiver_city;
                req.ReceiverDistrict = receiver_district;
                req.ReceiverAddress = receiver_address;
                req.ReceiverZip = receiver_zip;
                TradeShippingaddressUpdateResponse response = oTopClient.Execute(req, strAppSession);
                oEventBLL.WriteEvent(String.Format("淘宝更改订单的收货地址: {0}, {1}, {2}", response.Trade.Tid, response.Trade.Modified, receiver_name),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.ORDER, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝更改订单的收货地址失败: {0}, {1}", tid, ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.ORDER, this.ToString());
            }
        }
    }

    /// <summary>
    /// 淘宝-评价API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:6"/>
    public class TradeRateAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public TradeRateAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

    }

    /// <summary>
    /// 淘宝-物流API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:7"/>
    public class LogisticsAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public LogisticsAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        /// <summary>
        /// 获取淘宝物流公司表
        /// </summary>
        public void GetLogisticsCompanies()
        {
            try
            {
                LogisticsCompaniesGetRequest req = new LogisticsCompaniesGetRequest();
                req.Fields = "id, code, name, reg_mail_no";
                LogisticsCompaniesGetResponse response = oTopClient.Execute(req, strAppSession);
                foreach (LogisticsCompany oCompany in response.LogisticsCompanies)
                {
                    ExTaobaoLogisticsCompany oExCompany = (from c in dbEntity.ExTaobaoLogisticsCompanies
                                                           where c.id == oCompany.Id
                                                           select c).FirstOrDefault();
                    if (oExCompany == null)
                    {
                        oExCompany = new ExTaobaoLogisticsCompany { id = oCompany.Id };
                        dbEntity.ExTaobaoLogisticsCompanies.Add(oExCompany);
                    }
                    oExCompany.code = oCompany.Code;
                    oExCompany.name = oCompany.Name;
                    oExCompany.reg_mail_no = oCompany.RegMailNo;    // 运单号正则表达式
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), oExCompany.id, oExCompany.name);
                }
                oEventBLL.WriteEvent(String.Format("淘宝获取物流公司表"),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.EXCHANGE, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝获取物流公司表失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.EXCHANGE, this.ToString());
            }
        }

        /// <summary>
        /// 获取淘宝地区表，并与Stage系统地区表关联
        /// </summary>
        public void GetAreas()
        {
            try
            {
                AreasGetRequest req = new AreasGetRequest();
                req.Fields = "id, type, name, parent_id, zip";
                AreasGetResponse response = oTopClient.Execute(req, strAppSession);
                foreach (Area oArea in response.Areas)
                {
                    ExTaobaoArea oExArea = (from a in dbEntity.ExTaobaoAreas
                                            where a.id == oArea.Id
                                            select a).FirstOrDefault();
                    string sRegionCode = oArea.Id.ToString();
                    if (oArea.Id == 1) sRegionCode = "CHN";             // 中国
                    Guid? oRegionID = (from r in dbEntity.GeneralRegions
                                       where r.Code == sRegionCode
                                       select r.Gid).FirstOrDefault();
                    if (oExArea == null)
                    {
                        oExArea = new ExTaobaoArea();
                        dbEntity.ExTaobaoAreas.Add(oExArea);
                    }
                    if ((oRegionID != null) && (oRegionID != Guid.Empty))
                        oExArea.RegionID = oRegionID;
                    oExArea.id = oArea.Id;
                    oExArea.type = oArea.Type;
                    oExArea.name = oArea.Name;
                    oExArea.parent_id = oArea.ParentId;
                    oExArea.zip = oArea.Zip;
                    dbEntity.SaveChanges();
                    if (Utility.ConfigHelper.GlobalConst.IsDebug)
                        Debug.WriteLine("{0} {1} {2}", this.ToString(), oExArea.id, oExArea.name);
                }
                oEventBLL.WriteEvent(String.Format("淘宝获取并关联地区表"),
                    ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.EXCHANGE, this.ToString());
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝获取并关联地区表失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.EXCHANGE, this.ToString());
            }
        }

        /// <summary>
        /// 在线订单发货，未经测试
        /// </summary>
        public void OnlineSend()
        {
            bool bIsError = false;
            try
            {
                var oPendings = from d in dbEntity.ExTaobaoDeliveryPendings.Include("Order.Location")
                                where d.Deleted == false && d.Dstatus == (byte)ModelEnum.TaobaoDeliveryStatus.WAIT_FOR_SEND
                                select d;
                foreach (ExTaobaoDeliveryPending item in oPendings)
                {
                    string[] tids = item.tid.Split(',');  // 合并订单，每个tid都需要单独发货
                    foreach (string tid in tids)
                    {
                        long ntid = 0;
                        Int64.TryParse(tid, out ntid);
                        TradeGetRequest req1 = new TradeGetRequest();
                        req1.Fields = "tid, status";
                        req1.Tid = ntid;
                        TradeGetResponse res1 = oTopClient.Execute(req1, strAppSession);
                        if (res1.Trade.Status == "WAIT_SELLER_SEND_GOODS")
                        {
                            // 测试承运范围
                            //LogisticsPartnersGetRequest oPartners = new LogisticsPartnersGetRequest();
                            //oPartners.ServiceType = "online";
                            //oPartners.SourceId = "310114";  // 上海嘉定
                            //oPartners.TargetId = item.Order.Location.Code;
                            //LogisticsPartnersGetResponse res2 = oTopClient.Execute(oPartners, strAppSession);
                            //foreach (LogisticsPartner oPartner in res2.LogisticsPartners)
                            //{
                            //    Debug.WriteLine("{0} {1} {2} {3}", oPartner.Partner.CompanyCode, oPartner.Partner.CompanyName,
                            //        oPartner.CoverRemark, oPartner.UncoverRemark);
                            //}
                            LogisticsOnlineSendRequest req = new LogisticsOnlineSendRequest();
                            req.Tid = ntid;
                            req.OutSid = item.out_sid;
                            req.CompanyCode = item.logistics;
                            // 发货动作
                            LogisticsOnlineSendResponse response = oTopClient.Execute(req, strAppSession);
                            if (response.IsError)
                            {
                                item.err_msg = response.ErrCode + "|" + response.ErrMsg;
                                bIsError = true;
                            }
                            else
                            {
                                item.Dstatus = (byte)ModelEnum.TaobaoDeliveryStatus.ALREADY_SENT;
                            }
                        }
                        else
                        {
                            bIsError = true;
                            item.err_msg = "淘宝状态不一致：" + res1.Trade.Status;
                            item.Dstatus = (byte)ModelEnum.TaobaoDeliveryStatus.NO_NEED_SEED;
                        }
                    }
                    oEventBLL.WriteEvent(String.Format("淘宝发货 {0} {1} {2}", item.Order.Code, item.tid, item.err_msg),
                        ModelEnum.ActionLevel.GENERIC, ModelEnum.ActionSource.EXCHANGE, this.ToString());
                }
                dbEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                oEventBLL.WriteEvent(String.Format("淘宝发货失败: {0}", ex.Message),
                    ModelEnum.ActionLevel.ERROR, ModelEnum.ActionSource.EXCHANGE, this.ToString());
            }
            if (bIsError)
            {
                GeneralTodoList oTodo = new GeneralTodoList
                {
                    Ttype = (byte)ModelEnum.TodoType.TAOBAO_DELIVERY_ERROR,
                    Tstatus = 0,
                    Title = "淘宝发货失败",
                    Keyword = "淘宝,发货"
                };
                dbEntity.GeneralTodoLists.Add(oTodo);
                dbEntity.SaveChanges();
            }
        }
    }

    /// <summary>
    /// 淘宝-旺旺API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:18"/>
    public class WangwangAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public WangwangAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        public void WangwangEserviceChatlogGet() 
        {
            WangwangEserviceChatlogGetRequest req = new WangwangEserviceChatlogGetRequest();
            req.FromId = "cntaobao筑巢家居专营店:小乔";
            req.ToId = "cntaobaorain19851201";
            req.StartDate = "2011-08-10";
            req.EndDate = "2011-08-11";
            WangwangEserviceChatlogGetResponse response = oTopClient.Execute(req, strAppSession);
            List<Msg> result = response.Msgs;
            foreach (Msg newMsg in result){
                Debug.WriteLine("Content {0} {1}", newMsg.Content, newMsg.Time);
            }
        }

        public void WangwangEserviceChatpeersGet()
        {
            WangwangEserviceChatpeersGetRequest req = new WangwangEserviceChatpeersGetRequest();
            req.ChatId = "cntaobao筑巢家居专营店:小乔";
            req.StartDate = "2011-08-10";
            req.EndDate = "2011-08-11";
            //req.Charset = "utf-8"; 可选项
            WangwangEserviceChatpeersGetResponse response = oTopClient.Execute(req, strAppSession);
            List<Chatpeer> result = response.Chatpeers;
            foreach (Chatpeer newChatpeer in result) 
            {
                Debug.WriteLine("Chatpeer: {0} {1}", newChatpeer.Uid, newChatpeer.Date);
            }
        }

    }

    /// <summary>
    /// 淘宝-系统API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:40"/>
    public class SystemAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public SystemAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

        public DateTimeOffset TimeGet()
        {
            TimeGetRequest req = new TimeGetRequest();
            TimeGetResponse response = oTopClient.Execute(req, strAppSession);
            DateTimeOffset result = DateTimeOffset.Now;
            DateTimeOffset.TryParseExact(response.Time, "yyyy-MM-dd HH:mm:ss", new CultureInfo("zh-CN"), DateTimeStyles.None, out result);
            return result;
        }
    }

    /// <summary>
    /// 淘宝-会员管理API-封装
    /// </summary>
    /// <see cref="http://my.open.taobao.com/apidoc/index.htm#categoryId:10102"/>
    public class CrmAPI : BaseAPI
    {
        /// <summary>
        /// 构造函数，必须提供淘宝接口系统级参数
        /// </summary>
        /// <param name="channel">操作的组织和渠道</param>
        public CrmAPI(LiveEntities entity, MemberOrgChannel channel) : base(entity, channel) { }

    }
    
}

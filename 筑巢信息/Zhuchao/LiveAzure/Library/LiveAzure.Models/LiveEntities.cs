using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using LiveAzure.Utility;
using LiveAzure.Models.General;
using LiveAzure.Models.Mall;
using LiveAzure.Models.Finance;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using LiveAzure.Models.Purchase;
using LiveAzure.Models.Warehouse;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.Order;
using LiveAzure.Models.Union;
using LiveAzure.Models.Exchange;

namespace LiveAzure.Models
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-01         -->
    /// <summary>
    /// 数据访问层
    /// </summary>
    public class LiveEntities : DbContext
    {
        /// <summary>
        /// 构造函数，直接使用数据库连接
        /// </summary>
        /// <param name="oConnection">数据库连接对象</param>
        public LiveEntities(DbConnection oConnection)
            : base(oConnection, true)
        {
            this.Configuration.LazyLoadingEnabled = true;
            // this.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// 使用单数表名
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // modelBuilder.Entity<GeneralConfig>().Property(x => x.DecValue).HasColumnType("decimal").HasPrecision(18, 4);
        }

        #region General 模块表定义

        /// <summary>
        /// 动态配置参数
        /// </summary>
        public DbSet<GeneralConfig> GeneralConfigs { get; set; }

        /// <summary>
        /// 多语言资源文件（主语言），超过512字符用大对象
        /// </summary>
        public DbSet<GeneralResource> GeneralResources { get; set; }

        /// <summary>
        /// 多语言资源文件（其他语言）
        /// </summary>
        public DbSet<GeneralResItem> GeneralResItems { get; set; }

        /// <summary>
        /// 多语言大对象资源文件（主语言）
        /// </summary>
        public DbSet<GeneralLargeObject> GeneralLargeObjects { get; set; }

        /// <summary>
        /// 多语言大对象资源文件（其他语言）
        /// </summary>
        public DbSet<GeneralLargeItem> GeneralLargeItems { get; set; }

        /// <summary>
        /// 通用属性表 - 仅所属组织可见，组织自定义
        /// </summary>
        public DbSet<GeneralOptional> GeneralOptionals{get;set;}

        /// <summary>
        /// 属性选项内容，即下拉框的内容
        /// </summary>
        public DbSet<GeneralOptItem> GeneralOptItems { get; set; }

        /// <summary>
        /// 标准分类表 - 全局可见，类似淘宝，符合全行业，简洁定义
        /// </summary>
        public DbSet<GeneralStandardCategory> GeneralStandardCategorys { get; set; }

        /// <summary>
        /// 私有分类表 - 仅所属组织可见，组织自定义
        /// </summary>
        public DbSet<GeneralPrivateCategory> GeneralPrivateCategorys { get; set; }

        /// <summary>
        /// 国家地区，按默认语言
        /// </summary>
        public DbSet<GeneralRegion> GeneralRegions { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public DbSet<GeneralMeasureUnit> GeneralMeasureUnits { get; set; }

        /// <summary>
        /// 默认计量单位
        /// </summary>
        public DbSet<GeneralCultureUnit> GeneralCultureUnits { get; set; }

        /// <summary>
        /// 消息模板
        /// </summary>
        public DbSet<GeneralMessageTemplate> GeneralMessageTemplates { get; set; }

        /// <summary>
        /// 消息队列
        /// </summary>
        public DbSet<GeneralMessagePending> GeneralMessagePendings { get; set; }

        /// <summary>
        /// 收到短信
        /// </summary>
        public DbSet<GeneralMessageReceive> GeneralMessageReceives { get; set; }

        /// <summary>
        /// IP地址库，仅适用于中国环境
        /// </summary>
        public DbSet<GeneralIpBase> GeneralIpBases { get; set; }

        /// <summary>
        /// 程序定义
        /// </summary>
        public DbSet<GeneralProgram> GeneralPrograms { get; set; }

        /// <summary>
        /// 程序功能定义 指程序中的特殊按钮和操作限制等
        /// </summary>
        public DbSet<GeneralProgNode> GeneralProgNodes { get; set; }

        /// <summary>
        /// 权限模板，可与用户授权互相复制，参考用户授权说明
        /// </summary>
        public DbSet<GeneralPrivTemplate> GeneralPrivTemplates { get; set; }

        /// <summary>
        /// 权限控制明细，主要针对商品类别，供应商类别等
        /// </summary>
        public DbSet<GeneralPrivItem> GeneralPrivItems { get; set; }

        /// <summary>
        /// 操作日志
        /// </summary>
        public DbSet<GeneralAction> GeneralActions { get; set; }

        /// <summary>
        /// 错误报告提交与处理
        /// </summary>
        public DbSet<GeneralErrorReport> GeneralErrorReports { get; set; }

        /// <summary>
        /// 待办事项和后天程序紧急事件
        /// </summary>
        public DbSet<GeneralTodoList> GeneralTodoLists { get; set; }

        #endregion

        #region Member 模块表定义

        /// <summary>
        /// 组织（公司）
        /// </summary>
        public DbSet<MemberOrganization> MemberOrganizations { get; set; }

        /// <summary>
        /// 支持的渠道
        /// </summary>
        public DbSet<MemberChannel> MemberChannels { get; set; }

        /// <summary>
        /// 组织支持的渠道
        /// </summary>
        public DbSet<MemberOrgChannel> MemberOrgChannels { get; set; }

        /// <summary>
        /// 组织支持的语言文化
        /// </summary>
        public DbSet<MemberOrgCulture> MemberOrgCultures { get; set; }

        /// <summary>
        /// 组织自定义属性
        /// </summary>
        public DbSet<MemberOrgAttribute> MemberOrgAttributes { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<MemberRole> MemberRoles { get; set; }

        /// <summary>
        /// 用户，包括组织用户和个人用户
        /// </summary>
        public DbSet<MemberUser> MemberUsers { get; set; }

        /// <summary>
        /// 注册地址，配送地址
        /// </summary>
        public DbSet<MemberAddress> MemberAddresses { get; set; }

        /// <summary>
        /// 用户自定义属性
        /// </summary>
        public DbSet<MemberUserAttribute> MemberUserAttributes { get; set; }

        /// <summary>
        /// 会员积分，积分方案待定
        /// </summary>
        public DbSet<MemberPoint> MemberPoints { get; set; }

        /// <summary>
        /// 会员积分消费记录
        /// </summary>
        public DbSet<MemberUsePoint> MemberUsePoints { get; set; }

        /// <summary>
        /// 会员等级，升级策略待定
        /// </summary>
        public DbSet<MemberLevel> MemberLevels { get; set; }

        /// <summary>
        /// 订阅
        /// </summary>
        public DbSet<MemberSubscribe> MemberSubscribes { get; set; }

        /// <summary>
        /// 用户事件，例如打电话联系，发DM时间等信息
        /// </summary>
        public DbSet<MemberUserEvent> MemberUserEvents { get; set; }

        /// <summary>
        /// 用户快捷按钮
        /// </summary>
        public DbSet<MemberUserShortcut> MemberUserShortcuts { get; set; }

        /// <summary>
        /// 权限控制，按组织，渠道，程序，程序功能
        /// </summary>
        public DbSet<MemberPrivilege> MemberPrivileges { get; set; }

        /// <summary>
        /// 权限控制明细，主要针对商品类别，供应商类别等
        /// </summary>
        public DbSet<MemberPrivItem> MemberPrivItems { get; set; }

        #endregion

        #region Product模块定义

        /// <summary>
        /// 产品基础信息 - PU
        /// </summary>
        public DbSet<ProductInformation> ProductInformations { get; set; }

        /// <summary>
        /// 产品基础信息 - SKU
        /// </summary>
        public DbSet<ProductInfoItem> ProductInfoItems { get; set; }

        /// <summary>
        /// 扩展分类
        /// </summary>
        public DbSet<ProductExtendCategory> ProductExtendCategories { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public DbSet<ProductExtendAttribute> ProductExtendAttributes { get; set; }

        /// <summary>
        /// 商品图片，按SKU
        /// </summary>
        public DbSet<ProductGallery> ProductGalleries { get; set; }

        /// <summary>
        /// 商品上架 - PU
        /// </summary>
        public DbSet<ProductOnSale> ProductOnSales { get; set; }

        /// <summary>
        /// 商品上架明细 - SKU
        /// </summary>
        public DbSet<ProductOnItem> ProductOnItems { get; set; }

        /// <summary>
        /// 商品上架计量单位 - SKU
        /// </summary>
        public DbSet<ProductOnUnitPrice> ProductOnUnitPrices { get; set; }

        /// <summary>
        /// 按用户级别的折扣体系，按上架PU
        /// </summary>
        public DbSet<ProductOnLevelDiscount> ProductOnLevelDiscounts { get; set; }

        /// <summary>
        /// 商品上架模板
        /// </summary>
        public DbSet<ProductOnTemplate> ProductOnTemplates { get; set; }

        /// <summary>
        /// 上架商品支持的运输公司，运费策略等
        /// </summary>
        public DbSet<ProductOnShipping> ProductOnShippings { get; set; }

        /// <summary>
        /// 上架商品承运区域
        /// </summary>
        public DbSet<ProductOnShipArea> ProductOnShipAreas { get; set; }

        /// <summary>
        /// 上架商品支持的付款方式
        /// </summary>
        public DbSet<ProductOnPayment> ProductOnPayments { get; set; }

        /// <summary>
        /// 关联商品
        /// </summary>
        public DbSet<ProductOnRelation> ProductOnRelations { get; set; }

        /// <summary>
        /// 商品调价定时器，按SKU调整
        /// </summary>
        public DbSet<ProductOnAdjust> ProductOnAdjusts { get; set; }

        #endregion

        #region Purchase模块定义

        /// <summary>
        /// 供应商
        /// </summary>
        public DbSet<PurchaseSupplier> PurchaseSuppliers { get; set; }

        /// <summary>
        /// 采购单
        /// </summary>
        public DbSet<PurchaseInformation> PurchaseInformations { get; set; }

        /// <summary>
        /// 采购明细单
        /// </summary>
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        /// <summary>
        /// 采购单变更记录
        /// </summary>
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

        /// <summary>
        /// 采购单变更记录明细
        /// </summary>
        public DbSet<PurchaseHisItem> PurchaseHisItems { get; set; }

        /// <summary>
        /// 质检表
        /// </summary>
        public DbSet<PurchaseInspection> PurchaseInspections { get; set; }

        /// <summary>
        /// 质检明细表
        /// </summary>
        public DbSet<PurchaseInspItem> PurchaseInspItems { get; set; }

        #endregion

        #region Warehouse模块定义

        /// <summary>
        /// 仓库表
        /// </summary>
        public DbSet<WarehouseInformation> WarehouseInformations { get; set; }

        /// <summary>
        /// 库位（货架）定义表
        /// </summary>
        public DbSet<WarehouseShelf> WarehouseShelves { get; set; }

        /// <summary>
        /// 仓库支持的配送区域
        /// </summary>
        public DbSet<WarehouseRegion> WarehouseRegions { get; set; }

        /// <summary>
        /// 仓库支持的运输公司
        /// </summary>
        public DbSet<WarehouseShipping> WarehouseShippings { get; set; }

        /// <summary>
        /// 库存总账 按SKU统计值 = 入库 - 出库
        /// </summary>
        public DbSet<WarehouseLedger> WarehouseLedgers { get; set; }

        /// <summary>
        /// 库位/货位对照表
        /// </summary>
        public DbSet<WarehouseSkuShelf> WarehouseSkuShelves { get; set; }

        /// <summary>
        /// 入库单
        /// </summary>
        public DbSet<WarehouseStockIn> WarehouseStockIns { get; set; }

        /// <summary>
        /// 入库单明细
        /// </summary>
        public DbSet<WarehouseInItem> WarehouseInItems { get; set; }

        /// <summary>
        /// 出库记录 扫描出库，报废出库等
        /// </summary>
        public DbSet<WarehouseStockOut> WarehouseStockOuts { get; set; }

        /// <summary>
        /// 出库记录明细
        /// </summary>
        public DbSet<WarehouseOutItem> WarehouseOutItems { get; set; }

        /// <summary>
        /// 出库扫描记录
        /// </summary>
        public DbSet<WarehouseOutScan> WarehouseOutScans { get; set; }

        /// <summary>
        /// 出库运输记录
        /// </summary>
        public DbSet<WarehouseOutDelivery> WarehouseOutDeliveries { get; set; }

        /// <summary>
        /// 移库单/移货位单
        /// </summary>
        public DbSet<WarehouseMoving> WarehouseMovings { get; set; }

        /// <summary>
        /// 移库单/移货位单明细表
        /// </summary>
        public DbSet<WarehouseMoveItem> WarehouseMoveItems { get; set; }

        /// <summary>
        /// 盘点记录
        /// </summary>
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }

        /// <summary>
        /// 盘点单明细
        /// </summary>
        public DbSet<WarehouseInvItem> WarehouseInvItems { get; set; }

        #endregion

        #region Order 模块定义

        /// <summary>
        /// 订单表
        /// </summary>
        public DbSet<OrderInformation> OrderInformations { get; set; }

        /// <summary>
        /// 订单商品表
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// 配送方式，按权重审单
        /// </summary>
        public DbSet<OrderShipping> OrderShippings { get; set; }

        /// <summary>
        /// 订单自定义属性
        /// </summary>
        public DbSet<OrderAttribute> OrderAttributes { get; set; }

        /// <summary>
        /// 订单处理日志
        /// </summary>
        public DbSet<OrderProcess> OrderProcesses { get; set; }

        /// <summary>
        /// 拆单策略
        /// </summary>
        public DbSet<OrderSplitPolicy> OrderSplitPolicies { get; set; }

        /// <summary>
        /// 订单变更表
        /// </summary>
        public DbSet<OrderHistory> OrderHistories { get; set; }

        /// <summary>
        /// 订单变更商品表
        /// </summary>
        public DbSet<OrderHisItem> OrderHisItems { get; set; }

        /// <summary>
        /// 促销方案
        /// </summary>
        public DbSet<PromotionInformation> PromotionInformations { get; set; }

        /// <summary>
        /// 促销互斥关系
        /// </summary>
        public DbSet<PromotionMutex> PromotionMutexes { get; set; }

        /// <summary>
        /// 促销商品
        /// </summary>
        public DbSet<PromotionProduct> PromotionProducts { get; set; }

        /// <summary>
        /// 券，包括抵用券和现金券
        /// </summary>
        public DbSet<PromotionCoupon> PromotionCoupons { get; set; }

        #endregion

        #region Shipping 模块定义

        /// <summary>
        /// 运输公司
        /// </summary>
        public DbSet<ShippingInformation> ShippingInformations { get; set; }

        /// <summary>
        /// 运输公司支持的区域和标准费率定义
        /// </summary>
        public DbSet<ShippingArea> ShippingAreas { get; set; }

        /// <summary>
        /// 运输公司面单，运输一个公司使用多个版本的面单
        /// </summary>
        public DbSet<ShippingEnvelope> ShippingEnvelopes { get; set; }

        #endregion

        #region Mall 模块表定义

        /// <summary>
        /// 商城文章/广告位置
        /// </summary>
        public DbSet<MallArtPosition> MallArtPositions { get; set; }

        /// <summary>
        /// 商城文章
        /// </summary>
        public DbSet<MallArticle> MallArticles { get; set; }

        /// <summary>
        /// 文章发布渠道
        /// </summary>
        public DbSet<MallArtPublish> MallArtPublishes { get; set; }

        /// <summary>
        /// 商城热门关键字
        /// </summary>
        public DbSet<MallHotWord> MallHotWords { get; set; }

        /// <summary>
        /// 热门关键字明细，每次搜索都记录，并汇总到MallHotWord
        /// </summary>
        public DbSet<MallHotItem> MallHotItems { get; set; }

        /// <summary>
        /// 商城商品收藏
        /// </summary>
        public DbSet<MallFavorite> MallFavorites { get; set; }

        /// <summary>
        /// 商城商品关注，缺货通知
        /// </summary>
        public DbSet<MallFocusProduct> MallFocusProducts { get; set; }

        /// <summary>
        /// 友情链接
        /// </summary>
        public DbSet<MallFriendLink> MallFriendLinks { get; set; }

        /// <summary>
        /// 商城敏感词，或正则表达式
        /// </summary>
        public DbSet<MallSensitiveWord> MallSensitiveWords { get; set; }

        /// <summary>
        /// 禁用IP地址
        /// </summary>
        public DbSet<MallDisabledIp> MallDisabledIps { get; set; }

        /// <summary>
        /// 商城页面访问，简单记录
        /// </summary>
        public DbSet<MallVisitClick> MallVisitClicks { get; set; }

        /// <summary>
        /// 商城购物车
        /// </summary>
        public DbSet<MallCart> MallCarts { get; set; }

        #endregion

        #region Union 模块定义

        /// <summary>
        /// 向上层分成比例
        /// </summary>
        public DbSet<UnionLevelPercent> UnionLevelPercents { get; set; }

        /// <summary>
        /// 其它分成比例
        /// </summary>
        /// <remarks>废弃，使用Public角色代替</remarks>
        [Obsolete]
        public DbSet<UnionFixedPercent> UnionFixedPercents { get; set; }

        /// <summary>
        /// 广告内容和代码
        /// </summary>
        public DbSet<UnionAdvertising> UnionAdvertisings { get; set; }

        #endregion

        #region Finance模块定义

        /// <summary>
        /// 付款方式
        /// </summary>
        public DbSet<FinancePayType> FinancePayTypes { get; set; }

        /// <summary>
        /// 发票信息
        /// </summary>
        public DbSet<FinanceInvoice> FinanceInvoices { get; set; }

        /// <summary>
        /// 应付账
        /// </summary>
        public DbSet<FinancePayment> FinancePayments { get; set; }

        #endregion

        #region Exchange模块定义

        /// <summary>
        /// 淘宝订单
        /// </summary>
        public DbSet<ExTaobaoOrder> ExTaobaoOrders { get; set; }

        /// <summary>
        /// 淘宝订单SKU
        /// </summary>
        public DbSet<ExTaobaoOrdItem> ExTaobaoOrdItems { get; set; }

        /// <summary>
        /// 淘宝订单促销
        /// </summary>
        public DbSet<ExTaobaoPromotion> ExTaobaoPromotions { get; set; }

        /// <summary>
        /// 淘宝订单评价
        /// </summary>
        public DbSet<ExTaobaoTradeRate> ExTaobaoTradeRates { get; set; }

        /// <summary>
        /// 淘宝订单退款
        /// </summary>
        public DbSet<ExTaobaoRefund> ExTaobaoRefunds { get; set; }

        /// <summary>
        /// 淘宝订单退款超时
        /// </summary>
        public DbSet<ExTaobaoRefundRemind> ExTaobaoRefundReminds { get; set; }

        /// <summary>
        /// 淘宝用户
        /// </summary>
        public DbSet<ExTaobaoUser> ExTaobaoUsers { get; set; }

        /// <summary>
        /// 淘宝用户等级
        /// </summary>
        public DbSet<ExTaobaoUserCredit> ExTaobaoUserCredits { get; set; }

        /// <summary>
        /// 淘宝用户地址
        /// </summary>
        public DbSet<ExTaobaoLocation> ExTaobaoLocations { get; set; }

        /// <summary>
        /// 淘宝用户所属组织/渠道
        /// </summary>
        public DbSet<ExTaobaoUserOrgan> ExTaobaoUserOrgans { get; set; }

        /// <summary>
        /// 淘宝地区
        /// </summary>
        public DbSet<ExTaobaoArea> ExTaobaoAreas { get; set; }

        /// <summary>
        /// 淘宝物流公司
        /// </summary>
        public DbSet<ExTaobaoLogisticsCompany> ExTaobaoLogisticsCompanies { get; set; }

        /// <summary>
        /// 淘宝发货队列
        /// </summary>
        public DbSet<ExTaobaoDeliveryPending> ExTaobaoDeliveryPendings { get; set; }

        #endregion

    }

    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-25         -->
    /// <summary>
    /// 初始化数据库，创建数据库时能且仅能执行一次
    /// </summary>
    public static class InitialiseDatabase
    {
        /// <summary>
        /// 只能执行一次
        /// </summary>
        [Obsolete]
        public static void RunOnce()
        {
            try
            {
                // 创建数据库事务
                using (var scope = new TransactionScope())
                {
                    // 创建EF实体
                    using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
                    {
                        #region GeneralConfig配置参数
                        GeneralConfig oConfig_System = new GeneralConfig
                        {
                            // 系统配置参数
                            Code = "SYSTEM",
                            StrValue = "System",
                            Remark = "一级菜单(系统配置参数)",
                            ChildItems = new List<GeneralConfig>
                            {
                                // 系统启用日期
                                new GeneralConfig
                                {
                                    Code = "StartDate",
                                    Ctype = (byte)ModelEnum.ConfigParamType.DATETIME,
                                    DateValue = DateTimeOffset.Now,
                                    Remark = "二级菜单(系统启用日期)"
                                },
                                // 国家统计局最新行政区划
                                new GeneralConfig
                                {
                                    Code = "LastRegionDefine",
                                    Ctype = (byte)ModelEnum.ConfigParamType.DATETIME,
                                    DateValue = new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)),
                                    Remark = "最新县及县以上行政区划代码（截止2010年12月31日）；http://www.stats.gov.cn/tjbz/xzqhdm/t20110726_402742468.htm"
                                },
                                // Session 名称
                                new GeneralConfig
                                {
                                    Code = "SessionName",
                                    StrValue = "ZhuchaoSession",
                                    Remark = "二级菜单(Session名称)"
                                },
                                // Cookie 名称
                                new GeneralConfig
                                {
                                    Code ="CookieName",
                                    StrValue = "ZhuchaoCookie",
                                    Remark = "二级菜单(Cookie名称)"
                                }
                            }
                        };
                        GeneralConfig oConfig_Code = new GeneralConfig
                        {
                            // 代码生成器参数
                            Code = "CODE",
                            StrValue = "Code",
                            Remark = "一级菜单(代码生成器参数)",
                            ChildItems = new List<GeneralConfig>
                            {
                                // 字符代码前缀
                                new GeneralConfig
                                {
                                    Code = "CodePrefix_C",
                                    Ctype = (byte)ModelEnum.ConfigParamType.STRING,
                                    StrValue = "C",
                                    Remark = "二级菜单(字符代码前缀，每个数据库不一样，中国C，美国U，欧洲E)"
                                },
                                // 数字代码前缀
                                new GeneralConfig
                                {
                                    Code = "CodePrefix_N",
                                    Ctype = (byte)ModelEnum.ConfigParamType.STRING,
                                    StrValue = "1",
                                    Remark = "二级菜单(数字代码前缀，每个数据库不一样，中国1，美国2，欧洲3)"
                                },
                                // 错误报告代码 GeneralErrorReport.Code
                                new GeneralConfig
                                {
                                    Code = "ErrorReportCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(错误报告代码)"
                                },
                                // 待办事项代码 GeneralTodoList.Code
                                new GeneralConfig
                                {
                                    Code = "TodoListCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(待办事项代码)"
                                },
                                // 采购单号 PurchaseInformation.Code
                                new GeneralConfig
                                {
                                    Code = "PurchaseCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(采购单号，递增，不可跳号)"
                                },
                                // 质检单号 PurchaseInspection.Code
                                new GeneralConfig
                                {
                                    Code ="InspectionCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(质检单号，递增，不可跳号)"
                                },
                                // 入库单号 WarehouseStockIn.Code
                                new GeneralConfig
                                {
                                    Code ="StockInCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(入库单号，递增，不可跳号)"
                                },
                                // 出库单号 WarehouseStockOut.Code
                                new GeneralConfig
                                {
                                    Code ="StockOutCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(出库单号，递增，不可跳号)"
                                },
                                // 移库单号 WarehouseMoving.Code
                                new GeneralConfig
                                {
                                    Code ="MovingCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(移库单号，递增，不可跳号)"
                                },
                                // 盘点单号 WarehouseInventory.Code
                                new GeneralConfig
                                {
                                    Code ="InventoryCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(盘点单号，递增，不可跳号)"
                                },
                                // 订单号 OrderInformation.Code
                                new GeneralConfig
                                {
                                    Code ="OrderCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(订单号，递增，不可跳号)"
                                },
                                // 应付单号 FinancePayment.Code
                                new GeneralConfig
                                {
                                    Code ="PaymentCode",
                                    Ctype = (byte)ModelEnum.ConfigParamType.INTEGER,
                                    IntValue = 0,
                                    Remark = "二级菜单(应付号，递增，不可跳号)"
                                }
                            }
                        };
                        oLiveEntities.GeneralConfigs.Add(oConfig_System);
                        oLiveEntities.GeneralConfigs.Add(oConfig_Code);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: GeneralConfig 配置参数");
                        #endregion

                        #region GeneralStandardCategory标准分类

                        #region 标准组织类别
                        GeneralStandardCategory oStandardOrganization = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.ORGANIZATION,
                            Code = "Standard",
                            Remark = "标准组织类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "标准组织",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Standard Organization" }
                                }
                            }
                        };
                        #endregion

                        #region 渠道类别
                        GeneralStandardCategory oChannelType01 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "OfficialWeb",
                            Remark = "官网",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "正式官网",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Official Website" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType02 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "Taobao",
                            Remark = "淘宝店，包括淘宝旗舰店和专营店等",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "淘宝店",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Taobao" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType03 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "Paipai",
                            Remark = "拍拍店",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "拍拍店",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Paipai" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType04 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "Sina",
                            Remark = "新浪商城",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "新浪商城",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Sina Mall" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType05 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "tg.com.cn",
                            Remark = "齐家网",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "齐家网",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Qijia" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType06 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "360buy",
                            Remark = "京东商城",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "京东商城",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "360buy" }
                                }
                            }
                        };
                        GeneralStandardCategory oChannelType07 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.CHANNEL,
                            Code = "dangdang",
                            Remark = "当当网",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "当当网",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "dangdang" }
                                }
                            }
                        };
                        #endregion

                        #region 入库类型
                        GeneralStandardCategory oStockInType01 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKIN,
                            Code = "PurchaseIn",
                            Remark = "大货入库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "大货入库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Bulk In" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockInType02 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKIN,
                            Code = "ReturnIn",
                            Remark = "退货入库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "退货入库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Return In" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockInType03 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKIN,
                            Code = "InventoryProfit",
                            Remark = "盘盈入库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "盘盈",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Inventory Profit" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockInType04 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKIN,
                            Code = "AdjustStockIn",
                            Remark = "调整入库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "调整入库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Adjust Stock In" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockInType05 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKIN,
                            Code = "MoveIn",
                            Remark = "移入库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "移入库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Move In" }
                                }
                            }
                        };
                        #endregion

                        #region 出库类型
                        GeneralStandardCategory oStockOutType01 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "Sale",
                            Remark = "销售出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "销售出库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Sale" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType01b = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "Resend",
                            Remark = "销售补发货出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "销售补发/换货",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Resend/Change" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType02 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "ReturnSupplier",
                            Remark = "采购退货出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "采购退货",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Return Supplier" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType03 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "Discard",
                            Remark = "报废出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "报废",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Discard" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType04 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "InventoryLosses",
                            Remark = "盘亏出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "盘亏",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Inventory Losses" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType05 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "Adjust Stock Out",
                            Remark = "调整出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "调整出库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Adjust Stock Out" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType06 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "MoveOut",
                            Remark = "移出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "移出库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Move Out" }
                                }
                            }
                        };
                        GeneralStandardCategory oStockOutType07 = new GeneralStandardCategory
                        {
                            Ctype = (byte)ModelEnum.StandardCategoryType.STOCKOUT,
                            Code = "InternalUse",
                            Remark = "内部领用出库类型",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "内部领用出库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Internal Use" }
                                }
                            }
                        };
                        #endregion

                        oLiveEntities.GeneralStandardCategorys.Add(oStandardOrganization);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType01);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType02);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType03);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType04);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType05);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType06);
                        oLiveEntities.GeneralStandardCategorys.Add(oChannelType07);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockInType01);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockInType02);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockInType03);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockInType04);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockInType05);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType01);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType01b);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType02);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType03);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType04);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType05);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType06);
                        oLiveEntities.GeneralStandardCategorys.Add(oStockOutType07);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: GeneralStandardCategory 标准分类");
                        #endregion

                        #region GeneralMeasureUnit 主要计量单位
                        GeneralMeasureUnit oUnitPiece = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.PIECE,
                            Code = "PCS",
                            Remark = "个/件",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "个",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Piece" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitKG = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.WEIGHT,
                            Code = "KG",
                            Remark = "千克/公斤",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "千克",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "KG" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitG = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.WEIGHT,
                            Code = "G",
                            Remark = "克",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "克",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "G" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitPound = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.WEIGHT,
                            Code = "Pound",
                            Remark = "英磅",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "英磅",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Pound" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitCube = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.VOLUME,
                            Code = "CUBE",
                            Remark = "平方米",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "立方米",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Cube Meter" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitLitre = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.FLUID,
                            Code = "Litre",
                            Remark = "升",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "升",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Litre" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitGallon = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.FLUID,
                            Code = "Gallon",
                            Remark = "加仑",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "加仑",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Gallon" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitSquare = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.AREA,
                            Code = "Square",
                            Remark = "平方米",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "平方米",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Square Meter" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitSF = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.AREA,
                            Code = "SF",
                            Remark = "平方英尺",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "平方英尺",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Square Feet" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitMeter = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.LINEAR,
                            Code = "Meter",
                            Remark = "米",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "米",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Meter" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitFeet = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.LINEAR,
                            Code = "Feet",
                            Remark = "英尺",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "英尺",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Feet" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitRMB = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                            Code = (new CultureInfo("zh-CN")).NumberFormat.CurrencySymbol,
                            Remark = "人民币",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "人民币",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "RMB" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitUSD = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                            Code = (new CultureInfo("en-US")).NumberFormat.CurrencySymbol,
                            Remark = "美元",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "美元",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "USD" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitEUR = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                            Code = (new CultureInfo("fr-FR")).NumberFormat.CurrencySymbol,
                            Remark = "欧元",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "欧元",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "EUR" }
                                }
                            }
                        };
                        GeneralMeasureUnit oUnitGRP = new GeneralMeasureUnit
                        {
                            Utype = (byte)ModelEnum.MeasureUnit.CURRENCY,
                            Code = (new CultureInfo("en-GB")).NumberFormat.CurrencySymbol,
                            Remark = "英镑",
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "英镑",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "GRP" }
                                }
                            }
                        };
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitPiece);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitKG);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitG);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitPound);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitCube);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitLitre);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitGallon);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitSquare);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitSF);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitMeter);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitFeet);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitRMB);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitUSD);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitEUR);
                        oLiveEntities.GeneralMeasureUnits.Add(oUnitGRP);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: GeneralMeasureUnit 主要计量单位");
                        #endregion

                        #region GeneralCultureUnit 默认语言和计量单位
                        GeneralCultureUnit oCultureZHCN = new GeneralCultureUnit
                        {
                            // 中文(中国)
                            Culture = 2052,  // zh-CN
                            Piece = oUnitPiece,
                            Weight = oUnitKG,
                            Volume = oUnitCube,
                            Fluid = oUnitLitre,
                            Area = oUnitSquare,
                            Linear = oUnitMeter,
                            Currency = oUnitRMB
                        };
                        GeneralCultureUnit oCultureENUS = new GeneralCultureUnit
                        {
                            // 英语(美国)
                            Culture = 1033,  // en-US
                            Piece = oUnitPiece,
                            Weight = oUnitPound,
                            Volume = oUnitCube,
                            Fluid = oUnitGallon,
                            Area = oUnitSF,
                            Linear = oUnitFeet,
                            Currency = oUnitUSD
                        };
                        GeneralCultureUnit oCultureENGB = new GeneralCultureUnit
                        {
                            // 英语(英国)
                            Culture = 2057,  // en-GB
                            Piece = oUnitPiece,
                            Weight = oUnitPound,
                            Volume = oUnitCube,
                            Fluid = oUnitGallon,
                            Area = oUnitSF,
                            Linear = oUnitFeet,
                            Currency = oUnitGRP
                        };
                        GeneralCultureUnit oCultureFRFR = new GeneralCultureUnit
                        {
                            // 法文(法国)
                            Culture = 1036,  // fr-FR
                            Piece = oUnitPiece,
                            Weight = oUnitKG,
                            Volume = oUnitCube,
                            Fluid = oUnitLitre,
                            Area = oUnitSquare,
                            Linear = oUnitMeter,
                            Currency = oUnitEUR
                        };
                        GeneralCultureUnit oCultureDEDE = new GeneralCultureUnit
                        {
                            // 德文(德国)
                            Culture = 1031,  // de-DE
                            Piece = oUnitPiece,
                            Weight = oUnitKG,
                            Volume = oUnitCube,
                            Fluid = oUnitLitre,
                            Area = oUnitSquare,
                            Linear = oUnitMeter,
                            Currency = oUnitEUR
                        };
                        oLiveEntities.GeneralCultureUnits.Add(oCultureZHCN);
                        oLiveEntities.GeneralCultureUnits.Add(oCultureENUS);
                        oLiveEntities.GeneralCultureUnits.Add(oCultureENGB);
                        oLiveEntities.GeneralCultureUnits.Add(oCultureFRFR);
                        oLiveEntities.GeneralCultureUnits.Add(oCultureDEDE);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: GeneralCultureUnit 默认语言和计量单位");
                        #endregion

                        #region 预定义第一个组织、两个渠道、第一个仓库、两个角色由触发器生成
                        // 第一个组织
                        MemberOrganization oFirstOrgan = new MemberOrganization
                        {
                            Code = "Zhuchao",
                            Ostatus = (byte)ModelEnum.OrganizationStatus.VALID,
                            ExtendType = oStandardOrganization,  // 标准组织
                            Terminal = true,
                            FullName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "上海筑巢信息科技有限公司",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shanghai Zhuchao Information Technologo Co., Ltd." }
                                }
                            },
                            ShortName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "筑巢",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Zhuchao" }
                                }
                            },
                            FullAddress = "上海市嘉定区马陆镇丰功路958号",
                            PostCode = "201801",
                            Contact = "伯鉴",
                            CellPhone = "13816626660",
                            WorkPhone = "(86)21-60831660",
                            WorkFax = "(86)21-60831657",
                            Email = "admin@zhuchao.com",
                            HomeUrl = "http://www.zhuchao.com"
                        };
                        // 第一个官网渠道
                        MemberChannel oFirstChannel = new MemberChannel
                        {
                            Code = "ZCWEB001",
                            Ostatus = (byte)ModelEnum.OrganizationStatus.VALID,
                            ExtendType = oChannelType01,        // 官网
                            Terminal = true,
                            FullName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "筑巢官网",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Zhuchao Website" }
                                }
                            },
                            ShortName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "筑巢官网",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Zhuchao Website" }
                                }
                            },
                            FullAddress = "上海市嘉定区马陆镇丰功路958号",
                            PostCode = "201801",
                            Contact = "伯鉴",
                            CellPhone = "13816626660",
                            WorkPhone = "(86)21-60831660",
                            WorkFax = "(86)21-60831657",
                            Email = "admin@zhuchao.com",
                            HomeUrl = "http://www.zhuchao.com"
                        };
                        // 第二个淘宝专营店渠道
                        MemberChannel oSecondChannel = new MemberChannel
                        {
                            Code = "ZCTB001",
                            Ostatus = (byte)ModelEnum.OrganizationStatus.VALID,
                            ExtendType = oChannelType02,        // 淘宝网
                            Terminal = true,
                            FullName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "淘宝专营店",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Taobao 专营店" }
                                }
                            },
                            ShortName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "淘宝专营店",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Taobao 专营店" }
                                }
                            },
                            FullAddress = "上海市嘉定区马陆镇丰功路958号",
                            PostCode = "201801",
                            Contact = "伯鉴",
                            CellPhone = "13816626660",
                            WorkPhone = "(86)21-60831660",
                            WorkFax = "(86)21-60831657",
                            Email = "admin@zhuchao.com",
                            HomeUrl = "http://www.zhuchao.com.cn"
                        };
                        // 组织和渠道关联
                        MemberOrgChannel oOrganChannel1 = new MemberOrgChannel
                        {
                            Organization = oFirstOrgan,
                            Channel = oFirstChannel,
                            Cstatus = (byte)ModelEnum.GenericStatus.VALID
                        };
                        MemberOrgChannel oOrganChannel2 = new MemberOrgChannel
                        {
                            Organization = oFirstOrgan,
                            Channel = oSecondChannel,
                            Cstatus = (byte)ModelEnum.GenericStatus.VALID,
                            // RemoteUrl = "http://gw.api.tbsandbox.com/router/rest",         // 沙箱地址
                            RemoteUrl = "http://gw.api.taobao.com/router/rest",               // 正式地址
                            ConfigKey = "12176743",
                            SecretKey = "a9e366dde6816c2866e4f60af62162ca",
                            SessionKey = "23767603b359d0623b84b6963b5507db8f6b3_1"
                        };
                        // 组织支持的默认语言
                        MemberOrgCulture oOrganCulture01 = new MemberOrgCulture
                        {
                            Organization = oFirstOrgan,
                            Ctype = (byte)ModelEnum.CultureType.LANGUAGE,
                            Culture = oCultureZHCN
                        };
                        MemberOrgCulture oOrganCulture02 = new MemberOrgCulture
                        {
                            Organization = oFirstOrgan,
                            Ctype = (byte)ModelEnum.CultureType.LANGUAGE,
                            Culture = oCultureENUS
                        };
                        MemberOrgCulture oOrganCulture03 = new MemberOrgCulture
                        {
                            Organization = oFirstOrgan,
                            Ctype = (byte)ModelEnum.CultureType.CURRENCY,
                            Currency = oUnitRMB
                        };
                        // 第一个仓库及其支持的渠道
                        WarehouseInformation oWarehouse = new WarehouseInformation
                        {
                            Code = "ZCWH001",
                            Ostatus = (byte)ModelEnum.OrganizationStatus.VALID,
                            Parent = oFirstOrgan,
                            Terminal = true,
                            FullName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "上海丰功路仓库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shanghai Fenggong Road" }
                                }
                            },
                            ShortName = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "上海丰功路仓库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shanghai Fenggong Road" }
                                }
                            },
                            FullAddress = "上海市嘉定区马陆镇丰功路958号",
                            PostCode = "201801",
                            Contact = "伯鉴",
                            CellPhone = "13816626660",
                            WorkPhone = "(86)21-60831660",
                            WorkFax = "(86)21-60831657",
                            Email = "admin@zhuchao.com",
                            HomeUrl = "http://www.zhuchao.com",
                            Channels = new List<MemberOrgChannel>
                            {
                                new MemberOrgChannel
                                {
                                    Channel = oFirstChannel,
                                    Cstatus = (byte)ModelEnum.GenericStatus.VALID
                                }
                            }
                        };

                        oLiveEntities.MemberOrgChannels.Add(oOrganChannel1);
                        oLiveEntities.MemberOrgChannels.Add(oOrganChannel2);
                        oLiveEntities.MemberOrgCultures.Add(oOrganCulture01);
                        oLiveEntities.MemberOrgCultures.Add(oOrganCulture02);
                        oLiveEntities.MemberOrgCultures.Add(oOrganCulture03);
                        oLiveEntities.WarehouseInformations.Add(oWarehouse);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: 预定义第一个组织、两个渠道、第一个仓库、角色由触发器生成");
                        #endregion

                        #region 管理员角色，第一个管理员用户，第一个测试用户
                        MemberRole oAdminRole = new MemberRole
                        {
                            Organization = oFirstOrgan,
                            Code = "Supervisor",               // 唯一的一个超级管理员角色，不需要任何权限
                            Parent = oLiveEntities.MemberRoles.Where(r => r.Code == "Internal" && r.OrgID == oFirstOrgan.Gid).FirstOrDefault(),
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "超级管理员",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Supervisor" }
                                }
                            },
                            Remark = "超级管理员"
                        };
                        // 第一个用户
                        MemberUser oAdminUser = new MemberUser
                        {
                            Organization = oFirstOrgan,
                            Role = oAdminRole,
                            Channel = oFirstChannel,
                            LoginName = "admin",
                            Ustatus = (byte)ModelEnum.UserStatus.VALID,
                            NickName = "admin",
                            FirstName = "System",
                            LastName = "Admin",
                            DisplayName = "System Admin",
                            Culture = oCultureENUS,//天佑
                            Passcode = "izhuchao.com"
                        };
                        //测试用户Tester 测试用
                        MemberUser oTester = new MemberUser
                        {
                            Organization = oFirstOrgan,
                            Role = oLiveEntities.MemberRoles.Single(r => (r.OrgID == oFirstOrgan.Gid && r.Code == "Internal")),
                            Channel = oFirstChannel,
                            LoginName = "test",
                            Ustatus = (byte)ModelEnum.UserStatus.VALID,
                            NickName = "test",
                            FirstName = "System",
                            LastName = "Tester",
                            DisplayName = "System Tester",
                            Culture = oCultureZHCN,
                            Passcode = "izhuchao.com"
                        };
                        oLiveEntities.MemberUsers.Add(oAdminUser);
                        oLiveEntities.MemberUsers.Add(oTester);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: 预定义超级管理员角色，第一个管理员和测试用户");
                        #endregion

                        #region GeneralProgram 程序定义

                        #region 首页
                        GeneralProgram oProgramHome = new GeneralProgram
                        {
                            Code = "HomeHomePage",
                            Parent = null,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "首页",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Home" }
                                }
                            },
                            ProgUrl = "/Home/HomePage"
                        };
                        #endregion

                        #region 系统菜单
                        GeneralProgram oProgramSys = new GeneralProgram
                        {
                            Code = "System",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "系统",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "System" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramSys01 = new GeneralProgram
                        {
                            Code = "ConfigIndex",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "配置参数",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Config Parameters" }
                                }
                            },
                            ProgUrl = "/Config/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys02 = new GeneralProgram
                        {
                            Code = "ProgramIndex",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "程序定义",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Program Define" }
                                }
                            },
                            ProgUrl = "/Program/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys03 = new GeneralProgram
                        {
                            Code = "RegionIndex",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "地区维护",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Region Define" }
                                }
                            },
                            ProgUrl = "/Region/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys04 = new GeneralProgram
                        {
                            Code = "CategoryIndex",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "分类管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Category Define" }
                                }
                            },
                            ProgUrl = "/Category/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEditPrivate",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑私有分类",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Private" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableEditStandard",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑标准分类",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Standard" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys05 = new GeneralProgram
                        {
                            Code = "OptionalIndex",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "属性管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Attribute Manage" }
                                }
                            },
                            ProgUrl = "/Optional/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys06 = new GeneralProgram
                        {
                            Code = "ConfigUnit",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "计量单位",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Measure Unit" }
                                }
                            },
                            ProgUrl = "/Config/MeasureUnit",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys07 = new GeneralProgram
                        {
                            Code = "ConfigCulture",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "语言文化",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Culture Manage" }
                                }
                            },
                            ProgUrl = "/Config/Culture",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys08 = new GeneralProgram
                        {
                            Code = "ConfigMessage",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "消息队列",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Message" }
                                }
                            },
                            ProgUrl = "/Config/Message",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableSend",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许发短信",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Send" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramSys11 = new GeneralProgram
                        {
                            Code = "ConfigErrorReport",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "错误报告",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Error Report" }
                                }
                            },
                            ProgUrl = "/Config/ErrorReport"
                        };
                        GeneralProgram oProgramSys12 = new GeneralProgram
                        {
                            Code = "ConfigAction",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "事件日志",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Action Event" }
                                }
                            },
                            ProgUrl = "/Config/Action"
                        };
                        GeneralProgram oProgramSys13 = new GeneralProgram
                        {
                            Code = "ConfigShortcut",
                            Parent = oProgramSys,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "快捷方式",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shortcut" }
                                }
                            },
                            ProgUrl = "/Config/Shortcut"
                        };
                        #endregion

                        #region 会员菜单
                        GeneralProgram oProgramMem = new GeneralProgram
                        {
                            Code = "Member",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "会员",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Member" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramMem01 = new GeneralProgram
                        {
                            Code = "OrganizationIndex",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "组织管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Organization Manage" }
                                }
                            },
                            ProgUrl = "/Organization/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMem02 = new GeneralProgram
                        {
                            Code = "OrganizationChannel",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "渠道管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Channel Manage" }
                                }
                            },
                            ProgUrl = "/Organization/ChannelIndex",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMem03 = new GeneralProgram
                        {
                            Code = "MemberRole",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "角色管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Role Manage" }
                                }
                            },
                            ProgUrl = "/User/MemberRole",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMem04 = new GeneralProgram
                        {
                            Code = "UserIndex",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "用户管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "User Manage" }
                                }
                            },
                            ProgUrl = "/User/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMem05 = new GeneralProgram
                        {
                            Code = "UserPrivilege",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "用户授权",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "User Privilege" }
                                }
                            },
                            ProgUrl = "/Privilege/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMem06 = new GeneralProgram
                        {
                            Code = "UserPoint",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "用户积分",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "User Point" }
                                }
                            },
                            ProgUrl = "/User/Point"
                        };
                        GeneralProgram oProgramMem07 = new GeneralProgram
                        {
                            Code = "UserLevel",
                            Parent = oProgramMem,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "用户级别",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "User Level" }
                                }
                            },
                            ProgUrl = "/User/Level"
                        };
                        #endregion

                        #region 商品菜单
                        GeneralProgram oProgramProd = new GeneralProgram
                        {
                            Code = "Product",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "商品",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramProd01 = new GeneralProgram
                        {
                            Code = "ProductIndex",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "商品列表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product List" }
                                }
                            },
                            ProgUrl = "/Product/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramProd02 = new GeneralProgram
                        {
                            Code = "ProductGallery",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "图片处理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product Gallery" }
                                }
                            },
                            ProgUrl = "/Product/Gallery",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramProd03 = new GeneralProgram
                        {
                            Code = "ProductImport",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "批量导入",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product Import" }
                                }
                            },
                            ProgUrl = "/Product/Import"
                        };
                        GeneralProgram oProgramProd04 = new GeneralProgram
                        {
                            Code = "ProductValidation",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "数据验证",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product Validation" }
                                }
                            },
                            ProgUrl = "/Product/Validation"
                        };
                        GeneralProgram oProgramProd05 = new GeneralProgram
                        {
                            Code = "OnSaleIndex",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "商品上架",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Product On Sale" }
                                }
                            },
                            ProgUrl = "/Product/OnSaleIndex",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认(上架)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm (On Sale)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramProd06 = new GeneralProgram
                        {
                            Code = "OnSaleTemplate",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "上架模板",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "On Sale Template" }
                                }
                            },
                            ProgUrl = "/Product/OnSaleTemplate",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramProd07 = new GeneralProgram
                        {
                            Code = "ProductCodePolicy",
                            Parent = oProgramProd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "代码规则",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Code Policy" }
                                }
                            },
                            ProgUrl = "/Product/CodePolicy",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        #endregion

                        #region 采购菜单
                        GeneralProgram oProgramPur = new GeneralProgram
                        {
                            Code = "Purchase",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "采购",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Purchase" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramPur01 = new GeneralProgram
                        {
                            Code = "PurchaseIndex",
                            Parent = oProgramPur,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "采购单据",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Purchase List" }
                                }
                            },
                            ProgUrl = "/Purchase/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableClose",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许结算",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Close" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramPur02 = new GeneralProgram
                        {
                            Code = "PurchaseQuality",
                            Parent = oProgramPur,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "质量检查",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Quality Control" }
                                }
                            },
                            ProgUrl = "/Purchase/Quality",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramPur03 = new GeneralProgram
                        {
                            Code = "OrganizationSupplier",
                            Parent = oProgramPur,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "供应商",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Supplier" }
                                }
                            },
                            ProgUrl = "/Organization/SupplierIndex",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        #endregion

                        #region 仓库菜单
                        GeneralProgram oProgramWh = new GeneralProgram
                        {
                            Code = "Warehouse",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "仓库",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramWh01 = new GeneralProgram
                        {
                            Code = "OrganizationWarehouse",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "仓库列表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse List" }
                                }
                            },
                            ProgUrl = "/Organization/WarehouseIndex",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh02 = new GeneralProgram
                        {
                            Code = "WarehouseIndex",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "库存总账",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse Ledger" }
                                }
                            },
                            ProgUrl = "/Warehouse/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑(冻结)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit (Lock)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh03 = new GeneralProgram
                        {
                            Code = "WarehouseStockIn",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "入库记录",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Stock In" }
                                }
                            },
                            ProgUrl = "/Warehouse/StockIn",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrint",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许打印",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Print" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh04 = new GeneralProgram
                        {
                            Code = "WarehouseStockOut",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "出库记录",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Stock Out" }
                                }
                            },
                            ProgUrl = "/Warehouse/StockOut",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrint",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许打印",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Print" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认(扫描/发货)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm (Scan/Delivery)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableSignFor",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许签收",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow SignFor" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh05 = new GeneralProgram
                        {
                            Code = "WarehouseMoving",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "移库申请",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Moving" }
                                }
                            },
                            ProgUrl = "/Warehouse/Moving",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh06 = new GeneralProgram
                        {
                            Code = "WarehouseInventory",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "盘点记录",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Inventory" }
                                }
                            },
                            ProgUrl = "/Warehouse/Inventory",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnablePrepare",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许制表(编辑)",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Prepare (Edit)" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableApprove",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableSnapshot",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许仓库快照",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Snapshot" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh07 = new GeneralProgram
                        {
                            Code = "WarehouseShelf",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "货架管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse Shelf" }
                                }
                            },
                            ProgUrl = "/Warehouse/Shelf",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEditShelf",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许定义货架",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Shelf" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableEditLock",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑冻结",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Lock" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            },
                            Remark = "包括WarehouseSkuShelf"
                        };
                        GeneralProgram oProgramWh08 = new GeneralProgram
                        {
                            Code = "WarehouseRegion",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "送达地区",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Support Region" }
                                }
                            },
                            ProgUrl = "/Warehouse/Region",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramWh09 = new GeneralProgram
                        {
                            Code = "WarehouseShipping",
                            Parent = oProgramWh,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "承运商",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Support Shipping" }
                                }
                            },
                            ProgUrl = "/Warehouse/Shipping",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        #endregion

                        #region 订单菜单
                        GeneralProgram oProgramOrd = new GeneralProgram
                        {
                            Code = "Order",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "订单",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Order" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramOrd01 = new GeneralProgram
                        {
                            Code = "OrderIndex",
                            Parent = oProgramOrd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "订单列表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Order List" }
                                }
                            },
                            ProgUrl = "/Order/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableNew",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许新建订单",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow New Order" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableConfig",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许确认订单",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Confirm Order" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableArrange",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许排单",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Arrange" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableClose",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许结算",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Close" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableCancel",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许取消",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Cancel" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnablePayment",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许收/付款",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Payment" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableEditAddress",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑地址",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Address" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                },
                                new GeneralProgNode
                                {
                                    Code = "EnableEditPrice",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑价格",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit Price" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramOrd02 = new GeneralProgram
                        {
                            Code = "OrderConfirm",
                            Parent = oProgramOrd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "订单确认",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Order Confirm" }
                                }
                            },
                            ProgUrl = "/Order/Confirm",
                            Remark = "跳转到/Order/Index?Confirm=待确认，使用OrderIndex的权限"
                        };
                        GeneralProgram oProgramOrd03 = new GeneralProgram
                        {
                            Code = "OrderPolicy",
                            Parent = oProgramOrd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "订单策略",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Order Policy" }
                                }
                            },
                            ProgUrl = "/Order/Policy",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            },
                            Remark = "包括拆单，自动处理等策略"
                        };
                        GeneralProgram oProgramOrd04 = new GeneralProgram
                        {
                            Code = "PromotionIndex",
                            Parent = oProgramOrd,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "促销管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Promotion" }
                                }
                            },
                            ProgUrl = "/Promotion/Index"
                        };
                        #endregion

                        #region 承运菜单
                        GeneralProgram oProgramShip = new GeneralProgram
                        {
                            Code = "Shipping",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "承运",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shipping" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramShip01 = new GeneralProgram
                        {
                            Code = "OrganizationShipper",
                            Parent = oProgramShip,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "承运商列表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shipper List" }
                                }
                            },
                            ProgUrl = "/Organization/ShippingIndex",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramShip02 = new GeneralProgram
                        {
                            Code = "ShippingRegion",
                            Parent = oProgramShip,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "承运商地区",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shipper Region" }
                                }
                            },
                            ProgUrl = "/Shipping/ShipperRegion",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramShip03 = new GeneralProgram
                        {
                            Code = "ShippingIndex",
                            Parent = oProgramShip,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "面单管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Shipper Envelope" }
                                }
                            },
                            ProgUrl = "/Shipping/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        #endregion

                        #region 财务菜单
                        GeneralProgram oProgramFin = new GeneralProgram
                        {
                            Code = "Finance",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "财务",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Finance" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramFin01 = new GeneralProgram
                        {
                            Code = "FinanceIndex",
                            Parent = oProgramFin,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "付款方式",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Payment Type" }
                                }
                            },
                            ProgUrl = "/Finance/Index",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramFin02 = new GeneralProgram
                        {
                            Code = "FinanceInvoice",
                            Parent = oProgramFin,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "发票管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Invoice" }
                                }
                            },
                            ProgUrl = "/Finance/Invoice",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramFin03 = new GeneralProgram
                        {
                            Code = "FinancePayable",
                            Parent = oProgramFin,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "应付款",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Account Payable" }
                                }
                            },
                            ProgUrl = "/Finance/Payable",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        #endregion

                        #region 商城管理
                        GeneralProgram oProgramMall = new GeneralProgram
                        {
                            Code = "Mall",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "商城",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Mall" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramMall01 = new GeneralProgram
                        {
                            Code = "MallIndex",
                            Parent = oProgramMall,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "内容管理",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Mall Content" }
                                }
                            },
                            ProgUrl = "/Mall/Index",
                            Remark = "包括发布",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMall02 = new GeneralProgram
                        {
                            Code = "MallPosition",
                            Parent = oProgramMall,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "位置定义",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Art Position" }
                                }
                            },
                            ProgUrl = "/Mall/Position",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            }
                        };
                        GeneralProgram oProgramMall03 = new GeneralProgram
                        {
                            Code = "MallSetting",
                            Parent = oProgramMall,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "商城配置",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Mall Setting" }
                                }
                            },
                            ProgUrl = "/Mall/Setting",
                            ProgramNodes = new List<GeneralProgNode>
                            {
                                new GeneralProgNode
                                {
                                    Code = "EnableEdit",
                                    Name = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "允许编辑",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "Allow Edit" }
                                        }
                                    },
                                    InputMode = (byte)ModelEnum.OptionalInputMode.COMBOBOX,
                                    Optional = new GeneralResource
                                    {
                                        Culture = 2052, Matter = "0|否,1|是",
                                        ResourceItems = new List<GeneralResItem>
                                        {
                                            new GeneralResItem { Culture = 1033, Matter = "0|No,1|Yes" }
                                        }
                                    }
                                }
                            },
                            Remark = "包括热门关键词，商品收藏，关注，友情链接，敏感词，黑名单等"
                        };
                        GeneralProgram oProgramMall04 = new GeneralProgram
                        {
                            Code = "MallClick",
                            Parent = oProgramMall,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "点击统计",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Click Statistics" }
                                }
                            },
                            ProgUrl = "/Mall/Click"
                        };
                        #endregion

                        #region 知识
                        GeneralProgram oProgramKnow = new GeneralProgram
                        {
                            Code = "Knowledge",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "知识",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Knowledge" }
                                }
                            },
                            ProgUrl = ""
                        };
                        #endregion

                        #region 报表集合
                        GeneralProgram oProgramRpt = new GeneralProgram
                        {
                            Code = "Report",
                            Parent = null,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "报表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Report" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramRpt01 = new GeneralProgram
                        {
                            Code = "ReportSales",
                            Parent = oProgramRpt,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "销售报表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Sales Report" }
                                }
                            },
                            ProgUrl = ""
                        };
                        GeneralProgram oProgramRpt0101 = new GeneralProgram
                        {
                            Code = "ReportSalesDaily",
                            Parent = oProgramRpt01,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "日销售报表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Sales Daily Report" }
                                }
                            },
                            ProgUrl = "/Report/Sales/Daily"
                        };
                        GeneralProgram oProgramRpt02 = new GeneralProgram
                        {
                            Code = "ReportWarehouse",
                            Parent = oProgramRpt,
                            Terminal = false,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "库存报表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse Report" }
                                }
                            },
                            ProgUrl = "/Report/Warehouse/Analysis"
                        };
                        GeneralProgram oProgramRpt0201 = new GeneralProgram
                        {
                            Code = "ReportWarehouseStock",
                            Parent = oProgramRpt02,
                            Terminal = true,
                            Name = new GeneralResource
                            {
                                Culture = 2052,
                                Matter = "库存分析表",
                                ResourceItems = new List<GeneralResItem>
                                {
                                    new GeneralResItem { Culture = 1033, Matter = "Warehouse Stock" }
                                }
                            },
                            ProgUrl = "/Report/Warehouse/Analysis"
                        };
                        #endregion

                        oLiveEntities.GeneralPrograms.Add(oProgramHome);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys01);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys02);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys03);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys04);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys05);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys06);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys07);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys08);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys11);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys12);
                        oLiveEntities.GeneralPrograms.Add(oProgramSys13);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem01);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem02);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem03);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem04);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem05);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem06);
                        oLiveEntities.GeneralPrograms.Add(oProgramMem07);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd01);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd02);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd03);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd04);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd05);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd06);
                        oLiveEntities.GeneralPrograms.Add(oProgramProd07);
                        oLiveEntities.GeneralPrograms.Add(oProgramPur01);
                        oLiveEntities.GeneralPrograms.Add(oProgramPur02);
                        oLiveEntities.GeneralPrograms.Add(oProgramPur03);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh01);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh02);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh03);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh04);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh05);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh06);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh07);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh08);
                        oLiveEntities.GeneralPrograms.Add(oProgramWh09);
                        oLiveEntities.GeneralPrograms.Add(oProgramOrd01);
                        oLiveEntities.GeneralPrograms.Add(oProgramOrd02);
                        oLiveEntities.GeneralPrograms.Add(oProgramOrd03);
                        oLiveEntities.GeneralPrograms.Add(oProgramOrd04);
                        oLiveEntities.GeneralPrograms.Add(oProgramShip01);
                        oLiveEntities.GeneralPrograms.Add(oProgramShip02);
                        oLiveEntities.GeneralPrograms.Add(oProgramShip03);
                        oLiveEntities.GeneralPrograms.Add(oProgramFin01);
                        oLiveEntities.GeneralPrograms.Add(oProgramFin02);
                        oLiveEntities.GeneralPrograms.Add(oProgramFin03);
                        oLiveEntities.GeneralPrograms.Add(oProgramMall01);
                        oLiveEntities.GeneralPrograms.Add(oProgramMall02);
                        oLiveEntities.GeneralPrograms.Add(oProgramMall03);
                        oLiveEntities.GeneralPrograms.Add(oProgramMall04);
                        oLiveEntities.GeneralPrograms.Add(oProgramKnow);
                        oLiveEntities.GeneralPrograms.Add(oProgramRpt0101);
                        oLiveEntities.GeneralPrograms.Add(oProgramRpt0201);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: GeneralProgram 程序定义");
                        #endregion

                        #region MemberPrivilege 权限(测试用)
                        //用户1程序权限
                        MemberPrivilege oMemberPrivilegeProgram = new MemberPrivilege
                        {
                            User = oTester,
                            Ptype = 0,// 0: 程序
                            PrivilegeItems = new List<MemberPrivItem>
                            { 
                                new MemberPrivItem { RefID = oProgramHome.Gid },
                                new MemberPrivItem { RefID = oProgramSys01.Gid },
                                new MemberPrivItem { RefID = oProgramSys02.Gid }
                            }
                        };
                        oLiveEntities.MemberPrivileges.Add(oMemberPrivilegeProgram);
                        oLiveEntities.SaveChanges();

                        Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.RunOnce: MemberPrivilege 权限(测试用)");
                        #endregion
                    }
                    // 提交事务，数据库物理写入
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase: TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase: ApplicationException Message: {0}", ex.Message);
            }
            
            GC.Collect();
        }

        /// <summary>
        /// 导入中国的地区表，只能执行一次
        /// </summary>
        /// <param name="sExcelFile">Excel文件名，Excel格式为Openshop.SystemRegion格式</param>
        [Obsolete]
        public static void ImportRegionExcel(string sExcelFile)
        {
            // 创建EF实体
            //using (var oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection))
            //{
            //    // 中国
            //    GeneralRegion oChina = new GeneralRegion
            //    {
            //        Code = "CHN",
            //        FullName = "中华人民共和国",
            //        ShortName = "中国",
            //        RegionLevel = 0
            //    };
            //    oLiveEntities.GeneralRegions.Add(oChina);

            //    if (!String.IsNullOrEmpty(sExcelFile) && File.Exists(sExcelFile))
            //    {
            //        Excel.Application objExcel = new Excel.Application();
            //        Excel.Workbooks objWorkbooks = objExcel.Workbooks;
            //        Excel.Workbook objWorkbook = objWorkbooks.Open(sExcelFile);
            //        Excel.Worksheet objWorksheet = objWorkbook.ActiveSheet;
            //        int nRowsCount = objWorksheet.UsedRange.Rows.Count;
            //        GeneralRegion oParent;
            //        for (int i = 3; i <= nRowsCount; i++)
            //        {
            //            string strCode = objWorksheet.Cells[i, 1].Text;
            //            string strParent = objWorksheet.Cells[i, 2].Text;
            //            string strFullName = objWorksheet.Cells[i, 3].Text;
            //            int nLevel = int.Parse(objWorksheet.Cells[i, 4].Text);
            //            string strShortName = objWorksheet.Cells[i, 10].Text;
            //            int nStatistics01 = 0;
            //            int nStatistics02 = 0;
            //            if (!String.IsNullOrEmpty(objWorksheet.Cells[i, 12].Text))
            //                nStatistics01 = int.Parse(objWorksheet.Cells[i, 12].Text);
            //            if (!String.IsNullOrEmpty(objWorksheet.Cells[i, 13].Text))
            //                nStatistics02 = int.Parse(objWorksheet.Cells[i, 13].Text);

            //            if (strParent == "1")
            //                oParent = oChina;
            //            else
            //                oParent = oLiveEntities.GeneralRegions.Single(r => r.Code == strParent);

            //            GeneralRegion oRegion = new GeneralRegion
            //            {
            //                Code = strCode,
            //                FullName = strFullName,
            //                ShortName = strShortName,
            //                RegionLevel = nLevel,
            //                Statistics01 = nStatistics01,
            //                Statistics02 = nStatistics02,
            //                Parent = oParent
            //            };

            //            oLiveEntities.GeneralRegions.Add(oRegion);
            //            oLiveEntities.SaveChanges();

            //            Debug.WriteLine("LiveAzure.Models.General.InitialiseDatabase.ImportRegion: {0} of {1} Completed.", i, nRowsCount);
            //        }
            //        objWorkbook.Close();
            //        objWorkbooks.Close();
            //        objExcel.Quit();
            //        objWorkbook = null;
            //        objWorkbooks = null;
            //        objExcel = null;
            //        GC.Collect();
            //    }
            //    oLiveEntities.SaveChanges();
            //}
            //GC.Collect();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LiveAzure.Models
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-01         -->
    /// <summary>
    /// Models通用基类，所有表均继承自该类
    /// </summary>
    public class ModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ModelBase()
        {
            this.Deleted = false;
            this.LastModifyTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// GUID主键
        /// </summary>
        [Key]
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Gid { get; set; }

        /// <summary>
        /// 是否删除了，true已删除，false未删除
        /// </summary>
        [ScaffoldColumn(false)]
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.ModelBase), Name = "Deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.ModelBase), Name = "CreateTime")]
        public DateTimeOffset? CreateTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.ModelBase), Name = "LastModifiedBy")]
        public Guid? LastModifiedBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        // [Display(ResourceType = typeof(LiveAzure.Resource.Model.ModelBase), Name = "LastModifyTime")]
        public DateTimeOffset? LastModifyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(256, ErrorMessageResourceType = typeof(LiveAzure.Resource.Model.ModelBase),
            ErrorMessageResourceName = "RemarkLong")]
        //[Display(ResourceType = typeof(LiveAzure.Resource.Model.ModelBase), Name = "Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 获取枚举类型的列表值
        /// </summary>
        /// <param name="oEnumType">枚举类型</param>
        /// <param name="nSelectedIndex">选中的项</param>
        /// <returns>符合MVC页面要求的ListItem列表值（多语言）</returns>
        public List<ListItem> SelectEnumList(Type oEnumType, byte nSelectedIndex)
        {
            List<ListItem> oList = new List<ListItem>();
            string[] sKeys = Enum.GetNames(oEnumType);
            int keysLength = sKeys.Length;
            string[] sNames = LiveAzure.Resource.Common.ResourceManager.GetString(oEnumType.Name).Split(',');
            int namesLength = sNames.Length;
            for (byte i = 0; i < keysLength; i++)
            {
                ListItem newItem = new ListItem
                {
                    Selected = (i == nSelectedIndex),
                    Text = (i < namesLength) ? sNames[i].Trim() : sKeys[i],
                    Value = i.ToString()
                };
                oList.Add(newItem);
            }
            return oList;
        }

        /// <summary>
        /// 某个枚举类型的名称
        /// </summary>
        /// <param name="oEnumType">枚举类型</param>
        /// <param name="nSelectedIndex">选中的项</param>
        /// <returns>某个枚举值的名称（多语言）</returns>
        public string SelectEnumName(Type oEnumType, byte nSelectedIndex)
        {
            string[] sNames = LiveAzure.Resource.Common.ResourceManager.GetString(oEnumType.Name).Split(',');
            if (nSelectedIndex < sNames.Length)
                return sNames[nSelectedIndex].Trim();
            else
                return string.Empty;
        }
    }
    
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-07-01         -->
    /// <summary>
    /// Models固定配置参数，主要是tinyint枚举，对应的多语言的名称存放在资源文件或xml中
    /// </summary>
    public class ModelEnum
    {
        #region 全局枚举

        /// <summary>
        /// 0否 1是
        /// </summary>
        public enum YesNo { NO, YES }

        /// <summary>
        /// 单据类型 0订单 1采购单 2发货单 3快递单, 4应付单, 5补/换发货(RefID=订单), 6移库单, 7无单据
        /// </summary>
        public enum NoteType { ORDER, PURCHASE, DELIVERY, EXPRESS, PAYMENT, RESEND, MOVE, NONE }

        /// <summary>
        /// 通用状态 0无效 1有效
        /// </summary>
        public enum GenericStatus { NONE, VALID }

        /// <summary>
        /// 计费方案，0按重量 1按体积 2按件
        /// </summary>
        public enum BillingType { WEIGHT, VOLUMN, PIECE }

        #endregion

        #region General Models枚举类型

        /// <summary>
        /// GeneralConfig 参数类型
        /// </summary>
        public enum ConfigParamType { INTEGER, DECIMAL, STRING, DATETIME }

        /// <summary>
        /// GeneralResource 资源类型，0字符型资源，1金额型资源
        /// </summary>
        public enum ResourceType { STRING, MONEY }

        /// <summary>
        /// GeneralLarge 大对象存放位置，0:未指定；1:数据库；2:操作系统; 3:网站URL
        /// </summary>
        public enum LargeObjectSource { NONE, DATEBASE, OPERATIONSYSTEM, WEBSITEURL }

        /// <summary>
        /// 用途 0通用 1组织 2会员 3产品 4订单
        /// </summary>
        public enum OptionalType { GENERIC, ORGANIZATION, USER, PRODUCT, ORDER }

        /// <summary>
        /// 输入模式 0编辑框 1下拉框
        /// </summary>
        public enum OptionalInputMode { TEXTBOX, COMBOBOX }

        /// <summary>
        /// 对标准分类的分类，用于全局分类，不依赖组织
        /// 类别 0组织分类 1渠道类型 2角色类型 3商品全局标准分类 4入库类型 5出库类型
        /// </summary>
        public enum StandardCategoryType { ORGANIZATION, CHANNEL, ROLE, PRODUCT, STOCKIN, STOCKOUT }

        /// <summary>
        /// 对私有分类的分类，仅适用于组织
        /// 0商品私有分类 1仓库分类 2运输公司分类 3采购类型 4供应商类型 5退单类型 6文章/评论类型 7文章/评论目录 8支付类型
        /// </summary>
        public enum PrivateCategoryType { PRODUCT, WAREHOUSE, SHIPPING, PURCHASE, SUPPLIER, RETURNTYPE, CMS_TYPE, CMS_FOLDER, PAY_TYPE }

        /// <summary>
        /// 计量单位，0计件 1重量 2体积 3容积 4面积 5长度 6货币 7其他
        /// </summary>
        public enum MeasureUnit { PIECE, WEIGHT, VOLUME, FLUID, AREA, LINEAR, CURRENCY, OTHER }

        /// <summary>
        /// 消息类型，0系统消息 1手机短信 2邮件
        /// </summary>
        public enum MessageType { SYSTEM, SMS, EMAIL }

        /// <summary>
        /// 消息状态 0待发送 1发送成功 2发送失败 3取消发送
        /// </summary>
        public enum MessageStatus { PENDING, SENDSUCCESS, SENDFAILED, CANCELLED }

        /// <summary>
        /// 日志级别 0普通 1警告 2错误
        /// </summary>
        public enum ActionLevel { GENERIC, WARNING, ERROR }

        /// <summary>
        /// 日志产生模块 0系统 1会员 2产品 3采购 4仓库 5订单 6承运 7商城 8联盟 9知识库 10财务 11绩效 12接口 13呼叫中心
        /// </summary>
        public enum ActionSource { SYSTEM, MEMBER, PRODUCT, PURCHASE, WAREHOUSE, ORDER, SHIPPING, MALL, UNION, KNOWLEDGE, FINANCE, PERFORMANCE, EXCHANGE, CALLCENTER }

        /// <summary>
        /// 类别 0订单待确认 1订单待发货 2转淘宝订单提示 3淘宝发货提示
        /// </summary>
        public enum TodoType { WAITFOR_CONFIRM, WAITFOR_DELIVERY, TAOBAO_TRANS_ERROR, TAOBAO_DELIVERY_ERROR }

        #endregion

        #region Member Models枚举类型

        /// <summary>
        /// 组织状态 0无效 1有效
        /// </summary>
        public enum OrganizationStatus { NONE, VALID }

        /// <summary>
        /// 组织类型，在xml中定义名称，0运营商、1渠道、2仓库、3供应商、4承运商
        /// </summary>
        public enum OrganizationType { CORPORATION, CHANNEL, WAREHOUSE, SUPPLIER, SHIPPER }

        /// <summary>
        /// 组织语言类型 0语言 1货币
        /// </summary>
        public enum CultureType { LANGUAGE, CURRENCY }

        /// <summary>
        /// 用户状态 0无效 1有效
        /// </summary>
        public enum UserStatus { NONE, VALID }

        /// <summary>
        /// 性别 0未知 1男 2女
        /// </summary>
        public enum Gender { UNKNOWN, MALE, FEMALE }

        /// <summary>
        /// 积分类别，在xml中定义名称 0积分 1现金余额 2抵用券 3联盟返点 4CPS返点 5代销返点
        /// </summary>
        public enum PointType { POINT, CASH, COUPON, REWARD_UNION, REWARD_CPS, REWARD_AGENT }

        /// <summary>
        /// 是否有效 0无效 1有效 2已使用
        /// </summary>
        public enum PointStatus { NONE, VALID, USED }

        /// <summary>
        /// 是否有效 0无效 1消费 2提现
        /// </summary>
        public enum PointUsed { NONE, USED, CASHIER }

        /// <summary>
        /// 用户事件类型 0电话呼入 1电话呼出 2发DM 3在线沟通
        /// </summary>
        public enum UserEventType { INBOUND, OUTBOUND, SEND_DM, CHAT }

        /// <summary>
        /// 授权类型，名称在xml中定义 类型xml定义 0程序 1程序功能 2组织 3渠道 4仓库 5商品类别私有 6供应商类别 7...
        /// </summary>
        public enum UserPrivType { PROGRAM, PROGRAM_NODE, ORGANIZATION, CHANNEL, WAREHOUSE, PRODUCT_CATEGORY, SUPPLIER_CATEGORY }

        /// <summary>
        /// MemberChannel表中渠道状态，Cstatus
        /// </summary>
        public enum ChannelStatus { DOESNOTSUPPORT, SUPPORT}
        #endregion

        #region Product Model 枚举类型

        /// <summary>
        /// 产品模式 0:pu-sku模式，1:pu-配件模式
        /// </summary>
        public enum ProductMode { PU_SKU, PU_PARTS }

        /// <summary>
        /// 产品类型拆单标识 0小件 1大件 2易碎
        /// </summary>
        public enum SplitBlock { SMALL, LARGE, FRAGILE }

        /// <summary>
        /// 图片类型，0PU图 1SKU图
        /// </summary>
        public enum PictureType { PU, SKU }

        /// <summary>
        /// 状态 0待审批/下架 1已上架
        /// </summary>
        public enum OnSaleStatus { NONE, ONSALE }

        /// <summary>
        /// 关联商品标识 0普通 1主商品 2从商品,保留小数 3从商品,四舍五入 4从商品,进位法 5从商品,舍去法
        /// </summary>
        public enum OnSaleDependence { NORMAL, MAIN, DEPEND_PERCISION, DEPEND_ROUND, DEPEND_CARRY, DEPEND_INTPART }

        /// <summary>
        /// 关联类型 类型：0推荐 1买过该商品的人还买过 2
        /// </summary>
        public enum RelationType { SUGGEST, HOTSALE }

        /// <summary>
        /// 状态 0待审批 1审批通过 2驳回/废弃 3已发布
        /// </summary>
        public enum AdjustStatus { NONE, APPROVED, REJECTED, RELEASED }

        #endregion

        #region Purchase Models 枚举类型

        /// <summary>
        /// 采购单状态 0未确定 1已确认 2已结算
        /// </summary>
        public enum PurchaseStatus { NONE, CONFIRMED, CLOSED }

        /// <summary>
        /// 挂起状态 0未挂起 1挂起（问题单） 2变更过程中，页面按钮修订时产生新版本，并设置变更过程中，在修订完成之前不能做任何动作
        /// </summary>
        public enum HangStatus { NONE, HANGED, CHANGEING }

        /// <summary>
        /// 金额计算方式 0从总价计算单价 1从单价计算总价
        /// </summary>
        public enum CalculateMode { FROM_AMOUNT, FROM_PIECE }

        /// <summary>
        /// 历史记录变更原因
        /// </summary>
        public enum HistoryType { NONE, CHANGE, REJECT }

        #endregion

        #region Warehouse Models 枚举类型

        /// <summary>
        /// 入库单状态,0未确认 1已确认
        /// </summary>
        public enum StockInStatus { NONE, CONFIRMED }

        /// <summary>
        /// 出库状态，0未确认 1拣货/扫描中 2已发货 3已签收
        /// </summary>
        public enum StockOutStatus { NONE, SCANNING, DELIVERIED, SIGNED }

        /// <summary>
        /// 移库单状态,0未确认 1已确认
        /// </summary>
        public enum MovingStatus { NONE, CONFIRMED }

        /// <summary>
        /// 移库类型，0移库 1移货位
        /// </summary>
        public enum MovingType { FOR_WAREHOUSE, FOR_SHELF }

        /// <summary>
        /// 盘点状态，0未确认 1已确认 2仓库快照
        /// </summary>
        public enum InventoryStatus { NONE, CONFIRMED, SNAPSHOT }

        #endregion

        #region Order Models 枚举类型

        /// <summary>
        /// 订单状态，0待确认 1已确认 2已排单 3已发货 4已结算 5已取消
        /// </summary>
        public enum OrderStatus { NONE, CONFIRMED, ARRANGED, DELIVERIED, CLOSED,CANCELLED }

        /// <summary>
        /// 锁定状态 0解锁 1锁定 最后修改人即锁定人，锁定后其他人不能修改，管理员可强制解锁
        /// </summary>
        public enum LockStatus { UNLOCK, LOCKED }

        /// <summary>
        /// 付款状态 0未付款 1付款中/货到付款 2通知收款 3已付款
        /// </summary>
        public enum PayStatus { NONE, ONPAYMENT, NOTICE, PAID }

        /// <summary>
        /// 交易类型 0款到发货 1货到付款 2担保交易
        /// </summary>
        public enum TransType { CASH, COD, SECURED }

        /// <summary>
        /// 快递审单结果 0待审核 1通过 2不通过 3不需要审单
        /// </summary>
        public enum ShippingCheck { NONE, PASSED, REJECTED, NOT_REQUIRED }

        /// <summary>
        /// 券状态 0无效 1有效
        /// </summary>
        public enum CouponStatus { NONE, VALID }

        /// <summary>
        /// 促销方案状态　0无效　1有效
        /// </summary>
        public enum PromotionStatus { NONE, VALID }

        /// <summary>
        /// 促销互斥关系 0互斥 1互容
        /// </summary>
        public enum PromotionRelation { CONTRADICT, COMPATIBLE }

        #endregion

        #region Shipping Models 枚举类型

        /// <summary>
        /// 支持语言状态 0无效 1默认值 2其他版本
        /// </summary>
        public enum EnvelopeStatus { NONE, DEFAULT, OTHER }

        #endregion

        #region Mall Model 枚举类型

        /// <summary>
        /// 文章审核状态，0未审核 1审核通过 2审核不通过 3不需要审核
        /// </summary>
        public enum ArticleStatus { NONE, PASSED, REJECTED, FREE }

        /// <summary>
        /// 类别，0降价关注 1缺货通知
        /// </summary>
        public enum FocusProductType { DEDUCTION, STOCKOUT }

        /// <summary>
        /// IP是否禁用，0禁用 1启用
        /// </summary>
        public enum IpStatus { DISABLED,ENABLED}

        #endregion

        #region Finance Model 枚举类型

        /// <summary>
        /// 支付方向 0到积分 1到现金 2到个人银行帐 2到公司银行账
        /// </summary>
        public enum PayDirection { TO_SCORE, TO_CASH, TO_INDIVIDUAL, TO_CORPBANK }

        /// <summary>
        /// 应收应付状态 0待收付  1已收付  2已结算
        /// </summary>
        public enum FinanceStatus { NONE, CONFIRMED, CLOSED }

        #endregion

        #region Exchange 枚举类型

        /// <summary>
        /// 淘宝订单发货类型 0待同步发货 1已同步发货 2不需要同步
        /// </summary>
        public enum TaobaoDeliveryStatus { WAIT_FOR_SEND, ALREADY_SENT, NO_NEED_SEED }

        #endregion
    }

    /// <summary>
    /// 对应于System.Web.MVC中的SelectListItem，在BaseController中转换
    /// </summary>
    public class ListItem
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 显示字符
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}

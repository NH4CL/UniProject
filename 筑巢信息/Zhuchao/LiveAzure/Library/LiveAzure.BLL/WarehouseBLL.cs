using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models;
using LiveAzure.Models.Product;
using LiveAzure.Models.Member;
using LiveAzure.Models.General;
using LiveAzure.Models.Order;

namespace LiveAzure.BLL
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-09-08         -->
    /// <summary>
    /// 仓库BLL类，包括总则核算，入库，出库处理等
    /// </summary>
    public class WarehouseBLL : BaseBLL
    {
        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public WarehouseBLL(LiveEntities entity) : base(entity) { }

        #region 总账核算
        /// <summary>
        /// 按仓库和SKU统计实际入库数和出库数，更新总账和货架库存等
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <param name="skuID">SKU ID</param>
        public void InventoryByWarehouseSku(Guid whID, Guid skuID)
        {
            dbEntity.Database.ExecuteSqlCommand("EXECUTE sp_InventoryByWarehouseSku {0}, {1}", whID, skuID);
        }

        /// <summary>
        /// 重新计算采购单的已入库数量
        /// </summary>
        /// <param name="purchaseID">采购单ID</param>
        public void UpdatePurchaseInQty(Guid purchaseID)
        {
            dbEntity.Database.ExecuteSqlCommand("EXECUTE sp_UpdatePurchaseInQty {0}", purchaseID);
        }
        #endregion

        #region 入库相关逻辑
        /// <summary>
        /// 由采购单生成入库单，成功后返回入库单
        /// </summary>
        /// <param name="purchaseID">采购单ID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="stockIn">输出参数，出库单ID</param>
        /// <returns>返回值 0成功 1采购单状态不正确</returns>
        public int GenerateStockInFromPurchase(Guid purchaseID, Guid? userID, out Guid stockIn)
        {
            int nResultFlag = 9;
            stockIn = Guid.Empty;
            var spResultFlag = dbEntity.Database.SqlQuery<string>("EXECUTE sp_GenerateStockInFromPurchase {0}, {1}", purchaseID, userID).FirstOrDefault();
            if (spResultFlag != null)
                if (spResultFlag != null)
                    if (Guid.TryParse(spResultFlag, out stockIn))
                        nResultFlag = 0;
                    else
                        Int32.TryParse(spResultFlag, out nResultFlag);
            return nResultFlag;
        }

        /// <summary>
        /// 入库单确认，增加库存数量
        /// </summary>
        /// <param name="stockInID">入库单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>返回代码0成功 1入库单不存在或状态不正确 2入库单货位尚未明确 3没有明细数据</returns>
        public int StockInConfirm(Guid stockInID, Guid? userID)
        {
            return dbEntity.Database.SqlQuery<int>("EXECUTE sp_StockInConfirm {0}, {1}", stockInID, userID).FirstOrDefault();
        }

        /// <summary>
        /// 入库单作废，不变库存；只有未确认的入库单才能作废
        /// </summary>
        /// <param name="stockInID">出库单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>返回代码0成功，1入库单状态不正确</returns>
        public int StockInDiscard(Guid stockInID, Guid? userID)
        {
            return dbEntity.Database.SqlQuery<int>("EXECUTE sp_StockInDiscard {0}, {1}", stockInID, userID).FirstOrDefault();
        }
        #endregion

        #region 出库相关逻辑
        /// <summary>
        /// 从订单表中生成出库单，要求订单状态已确认未锁定未挂起，款到发货，则必须已付款；
        /// 一个订单只能生成一个销售类型的出库单，但可作废后重新生成。
        /// 退货/换货可生成多个出库单，RefType类型为退/换货出库
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <param name="userID">当前用户ID，可以为空</param>
        /// <param name="stockOut">输出参数，出库单ID</param>
        /// <returns>返回值：0成功且记录入库单ID，1出库单已经存在，2订单状态不对，3库存不足，9未知错误</returns>
        public int GenerateStockOutFromOrder(Guid orderID, Guid? userID, out Guid stockOut)
        {
            int nResultFlag = 9;
            stockOut = Guid.Empty;
            var spResultFlag = dbEntity.Database.SqlQuery<string>("EXECUTE sp_GenerateStockOutFromOrder {0}, {1}", orderID, userID).FirstOrDefault();
            if (spResultFlag != null)
                if (Guid.TryParse(spResultFlag, out stockOut))
                    nResultFlag = 0;
                else
                    Int32.TryParse(spResultFlag, out nResultFlag);
            return nResultFlag;
        }

        /// <summary>
        /// 出库单确认（即已发货），仅做确认和减库存动作，不检查出库扫描情况，不检查运单情况
        /// </summary>
        /// <param name="stockOutID">出库单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>
        /// 返回代码0成功 1出库单不存在或状态不正确  2订单状态不正确
        ///               3出库单货位尚未明确，即存在货位为Buffer
        ///               4货架不存在，无法确认发货
        ///               5没有明细数据
        /// </returns>
        public int StockOutConfirm(Guid stockOutID, Guid? userID)
        {
            return dbEntity.Database.SqlQuery<int>("EXECUTE sp_StockOutConfirm {0}, {1}", stockOutID, userID).FirstOrDefault();
        }

        /// <summary>
        /// 出库单作废，只有未确认的出库单才能作废，释放占用库存
        /// </summary>
        /// <param name="stockOutID">出库单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>返回代码0成功，1出库单不存在或状态不正确</returns>
        public int StockOutDiscard(Guid stockOutID, Guid? userID)
        {
            return dbEntity.Database.SqlQuery<int>("EXECUTE sp_StockOutDiscard {0}, {1}", stockOutID, userID).FirstOrDefault();
        }

        /// <summary>
        /// 设置消息队列
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="confirmType">提醒类型, 0： 订单确认提醒   1： 订单发货提醒</param>
        /// <param name="OrderID">订单ID</param>
        /// <returns></returns>
        public int DeliveryMessageSet(Guid userID, byte confirmType, Guid OrderID,int DefaultCulture)
        {
            int result = 11;//用户没有订阅信息
            MemberSubscribe oMemberSubscribe = dbEntity.MemberSubscribes.Where(s => s.UserID == userID && s.Deleted == false).FirstOrDefault();//获取用户订阅信息
            ////----test------------
            //if (!SetDeliveryMessage(OrderID, userID, (byte)ModelEnum.MessageType.SMS))
            //    result = 13;//邮件提醒消息队列添加失败
            //else result = 0;
            //--------------------
            if (oMemberSubscribe != null)
            {
                switch (confirmType)//判断提醒类型
                {
                    case 0: if (oMemberSubscribe.OrderConfirm)
                        {//如果设置了订单确认提醒
                            if (oMemberSubscribe.Email)
                            {//getMessage

                            }
                            if (oMemberSubscribe.ShortMessage)
                            {

                            }
                            if (oMemberSubscribe.Bulletin)//简报
                            {

                            }
                        }; break;
                    case 1: if (oMemberSubscribe.OrderDelivery)
                        {//如果设置了订单发货提醒
                            if (oMemberSubscribe.Email)
                            {//有设置邮件提醒
                                if (!SetDeliveryMessage(OrderID, userID, (byte)ModelEnum.MessageType.EMAIL, DefaultCulture))
                                    result = 12;//邮件提醒消息队列添加失败
                            }
                            if (oMemberSubscribe.ShortMessage)
                            { //有设置短信提醒
                                if (!SetDeliveryMessage(OrderID, userID, (byte)ModelEnum.MessageType.SMS, DefaultCulture))
                                    result = 13;//短信提醒消息队列添加失败
                            }
                            if (oMemberSubscribe.Bulletin)
                            {//有设置简报

                            }
                        }
                        ; break;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取发货消息并保存进消息队列
        /// </summary>
        /// <param name="OrderID">订单ID</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="MessageType">消息类型 0：邮件 1：短信</param>
        /// <returns></returns>
        public bool SetDeliveryMessage(Guid OrderID, Guid UserID, byte MessageType, int DefaultCulture)
        {
            bool result = false;//返回值 true 正确 false 错误
            string Message = null;//提醒的消息内容
            string Recipient = null;//消息发送的地址或者手机号
            string Title = null;//消息标题-----------------------------------------------------------------应有资源文件代替
            string EmailCode = "DeliveryEmail";//Email模板代码
            string ShortMessageCode = "DeliverySms";//短信模板代码
            string Code = null;//模板代码
            switch (MessageType)//Code,标题赋值
            {
                case (byte)ModelEnum.MessageType.EMAIL: Code = EmailCode; Title = "ZhuChao's Email"; break;
                case (byte)ModelEnum.MessageType.SMS: Code = ShortMessageCode; Title = "ZhuChao's SMS"; break;
            }
            MemberUser oMemberUser = dbEntity.MemberUsers.Where(u => u.Gid == UserID && u.Deleted == false).FirstOrDefault();//查找出用户
            if (oMemberUser != null)
            {
                GeneralMessageTemplate oMessageTemp = new GeneralMessageTemplate();
                //根据订单组织取出对应的组织消息模板
                oMessageTemp = (from t in dbEntity.GeneralMessageTemplates
                                join o in dbEntity.OrderInformations on t.OrgID equals o.OrgID
                                where o.Gid == OrderID && t.Code == Code && o.Deleted == false && t.Deleted == false
                                select t).FirstOrDefault();
                if (oMessageTemp != null)
                {
                    int culture = oMemberUser.Culture == null? DefaultCulture : oMemberUser.Culture.LCID;//语言
                    Message = oMessageTemp.Matter.GetLargeObject(culture);
                    //替换模板宏信息
                    Message = ReplaceTempMessage(OrderID, Message);
                    if (Message != null)
                    {
                         switch (MessageType)//消息发送的地址赋值 手机/Eamil
                        {
                            case (byte)ModelEnum.MessageType.EMAIL: Recipient = oMemberUser.Email; break;
                            case (byte)ModelEnum.MessageType.SMS: Recipient = oMemberUser.CellPhone; break;
                        }
                        GeneralMessagePending oNewMessagePending = new GeneralMessagePending
                        {
                            UserID = UserID,
                            Mtype = MessageType,
                            Name = oMemberUser.DisplayName,
                            Mstatus = (byte)ModelEnum.MessageStatus.PENDING,//初始化状态为 未发送
                            Recipient = Recipient,
                            Title = Title,
                            Matter = Message,//消息内容
                            RefType = (byte)ModelEnum.NoteType.ORDER,//关联单据类型 订单
                            RefID = OrderID,//关联单据号 订单号
                            //Schedule
                            //SentTime
                            //Remark
                        };
                        try
                        {
                            dbEntity.GeneralMessagePendings.Add(oNewMessagePending);
                            dbEntity.SaveChanges();
                            result = true;
                        }
                        catch
                        { //如果添加有错
                            oEventBLL.WriteEvent("Order:" + OrderID + "| UserID:" + UserID + "| MessagePendingAddingError(消息队列添加失败)");
                            result = false;
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 替换消息模板宏
        /// </summary>
        /// <param name="OrderID">订单ID</param>
        /// <param name="Message">所替换的消息</param>
        /// <returns></returns>
        public string ReplaceTempMessage(Guid OrderID,string Message)
        {
            string result = null;//返回值
            string UserName_temp = "{$user_name}";//待替换的用户名宏
            string OrderCode_temp = "{$order_sn}";//待替换的订单Code宏
            Dictionary<string,string> tempreplace = new Dictionary<string,string>();//待替换的字典序
            OrderInformation oOrder = dbEntity.OrderInformations.Where(o=>o.Deleted==false && o.Gid == OrderID).FirstOrDefault();//找到订单获取订单信息用于替换
            if(oOrder!=null)
            {
                MemberUser oUser = dbEntity.MemberUsers.Where(u => u.Gid == oOrder.UserID&&u.Deleted==false).FirstOrDefault();
                if (oUser != null)
                {
                    tempreplace.Add(UserName_temp, oUser.DisplayName);
                    tempreplace.Add(OrderCode_temp, oOrder.Code);
                    for(int i = 0; i < tempreplace.Count ; i++)
                    {//替换宏
                        Message = Message.Replace(tempreplace.Keys.ElementAt(i), tempreplace.Values.ElementAt(i));
                    }
                    result = Message;
                }
            }
            return result;
        }
        #endregion

        #region 移库/位相关逻辑

        /// <summary>
        /// 移库单确认
        /// </summary>
        /// <param name="movingID">移库/位单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>  返回代码0成功，1移库单不存在或状态不正确，2库存不足
        ///            3移库单货位尚未明确，即存在货位为Buffer
        /// </returns>
        public int MoveingConfirm(Guid movingID, Guid? userID)
        {
            return dbEntity.Database.SqlQuery<int>("EXECUTE sp_MovingConfirm {0}, {1}", movingID, userID).FirstOrDefault();
        }

        /// <summary>
        /// 移库单作废，只有未确认的出库单才能作废，释放占用库存
        /// </summary>
        /// <param name="stockOutID">出库单ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns>返回代码0成功，1出库单不存在或状态不正确</returns>
        public int MovingDiscard(Guid movingID, Guid? userID)
        {
            return -1;
        }

        #endregion

        #region 陆旻添加，通过OnskuID查找可销售库存量

        /// <summary>
        /// 通过OnskuID查找可销售库存量，默认计量单位
        /// </summary>
        /// <param name="onSkuID">上架SKU</param>
        /// <param name="unitGid">计量单位，空表示使用默认计量单位</param>
        /// <returns></returns>
        public decimal GetCansaleQty(Guid onSkuID, Guid? unitGid = null)
        {
            decimal nResult = -1m;
            try
            {
                ProductOnItem oProductOnItem = dbEntity.ProductOnItems.Where(p => p.Gid == onSkuID && p.Deleted == false).FirstOrDefault();
                if (oProductOnItem != null)
                {
                    if (oProductOnItem.MaxQuantity < 0)
                    {
                        decimal nCanSaleQty = dbEntity.WarehouseLedgers.Where(p => p.SkuID == oProductOnItem.SkuID && p.Deleted == false).Sum(p => p.CanSaleQty);

                        //如果存在计量单位，则需要将当前的计量单位转换为库存的标准计量单位
                        if (unitGid != null)
                        {
                            ProductOnUnitPrice oUnitPrice = dbEntity.ProductOnUnitPrices.Include("OnSkuItem.OnSale").Include("ShowUnit").Include("MarketPrice").Include("SalePrice").Where(p => p.OnSkuID == onSkuID && p.aShowUnit == unitGid && p.Deleted == false).FirstOrDefault();
                            //转换后的实际默认计量单位的数量
                            if (oUnitPrice.UnitRatio == 0)
                                oUnitPrice.UnitRatio = 1;
                            decimal nPercent = Decimal.Parse("1" + new string('0', oUnitPrice.Percision));
                            decimal nDefaultQty = Math.Ceiling(nCanSaleQty / oUnitPrice.UnitRatio / nPercent) * nPercent;
                            nResult = nDefaultQty;
                        }
                        else
                        {
                            nResult = nCanSaleQty;
                        }
                    }
                    else
                    {
                        nResult = oProductOnItem.MaxQuantity;
                    }
                }
            }
            catch
            {
                nResult = -1;
            }
            return nResult;
        }

        #endregion

    }
}

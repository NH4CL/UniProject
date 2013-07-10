using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.General;

namespace LiveAzure.BLL
{
    public class BaseBLL : IDisposable
    {
        public LiveEntities dbEntity;                // 数据库连接
        public EventBLL oEventBLL;                   // 事件记录工具

        #region 析构

        /// <summary>
        /// 构造函数，从调用处传数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接实体</param>
        public BaseBLL(LiveEntities entity)
        {
            this.dbEntity = entity;
            this.oEventBLL = new EventBLL(entity);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~BaseBLL()
        {
            Dispose(false);
        }

        /// <summary>
        /// 调用虚拟的Dispose方法。禁止Finalization（终结操作） 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            oEventBLL.Dispose();
        }

        #endregion

        /// <summary>
        /// 获取枚举类型的列表值
        /// </summary>
        /// <param name="oEnumType">枚举类型</param>
        /// <param name="nSelectedIndex">选中的项</param>
        /// <returns>符合MVC页面要求的ListItem列表值（多语言）</returns>
        public List<ListItem> SelectEnumList(Type oEnumType, byte nSelectedIndex)
        {
            List<ListItem> oList = new List<ListItem>();
            bool bSelected = false;     // 是否选中
            string[] sKeys = Enum.GetNames(oEnumType);
            string[] sNames = LiveAzure.Resource.Common.ResourceManager.GetString(oEnumType.Name).Split(',');
            for (byte i = 0; i < sKeys.Length; i++)
            {
                bSelected = (i == nSelectedIndex) ? true : false;
                if (i < sNames.Length)
                    oList.Add(new ListItem { Selected = bSelected, Text = sNames[i].Trim(), Value = i.ToString() });
                else
                    oList.Add(new ListItem { Selected = bSelected, Text = sKeys[i], Value = i.ToString() });
            }
            return oList;
        }

        /// <summary>
        /// 查询所有带货币符合的金额
        /// </summary>
        /// <param name="resID">资源ID</param>
        /// <param name="currency">货币ID</param>
        /// <returns>例如：￥20.23</returns>
        public List<string> GetMoneyList(Guid resID, Guid? currency = null)
        {
            List<string> oMoney;
            if (currency.HasValue)
                oMoney = dbEntity.Database.SqlQuery<string>(
                    @"SELECT mu.Code + CONVERT(nvarchar, rm.Cash, 2) AS UnitCash
                        FROM viewResourceMoney rm
                        JOIN GeneralMeasureUnit mu ON rm.Currency = mu.Gid
                        WHERE rm.Gid = {0} AND rm.Currency = {1}", resID, currency).ToList();
            else
                oMoney = dbEntity.Database.SqlQuery<string>(
                    @"SELECT mu.Code + CONVERT(nvarchar, rm.Cash, 2) AS UnitCash
                        FROM viewResourceMoney rm
                        JOIN GeneralMeasureUnit mu ON rm.Currency = mu.Gid
                        WHERE rm.Gid = {0}", resID).ToList();
            return oMoney;
        }

        /// <summary>
        /// 串联所有带货币符合的金额
        /// </summary>
        /// <param name="resID">资源ID</param>
        /// <param name="currency">货币ID</param>
        /// <returns>例如：￥20.23 $12.45</returns>
        public string GetMoneyString(Guid resID, Guid? currency = null)
        {
            string strResult = "";
            List<string> oMoney = GetMoneyList(resID, currency);
            foreach (string item in oMoney)
                strResult += String.Format("{0} ", item);
            return strResult.Trim();
        }
    }
}

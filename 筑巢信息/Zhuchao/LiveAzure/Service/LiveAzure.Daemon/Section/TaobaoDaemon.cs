using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models;
using LiveAzure.BLL;

namespace LiveAzure.Daemon.Section
{
    /// <!--作者：伯鉴 bojian@zhuchao.com -->
    /// <!--版本：v1.0 2011-10-24         -->
    /// <summary>
    /// 与淘宝接口有关的进程
    /// </summary>
    public class TaobaoDaemon : DaemonBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">数据库连接</param>
        /// <param name="eventBLL">事件记录器</param>
        public TaobaoDaemon(LiveEntities entity, EventBLL eventBLL) : base(entity, eventBLL) { }

        /// <summary>
        /// 淘宝进程主程序
        /// </summary>
        public void Main()
        {
            string sTaobaoConfig = Utility.ConfigHelper.GlobalConst.GetSetting("SynchroTaobao");
            string[] sConfigs = sTaobaoConfig.Split(';');
            foreach (string sOrganChannel in sConfigs)
            {
                string[] sChannelSet = sOrganChannel.Split(':');
                var oOrgan = (from o in dbEntity.MemberOrganizations
                              where o.Deleted == false && o.Otype == (byte)ModelEnum.OrganizationType.CORPORATION
                                    && o.Code == sChannelSet[0]
                              select o).FirstOrDefault();
                var oChannel = (from c in dbEntity.MemberOrgChannels.Include("Organization").Include("Channel")
                                where c.Deleted == false
                                      && c.Organization.Code == sChannelSet[0]
                                      && c.Channel.Code == sChannelSet[1]
                                select c).FirstOrDefault();
                
                // 下载淘宝订单
                LiveAzure.BLL.Taobao.TranscationAPI oTranscation = new BLL.Taobao.TranscationAPI(dbEntity, oChannel);
                oTranscation.DownloadOrders(DateTime.Now.AddDays(-1), DateTime.Now);

                // 同步淘宝价格和库存
                var oOnSales = (from s in dbEntity.ProductOnSales
                                where s.Deleted == false
                                      && s.Ostatus == (byte)ModelEnum.OnSaleStatus.ONSALE
                                      && s.Organization.Code == sChannelSet[0]
                                      && s.Channel.Code == sChannelSet[1]
                                select s).ToList();
                LiveAzure.BLL.Taobao.ProductAPI oProductAPI = new BLL.Taobao.ProductAPI(dbEntity, oChannel);
                foreach (var oOnSale in oOnSales)
                    oProductAPI.ProductUpdate(oOnSale);

                // 转换成Stage订单
                OrderBLL oOrderBLL = new OrderBLL(dbEntity);
                oOrderBLL.GenerateOrderFromTaobao(oOrgan, oChannel.Channel);

                // 下载淘宝评论

                // 下载旺旺记录

            }
        }

    }
}

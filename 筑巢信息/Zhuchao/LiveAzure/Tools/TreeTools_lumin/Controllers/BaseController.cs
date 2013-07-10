using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using LiveAzure.Utility;
using LiveAzure.Models;
using TreeTools_lumin.Models;
using LiveAzure.BLL;
using LiveAzure.Models.General;

namespace TreeTools_lumin.Controllers
{
    /// <summary>
    /// 控制器基类，所有控制器继承自该类
    /// </summary>
    public class BaseController : Controller
    {

        public LiveEntities dbEntity;                     // 数据库连接，全局变量
        public EventBLL oEventBLL;                        // 事件记录工具
        public GeneralBLL oGeneralBLL;                    // 通用业务逻辑
        public static Dictionary<string, string> oProgramNodes = new Dictionary<string, string>();  // 程序节点，功能权限
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseController()
        {
            dbEntity = new LiveEntities(ConfigHelper.LiveConnection.Connection);
            oEventBLL = new EventBLL(dbEntity);
            oGeneralBLL = new GeneralBLL(dbEntity);
        }
        
        /// <summary>
        /// 释放数据库连接
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            dbEntity.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 将ListItem项转换成SelectListItem（MVC使用）
        /// </summary>
        /// <param name="oItems">ListItem原始项</param>
        /// <returns>SelectListItem项</returns>
        public List<SelectListItem> GetSelectList(List<ListItem> oItems)
        {
            List<SelectListItem> oResult = new List<SelectListItem>();
            if (oItems != null)
                foreach (ListItem oItem in oItems)
                    oResult.Add(new SelectListItem { Selected = oItem.Selected, Text = oItem.Text, Value = oItem.Value });
            return oResult;
        }

        /// <summary>
        /// 枚举类型列表
        /// </summary>
        /// <param name="oEnumType">枚举类型</param>
        /// <param name="nSelectedIndex">选中的项</param>
        /// <returns></returns>
        public List<SelectListItem> SelectEnumList(Type oEnumType, byte nSelectedIndex = 0)
        {
            return GetSelectList(oGeneralBLL.SelectEnumList(oEnumType, nSelectedIndex));
        }

        /// <summary>
        /// 枚举类型ture/false
        /// </summary>
        /// <param name="bSelectedIndex">选中的项</param>
        /// <returns></returns>
        public List<SelectListItem> SelectEnumList(bool bSelectedIndex)
        {
            List<SelectListItem> oResult = new List<SelectListItem>();
            if (bSelectedIndex)
            {
                oResult.Add(new SelectListItem { Selected = false, Text = LiveAzure.Resource.Common.No, Value = "false" });
                oResult.Add(new SelectListItem { Selected = true, Text = LiveAzure.Resource.Common.Yes, Value = "true" });
            }
            else
            {
                oResult.Add(new SelectListItem { Selected = true, Text = LiveAzure.Resource.Common.No, Value = "false" });
                oResult.Add(new SelectListItem { Selected = false, Text = LiveAzure.Resource.Common.Yes, Value = "true" });
            }
            return oResult;
        }
             
        /// <summary>
        /// 生成带有root节点的树状结构的Json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string CreateTree(List<LiveTreeNode> list)
        {
            string strTreeJson = "{\"id\": \"00000000-0000-0000-0000-000000000000\", \"name\":\"root\", \"progUrl\":\"\", \"icon\":\"\", \"iconClose\":\"\", \"iconOpen\":\"\", \"isParent\":\"true\","
                + " \"nodeChecked\":false, \"open\":true, \"nodes\":[";

            strTreeJson = CreateTreeJson(list, strTreeJson);

            strTreeJson += "]}";

            return "[" + strTreeJson + "]";
        }

        /// <summary>
        /// 递归生成树节点的JSON字符串
        /// </summary>
        /// <param name="list">treenode的list</param>
        /// <param name="strJson">返回字符串</param>
        /// <returns></returns>
        public string CreateTreeJson(List<LiveTreeNode> list, string strJson)
        {
            string strTreeJson = strJson;

            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                LiveTreeNode treeNode = list.ElementAt(i);

                strTreeJson += "{\"id\": \"" + treeNode.id + "\", \"name\":\"" + treeNode.name + "\", \"progUrl\":\"" + treeNode.progUrl + "\", \"icon\":\""
                    + treeNode.icon + "\", \"iconClose\":\"" + treeNode.iconClose + "\", \"iconOpen\":\"" + treeNode.iconOpen + "\", \"isParent\":\""
                    + treeNode.isParent.ToString().ToLower() + "\", \"nodeChecked\":" + treeNode.nodeChecked.ToString().ToLower()
                    + ", \"nodes\":[";

                if (treeNode.nodes.Count > 0)
                {
                    strTreeJson = CreateTreeJson(treeNode.nodes, strTreeJson);
                }

                strTreeJson += "]";

                if (i == nListCount - 1)
                {
                    strTreeJson += "}";
                }
                else
                {
                    strTreeJson += "},";
                }
            }

            return strTreeJson;

        }

        /// <summary>
        /// 产生多语言的资源文件
        /// </summary>
        /// <param name="rtype">资源类型：字符或金额</param>
        /// <param name="organID">组织，空则表示用系统支持的所有语言</param>
        /// <returns>新资源文件</returns>
        public GeneralResource NewResource(ModelEnum.ResourceType rtype, Guid? organID = null)
        {
            GeneralResource oResource = new GeneralResource();
            oResource.Rtype = (byte)rtype;
            if (rtype == ModelEnum.ResourceType.MONEY)
            {
                List<GeneralMeasureUnit> oUnits = oGeneralBLL.GetSupportCurrencies(organID);
                bool bIsFirst = true;
                foreach (var item in oUnits)
                {
                    if (bIsFirst)
                        oResource.Currency = item.Gid;
                    else
                        oResource.ResourceItems.Add(new GeneralResItem { Currency = item.Gid });
                    bIsFirst = false;
                }
            }
            else
            {
                List<GeneralCultureUnit> oCultures = oGeneralBLL.GetSupportCultures(organID);
                bool bIsFirst = true;
                foreach (var item in oCultures)
                {
                    if (bIsFirst)
                        oResource.Culture = item.Culture;
                    else
                        oResource.ResourceItems.Add(new GeneralResItem { Culture = item.Culture });
                    bIsFirst = false;
                }
            }
            return oResource;
        }

        /// <summary>
        /// 更新已经存在的资源文件，包括插入新语言/货币，删除过期的语言/货币等
        /// </summary>
        /// <param name="rtype">资源类型：字符，金额</param>
        /// <param name="resource">原资源文件</param>
        /// <param name="organID">组织ID，空表示用系统支持的语言/货币刷新</param>
        /// <returns>新资源文件</returns>
        public GeneralResource RefreshResource(ModelEnum.ResourceType rtype, GeneralResource resource, Guid? organID = null)
        {
            GeneralResource oResource = resource;
            if (oResource == null)
                oResource = this.NewResource(rtype, organID);
            oResource.Rtype = (byte)rtype;
            List<Guid> oGuidList = new List<Guid>();
            if (rtype == ModelEnum.ResourceType.MONEY)
            {
                List<GeneralMeasureUnit> oUnits = oGeneralBLL.GetSupportCurrencies(organID);
                bool bIsFirst = true;
                foreach (var item in oUnits)
                {
                    if (bIsFirst)
                    {
                        oResource.Currency = item.Gid;
                    }
                    else
                    {
                        var resitem = oResource.ResourceItems.FirstOrDefault(i => i.Currency == item.Gid);
                        if (resitem == null)
                            oResource.ResourceItems.Add(new GeneralResItem { Currency = item.Gid });
                        else
                            oGuidList.Add(resitem.Gid);
                    }
                    bIsFirst = false;
                }
            }
            else
            {
                List<GeneralCultureUnit> oCultures = oGeneralBLL.GetSupportCultures(organID);
                bool bIsFirst = true;
                foreach (var item in oCultures)
                {
                    if (bIsFirst)
                    {
                        oResource.Culture = item.Culture;
                    }
                    else
                    {
                        var resitem = oResource.ResourceItems.FirstOrDefault(i => i.Culture == item.Culture);
                        if (resitem == null)
                            oResource.ResourceItems.Add(new GeneralResItem { Culture = item.Culture });
                        else
                            oGuidList.Add(resitem.Gid);
                    }
                    bIsFirst = false;
                }
            }
            // 删除过时的语言资源
            for (int i = 0; i < oResource.ResourceItems.Count; i++)
            {
                var item = oResource.ResourceItems.ElementAt(i);
                if (!item.Gid.Equals(Guid.Empty) && !oGuidList.Contains(item.Gid))
                    oResource.ResourceItems.Remove(item);
            }
            return oResource;
        }

    }
}

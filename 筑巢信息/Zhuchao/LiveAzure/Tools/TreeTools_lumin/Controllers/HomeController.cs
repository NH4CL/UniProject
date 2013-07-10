using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TreeTools_lumin.Models;
using LiveAzure.Models;
using LiveAzure.Utility;

namespace TreeTools_lumin.Controllers
{
    public class HomeController : Controller
    {
        private LiveEntities db = new LiveEntities(ConfigHelper.LiveConnection.Connection);

        public ActionResult Index()
        {
            ViewBag.Message = "欢迎使用 ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public string testJson()
        {
            List<TreeNode> childList = new List<TreeNode>();

            for (int i = 0; i < 1; i++)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.id = "Id1" + i.ToString();
                treeNode.name = "Name1" + i.ToString();                
                treeNode.progUrl = "Url1" + i.ToString();
                treeNode.icon = "Icon1" + i.ToString();
                treeNode.iconClose = "IconClose1" + i.ToString();
                treeNode.iconOpen = "IconOpen1" + i.ToString();
                treeNode.isParent = false;
                treeNode.nodeChecked = true;
                treeNode.target = "Target1" + i.ToString();
                treeNode.nodes = new List<TreeNode>();

                childList.Add(treeNode);
            }

            List<TreeNode> list = new List<TreeNode>();

            for (int i = 0; i < 2; i++)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.id = "Id" + i.ToString();
                treeNode.name = "Name" + i.ToString();
                treeNode.progUrl = "Url" + i.ToString();
                treeNode.icon = "Icon" + i.ToString();
                treeNode.iconClose = "IconClose" + i.ToString();
                treeNode.iconOpen = "IconOpen" + i.ToString();
                treeNode.isParent = false;
                treeNode.nodeChecked = true;
                treeNode.target = "Target" + i.ToString();
                treeNode.nodes = childList;

                list.Add(treeNode);
            }

            string test = CreateTreeJson(list, "");

            return "";
        }

        /// <summary>
        /// 生成带有root节点的树状结构的Json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string CreateTree(List<TreeTools_lumin.Models.TreeNode> list)
        {
            string strTreeJson = "{\"id\": \"root\", \"name\":\"root\", \"url\":\"\", \"icon\":\"\", \"iconClose\":\"\", \"iconOpen\":\"\", \"isParent\":\"true\","
                + "\"checkedCol\":\"nodeChecked\", \"nodeChecked\":\"true\", \"target\":\" \", \"open\":\"true\", \"nodes\":[";

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
        public string CreateTreeJson(List<TreeTools_lumin.Models.TreeNode> list, string strJson)
        {
            string strTreeJson = strJson;

            int nListCount = list.Count;

            for (int i = 0; i < nListCount; i++)
            {
                TreeTools_lumin.Models.TreeNode treeNode = list.ElementAt(i);

                strTreeJson += "{\"id\": \"" + treeNode.id + "\", \"name\":\"" + treeNode.name + "\", \"url\":\"" + treeNode.progUrl + "\", \"icon\":\""
                    + treeNode.icon + "\", \"iconClose\":\"" + treeNode.iconClose + "\", \"iconOpen\":\"" + treeNode.iconOpen + "\", \"isParent\":\""
                    + treeNode.isParent.ToString().ToLower() + "\", \"checkedCol\":\"" + treeNode.checkedCol + "\", \"nodeChecked\":\"" + treeNode.nodeChecked.ToString().ToLower()
                    + "\", \"target\":\"" + treeNode.target + "\", \"nodes\":[";

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

    }
}

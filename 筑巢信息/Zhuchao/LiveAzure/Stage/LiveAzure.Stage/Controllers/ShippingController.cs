using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Models.Shipping;
using LiveAzure.BLL;
using System.Data;

namespace LiveAzure.Stage.Controllers
{
    public class ShippingController : BaseController
    {
        //
        // GET: /Shipping/

        public static Guid gShipperID;

        /// <summary>
        /// 运输公司面单添加和编辑页面
        /// </summary>
        /// <param name="shippingGid">运输公司的Gid</param>
        /// <returns></returns>
        public ActionResult Index(Guid? shippingGid)
        {
            // 权限验证
            string strProgramCode = Request.RequestContext.RouteData.Values["Controller"].ToString() +
                Request.RequestContext.RouteData.Values["Action"].ToString();
            if (!base.Permission(strProgramCode))
                return RedirectToAction("ErrorPage", "Home", new { LiveAzure.Resource.Common.NoPermission });

            //运输公司的下拉列表
            List<SelectListItem> oList = new List<SelectListItem>();
            List<ShippingInformation> oShippers = dbEntity.ShippingInformations.Where(o=>o.Otype==(byte)ModelEnum.OrganizationType.SHIPPER).ToList();
            foreach (ShippingInformation item in oShippers)
            {
                oList.Add(new SelectListItem { Text = item.FullName.GetResource(CurrentSession.Culture), Value = item.Gid.ToString() });
            }
            ViewBag.shipperList = oList;

            //shippingGid为空表示第一次进入页面 ，不为空表示更变运输公司后重新加载页面
            if (shippingGid == null)
            {
                ShippingEnvelope oEnvelope = new ShippingEnvelope
                {
                    Template = NewLargeObject(CurrentSession.OrganizationGID)
                };
                return View(oEnvelope);
            }
            else
            {
                //oEnvelope为空表示此运输公司还没有模板，不为空表示已经有了模板
                ShippingEnvelope oEnvelope = dbEntity.ShippingEnvelopes.Where(o => o.ShipID == shippingGid).SingleOrDefault();
                if (oEnvelope != null)
                {
                    oEnvelope.Template = RefreshLargeObject(oEnvelope.Template, CurrentSession.OrganizationGID);
                    return View(oEnvelope);
                }
                else
                {
                    ShippingEnvelope newEnvelope = new ShippingEnvelope
                    {
                        ShipID = (Guid)shippingGid,
                        Template = NewLargeObject(CurrentSession.OrganizationGID)
                    };
                    return View(newEnvelope);
                }
            }
        }
        
        public ActionResult Region(Guid? ShipId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (ShipId == null)
            {
                //获得当前用户的ID
                Guid guserid = (Guid)CurrentSession.UserID;
                //该用户有权限查看的组织列表
                var oOrgPriv = (from o in dbEntity.MemberPrivileges
                                from p in dbEntity.MemberPrivItems
                                where (o.Gid == p.PrivID && o.Ptype == 2 && o.Deleted == false && p.Deleted == false)
                                select p).ToList();
                //MemberUsers中读出该用户所属的组织ID
                Guid gOrg = (from o in dbEntity.MemberUsers
                             where (o.Gid == guserid && o.Deleted == false)
                             select o.OrgID).Single();
                //该用户所属组织详细信息
                MemberOrganization oMemOrg = (from o in dbEntity.MemberOrganizations.Include("ShortName")
                                              where (o.Gid == gOrg && o.Deleted == false)
                                              select o).Single();
                int nOrgPriv = oOrgPriv.Count;

                //该用户只有权限查看自己的组织
                if (nOrgPriv == 0)
                {
                    //默认组织下支持的承运商
                    var oOrgShip = (from o in dbEntity.ShippingInformations.Include("Parent")
                                    where (o.Parent.Gid == gOrg && o.Deleted == false)
                                    select o).ToList();

                    foreach (var Shipitem in oOrgShip)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = Shipitem.ShortName.GetResource(CurrentSession.Culture);
                        item.Value = Shipitem.Gid.ToString();
                        if (Shipitem.Gid == gShipperID) item.Selected = true;
                        list.Add(item);
                    }
                }
                //该用户有权查看其他组织
                else
                {
                    for (int i = 0; i < nOrgPriv; i++)
                    {
                        Guid gorgid = (Guid)oOrgPriv[i].RefID;

                        var oOrgShip = (from o in dbEntity.ShippingInformations.Include("Parent").Include("ShortName")
                                        where (o.Parent.Gid == gorgid && o.Deleted == false)
                                        select o).ToList();
                        foreach (var vitem in oOrgShip)
                        {
                            SelectListItem oitem = new SelectListItem
                            {
                                Text = vitem.ShortName.GetResource(CurrentSession.Culture),
                                Value = vitem.Gid.ToString()
                            };
                            if (vitem.Gid == gShipperID) oitem.Selected = true;
                            if (list.Contains(oitem) == false) list.Add(oitem);
                        }
                    }
                    //默认组织下支持的承运商
                    var oMemOrgShip = (from o in dbEntity.ShippingInformations.Include("Parent").Include("ShortName")
                                       where (o.Parent.Gid == gOrg && o.Deleted == false)
                                       select o).ToList();
                    foreach (var Shipitem in oMemOrgShip)
                    {
                        SelectListItem sitem = new SelectListItem();

                        sitem.Text = Shipitem.ShortName.GetResource(CurrentSession.Culture);
                        sitem.Value = Shipitem.Gid.ToString();
                        if (Shipitem.Gid == gShipperID) sitem.Selected = true;
                        if (list.Contains(sitem) == false) list.Add(sitem);
                    }
                }

            }
            else
            {
                GetID((Guid)ShipId);
                ShippingInformation oShippingInformation = (from o in dbEntity.ShippingInformations
                                                            where (o.Deleted == false && o.Gid == ShipId)
                                                            select o).Single();

                SelectListItem item = new SelectListItem();
                item.Text = oShippingInformation.ShortName.GetResource(CurrentSession.Culture);
                item.Value = oShippingInformation.Gid.ToString();
                item.Selected = true;
                list.Add(item);
            }
            ViewBag.olist = list;
            return View();
        }

        /// <summary>
        /// 获得承运商ID
        /// </summary>
        /// <param name="id"></param>
        public void GetID(Guid id)
        {
            gShipperID = id;
        }

        public ActionResult RegionTree()
        {
            return View();
        }

        /// <summary>
        /// 承运商支持的地区树状结构
        /// </summary>
        /// <returns></returns>
        public string RegionTreeLoad()
        {
            string strTreeJson = "";
            if (!gShipperID.Equals(Guid.Empty))
            {
                //首次加载的树节点
                var oRegion = (from o in dbEntity.GeneralRegions
                               where (o.Parent == null && o.Deleted == false)
                               select o).ToList();
                int nRegion = oRegion.Count;
                //该承运商已经支持的地区
                var oShipRegion = (from o in dbEntity.ShippingAreas
                                   where (o.ShipID == gShipperID && o.Deleted == false)
                                   select o).ToList();
                int nShipRegion = oShipRegion.Count;

                bool flag = false;

                List<LiveTreeNode> list = new List<LiveTreeNode>();
                foreach (var item in oRegion)
                {
                    for (int i = 0; i < nShipRegion; i++)
                    {
                        if (item.Gid == oShipRegion[i].RegionID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    LiveTreeNode TreeNode = new LiveTreeNode();
                    TreeNode.id = item.Gid.ToString();
                    TreeNode.name = item.ShortName;
                    if (flag == true) TreeNode.nodeChecked = true;
                    else TreeNode.nodeChecked = false;
                    if (item.ChildItems.Count > 0) TreeNode.isParent = true;
                    else TreeNode.isParent = false;
                    TreeNode.nodes = new List<LiveTreeNode>();
                    list.Add(TreeNode);
                    flag = false;
                }
                strTreeJson = CreateTree(list);
            }
            return strTreeJson;
        }

        /// <summary>
        /// 展开树节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string RegionTreeExpand(Guid? id)
        {
            List<LiveTreeNode> list = new List<LiveTreeNode>();
            if (gShipperID != Guid.Empty)
            {
                //展开root
                if (id.Equals(Guid.Empty) || id == null)
                {
                    //加载父亲为空的地区
                    var oRegion = (from o in dbEntity.GeneralRegions
                                   where (o.Parent == null && o.Deleted == false)
                                   select o).ToList();
                    int nRegion = oRegion.Count;
                    //该承运商已经支持的地区
                    var oShipRegion = (from o in dbEntity.ShippingAreas
                                       where (o.ShipID == gShipperID && o.Deleted == false)
                                       select o).ToList();
                    int nShipRegion = oShipRegion.Count;

                    bool flag = false;
                    foreach (var item in oRegion)
                    {
                        for (int i = 0; i < nShipRegion; i++)
                        {
                            if (item.Gid == oShipRegion[i].RegionID)
                            {
                                flag = true;
                                break;
                            }
                        }
                        LiveTreeNode TreeNode = new LiveTreeNode();
                        TreeNode.id = item.Gid.ToString();
                        TreeNode.name = item.ShortName;
                        if (flag == true) TreeNode.nodeChecked = true;
                        else TreeNode.nodeChecked = false;
                        if (item.ChildItems.Count > 0) TreeNode.isParent = true;
                        else TreeNode.isParent = false;
                        TreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(TreeNode);
                    }
                }
                else
                {
                    Guid gid = (Guid)id;
                    //展开非root节点
                    var oRegion = (from o in dbEntity.GeneralRegions
                                   where (o.Parent.Gid == gid && o.Deleted == false)
                                   select o).ToList();
                    //该承运商已经支持的地区
                    var oShipRegion = (from o in dbEntity.ShippingAreas
                                       where (o.ShipID == gShipperID && o.Deleted == false)
                                       select o).ToList();
                    int nShipRegion = oShipRegion.Count;

                    bool flag = false;
                    foreach (var item in oRegion)
                    {
                        for (int i = 0; i < nShipRegion; i++)
                        {
                            if (item.Gid == oShipRegion[i].RegionID)
                            {
                                flag = true;
                                break;
                            }
                        }
                        LiveTreeNode TreeNode = new LiveTreeNode();
                        TreeNode.id = item.Gid.ToString();
                        TreeNode.name = item.ShortName;
                        if (flag == true) TreeNode.nodeChecked = true;
                        else TreeNode.nodeChecked = false;
                        if (item.ChildItems.Count > 0) TreeNode.isParent = true;
                        else TreeNode.isParent = false;
                        TreeNode.nodes = new List<LiveTreeNode>();
                        list.Add(TreeNode);
                        flag = false;
                    }
                }
            }
            return list.ToJsonString();

        }

        /// <summary>
        /// checkbox勾选中后保存到数据库
        /// </summary>
        /// <param name="id">选中的地区ID</param>
        /// <returns></returns>
        public void AddRegion(Guid id)
        {
            var oRegion = (from o in dbEntity.ShippingAreas
                          where (o.RegionID == id && o.ShipID == gShipperID)
                          select o).FirstOrDefault();
            if (oRegion == null)
            {
                ShippingArea oShippingArea = new ShippingArea();
                
                oShippingArea.ShipID = gShipperID;
                oShippingArea.RegionID = id;

                
                dbEntity.ShippingAreas.Add(oShippingArea);
                dbEntity.SaveChanges();
            }
            else
            {
                oRegion.Deleted = false;
                dbEntity.SaveChanges();
            }
            //return View();
        }

        /// <summary>
        /// checkbox取消选中时，数据库中逻辑删除
        /// </summary>
        /// <param name="id">要删除的地区ID</param>
        /// <returns></returns>
        public void DeleteRegion(Guid id)
        {
            ShippingArea oShippingArea = (from o in dbEntity.ShippingAreas
                                         where (o.ShipID == gShipperID && o.RegionID == id && o.Deleted == false)
                                         select o).Single();
            oShippingArea.Deleted = true;
            dbEntity.SaveChanges();
            //return View();
        }

        /// <summary>
        /// 配送区域属性设置
        /// </summary>
        /// <returns></returns>
        public ActionResult RegionAttribute(Guid id)
        {

            ShippingArea oShipArea = new ShippingArea();
            oShipArea = (from o in dbEntity.ShippingAreas.Include("Residential").Include("LiftGate")
                             .Include("Installation").Include("PriceWeight").Include("PriceVolume")
                             .Include("PricePiece").Include("PriceHigh").Include("PriceLow")
                        where (o.Deleted == false && o.RegionID == id && o.ShipID == gShipperID)
                        select o).Single();

            ViewData["oSupport"] = SelectEnumList(oShipArea.SupportCod);

            oShipArea.Residential = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.Residential);
            oShipArea.LiftGate = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.LiftGate);
            oShipArea.Installation = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.Installation);
            oShipArea.PriceWeight = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.PriceWeight);
            oShipArea.PriceVolume = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.PriceVolume);
            oShipArea.PricePiece = RefreshResource(ModelEnum.ResourceType.MONEY,oShipArea.PricePiece);
            oShipArea.PriceHigh = RefreshResource(ModelEnum.ResourceType.MONEY, oShipArea.PriceHigh);
            oShipArea.PriceLow= RefreshResource(ModelEnum.ResourceType.MONEY, oShipArea.PriceLow);

            return View("RegionAttribute",oShipArea);
        }

        [HttpPost]
        public ActionResult RegionAttribute(ShippingArea shipArea)
        {
            ShippingArea oShippingArea = (from o in dbEntity.ShippingAreas
                                         where (o.Gid == shipArea.Gid && o.Deleted == false)
                                         select o).Single();

            ViewData["oSupport"] = SelectEnumList(shipArea.SupportCod);
            oShippingArea.Residential = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.LiftGate = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.Installation = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.PriceWeight = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.PriceVolume = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.PricePiece = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.PriceHigh = NewResource(ModelEnum.ResourceType.MONEY);
            oShippingArea.PriceLow = NewResource(ModelEnum.ResourceType.MONEY);

            oShippingArea.ShipID = shipArea.ShipID;
            oShippingArea.RegionID = shipArea.RegionID;
            oShippingArea.SupportCod = shipArea.SupportCod;


            oShippingArea.Residential.SetResource(ModelEnum.ResourceType.MONEY, shipArea.Residential);
            oShippingArea.LiftGate.SetResource(ModelEnum.ResourceType.MONEY, shipArea.LiftGate);
            oShippingArea.Installation.SetResource(ModelEnum.ResourceType.MONEY, shipArea.Installation);
            oShippingArea.PriceWeight.SetResource(ModelEnum.ResourceType.MONEY, shipArea.PriceWeight);
            oShippingArea.PriceVolume.SetResource(ModelEnum.ResourceType.MONEY, shipArea.PriceVolume);
            oShippingArea.PricePiece.SetResource(ModelEnum.ResourceType.MONEY, shipArea.PricePiece);
            oShippingArea.PriceHigh.SetResource(ModelEnum.ResourceType.MONEY, shipArea.PriceHigh);
            oShippingArea.PriceLow.SetResource(ModelEnum.ResourceType.MONEY, shipArea.PriceLow);

            oShippingArea.Remark = shipArea.Remark;

            dbEntity.SaveChanges();
            return RedirectToAction("Region");
        }

        /// <summary>
        /// 保存面单模板
        /// </summary>
        /// <param name="oEnvelope"></param>
        public void SaveEnvelopeTemplate(ShippingEnvelope oEnvelope)
        {
            ShippingEnvelope oldEnelope = dbEntity.ShippingEnvelopes.Where(o => o.ShipID == oEnvelope.ShipID && o.Code == oEnvelope.Code).SingleOrDefault();      //查出此运输公司是否已经有模板
            if (oldEnelope == null)         //没模板，添加操作
            {
                oEnvelope.Template = new GeneralLargeObject(oEnvelope.Template);
                dbEntity.ShippingEnvelopes.Add(oEnvelope);
                dbEntity.SaveChanges();
            }
            else                  //有模板，更新操作
            {
                oEnvelope.Template.SetLargeObject(oEnvelope.Template);
                if (ModelState.IsValid)
                {
                    dbEntity.Entry(oldEnelope).State = EntityState.Modified;
                    dbEntity.SaveChanges();
                }
            }
        }
    }
}

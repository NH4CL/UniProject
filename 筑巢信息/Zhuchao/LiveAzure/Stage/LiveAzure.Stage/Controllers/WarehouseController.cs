using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveAzure.Models;
using LiveAzure.Models.Member;
using LiveAzure.Models.Warehouse;
using MVC.Controls;
using MVC.Controls.Grid;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Transactions;
using System.Text;
using LiveAzure.Models.Shipping;
using LiveAzure.Models.General;
using LiveAzure.Controls.LiveRegionSelector;
using LiveAzure.Models.Order;
using LiveAzure.Models.Product;
using System.Web.Script.Serialization;
using LiveAzure.Models.Purchase;
using LiveAzure.Models.Exchange;
using System.Globalization;
using LiveAzure.BLL;
using System.Windows.Forms;
using System.Drawing;
using LiveAzure.Stage.Helpers;

namespace LiveAzure.Stage.Controllers
{
    public class WarehouseController : BaseController
    {
        public WarehouseBLL whBll;
        public static Guid OrgSelected =Guid.Empty;//当前选中的组织
        public static Guid WarehouseSelected = Guid.Empty;//当前选中的仓库
        #region 库存总账
        /// <summary>
        /// 仓库总账页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            return View();
        }
        /// <summary>
        /// 编辑仓库总账的几个字段
        /// </summary>
        /// <param name="ledgerID">要编辑的仓库总账的GUID</param>
        /// <returns></returns>
        public ActionResult LedgerEdit(Guid ledgerID)
        {
            if (!base.CheckPrivilege("EnableEdit"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            string EnableEdit = "0";
            try
            {
                EnableEdit = CurrentSession.IsAdmin ? "1" : oProgramNodes["EnableEdit"];
            }
            catch (KeyNotFoundException)//捕捉到oProgramNodes没有Key 说明没有授权该节点
            {
                EnableEdit = "0";
            }
            if (EnableEdit == "0")
            {
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege" });
            }
            WarehouseLedger model = dbEntity.WarehouseLedgers.Find(ledgerID);
            if (model == null|| model.Deleted)
            {
                return Error("记录不存在", Url.Action("Index"));
            }
            return View(model);
        }
        /// <summary>
        /// 将编辑的仓库总账的结果存储到数据库
        /// </summary>
        /// <param name="model">被编辑的仓库总账</param>
        /// <returns></returns>
        public ActionResult LedgerEditDB(WarehouseLedger model)
        {
            if (!base.CheckPrivilege("EnableEdit"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseLedger ledger = dbEntity.WarehouseLedgers.Find(model.Gid);
            if (ledger == null || ledger.Deleted)
            {
                return Error("记录不存在", Url.Action("Index"));
            }
            ledger.MaxQty = model.MaxQty;
            ledger.LockQty = model.LockQty;
            ledger.SafeQty = model.SafeQty;
            dbEntity.SaveChanges();
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 仓库列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult WarehouseLedgerList(SearchModel searchModel, Guid? whID = null, string code ="", string name ="")
        {
            List<WarehouseLedger> warehouseLedgers = (from ledger in dbEntity.WarehouseLedgers.ToList()
                                                join gid in Permission(ModelEnum.UserPrivType.WAREHOUSE) on ledger.WhID equals gid
                                                where (whID == null) ? ledger.WhID==WarehouseSelected : ledger.WhID == whID
                                                && ledger.SkuItem.Code.IndexOf(code, StringComparison.OrdinalIgnoreCase) > -1
                                                && ledger.SkuItem.FullName.Matter.IndexOf(name, StringComparison.OrdinalIgnoreCase) > -1
                                                && !ledger.Deleted
                                                orderby ledger.LastModifyTime
                                                select ledger).ToList();
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseLedger> columns = new GridColumnModelList<WarehouseLedger>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuItem.Barcode);
            columns.Add(p => p.SkuItem.Code);
            columns.Add(p => p.SkuItem.FullName.GetResource(culture)).SetName("SkuID");
            columns.Add(p => p.InQty);
            columns.Add(p => p.OutQty);
            columns.Add(p => p.RealQty);
            columns.Add(p => p.LockQty);
            columns.Add(p => p.CanSaleQty);
            columns.Add(p => p.CanDelivery);
            columns.Add(p => p.TobeDelivery);
            columns.Add(p => p.Arranged);
            columns.Add(p => p.Presale);
            columns.Add(p => p.Ontheway);
            columns.Add(p => p.SafeQty);
            columns.Add(p => p.MaxQty);
            columns.Add(p => p.AverageCost != null ? p.AverageCost.GetResource(CurrentSession.Currency.Value).ToString():"0").SetName("AverageCost");
            GridData gridData = warehouseLedgers.AsQueryable().ToGridData(searchModel,columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 查看总账SKU的详情
        /// </summary>
        /// <param name="ledgerID">要查看的仓库总账的GUID</param>
        /// <returns></returns>
        public ViewResult SKUDetail(Guid ledgerID)
        {
            WarehouseLedger ledger = dbEntity.WarehouseLedgers.Find(ledgerID);
            if (ledger == null || ledger.Deleted)
            {
                return Error("记录不存在", Url.Action("Index"));
            }
            IEnumerable<WarehouseSkuShelf> skuShelves = (from s in dbEntity.WarehouseSkuShelves
                                                        where s.WhID == ledger.WhID
                                                           && s.SkuID == ledger.SkuID
                                                           && !s.Deleted
                                                        orderby s.LastModifyTime
                                                        select s).ToList();
            return View(skuShelves);
        }
        #endregion 库存总账

        #region 货架
        /// <summary>
        /// 货架定义页面
        /// </summary>
        /// <returns>货架View</returns>
        public ActionResult Shelf()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            ViewBag.privEnableEditShelf = GetProgramNode("EnableEditShelf");//允许定义货架
            return View();
        }
        /// <summary>
        /// 返回货架Grid的部分页面
        /// </summary>
        /// <param name="whID">货架对应的仓库的GUID</param>
        /// <returns>包含货架Grid的部分页</returns>
        public ActionResult ShelfGrid(Guid? whID)
        {
            WarehouseInformation model = (from w in dbEntity.WarehouseInformations
                                          where w.Gid == whID
                                             && !w.Deleted
                                          select w).SingleOrDefault();
            if (model == null)
            {
                return Error("记录不存在", Url.Action("Shelf"));
            }
            return PartialView(model);
        }
        /// <summary>
        /// 仓库中的货架
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="whID">仓库的Gid</param>
        /// <param name="code">用于搜索的Code</param>
        /// <param name="barcode">用于搜索的Barcode</param>
        /// <param name="name">用于搜索的Name</param>
        /// <returns>返回一个仓库里所有货架</returns>
        public JsonResult ShelfList(SearchModel searchModel, Guid? whID = null, string code = "", string barcode = "", string name = "")
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            IEnumerable<WarehouseShelf> WHShelves = (from item in dbEntity.WarehouseShelves.ToList()
                                                     where item.WhID == whID
                                                        && item.Code.IndexOf(code, StringComparison.OrdinalIgnoreCase) > -1
                                                        && item.Barcode.IndexOf(barcode, StringComparison.OrdinalIgnoreCase) > -1
                                                        && item.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) > -1
                                                        && !item.Deleted
                                                     select item).OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseShelf> columns = new GridColumnModelList<WarehouseShelf>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Barcode);
            columns.Add(p => p.Name);
            columns.Add(p => p.Reserved);
            columns.Add(p => p.ShelfQuantity);
            columns.Add(p => p.Brief);
            columns.Add(p => p.Remark);
            GridData gridData = WHShelves.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加货架
        /// </summary>
        /// <param name="whID">仓库Id</param>
        /// <returns></returns>
        public ActionResult ShelfAdd(Guid whID)
        {
            if (!base.CheckPrivilege("EnableEditShelf"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation warehouse = (from w in dbEntity.WarehouseInformations.Include("FullName.ResourceItems")
                                              where w.Gid == whID && !w.Deleted
                                              select w).FirstOrDefault();
            if (warehouse == null)
            {
                return Error("仓库不存在", Url.Action("Shelf"));
            }
            WarehouseShelf model = new WarehouseShelf
            {
                WhID = whID,
                Warehouse = warehouse
            };
            return View(model);
        }
        /// <summary>
        /// 编辑货架页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ShelfEdit(Guid shelfID)
        {
            if (!base.CheckPrivilege("EnableEditShelf"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseShelf model = (from shelf in dbEntity.WarehouseShelves.Include("Warehouse.FullName.ResourceItems")
                                    where shelf.Gid == shelfID && !shelf.Deleted
                                    select shelf).FirstOrDefault();
            if (model == null)
                return Error("记录不存在", Url.Action("Shelf"));
            else
                return View(model);
        }
        /// <summary>
        /// 添加货架
        /// </summary>
        /// <param name="model">从View接收到的货架对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShelfAddDB(WarehouseShelf model)
        {
            if (!base.CheckPrivilege("EnableEditShelf"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseShelf newShelf = (from shelf in dbEntity.WarehouseShelves
                                       where shelf.WhID == model.WhID
                                          && shelf.Code == model.Code
                                       select shelf).FirstOrDefault();
            if (newShelf == null)
            {
                newShelf = new WarehouseShelf
                {
                    WhID = model.WhID,
                    Code = model.Code,
                    Barcode = model.Barcode,
                    Name = model.Name,
                    Reserved = model.Reserved,
                    Brief = model.Brief,
                    Remark = model.Remark
                };
                dbEntity.WarehouseShelves.Add(newShelf);
            }
            else if (newShelf.Deleted)
            {
                newShelf.Deleted = false;
                newShelf.Name = model.Name;
                newShelf.Barcode = model.Barcode;
                newShelf.Brief = model.Brief;
                newShelf.Remark = model.Remark;
            }
            else
            {
                return Error("输入数据存在冲突（待改进d(>_<)b）", Url.Action("Shelf"));
            }
            dbEntity.SaveChanges();
            return RedirectToAction("Shelf");
        }
        /// <summary>
        /// 编辑货架
        /// </summary>
        /// <param name="model">从View接收的货架对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShelfEditDB(WarehouseShelf model)
        {
            if (!base.CheckPrivilege("EnableEditShelf"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseShelf shelf = dbEntity.WarehouseShelves.Find(model.Gid);
            if (shelf == null || shelf.Deleted)
            {
                return Error("记录不存在", Url.Action("Shelf"));
            }
            else
            {
                if (shelf.WhID != model.WhID)
                {
                    return Error("记录异常", Url.Action("Shelf"));
                }
                shelf.Code = model.Code;
                shelf.Barcode = model.Barcode;
                shelf.Brief = model.Brief;
                shelf.Reserved = model.Reserved;
                shelf.Remark = model.Remark;
                dbEntity.SaveChanges();
                return RedirectToAction("Shelf");
            }
        }
        /// <summary>
        /// 删除货架
        /// </summary>
        /// <param name="shelfID">货架Id</param>
        /// <returns>返回货架查看页面</returns>
        [HttpPost]
        public JsonResult ShelfDeleteDB(Guid shelfID)
        {
            if (!base.CheckPrivilege("EnableDelete"))
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            WarehouseShelf temp = dbEntity.WarehouseShelves.Find(shelfID);
            if (temp != null)
            {
                temp.Deleted = true;
                dbEntity.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Excel数据导入
        /// </summary>
        /// <param name="uploadFile">上传的文件</param>
        /// <returns></returns>
        public ActionResult UpdateXls(HttpPostedFileBase uploadFile, Guid WhlID)
        {
            #region 处理上传Excel文件
            if (uploadFile == null || uploadFile.ContentLength == 0)
                return RedirectToAction("Error");
            string ServerTempPath = HttpContext.Server.MapPath("~/Temp");
            if (!Directory.Exists(ServerTempPath))
                Directory.CreateDirectory(ServerTempPath);
            string OldFileName = Path.GetFileName(uploadFile.FileName);
            string extension = Path.GetExtension(OldFileName);
            string NewFileName = Guid.NewGuid() + extension;
            string filePath = Path.Combine(ServerTempPath, NewFileName);
            uploadFile.SaveAs(filePath);
            #endregion 处理上传Excel文件

            #region 转储数据库
            ExcelData data = new ExcelData(filePath);

            DataColumn CODE = data.ExcelTable.Columns["Code"];
            DataColumn BARCODE = data.ExcelTable.Columns["BarCode"];
            DataColumn NAME = data.ExcelTable.Columns["Name"];
            DataColumn BRIEF = data.ExcelTable.Columns["Brief"];
            DataColumn REMARK = data.ExcelTable.Columns["Remark"];


            List<int> wrongRowNums = new List<int>();

            using (var scope = new TransactionScope())
            {
                int rowNum = -1;
                foreach (DataRow row in data.ExcelTable.Rows)
                {
                    rowNum++;
                    try
                    {
                        string tempCode = row[CODE].ToString();
                        string tempBarCode = row[BARCODE].ToString();
                        bool isCodeNull = (tempCode == string.Empty);
                        bool isBarCodeNull = (tempBarCode == string.Empty);
                        if (isCodeNull && isBarCodeNull)
                        {
                            throw new Exception("Both Code and BarCode are null.");
                        }
                        if (isCodeNull)
                        {
                            tempCode = tempBarCode;
                        }
                        else if (isBarCodeNull)
                        {
                            tempBarCode = tempCode;
                        }
                        WarehouseShelf wh = dbEntity.WarehouseShelves.FirstOrDefault(shelf =>
                                                                                shelf.Code == tempCode
                                                                             && shelf.WhID == WhlID
                                                                             && shelf.Deleted);
                        if (wh == null)
                        {
                            wh = new WarehouseShelf
                            {
                                WhID = WhlID,
                                Code = tempCode,
                                Barcode = tempBarCode,
                                Name = row[NAME].ToString(),
                                Brief = row[BRIEF].ToString(),
                                Remark = row[REMARK].ToString()
                            };
                            dbEntity.WarehouseShelves.Add(wh);
                        }
                        else
                        {
                            wh.Deleted = false;
                            wh.Barcode = tempBarCode;
                            wh.Name = row[NAME].ToString();
                            wh.Brief = row[BRIEF].ToString();
                            wh.Remark = row[REMARK].ToString();
                        }
                    }
                    catch
                    {
                        wrongRowNums.Add(rowNum);
                    }
                }
                if (wrongRowNums.Count == 0)
                {
                    dbEntity.SaveChanges();
                    // 提交事务，数据库物理写入
                    scope.Complete();
                    FileInfo f = new FileInfo(NewFileName);
                    f.Delete();
                }
            }
            #endregion 转储数据库

            if (wrongRowNums.Count == 0)
            {
                return RedirectToAction("WarehouseShelf");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Wrong Rows:");
                sb.Append(string.Join(",", wrongRowNums));
                ViewBag.ErrorMessage = sb.ToString();
                return View("Error");
            }
        }

        #region 货架对应的SKU
        /// <summary>
        /// 货架对应SKU
        /// </summary>
        /// <param name="shelfID">货架GUID</param>
        /// <returns></returns>
        public ViewResult ShelfSKU(Guid? shelfID)
        {
            WarehouseShelf model = (from s in dbEntity.WarehouseShelves
                                    where s.Gid == shelfID
                                       && !s.Deleted
                                    select s).SingleOrDefault();
            if (model == null)
            {
                return Error("记录不存在", Url.Action("Shelf"));
            }
            return View(model);
        }
        /// <summary>
        /// 返回某货架上所有的SKU
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="shelfID">货架Gid</param>
        /// <returns>返回货架SKU对应的Json数据</returns>
        public JsonResult ShelfSKUList(SearchModel searchModel, Guid shelfID)
        {
            IQueryable<WarehouseSkuShelf> list;
            if (shelfID == null)
                list = new List<WarehouseSkuShelf>().AsQueryable();
            else
                list = from item in dbEntity.WarehouseSkuShelves
                       where item.ShelfID == shelfID
                          && !item.Deleted
                       select item;
            GridColumnModelList<WarehouseSkuShelf> columns = new GridColumnModelList<WarehouseSkuShelf>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuItem.Code).SetName("SKUItem");
            columns.Add(p => p.SkuItem.FullName.Matter).SetName("SkuID");
            columns.Add(p => p.TrackLot);
            columns.Add(p => p.Quantity);
            columns.Add(p => p.LockQty);
            GridData gridData = list.ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        #endregion 货架对应的SKU

        #endregion 货架
        
        #region 仓库支持的配送区域
        public ActionResult Region(Guid? gid)
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            return View();
        }
        /// <summary>
        /// 仓库所支持的配送地区的列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="whID">对应的仓库</param>
        /// <returns></returns>
        public JsonResult RegionList(SearchModel searchModel,Guid? whID = null)
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            IEnumerable<WarehouseRegion> regions = (from r in dbEntity.WarehouseRegions
                                                    where r.WhID == whID
                                                       && r.Deleted == false
                                                    select r).ToList().OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseRegion> columns = new GridColumnModelList<WarehouseRegion>();
            columns.Add(p=>p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Region.Code);
            columns.Add(p => p.Region.ShortName);
            columns.Add(p => p.Region.GetRegionAddress()).SetName("Region");
            GridData gridData = regions.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 仓库地区分配页面
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <returns></returns>
        public ActionResult RegionEdit(Guid whID)
        {
            if (!base.CheckPrivilege("EnableEdit"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation model = dbEntity.WarehouseInformations.Find(whID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("Region"));
            }
            else
            {
                return View(model);
            }
        }
        /// <summary>
        /// 仓库支持配送区域树加载
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <param name="id">父级地区GUID，顶级为空</param>
        /// <returns>树结构Json字符串</returns>
        [HttpPost]
        public string RegionTreeLoad(Guid whID, Guid? id = null)
        {
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            List<LiveTreeNode> treeNodes;
            if (warehouse == null || warehouse.Deleted)
            {
                //不存在仓库
                treeNodes = new List<LiveTreeNode>();
            }
            else
            {
                //存在仓库
                List<GeneralRegion> generalRegions = (from r in dbEntity.GeneralRegions.Include("ChildItems")
                                                      where id == null ? r.aParent == null : r.aParent == id
                                                         && !r.Deleted
                                                      orderby r.Sorting descending
                                                      select r).ToList();
                List<Guid> wrIDs = (from wr in dbEntity.WarehouseRegions
                                    where wr.WhID == whID
                                        && !wr.Deleted
                                    select wr.RegionID).ToList();
                treeNodes = (from r in generalRegions
                             select new LiveTreeNode
                             {
                                 id = r.Gid.ToString(),
                                 name = r.FullName,
                                 nodeChecked = wrIDs.Any(wrid => wrid == r.Gid),
                                 isParent = (r.ChildCount > 0)
                             }).ToList();
            }
            return treeNodes.ToJsonString();
        }
        /// <summary>
        /// 添加仓库支持的配送地区
        /// </summary>
        /// <param name="whID">仓库GUID</param>
        /// <param name="regionID">地区GUID</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult RegionAdd(Guid whID, Guid regionID)
        {
            bool result;
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //不存在仓库
                result = false;
            }
            else
            {
                //存在仓库
                List<WarehouseRegion> warehouseRegions = (from r in dbEntity.WarehouseRegions
                                                          where r.WhID == whID
                                                             && r.RegionID == regionID
                                                          select r).ToList();
                if (warehouseRegions.Any(r => !r.Deleted))
                {
                    //已存在仓库支持地区
                    result = true;
                }
                else
                {
                    //不存在仓库支持地区
                    WarehouseRegion warehouseRegion;
                    if (warehouseRegions.Any())
                    {
                        warehouseRegion = warehouseRegions.First();
                        warehouseRegion.Deleted = false;
                    }
                    else
                    {
                        warehouseRegion = new WarehouseRegion
                        {
                            WhID = whID,
                            RegionID = regionID,
                            Terminal = true
                        };
                        dbEntity.WarehouseRegions.Add(warehouseRegion);
                    }
                    DeleteChildWarehouseRegion(warehouseRegion);
                    warehouseRegion.Deleted = false;
                    dbEntity.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除仓库支持的配送地区
        /// </summary>
        /// <param name="whID">仓库GUID</param>
        /// <param name="regionID">地区GUID</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult RegionDelete(Guid whID, Guid regionID)
        {
            bool result;
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //仓库不存在
                result = false;
            }
            else
            {
                WarehouseRegion warehouseRegion = (from r in dbEntity.WarehouseRegions
                                                   where r.WhID == whID
                                                      && r.RegionID == regionID
                                                      && r.Deleted == false
                                                   select r).SingleOrDefault();
                if (warehouseRegion == null)
                {
                    //已被删除
                    result = true;
                }
                else
                {
                    DeleteChildWarehouseRegion(warehouseRegion);
                    dbEntity.SaveChanges();
                    result = true;
                }
            }
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 级联删除所有给定配送地区子地区的数据库记录
        /// </summary>
        /// <param name="warehouseRegion"></param>
        private void DeleteChildWarehouseRegion(WarehouseRegion warehouseRegion)
        {
            List<Guid> regionIDs = (from r in dbEntity.GeneralRegions
                                    where r.aParent == warehouseRegion.RegionID
                                       && r.Deleted == false
                                    select r.Gid).ToList();
            List<WarehouseRegion> childItems = (from wr in dbEntity.WarehouseRegions
                                                join id in regionIDs
                                                on wr.RegionID equals id
                                                where wr.Deleted == false
                                                select wr).ToList();
            if (childItems.Any())
            {
                foreach (WarehouseRegion item in childItems)
                    DeleteChildWarehouseRegion(item);
            }
            warehouseRegion.Deleted = true;
        }
        #endregion 仓库支持的配送区域

        #region 仓库支持的承运商
        /// <summary>
        /// 仓库支持的运输公司
        /// </summary>
        /// <returns>仓库所支持的运输公司的页面</returns>
        public ActionResult Shipping()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            return View();
        }
        /// <summary>
        /// 仓库所支持的用书公司
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <returns>仓库所支持的运输公司的Json数据</returns>
        public JsonResult ShippingList(SearchModel searchModel, Guid? whID = null)
        {
            if (whID == null)
            {
                whID = GetCurrentUserPriWhl();
            }
            IEnumerable<WarehouseShipping> shippings = (from ship in dbEntity.WarehouseShippings
                                                        where ship.WhID == whID
                                                           && !ship.Deleted
                                                        select ship).ToList().OrderByDescending(item => item.LastModifyTime);
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseShipping> columns = new GridColumnModelList<WarehouseShipping>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Warehouse.FullName.GetResource(culture)).SetName("Warehouse");
            columns.Add(p => p.Shipper.FullName.GetResource(culture)).SetName("Shipper");
            columns.Add(p => p.Remark);
            GridData gridData = shippings.AsQueryable().ToGridData(searchModel,columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 显示指定仓库支持地区
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <returns></returns>
        public ActionResult ShippingEdit(Guid whID)
        {
            if (!base.CheckPrivilege("EnableEdit"))
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //仓库不存在
                return Error("仓库不存在", Url.Action("Shipping"));
            }
            else
            {
                return View(warehouse);
            }
        }
        /// <summary>
        /// 加载仓库运输公司树状结构
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <returns>仓库运输公司树状结构Json字符串</returns>
        [HttpPost]
        public string ShippingTreeLoad(Guid whID)
        {
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //仓库不存在
                return "[]";
            }
            else
            {
                List<ShippingInformation> allShippings = (from shipping in dbEntity.ShippingInformations
                                                              .Include("FullName").Include("FullName.ResourceItems")
                                                          where shipping.aParent == warehouse.aParent
                                                             && !shipping.Deleted
                                                          select shipping).ToList();
                List<Guid> ShippingIDs = (from wshipping in dbEntity.WarehouseShippings
                                          where wshipping.WhID == whID
                                             && !wshipping.Deleted
                                          select wshipping.ShipID).ToList();
                List<LiveTreeNode> nodes = (from shipping in allShippings
                                            select new LiveTreeNode
                                            {
                                                id = shipping.Gid.ToString(),
                                                name = shipping.FullName.GetResource(CurrentSession.Culture),
                                                nodeChecked = ShippingIDs.Contains(shipping.Gid)
                                            }).ToList();
                return nodes.ToJsonString();
            }
        }

        /// <summary>
        /// 为仓库添加支持的运输公司
        /// </summary>
        /// <param name="whID">仓库GUID</param>
        /// <param name="shipID">运输公司GUID</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult ShippingAdd(Guid whID, Guid shipID)
        {
            bool result;
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //仓库不存在
                result = false;
            }
            else
            {
                WarehouseShipping warehouseShipping = (from ws in dbEntity.WarehouseShippings
                                                       where ws.WhID == whID
                                                          && ws.ShipID == shipID
                                                       select ws).SingleOrDefault();
                if (warehouseShipping == null)
                {
                    //不存在记录
                    warehouseShipping = new WarehouseShipping
                    {
                        WhID = whID,
                        ShipID = shipID
                    };
                    dbEntity.WarehouseShippings.Add(warehouseShipping);
                    dbEntity.SaveChanges();
                    result = true;
                }
                else
                {
                    if (warehouseShipping.Deleted)
                    {
                        //存在已删除记录
                        warehouseShipping.Deleted = false;
                        dbEntity.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        //存在未删除记录
                        result = false;
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 为仓库删除支持的运输公司
        /// </summary>
        /// <param name="whID">仓库GUID</param>
        /// <param name="shipID">运输公司GUID</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult ShippingDelete(Guid whID, Guid shipID)
        {
            bool result;
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                //仓库不存在
                result = false;
            }
            else
            {
                WarehouseShipping warehouseShipping = (from ws in dbEntity.WarehouseShippings
                                                       where ws.WhID == whID
                                                          && ws.ShipID == shipID
                                                       select ws).SingleOrDefault();
                if (warehouseShipping == null || warehouseShipping.Deleted)
                {
                    result = true;
                }
                else
                {
                    warehouseShipping.Deleted = true;
                    dbEntity.SaveChanges();
                    result = true;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        ///// <summary>
        ///// 对WarehouseShipping进行编辑的页面
        ///// </summary>
        ///// <param name="outID">要编辑的WarehouseShipping对象的Gid</param>
        ///// <returns>返回对WarehouseShipping进行操作的部分页面</returns>
        //[HttpPost]
        //public PartialViewResult WarehouseShippingAdd()
        //{
        //    WarehouseShipping warehouseShipping = new WarehouseShipping();
        //    ViewBag.Warehouselist = GetWarehouseSelectList(GetPriWarehouses(Guid.Empty));
        //    List<ShippingInformation> shippings = dbEntity.ShippingInformations.Where(item=>item.Otype == (byte)4).ToList();
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    foreach(ShippingInformation shipping in shippings)
        //    {
        //        list.Add(new SelectListItem
        //        {
        //            Text = shipping.FullName.Matter,
        //            Value = shipping.Gid.ToString()
        //        });   
        //    }
        //    ViewBag.ShippingList = list;
        //    return PartialView(warehouseShipping);
        //}
        ///// <summary>
        ///// WarehouseShipping 添加处理
        ///// </summary>
        ///// <returns>返回到WarehouseShipping页面</returns>
        //[HttpPost]
        //public ActionResult WarehouseShippingEdit(WarehouseShipping warehouseShipping)
        //{
        //    WarehouseShipping outItem = (from item in dbEntity.WarehouseShippings
        //                              where item.WhID == warehouseShipping.WhID
        //                                 && item.ShipID == warehouseShipping.ShipID
        //                              select item).FirstOrDefault();
        //    if (outItem != null)
        //        outItem.Deleted = false;
        //    else
        //    {
        //        outItem = new WarehouseShipping
        //        {
        //            WhID = warehouseShipping.WhID,
        //            ShipID = warehouseShipping.ShipID,
        //            Remark = warehouseShipping.Remark
        //        };
        //        dbEntity.WarehouseShippings.Add(outItem);
        //    }
        //    dbEntity.SaveChanges();
        //    return RedirectToAction("WarehouseShipping");
        //}
        #endregion 仓库支持的承运商

        #region 入库单
        /// <summary>
        /// 入库单页面
        /// </summary>
        /// <param name="outID"></param>
        /// <returns>返回入库单总览页面</returns>
        public ActionResult StockIn()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn temp = new WarehouseStockIn();
            List<SelectListItem> list = base.GetSelectList(temp.InStatusList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.InStatus = list;
            list = base.GetSelectList(temp.RefTypeList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.RefType = list;
            return View();
        }
        /// <summary>
        /// 入库单Grid部分页
        /// </summary>
        /// <param name="whID">入库但所对应的仓库GUID</param>
        /// <returns>入库但Grid的部分页面</returns>
        public PartialViewResult StockInGrid(Guid? whID)
        {
            ViewBag.WHID = whID;
            return PartialView();
        }
        /// <summary>
        /// StockInList
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="whID">仓库Gid</param>
        /// <returns>返回某仓库的入库单</returns>
        public ActionResult StockInList(SearchModel searchModel, Guid? whID = null, byte? status = null, byte? type = null, int? n = 0, string itemVal = "")
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            int culture = CurrentSession.Culture;
            IEnumerable<WarehouseStockIn> stockIns;
            if (itemVal != "")
            {
                string SKUName = "";
                string SKUCode = "";
                string SKUBarcode = "";
                DateTimeOffset? StartTime = new DateTimeOffset();
                switch (n)
                {
                    case 1: SKUName = itemVal; break;
                    case 2: SKUCode = itemVal; break;
                    case 3: SKUBarcode = itemVal; break;
                    case 4: StartTime = DateTime.Parse(itemVal); break;
                    default: break;
                }
                stockIns = (from i in dbEntity.WarehouseInItems
                            where i.StockIn.WhID == whID
                                && ((status != null && status != 255) ? i.StockIn.Istatus == status : true)   //按状态搜索
                                && ((type != null && type != 255) ? i.StockIn.RefType == type : true)    //按类型搜索
                                && ((SKUName != "") ? i.SkuItem.FullName.Matter.Contains(SKUName) : true)
                                && ((SKUCode != "") ? i.SkuItem.Code.Contains(SKUCode) : true)
                                && ((SKUBarcode != "") ? i.SkuItem.Barcode.Contains(SKUCode) : true)
                                && ((StartTime != null) ? i.StockIn.LastModifyTime > StartTime : true)
                                && !i.StockIn.Deleted
                            orderby i.StockIn.LastModifiedBy descending
                            select i.StockIn).Distinct().ToList();
            }
            else
            {
                stockIns = (from i in dbEntity.WarehouseInItems
                            where i.StockIn.WhID == whID
                                && ((status != null && status != 255) ? i.StockIn.Istatus == status : true)   //按状态搜索
                                && ((type != null && type != 255) ? i.StockIn.RefType == type : true)    //按类型搜索
                                && !i.StockIn.Deleted
                            orderby i.StockIn.LastModifiedBy descending
                            select i.StockIn).Distinct().ToList();
            }
            GridColumnModelList<WarehouseStockIn> columns = new GridColumnModelList<WarehouseStockIn>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Istatus);
            columns.Add(p => p.InStatusName);
            columns.Add(p => p.StockInType.Name.GetResource(culture)).SetName("StockInType");
            columns.Add(p => p.RefTypeName).SetName("RefType");
            columns.Add(p => p.Total);
            columns.Add(p => GetUserName(p.Prepared)).SetName("Prepared");
            columns.Add(p => GetUserName(p.Approved)).SetName("Approved");
            columns.Add(p => GetTimeString(p.ApproveTime)).SetName("ApproveTime");
            columns.Add(p => p.PrintInSheet);
            GridData gridData = stockIns.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加WarehouseStockInAdd部分页Action
        /// </summary>
        /// <param name="warehouseID">对应的仓库的Gid</param>
        /// <returns>返回添加仓库的部分页</returns>
        /// //edit by tianyou 2011/10/15 添加whID 参数为null情况
        public ActionResult StockInAdd(Guid? whID = null,Guid? inType = null ,byte? refType = null, Guid? refID = null)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation warehouse = whID == null ? null : dbEntity.WarehouseInformations.Where(w => w.Gid == whID).FirstOrDefault();
            if (whID == null || warehouse == null || warehouse.Deleted)
            {
                return Error("仓库不存在", Url.Action("StockIn"));
            }
            else
            {
                Guid? bulkInGid = (from cat in dbEntity.GeneralStandardCategorys
                                   where cat.Code == "PurchaseIn"
                                      && !cat.Deleted
                                   select cat.Gid).SingleOrDefault();
                if (bulkInGid == null)
                {
                    return Error("标准分类出现异常", Url.Action("StockIn"));
                }
                WarehouseStockIn model = new WarehouseStockIn
                {
                    WhID = (Guid)whID,
                    Warehouse = warehouse,
                    RefType = (byte)ModelEnum.NoteType.PURCHASE,
                    InType = bulkInGid,
                    RefID = Guid.Empty
                };
                if (inType.HasValue)
                    model.InType = inType;
                if (refType.HasValue)
                    model.RefType = refType.Value;
                if (refID.HasValue)
                    model.RefID = refID;
                return View(model);
            }
        }
        /// <summary>
        /// 编辑入库单
        /// </summary>
        /// <param name="inID">入库单Id</param>
        /// <returns></returns>
        public ActionResult StockInEdit(Guid inID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn model = dbEntity.WarehouseStockIns.Find(inID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("StockIn"));
            }
            if (model.Istatus != (byte)ModelEnum.StockInStatus.NONE)
            {
                return Error("已确认，不能编辑", Url.Action("StockIn"));
            }
            else
            {
                return View(model);
            }
        }
        /// <summary>
        /// 入库详情查看
        /// </summary>
        /// <param name="inID">入库ID</param>
        /// <returns></returns>
        public ActionResult StockInDetail(Guid inID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn model = (from stockIn in dbEntity.WarehouseStockIns.Include("Warehouse")
                                      where stockIn.Gid == inID
                                         && !stockIn.Deleted
                                      select stockIn).SingleOrDefault();
            if (model == null)
            {
                return Error("记录不存在", Url.Action("StockIn"));
            }
            else
            {
                return View(model);
            }
        }
        /// <summary>
        /// 将新添加的入库单添加到数据库
        /// </summary>
        /// <param name="model">新添加的入库单</param>
        /// <returns>返回入库单查看页面</returns>
        [HttpPost]
        public ActionResult StockInAddDB(WarehouseStockIn model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            try
            {
                WarehouseStockIn stockIn = new WarehouseStockIn
                {
                    WhID = model.WhID,
                    InType = model.InType,
                    Prepared = CurrentSession.UserID,
                    Remark = model.Remark
                };
                ModelEnum.NoteType refType = GetRefType((Guid)model.InType);
                if (refType != ModelEnum.NoteType.NONE && !model.RefID.HasValue)
                {
                    return Error("关联单据必须填写", Url.Action("StockIn"));
                }
                stockIn.RefType = (byte)refType;
                if (refType != ModelEnum.NoteType.NONE)
                    stockIn.RefID = model.RefID;
                dbEntity.WarehouseStockIns.Add(stockIn);
                dbEntity.SaveChanges();
                //需要控制页面流到对应的组织和仓库
                return RedirectToAction("StockInEdit", new { inID = stockIn.Gid });
            }
            catch (RefTypeNotFoundExcetpion)
            {
                return Error("关联单据类型出现异常", Url.Action("StockIn"));
            }
        }
        /// <summary>
        /// 入库单编辑
        /// </summary>
        /// <param name="model">要添加的WarehouseStockIn</param>
        /// <returns>WarehouseStockIn页面</returns>
        [HttpPost]
        public ActionResult StockInEditDB(WarehouseStockIn model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn temp = dbEntity.WarehouseStockIns.Find(model.Gid);
            if (temp == null || temp.Deleted)
            {
                return Error("记录不存在", Url.Action("StockIn"));
            }
            if (model.Istatus == (byte)ModelEnum.StockInStatus.CONFIRMED)
            {
                return Error("已确认，不能编辑", Url.Action("StockIn"));
            }
            temp.InType = model.InType;
            ModelEnum.NoteType refType = GetRefType((Guid)model.InType);
            if (refType != ModelEnum.NoteType.NONE && !model.RefID.HasValue)
            {
                return Error("必须填写关联单据", Url.Action("StockIn"));
            }
            temp.RefType = (byte)refType;
            if (refType != ModelEnum.NoteType.NONE)
                temp.RefID = model.RefID;
            dbEntity.SaveChanges();
            return RedirectToAction("StockIn");
        }
        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="inID">要删除的入库单ID</param>
        /// <returns>返回删除结果</returns>
        [HttpPost]
        public JsonResult StockInDeleteDB(Guid inID)
        {
            if (!base.CheckPrivilege("EnableDelete"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int state = whBll.StockInDiscard(inID, CurrentSession.UserID);
            bool result = (state == 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 执行确认入库单到数据库
        /// </summary>
        /// <param name="inID">入库记录ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StockInConfirmDB(Guid inID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int result = whBll.StockInConfirm(inID, CurrentSession.UserID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 执行取消确认入库单到数据库
        /// </summary>
        /// <param name="inID">入库记录ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StockInCancelConfirmDB(Guid inID)
        {
            bool result;
            WarehouseStockIn stockIn = dbEntity.WarehouseStockIns.Find(inID);
            if (stockIn == null || stockIn.Deleted)
            {
                //不存在入库记录
                result = false;
            }
            else if (stockIn.Istatus != (byte)ModelEnum.StockInStatus.CONFIRMED)
            {
                //未确认
                result = false;
            }
            else
            {
                //已确认
                stockIn.Istatus = (byte)ModelEnum.StockInStatus.CONFIRMED;
                stockIn.Approved = null;
                stockIn.ApproveTime = null;
                dbEntity.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion 入库单

        #region 入库单明细
        /// <summary>
        /// 入库单明细页面
        /// </summary>
        /// <param name="inID">入库单ID</param>
        /// <returns>返回入库单明细页面</returns>
        public ActionResult InItem(Guid inID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            ViewBag.privEnablePrepare = GetProgramNode("EnablePrepare");//允许制表(编辑)
            ViewBag.privEnableApprove = GetProgramNode("EnableApprove");//允许确认
            ViewBag.privEnableDelete = GetProgramNode("EnableDelete");//允许删除/作废
            WarehouseStockIn model = dbEntity.WarehouseStockIns.Find(inID);
            if (model == null || model.Deleted)
            {
                //入库明细不存在
                return Error("记录不存在", Url.Action("StockIn"));
            }
            else
            {
                return View(model);
            }
        }
        /// <summary>
        /// 入库明细的Grid部分页面
        /// </summary>
        /// <param name="inID"></param>
        /// <returns></returns>
        public PartialViewResult InItemGrid(Guid inID)
        {
            ViewBag.InID = inID;
            return PartialView();
        }
        /// <summary>
        /// 入库单列表
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="outID">入库单ID</param>
        /// <returns></returns>
        public JsonResult InItemList(SearchModel searchModel,Guid inID)
        {
            int currentCultre = CurrentSession.Culture;
            CultureInfo culture = new CultureInfo(currentCultre);
            IEnumerable<WarehouseInItem> stockInItems = (from item in dbEntity.WarehouseInItems
                                                         where item.InID == inID
                                                            && !item.Deleted
                                                         select item).ToList().OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseInItem> columns = new GridColumnModelList<WarehouseInItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuItem.Code).SetName("SkuID");
            columns.Add(p => p.SkuItem.FullName.GetResource(currentCultre)).SetName("SkuItem");
            columns.Add(p => p.Shelf.Code+"("+p.Shelf.Name+")").SetName("Shelf");;
            columns.Add(p=>p.TrackLot);
            columns.Add(p=>p.Quantity);
            columns.Add(p => GetTimeString(p.Guarantee)).SetName("Guarantee");
            columns.Add(p=>p.GenBarcode);
            GridData gridData = stockInItems.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 入库单明细添加
        /// </summary>
        /// <param name="inID">对应的入库单ID</param>
        /// <returns>返回添加入库单条目的部分页</returns>
        public ActionResult InItemAdd(Guid inID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn stockIn = dbEntity.WarehouseStockIns.Find(inID);
            if (stockIn == null || stockIn.Deleted)
            {
                return Error("记录不存在", Url.Action("StockInEdit", new { inID = inID }));
            }
            else
            {
                WarehouseInItem inItem = new WarehouseInItem
                {
                    InID = inID,
                    StockIn = stockIn,
                    Quantity = 1m
                };

                #region 货架下拉框数据
                var shelves = (from shelf in dbEntity.WarehouseShelves
                               where shelf.WhID == stockIn.WhID
                                  && shelf.Deleted == false
                               select new { Code = shelf.Code, Gid = shelf.Gid }).ToList();
                List<SelectListItem> list = (from shelf in shelves
                                             select new SelectListItem
                                             {
                                                 Text = shelf.Code,
                                                 Value = shelf.Gid.ToString()
                                             }).ToList();
                ViewBag.Shelf = list;

                #endregion 货架下拉框数据

                return View(inItem);
            }
        }
        /// <summary>
        /// 入库单明细编辑
        /// </summary>
        /// <param name="itemID">入库单明细ID</param>
        /// <returns></returns>
        public ActionResult InItemEdit(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInItem inItem = (from item in dbEntity.WarehouseInItems.Include("StockIn")
                                      where item.Gid == itemID
                                         && !item.Deleted
                                      select item).SingleOrDefault();
            if (inItem == null)
            {
                return Error("记录不存在", Url.Action("StockIn"));
            }
            else
            {
                #region 货架下拉框数据
                List<WarehouseShelf> shelves = (from shelf in dbEntity.WarehouseShelves
                                                where shelf.WhID == inItem.StockIn.WhID
                                                   && shelf.Deleted == false
                                                select shelf).ToList();
                List<SelectListItem> list = (from shelf in shelves
                                             select new SelectListItem
                                             {
                                                 Text = shelf.Code,
                                                 Value = shelf.Gid.ToString()
                                             }).ToList();
                ViewBag.Shelf = list;

                #endregion 货架下拉框数据
                return View(inItem);
            }
        }
        /// <summary>
        /// 将添加的入库单明细写入数据库
        /// </summary>
        /// <param name="model">从View接收到的入库单明细</param>
        /// <returns>返回到入库单页面</returns>
        [HttpPost]
        public ActionResult InItemAddDB(WarehouseInItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (model.Quantity == 0m)
            {
                return Error("数量必须大于0", Url.Action("InItemAdd", new { InID = model.InID }));
            }
            WarehouseInItem inItem = (from item in dbEntity.WarehouseInItems
                                    where item.InID == model.InID
                                       && item.SkuID == model.SkuID
                                       && item.ShelfID == model.ShelfID
                                       && item.TrackLot == model.TrackLot
                                    select item).SingleOrDefault();
            if (inItem == null)
            {
                inItem = new WarehouseInItem
                {
                    InID = model.InID,
                    SkuID = model.SkuID,
                    ShelfID = model.ShelfID,
                    TrackLot = model.TrackLot,
                    Quantity = model.Quantity,
                    Guarantee = model.Guarantee,
                    GenBarcode = model.GenBarcode
                };
                dbEntity.WarehouseInItems.Add(inItem);
            }
            else
            {
                if (inItem.Deleted)
                {
                    inItem.Deleted = false;
                    inItem.Quantity = model.Quantity;
                    inItem.Guarantee = model.Guarantee;
                    inItem.GenBarcode = model.GenBarcode;
                }
                else
                    return Error("记录冲突", Url.Action("InItemAdd", new { inID = model.InID }));
            }
            dbEntity.SaveChanges();
            WarehouseStockIn stockIn = (from s in dbEntity.WarehouseStockIns.Include("StockInItems")
                                        where s.Gid == inItem.InID
                                           && !s.Deleted
                                        select s).Single();
            stockIn.Total = stockIn.StockInItems.Select(item => item.Quantity).Sum();
            dbEntity.SaveChanges();
            whBll = new WarehouseBLL(dbEntity);
            whBll.InventoryByWarehouseSku(stockIn.WhID, model.SkuID);
            return RedirectToAction("StockInEdit", new { inID = model.InID });
        }
        /// <summary>
        /// 将对入库明细的编辑写入数据库
        /// </summary>
        /// <param name="model">从View接收到的入库明细对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InItemEditDB(WarehouseInItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (model.Quantity == 0m)
            {
                return Error("数量必须大于零", Url.Action("InItemEdit", new { itemID = model.Gid }));
            }
            WarehouseInItem inItem = dbEntity.WarehouseInItems.Find(model.Gid);
            if (inItem == null || inItem.Deleted)
            {
                return Error("记录不存在", Url.Action("StockIn"));
            }
            else
            {
                inItem.SkuID = model.SkuID;
                inItem.ShelfID = model.ShelfID;
                inItem.TrackLot = model.TrackLot;
                inItem.Quantity = model.Quantity;
                inItem.Guarantee = model.Guarantee;
                inItem.GenBarcode = model.GenBarcode;
                dbEntity.SaveChanges();
                WarehouseStockIn stockIn = (from s in dbEntity.WarehouseStockIns.Include("StockInItems")
                                            where s.Gid == inItem.InID
                                               && !s.Deleted
                                            select s).Single();
                stockIn.Total = stockIn.StockInItems.Select(item => item.Quantity).Sum();
                dbEntity.SaveChanges();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(stockIn.WhID, model.SkuID);
                return RedirectToAction("StockInEdit", new { inID = inItem.InID });
            }
        }
        /// <summary>
        /// 删除入库明细item
        /// </summary>
        /// <param name="itemID">被删除的入库明细条目ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InItemDeleteDB(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证,有制表权限就能删除明细条目，而不是根据入库单的删除权限
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            WarehouseInItem inItem = dbEntity.WarehouseInItems.Find(itemID);
            if (inItem == null || inItem.Deleted)
            {
                //不存在记录
            }
            else
            {
                inItem.Deleted = true;
                dbEntity.SaveChanges();
                WarehouseStockIn stockIn = (from s in dbEntity.WarehouseStockIns.Include("StockInItems")
                                            where s.Gid == inItem.InID
                                               && !s.Deleted
                                            select s).Single();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(stockIn.WhID, inItem.SkuID);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion 入库单明细

        #region 出库记录
        /// <summary>
        /// 出库记录
        /// </summary>
        /// <returns>出库记录页面</returns>
        public ActionResult StockOut()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockIn temp = new WarehouseStockIn();
            List<SelectListItem> list = base.GetSelectList(temp.InStatusList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.InStatus = list;
            list = base.GetSelectList(temp.RefTypeList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.RefType = list;
            return View();
        }
        /// <summary>
        /// 出库记录Grid部分页面
        /// </summary>
        /// <param name="whID">出库记录所对应的仓库的GUID</param>
        /// <returns></returns>
        public PartialViewResult StockOutGrid(Guid whID)
        {
            ViewBag.WHID = whID;
            return PartialView();
        }
        /// <summary>
        /// 出库记录列表
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="whID">要查看出库记录的仓库</param>
        /// <returns>返回某仓库出库记录列表</returns>
        public JsonResult StockOutList(SearchModel searchModel, Guid? whID = null, byte? status = null, byte? type = null,int? n = 0,string itemVal = "")
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            IEnumerable<WarehouseStockOut> StockOuts;
            if (itemVal != "")
            {
                string SKUName = "";
                string SKUCode = "";
                string SKUBarcode = "";
                DateTimeOffset? StartTime = new DateTimeOffset();
                switch (n)
                {
                    case 1: SKUName = itemVal; break;
                    case 2: SKUCode = itemVal; break;
                    case 3: SKUBarcode = itemVal; break;
                    case 4: StartTime = DateTime.Parse(itemVal); break;
                    default: break;
                }
                StockOuts = (from s in dbEntity.WarehouseStockOuts
                             join i in dbEntity.WarehouseOutItems on s.Gid equals i.OutID
                             where s.WhID == whID
                                 && ((status != null && status != 255) ? s.Ostatus == status : true)   //按状态搜索
                                 && ((type != null && status != 255) ? s.RefType == type : true)    //按类型搜索
                                 && ((SKUName != "") ? i.SkuItem.FullName.Matter.Contains(SKUName) : true)
                                 && ((SKUCode != "") ? i.SkuItem.Code.Contains(SKUCode) : true)
                                 && ((SKUBarcode != "") ? i.SkuItem.Barcode.Contains(SKUCode) : true)
                                 && ((StartTime != null) ? s.LastModifyTime > StartTime : true)
                                 && !s.Deleted
                             select s).ToList().OrderByDescending(item => item.LastModifyTime);
            }
            else
            {
                StockOuts = (from s in dbEntity.WarehouseStockOuts
                             join i in dbEntity.WarehouseOutItems on s.Gid equals i.OutID
                             where s.WhID == whID
                                 && ((status != null && status != 255) ? s.Ostatus == status : true)   //按状态搜索
                                 && ((type != null && status != 255) ? s.RefType == type : true)    //按类型搜索
                                 && !s.Deleted
                             select s).ToList().OrderByDescending(item => item.LastModifyTime);
            }            
            GridColumnModelList<WarehouseStockOut> columns = new GridColumnModelList<WarehouseStockOut>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.Code);
            columns.Add(p => p.Ostatus);
            columns.Add(p => p.OutStatusName);
            columns.Add(p => GetStandardCategoryName(p.OutType)).SetName("OutType");
            columns.Add(p => p.RefType);
            columns.Add(p => p.RefTypeName);
            columns.Add(p => GetRefCode((ModelEnum.NoteType)p.RefType, p.RefID)).SetName("RefID");
            columns.Add(p => p.PrintOutSheet);
            columns.Add(p => p.PrintEnvelope);
            columns.Add(p => p.Total);
            columns.Add(p => GetUserName(p.Prepared)).SetName("Prepared");
            columns.Add(p => GetUserName(p.Approved)).SetName("Approved");
            columns.Add(p => GetTimeString(p.ApproveTime)).SetName("ApproveTime");
            GridData gridData = StockOuts.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加出库记录
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <returns></returns>
        public ActionResult StockOutAdd(Guid whID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if(warehouse == null || warehouse.Deleted)
            {
                return Error("仓库不存在", Url.Action("StockOut"));
            }
            else
            {
                Guid? SaleGid = (from cat in dbEntity.GeneralStandardCategorys
                                   where cat.Code == "Sale"
                                      && !cat.Deleted
                                   select cat.Gid).SingleOrDefault();
                if (SaleGid == null)
                {
                    return Error("标准分类出现异常", Url.Action("Index", "Home"));
                }
                WarehouseStockOut model = new WarehouseStockOut
                {
                    WhID = whID,
                    Warehouse = warehouse,
                    OutType = SaleGid,
                    RefType = (byte)ModelEnum.NoteType.ORDER,
                    RefID = Guid.Empty
                };
                #region 运输公司下拉列表

                List<WarehouseShipping> shippings = (from shipping in dbEntity.WarehouseShippings
                                                         .Include("Shipper.FullName")
                                                         .Include("Shipper.FullName.ResourceItems")
                                                     where shipping.WhID == whID
                                                        && !shipping.Deleted
                                                     select shipping).ToList();
                List<SelectListItem> list = (from shipping in shippings
                                             select new SelectListItem
                                             {
                                                 Text = shipping.Shipper.FullName.GetResource(CurrentSession.Culture),
                                                 Value = shipping.ShipID.ToString()
                                             }).ToList();
                ViewBag.Shippings = list;

                #endregion 运输公司下拉列表
                #region 类型下拉列表

                ViewBag.RefType = base.GetSelectList(model.RefTypeList);

                #endregion 类型下拉列表
                return View(model);
            }
        }
        /// <summary>
        /// 编辑出库记录
        /// </summary>
        /// <param name="outID">出库记录ID</param>
        /// <returns></returns>
        public ActionResult StockOutEdit(Guid outID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut model = (from o in dbEntity.WarehouseStockOuts
                                         .Include("Warehouse")
                                       where o.Gid == outID
                                          && !o.Deleted
                                       select o).SingleOrDefault();
            if (model == null)
            {
                return Error("记录不存在", Url.Action("StockOut"));
            }
            if (model.Ostatus != (byte)ModelEnum.StockOutStatus.NONE)
            {
                return Error("已确认，不可编辑", Url.Action("StockOut"));
            }
            else
            {
                #region 运输公司下拉列表

                List<WarehouseShipping> shippings = (from shipping in dbEntity.WarehouseShippings
                                                         .Include("Shipper.FullName")
                                                         .Include("Shipper.FullName.ResourceItems")
                                                     where shipping.WhID == model.WhID
                                                        && !shipping.Deleted
                                                     select shipping).ToList();
                List<SelectListItem> list = (from shipping in shippings
                                             select new SelectListItem
                                             {
                                                 Text = shipping.Shipper.FullName.GetResource(CurrentSession.Culture),
                                                 Value = shipping.ShipID.ToString()
                                             }).ToList();
                ViewBag.Shippings = list;

                #endregion 运输公司下拉列表
                return View(model);
            }
        }
        /// <summary>
        /// 出库详情查看
        /// </summary>
        /// <param name="outID">出库记录ID</param>
        /// <returns></returns>
        public ActionResult StockOutDetail(Guid outID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut model = (from o in dbEntity.WarehouseStockOuts
                                         .Include("Warehouse")
                                       where o.Gid == outID
                                          && !o.Deleted
                                       select o).SingleOrDefault();
            if (model == null)
            {
                return Error("出库单不存在", Url.Action("StockOut"));
            }
            else
            {
                return View(model);
            }
        }
        /// <summary>
        /// 执行添加出库记录到数据库
        /// </summary>
        /// <param name="model">从View接收到的出库记录对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StockOutAddDB(WarehouseStockOut model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut newOut = (from o in dbEntity.WarehouseStockOuts
                                        where o.WhID == model.WhID
                                           && o.Code == model.Code
                                        select o).SingleOrDefault();
            if (newOut == null)
            {
                newOut = new WarehouseStockOut
                {
                    WhID = model.WhID,
                    OutType = model.OutType,
                    ShipID = model.ShipID,
                    Remark = model.Remark,
                    Prepared = CurrentSession.UserID
                };
                ModelEnum.NoteType refType = GetRefType((Guid)model.OutType);
                if (refType != ModelEnum.NoteType.NONE && !model.RefID.HasValue)
                {
                    return Error("关联单据号必填", Url.Action("StockOutAdd", new { whID = model.WhID }));
                }
                newOut.RefType = (byte)refType;
                if (refType != ModelEnum.NoteType.NONE)
                    newOut.RefID = model.RefID;
                dbEntity.WarehouseStockOuts.Add(newOut);
            }
            else if (newOut.Deleted)
            {
                newOut.Deleted = false;
                newOut.OutType = model.OutType;
                newOut.ShipID = model.ShipID;
                newOut.Remark = model.Remark;
                ModelEnum.NoteType refType = GetRefType((Guid)model.OutType);
                newOut.RefType = model.RefType;
                if (model.RefType != (byte)ModelEnum.NoteType.NONE)
                    newOut.RefID = model.RefID;
            }
            else
            {
                return Error("代码冲突", Url.Action("StockOutAdd", new { whID = model.WhID }));
            }
            dbEntity.SaveChanges();
            return RedirectToAction("StockOutEdit", new { outID = newOut.Gid });
        }
        /// <summary>
        /// 将编辑出库记录保存到数据库
        /// </summary>
        /// <param name="model">从View接收到的出库记录对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StockOutEditDB(WarehouseStockOut model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut stockOut = dbEntity.WarehouseStockOuts.Find(model.Gid);
            if (stockOut == null || stockOut.Deleted)
            {
                return Error("出库单不存在", Url.Action("StockOut"));
            }
            else if (model.Ostatus != (byte)ModelEnum.StockOutStatus.NONE)
            {
                return Error("出库单状态不正确", Url.Action("StockOut"));
            }
            else
            {
                stockOut.OutType = model.OutType;
                stockOut.ShipID = model.ShipID;
                stockOut.Remark = model.Remark;
                ModelEnum.NoteType refType = GetRefType((Guid)model.OutType);
                if (refType != ModelEnum.NoteType.NONE && !model.RefID.HasValue)
                {
                    return Error("关联单据号必填", Url.Action("StockOut"));
                }
                stockOut.RefType = (byte)refType;
                if (refType != ModelEnum.NoteType.NONE)
                    stockOut.RefID = model.RefID;
                dbEntity.SaveChanges();
                return RedirectToAction("StockOut");
            }
        }
        /// <summary>
        /// 执行删除出库记录到数据库
        /// </summary>
        /// <param name="outID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StockOutDeleteDB(Guid outID)
        {
            if (!base.CheckPrivilege("EnableDelete"))//删除权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int state = whBll.StockOutDiscard(outID, CurrentSession.UserID);
            bool result = (state == 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认出库单到数据库
        /// </summary>
        /// <param name="outID">出库单ID</param>
        /// <returns></returns>0 1 2 3 4 5 11：确认成功，订单用户无订阅消息12：邮件提醒消息队列添加失败13：短信提醒消息队列添加失败
        public JsonResult StockOutConfirmDB(Guid outID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//确认出库单权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int result = whBll.StockOutConfirm(outID, CurrentSession.UserID);
            //add by tianyou 2011/10/25 设置消息队列
            int _SetMessagePending ;//判断消息队列设置返回值
            if (result == 0)
            {
                //验证单据类型是否为订单 RefType==0?
                WarehouseStockOut oStockOut = dbEntity.WarehouseStockOuts.Where(o => o.Gid == outID && o.RefType == (byte)ModelEnum.NoteType.ORDER && o.Deleted == false).FirstOrDefault();
                if (oStockOut != null)
                {//如果是订单 则判断用户是否订阅发货消息
                    OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Deleted == false && o.Gid == oStockOut.RefID).FirstOrDefault();
                    if (oOrder != null)
                    { 
                        //设置消息队列
                        _SetMessagePending = whBll.DeliveryMessageSet(oOrder.UserID, 1, oOrder.Gid,CurrentSession.Culture);
                        result = _SetMessagePending;
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        #endregion 出库记录

        #region 出库记录明细

        /// <summary>
        /// 出库记录明细查看页面
        /// </summary>
        /// <param name="outID">出库单号</param>
        /// <returns>返回出库单所对应的出库记录明细页面</returns>
        public ActionResult OutItem(Guid outID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            ViewBag.privEnablePrepare = GetProgramNode("EnablePrepare");//允许制表(编辑)
            ViewBag.privEnableApprove = GetProgramNode("EnableApprove");//允许确认
            ViewBag.privEnableDelete = GetProgramNode("EnableDelete");//允许删除/作废
            WarehouseStockOut model = (from s in dbEntity.WarehouseStockOuts
                                       where s.Gid == outID
                                       select s).Single();
            return View(model);
        }
        public PartialViewResult OutItemGrid(Guid outID)
        {
            ViewBag.OutID = outID;
            return PartialView();
        }
        /// <summary>
        /// 出库记录List
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="outID">出库单号</param>
        /// <returns>出库单对应的出库记录列表</returns>
        public JsonResult OutItemList(SearchModel searchModel, Guid outID)
        {
            IEnumerable<WarehouseOutItem> outItems = (from item in dbEntity.WarehouseOutItems
                                                      where item.OutID == outID
                                                         && !item.Deleted
                                                      select item).ToList().OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseOutItem> columns = new GridColumnModelList<WarehouseOutItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuItem.Code).SetName("SkuID");
            columns.Add(p => p.SkuItem.FullName.GetResource(CurrentSession.Culture)).SetName("SkuItem");
            columns.Add(p => p.Shelf.Code+"("+p.Shelf.Name+")").SetName("Shelf");
            columns.Add(p => p.TrackLot);
            columns.Add(p => p.Quantity);
            GridData gridData = outItems.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 出库记录明细添加
        /// </summary>
        /// <param name="outID">要添加书库记录明细的出库单GUID</param>
        /// <returns>出库记录明细添加页面</returns>
        public ActionResult OutItemAdd(Guid outID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut stockOut = dbEntity.WarehouseStockOuts.Find(outID);
            if (stockOut == null || stockOut.Deleted) 
            {
                return Error("记录不存在", Url.Action("StockOut"));
            }
            WarehouseOutItem model = new WarehouseOutItem
            {
                OutID = outID,
                StockOut = stockOut
            };
            #region 货架下拉框数据
            var shelves = (from shelf in dbEntity.WarehouseShelves
                           where shelf.WhID == stockOut.WhID
                              && shelf.Deleted == false
                           select new { Code = shelf.Code, Gid = shelf.Gid }).ToList();
            List<SelectListItem> list = (from shelf in shelves
                                         select new SelectListItem
                                         {
                                             Text = shelf.Code,
                                             Value = shelf.Gid.ToString()
                                         }).ToList();
            ViewBag.Shelf = list;

            #endregion 货架下拉框数据
            return View(model);
        }
        /// <summary>
        /// 出库单明细编辑
        /// </summary>
        /// <param name="itemID">要编辑的出库单明细的GUID</param>
        /// <returns>出库单明细编辑页面</returns>
        public ActionResult OutItemEdit(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseOutItem OutItem = (from oI in dbEntity.WarehouseOutItems
                                        where oI.Gid == itemID
                                           && !oI.Deleted
                                        select oI).SingleOrDefault();
            if (OutItem == null)
            {
                return Error("记录不存在", Url.Action("StockOut"));
            }
            #region 货架下拉框数据
            var shelves = (from shelf in dbEntity.WarehouseShelves
                           where shelf.WhID == OutItem.StockOut.WhID
                              && shelf.Deleted == false
                           select new { Code = shelf.Code, Gid = shelf.Gid }).ToList();
            List<SelectListItem> list = (from shelf in shelves
                                         select new SelectListItem
                                         {
                                             Text = shelf.Code,
                                             Value = shelf.Gid.ToString()
                                         }).ToList();
            ViewBag.Shelf = list;

            #endregion 货架下拉框数据
            return View(OutItem);
        }
        /// <summary>
        /// 出库单明细添加的记录写到数据库
        /// </summary>
        /// <param name="model">新添加的出库记录明细</param>
        /// <returns>出库记录明细纵览页面</returns>
        [HttpPost]
        public ActionResult OutItemAddDB(WarehouseOutItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (model.Quantity == 0m)
            {
                return Error("数量必须大于0", Url.Action("StockOutEdit", new { outID = model.OutID }));
            }
            WarehouseOutItem outItem;

            outItem = (from o in dbEntity.WarehouseOutItems
                    where  o.SkuID == model.SkuID
                        && o.OutID == model.OutID
                        && o.ShelfID == model.ShelfID
                        && !o.Deleted
                    select o).SingleOrDefault();
            if (outItem != null)
            {
                return Error("记录冲突", Url.Action("StockOutEdit", new { outID = model.OutID }));
            }
            else
            {
                outItem = new WarehouseOutItem
                {
                    OutID = model.OutID,
                    SkuID = model.SkuID,
                    ShelfID = model.ShelfID,
                    TrackLot = model.TrackLot,
                    Quantity = model.Quantity
                };
                dbEntity.WarehouseOutItems.Add(outItem);
                dbEntity.SaveChanges();
                WarehouseStockOut stockOut = (from s in dbEntity.WarehouseStockOuts.Include("StockOutItems")
                                              where s.Gid == outItem.OutID
                                               && !s.Deleted
                                            select s).Single();
                stockOut.Total = stockOut.StockOutItems.Select(item => item.Quantity).Sum();
                dbEntity.SaveChanges();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(stockOut.WhID, model.SkuID);
            }
            return RedirectToAction("StockOutEdit", new { outID = model.OutID });
        }
        
        /// <summary>
        /// 出库单明细编辑结果添加的到数据库
        /// </summary>
        /// <param name="model">被编辑的出库单明细条目</param>
        /// <returns>出库单纵览页面</returns>
        [HttpPost]
        public ActionResult OutItemEditDB(WarehouseOutItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            if (model.Quantity == 0m)
            {
                return Error("数量必须大于0", Url.Action("StockOutEdit", new { outID = model.OutID }));
            }
            WarehouseOutItem temp;
            temp = (from o in dbEntity.WarehouseOutItems
                    where o.Gid == model.Gid
                        && !o.Deleted
                    select o).SingleOrDefault();
            if (temp == null)
            {
                return Error("记录不存在", Url.Action("StockOutEdit", new { outID = model.OutID }));
            }
            temp.TrackLot = model.TrackLot;
            temp.Quantity = model.Quantity;
            temp.ShelfID = model.ShelfID;
            dbEntity.SaveChanges();
            WarehouseStockOut stockOut = (from s in dbEntity.WarehouseStockOuts.Include("StockOutItems")
                                          where s.Gid == temp.OutID
                                           && !s.Deleted
                                          select s).Single();
            stockOut.Total = stockOut.StockOutItems.Select(item => item.Quantity).Sum();
            dbEntity.SaveChanges();
            whBll = new WarehouseBLL(dbEntity);
            whBll.InventoryByWarehouseSku(stockOut.WhID, model.SkuID);
            return RedirectToAction("StockOutEdit", new { temp.OutID });
        }
        /// <summary>
        /// 执行删除出库明细到数据库
        /// </summary>
        /// <param name="itemID">明细ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OutItemDeleteDB(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            WarehouseOutItem item = dbEntity.WarehouseOutItems.Find(itemID);
            if (item == null || item.Deleted)
            {

            }
            else
            {
                item.Deleted = true;
                dbEntity.SaveChanges();
                WarehouseStockOut stockOut = (from s in dbEntity.WarehouseStockOuts.Include("StockOutItems")
                                              where s.Gid == item.OutID
                                               && !s.Deleted
                                              select s).Single();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(stockOut.WhID, item.SkuID);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion 出库记录明细

        #region 出库扫描和承运页面
        /// <summary>
        /// 出库扫描页面
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        public ActionResult OutScan(Guid outID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//是否允许扫描确认
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut model = dbEntity.WarehouseStockOuts.Find(outID);
            if (model == null || model.Deleted)
            {
                return Error("出库单不存在", Url.Action("StokOut"));
            }
            if (model.RefType != (byte)ModelEnum.NoteType.ORDER)
            {
                return Error("出库单类型不是订单，无法出库扫描", Url.Action("StokOut"));
            }
            else if (model.RefID == null || model.RefID == Guid.Empty)
            {
                return Error("关联订单为空，无法出库扫描", Url.Action("StokOut"));
            }
            else if (model.Ostatus != (byte)ModelEnum.StockOutStatus.NONE)
            {
                return Error("出库单据状态不正确，无法出库扫描", Url.Action("StokOut"));
            }
            else
            {
                OrderInformation order = dbEntity.OrderInformations.Find(model.RefID);
                if (order == null || order.Deleted)
                {
                    return Error("关联订单不存在，无法出库扫描", Url.Action("StokOut"));
                }
                if (order.Ostatus != (byte)ModelEnum.OrderStatus.ARRANGED)
                {
                    return Error("关联订单状态不正确，无法出库扫描", Url.Action("StokOut"));
                }
                return View(model);
            }
        }
        /// <summary>
        /// 出库扫描Grid
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="outID">出库单ID</param>
        /// <returns></returns>
        public ActionResult OutScanList(SearchModel searchModel, Guid outID)
        {
            IEnumerable<WarehouseOutItem> outItems = (from item in dbEntity.WarehouseOutItems
                                                      where item.OutID == outID
                                                         && !item.Deleted
                                                      select item).ToList().OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseOutItem> columns = new GridColumnModelList<WarehouseOutItem>();
            columns.Add(p => p.Gid).SetAsPrimaryKey();
            columns.Add(p => p.SkuID);
            columns.Add(p => p.SkuItem.FullName.GetResource(CurrentSession.Culture)).SetName("SkuItem");
            columns.Add(p => p.Shelf.Name + '(' + p.Shelf.Code + ')').SetName("Shelf");
            columns.Add(p => p.TrackLot);
            columns.Add(p => p.Quantity);
            columns.Add(p => 0).SetName("Scanned");
            GridData gridData = outItems.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 出库单对应的承运商列表
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="outID">出库单的Gid</param>
        /// <returns>出库单对应的承运扫描的</returns>
        public ActionResult OutDeliveryList(SearchModel searchModel, Guid outID)
        {
            IEnumerable<WarehouseOutDelivery> deliveries = (from item in dbEntity.WarehouseOutDeliveries
                                                            where item.OutID == outID
                                                               && !item.Deleted
                                                            select item).ToList().OrderByDescending(item => item.LastModifyTime);
            GridColumnModelList<WarehouseOutDelivery> columns = new GridColumnModelList<WarehouseOutDelivery>();
            columns.Add(x => x.Gid).SetAsPrimaryKey();
            columns.Add(x => x.ShipID);
            columns.Add(x => x.Envelope);
            columns.Add(x => x.PackWeight);
            GridData gridData = deliveries.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 同过条形码获得SKU的Gid
        /// </summary>
        /// <param name="barcode">要获的SKU的条形码</param>
        /// <returns></returns>
        [HttpPost]
        public Guid BarcodeToSkuID(string barcode, Guid outID)
        {
            //获得出库单对应的仓库
            WarehouseStockOut stockOut = dbEntity.WarehouseStockOuts.Find(outID);
            if (stockOut == null || stockOut.Deleted)
            {
                return Guid.Empty;
                //return ;
            }
            Guid orgGid = dbEntity.WarehouseInformations.Find(stockOut.WhID).aParent.Value;
            //找到对应的产品
            ProductInfoItem pItem = (from i in dbEntity.ProductInfoItems
                                     where i.OrgID == orgGid
                                        && i.Barcode == barcode
                                        && !i.Deleted
                                     select i).SingleOrDefault();
            if (pItem == null)          //次运货单不支持的产品
            {
                return Guid.Empty;
            }
            return pItem.Gid;
        }
        /// <summary>
        /// 确认发货
        /// </summary>
        /// <param name="data">发货承运信息</param>
        /// <param name="outID">出库单Gid</param>
        /// <returns>出库单页面，继续继续扫描，或查看</returns>
        [HttpPost]
        public ActionResult ConfirmDelivery(string data, Guid outID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//是否允许确认(扫描/发货)
                RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseStockOut stockOut = dbEntity.WarehouseStockOuts.Find(outID);
            if (stockOut == null || stockOut.Deleted)
            {
                return Error("记录不存在", Url.Action("StockOut"));
            }
            else if (stockOut.Ostatus != (byte)ModelEnum.StockOutStatus.NONE)
            {
                return Error("已确认，不能再次确认", Url.Action("StockOut"));
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            IEnumerable<WarehouseOutDelivery> models = jss.Deserialize<IEnumerable<WarehouseOutDelivery>>(data);
            foreach (WarehouseOutDelivery model in models)
            {
                WarehouseOutDelivery delivery = new WarehouseOutDelivery
                {
                    OutID = outID,
                    Envelope = model.Envelope,
                    PackWeight = model.PackWeight,
                    ShipID = model.ShipID
                };
                dbEntity.WarehouseOutDeliveries.Add(delivery);
            }
            Guid shipper = models.ElementAt(0).ShipID;//最终承运商
            #region 修改出库单的最终信息
            stockOut.ShipID = models.ElementAt(0).ShipID;//设置出库单的最终承运商
            stockOut.Ostatus = (byte)ModelEnum.StockOutStatus.DELIVERIED;//设置出库单的状态
            stockOut.SendMan = CurrentSession.UserID;
            #endregion 修改出库单的最终信息
            #region 设置订单的最终承运商
            OrderShipping os = (from i in dbEntity.OrderShippings
                                where i.OrderID == stockOut.RefID
                                   && i.ShipID == shipper
                                   && i.Deleted == false
                                select i).FirstOrDefault();
            //如果订单对应的运输信还没有，新建
            if (os == null)
            {
                os = new OrderShipping
                {
                    ShipID = shipper,
                    OrderID = stockOut.RefID.Value,
                    Candidate = true
                };
                dbEntity.OrderShippings.Add(os);
            }
            else
            {
                os.Candidate = true;
            }
            #endregion 设置订单的最终承运商
            #region 修改订单最终信息
            OrderInformation order = dbEntity.OrderInformations.Find(stockOut.RefID);
            order.Ostatus = (byte)ModelEnum.OrderStatus.DELIVERIED;
            //若是淘宝订单，需要另外做的修改
            if (order.Channel.ExtendType.Ctype == (byte)ModelEnum.StandardCategoryType.CHANNEL && order.Channel.Code == "Taobao")
            {
                ExTaobaoDeliveryPending TDelivery = (from t in dbEntity.ExTaobaoDeliveryPendings
                                                    where t.OrderID == stockOut.RefID
                                                       && !t.Deleted
                                                    select t).SingleOrDefault();
                if (TDelivery != null)
                {
                    TDelivery.ShipID = shipper;
                    TDelivery.tid = order.LinkCode;
                    TDelivery.out_sid = stockOut.Shipper.Code;
                }
                else
                {
                    TDelivery = new ExTaobaoDeliveryPending
                    {
                        OrderID = order.Gid,
                        ShipID = shipper,
                        tid = order.LinkCode,
                        out_sid = stockOut.Shipper.Code
                    };
                    dbEntity.ExTaobaoDeliveryPendings.Add(TDelivery);
                }
            }
            #endregion 修改订单最终信息
            dbEntity.SaveChanges();
            return RedirectToAction("StockOut");
        }
        /// <summary>
        /// 根据给定的仓库GUID获得支持的运输公司Grid下拉框数据
        /// </summary>
        /// <param name="whID">仓库GUID</param>
        /// <returns>下拉框Json数据</returns>
        public JsonResult GetDeliveryShippers(Guid whID)
        {
            List<ShippingInformation> shippings = (from ship in dbEntity.WarehouseShippings.Include("Shipper").Include("Shipper.FullName").Include("Shipper.FullName.ResourceItems")
                                                   where ship.WhID == whID
                                                      && !ship.Deleted
                                                   select ship.Shipper).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (ShippingInformation ship in shippings)
            {
                sb.Append(ship.Gid);
                sb.Append(':');
                sb.Append(ship.FullName.GetResource(CurrentSession.Culture));
                sb.Append(';');
            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion 出库扫描和承运页面

        #region 移库单/移货位单
        public ActionResult Moving()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoving temp = new WarehouseMoving();
            List<SelectListItem> list = base.GetSelectList(temp.MoveStatusList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.MStatus = list;
            list = base.GetSelectList(temp.MoveTypeList);
            list.Add(new SelectListItem
            {
                Text = "全部",
                Value = "255"
            });
            ViewBag.MType = list;
            return View();
        }
        /// <summary>
        /// 移库Grid页面
        /// </summary>
        /// <param name="whID">原仓库ID</param>
        /// <returns></returns>
        public PartialViewResult MovingGrid(Guid whID)
        {
            ViewBag.WHID = whID;
            return PartialView();
        }
        /// <summary>
        /// 生成移库列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult MovingList(SearchModel searchModel, Guid? whID = null, byte? status = null, byte? type = null, int? n = 0, string itemVal = "")
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            IEnumerable<WarehouseMoving> movings;
            if (itemVal == "")
            {
                string SKUName = "";
                string SKUCode = "";
                string SKUBarcode = "";
                DateTimeOffset? StartTime = new DateTimeOffset();
                switch (n)
                {
                    case 1: SKUName = itemVal; break;
                    case 2: SKUCode = itemVal; break;
                    case 3: SKUBarcode = itemVal; break;
                    case 4: StartTime = DateTime.Parse(itemVal); break;
                    default: break;
                }
                movings = (from mov in dbEntity.WarehouseMovings
                           join i in dbEntity.WarehouseMoveItems on mov.Gid equals i.MoveID
                           where (mov.OldWhID == whID || mov.NewWhID == whID)
                              && ((status != null && status != 255) ? mov.Mstatus == status : true)   //按状态搜索
                              && ((type != null && type != 255) ? mov.Mtype == type : true)    //按类型搜索
                              && ((SKUName != "") ? i.SkuItem.FullName.Matter.Contains(SKUName) : true)
                              && ((SKUCode != "") ? i.SkuItem.Code.Contains(SKUCode) : true)
                              && ((SKUBarcode != "") ? i.SkuItem.Barcode.Contains(SKUCode) : true)
                              && ((StartTime != null) ? mov.LastModifyTime > StartTime : true)
                              && !mov.Deleted
                           select mov).ToList().OrderByDescending(item => item.LastModifyTime);
            }
            else
            {
                movings = (from mov in dbEntity.WarehouseMovings
                           join i in dbEntity.WarehouseMoveItems on mov.Gid equals i.MoveID
                           where (mov.OldWhID == whID || mov.NewWhID == whID)
                              && ((status != null && status != 255) ? mov.Mstatus == status : true)   //按状态搜索
                              && ((type != null && type != 255) ? mov.Mtype == type : true)    //按类型搜索
                              && !mov.Deleted
                           select mov).ToList().OrderByDescending(item => item.LastModifyTime);
            }
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseMoving> columns = new GridColumnModelList<WarehouseMoving>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.Code);
            columns.Add(m => m.OldWarehouse.FullName.GetResource(culture)).SetName("OldWarehouse");
            columns.Add(m => m.NewWarehouse.FullName.GetResource(culture)).SetName("NewWarehouse");
            columns.Add(m => m.Mstatus);
            columns.Add(m => m.MoveStatusName);
            columns.Add(m => m.MoveTypeName);
            columns.Add(m => m.Reason);
            columns.Add(m => m.Total);
            columns.Add(m => GetUserName(m.Prepared)).SetName("Prepared");
            columns.Add(m => GetUserName(m.Approved)).SetName("Approved");
            columns.Add(m => GetTimeString(m.ApproveTime)).SetName("ApproveTime");
            columns.Add(m => m.Remark);
            GridData gridData = movings.AsQueryable().ToGridData(searchModel, columns);
            return Json(gridData, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 添加移库记录
        /// </summary>
        /// <returns></returns>
        public ActionResult MovingAdd()
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            List<WarehouseInformation> warehouses = GetPriWarehouses();
            WarehouseInformation firstWH = warehouses.First();  //第一个仓库
            WarehouseMoving model = new WarehouseMoving
            {
                OldWhID = firstWH.Gid,
                NewWhID = firstWH.Gid,
                Mtype = (byte)ModelEnum.MovingType.FOR_SHELF,
                ShipID = null,
            };
            #region 仓库下拉框
            List<SelectListItem> ddlWarehouse = GetWarehouseSelectList(warehouses);
            ViewBag.WarehouseList = ddlWarehouse;
            #endregion 仓库下拉框
            #region 移库类型数据
            List<SelectListItem> ddlMtype = (from item in model.MoveTypeList
                                             select new SelectListItem
                                             {
                                                 Text = item.Text,
                                                 Value = item.Value
                                             }).ToList();
            ViewBag.MtypeList = ddlMtype;
            #endregion 移库类型数据
            return View(model);
        }

        /// <summary>
        /// 编辑移库单
        /// </summary>
        /// <param name="moveID">移库单GUID</param>
        /// <returns></returns>
        public ActionResult MovingEdit(Guid moveID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoving model = dbEntity.WarehouseMovings.Find(moveID);
            if (model == null || model.Deleted)
            {
                //不存在记录
                return Error("记录不存在", Url.Action("Moving"));
            }
            if (model.Mstatus == (byte)ModelEnum.MovingStatus.CONFIRMED)
            {
                //已确认则不可以编辑
                return Error("已确认，不可编辑", Url.Action("Moving"));
            }
            #region 仓库下拉框
            List<WarehouseInformation> warehouses = GetPriWarehouses();
            List<SelectListItem> ddlWarehouse = GetWarehouseSelectList(warehouses);
            ViewBag.WarehouseList = ddlWarehouse;
            #endregion 仓库下拉框
            #region 移库类型数据
            List<SelectListItem> ddlMtype = (from item in model.MoveTypeList
                                                select new SelectListItem
                                                {
                                                    Text = item.Text,
                                                    Value = item.Value
                                                }).ToList();
            ViewBag.MtypeList = ddlMtype;
            #endregion 移库类型数据
            return View(model);
        }

        /// <summary>
        /// 移库记录详情页
        /// </summary>
        /// <param name="movID">移库记录ID</param>
        /// <returns></returns>
        public ActionResult MovingDetail(Guid moveID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoving model = dbEntity.WarehouseMovings.Find(moveID);
            if (model == null || model.Deleted)
            {
                //不存在记录
                return Error("记录不存在", Url.Action("Moving"));
            }
            return View(model);
        }

        /// <summary>
        /// 将移库记录添加到数据库
        /// </summary>
        /// <param name="model">从View接收到的移库记录</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MovingAddDB(WarehouseMoving model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            Guid orgID = dbEntity.WarehouseInformations.SingleOrDefault(item => item.Gid == model.OldWhID).aParent.Value;
            if (orgID == Guid.Empty)
            {
                return Error("组织不存在", Url.Action("Moving"));
            }
            WarehouseMoving newMove = new WarehouseMoving
            {
                OrgID = orgID,
                Prepared = CurrentSession.UserID,
                OldWhID = model.OldWhID,
                NewWhID = model.NewWhID,
                Mtype = model.Mtype,
                ShipID = (model.Mtype == (byte)ModelEnum.MovingType.FOR_SHELF) ? null : model.ShipID,
                Reason = model.Reason,
                Remark = model.Remark
            };
            dbEntity.WarehouseMovings.Add(newMove);
            dbEntity.SaveChanges();
            return RedirectToAction("MovingEdit", new { moveID = newMove.Gid });
        }

        /// <summary>
        /// 修改记录保存到数据库
        /// </summary>
        /// <param name="model">从View接收到的移库记录</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MovingEditDB(WarehouseMoving model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoving mov = dbEntity.WarehouseMovings.Find(model.Gid);
            if (mov == null || mov.Deleted)
            {
                return Error("记录不存在", Url.Action("Moving"));
            }
            if(mov.Mstatus == (byte)ModelEnum.MovingStatus.CONFIRMED)
            {
                return Error("已确认，不能编辑", Url.Action("Moving"));
            }
            mov.Mtype = model.Mtype;
            mov.ShipID = model.ShipID;
            mov.Reason = model.Reason;
            mov.Remark = model.Remark;
            dbEntity.SaveChanges();
            return RedirectToAction("Moving");
        }

        /// <summary>
        /// 执行删除移库记录到数据库
        /// </summary>
        /// <param name="movID">移库单ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MovingDeleteDB(Guid moveID)
        {
            if (!base.CheckPrivilege("EnableDelete"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int state = whBll.MovingDiscard(moveID, CurrentSession.UserID);
            bool result = (state == 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认移库单
        /// </summary>
        /// <param name="movID">移库单ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MovingConfirmDB(Guid moveID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            whBll = new WarehouseBLL(dbEntity);
            int result = whBll.MoveingConfirm(moveID, CurrentSession.UserID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消确认移库单
        /// </summary>
        /// <param name="movID">移库单ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MovingCancelConfirmDB(Guid moveID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//权限验证??????
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            bool result;
            WarehouseMoving moving = dbEntity.WarehouseMovings.Find(moveID);
            if (moving == null || moving.Deleted)
            {
                result = false;
            }
            else if (moving.Mstatus != (byte)ModelEnum.StockInStatus.CONFIRMED)
            {
                result = false;
            }
            else
            {
                byte moveType = (byte)ModelEnum.NoteType.MOVE;
                WarehouseStockOut stockOut = (from s in dbEntity.WarehouseStockOuts
                                              where s.RefType == moveType
                                                 && s.RefID == moveID
                                                 && !s.Deleted
                                              select s).Single();
                stockOut.Ostatus = (byte)ModelEnum.StockOutStatus.NONE;
                stockOut.Approved = null;
                stockOut.ApproveTime = null;
                moving.Mstatus = (byte)ModelEnum.MovingStatus.NONE;
                dbEntity.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取移库类型数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetMovingTypeName(byte type)
        {
            List<ListItem> mTypeList = new WarehouseMoving().MoveTypeList;
            string name = (from item in mTypeList
                           where item.Value == type.ToString()
                           select item.Text).Single();
            return name;
        }

        /// <summary>
        /// 获取指定仓库支持的运输公司的JSON数据
        /// </summary>
        /// <param name="whID">仓库ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetShippings(Guid whID)
        {
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<WarehouseShipping> shippings = (from ship in dbEntity.WarehouseShippings
                                                         .Include("Shipper").Include("Shipper.FullName").Include("Shipper.FullName.ResourceItems")
                                                     where ship.WhID == whID
                                                        && !ship.Deleted
                                                     select ship).ToList();
                var data = from ship in shippings
                           select new
                           {
                               Text = ship.Shipper.FullName.GetResource(CurrentSession.Culture),
                               Value = ship.ShipID
                           };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion 移库单/移货位单

        #region 移库单/移货位单明细表
        /// <summary>
        /// 查看移库记录明细
        /// </summary>
        /// <param name="moveID"><seealso cref="WarehouseMoving"/>对象的Gid</param>
        /// <returns></returns>
        public ActionResult MoveItem(Guid moveID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            ViewBag.privEnableApprove = GetProgramNode("EnableApprove");//允许确认
            ViewBag.privEnablePrepare = GetProgramNode("EnablePrepare");//允许制表(编辑)
            ViewBag.privEnableDelete = GetProgramNode("EnableDelete");//允许删除/作废
            WarehouseMoving model = dbEntity.WarehouseMovings.Find(moveID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("Moving"));
            }
            return View(model);
        }

        /// <summary>
        /// 获取<seealso cref="WarehouseMoveItem"/>对象列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="moveID"><seealso cref="WarehouseMoving"/>对象的Gid</param>
        /// <returns></returns>
        public JsonResult MoveItemList(SearchModel searchModel,Guid moveID)
        {
            IEnumerable<WarehouseMoveItem> items = (from item in dbEntity.WarehouseMoveItems
                                                    where item.MoveID == moveID
                                                       && !item.Deleted
                                                    select item).ToList().OrderByDescending(item => item.LastModifyTime);
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseMoveItem> columns = new GridColumnModelList<WarehouseMoveItem>();
            columns.Add(m => m.Gid).SetAsPrimaryKey();
            columns.Add(m => m.SkuItem.FullName.GetResource(culture)).SetName("SkuItem");
            columns.Add(m => m.ShelfOld.Name).SetName("ShelfOld");
            columns.Add(m => m.ShelfNew.Name).SetName("ShelfNew");
            columns.Add(m => m.Quantity);
            columns.Add(m => m.TrackLot);
            columns.Add(m => m.Remark);
            GridData data = items.AsQueryable().ToGridData(searchModel, columns);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult MoveItemGrid(Guid moveID)
        {
            ViewBag.MoveID = moveID;
            return PartialView();
        }

        /// <summary>
        /// 添加移库明细
        /// </summary>
        /// <param name="moveID">移库单Id</param>
        /// <returns></returns>
        public ActionResult MoveItemAdd(Guid moveID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoving mov = dbEntity.WarehouseMovings.Find(moveID);
            if (mov == null || mov.Deleted)
            {
                return Error("记录不存在", Url.Action("Moving"));
            }
            else
            {
                List<WarehouseShelf> oldShelves = mov.OldWarehouse.Shelves.ToList();
                List<WarehouseShelf> newShelves = mov.NewWarehouse.Shelves.ToList();
                WarehouseMoveItem model = new WarehouseMoveItem
                {
                    OldShelf = oldShelves.First().Gid,
                    NewShelf = newShelves.First().Gid,
                    MoveID = moveID,
                    Moving = mov,
                    Quantity = 1
                };
                #region 旧仓库下拉框
                List<SelectListItem> ddlOldShelves = (from shelf in oldShelves
                                                      select new SelectListItem
                                                      {
                                                          Text = shelf.Name,
                                                          Value = shelf.Gid.ToString()
                                                      }).ToList();
                ViewBag.OldShelves = ddlOldShelves;

                #endregion 旧仓库下拉框
                #region 新仓库下拉框
                List<SelectListItem> ddlNewShelves = (from shelf in newShelves
                                                      select new SelectListItem
                                                      {
                                                          Text = shelf.Name,
                                                          Value = shelf.Gid.ToString()
                                                      }).ToList();
                ViewBag.NewShelves = ddlNewShelves;

                #endregion 新仓库下拉框
                return View(model);
            }
        }

        /// <summary>
        /// 编辑已存在的移库单明细
        /// </summary>
        /// <param name="itemID">明细GUID</param>
        /// <returns></returns>
        public ActionResult MoveItemEdit(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoveItem item = dbEntity.WarehouseMoveItems.Include("Moving").SingleOrDefault(i => i.Gid == itemID);
            if (item == null || item.Deleted)
            {
                return Error("记录不存在", Url.Action("Moving"));
            }
            if (item.Moving.Mstatus == (byte)ModelEnum.MovingStatus.CONFIRMED)
            {
                return Error("已确认，不能编辑", Url.Action("Moving"));
            }
            List<WarehouseShelf> oldShelves = item.Moving.OldWarehouse.Shelves.ToList();
            List<WarehouseShelf> newShelves = item.Moving.NewWarehouse.Shelves.ToList();
            #region 旧仓库下拉框
            List<SelectListItem> ddlOldShelves = (from shelf in oldShelves
                                                  select new SelectListItem
                                                  {
                                                      Text = shelf.Name,
                                                      Value = shelf.Gid.ToString()
                                                  }).ToList();
            ViewBag.OldShelves = ddlOldShelves;

            #endregion 旧仓库下拉框
            #region 新仓库下拉框
            List<SelectListItem> ddlNewShelves = (from shelf in newShelves
                                                  select new SelectListItem
                                                  {
                                                      Text = shelf.Name,
                                                      Value = shelf.Gid.ToString()
                                                  }).ToList();
            ViewBag.NewShelves = ddlNewShelves;

            #endregion 新仓库下拉框
            return View(item);
        }

        /// <summary>
        /// 将移库明细添加到数据库
        /// </summary>
        /// <param name="model">从View接收到的移库明细</param>
        /// <returns></returns>
        [HttpPost]
        public RedirectToRouteResult MoveItemAddDB(WarehouseMoveItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoveItem item = new WarehouseMoveItem
            {
                SkuID = model.SkuID,
                OldShelf = model.OldShelf,
                NewShelf = model.NewShelf,
                Quantity = model.Quantity,
                TrackLot = model.TrackLot,
                MoveID = model.MoveID
            };
            ///移货位数量不能大于移库单总数量判断
            //////
            /////
            dbEntity.WarehouseMoveItems.Add(item);
            dbEntity.SaveChanges();
            WarehouseMoving moving = (from s in dbEntity.WarehouseMovings.Include("MoveItems")
                                          where s.Gid == item.MoveID
                                           && !s.Deleted
                                          select s).Single();
            moving.Total = moving.MoveItems.Select(i => i.Quantity).Sum();
            dbEntity.SaveChanges();
            whBll = new WarehouseBLL(dbEntity);
            whBll.InventoryByWarehouseSku(moving.OldWhID, item.SkuID);
            whBll.InventoryByWarehouseSku(moving.NewWhID, item.SkuID);
            return RedirectToAction("MovingEdit", new { moveID = model.MoveID });
        }

        /// <summary>
        /// 将编辑入库明细存入数据库
        /// </summary>
        /// <param name="model">从View接收到的明细对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MoveItemEditDB(WarehouseMoveItem model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseMoveItem item = dbEntity.WarehouseMoveItems.Include("Moving").SingleOrDefault(i => i.Gid == model.Gid);
            if (item == null || item.Deleted)
            {
                return Error("记录不存在", Url.Action("MovingEdit", new { moveID = model.MoveID }));
            }
            if (item.Moving.Mstatus == (byte)ModelEnum.MovingStatus.CONFIRMED)
            {
                return Error("已确认，不能编辑", Url.Action("MovingEdit", new { moveID = item.MoveID }));
            }
            item.SkuID = model.SkuID;
            item.OldShelf = model.OldShelf;
            item.NewShelf = model.NewShelf;
            item.Quantity = model.Quantity;
            item.TrackLot = model.TrackLot;
            dbEntity.SaveChanges();
            WarehouseMoving moving = (from s in dbEntity.WarehouseMovings.Include("MoveItems")
                                      where s.Gid == item.MoveID
                                       && !s.Deleted
                                      select s).Single();
            moving.Total = moving.MoveItems.Select(i => i.Quantity).Sum();
            dbEntity.SaveChanges();
            whBll = new WarehouseBLL(dbEntity);
            whBll.InventoryByWarehouseSku(moving.OldWhID, item.SkuID);
            whBll.InventoryByWarehouseSku(moving.NewWhID, item.SkuID);
            return RedirectToAction("MovingEdit", new { moveID = item.MoveID });
        }

        /// <summary>
        /// 执行删除移库明细到数据库
        /// </summary>
        /// <param name="itemID">移库明细ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MoveItemDeleteDB(Guid itemID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            WarehouseMoveItem item = dbEntity.WarehouseMoveItems.Find(itemID);
            if (item == null || item.Deleted)
            {
                //已删除
            }
            else
            {
                item.Deleted = true;
                dbEntity.SaveChanges();
                WarehouseMoving moving = (from s in dbEntity.WarehouseMovings.Include("MoveItems")
                                          where s.Gid == item.MoveID
                                           && !s.Deleted
                                          select s).Single();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(moving.OldWhID, item.SkuID);
                whBll.InventoryByWarehouseSku(moving.NewWhID, item.SkuID);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion 移库单/移货位单明细表

        #region 盘点记录

        /// <summary>
        /// 盘点记录列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult Inventory()
        {
            // 权限验证
            if (!base.CheckPrivilege())
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInventory temp = new WarehouseInventory();
            ViewBag.InvStatus = base.GetSelectList(temp.InventoryStatusList);
            return View();
        }

        /// <summary>
        /// 盘点的Grid部分页面
        /// </summary>
        /// <param name="whID">正在盘点的仓库GUID</param>
        /// <returns>Grid部分页</returns>
        public ActionResult InventoryGrid(Guid whID)
        {
            ViewBag.WHID = whID;
            return PartialView();
        }

        /// <summary>
        /// 生成盘点记录列表数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="whID">仓库GUID</param>
        /// <returns></returns>
        public JsonResult InventoryList(SearchModel searchModel, Guid? whID = null, byte? status = null,int? n = 0,string itemVal="")
        {
            if (whID == null)
                whID = GetCurrentUserPriWhl();
            IEnumerable<WarehouseInventory> invs;
            if (itemVal != null)
            {
                string SKUName = "";
                string SKUCode = "";
                string SKUBarcode = "";
                DateTimeOffset? StartTime = null;
                switch (n)
                {
                    case 1: SKUName = itemVal; break;
                    case 2: SKUCode = itemVal; break;
                    case 3: SKUBarcode = itemVal; break;
                    case 4: StartTime = DateTime.Parse(itemVal); break;
                    default: break;
                }
                invs = (from inv in dbEntity.WarehouseInventories
                        join i in dbEntity.WarehouseInvItems on inv.Gid equals i.InvID
                        where inv.WhID == whID
                            && ((status != null) ? inv.Istatus == status : true)   //按状态搜索
                            && ((SKUName != "") ? i.SkuItem.FullName.Matter.Contains(SKUName) : true)
                            && ((SKUCode != "") ? i.SkuItem.Code.Contains(SKUCode) : true)
                            && ((SKUBarcode != "") ? i.SkuItem.Barcode.Contains(SKUCode) : true)
                            && ((StartTime != null) ? inv.LastModifyTime > StartTime : true)
                            && !inv.Deleted
                        select inv).ToList().OrderByDescending(item => item.LastModifiedBy);
            }
            else
            {
                invs = (from inv in dbEntity.WarehouseInventories
                        join i in dbEntity.WarehouseInvItems on inv.Gid equals i.InvID
                        where inv.WhID == whID
                            && ((status != null) ? inv.Istatus == status : true)   //按状态搜索
                            && !inv.Deleted
                        select inv).ToList().OrderByDescending(item => item.LastModifiedBy);
            }
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseInventory> columns = new GridColumnModelList<WarehouseInventory>();
            columns.Add(x => x.Gid).SetAsPrimaryKey();
            columns.Add(x => x.Warehouse.FullName.GetResource(culture)).SetName("Warehouse");
            columns.Add(x => x.InventoryStatusName).SetName("Istatus");
            columns.Add(x => x.Quantity);
            columns.Add(x => GetUserName(x.Prepared)).SetName("Prepared");
            columns.Add(x => GetUserName(x.Approved)).SetName("Approved");
            columns.Add(x => GetTimeString(x.ApproveTime)).SetName("ApproveTime");
            columns.Add(x => x.Remark);
            GridData griddata = invs.AsQueryable().ToGridData(searchModel, columns);
            return Json(griddata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加仓库盘点单
        /// </summary>
        /// <returns></returns>
        public ActionResult InventoryAdd(Guid whID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.Find(whID);
            if (warehouse == null || warehouse.Deleted || warehouse.Parent == null)
            {
                return Error("仓库不存在", Url.Action("Inventory"));
            }
            WarehouseInventory model = new WarehouseInventory
            {
                OrgID = warehouse.aParent.Value,
                WhID = warehouse.Gid,
                Warehouse = warehouse,
                Organization = (MemberOrganization)warehouse.Parent
            };
            return View(model);
        }

        /// <summary>
        /// 编辑仓库盘点单
        /// </summary>
        /// <param name="invID">盘点单ID</param>
        /// <returns></returns>
        public ActionResult InventoryEdit(Guid invID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInventory model = dbEntity.WarehouseInventories.Find(invID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            else
            {
                if (model.Istatus == (byte)ModelEnum.InventoryStatus.CONFIRMED)
                {
                    return Error("已确认，不能编辑", Url.Action("inventory"));
                }
                return View(model);
            }
        }

        /// <summary>
        /// 将仓库盘点单添加到数据库
        /// </summary>
        /// <param name="model">从View接收到的盘点单对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InventoryAddDB(WarehouseInventory model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            bool result = false;
            WarehouseInventory inv = (from i in dbEntity.WarehouseInventories
                                      where i.OrgID == model.OrgID
                                         && i.Code == model.Code
                                      select i).SingleOrDefault();
            if (inv == null)
            {
                inv = new WarehouseInventory
                {
                    OrgID = model.OrgID,
                    WhID = model.WhID,
                    Code = model.Code,
                    Quantity = model.Quantity,
                    Prepared = CurrentSession.UserID,
                    Remark = model.Remark
                };
                dbEntity.WarehouseInventories.Add(inv);
                dbEntity.SaveChanges();
                result = true;
            }
            else if (inv.Deleted)
            {
                inv.Deleted = false;
                inv.WhID = model.WhID;
                inv.Quantity = model.Quantity;
                inv.Prepared = model.Prepared;
                inv.Remark = model.Remark;
                dbEntity.SaveChanges();
                result = true;
            }
            if (result)
            {
                return RedirectToAction("InventoryEdit", new { invID = inv.Gid });
            }
            else
            {
                return Error("记录冲突", Url.Action("InventoryAdd", new { whID = model.WhID }));
            }
        }

        /// <summary>
        /// 将编辑仓库盘点单保存到数据库
        /// </summary>
        /// <param name="model">从View接收到的盘点单对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InventoryEditDB(WarehouseInventory model)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInventory inv = dbEntity.WarehouseInventories.Find(model.Gid);
            if (inv == null || inv.Deleted)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            else
            {
                inv.Quantity = model.Quantity;
                inv.Remark = model.Remark;
                dbEntity.SaveChanges();
                return RedirectToAction("Inventory");
            }
        }

        /// <summary>
        /// 删除仓库盘点单
        /// </summary>
        /// <param name="invID">欲删除的盘点单ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InventoryDeleteDB(Guid invID)
        {
            if (!base.CheckPrivilege("EnableDelete"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            WarehouseInventory inv = dbEntity.WarehouseInventories.Find(invID);
            if (inv == null || inv.Deleted)
            {
                //
                // To do
                //
            }
            IEnumerable<WarehouseInvItem> items = inv.InvItems;
            foreach (WarehouseInvItem item in items)//删除对应的盘点明细
            {
                item.Deleted = true;
            }
            inv.Deleted = true;
            dbEntity.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认盘点单信息
        /// </summary>
        /// <param name="invID">盘点单ID</param>
        /// <returns>结果是否成功的JSON数据</returns>
        [HttpPost]
        public JsonResult InventoryConfirm(Guid invID)
        {
            if (!base.CheckPrivilege("EnableApprove"))//制表权限验证
                return Json("NoPrivilege", JsonRequestBehavior.AllowGet);
            int result = 5;
            WarehouseInventory inv = dbEntity.WarehouseInventories.Find(invID);
            if (inv == null || inv.Deleted)
            {
                result = 1; //盘点记录不存在或已作废
            }
            else if (inv.Istatus != (byte)ModelEnum.InventoryStatus.NONE)
            {
                result = 2; //盘点记录状态不正确
            }
            else
            {
                foreach (WarehouseInvItem item in inv.InvItems)
                {
                    WarehouseSkuShelf skuShelf = (from s in dbEntity.WarehouseSkuShelves
                                                  where s.WhID == inv.WhID
                                                     && s.ShelfID == item.ShelfID
                                                     && s.SkuID == item.SkuID
                                                  select s).SingleOrDefault();
                    if (skuShelf == null)
                    {
                        result = 3;//不存在相应的货架或SKU
                        break;
                    }
                    else if (item.Quantity != (skuShelf.Quantity + skuShelf.LockQty))
                    {
                        result = 4;//盘点结果与实际不符，需调整货位数量或盘点结果
                        break;
                    }
                }
                if (result == 5)
                {
                    inv.Istatus = (byte)ModelEnum.InventoryStatus.CONFIRMED;
                    inv.Approved = CurrentSession.UserID;
                    inv.ApproveTime = DateTime.Now;
                    dbEntity.SaveChanges();
                    result = 0;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Inventory详细页面
        /// </summary>
        /// <param name="invID">盘点GUID</param>
        /// <returns></returns>
        public ActionResult InventoryDetail(Guid invID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInventory model = dbEntity.WarehouseInventories.Find(invID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            return View(model);
        }

        #endregion 盘点记录

        #region 盘点明细

        /// <summary>
        /// 盘点明细页面
        /// </summary>
        /// <param name="invID">盘点单ID</param>
        /// <returns></returns>
        public ActionResult InvItem(Guid invID)
        {
            if (!base.CheckPrivilege("EnablePrepare"))//制表权限验证
                return RedirectToAction("ErrorPage", "Home", new { message = "Sorry you have no privilege to visit the Page" });
            WarehouseInventory model = dbEntity.WarehouseInventories.Find(invID);
            if (model == null || model.Deleted)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            return View(model);
        }

        public ActionResult InvItemGrid(Guid invID)
        {
            ViewBag.InvID = invID;
            return PartialView();
        }
        /// <summary>
        /// 盘点明细列表
        /// </summary>
        /// <param name="searchModel">SearchModel</param>
        /// <param name="invID">盘点单ID</param>
        /// <returns></returns>
        public JsonResult InvItemList(SearchModel searchModel, Guid? invID = null)
        {
            IEnumerable<WarehouseInvItem> invItems = (from item in dbEntity.WarehouseInvItems
                                                      where item.InvID == invID
                                                         && !item.Deleted
                                                      select item).ToList().OrderByDescending(item => item.LastModifyTime);
            int culture = CurrentSession.Culture;
            GridColumnModelList<WarehouseInvItem> columns = new GridColumnModelList<WarehouseInvItem>();
            columns.Add(x => x.Gid).SetAsPrimaryKey();
            columns.Add(x => x.SkuItem.FullName.GetResource(culture)).SetName("SkuItem");
            columns.Add(x => x.SkuItem.Code);
            columns.Add(x => x.Shelf.Name).SetName("Shelf");
            columns.Add(x => x.TrackLot);
            columns.Add(x => x.Quantity);
            columns.Add(x => x.Remark);
            GridData data = invItems.AsQueryable().ToGridData(searchModel, columns);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加盘点明细
        /// </summary>
        /// <param name="invID">盘点单ID</param>
        /// <returns></returns>
        public ViewResult InvItemAdd(Guid invID)
        {
            WarehouseInventory inv = dbEntity.WarehouseInventories.Find(invID);
            if(inv == null || inv.Deleted)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            else
            {
                WarehouseInvItem model = new WarehouseInvItem
                {
                    InvID = invID,
                    Inventory = inv
                };

                #region 仓库下拉框
                IEnumerable<WarehouseShelf> shelves = (from shelf in dbEntity.WarehouseShelves
                                                       where shelf.WhID == inv.WhID
                                                          && !shelf.Deleted
                                                       select shelf).ToList();
                IEnumerable<SelectListItem> shelfList = from shelf in shelves
                                                        select new SelectListItem
                                                        {
                                                            Text = shelf.Name,
                                                            Value = shelf.Gid.ToString()
                                                        };
                ViewBag.ShelfList = shelfList;
                #endregion 仓库下拉框

                return View(model);
            }
        }

        /// <summary>
        /// 编辑盘点单明细
        /// </summary>
        /// <param name="itemID">盘点单明细ID</param>
        /// <returns></returns>
        public ViewResult InvItemEdit(Guid itemID)
        {
            WarehouseInvItem item = dbEntity.WarehouseInvItems.Find(itemID);
            item = (from i in dbEntity.WarehouseInvItems.Include("Inventory")
                    where i.Gid == itemID
                       && !i.Deleted
                    select i).SingleOrDefault();
            if (item == null)
            {
                return Error("记录不存在", Url.Action("Inventory"));
            }
            else
            {
                #region 仓库下拉框
                IEnumerable<WarehouseShelf> shelves = (from shelf in dbEntity.WarehouseShelves
                                                       where shelf.WhID == item.Inventory.WhID
                                                          && !shelf.Deleted
                                                       select shelf).ToList();
                IEnumerable<SelectListItem> shelfList = from shelf in shelves
                                                        select new SelectListItem
                                                        {
                                                            Text = shelf.Name,
                                                            Value = shelf.Gid.ToString()
                                                        };
                ViewBag.ShelfList = shelfList;
                #endregion 仓库下拉框
                return View(item);
            }
        }

        /// <summary>
        /// 执行添加盘点单明细到数据库
        /// </summary>
        /// <param name="model">从View接收到的明细对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InvItemAddDB(WarehouseInvItem model)
        {
            WarehouseInvItem newItem = (from i in dbEntity.WarehouseInvItems
                                     where i.InvID == model.InvID
                                        && i.SkuID == model.SkuID
                                        && i.ShelfID == model.ShelfID
                                     select i).SingleOrDefault();
            if (newItem == null)
            {
                newItem = new WarehouseInvItem
                {
                    InvID = model.InvID,
                    SkuID = model.SkuID,
                    ShelfID = model.ShelfID,
                    TrackLot = model.TrackLot,
                    Quantity = model.Quantity,
                    Remark = model.Remark
                };
                dbEntity.WarehouseInvItems.Add(newItem);
            }
            else if (newItem.Deleted)
            {
                newItem.Deleted = false;
                newItem.TrackLot = model.TrackLot;
                newItem.Quantity = model.Quantity;
                newItem.Remark = model.Remark;
            }
            else
            {
                return Error("记录冲突", Url.Action("InventoryEdit", new { invID = model.InvID }));
            }
            dbEntity.SaveChanges();
            WarehouseInventory inventory = (from s in dbEntity.WarehouseInventories.Include("InvItems")
                                            where s.Gid == newItem.InvID
                                               && !s.Deleted
                                            select s).Single();
            inventory.Quantity = inventory.InvItems.Select(item => item.Quantity).Sum();
            dbEntity.SaveChanges();
            whBll = new WarehouseBLL(dbEntity);
            whBll.InventoryByWarehouseSku(inventory.WhID, model.SkuID);
            return RedirectToAction("InventoryEdit", new { invID = model.InvID });
            
        }

        /// <summary>
        /// 执行编辑盘点单明细到数据库
        /// </summary>
        /// <param name="model">从View接收到的明细对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InvItemEditDB(WarehouseInvItem model)
        {
            WarehouseInvItem item = dbEntity.WarehouseInvItems.Find(model.Gid);
            if (item == null || item.Deleted)
            {
                return Error("记录不存在", Url.Action("InventoryEdit", new { invID = model.InvID }));
            }
            else
            {
                item.SkuID = model.SkuID;
                item.ShelfID = model.ShelfID;
                item.TrackLot = model.TrackLot;
                item.Quantity = model.Quantity;
                item.Remark = model.Remark;
                dbEntity.SaveChanges();
                WarehouseInventory inventory = (from s in dbEntity.WarehouseInventories.Include("InvItems")
                                                where s.Gid == item.InvID
                                                   && !s.Deleted
                                                select s).Single();
                inventory.Quantity = inventory.InvItems.Select(i => i.Quantity).Sum();
                dbEntity.SaveChanges();
                whBll = new WarehouseBLL(dbEntity);
                whBll.InventoryByWarehouseSku(inventory.WhID, model.SkuID);
                return RedirectToAction("InventoryEdit", new { invID = model.InvID });
            }
        }

        /// <summary>
        /// 执行删除盘点单明细到数据库
        /// </summary>
        /// <param name="itemID">盘点单明细ID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InvItemDeleteDB(Guid itemID)
        {
            WarehouseInvItem item = dbEntity.WarehouseInvItems.Find(itemID);
            if (item == null || item.Deleted)
            {
                //
                // To do
                //
            }
            else
            {
                item.Deleted = true;
                dbEntity.SaveChanges();
                whBll = new WarehouseBLL(dbEntity);
                WarehouseInventory inventory = (from s in dbEntity.WarehouseInventories.Include("InvItems")
                                                where s.Gid == item.InvID
                                                   && !s.Deleted
                                                select s).Single();
                whBll.InventoryByWarehouseSku(inventory.WhID, item.SkuID);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion 盘点明细

        #region 打印单据

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        public ActionResult PrintStockOut(Guid? stockOutId)
        {
            List<StockOutBill> oStockOutBills = new List<StockOutBill>(); //StockOutBill的集合，传到页面后实现多单据打印
            List<Guid> oStockOutList = new List<Guid>();
            if (stockOutId != null)
            {
                oStockOutList.Add((Guid)stockOutId);
            }
            foreach (Guid item in oStockOutList)
            {
                List<StockOutSku> oOutSkus=new List<StockOutSku>();

                WarehouseStockOut oStockOut = dbEntity.WarehouseStockOuts.Include("Shipper").Single(o => o.Gid == item); //出库单
                oStockOut.Shipper.FullName.Matter = oStockOut.Shipper.FullName.GetResource(CurrentSession.Culture); //将Matter字段赋值，便于页面的多语言显示

                OrderInformation oOrder = dbEntity.OrderInformations.Single(o => o.Gid == oStockOut.RefID); //关联的订单
                oOrder.Organization.FullName.Matter = oOrder.Organization.FullName.GetResource(CurrentSession.Culture);
                oOrder.PayType.Name.Matter = oOrder.PayType.Name.GetResource(CurrentSession.Culture);
                oOrder.Currency.Name.Matter = oOrder.Currency.Name.GetResource(CurrentSession.Culture);

                //出库单对应的出库单明细
                List<WarehouseOutItem> oStockOutItems = dbEntity.WarehouseOutItems.Include("SkuItem").Where(o => o.OutID == oStockOut.Gid && o.Deleted == false).ToList();
                
                //根据出库单明细，查出每个明细中的商品 所对应的OrderItem, 以便于查出该SKU的执行价格
                foreach (WarehouseOutItem outItem in oStockOutItems)
                {
                    outItem.SkuItem.FullName.Matter = outItem.SkuItem.FullName.GetResource(CurrentSession.Culture);
                    OrderItem oOrderItem = dbEntity.OrderItems.Where(o => o.OrderID == oOrder.Gid && o.SkuID == outItem.SkuID).SingleOrDefault();
                    oOutSkus.Add(new StockOutSku(outItem, oOrderItem));
                }

                StockOutBill oOutBill = new StockOutBill(oStockOut, oOrder, oOutSkus);
                oStockOutBills.Add(oOutBill);
            }
            return PartialView(oStockOutBills);
        }

        /// <summary>
        /// 打印入库单
        /// </summary>
        /// <param name="stockInId"></param>
        /// <returns></returns>
        public ActionResult PrintStockIn(Guid? stockInId)
        {
            List<Guid> oStockInGids = new List<Guid>();
            List<StockInBill> oStockInBillList = new List<StockInBill>();
            if (stockInId != null)
            {
                oStockInGids.Add((Guid)stockInId);
            }
            foreach (Guid item in oStockInGids)
            {
                List<StockInSku> oInSkus = new List<StockInSku>();
                List<StockInSkuAndPurchase> oInSkuAndPurchases = new List<StockInSkuAndPurchase>();

                WarehouseStockIn oStockIn = dbEntity.WarehouseStockIns.Include("Warehouse").Include("StockInType").Where(o => o.Gid == item).SingleOrDefault();
                oStockIn.StockInType.Name.Matter = oStockIn.StockInType.Name.GetResource(CurrentSession.Culture);
                oStockIn.Warehouse.FullName.Matter = oStockIn.Warehouse.FullName.GetResource(CurrentSession.Culture);
                List<WarehouseInItem> oInItems = dbEntity.WarehouseInItems.Where(o => o.InID == oStockIn.Gid && o.Deleted == false).ToList();
                
                if (oStockIn.RefType == (byte)ModelEnum.NoteType.ORDER)
                {
                    OrderInformation oOrder = dbEntity.OrderInformations.Where(o => o.Gid == oStockIn.RefID).SingleOrDefault();
                    oOrder.Currency.Name.Matter = oOrder.Currency.Name.GetResource(CurrentSession.Culture);
                    foreach (WarehouseInItem inItem in oInItems)
                    {
                        OrderItem oItem = dbEntity.OrderItems.Where(o => o.SkuID == inItem.SkuID && o.OrderID == oOrder.Gid).SingleOrDefault();
                        inItem.SkuItem.FullName.Matter = inItem.SkuItem.FullName.GetResource(CurrentSession.Culture);
                        oInSkus.Add(new StockInSku(inItem, oItem));
                    }
                    oStockInBillList.Add(new StockInBill(oStockIn, oOrder, oInSkus));  //当关联单据为订单时，调用此构造方法
                }
                if (oStockIn.RefType == (byte)ModelEnum.NoteType.PURCHASE)
                {
                    PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(o => o.Gid == oStockIn.RefID).SingleOrDefault();
                    oPurchase.Currency.Name.Matter = oPurchase.Currency.Name.GetResource(CurrentSession.Culture);
                    oPurchase.Supplier.FullName.Matter = oPurchase.Supplier.FullName.GetResource(CurrentSession.Culture);
                    foreach (WarehouseInItem inItem in oInItems)
                    {
                        PurchaseItem oItem = dbEntity.PurchaseItems.Where(o => o.SkuID == inItem.SkuID && o.PurID == oPurchase.Gid).SingleOrDefault();
                        inItem.SkuItem.FullName.Matter = inItem.SkuItem.FullName.GetResource(CurrentSession.Culture);
                        oInSkuAndPurchases.Add(new StockInSkuAndPurchase(inItem, oItem));
                    }
                    oStockInBillList.Add(new StockInBill(oStockIn, oPurchase, oInSkuAndPurchases));  //当关联单据为采购单时，调用此构造方法
                }
            }
            return View(oStockInBillList);
        }

        /// <summary>
        /// 打印库存总帐
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintLedger()
        {
            List<WarehouseLedger> oList = dbEntity.WarehouseLedgers.Include("Warehouse").Include("SkuItem").ToList();
            return View(oList);
        }

        /// <summary>
        /// 打印快递单
        /// </summary>
        public void PrintEnvelope(Guid? sGid,byte refType,string RefCode)
        {
            ShippingEnvelope oShippingTemplate = dbEntity.ShippingEnvelopes.Where(s => s.ShipID == new Guid("ee76e2b5-87ff-e011-80e2-60eb69d65ae8") && s.Deleted == false).Single();
            string text = ReplaceClomns(oShippingTemplate.Template.CLOB, refType, RefCode);

            string[] ArrayFormText = text.Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);  //以‘\r’,‘\n’为标记将text分成若干字符串数组，去除空数组
            List<string[]> oList = new List<string[]>();
            foreach (string item in ArrayFormText)
            {
                string[] strArray = item.Split(',');
                oList.Add(strArray);
            }

            Application.Run(new PrintForm(oList));   //运行打印Form
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="text"></param>
        /// <param name="refType">关联单据类型</param>
        /// <param name="RefCode">关联单据号</param>
        /// <returns></returns>
        public string ReplaceClomns(string text, byte refType, string RefCode)
        {
            switch (refType)
            {
                case (byte)ModelEnum.NoteType.ORDER:
                    {
                        OrderInformation oOrder = dbEntity.OrderInformations.Include("User").Include("Organization").Where(o => o.Code == RefCode).Single();
                        text = text.Replace("{From}", oOrder.Organization.ShortName.GetResource(CurrentSession.Culture))
                        .Replace("{FromAddress}", oOrder.Organization.FullAddress)
                        .Replace("{Departure}", oOrder.Organization.Location.FullName)
                        .Replace("{FromCompany}", oOrder.Organization.FullName.Matter)
                        .Replace("{FromTelephone}", oOrder.Organization.CellPhone)
                        .Replace("{FromPostcode}", oOrder.Organization.PostCode)
                        .Replace("{To}", oOrder.Consignee)
                        .Replace("{Destination}", oOrder.Location.FullName)
                        .Replace("{ToAddress}", oOrder.FullAddress)
                        .Replace("{ToCompany}", oOrder.User.NickName)
                        .Replace("{ToTelephone}", oOrder.Telephone)
                        .Replace("{ToPostcode}", oOrder.PostCode)
                        .Replace("{PostDate}", "计件日期")
                        .Replace("{Remark}", "备注")
                        .Replace("{Amount}", Convert.ToString((int)oOrder.Pieces))
                        .Replace("{Weight}", "称重")
                        .Replace("{Charge}", "费用")
                        .Replace("{InsuranceFee}", "保价费")
                        .Replace("{TotalFee}", "总计");
                        break;
                    }
                case (byte)ModelEnum.NoteType.PURCHASE:
                    {
                        PurchaseInformation oPurchase = dbEntity.PurchaseInformations.Where(p => p.Code == RefCode).Single();
                        text = text.Replace("{From}", oPurchase.Organization.ShortName.Matter)
                        .Replace("{FromAddress}", oPurchase.Organization.FullAddress)
                        .Replace("{Departure}", oPurchase.Organization.Location.FullName)
                        .Replace("{FromCompany}", oPurchase.Organization.FullName.Matter)
                        .Replace("{FromTelephone}", oPurchase.Organization.CellPhone)
                        .Replace("{FromPostcode}", oPurchase.Organization.PostCode)
                        .Replace("{To}", oPurchase.Supplier.ShortName.Matter)
                        .Replace("{Destination}", oPurchase.Supplier.Location.FullName)
                        .Replace("{ToAddress}", oPurchase.Supplier.FullAddress)
                        .Replace("{ToCompany}", oPurchase.Supplier.FullName.Matter)
                        .Replace("{ToTelephone}", oPurchase.Supplier.WorkPhone)
                        .Replace("{ToPostcode}", oPurchase.Supplier.PostCode)
                        .Replace("{PostDate}", "计件日期")
                        .Replace("{Remark}", "备注")
                        .Replace("{Amount}", Convert.ToString((int)oPurchase.Quantity))
                        .Replace("{Weight}", "称重")
                        .Replace("{Charge}", "费用")
                        .Replace("{InsuranceFee}", "保价费")
                        .Replace("{TotalFee}", "总计");
                        break;
                    }
            }
            return text;
        }

        #endregion

        //====================================================================================================
        #region 公用

        private ModelEnum.NoteType GetRefType(Guid typeID)
        {
            GeneralStandardCategory type = dbEntity.GeneralStandardCategorys.Find(typeID);
            if (type == null || type.Deleted)
            {
                //错误处理
                throw new RefTypeNotFoundExcetpion();
            }
            ModelEnum.NoteType refType;
            #region 判断单据类型
            switch (type.Code)
            {
                case "PurchaseIn":
                case "ReturnSupplier":
                    refType = ModelEnum.NoteType.PURCHASE; break;
                case "ReturnIn":
                case "Sale":
                case "Resend":
                    refType = ModelEnum.NoteType.ORDER; break;
                case "MoveIn":
                case "MoveOut":
                    refType = ModelEnum.NoteType.MOVE; break;
                default:
                    refType = ModelEnum.NoteType.NONE; break;
            }

            #endregion 判断单据类型
            return refType;
        }
        public class RefTypeNotFoundExcetpion : Exception
        {
            public RefTypeNotFoundExcetpion()
                : base()
            {
            }
        }
        /// <summary>
        /// 获取给定标准分类对应的RefType数据
        /// </summary>
        /// <param name="typeID">标准分类GUID</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRefTypeJson(Guid typeID)
        {
            ModelEnum.NoteType refType;
            try
            {
                refType = GetRefType(typeID);
                return Json(refType, JsonRequestBehavior.AllowGet);
            }
            catch (RefTypeNotFoundExcetpion)
            {
                //错误处理
                var data = new
                {
                    State = false    //是否需要单据
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 得到当前登录用户具有权限的仓库列表
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<WarehouseInformation> GetPriWarehouses(Guid? parent = null)
        {
            List<WarehouseInformation> warehouses;
            if (CurrentSession.IsAdmin)
            {
                warehouses = (from w in dbEntity.WarehouseInformations
                              where w.Otype == (byte)ModelEnum.OrganizationType.WAREHOUSE
                                 && !w.Deleted
                              select w).ToList();
            }
            else
            {
                warehouses = (from w in dbEntity.WarehouseInformations
                              join p in Permission(ModelEnum.UserPrivType.WAREHOUSE)
                              on w.Gid equals p
                              where parent == null ? true : w.aParent == parent
                              && !w.Deleted
                              select w).ToList();
            }
            return warehouses;
        }

        /// <summary>
        /// 返回Warehouse的下拉选列表
        /// </summary>
        /// <param name="warehouses">下拉列表里面的Warehouse实例</param>
        /// <returns>返回Warehouse的下拉列表的List</returns>
        public List<SelectListItem> GetWarehouseSelectList(IEnumerable<WarehouseInformation> warehouses)
        {
            List<SelectListItem> list = (from warehouse in warehouses
                                         select new SelectListItem
                                         {
                                             Text = warehouse.FullName.Matter,
                                             Value = warehouse.Gid.ToString()
                                         }).ToList();
            return list;
        }
        /// <summary>
        /// 得到当前登录用户拥有privilege的组织的 List
        /// </summary>
        /// <returns>返回当前登录用户具有权限的organization的 List</returns>
        private List<SelectListItem> GetUserPrivilegeOrg()
        {
            List<MemberOrganization> orgs = (from item in dbEntity.MemberOrganizations
                                             join orgID in Permission(ModelEnum.UserPrivType.ORGANIZATION)
                                             on item.Gid equals orgID
                                             where !item.Deleted
                                             select item).ToList();
            List<SelectListItem> privilegeItems = (from org in orgs
                                                   select new SelectListItem
                                                   {
                                                       Text = org.FullName.GetResource(CurrentSession.Culture),
                                                       Value = org.Gid.ToString(),
                                                   }).ToList();
            return privilegeItems;
        }
        /// <summary>
        /// 得到当前登录用户有权限管理的仓库的 List
        /// </summary>
        /// <param name="selectedOrgGid">当前登陆用户具有权限且被选中的组织Guid，作为即将备选仓库的父节点Guid</param>
        /// <returns>返回当前登录用户有权限管理的仓库的 List </returns>
        private List<SelectListItem> GetUserPrivilegeWhl(Guid? selectedOrgGid)
        {
            if (selectedOrgGid == null || selectedOrgGid == Guid.Empty)
            {
                MemberUser currentUser = oGeneralBLL.getUser(CurrentSession.UserID);
                selectedOrgGid = currentUser.OrgID;
            }
            List<WarehouseInformation> warehouses = GetPriWarehouses(selectedOrgGid);
            return GetWarehouseSelectList(warehouses);
        }

        /// <summary>
        /// 获取用户有权限的组织集合
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MemberOrganization> GetPriOrganizations()
        {
            IEnumerable<MemberOrganization> orgs = (from org in dbEntity.MemberOrganizations
                                                    join orgID in Permission(ModelEnum.UserPrivType.ORGANIZATION)
                                                    on org.Gid equals orgID
                                                    where !org.Deleted
                                                    select org).ToList();
            return orgs;
        }

        /// <summary>
        /// 得到用户具有权限的第一个仓库ID
        /// </summary>
        /// <returns></returns>
        private Guid GetCurrentUserPriWhl()
        {
            MemberPrivilege pri = (from item in dbEntity.MemberPrivileges
                                   where item.UserID == CurrentSession.UserID
                                      && item.Ptype == (byte)ModelEnum.UserPrivType.WAREHOUSE
                                      && item.Pstatus == (byte)ModelEnum.UserStatus.VALID
                                      && !item.Deleted
                                   select item).FirstOrDefault();
            Guid orgGid = (from o in dbEntity.MemberOrganizations
                           join orgID in base.Permission(ModelEnum.UserPrivType.ORGANIZATION)
                           on o.Gid equals orgID
                           select orgID).FirstOrDefault();
            Guid whID = (from w in dbEntity.WarehouseInformations
                         join p in Permission(ModelEnum.UserPrivType.WAREHOUSE)
                         on w.Gid equals p
                         where w.aParent == orgGid
                            && !w.Deleted
                         select w.Gid).FirstOrDefault();
            return whID;
        }
        #endregion 公用

        #region 公用部分页

        /// <summary>
        /// 获得用户有权限组织下拉框
        /// </summary>
        /// <param name="id">select元素Id，可空，默认为随机Guid</param>
        /// <param name="selectedOrg">选中的组织Gid，可空，默认为第一个</param>
        /// <returns></returns>
        public PartialViewResult WarehouseOrgSelect(string id = null, Guid? selectedOrg = null)
        {
            List<MemberOrganization> orgs;
            if (CurrentSession.IsAdmin)//超级管理员
            {
                orgs = (from org in dbEntity.MemberOrganizations
                       where !org.Deleted
                       select org).ToList();
            }
            else
            {
                orgs = (from org in dbEntity.MemberOrganizations
                        join orgID in base.Permission(ModelEnum.UserPrivType.ORGANIZATION)
                        on org.Gid equals orgID
                        select org).ToList();
            }
            int currentCulture = CurrentSession.Culture;
            
            IEnumerable<SelectListItem> orgList = from org in orgs
                                                  select new SelectListItem
                                                  {
                                                      Text = org.FullName.GetResource(currentCulture),
                                                      Value = org.Gid.ToString(),                                                      
                                                  };
            ViewBag.OrgList = orgList;
            ViewBag.ID = id;
            if (selectedOrg == null)
                if (orgs.Count > 0)//edit by tianyou 2011/10/15 添加判断 orgs不为空
                    selectedOrg = orgs.First().Gid;
            //设置选中组织
            OrgSelected =(Guid)selectedOrg;
            ViewBag.SelectedOrg = selectedOrg;
            return PartialView();
        }

        /// <summary>
        /// 获取指定组织支持的仓库下拉框
        /// </summary>
        /// <param name="id">select元素Id，可空，默认为随机Guid</param>
        /// <param name="orgID">组织Id，若为空，则返回所有组织的仓库下拉框</param>
        /// <param name="selectedWarehouse">选中的仓库，可空，默认为第一个</param>
        /// <returns></returns>
        public PartialViewResult WarehouseSelect(string id = null, Guid? orgID = null, Guid? selectedWarehouse = null)
        {
            List<WarehouseInformation> warehouses;
            if (CurrentSession.IsAdmin)
            {
                warehouses = (from w in dbEntity.WarehouseInformations
                              where orgID == null ? true : w.aParent == orgID
                                 && !w.Deleted
                              select w).ToList();
            }
            else
            {
                warehouses = (from w in dbEntity.WarehouseInformations
                              join p in Permission(ModelEnum.UserPrivType.WAREHOUSE)
                              on w.Gid equals p
                              where orgID == null ? true : w.aParent == orgID
                                 && !w.Deleted
                              select w).ToList();
            }
            int currentCulture = CurrentSession.Culture;
            IEnumerable<SelectListItem> warehouseList = from w in warehouses
                                                        select new SelectListItem
                                                        {
                                                            Text = w.FullName.GetResource(currentCulture),
                                                            Value = w.Gid.ToString()
                                                        };
            ViewBag.WarehouseList = warehouseList;
            ViewBag.ID = id;
            if (selectedWarehouse == null)
                if (warehouses.Count > 0)//edit by tianyou 2011/10/15 添加判断 orgs不为空
                    selectedWarehouse = warehouses.First().Gid;
            WarehouseSelected =(Guid)selectedWarehouse;
            ViewBag.SelectedWarehouse = selectedWarehouse;
            return PartialView();
        }

        /// <summary>
        /// 仓库操作错误提示alert页面
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="url">回调URL</param>
        /// <returns></returns>
        public ViewResult Error(string message, string url)
        {
            ViewBag.Message = message;
            ViewBag.Url = url;
            return View("WarehouseError");
        }
        #endregion 公用部分页
    }

    #region  自定义类

    /// <summary>
    /// 发货单模板类,包括WarehouseStockOut,OrderInformation,List<StockOutSku>
    /// </summary>
    public class StockOutBill
    {
        private WarehouseStockOut oStockOut;
        private OrderInformation oOrder;
        private List<StockOutSku> oOutSkus;

        public StockOutBill() { }

        public StockOutBill( WarehouseStockOut oStockOut, OrderInformation oOrder, List<StockOutSku> oOutSkus)
        {
            this.oStockOut = oStockOut;
            this.oOrder = oOrder;
            this.oOutSkus = oOutSkus;
        }

        public WarehouseStockOut StockOut
        {
            get
            {
                return oStockOut;
            }
            set
            {
                oStockOut = value;
            }
        }

        public OrderInformation Order
        {
            get
            {
                return oOrder;
            }
            set
            {
                oOrder = value;
            }
        }

        public List<StockOutSku> OutSkus
        {
            get
            {
                return oOutSkus;
            }
            set
            {
                oOutSkus = value;
            }
        }
    }

    /// <summary>
    /// 入库单模板类，有两种情况：1.WarehouseStockIn，OrderInformation，List<StockInSku> 2.WarehouseStockIn，PurchaseInformation，List<StockInSkuAndPurchase>
    /// </summary>
    public class StockInBill
    {
        private WarehouseStockIn oStockIn;
        private OrderInformation oOrder;
        private List<StockInSku> oStockInSkus;
        private PurchaseInformation oPurchase;
        private List<StockInSkuAndPurchase> oInSkuAndPurchase;

        public StockInBill() { }

        public StockInBill(WarehouseStockIn oStockIn, OrderInformation oOrder, List<StockInSku> oStockInSkus,PurchaseInformation oPurchase, List<StockInSkuAndPurchase> oInSkuAndPurchase)
        {
            this.oStockIn = oStockIn;
            this.oOrder = oOrder;
            this.oStockInSkus = oStockInSkus;
            this.oPurchase = oPurchase;
            this.oInSkuAndPurchase = oInSkuAndPurchase;
        }

        public StockInBill(WarehouseStockIn oStockIn, OrderInformation oOrder, List<StockInSku> oStockInSkus)
        {
            this.oStockIn = oStockIn;
            this.oOrder = oOrder;
            this.oStockInSkus = oStockInSkus;
        }

        public StockInBill(WarehouseStockIn oStockIn, PurchaseInformation oPurchase, List<StockInSkuAndPurchase> oInSkuAndPurchase)
        {
            this.oStockIn = oStockIn;
            this.oPurchase = oPurchase;
            this.oInSkuAndPurchase = oInSkuAndPurchase;
        }

        public WarehouseStockIn StockIn
        {
            get
            {
                return oStockIn;
            }
            set
            {
                oStockIn = value;
            }
        }
        public OrderInformation Order
        {
            get
            {
                return oOrder;
            }
            set
            {
                oOrder = value;
            }
        }
        public List<StockInSku> StockInSkus
        {
            get
            {
                return oStockInSkus;
            }
            set
            {
                oStockInSkus = value;
            }
        }
        public PurchaseInformation Purchase
        {
            get
            {
                return oPurchase;
            }
            set
            {
                oPurchase = value;
            }
        }
        public List<StockInSkuAndPurchase> InSkuAndPurchase
        {
            get
            {
                return oInSkuAndPurchase;
            }
            set
            {
                oInSkuAndPurchase = value;
            }
        }
    }

    /// <summary>
    /// 出库单模板中间类，包括WarehouseOutItem和对应SKU的OrderItem
    /// </summary>
    public class StockOutSku
    {
        private WarehouseOutItem oOutItem;
        private OrderItem oOderItem;

        public StockOutSku() { }
        public StockOutSku(WarehouseOutItem oOutItem, OrderItem oOderItem)
        {
            this.oOutItem = oOutItem;
            this.oOderItem = oOderItem;
        }

        public WarehouseOutItem OutItem
        {
            get
            {
                return oOutItem;
            }
            set
            {
                oOutItem = value;
            }
        }

        public OrderItem OderItem
        {
            get
            {
                return oOderItem;
            }
            set
            {
                oOderItem = value;
            }
        }
    }

    /// <summary>
    /// 入库单模板中间类，包括WarehouseInItem和对应SKU的OrderItem
    /// </summary>
    public class StockInSku
    {
        private WarehouseInItem oInItem;
        private OrderItem oOderItem;

        public StockInSku() { }
        public StockInSku(WarehouseInItem oInItem, OrderItem oOderItem)
        {
            this.oInItem = oInItem;
            this.oOderItem = oOderItem;
        }

        public WarehouseInItem InItem
        {
            get
            {
                return oInItem;
            }
            set
            {
                oInItem = value;
            }
        }

        public OrderItem OderItem
        {
            get
            {
                return oOderItem;
            }
            set
            {
                oOderItem = value;
            }
        }
    }

    /// <summary>
    /// 入库单模板中间类，包括WarehouseInItem和对应SKU的PurchaseItem
    /// </summary>
    public class StockInSkuAndPurchase
    {
        private WarehouseInItem oInItem;
        private PurchaseItem oPurchaseItem;

        public StockInSkuAndPurchase() { }
        public StockInSkuAndPurchase(WarehouseInItem oInItem, PurchaseItem oPurchaseItem)
        {
            this.oInItem = oInItem;
            this.oPurchaseItem = oPurchaseItem;
        }

        public WarehouseInItem InItem
        {
            get
            {
                return oInItem;
            }
            set
            {
                oInItem = value;
            }
        }

        public PurchaseItem PurchaseItem
        {
            get
            {
                return oPurchaseItem;
            }
            set
            {
                oPurchaseItem = value;
            }
        }
    }

}
    #endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LiveAzure.BLL;
using LiveAzure.Models;
using LiveAzure.Models.General;
using LiveAzure.Models.Member;
using LiveAzure.Utility;
using LiveAzure.Controls.LiveRegionSelector;
using System.IO;
using LiveAzure.Models.Warehouse;
using LiveAzure.Models.Product;

namespace LiveAzure.Stage.Controllers
{
    public partial class BaseController : Controller
    {
        #region 地区选择器

        /// <summary>
        /// 根据GUID获取地区详细地址
        /// </summary>
        /// <param name="gid">地区Guid</param>
        /// <returns></returns>
        [HttpPost]
        public string GetRegionAddress(Guid? gid)
        {
            string address = string.Empty;
            GeneralRegion region = (from r in dbEntity.GeneralRegions
                                    where r.Gid == gid
                                       && r.Deleted == false
                                    select r).FirstOrDefault();
            if (region != null)
                address = region.GetRegionAddress();
            return address;
        }

        #endregion 地区选择器

        #region 程序选择器

        [HttpPost]
        public string GetProgramName(Guid? pid)
        {
            GeneralProgram program = (from p in dbEntity.GeneralPrograms.Include("Name").Include("Name.ResourceItems")
                                      where p.Gid == pid
                                         && !p.Deleted
                                      select p).SingleOrDefault();
            if (program == null)
            {
                return string.Empty;
            }
            else
            {
                return program.Name.GetResource(CurrentSession.Culture);
            }
        }

        #endregion 程序选择器

        #region 图片选择器

        /// <summary>
        /// 根据指定路径获取图片路径
        /// </summary>
        /// <param name="rootURL"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetImageNames(string rootURL)
        {
            string rootPath = System.Web.HttpContext.Current.Server.MapPath(rootURL);
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            FileInfo[] files = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
            List<string> list = new List<string>();
            foreach (FileInfo file in files)
            {
                list.Add(file.Name);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion 图片选择器

        #region 私有分类选择器

        public string PrivateCategorySelectLoad(byte categoryType, Guid? id = null, Guid? orgID = null)
        {
            //若传入组织ID为空则自动获取当前用户所属组织ID
            if (orgID == null)
            {
                orgID = (from u in dbEntity.MemberUsers
                         where u.Gid == CurrentSession.UserID
                            && u.Deleted == false
                         select u.OrgID).Single();
            }
            List<Guid> list = Permission(ModelEnum.UserPrivType.PRODUCT_CATEGORY);
            List<GeneralPrivateCategory> categories = (from c in dbEntity.GeneralPrivateCategorys.Include("Name").Include("Name.ResourceItems").AsEnumerable()
                                                       //join gid in list.AsEnumerable() on c.Gid equals gid
                                                       where c.Deleted == false
                                                          && id == null ? c.aParent.Equals(id) : c.aParent == id
                                                          && c.OrgID == orgID
                                                       select c).ToList();
            List<LiveTreeNode> nodes = new List<LiveTreeNode>();
            if (id == null)
            {
                foreach (GeneralPrivateCategory category in categories)
                {
                    LiveTreeNode node = new LiveTreeNode
                    {
                        name = category.Name.GetResource(CurrentSession.Culture),
                        id = category.Gid.ToString(),
                        isParent = category.ChildItems.Any(),
                        open = category.ChildItems.Any(),
                        nodes = (from c in category.ChildItems
                                 where categories.Contains(c)
                                 select new LiveTreeNode
                                 {
                                     name = c.Name.GetResource(CurrentSession.Culture),
                                     id = c.Gid.ToString(),
                                     isParent = c.ChildItems.Any()
                                 }).ToList()
                    };
                    nodes.Add(node);
                }
            }
            else
            {
                foreach (var category in categories)
                {
                    LiveTreeNode node = new LiveTreeNode
                    {
                        name = category.Name.GetResource(CurrentSession.Culture),
                        id = category.Gid.ToString(),
                        isParent = category.ChildItems.Any()
                    };
                    nodes.Add(node);
                }
            }
            return nodes.ToJsonString();
        }

        #endregion 私有分类选择器

        #region 公有分类选择器

        /// <summary>
        /// 获取标准分类对应的名称
        /// </summary>
        /// <param name="cid">标准分类GUID</param>
        /// <returns></returns>
        [HttpPost]
        public string GetStandardCategoryName(Guid? cid)
        {
            GeneralStandardCategory category = (from c in dbEntity.GeneralStandardCategorys
                                                    .Include("Name").Include("Name.ResourceItems")
                                                where c.Gid == cid
                                                   && !c.Deleted
                                                select c).SingleOrDefault();
            if (category == null)
            {
                return string.Empty;
            }
            else
            {
                return category.Name.GetResource(CurrentSession.Culture);
            }
        }

        public string StandardCategorySelectLoad(byte categoryType = 0, Guid? id = null)
        {
            List<GeneralStandardCategory> categories = (from c in dbEntity.GeneralStandardCategorys.Include("ChildItems").Include("Name").Include("Name.ResourceItems")
                                                        where c.Ctype == categoryType
                                                           && id == null ? c.aParent == null : c.aParent == id
                                                           && c.Deleted == false
                                                        select c).ToList();
            List<LiveTreeNode> nodes = (from category in categories
                                        select new LiveTreeNode
                                        {
                                            name = (category.Name == null) ? "" : category.Name.GetResource(CurrentSession.Culture),
                                            id = category.Gid.ToString(),
                                            isParent = category.ChildItems.Any(c => !c.Deleted)
                                        }).ToList();
            return nodes.ToJsonString();
        }

        #endregion 公有分类选择器

        #region SKU选择器

        /// <summary>
        /// 上次搜索Sku的输入字符串
        /// </summary>
        private static string lastSkuInput;

        /// <summary>
        /// 上次搜索Sku的结果
        /// </summary>
        private static IEnumerable<ProductInfoItem> lastSkuResults;

        /// <summary>
        /// 上次搜索Sku的仓库Id
        /// </summary>
        private static Guid? lastSkuOrgID;

        /// <summary>
        /// 上次搜索的满足前提条件的所有Sku
        /// </summary>
        private static IEnumerable<ProductInfoItem> lastSkus;
        /// <summary>
        /// 获取指定输入字符串的SKU智能感知
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="orgID">组织ID</param>
        /// <returns>用于前端显示的Json数据</returns>
        [HttpPost]
        public JsonResult GetSKUIntelliSense(string input, Guid? orgID)
        {
            if (lastSkuOrgID != orgID)
            {
                lastSkuOrgID = orgID;
                lastSkus = GetSKUs(orgID);
            }
            IEnumerable<ProductInfoItem> skus = lastSkus;


            if (lastSkuInput == null || !input.Contains(lastSkuInput))
            {
                lastSkuInput = input;
                lastSkuResults = GetSkuResults(skus, input);
            }

            IEnumerable<ProductInfoItem> results = GetSkuResults(lastSkuResults, input);
            var data = (from result in results
                        select new
                        {
                            Gid = result.Gid,
                            Name = result.FullName.Matter,
                            Code = result.Code
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获得SKU的名称
        /// </summary>
        /// <param name="skuID">SKU的GUID</param>
        /// <returns></returns>
        [HttpPost]
        public string GetSKUName(Guid? skuID)
        {
            ProductInfoItem sku = (from p in dbEntity.ProductInfoItems.Include("FullName").Include("FullName.ResourceItems")
                                   where p.Gid == skuID
                                      && !p.Deleted
                                   select p).SingleOrDefault();
            if (sku == null)
            {
                return string.Empty;
            }
            else
            {
                return sku.FullName.GetResource(CurrentSession.Culture);
            }
        }

        /// <summary>
        /// 获取组织对应的SKU集合
        /// </summary>
        /// <param name="orgID">组织GUID</param>
        /// <returns>结果SKU集合</returns>
        private IEnumerable<ProductInfoItem> GetSKUs(Guid? orgID)
        {
            List<ProductInfoItem> skus;
            if (orgID == null)
            {
                skus = new List<ProductInfoItem>();
                return skus;
            }
            else
            {
                List<Guid> productIDs = (from p in dbEntity.ProductInformations
                                         where p.OrgID == orgID
                                            && !p.Deleted
                                         select p.Gid).ToList();
                skus = (from sku in dbEntity.ProductInfoItems
                            .Include("FullName").Include("FullName.ResourceItems")
                            .Include("ShortName").Include("ShortName.ResourceItems")
                        where productIDs.Contains(sku.ProdID)
                           && !sku.Deleted
                        select sku).ToList();
                return skus;
            }
        }

        /// <summary>
        /// 从给定Sku集合中获取包含input字符串的结果集
        /// </summary>
        /// <param name="skus">SKU集合</param>
        /// <param name="input">输入字符串</param>
        /// <returns></returns>
        private IEnumerable<ProductInfoItem> GetSkuResults(IEnumerable<ProductInfoItem> skus, string input)
        {
            List<ProductInfoItem> results = (from sku in skus
                                             where (sku.FullName.Matter.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1
                                                      || sku.FullName.ResourceItems.Any(item =>
                                                          item.Matter.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1))   //按FullName查找
                                                || (sku.ShortName.Matter.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1
                                                      || sku.ShortName.ResourceItems.Any(item =>
                                                          item.Matter.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1))   //按ShortName查找
                                                || sku.Code.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1                //按Code查找
                                             select sku).Take(20).ToList();
            return results;
        }

        #endregion SKU选择器

        #region 多语言资源

        /// <summary>
        /// 获得资源文件本地化名称
        /// </summary>
        /// <param name="resID">多语言资源</param>
        /// <returns></returns>
        [HttpPost]
        public string GetLocaleMatter(Guid resID)
        {
            GeneralResource res = (from r in dbEntity.GeneralResources
                                     .Include("ResourceItems")
                                   where r.Gid == resID
                                      && r.Rtype == (byte)ModelEnum.ResourceType.STRING
                                      && !r.Deleted
                                   select r).SingleOrDefault();
            return res.GetResource(CurrentSession.Culture);
        }

        #endregion 多语言资源

        #region 枚举名称

        public PartialViewResult EnumName(string enumName, string id = null, byte value = 0)
        {
            Type oEnumType = typeof(ModelEnum).GetNestedType(enumName);
            ViewBag.ID = id;
            ViewBag.Options = SelectEnumList(oEnumType, value);
            ViewBag.Value = value;
            return PartialView();
        }

        #endregion 枚举名称

        #region 用户名称

        /// <summary>
        /// 获取用户显示名
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public string GetUserName(Guid? userID)
        {
            if (userID.HasValue)
            {
                MemberUser user = dbEntity.MemberUsers.Find(userID);
                if (user == null || user.Deleted)
                    return string.Empty;
                return user.DisplayName;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion 用户名称

        #region 日期时间

        [HttpPost]
        public string GetTimeString(DateTimeOffset? time)
        {
            string str = string.Empty;
            if (time.HasValue)
            {
                CultureInfo culture = new CultureInfo(CurrentSession.Culture);
                str = time.Value.ToString(culture.DateTimeFormat.ShortDatePattern);
            }
            return str;
        }

        #endregion 日期时间

        #region RefID选择器

        /// <summary>
        /// 获取RefID对应的单据Code
        /// </summary>
        /// <param name="refType">类型</param>
        /// <param name="refID">对应单据Guid</param>
        /// <returns></returns>
        [HttpPost]
        public string GetRefCode(ModelEnum.NoteType? refType, Guid? refID)
        {
            string code = null;
            if (refType.HasValue && refID.HasValue)
            {
                switch (refType)
                {
                    case ModelEnum.NoteType.PURCHASE:
                        code = (from p in dbEntity.PurchaseInformations
                                where p.Gid == refID
                                   && !p.Deleted
                                select p.Code).SingleOrDefault();
                        break;
                    case ModelEnum.NoteType.ORDER:
                        code = (from o in dbEntity.OrderInformations
                                where o.Gid == refID
                                   && !o.Deleted
                                select o.Code).SingleOrDefault();
                        break;
                    case ModelEnum.NoteType.MOVE:
                        code = (from mov in dbEntity.WarehouseMovings
                                where mov.Gid == refID
                                   && !mov.Deleted
                                select mov.Code).SingleOrDefault();
                        break;
                    default: break;
                }
            }
            if (code == null)
                code = string.Empty;
            return code;
        }

        /// <summary>
        /// 获取RefID对应的GUID
        /// </summary>
        /// <param name="refType">RefType</param>
        /// <param name="code">输入的Code</param>
        /// <returns></returns>
        [HttpPost]
        public Guid GetNoteGid(ModelEnum.NoteType refType, string code = "")
        {
            if (code == string.Empty)
            {
                //To do
            }
            Guid? gid = null;
            switch (refType)
            {
                case ModelEnum.NoteType.PURCHASE:
                    gid = (from p in dbEntity.PurchaseInformations
                           where p.Code == code
                               //&& p.Pstatus ==
                               //需要添加额外逻辑
                               //
                              && !p.Deleted
                           select p.Gid).SingleOrDefault();
                    break;
                case ModelEnum.NoteType.ORDER:
                    gid = (from o in dbEntity.OrderInformations
                           where o.Code == code
                               //&& o.Ostatus ==
                               //需要添加额外逻辑
                               //
                              && !o.Deleted
                           select o.Gid).SingleOrDefault();
                    break;
                case ModelEnum.NoteType.MOVE:
                    gid = (from mov in dbEntity.WarehouseMovings
                           where mov.Code == code
                               //&& mov.Mstatus == 
                               //需要添加额外逻辑
                               //
                              && !mov.Deleted
                           select mov.Gid).SingleOrDefault();
                    break;
                default: break;
            }
            Guid data = gid.HasValue ? gid.Value : Guid.Empty;
            return data;
        }

        #endregion RefID选择器
    }
    public class ResourceController : BaseController
    {
        #region 测试方法

        /// <summary>
        /// 首页，呈现一些测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ViewResult TestEditMatter()
        {
            GeneralResource res = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.STRING,
                Culture = 2052,
                Matter = "我是中文名字"
            };
            GeneralResItem resitem1 = new GeneralResItem
            {
                Culture = 1036,
                Matter = "我是法国名字（PS:我不会法文，囧）",
                Resource = res
            };
            GeneralResItem resitem2 = new GeneralResItem
            {
                Culture = 1031,
                Matter = "It's an English name.",
                Resource = res
            };
            dbEntity.GeneralResources.Add(res);
            dbEntity.GeneralResItems.Add(resitem1);
            dbEntity.GeneralResItems.Add(resitem2);
            dbEntity.SaveChanges();

            return View("TestEditMatter", res);
        }

        /// <summary>
        /// 测试方法，最终将被舍弃
        /// </summary>
        /// <returns></returns>
        public ViewResult TestEditCash()
        {
            GeneralMeasureUnit unitCNY = dbEntity.GeneralMeasureUnits.Single(item => item.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                                                        && item.Code == "¥"
                                                        && item.Deleted == false);
            GeneralMeasureUnit unitUSD = dbEntity.GeneralMeasureUnits.Single(item => item.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY
                                                        && item.Code == "$"
                                                        && item.Deleted == false);

            //添加金额资源
            GeneralResource res = new GeneralResource
            {
                Rtype = (byte)ModelEnum.ResourceType.MONEY,
                Currency = unitCNY.Gid,
                Cash = 10.0m,
                Code = "¥"
            };
            GeneralResItem resItem = new GeneralResItem
            {
                Resource = res,
                Cash = 20.0m,
                Currency = unitUSD.Gid,
                Code = "$"
            };
            dbEntity.GeneralResources.Add(res);
            dbEntity.GeneralResItems.Add(resItem);
            dbEntity.SaveChanges();

            return View("TestEditCash", res);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestHTMLEditor()
        {
            return View();
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestRegionSelector()
        {
            List<MemberAddress> list = new List<MemberAddress>
            {
                new MemberAddress(),
                new MemberAddress()
            };
            return View(list);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestProgramSelector()
        {
            List<GeneralErrorReport> list = new List<GeneralErrorReport>
            {
                new GeneralErrorReport(),
                new GeneralErrorReport()
            };
            return View(list);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestStandardCategorySelector()
        {
            List<MemberOrganization> list = new List<MemberOrganization>
            {
                new MemberOrganization(),
                new MemberOrganization()
            };
            return View(list);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestPrivateCategorySelector()
        {
            List<GeneralPrivItem> list = new List<GeneralPrivItem>
            {
                new GeneralPrivItem(),
                new GeneralPrivItem()
            };
            return View(list);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestImageSelector()
        {
            List<string> list = new List<string>
            {
                "",
                ""
            };
            return View(list);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestEditLargeMatter()
        {
            GeneralLargeItem item = new GeneralLargeItem
            {
                Culture = 1033,
                CLOB = "<p style='color:Green'>English</p>",
            };
            GeneralLargeObject obj = new GeneralLargeObject
            {
                Culture = 2052,
                CLOB = "<p style='color:Red'>中文</p>",
                LargeItems = new List<GeneralLargeItem>
                {
                    item
                }
            };
            dbEntity.GeneralLargeObjects.Add(obj);
            dbEntity.SaveChanges();
            return View(obj);
        }

        /// <summary>
        /// 测试方法，最终废弃不用
        /// </summary>
        /// <returns></returns>
        public ViewResult TestSKUSelector()
        {
            List<Guid> list = new List<Guid>
            {
                Guid.NewGuid()
            };
            WarehouseInformation warehouse = dbEntity.WarehouseInformations.First(w => !w.Deleted);
            ViewBag.whID = warehouse.Gid;
            return View(list);
        }
        #endregion 测试方法
    }
}
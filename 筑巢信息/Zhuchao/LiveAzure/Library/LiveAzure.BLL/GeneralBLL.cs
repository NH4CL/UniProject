using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Transactions;
using System.IO;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models;
using LiveAzure.Models.Member;

namespace LiveAzure.BLL
{
    public class GeneralBLL : BaseBLL
    {
        /// <summary>
        /// 构造函数，必须传入数据库连接参数
        /// </summary>
        /// <param name="entity">数据库连接参数</param>
        public GeneralBLL(LiveEntities entity) : base(entity) { }

        /// <summary>
        /// 查找用户
        /// </summary>
        /// <returns>用户</returns>
        public MemberUser getUser(Guid? userID)
        {
            if (userID == null)
                return null;
            else
                return dbEntity.MemberUsers.Where(
                    u => u.Gid == userID && u.Deleted == false && u.Ustatus == (byte)ModelEnum.UserStatus.VALID)
                    .FirstOrDefault();
        }

        public MemberUser getUser(string loginName)
        {
            MemberUser user = dbEntity.MemberUsers.Where(
                u => u.LoginName == loginName && u.Deleted == false && u.Ustatus == (byte)ModelEnum.UserStatus.VALID)
                .FirstOrDefault();
            return user;
        }

        /// <summary>
        /// 判断用户是否是内部用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>是否是内部用户</returns>
        public bool IsInternal(MemberUser user)
        {
            MemberUser oThisUser = dbEntity.MemberUsers.Include("Role").Where(
                u => u.Gid == user.Gid && u.Deleted == false && u.Ustatus == (byte)ModelEnum.UserStatus.VALID)
                .FirstOrDefault();
            MemberRole oThisRole = user.Role;
            bool bResult = false;
            while (oThisRole != null)
            {
                if (oThisRole.Code == "Internal")
                {
                    bResult = true;
                    break;
                }
                else
                {
                    //oThisRole = oThisRole.Parent;
                    oThisRole = dbEntity.MemberRoles.Where(r => r.Gid == oThisRole.aParent).FirstOrDefault();
                }
            }
            return bResult;
        }

        /// <summary>
        /// 获取用户组织的支持语言集，如果没有组织，则获取系统支持的所有语言集
        /// </summary>
        /// <param name="organID">组织ID</param>
        /// <returns>语言集</returns>
        public List<GeneralCultureUnit> GetSupportCultures(Guid? organID = null)
        {
            List<GeneralCultureUnit> list = new List<GeneralCultureUnit>();
            if (organID.HasValue)
                list = (from o in dbEntity.MemberOrgCultures.Include("Culture")
                        where o.OrgID == organID && o.Ctype == (byte)ModelEnum.CultureType.LANGUAGE && o.Deleted == false
                        select o.Culture).ToList();
            else
                list = (from u in dbEntity.GeneralCultureUnits
                        where u.Deleted == false
                        select u).ToList();
            return list;
        }

        /// <summary>
        /// 查询支持的货币，如果有组织参数，则查询组织支持的货币，否则查询系统支持的所有货币
        /// </summary>
        /// <param name="organID">组织ID</param>
        /// <returns>货币列表</returns>
        public List<GeneralMeasureUnit> GetSupportCurrencies(Guid? organID = null)
        {
            List<GeneralMeasureUnit> list = new List<GeneralMeasureUnit>();
            if (organID.HasValue)
                list = (from o in dbEntity.MemberOrgCultures.Include("Currency")
                        where o.OrgID == organID && o.Ctype == (byte)ModelEnum.CultureType.CURRENCY && o.Deleted == false
                        select o.Currency).ToList();
            else
                list = (from u in dbEntity.GeneralMeasureUnits
                        where u.Utype == (byte)ModelEnum.MeasureUnit.CURRENCY && u.Deleted == false
                        select u).ToList();
            return list;
        }

        /// <summary>
        /// 获取组织的默认语言
        /// </summary>
        /// <param name="organID">组织ID</param>
        /// <returns></returns>
        public Guid GetDefaultCulture(Guid organID)
        {
            var fnDefault = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindDefaultCulture({0})", organID).FirstOrDefault();
            if (fnDefault != null)
                return Guid.Parse(fnDefault.ToString());
            else
                return Guid.Empty;
        }

        /// <summary>
        /// 获取组织的默认货币
        /// </summary>
        /// <param name="organID">组织ID</param>
        /// <returns></returns>
        public Guid GetDefaultCurrency(Guid organID)
        {
            var fnDefault = dbEntity.Database.SqlQuery<Guid>("SELECT dbo.fn_FindDefaultCurrency({0})", organID).FirstOrDefault();
            if (fnDefault != null)
                return Guid.Parse(fnDefault.ToString());
            else
                return Guid.Empty;
        }

        /// <summary>
        /// 获取程序节点路径
        /// </summary>
        /// <param name="programId">程序ID</param>
        /// <param name="culture">文化</param>
        /// <returns></returns>
        public string getPath(Guid programId, int culture)
        {
            ArrayList programCodeList = new ArrayList();
            GeneralProgram childNode = dbEntity.GeneralPrograms.Find(programId);
            programCodeList.Add(childNode.Name.GetResource(culture));
            while (childNode.Parent != null)
            {
                programCodeList.Insert(0, childNode.Parent.Name.GetResource(culture));
                childNode = childNode.Parent;
            }
            string result = "root";
            foreach (var item in programCodeList)
                result += "/" + item;
            return result;
        }

        #region 资源文件预处理

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
                List<GeneralMeasureUnit> oUnits = this.GetSupportCurrencies(organID);
                bool bIsFirst = true;
                foreach (var item in oUnits)
                {
                    if (bIsFirst)
                    {
                        oResource.Code = item.Code;
                        oResource.Currency = item.Gid;
                    }
                    else
                    {
                        oResource.ResourceItems.Add(new GeneralResItem { Code = item.Code, Currency = item.Gid });
                    }
                    bIsFirst = false;
                }
            }
            else
            {
                List<GeneralCultureUnit> oCultures = this.GetSupportCultures(organID);
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
                List<GeneralMeasureUnit> oUnits = this.GetSupportCurrencies(organID);
                bool bIsFirst = true;
                foreach (var item in oUnits)
                {
                    if (bIsFirst)
                    {
                        oResource.Code = item.Code;
                        oResource.Currency = item.Gid;
                    }
                    else
                    {
                        var resitem = oResource.ResourceItems.FirstOrDefault(i => i.Currency == item.Gid && i.Deleted == false);
                        if (resitem == null)
                            oResource.ResourceItems.Add(new GeneralResItem { Code = item.Code, Currency = item.Gid });
                        else
                            oGuidList.Add(resitem.Gid);
                    }
                    bIsFirst = false;
                }
            }
            else
            {
                List<GeneralCultureUnit> oCultures = this.GetSupportCultures(organID);
                bool bIsFirst = true;
                foreach (var item in oCultures)
                {
                    if (bIsFirst)
                    {
                        oResource.Culture = item.Culture;
                    }
                    else
                    {
                        var resitem = oResource.ResourceItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
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

        /// <summary>
        /// 产生多语言的大对象资源文件
        /// </summary>
        /// <param name="organID">组织，空则表示用系统支持的所有语言</param>
        /// <returns>新资源文件</returns>
        public GeneralLargeObject NewLargeObject(Guid? organID = null)
        {
            GeneralLargeObject oLarge = new GeneralLargeObject();
            List<GeneralCultureUnit> oCultures = this.GetSupportCultures(organID);
            bool bIsFirst = true;
            foreach (var item in oCultures)
            {
                if (bIsFirst)
                    oLarge.Culture = item.Culture;
                else
                    oLarge.LargeItems.Add(new GeneralLargeItem { Culture = item.Culture });
                bIsFirst = false;
            }
            return oLarge;
        }

        /// <summary>
        /// 更新已经存在的大对象资源文件，包括插入新语言，删除过期的语言等
        /// </summary>
        /// <param name="large">原资源文件</param>
        /// <param name="organID">组织ID，空表示用系统支持的语言/货币刷新</param>
        /// <returns>新资源文件</returns>
        public GeneralLargeObject RefreshLargeObject(GeneralLargeObject large, Guid? organID = null)
        {
            GeneralLargeObject oLarge = large;
            if (oLarge == null)
                oLarge = this.NewLargeObject(organID);
            List<Guid> oGuidList = new List<Guid>();
            List<GeneralCultureUnit> oCultures = this.GetSupportCultures(organID);
            bool bIsFirst = true;
            foreach (var item in oCultures)
            {
                if (bIsFirst)
                {
                    oLarge.Culture = item.Culture;
                }
                else
                {
                    var resitem = oLarge.LargeItems.FirstOrDefault(i => i.Culture == item.Culture && i.Deleted == false);
                    if (resitem == null)
                        oLarge.LargeItems.Add(new GeneralLargeItem { Culture = item.Culture });
                    else
                        oGuidList.Add(resitem.Gid);
                }
                bIsFirst = false;
            }
            // 删除过时的语言资源
            for (int i = 0; i < oLarge.LargeItems.Count; i++)
            {
                var item = oLarge.LargeItems.ElementAt(i);
                if (!item.Gid.Equals(Guid.Empty) && !oGuidList.Contains(item.Gid))
                    oLarge.LargeItems.Remove(item);
            }
            return oLarge;
        }

        #endregion

    }
}

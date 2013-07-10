using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LiveAzure.Models.General;

namespace LiveAzure.Controls.LiveRegionSelector
{
    public static class GeneralRegionExtension
    {
        /// <summary>
        /// 递归获取地区完整地址
        /// </summary>
        /// <param name="region">地区</param>
        /// <returns></returns>
        public static string GetRegionAddress( this GeneralRegion region)
        {
            if (region == null)
                return string.Empty;
            else
            {
                string parentAddress = GetRegionAddress(region.Parent);
                if (parentAddress == string.Empty)
                    return region.ShortName;
                else
                    return string.Concat(parentAddress, " - ", region.FullName);
            }
        }
    }
}

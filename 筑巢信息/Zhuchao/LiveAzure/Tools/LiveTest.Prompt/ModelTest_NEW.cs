using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveAzure.Models.General;
using LiveAzure.Utility;
using LiveAzure.Models;

namespace LiveAzure.Tools.Tester
{
    public partial class ModelTest_NEW
    {
        private LiveEntities oLiveEntities = new LiveEntities(ConfigHelper.LiveConnection.Connection);
        /// <!--刘鑫-->
        /// <summary>
        /// 用于生成随机code的随机种子
        /// </summary>
        private Random random = new Random();
        /// 刘鑫
        /// <summary>
        /// 生成随机CODE
        /// </summary>
        /// <returns></returns>
        public string GetRandCode()
        {
            string s = DateTimeOffset.Now.Second.ToString();
            s += random.Next(9999);
            s += random.Next(9999);
            return s;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Utility
{
    public static class CultureHelper
    {
        // Include ONLY cultures you are implementing as views
        private static Dictionary<string, bool> _cultures
        {
            get
            {
                Dictionary<string, bool> oResult = new Dictionary<string, bool>();
                string sConfigs = Utility.ConfigHelper.GlobalConst.GetSetting("SupportViews");
                var sCultures = sConfigs.Split(',');
                foreach (var item in sCultures)
                {
                    try
                    {
                        string[] keys = item.Split(':');
                        oResult.Add(keys[0] , Boolean.Parse(keys[1]));
                    }
                    catch { }
                }
                return oResult;
            }
        }
        
        //    = new Dictionary<string, bool>
        //{
        //    {"en-US", true},    // first culture is the DEFAULT
        //    {"zh-CN", true},
        //    {"en-GB", true},
        //    {"fr-FR", true},
        //    {"de-DE", true}
        //};

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </summary>
        /// <param name="name">Culture's name (e.g. en-US)</param>
        public static string GetValidCulture(string name)
        {
            if (String.IsNullOrEmpty(name))
                return GetDefaultCulture(); // return Default culture
            if (_cultures.ContainsKey(name))
                return name;
            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (ie English)            
            foreach (var c in _cultures.Keys)
                if (c.StartsWith(name.Substring(0, 2)))
                    return c;
            // else             
            return GetDefaultCulture(); // return Default culture as no match found
        }

        /// <summary>
        /// Returns default culture name which is the first name decalared (e.g. en-US)
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultCulture()
        {
            return _cultures.Keys.ElementAt(0); // return Default culture
        }

        /// <summary>
        ///  Returns "true" if view is implemented separatley, and "false" if not.
        ///  For example, if "es-CL" is true, then separate views must exist e.g. Index.es-cl.cshtml, About.es-cl.cshtml
        /// </summary>
        /// <param name="name">Culture's name</param>
        /// <returns></returns>
        public static bool IsViewSeparate(string name)
        {
            return _cultures[name];
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TreeTools_lumin.Models
{
    public class TreeNode
    {
        public string id { get; set; }
        public string name { get; set; }
        public string progUrl { get; set; }
        public string icon { get; set; }
        public string iconClose { get; set; }
        public string iconOpen { get; set; }
        public bool isParent { get; set; }
        public string checkedCol { get; set; }
        public bool nodeChecked { get; set; }
        public string target { get; set; }
        public List<TreeNode> nodes { get; set; }

        public TreeNode()
        {
            this.checkedCol = "nodeChecked";
        }

    }
}
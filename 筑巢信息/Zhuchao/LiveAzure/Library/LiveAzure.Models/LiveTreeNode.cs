using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Models
{
    /// <summary>
    /// 生成树的节点类
    /// </summary>
    public class LiveTreeNode
    {
        /// <summary>
        /// 树节点id为guid
        /// </summary>
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _id = Guid.Empty.ToString();
        /// <summary>
        /// 树节点name为树节点显示名称
        /// </summary>
        public string name
        {
            get{return _name;}
            set { _name = value; }
        }
        private string _name = string.Empty;
        
        /// <summary>
        /// 树节点progUrl为树节点链接地址，页面点击跳转时必须在树的onclick函数中自定义方法
        /// </summary>
        public string progUrl {
            get { return _progUrl; }
            set { _progUrl = value; }
        }
        private string _progUrl = string.Empty;
        /// <summary>
        /// 树节点icon为树节点显示图标
        /// </summary>
        public string icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        private string _icon = string.Empty;

        /// <summary>
        /// 树节点iconClose为树节点收起时的图标
        /// </summary>
        public string iconClose
        {
            get { return _iconClose; }
            set { _iconClose = value; }
        }
        private string _iconClose = string.Empty;

        /// <summary>
        /// 树节点iconOpen为树节点展开时的图标
        /// </summary>
        public string iconOpen
        {
            get { return _iconOpen; }
            set { _iconOpen = value; }
        }
        private string _iconOpen = string.Empty;
        /// <summary>
        /// 树节点是否为父节点
        /// </summary>
        public bool isParent
        {
            get { return _isParent; }
            set { _isParent = value; }
        }
        private bool _isParent = false;

        /// <summary>
        /// 标记树节点checkbox是否显示的字段
        /// </summary>
        public string checkedCol
        {
            get { return _checkedCol; }
            set { _checkedCol = value; }
        }
        private string _checkedCol = "nodeChecked";

        /// <summary>
        /// 树节点checkbox是否选中，true为选中
        /// </summary>
        public bool nodeChecked
        {
            get { return _nodeChecked; }
            set { _nodeChecked = value; }
        }
        private bool _nodeChecked = false;
        /// <summary>
        /// 树节点是否已展开
        /// </summary>
        public bool open
        {
            get { return _open; }
            set { _open = value; }
        }
        private bool _open = false;
        /// <summary>
        /// 树节点的子节点的集合
        /// </summary>
        public List<LiveTreeNode> nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }
        private List<LiveTreeNode> _nodes = new List<LiveTreeNode>();

        public string JsonString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('{');
                builder.AppendFormat("\"id\":\"{0}\"", id);
                builder.Append(',');
                builder.AppendFormat("\"name\":\"{0}\"", name);
                if (progUrl != null && progUrl != string.Empty)
                {
                    builder.Append(',');
                    builder.AppendFormat("\"progUrl\":\"{0}\"", progUrl);
                }
                if (icon != null && icon != string.Empty)
                {
                    builder.Append(',');
                    builder.AppendFormat("\"icon\":\"{0}\"", icon);
                }
                if (iconClose != null && iconClose != string.Empty)
                {
                    builder.Append(',');
                    builder.AppendFormat("\"iconClose\":\"{0}\"", iconClose);
                }
                if (iconOpen != null && iconClose != string.Empty)
                {
                    builder.Append(',');
                    builder.AppendFormat("\"iconOpen\":\"{0}\"", iconOpen);
                }
                builder.Append(',');
                builder.AppendFormat("\"isParent\":{0}", isParent.ToString().ToLower());
                builder.Append(',');
                builder.AppendFormat("\"checkedCol\":\"{0}\"", checkedCol);
                if (nodeChecked)
                {
                    builder.Append(',');
                    builder.Append("\"nodeChecked\":true");
                }
                if (open)
                {
                    builder.Append(',');
                    builder.Append("\"open\":true");
                }
                if (isParent && nodes!= null && nodes.Any())
                {
                    builder.Append(',');
                    builder.Append("\"nodes\":");
                    builder.Append('[');
                    builder.Append(string.Join(",", nodes.Select(node => node.ToString())));
                    builder.Append(']');
                }
                builder.Append('}');
                return builder.ToString();
            }
        }

        /// <summary>
        /// 输出Json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonString;
        }

        /// <summary>
        /// 输出Json字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJsonString();
        }
    }
    public static class LiveTreeNodeExtensions
    {
        /// <summary>
        /// 生成树节点集合的Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this IEnumerable<T> nodes) where T : LiveTreeNode
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(string.Join(",", nodes.Select(node => node.ToJsonString())));
            sb.Append(']');
            return sb.ToString();
        }
    }
}

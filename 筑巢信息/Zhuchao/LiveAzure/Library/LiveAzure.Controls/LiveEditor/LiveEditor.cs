using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveAzure.Controls.LiveEditor
{
    /// <summary>
    /// 皮肤枚举
    /// </summary>
    public enum LiveEditorSkin
    {
        Kama,
        Office2003,
        V2
    }
    /// <summary>
    /// 工具栏类型
    /// </summary>
    public enum LiveEditorToolBar
    {
        Full,
        Basic,
        LiveEditor
    }
    /// <summary>
    /// HTML编辑器
    /// </summary>
    public class LiveEditor
    {
        #region 属性

        /// <summary>
        /// 获取和设置绑定TextArea名称
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        private string _Name = DefaultName;
        /// <summary>
        /// 默认绑定TextArea名称
        /// </summary>
        public const string DefaultName = "LiveEditor";
        /// <summary>
        /// 获取和设置皮肤名称
        /// </summary>
        public string SkinName
        {
            get
            {
                return _SkinName;
            }
            set
            {
                _SkinName = value;
            }
        }
        private string _SkinName = DefaultSkinName;
        /// <summary>
        /// 默认皮肤名称
        /// </summary>
        public const string DefaultSkinName = "kama";
        /// <summary>
        /// 设置皮肤
        /// </summary>
        public LiveEditorSkin Skin
        {
            set
            {
                SkinName = value.ToString().ToLower();
            }
        }
        /// <summary>
        /// 获取和设置宽度
        /// </summary>
        public string Width
        {
            get;
            set;
        }
        /// <summary>
        /// 获取和设置高度
        /// </summary>
        public string Height
        {
            get;
            set;
        }
        /// <summary>
        /// 获取和设置默认语言
        /// </summary>
        public string Language
        {
            get;
            set;
        }
        /// <summary>
        /// 或缺和设置工具条参数
        /// </summary>
        public string ToolBarParam
        {
            get { return _ToolBarParam; }
            set { _ToolBarParam = value; }
        }
        private string _ToolBarParam = "LiveEditor";
        /// <summary>
        /// 设置工具条
        /// </summary>
        public LiveEditorToolBar ToolBar
        {
            set
            {
                ToolBarParam = value.ToString();
            }
        }

        #endregion 属性

        #region 操作

        /// <summary>
        /// 设置名称，可选，默认为LiveEditor
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public LiveEditor SetName(string name)
        {
            Name = name;
            return this;
        }
        /// <summary>
        /// 设置皮肤，可选，默认为kama
        /// </summary>
        /// <param name="skin">皮肤名称</param>
        /// <returns></returns>
        public LiveEditor SetSkin(string skin)
        {
            SkinName = skin;
            return this;
        }
        /// <summary>
        /// 设置皮肤，可选，默认为XEditorSkin.Kama
        /// </summary>
        /// <param name="skin">皮肤</param>
        /// <returns></returns>
        public LiveEditor SetSkin(LiveEditorSkin skin)
        {
            Skin = skin;
            return this;
        }
        /// <summary>
        /// 设置宽度，可选，默认自动填充
        /// </summary>
        /// <param name="width">宽度值</param>
        /// <returns></returns>
        public LiveEditor SetWidth(string width)
        {
            Width = width;
            return this;
        }
        /// <summary>
        /// 设置高度，可选，默认自动填充
        /// </summary>
        /// <param name="height">高度值</param>
        /// <returns></returns>
        public LiveEditor SetHeight(string height)
        {
            Height = height;
            return this;
        }
        /// <summary>
        /// 设置默认语言，可选，默认为当前语言
        /// </summary>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        public LiveEditor SetLanguage(string lang)
        {
            Language = lang;
            return this;
        }
        /// <summary>
        /// 设置工具栏，可选
        /// </summary>
        /// <param name="param">工具栏配置名称或具体配置字符串</param>
        /// <returns></returns>
        public LiveEditor SetToolBar(string param)
        {
            ToolBarParam = param;
            return this;
        }
        /// <summary>
        /// 设置预设工具栏样式
        /// </summary>
        /// <param name="toolbar">工具栏样式</param>
        /// <returns></returns>
        public LiveEditor SetToolBar(LiveEditorToolBar toolbar)
        {
            ToolBar = toolbar;
            return this;
        }
        #endregion 操作

        #region 生成代码

        /// <summary>
        /// 获取CKEditor最终生成HTML代码
        /// </summary>
        public string FinalHtml
        {
            get
            {
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<script type='text/javascript'>");
                htmlBuilder.Append("CKEDITOR.replace");
                    htmlBuilder.Append('(');
                    htmlBuilder.AppendFormat("'{0}'", Name);
                    htmlBuilder.Append(',');
                        htmlBuilder.Append('{');
                        htmlBuilder.Append(AttributeHtml);
                        htmlBuilder.Append('}');
                    htmlBuilder.Append(')');
                htmlBuilder.Append(';');
                htmlBuilder.Append("</script>");
                return htmlBuilder.ToString();
            }
        }
        /// <summary>
        /// 获取CKEditor属性文本
        /// </summary>
        public string AttributeHtml
        {
            get
            {
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.AppendFormat("skin:'{0}'", SkinName);
                htmlBuilder.Append(',');
                if (Width != null)
                {
                    htmlBuilder.AppendFormat("width:'{0}'", Width);
                    htmlBuilder.Append(',');
                }
                if (Height != null)
                {
                    htmlBuilder.AppendFormat("height:'{0}'", Height);
                    htmlBuilder.Append(',');
                }
                if (Language != null)
                {
                    htmlBuilder.AppendFormat("language:'{0}'", Language);
                    htmlBuilder.Append(',');
                }
                if (ToolBarParam != null)
                {
                    if (ToolBarParam.StartsWith("["))
                        htmlBuilder.AppendFormat("toolbar:{0}", ToolBarParam);
                    else
                        htmlBuilder.AppendFormat("toolbar:'{0}'", ToolBarParam);
                    htmlBuilder.Append(',');
                }
                return htmlBuilder.ToString().TrimEnd(new char[] { ',' });
            }
        }
        #endregion 生成代码
    }
}

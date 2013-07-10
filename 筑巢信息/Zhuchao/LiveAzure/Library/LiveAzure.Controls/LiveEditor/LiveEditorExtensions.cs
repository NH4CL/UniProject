using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveAzure.Controls.LiveEditor
{
    public static class LiveEditorExtensions
    {
        /// <summary>
        /// 将一个TextArea扩展为LiveEditor
        /// </summary>
        /// <param name="html"></param>
        /// <param name="editor">LiveEditor实例</param>
        /// <returns></returns>
        public static MvcHtmlString LiveEditor(this HtmlHelper html, LiveEditor editor)
        {
            return LiveEditor(editor);
        }
        public static MvcHtmlString LiveEditor(LiveEditor editor)
        {
            return new MvcHtmlString(editor.FinalHtml);
        }
    }
}
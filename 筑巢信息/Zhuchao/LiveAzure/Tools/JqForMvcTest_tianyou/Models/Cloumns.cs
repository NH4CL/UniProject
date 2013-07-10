using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC.Controls.Grid;
using LiveAzure.Models.General;

namespace JqForMvcTest_tianyou.Models
{
    public static class Columns
    {
        private static GridColumnModelList<GeneralProgram> _programsColumns = CreatProColumns();
        public static GridColumnModelList<GeneralProgram> ProColumns { get { return _programsColumns; } }
        private static GridColumnModelList<GeneralProgram> CreatProColumns()
        {
            GridColumnModelList<GeneralProgram> cn = new GridColumnModelList<GeneralProgram>();
            cn.Add(p => p.Gid).SetAsPrimaryKey().SetEditable(false);
            cn.Add(p => p.Name.Matter).AddEvent("change", "doCascade").SetCaption("中文名");
            cn.Add(p => p.ProgUrl).AddEvent("change", "doCascade");
            return cn;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Reflection;

namespace MVC.Controls.Grid
{
    public class GridControl
    {
        private string _listUrl, _editUrl, _caption;
        private string _listParams = null;
        private string _attributes = null;
        private string _onSelectedRow = null;
        private string _height = null, _width = null;
        private string _onGridComplete = null;
        private string _onLoadComplete = null;
        private bool _altRows;
        private bool _multiSelect;

        private string _groupingFormat = null;

        private int _pageSize;
        private bool _isAutoSize, _isRowNumber, _isRTL = false;
        private GridControl _subGrid = null;
        private object _gridDataSource = null;
        private bool _renderEntireObject = true;
        private HttpVerbs _httpVerb = HttpVerbs.Get;

        /// <summary>
        /// If grouping has been set, configures how the grouping is formatted
        /// </summary>
        /// <param name="groupColumnShow">Whether or not to show the grouped column. Default to yes</param>
        /// <param name="groupText">The text title of the group. e.g. {0}-{1} items</param>
        /// <param name="groupCollapse">Whether or not the groups are collapsed by default. Default to no</param>
        /// <param name="groupOrder">The group order. Default to asc</param>
        /// <returns></returns>
        public GridControl SetGroupFormatting(bool groupColumnShow = true, string groupText = "", bool groupCollapse = false, string groupOrder = "asc")
        {
            _groupingFormat = "groupColumnShow:[" + groupColumnShow.ToString().ToLower() + "]" +
                               ", groupText: [\"" + groupText + "\"], groupCollapse: " + groupCollapse.ToString().ToLower() +
                               ", groupOrder: ['" + groupOrder + "']";

            //_groupColumnShow = groupColumnShow;
            //_groupCollpse = groupCollapse;
            //_groupText = groupText;
            //_groupOrder = groupOrder;
            return this;
        }

        private List<GridColumnModel> _columns = new List<GridColumnModel>();

        public GridControl()
        {
            this.isSubGrid = false;
            this.Pager = new GridPagerControl() { GridName = this.Name };
        }

        private bool isSubGrid { get; set; }

        public string Name { get; set; }
        public IGridPagerControl Pager { get; set; }

        /// <summary>
        /// Allows you to supply a function that determines which method is the primary key of the underlying object.
        /// This is used when determining if a columns should be used as a primary key. If you use another attribute
        /// than KeyAttribute, overriding this method will help you.
        /// </summary>
        public static Func<MemberInfo, bool> IsPrimaryKeyFunc { get; set; }

        public GridControl SetWidth(string val) { _width = val; return this; }
        public GridControl SetHeight(string val) { _height = val; return this; }

        /// <summary>
        /// Instead of setting a ListUrl with a controller url that will fetch the grid's data
        /// It is possible to give the grid it's data source statically thus reducing the amount of requests to the server
        /// </summary>
        /// <param name="gridData"></param>
        /// <param name="renderEntireObject">[Not yet fully supported] Whether or not to render the entire object, or only properties bound to a column</param>
        /// <returns></returns>
        public GridControl SetDataSource(object gridData, bool renderEntireObject = true)
        {
            _gridDataSource = gridData;
            _renderEntireObject = renderEntireObject;
            return this;
        }

        /// <summary>
        /// Sets a javascript function name
        /// That will raise when a row is selected
        /// </summary>
        public GridControl SetOnSelectedRowEvent(string val) { _onSelectedRow = val; return this; }

        public GridControl SetOnGridCompleteEvent(string val) { _onGridComplete = val; return this; }
        public GridControl SetOnLoadCompleteEvent(string val) { _onLoadComplete = val; return this; }

        /// <summary>
        /// Sets the title of the grid
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        public GridControl SetCaption(string caption) { _caption = caption; return this; }

        public GridControl SetHttpVerb(HttpVerbs verb) { _httpVerb = verb; return this; }

        /// <summary>
        /// The name of the div that will contain the grid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GridControl SetName(string name) { this.Name = name; return this; }

        /// <summary>
        /// The name of the div that will contain the pager
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public GridControl SetPager(IGridPagerControl pager) { this.Pager = pager; return this; }

        /// <summary>
        /// Sets the property Id to use to fetch the sub-grid's data
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public GridControl SetListQueryParams(string url) { _listParams = url; return this; }

        /// <summary>
        /// The url to the command that will return the list data of the grid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public GridControl SetListUrl(string url) { _listUrl = url; return this; }

        /// <summary>
        /// The url to the command that will update the edited row
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public GridControl SetEditUrl(string url) { _editUrl = url; return this; }

        /// <summary>
        /// The page size
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public GridControl SetPageSize(int pageSize) { _pageSize = pageSize; return this; }

        /// <summary>
        /// Set wether the grid columns will autosize themself
        /// </summary>
        /// <param name="autoSize"></param>
        /// <returns></returns>
        public GridControl SetIsAutoSize(bool autoSize) { _isAutoSize = autoSize; return this; }

        /// <summary>
        /// Set wether to show row numbers
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public GridControl SetIsRowNumber(bool rowNumber) { _isRowNumber = rowNumber; return this; }

        public GridControl SetAltRows(bool altRows) { _altRows = altRows; return this; }

        public GridControl SetMultiSelect(bool multiSelect) { _multiSelect = multiSelect; return this; }

        /// <summary>
        /// Add a column mapping
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public GridControl AddColumn(GridColumnModel column) { _columns.Add(column); return this; }

        public GridControl UpdateDefaultPager(Action<GridPagerControl> action)
        {
            if (Pager is GridPagerControl == false)
            {
                Pager = new GridPagerControl();
            }
            action((GridPagerControl)Pager);
            return this;
        }

        //public GridControl SetColumns<T>(Action<List<GridColumnModel<T>>> columns) { return this; }

        public GridControl SetColumns<T>(Action<GridColumnModelList<T>> initCols) where T : class
        {
            GridColumnModelList<T> cols = new GridColumnModelList<T>();
            initCols(cols);
            _columns.AddRange(cols.Items);
            return this;
        }

        public GridControl UseColumns<T>(GridColumnModelList<T> columns) where T : class
        {
            UseColumns(columns.Items);
            return this;
        }

        public GridControl UseColumns(IEnumerable<GridColumnModel> items)
        {
            _columns.AddRange(items);
            return this;
        }

        /// <summary>
        /// Creates a sub grid
        /// </summary>
        /// <param name="subGrid"></param>
        /// <returns></returns>
        public GridControl CreateSubGrid(GridControl subGrid) { _subGrid = subGrid; return this; }


        /// <summary>
        /// Renderes the grid as RTL
        /// </summary>
        /// <returns></returns>
        public GridControl IsRTL() { _isRTL = true; return this; }

        /// <summary>
        /// Add additional custom parameters to the Grid
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public GridControl SetAdditionalAttributes(string attributes) { _attributes = attributes; return this; }


        private string _scriptTemplate = "<script language=\"javascript\" type=\"text/javascript\">{0}</script>";

        public string RequiredData()
        {
            string dataBuilder = "";

            if (_gridDataSource != null)
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                dataBuilder = this.Name + "_dataSource = " + ser.Serialize(_gridDataSource) + ";";
            }

            if (_subGrid != null)
                dataBuilder += _subGrid.RequiredData();

            if ((this.isSubGrid) || (string.IsNullOrEmpty(dataBuilder)))
                return dataBuilder;
            else
                return string.Format(_scriptTemplate, dataBuilder);
        }

        public GridControl UseController<TControllerType>(object parent = null)
           where TControllerType : IGridController, new()
        {
            IGridController controller = Activator.CreateInstance<TControllerType>();

            if (parent == null)
            {
                SetListUrl(controller.GetListUrl());
                SetEditUrl(controller.GetEditUrl());
                UseColumns(controller.GetRawColumns());
            }
            else
            {
                SetListUrl(controller.GetListUrl(parent));
                SetEditUrl(controller.GetEditUrl(parent));
                UseColumns(controller.GetRawColumns());
            }

            return this;
        }

        public string Render()
        {
            if (this.Pager != null) this.Pager.GridName = this.Name;

            StringBuilder sb = new StringBuilder();

            if (!this.isSubGrid)
            {
                sb.AppendLine("$(\"#" + this.Name + "\").jqGrid({");
                sb.AppendFormat("url: \"{0}\",\r\n", _listUrl);
            }
            else
            {
                sb.AppendLine("jQuery(\"#\" + subgrid_table_id).jqGrid({");
                sb.AppendFormat("url: \"{0},\r\n", _listUrl);
            }


            if (!string.IsNullOrEmpty(_editUrl)) sb.AppendFormat("editurl: \"{0}\",\r\n", _editUrl);

            if (_gridDataSource != null)
            {
                sb.AppendFormat("data: " + this.Name + "_dataSource,\r\n");
                sb.AppendFormat("datatype: \"local\",\r\n");
            }
            else
            {
                sb.AppendFormat("mtype: \"" + _httpVerb.ToString().ToLower() + "\",\r\n");
                sb.AppendFormat("datatype: \"json\",\r\n");
            }

            sb.AppendFormat("colNames: [{0}],\r\n", renderColumnNames());
            sb.AppendFormat("colModel: [{0}],\r\n", renderColumnsModel());
            sb.AppendFormat("rowNum: {0},\r\n", _pageSize);
            //========================2011-10-12 陆旻添加 用于复选框============================
            if (_multiSelect == true)
            {
                sb.AppendFormat("multiselect: true,\r\n", _multiSelect.ToString().ToLower());
            }
            // Makes sure that the Delete method redurns the correct name for the id-field
            sb.AppendFormat("prmNames: {{id: \"{0}\"}},\rn", getKeyColumnName());

            if (!this.isSubGrid)
            {
                string pagerInit = "";
                if (this.Pager != null) pagerInit = this.Pager.OnGridLoad();

                string completeEvent = _onGridComplete != null ? _onGridComplete + "();" : "";
                sb.AppendLine("gridComplete: function f(){" + pagerInit + completeEvent + "updateButtonState($('#" + this.Name + "'));},");
            }

            if (!string.IsNullOrEmpty(_onLoadComplete))
            {
                sb.AppendLine("loadComplete: function(data){" + _onLoadComplete + "(data);},");
            }

            if (_isRTL)
                sb.AppendLine("direction: \"rtl\",");

            if (!string.IsNullOrEmpty(_height))
                sb.AppendFormat("height: {0},\r\n", _height);

            if (!string.IsNullOrEmpty(_width))
            {
                sb.AppendFormat("width: {0},\r\n", _width);
            }

            sb.AppendLine("rowList: -1,");

            if (!this.isSubGrid)
                sb.Append("pager: $(\"#" + this.Name + "Pager\"),");

            sb.AppendFormat("sortname: \"{0}\",\r\n", _columns[0].Name);

            sb.AppendFormat("autowidth: {0},\r\n", _isAutoSize.ToString().ToLower());

            if (_altRows)
            {
                sb.AppendFormat("altRows: true,\r\n", _isAutoSize.ToString().ToLower());
            }

            if (!string.IsNullOrEmpty(_onSelectedRow))
            {
                sb.AppendLine(
                    "onSelectRow: function(id){" +
                    _onSelectedRow + "($(\"#" + this.Name + "\").getRowData(id));" +
                    "updateButtonState($('#" + this.Name + "'));},");
            }
            else
            {
                sb.Append("onSelectRow: function(id){");
                sb.AppendLine("updateButtonState($('#" + this.Name + "'));");
                sb.Append("if ($(\"#Id\").length == 0) {return;}");
                sb.Append("$(\"#Id\")[0].value = " + "$(\"#" + this.Name + "\").getRowData(id)." + getKeyColumnName() + ";},");
            }

            foreach (GridColumnModel col in _columns)
            {
                if (col.AsGroup)
                {
                    sb.Append("grouping:true, groupingView:{groupField:['" + col.Name + "']");

                    if (!string.IsNullOrEmpty(_groupingFormat))
                        sb.Append(", " + _groupingFormat);

                    sb.Append("},");

                    break;
                }
            }

            sb.AppendFormat("caption: \"{0}\"", _caption);

            if (!string.IsNullOrEmpty(_attributes)) sb.AppendLine(_attributes);

            if (_subGrid != null)
            {
                sb.AppendLine(",");
                sb.AppendLine("subGrid: true,");
                sb.AppendLine("subGridRowExpanded: function(subgrid_id, row_id) {");
                sb.AppendLine("var subgrid_table_id;");
                sb.AppendLine("subgrid_table_id = subgrid_id+\"_t\";");
                sb.AppendLine("$(\"#\"+subgrid_id).html(\"<table id='\"+subgrid_table_id+\"' class='scroll'></table>\");");

                _subGrid.isSubGrid = true;
                _subGrid._listUrl += "\" + $(\"#" + this.Name + "\").getRowData(row_id)." + getKeyColumnName();
                _subGrid._height = "\"100%\"";
                sb.AppendFormat("{0}{1}\r\n", _subGrid.Render(), "}");
            }

            sb.Append("})");

            return sb.ToString();
        }

        private string getKeyColumnName()
        {
            // First, use any specific primary key
            foreach (GridColumnModel col in _columns)
                if (col.IsPrimaryKey) return col.Name;

            // Secondly, use any implicit primary key (set through DataAnnotations)
            foreach (GridColumnModel col in _columns)
                if (col.IsImplicitPrimaryKey) return col.Name;

            throw new Exception("Grid Renderer Failed: Please choose a column as a primary key");
        }

        private string renderColumnNames()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append("\"");
                sb.Append(_columns[i].GetColumnCaption());
                sb.Append("\"");
                if (i < _columns.Count - 1) sb.Append(",");
            }
            return sb.ToString();
        }

        private string renderColumnsModel()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_columns[i].Render());
                if (i < _columns.Count - 1) sb.AppendLine(", ");
            }

            return sb.ToString();
        }
    }
}

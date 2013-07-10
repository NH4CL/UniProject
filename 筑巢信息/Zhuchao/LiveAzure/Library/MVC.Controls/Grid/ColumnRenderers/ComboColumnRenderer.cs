using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVC.Controls.Grid
{
    /// <summary>
    /// Renders the column as a combobox (HTML select input)
    /// </summary>
    public class ComboColumnRenderer : IColumnRenderer
    {
        private string _url;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Define where the list should be filled from.
        /// Possible values are: 1. action url (i.e: Users/CityList) 2. jscript function (i.e: getCities()) </param>
        public ComboColumnRenderer(string source) { _url = source; }

        public GridColumnModel Column { get; set; }

        public string Render()
        {
            string dataEvents = this.Column.renderDataEvents();
            if (!string.IsNullOrEmpty(dataEvents)) dataEvents = ", " + dataEvents;
            return "formatter:'select', edittype: 'select', editoptions: { value: gridFillList(\"" + _url + "\") " + dataEvents + "}";
            //return "formatter:'select', edittype: 'select', editoptions: { dataUrl: '" + _url + "'}";
        }
    }
}

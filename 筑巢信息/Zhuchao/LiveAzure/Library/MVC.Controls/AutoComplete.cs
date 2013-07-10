using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVC.Controls
{
    public class AutoComplete
    {

        public AutoComplete()
        {
            this.OnBeforeCallback = "null";
        }

        /// <summary>
        /// Creates an AutoComplete behaviour
        /// </summary>
        /// <param name="valueId">The name and id of the hidden element to create to store the selected element Id</param>
        /// <param name="onBeforeCallback">onBefore js event handler</param>
        /// <param name="searchUrl">the controller action to use to fetch the autocomplete results</param>
        /// <param name="delay">how many milliseconds to wait before fetching the results</param>
        /// <param name="resultCount">how many results to fetch from server</param>
        /// <param name="minLength">the minimun length to start autocompletion</param>
        public AutoComplete(string valueId = null,
                            string onBeforeCallback = "null",
                            string searchUrl = null,
                            int delay = 500,
                            int resultCount = 5,
                            int minLength = 1)
        {
            this.ValueId = valueId;
            this.OnBeforeCallback = onBeforeCallback;
            this.SearchUrl = searchUrl;
            this.Delay = delay;
            this.ResultCount = resultCount;
            this.MinLength = minLength;
        }

        /// <summary>
        /// name and id of the hidden element to create to store the selected element Id
        /// </summary>
        public string ValueId { get; set; }

        /// <summary>
        /// The onBefore js event handler
        /// </summary>
        public string OnBeforeCallback { get; set; }

        /// <summary>
        /// The controller action to use to fetch the autocomplete results
        /// </summary>
        public string SearchUrl { get; set; }

        /// <summary>
        /// How many milliseconds to wait before fetching the results
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// How many results to fetch from server
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        /// The minimun length to start autocompletion
        /// </summary>
        public int MinLength { get; set; }


        /// <summary>
        /// Renders the AutoComplete extension for the specified control
        /// </summary>
        /// <param name="id">The control's html-id</param>
        /// <returns></returns>
        public string Render(string id)
        {
            string hidden = string.Format("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" />", this.ValueId);

            string js = string.Format("_autoCompleteBinder(\"{0}\",\"{1}\",{2},\"{3}\",{4},{5},{6});",
                           "#" + id, 
                           "#" + this.ValueId, 
                           this.OnBeforeCallback, 
                           this.SearchUrl, 
                           this.Delay, 
                           this.ResultCount, 
                           this.MinLength);
            return hidden + string.Format(MVCControlsScriptManager.SCRIPT_TEMPLATE, js);
        }

    }
}

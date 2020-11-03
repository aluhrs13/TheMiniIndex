using System;
using System.Collections.Generic;
using System.Text;

namespace Supporting.ProfileParsing.JSONClasses
{

    public class GumroadObject
    {
        public int total { get; set; }
        public string total_formatted { get; set; }
        public int size { get; set; }
        public int result_count { get; set; }
        public Tags_Data[] tags_data { get; set; }
        public Filetypes_Data[] filetypes_data { get; set; }
        public object creator_counts { get; set; }
        public object category_slug { get; set; }
        public string category_name { get; set; }
        public string products_html { get; set; }
        public string sort_key { get; set; }
        public bool hide_product_filters_on_profile { get; set; }
        public int purchase_results_count { get; set; }
    }

    public class Tags_Data
    {
        public string key { get; set; }
        public int doc_count { get; set; }
    }

    public class Filetypes_Data
    {
        public string key { get; set; }
        public int doc_count { get; set; }
    }

}

using System;

public class ThingiverseModel
{
    public class Thing
    {
        public int id { get; set; }
        public string name { get; set; }
        public string thumbnail { get; set; }
        public string url { get; set; }
        public string public_url { get; set; }
        public Creator creator { get; set; }
        public DateTime added { get; set; }
        public DateTime modified { get; set; }
        public bool is_published { get; set; }
        public bool is_wip { get; set; }
        public bool is_featured { get; set; }
        public int like_count { get; set; }
        public bool is_liked { get; set; }
        public int collect_count { get; set; }
        public bool is_collected { get; set; }
        public bool is_watched { get; set; }
        public Default_Image default_image { get; set; }
        public string description { get; set; }
        public string instructions { get; set; }
        public string description_html { get; set; }
        public string instructions_html { get; set; }
        public string details { get; set; }
        public Details_Parts[] details_parts { get; set; }
        public string edu_details { get; set; }
        public Edu_Details_Parts[] edu_details_parts { get; set; }
        public string license { get; set; }
        public string files_url { get; set; }
        public string images_url { get; set; }
        public string likes_url { get; set; }
        public string ancestors_url { get; set; }
        public string derivatives_url { get; set; }
        public string tags_url { get; set; }
        public string categories_url { get; set; }
        public int file_count { get; set; }
        public int layout_count { get; set; }
        public string layouts_url { get; set; }
        public bool is_private { get; set; }
        public bool is_purchased { get; set; }
        public bool in_library { get; set; }
        public int print_history_count { get; set; }
        public object app_id { get; set; }
        public int download_count { get; set; }
        public int view_count { get; set; }
        public Education education { get; set; }
    }

    public class Creator
    {
        public int id { get; set; }
        public string name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string url { get; set; }
        public string public_url { get; set; }
        public string thumbnail { get; set; }
        public bool accepts_tips { get; set; }
    }

    public class Default_Image
    {
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public Size[] sizes { get; set; }
        public DateTime added { get; set; }
    }

    public class Size
    {
        public string type { get; set; }
        public string size { get; set; }
        public string url { get; set; }
    }

    public class Education
    {
        public object[] grades { get; set; }
        public object[] subjects { get; set; }
    }

    public class Details_Parts
    {
        public string type { get; set; }
        public string name { get; set; }
        public string required { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string content { get; set; }
    }

    public class Edu_Details_Parts
    {
        public string type { get; set; }
        public string name { get; set; }
        public object required { get; set; }
        public bool save_as_component { get; set; }
        public string template { get; set; }
        public string fieldname { get; set; }
        public string _default { get; set; }
        public object opts { get; set; }
        public string label { get; set; }
    }
}
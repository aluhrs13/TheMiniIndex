using System;
using System.Collections.Generic;
using System.Text;

namespace Supporting.ProfileParsing.JSONClasses
{
    public class ThingiverseObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string public_url { get; set; }
        public DateTime created_at { get; set; }
        public string thumbnail { get; set; }
        public string preview_image { get; set; }
        public Creator creator { get; set; }
        public int is_private { get; set; }
        public int is_purchased { get; set; }
        public int is_published { get; set; }
        public int comment_count { get; set; }
        public int like_count { get; set; }
        public Tag[] tags { get; set; }
        public object is_nsfw { get; set; }
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
        public int count_of_followers { get; set; }
        public int count_of_following { get; set; }
        public int count_of_designs { get; set; }
        public bool accepts_tips { get; set; }
        public bool is_following { get; set; }
        public string location { get; set; }
        public string cover { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string url { get; set; }
        public int count { get; set; }
        public string things_url { get; set; }
        public string absolute_url { get; set; }
    }


}

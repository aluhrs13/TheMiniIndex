using System;
using System.Collections.Generic;
using System.Text;

namespace Supporting.ProfileParsing.JSONClasses
{

    public class MyMiniFactoryObject
    {
        public int total_count { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public int visibility { get; set; }
        public string visibility_name { get; set; }
        public bool listed { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public string description { get; set; }
        public string description_html { get; set; }
        public object printing_details { get; set; }
        public object printing_details_html { get; set; }
        public bool featured { get; set; }
        public bool support { get; set; }
        public object complexity { get; set; }
        public object complexity_name { get; set; }
        public object dimensions { get; set; }
        public object material_quantity { get; set; }
        public Designer designer { get; set; }
        public Image1[] images { get; set; }
        public int views { get; set; }
        public int likes { get; set; }
        public DateTime? published_at { get; set; }
        public string[] tags { get; set; }
        public License[] licenses { get; set; }
        public object[] filters { get; set; }
        public Categories categories { get; set; }
        public Files files { get; set; }
        public Prints prints { get; set; }
        public string license { get; set; }
        public object archive_download_url { get; set; }
        public bool is_liked { get; set; }
        public object[] user_collections { get; set; }
        public bool is_saved { get; set; }
        public Price price { get; set; }
        public string purchase_url { get; set; }
        public bool is_bought { get; set; }
    }

    public class Designer
    {
        public string username { get; set; }
        public string name { get; set; }
        public bool is_admin { get; set; }
        public bool is_premium { get; set; }
        public bool is_store_manager { get; set; }
        public string profile_url { get; set; }
        public string profile_settings_url { get; set; }
        public string avatar_url { get; set; }
        public string avatar_thumbnail_url { get; set; }
        public string cover_url { get; set; }
        public string website { get; set; }
        public string bio { get; set; }
        public int followings { get; set; }
        public int followers { get; set; }
        public int likes { get; set; }
        public int views { get; set; }
        public int objects { get; set; }
        public int total_prints { get; set; }
        public int total_collections { get; set; }
        public int total_objects_store { get; set; }
        public object printing_since { get; set; }
        public Social_Networks social_networks { get; set; }
        public Printers printers { get; set; }
        public bool following { get; set; }
    }

    public class Social_Networks
    {
        public int total_count { get; set; }
        public Item1[] items { get; set; }
    }

    public class Item1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Printers
    {
        public int total_count { get; set; }
        public object[] items { get; set; }
    }

    public class Categories
    {
        public int total_count { get; set; }
        public Item2[] items { get; set; }
    }

    public class Item2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string slug { get; set; }
        public int? popularity { get; set; }
        public Parent parent { get; set; }
        public string banner_url { get; set; }
    }

    public class Parent
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string slug { get; set; }
        public int? popularity { get; set; }
        public Parent1 parent { get; set; }
        public string banner_url { get; set; }
    }

    public class Parent1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string slug { get; set; }
        public int popularity { get; set; }
        public object parent { get; set; }
        public string banner_url { get; set; }
    }

    public class Files
    {
        public int total_count { get; set; }
        public Item3[] items { get; set; }
    }

    public class Item3
    {
        public int id { get; set; }
        public string filename { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public string status_name { get; set; }
        public int? size { get; set; }
        public string thumbnail_url { get; set; }
        public string download_url { get; set; }
        public string[] render360_urls { get; set; }
        public string viewer_url { get; set; }
    }

    public class Prints
    {
        public int total_count { get; set; }
        public Item4[] items { get; set; }
    }

    public class Item4
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime published_at { get; set; }
        public int views { get; set; }
        public int likes { get; set; }
        public string url { get; set; }
        public Material material { get; set; }
        public Printer printer { get; set; }
        public Image[] images { get; set; }
        public User user { get; set; }
        public bool is_liked { get; set; }
    }

    public class Material
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Printer
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool automatic_slicing { get; set; }
        public bool? premium { get; set; }
        public string model { get; set; }
        public string brand { get; set; }
        public string image { get; set; }
        public string website { get; set; }
        public float? nozzle_diameter { get; set; }
        public Dimensions dimensions { get; set; }
    }

    public class Dimensions
    {
        public int? x { get; set; }
        public int? y { get; set; }
        public int? z { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public string name { get; set; }
        public bool is_admin { get; set; }
        public bool is_premium { get; set; }
        public bool is_store_manager { get; set; }
        public string profile_url { get; set; }
        public string profile_settings_url { get; set; }
        public string avatar_url { get; set; }
        public string avatar_thumbnail_url { get; set; }
        public string cover_url { get; set; }
        public object website { get; set; }
        public bool following { get; set; }
    }

    public class Image
    {
        public int id { get; set; }
        public string upload_id { get; set; }
        public bool is_primary { get; set; }
        public Original original { get; set; }
        public Tiny tiny { get; set; }
        public Thumbnail thumbnail { get; set; }
        public Standard standard { get; set; }
        public Large large { get; set; }
        public bool is_print_image_selected { get; set; }
    }

    public class Original
    {
        public string url { get; set; }
        public object width { get; set; }
        public object height { get; set; }
    }

    public class Tiny
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnail
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Standard
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Large
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Price
    {
        public string currency { get; set; }
        public string symbol { get; set; }
        public string value { get; set; }
    }

    public class Image1
    {
        public int id { get; set; }
        public string upload_id { get; set; }
        public bool is_primary { get; set; }
        public Original1 original { get; set; }
        public Tiny1 tiny { get; set; }
        public Thumbnail1 thumbnail { get; set; }
        public Standard1 standard { get; set; }
        public Large1 large { get; set; }
        public bool is_print_image_selected { get; set; }
    }

    public class Original1
    {
        public string url { get; set; }
        public object width { get; set; }
        public object height { get; set; }
    }

    public class Tiny1
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnail1
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Standard1
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Large1
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class License
    {
        public string type { get; set; }
        public bool value { get; set; }
    }

}

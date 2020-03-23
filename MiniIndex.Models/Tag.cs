using System.Collections.Generic;

namespace MiniIndex.Models
{
    public enum TagCategory
    {
        Gender,
        Race,
        Genre,
        Use,
        Size,
        Alignment,
        CreatureType,
        CreatureName,
        Class,
        Weapon,
        Armor,
        Clothing,
        Location,
        OtherDescription,
        Purpose,
        Scale
    }

    public class Tag
    {
        public Tag()
        {
            MiniTags = new List<MiniTag>();
        }

        public int ID { get; set; }
        public string TagName { get; set; }
        public List<MiniTag> MiniTags { get; set; }
        public TagCategory? Category { get; set; }
    }
}
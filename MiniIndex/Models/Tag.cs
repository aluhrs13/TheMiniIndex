using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        Purpose
    }
    public class Tag
    {
        public int ID { get; set; }
        public string TagName { get; set; }
        public List<MiniTag> MiniTags { get; set; }
        public TagCategory? Category { get; set; }

        public Tag()
        {
            MiniTags = new List<MiniTag>();
        }
    }

}

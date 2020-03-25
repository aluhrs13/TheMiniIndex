using System;
using System.Collections.Generic;
using System.Text;

namespace MiniIndex.Models
{
    public enum PairType
    {
        Synonym,
        Parent //For Parent/Child relationships, Tag1 is the child and Tag2 is the parent.
    }

    public class TagPair
    {
        public TagPair()
        {
        }

        public int ID { get; set; }
        public Tag Tag1 { get; set; }
        public Tag Tag2 { get; set; }
        public PairType Type { get; set; }

        public Tag GetPairedTag(Tag seedTag)
        {
            if(seedTag.ID == Tag1.ID)
            {
                return Tag2;
            }
            else
            {
                return Tag1;
            }
        }
    }
}

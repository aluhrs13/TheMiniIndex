using System;
using System.Collections.Generic;
using System.Text;

namespace MiniIndex.Core.Utilities
{
    public class AzureStorageConfig
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string ImageContainer { get; set; }
        public string ThumbnailContainer { get; set; }
    }
}

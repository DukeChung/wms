using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortuneLab.WebClient.Models
{
    public class Attachment
    {
        public Guid AttachmentId { get; set; }

        public string FileName { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string ThumbnailPath { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightAcademy.DomainLayer.ViewModel
{
    public class ChatMedia
    {
        public List<Image> Images { get; set; }
        public List<Video> Videos { get; set; }
        public List<File> Files { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Image
    {
        public string File { get; set; }
    }

    public class Video
    {
        public string File { get; set; }
    }

    public class File
    {
        public string Name { get; set; }
        public string FileBase64 { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
    }

    public class Link
    {
        public string Url { get; set; }
    }
}
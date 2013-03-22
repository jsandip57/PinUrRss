using System;
using System.Net;

namespace EasyPin
{
    public class DataToBind
    {
        public string Content
        {
            get;
            set;
        }
        public string Tag
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Image
        {
            get;
            set;
        }
        public string Pubdate
        {
            get;
            set;
        }
    }

    public class URI
    {
        public string Link
        {
            get;
            set;
        }
        public string Filename
        {
            get;
            set;
        }
        public string LastUpdate
        {
            get;
            set;
        }
        public string Frequency
        {
            get;
            set;
        }

    }
}

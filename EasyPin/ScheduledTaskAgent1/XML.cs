using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Xml;
using System.Net;

namespace ScheduledTaskAgent1
{
    class XML
    {
        public List<BindData> Retrive(string Data)
        {
            List<BindData> list = new List<BindData>();
            try
            {
            XDocument doc = XDocument.Parse(Data);
                foreach (XElement ele in doc.Descendants("item"))
                {
                    BindData d = new BindData();
                    d.Tag = ele.Element("link").Value;
                    d.Content = ele.Element("title").Value;
                    string destocheck = ele.Element("description").Value;
                    HtmlDocument HTdoc = new HtmlDocument();
                    HTdoc.LoadHtml(destocheck);
                    HTdoc.DetectEncodingHtml(destocheck);
                    d.Description = HttpUtility.HtmlDecode(HTdoc.DocumentNode.InnerText);
                    list.Add(d);
                }
                return list;
            }
            catch (Exception c)
            {
                list=null;
                return list;
            }
        }
    }
}

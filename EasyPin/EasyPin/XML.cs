using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyPin;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Xml;
using System.Net;

namespace EasyPin
{
    class XML
    {
        public List<DataToBind> Retrive(string Data)
        {
            List<DataToBind> list = new List<DataToBind>();
            try
            {
            XDocument doc = XDocument.Parse(Data);
                foreach (XElement ele in doc.Descendants("item"))
                {
                    DataToBind d = new DataToBind();
                    d.Tag = ele.Element("link").Value;
                    d.Content = ele.Element("title").Value;
                    d.Image = "/EasyPin;component/Images/Loading____Please_Wait.png";
                    string destocheck = ele.Element("description").Value;
                    HtmlDocument HTdoc = new HtmlDocument();
                    HTdoc.LoadHtml(destocheck);
                    HTdoc.DetectEncodingHtml(destocheck);
                    if (destocheck.Contains("</img>"))
                    {
                        try
                        {
                            HtmlAttribute att = HTdoc.DocumentNode.Element("//img").Attributes["src"];
                            d.Image = att.Value;
                        }
                        catch
                        {
                            d.Image = "/EasyPin;component/Images/Empty.png";
                        }
                    }
                    else
                    {
                        d.Image = "/EasyPin;component/Images/Empty.png";
                    }

                    d.Pubdate = ele.Element("pubDate").Value;
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

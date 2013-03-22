using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Xml;
using HtmlAgilityPack;

namespace EasyPin
{
    class GetData
    {
        string FileToSave, Link,Filename,LastUpdate,LatestDate;
        List<DataToBind> list = new List<DataToBind>();
        public void Update()
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Link);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
        }

        // STEP4 STEP4 STEP4
        private string ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string fil = httpwebStreamReader.ReadToEnd();
                    FileToSave = fil;
                    if (FileToSave != "")
                    {
                        FileManip f = new FileManip();
                        XML x = new XML();
                        list = x.Retrive(fil);
                        if (list.Select(e=>e.Pubdate).Contains(LatestDate))
                        {

                        }
                        f.Update(Filename, FileToSave);
                    }
                }
                myResponse.Close();
            }
            catch (Exception we)
            {

            }

        }
    }
}

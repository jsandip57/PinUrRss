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
    public partial class Navigate : PhoneApplicationPage
    {
        public string Link, weblink, filename;
        public bool key=false;
        List<DataToBind> list = new List<DataToBind>();
        public Navigate()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
                if (this.NavigationContext.QueryString.ContainsKey("Link")&&key==false)
                {
                    Link = NavigationContext.QueryString["Link"].ToString();
                    filename = NavigationContext.QueryString["FileName"].ToString();
                    GetData();
                }
        }

        public void GetData()
        {
            using (var read = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (read.FileExists(filename))
                {
                    try
                    {
                        using (StreamReader sw = new StreamReader(new IsolatedStorageFileStream(filename, FileMode.Open, read)))
                        {
                            string rea = sw.ReadToEnd();
                            EasyPin.XML x= new EasyPin.XML();
                            list = x.Retrive(rea);
                            Dispatcher.BeginInvoke(() => listBox1.ItemsSource = list);
                            key = true;
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("Error"));
                    }
                }
                else
                {
                   Dispatcher.BeginInvoke(() => MessageBox.Show("Does not exist"));
                }
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsCellularDataEnabled)
            {
                Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Visible);
                Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Collapsed);
                Refersh();
            }
            else
            {
               Dispatcher.BeginInvoke(() =>  MessageBox.Show("No Data Connection"));
            }
        }

        public void Refersh()
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Link);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }

        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string fil = httpwebStreamReader.ReadToEnd();
                    string FileToSave = fil;
                    EasyPin.XML x = new EasyPin.XML();
                    list = x.Retrive(fil);
                    FileManip manip = new FileManip();
                    if (manip.Update(filename, FileToSave) == "Updated")
                    {
                        Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Collapsed);
                        Dispatcher.BeginInvoke(() => listBox1.ItemsSource = list);
                        Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Visible);
                    }
                }
                myResponse.Close();
            }
            catch (Exception s)
            {
                Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Collapsed);
                Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Visible);
            }
        }


        private void TextBlock_DoubleTap(object sender, GestureEventArgs e)
        {
            if (Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsCellularDataEnabled)
            {
                TextBlock b = (TextBlock)sender;
                NavigationService.Navigate(new Uri("/Browse.xaml?Link=" + b.Tag, UriKind.RelativeOrAbsolute));
            }
            else
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("No data connection"));
            }
        }
    }
}
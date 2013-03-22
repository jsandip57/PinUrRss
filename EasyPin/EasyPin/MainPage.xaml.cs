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
using Microsoft.Phone.Scheduler;
using System.Threading;

namespace EasyPin
{
    public partial class MainPage : PhoneApplicationPage
    {
        string FileToSave;
        bool Data;
        public HttpWebRequest request;
        public MainPage()
        {
            InitializeComponent();
            FileManip m = new FileManip();
            IEnumerable<ShellTile> tilelist = ShellTile.ActiveTiles;
            m.Delete(tilelist);
            Data = Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsCellularDataEnabled;
            if (!Data)
            {
                MessageBox.Show("Data connection is not enabled. Kindly switch it on and comeback");
                Dispatcher.BeginInvoke(() =>  btn_Search.IsEnabled=false);
                Dispatcher.BeginInvoke(() => btn_SearchR.IsEnabled=false);
            }
        }
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Search.Text != "")
            {
                GetData();
            }
            else
            {
                MessageBox.Show("Search is invalid");
            }
        }
        public void GetData()
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                request = (HttpWebRequest)HttpWebRequest.Create("http://www.search4rss.com/search.php?lang=en&q=" + txt_Search.Text);
                Dispatcher.BeginInvoke(() => RssList.Visibility=Visibility.Collapsed);
                Dispatcher.BeginInvoke(() => Loading_for_Search.Visibility=Visibility.Visible);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
        }

        // STEP4 STEP4 STEP4
        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string fil = httpwebStreamReader.ReadToEnd();
                    int i = 0;
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(fil);
                    IEnumerable<HtmlNode> node = doc.DocumentNode.FirstChild.Elements("head");
                    if (node != null)
                    {
                        List<DataToBind> dat = new List<DataToBind>();
                        IEnumerable<HtmlNode> node1 = doc.DocumentNode.Descendants("div").Where(e => e.Id == "results");
                        foreach (HtmlNode nod1 in node1)
                        {
                            IEnumerable<HtmlNode> node2 = nod1.Descendants("a");
                            DataToBind data = new DataToBind();
                            data.Content = HttpUtility.HtmlDecode(node2.First().InnerText);
                            data.Description = HttpUtility.HtmlDecode(nod1.Descendants("div").ElementAt(2).InnerText);
                            data.Tag = node2.Last().InnerText;
                            dat.Add(data);
                            i++;
                        }
                        Dispatcher.BeginInvoke(() => Loading_for_Search.Visibility = Visibility.Collapsed);
                        Dispatcher.BeginInvoke(() => RssList.Visibility = Visibility.Visible);
                        Dispatcher.BeginInvoke(() => RssList.ItemsSource = dat);
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("No results found"));
                    }
                }
                myResponse.Close();
            }
            catch (Exception qw)
            {
                Dispatcher.BeginInvoke(() => Loading_for_Search.Visibility = Visibility.Collapsed);
                Dispatcher.BeginInvoke(() => MessageBox.Show("No results found"));
            }

        }

        private void TextBlock_DoubleTap(object sender, GestureEventArgs e)
        {
            try
            {
                TextBlock text = (TextBlock)sender;
                NavigationService.Navigate(new Uri("/TestWindow.xaml?Link=" + text.Tag, UriKind.RelativeOrAbsolute));
            }
            catch (Exception ew)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("No Link associated with it"));
            }
        }
        public string Link, weblink;
        private void btn_SearchR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_SearchR.Text != "")
                {
                    Link = txt_SearchR.Text;
                    GetData(txt_SearchR.Text);
                }
                else
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Invalid URL"));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>  MessageBox.Show(ex.Message));
            }
        }
        List<DataToBind> list = new List<DataToBind>();
        public void GetData(string Link)
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Link);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallbackR), request);
                Dispatcher.BeginInvoke(() => Loading_for_Read.Visibility=Visibility.Visible);
                Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Collapsed);
            }
        }

        // STEP4 STEP4 STEP4
        private void ReadWebRequestCallbackR(IAsyncResult callbackResult)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    string fil = httpwebStreamReader.ReadToEnd();
                    FileToSave = fil;
                    EasyPin.XML x = new EasyPin.XML();
                    list = x.Retrive(fil);
                    Dispatcher.BeginInvoke(() => Loading_for_Read.Visibility = Visibility.Collapsed);
                    Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Visible);
                    Dispatcher.BeginInvoke(() => listBox1.ItemsSource = list);
                }
                myResponse.Close();
            }
            catch (Exception d)
            {
                Dispatcher.BeginInvoke(() => Loading_for_Read.Visibility = Visibility.Collapsed);
                Dispatcher.BeginInvoke(() => txt_SearchR.Text="Page Not Found");
            }
        }

        private void TextBlock_DoubleTap_1(object sender, GestureEventArgs e)
        {
            try
            {
                TextBlock b = (TextBlock)sender;
                NavigationService.Navigate(new Uri("/Browse.xaml?Link=" + b.Tag, UriKind.RelativeOrAbsolute));
            }
            catch (Exception we)
            {
                Dispatcher.BeginInvoke(() => txt_SearchR.Text = "No Data in the give link");
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            PeriodicTask UpdateTask = new PeriodicTask("Update It");
            UpdateTask.Description = "Dynamic tiles";
            try
            {
                ScheduledActionService.Add(UpdateTask);
                ScheduledActionService.LaunchForTest("Update It", TimeSpan.FromSeconds(3));
                Dispatcher.BeginInvoke(() =>MessageBox.Show("Sheduled"));
            }
            catch (Exception exu)
            {
               Dispatcher.BeginInvoke(() => MessageBox.Show(exu.Message));
            }
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                ScheduledActionService.Remove("Update It");
               Dispatcher.BeginInvoke(() => MessageBox.Show("Turn off the background agent successfully"));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("doesn't exist"))
                {
                    Dispatcher.BeginInvoke(() =>MessageBox.Show("Nothing to turn off"));
                }
            }
            catch (SchedulerServiceException)
            {

            } 
        }

        private void Pinit_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                PopUp.Visibility = Visibility.Visible;
            }
            else
            {
                Dispatcher.BeginInvoke(() =>MessageBox.Show("Read a RSS Link before pinning it"));
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TXT_Create.Text != "")
                {

                    FileManip f = new FileManip();
                    if (!f.LinkList(ShellTile.ActiveTiles).Contains(txt_SearchR.Text))
                    {
                        string filename = f.CreateTile(FileToSave);
                        if (filename != null)
                        {
                            ShellTile.Create(new Uri("/Navigate.xaml?Link=" + Link + "&FileName=" + filename, UriKind.RelativeOrAbsolute), new StandardTileData { Title = TXT_Create.Text, BackContent = list.First().Content });
                        }
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("Tile exist"));
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Invalid Tile name"));
                }
            }
            catch
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("Error"));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => PopUp.Visibility = Visibility.Collapsed);
        }

        private void Pivot_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsCellularDataEnabled)
            {
               Dispatcher.BeginInvoke(() =>  MessageBox.Show("Data connection not enabled"));
                Dispatcher.BeginInvoke(() => btn_Search.IsEnabled = false);
                Dispatcher.BeginInvoke(() => btn_SearchR.IsEnabled = false);
            }
            else
            {
                Dispatcher.BeginInvoke(() => btn_Search.IsEnabled = true);
                Dispatcher.BeginInvoke(() => btn_SearchR.IsEnabled = true);
            }
        }

        private void Help_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.RelativeOrAbsolute));
        }

        private void txt_Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_Search.Text == "Enter a keyword ")
            {
                Dispatcher.BeginInvoke(() => txt_Search.Text = "");
            }
        }

        private void txt_SearchR_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_SearchR.Text == "Enter a URL to Read")
            {
                Dispatcher.BeginInvoke(() => txt_SearchR.Text = "");
            }
        }

        private void TXT_Create_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TXT_Create.Text == "TileName")
            {
                Dispatcher.BeginInvoke(() => TXT_Create.Text = "");
            }
        }
    }
}
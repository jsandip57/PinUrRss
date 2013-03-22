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
using HtmlAgilityPack;
using System.Xml;
using EasyPin;

namespace EasyPin
{
    public partial class TestWindow : PhoneApplicationPage
    {
        public string Link,weblink,FileToSave;
        List<DataToBind> list = new List<DataToBind>();
        public TestWindow()
        {
            InitializeComponent();

        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("Link"))
            {
                Link = NavigationContext.QueryString["Link"].ToString();
                GetData();
            }
        }

        public void GetData()
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Visible);
                Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Collapsed);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Link);
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
                    FileToSave = fil;
                    XML x = new XML();
                    list = x.Retrive(fil);
                    Dispatcher.BeginInvoke(() => listBox1.ItemsSource = list);
                    Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Collapsed);
                    Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Visible);
                    if (list == null)
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("No items in this link......:("));
                    }
                }
                myResponse.Close();
            }
            catch (Exception we)
            {
                Dispatcher.BeginInvoke(() => image1.Visibility = Visibility.Collapsed);
                Dispatcher.BeginInvoke(() => listBox1.Visibility = Visibility.Visible);
                Dispatcher.BeginInvoke(() => MessageBox.Show("No items in this link......:("));
            }
            
        }

        private void TextBlock_DoubleTap(object sender, GestureEventArgs e)
        {
            TextBlock b = (TextBlock)sender;
            NavigationService.Navigate(new Uri("/Browse.xaml?Link=" + b.Tag, UriKind.RelativeOrAbsolute));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke(() => adControl1.Visibility = Visibility.Collapsed);
            Dispatcher.BeginInvoke(() => PopUp.Visibility = Visibility.Visible);
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TXT_Create.Text != "")
                {
                    FileManip f = new FileManip();
                    if (!f.LinkList(ShellTile.ActiveTiles).Contains(Link))
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
            }
            catch
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("Error"));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Dispatcher.BeginInvoke(() => adControl1.Visibility = Visibility.Visible);
            Dispatcher.BeginInvoke(() => PopUp.Visibility = Visibility.Collapsed);
        }

        private void TXT_Create_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TXT_Create.Text == "TileName")
            {
                Dispatcher.BeginInvoke(() => TXT_Create.Text="");
            }
        }
    }
}
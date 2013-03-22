using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace EasyPin
{
    public partial class Browse : PhoneApplicationPage
    {
        public bool key=false;
        public Browse()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                if (this.NavigationContext.QueryString.ContainsKey("Link")&&key==false)
                {
                    string s;
                    s = NavigationContext.QueryString["Link"].ToString();
                    webBrowser1.Navigate(new Uri(s, UriKind.RelativeOrAbsolute));
                    key = true;
                }
            }
            catch (Exception r)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("Page does not exist"));
            }
        }
    }
}
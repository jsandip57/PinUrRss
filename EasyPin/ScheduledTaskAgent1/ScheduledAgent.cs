using System.Windows;
using Microsoft.Phone.Scheduler;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Net;
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
using System.Xml;
using HtmlAgilityPack;


namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in backgroun

            try
            {
                int q = ShellTile.ActiveTiles.Count();
                foreach (ShellTile t in ShellTile.ActiveTiles)
                {
                    if (t.NavigationUri != ShellTile.ActiveTiles.First().NavigationUri)
                    {
                        List<BindData> list = new List<BindData>();
                        ScheduledTaskAgent1.XML x = new ScheduledTaskAgent1.XML();
                        string link, filename, uri = t.NavigationUri.ToString();
                        string uri1 = uri.Remove(0, uri.IndexOf("=") + 1);
                        link = uri1.Substring(0, uri1.IndexOf("&FileName"));
                        string uri2 = uri1.Remove(0, uri1.IndexOf("&FileName") + 10);
                        filename = uri2;
                        string data = GetData(filename);
                        list = x.Retrive(data);
                        BindData toupdate = new BindData();
                        Random rand = new Random();
                        toupdate = list.ElementAt(rand.Next(list.Count));
                        t.Update(new StandardTileData { BackContent = toupdate.Content });
                    }
                }
                //ShellToast toast = new ShellToast();
                //Mutex mutex = new Mutex(true, "ScheduledAgentData");
                //mutex.WaitOne();
                //IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                //toast.Title = "Tile";
                //mutex.ReleaseMutex();
                //toast.Content = "Updated";
                //toast.Show();
                //ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(3));
            }
            catch(Exception e)
            {
                ShellToast toast = new ShellToast();
                Mutex mutex = new Mutex(true, "ScheduledAgentData");
                mutex.WaitOne();
                IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
                toast.Title = "Tile";
                mutex.ReleaseMutex();
                toast.Content = "Tile Updation failed due to"+e.Message;
                toast.Show();
                //ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromMinutes(3));
            }
            NotifyComplete();
        }

        public string GetData(string filename)
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
                            return rea;
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
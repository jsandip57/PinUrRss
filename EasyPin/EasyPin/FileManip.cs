using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.Generic;
using Microsoft.Phone.Shell;

namespace EasyPin
{
    public class FileManip
    {
        public string CreateTile(string FileToSave)
        {
            try
            {
                Random r = new Random();
                string filename = r.Next(100000).ToString() + ".txt";
                using (var File = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (!File.FileExists(filename))
                        {
                            IsolatedStorageFileStream cr = File.CreateFile(filename);
                            cr.Close();
                            try
                            {
                                using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(filename, FileMode.Open, File)))
                                {
                                    sw.WriteLine(FileToSave);
                                    sw.Close();
                                }
                                return filename;
                            }
                            catch (Exception ec)
                            {
                                //MessageBox.Show(ec.Message);
                                return null;
                            }
                           
                        }
                        else
                        {
                            //MessageBox.Show("Tile exists");
                            return null;
                        }
                    }
                    catch (Exception ec)
                    {
                        //MessageBox.Show("Error");
                        return null;
                    }
                }
            }
            catch (Exception q)
            {
                //MessageBox.Show(q.Message);
                return null;
            }
        }

        public string Update(string filename,string FileToSave)
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (File.FileExists(filename))
                    {
                        try
                        {
                            File.DeleteFile(filename);
                            IsolatedStorageFileStream cr = File.CreateFile(filename);
                            cr.Close();
                            using (StreamWriter sw = new StreamWriter(new IsolatedStorageFileStream(filename, FileMode.Open, File)))
                            {
                                sw.WriteLine(FileToSave);
                                sw.Close();
                            }
                            return "Updated";
                        }
                        catch (Exception ec)
                        {
                            //MessageBox.Show(ec.Message);
                            return null;
                        }

                    }
                    else
                    {
                        //MessageBox.Show("Tile Doesn't exists");
                        return null;
                    }
                }
                catch (Exception ec)
                {
                    //MessageBox.Show("Error");
                    return null;
                }
            }
        }

        public string Delete(IEnumerable<ShellTile> TileList)
        {
            using (var File = IsolatedStorageFile.GetUserStoreForApplication())
            {
                List<DataToBind> listtosen = new List<DataToBind>();
                List<string> Filelist = new List<string>();
                try
                {
                    int i = 0;
                    foreach (ShellTile t in TileList)
                    {
                        if (i > 0)
                        {
                            string uri = t.NavigationUri.ToString();
                            string uri1 = uri.Remove(0, uri.IndexOf("=") + 1);
                            string Filename = uri1.Remove(0, uri1.IndexOf("&FileName") + 10);
                            Filelist.Add(Filename);
                        }
                        i++;
                    }
                    foreach (string file in File.GetFileNames("*"))
                    {
                        if (!Filelist.Contains(file))
                        {
                            File.DeleteFile(file);
                        }
                    }
                    int j=0;
                    foreach (string a in File.GetFileNames("*"))
                    {
                        j++;
                    }
                    return j.ToString()+i.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<string> LinkList(IEnumerable<ShellTile> TileList)
        {
            try
            {
                List<string> Link = new List<string>();
                int i = 0;
                foreach (ShellTile t in TileList)
                {
                    if (i > 0)
                    {
                        string uri = t.NavigationUri.ToString();
                        string uri1 = uri.Remove(0, uri.IndexOf("=") + 1);
                        string link = uri1.Substring(0, uri1.IndexOf("&FileName"));
                        Link.Add(link);
                    }
                    i++;
                }
                return Link;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

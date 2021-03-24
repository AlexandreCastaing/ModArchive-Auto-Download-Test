using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace ModArchiveAutoDownloader
{
    class Work
    {
        int idWork = -1;
        ThreadWorks parent;

        string dataTrackerPath = "";

        int firstID;
        int lastID;

        public Work(ThreadWorks _parent, int _idWork
            , string _pathToDownload, int _firstID, int _lastID)
        {
            parent = _parent;
            idWork = _idWork;
            dataTrackerPath = _pathToDownload;
            firstID = _firstID;
            lastID = _lastID;

            _workByAPIID();

        }

        private const string URL =
            "https://modarchive.org/index.php?request=view_player&query=random";

        private const string DATA = @"{}";

        private string URLAPI =
            "https://api.modarchive.org/downloads.php?moduleid=";

        string downloadLinkDefault = "https://api.modarchive.org/downloads.php";



        /* public async void _workByHTML()
         {
             try
             {
                 string html = HTTPRequest();

                 string linkFile = get_filesurl_from_html(html);
                 if (linkFile == "") throw new Exception("link unfounded.");

                 string confirm = download_from_urlHTML(linkFile);
                 Console.WriteLine("> " + confirm);
             }
             catch (Exception e)
             {
                 Console.WriteLine("err: " + e);
             }
         }
        */

        public async void _workByAPIID()
        {
            try
            {

                string linkFile = get_random_link_APIID(
                    firstID, lastID); // 45000, 165000 );

                string confirm = download_from_urlAPI(linkFile);
                Console.WriteLine("> " + confirm);
            }
            catch (Exception e)
            {
                Console.WriteLine("err: " + e);
            }
        }


       
        private string get_random_link_APIID(int min, int max)
        {
            Random aleatoire = new Random();
            int id = aleatoire.Next(min, max); //Génère un entier compris entre 0 et 9

            string url = URLAPI + id.ToString();
            return url;
        }


        private string downloadFolderPath()
        {
            string f = "";

            f += dataTrackerPath;

            DateTime dt = DateTime.Now;
            string directory = "" + dt.ToString("yyyy_MM_dd");

            f += "" + directory + @"";

            return f;
        }

        private string HTTPRequest()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = DATA.Length;
            using (Stream webStream = request.GetRequestStream())
            using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            {
                requestWriter.Write(DATA);
            }

            try
            {
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    return response;
                }
            }
            catch (Exception e)
            {
                throw new Exception("REQUEST... "+ e.ToString());
            }

        }

        private string get_filesurl_from_html(string _html)
        {
            try { 
                int firstindex = _html.IndexOf(downloadLinkDefault);

                string lengthindexHTML = _html.Substring(firstindex);
                int lengthurl = lengthindexHTML.IndexOf('"');

                string link = _html.Substring(firstindex, lengthurl);

                return link;
            }
            catch
            {
                return "";
            }
        }


        private string download_from_urlHTML(string _url)
        {
            int indexfilename = _url.LastIndexOf('#');
            string filename = _url.Substring(indexfilename + 1);

            string folderPath = downloadFolderPath();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            Directory.SetCurrentDirectory(
               folderPath
            );

            using (var client = new WebClient())
            {
                client.DownloadFile(_url, filename);
            }

            return folderPath + "\\" + filename;
        }

        private string getFilename(string hreflink)
        {
            Uri uri = new Uri(hreflink);

            string filename = System.IO.Path.GetFileName(uri.LocalPath);

            return filename;
        }

        private string download_from_urlAPI(string _url)
        {

            string folderPath = "";
            string originalFileName = "";

            try
            {
                folderPath = downloadFolderPath();

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                Directory.SetCurrentDirectory(
                   folderPath
                );          

                using (var client = new WebClient())
                {
                    try
                    {
                       
                        WebRequest request = WebRequest.Create(_url);
                        WebResponse response = request.GetResponse();
                        originalFileName = response.Headers["Content-Disposition"];
                        //Stream streamWithFileBody = response.GetResponseStream();

                        if(originalFileName != null) { 
                            originalFileName = originalFileName.Replace("attachment; filename=", "");

                            /*
                             client.DownloadFile(_url, originalFileName);
                             */

                            client.DownloadProgressChanged += (s, e) =>
                            {
                                try { 
                                    parent.progresses[idWork] = e.ProgressPercentage;
                                    parent.titleInfoProgression();
                                }
                                catch { }
                            };

                            client.DownloadFileTaskAsync(new Uri(_url), originalFileName);

                            return "OK: " + folderPath + "\\" + originalFileName;
                        }
                        else
                        {
                            return "ERR 2.. " + folderPath + "\\" + originalFileName;
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return "ERR.. "+ folderPath+"\\"+ originalFileName;
        }

    }
}

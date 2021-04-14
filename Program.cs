using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ModArchiveAutoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {



            // path to download
            string dataTrackerPath = @"D:\alex\ARCHIVE\Dev\ModArchive\DATA\";
           
            // random between min & max id
            int firstID = 175000; //45000
            int lastID = 180000; //165000

            string format = "not working todo";

            int limitMusics = 5000;

            Console.WriteLine("Press + or - for change numb of threads.");

            string EnvDir = Directory.GetCurrentDirectory();
            ThreadWorks tws = new ThreadWorks(1, dataTrackerPath, firstID, lastID, limitMusics, format, EnvDir);

            while (true)
            {

                if (Console.KeyAvailable) {
                    ConsoleKeyInfo ck = Console.ReadKey(false);

                    if (ck.KeyChar == '-')
                        tws.remove_LastThread();

                    if (ck.KeyChar == '+')
                        tws.add_LastThread();
                }


                tws.frame();

                
            }

        }

    }


}

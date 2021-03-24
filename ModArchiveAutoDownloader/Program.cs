using System;
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
            int firstID = 45000;
            int lastID = 165000;

            Console.WriteLine("Press + or - for change numb of threads.");

            ThreadWorks tws = new ThreadWorks(1, dataTrackerPath, firstID, lastID);

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

using System;

namespace ModArchiveAutoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                    try
                    {
                        new Work();
                    }
                    catch (Exception EX)
                    {
                        Console.WriteLine("ERR: " + EX.ToString());
                    }
            }
        }

    }


}

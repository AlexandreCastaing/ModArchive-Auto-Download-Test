using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModArchiveAutoDownloader
{
    class ThreadWorks
    {
        List<Task<Work>> works;
        public List<int> progresses;

        string dataTrackerPath;
        int firstID;
        int lastID;

        public ThreadWorks(int _numThread, string _pathToDownload,
            int _first_ID_MODARCHV, int _last_ID_MODARCHV)
        {
            Console.Title = "";

            dataTrackerPath = _pathToDownload;
            firstID = _first_ID_MODARCHV;
            lastID = _last_ID_MODARCHV;

            works = new List<Task<Work>>();
            progresses = new List<int>();

            for (int i = 0; i < _numThread; i++) { 
                works.Add(null);
                progresses.Add(-1);
            }

        }

        public void add_LastThread()
        {
            works.Add(null);
            progresses.Add(-1);
            Console.WriteLine("- Num Threads: " + works.Count);
        }
        public void remove_LastThread()
        {
            if (works.Count >= 1)
            {
                progresses.RemoveAt(works.Count - 1);
                works.RemoveAt(works.Count - 1);
            }
            Console.WriteLine("- Num Threads: " + works.Count );
        }

        public void titleInfoProgression()
        {
            string title = "";

            for (int i = 0; i < works.Count; i++)
            {
                try
                {
                    Task<Work> taskWork = works[i];

                    if (taskWork == null)
                    {
                        title += /*i + "=" +*/ "null" + " ;  ";
                    }
                    else if (taskWork.IsCompleted)
                        title += /*i + "=" +*/ "ok" + " ;  ";
                    else
                    {
                        int val = progresses[i];

                        if(val >= 0)
                            title += /*i + "=" +*/ val +" ; ";
                    }
                        

                }
                catch{
      
                }
            }

            Console.Title = title;
        }

        public void frame()
        {
            for( int i = 0; i < works.Count ; i++)  
            {
                try
                {
                    int id = i;
                    Task<Work> taskWork = works[id];

                    if(taskWork == null)
                    {
                        taskWork = Task.Run(() => new Work(this, id, dataTrackerPath, firstID, lastID));
                    }
                    else if (taskWork.IsCompleted)
                    {
                        taskWork = Task.Run(() => new Work(this, id, dataTrackerPath, firstID, lastID));
                        titleInfoProgression();
                    }

                    works[i] = taskWork;


                }
                catch (Exception EX)
                {
                    Console.WriteLine("ERR MAIN TRY: " + EX.ToString());
                }

            }


        }

    }
}

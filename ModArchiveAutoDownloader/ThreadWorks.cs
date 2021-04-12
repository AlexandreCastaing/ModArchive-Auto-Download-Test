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
        public List<bool> iscompleted;

        string dataTrackerPath;
        int firstID;
        int lastID;
        int nTracks = 0;
        int nTracksTotal = 0;
        string format;
        public ThreadWorks(int _numThread, string _pathToDownload,
            int _first_ID_MODARCHV, int _last_ID_MODARCHV, int _totalNumTracks, string _format)
        {
            Console.Title = "";

            dataTrackerPath = _pathToDownload;
            firstID = _first_ID_MODARCHV;
            lastID = _last_ID_MODARCHV;
            nTracksTotal = _totalNumTracks;
            nTracks = 0;
            format = _format;

            works = new List<Task<Work>>();
            progresses = new List<int>();
            iscompleted = new List<bool>();

            add_LastThread();

        }

        public void add_LastThread()
        {
            works.Add(null);
            iscompleted.Add(false);
            progresses.Add(-1);
            Console.WriteLine("- Num Threads: " + works.Count);
        }
        public void remove_LastThread()
        {
            if (works.Count >= 1)
            {
                progresses.RemoveAt(works.Count - 1);
                iscompleted.RemoveAt(works.Count - 1);
                works.RemoveAt(works.Count - 1);
            }
            Console.WriteLine("- Num Threads: " + works.Count );
        }

        public void incrementNMusic()
        {
            nTracks++;
        }

        public void decrementNMusic()
        {
            nTracks--;
        }

        public void titleInfoProgression()
        {
            string title = "";

            title += " ["+nTracks+"/"+nTracksTotal+"] .. ";

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

        public void taskComplete(int _id)
        {
            try { 
                iscompleted[_id] = true;
            }
            catch { }
        }

        public void frame()
        {
            for( int i = 0; i < works.Count ; i++)  
            {
                try
                {
                    int id = i;
                    Task<Work> taskWork = works[id];

                    // limits
                    if(nTracks < nTracksTotal) { 

                        if(taskWork == null)
                        {
                            incrementNMusic();
                            taskWork = Task.Run(() => new Work(this, id, dataTrackerPath, firstID, lastID, format));
                        }
                        else if (taskWork.IsCompleted)
                        {
                            if(iscompleted[i] == true) { 

                                incrementNMusic();
                                iscompleted[i] = false;

                                taskWork = Task.Run(() => new Work(this, id, dataTrackerPath, firstID, lastID, format));
                                titleInfoProgression();

                            }
                        }

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

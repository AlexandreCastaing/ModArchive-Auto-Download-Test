

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Google.Apis.YouTube.Samples;

namespace ModArchiveAutoDownloader
{
    class ConvertMP3
    {

        public bool result = false;
        ThreadWorks gparent;
        public ConvertMP3(string folderPath, string filenameMOD, ThreadWorks _thr)
        {
            gparent = _thr;

            result = true;
            try
            {             

                string fileMod = folderPath + "\\" + filenameMOD;
                string pathMp3 = folderPath + "\\WAV";
                string pathMp4 = folderPath + "\\MP4";


                // openmpt123 [options] [--] file1 [file2] ...
                // openmpt123.exe --render icebreaker.mod
                ProcessStartInfo p = new ProcessStartInfo("openmpt123.exe");
                string args = "--render \"" + fileMod+"\"";

                //p.CreateNoWindow = true;
                p.RedirectStandardOutput = false; // true;
                p.Arguments = args;

                Process process = new Process();
                process.StartInfo = p;
                process.Start();
                process.WaitForExit(90000);
                process.Close();

                string pathWav = pathMp3 + "\\" + filenameMOD + ".wav";

                if (!Directory.Exists(pathMp3))
                    Directory.CreateDirectory(pathMp3);
                if (File.Exists(pathWav))
                    File.Delete(pathWav);
                File.Move(fileMod + ".wav", pathWav);

                replaceMP4(pathWav, pathMp4, filenameMOD);
            }
            catch (Exception e){ result = false; }; 
        }
        private void replaceMP4(string pathWAV, string pathMp4, string filenameMOD)
        {
            try
            {
                string pathPRJCT = gparent.envdir;
                string pathffmpeg = pathPRJCT + @"\_ffm\bin\";

                ProcessStartInfo p = new ProcessStartInfo(pathffmpeg+"ffmpeg.exe");
                /*string args = "-r 20 "
                    + "-i \"" + pathWAV + "\" "
                    + "-f image2 -s 740x414 "
                    + "-i rdmmdl740x414.png "
                    + "-vcodec libx264 -crf 25  -pix_fmt yuv420p "
                    + "\""+pathMp4+""
                    + "\\"+ filenameMOD+ ".mp4\"";
                */

                string pathFileMP4 = pathMp4 +
                     "\\" + filenameMOD + ".mkv";
                string pathFileMP4Guille = "\"" + pathFileMP4 + "\"";

                string args = "" +
                    "-loop 1 -r 4 -framerate 2 -i it1_560x340.png -i " +
                    "\"" + pathWAV + "\" " +
                    "-c:v libx264 -preset medium -tune stillimage -crf 18 -c:a copy -shortest -pix_fmt yuv420p " +
                     pathFileMP4Guille;



                p.WorkingDirectory = pathffmpeg;

                p.RedirectStandardOutput = false; // true;
                p.Arguments = args;
               
                Process process = new Process();
                process.StartInfo = p;

                if (!Directory.Exists(pathMp4))
                    Directory.CreateDirectory(pathMp4);

                process.Start();
                process.WaitForExit(90000);
                process.Close();


                File.Delete(pathWAV);

                //YOUTUBE(filenameMOD, pathFileMP4, pathPRJCT);
            }
            catch (Exception e){ }
        }
        void YOUTUBE(string _title, string _pathVIDEO, string _appliPath)
        {
            try
            {
                new UploadVideo().Run(_title, _pathVIDEO, _appliPath);
                
            }
            catch
            {}
            
        }
    }
}



/*
 * 
 * 
 *  // *** 


                //MikMod player;
                //player = new MikMod();
                //player.Init<NaudioDriver>();

                //NaudioDriver naudD = new NaudioDriver();
               
                byte[] f = File.ReadAllBytes(fileMod);
                Stream moduleStream = new MemoryStream(f);



                SongModule myMod = ModuleLoader.Instance.LoadModule("SongModule.Mod|S3M|XM");
                ModulePlayer player = new ModulePlayer(myMod);

                // Or NAudio Driver
                SharpMod.SoundRenderer.NAudioWaveChannelDriver drv = new SharpMod.SoundRenderer.NAudioWaveChannelDriver(NAudioWaveChannelDriver.Output.WaveOut);

                player.RegisterRenderer(drv);
                player.Start();


SongModule myMod = SharpMod.ModuleLoader.Instance.LoadModule("SongModule.Mod|S3M|XM");
ModulePlayer player = new ModulePlayer(myMod);

 // Or NAudio Driver
SharpMod.SoundRenderer.NAudioWaveChannelDriver drv = new SharpMod.SoundRenderer.NAudioWaveChannelDriver(NAudioWaveChannelDriver.Output.WaveOut);

player.RegisterRenderer(drv);
player.Start();*/


//sbyte[] signed = (sbyte[]) (Array) f;

//naudD.WriteBytes(signed, 0);



/*ModulePlayer player = new ModulePlayer(myMod);

Module test3 = player.LoadModule(moduleStream);

Module test4 = ModuleLoader.Load(moduleStream, 64, 0);


//Module test = ModuleLoader.Load(fileMod);

Module mod = null;

Directory.SetCurrentDirectory(folderPath);

ModDriver.LoadDriver<NaudioDriver>();
ModDriver.MikMod_Init("");

mod = player.LoadModule(fileMod);


mod.wrap = true;

mod.songname = filenameMP3;



StreamWriter mp3 = new StreamWriter(pathToConvert);

for (int j = 0; j < mod.tracks.Length; j++)
{
    byte[] buffer = mod.tracks[j];
    for (int i = 0; i < buffer.Length; i++)
        mp3.BaseStream.WriteByte(buffer[i]);
}
Console.WriteLine("OOF: " + mp3.BaseStream.Length);
*/
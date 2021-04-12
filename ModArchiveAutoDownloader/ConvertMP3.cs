

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ModArchiveAutoDownloader
{
    class ConvertMP3
    {

        public bool result = false;
        public ConvertMP3(string folderPath, string filenameMOD)
        {
            result = true;
            try
            {             

                string fileMod = folderPath + "\\" + filenameMOD;
                string pathMp3 = folderPath + "\\WAV";


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

                if(!Directory.Exists(pathMp3))
                    Directory.CreateDirectory(pathMp3);
                if (File.Exists(pathMp3 + "\\" + filenameMOD + ".wav"))
                    File.Delete(pathMp3 + "\\" + filenameMOD + ".wav");
                File.Move(fileMod + ".wav", pathMp3 + "\\" + filenameMOD + ".wav");

            }
            catch (Exception e){ result = false; }; 
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
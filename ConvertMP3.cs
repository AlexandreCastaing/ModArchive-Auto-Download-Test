

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Google.Apis.YouTube.Samples;
using NAudio.Wave;
using System.Linq;
using NPOI.Util;
using java.io;

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
                string pathMp3_2 = pathMp3 + "\\" + filenameMOD + ".mp3";

                if (!Directory.Exists(pathMp3))
                    Directory.CreateDirectory(pathMp3);
                if (System.IO.File.Exists(pathWav))
                    System.IO.File.Delete(pathWav);
                System.IO.File.Move(fileMod + ".wav", pathWav);



                // *o*o*o*o*o*o*o*o* STEP 2  CONVERT TO MP3 NORMALIZED
                //ffmpeg -i input.flac -filter:a "volume=1" output.mp3

                try // delete old file
                {
                    System.IO.File.Delete(pathMp3_2);
                }
                catch { }

                string args2 = "-i " +
                 "\"" + pathWav + "" + "\" " +
                 "-filter:a \"volume = 1\" " +
                "\""+pathMp3_2+"\"";

                string pathPRJCT = gparent.envdir;
                string pathffmpeg = pathPRJCT + @"\_ffm\bin\";
                ProcessStartInfo p2 = new ProcessStartInfo(pathffmpeg + "ffmpeg.exe");

                p2.WorkingDirectory = pathffmpeg;

                p2.RedirectStandardOutput = false; // true;
                p2.Arguments = args2;

                Process process2 = new Process();
                process2.StartInfo = p2;

                process2.Start();
                process2.WaitForExit(60000 * 60);
                process2.Close();

                System.IO.File.Delete(pathWav);
                // *o*o*o*o*o*o*o*o*



                makeMix(pathMp3 + "\\", pathMp3_2, pathMp4, filenameMOD);
            }
            catch (Exception e){ result = false; }; 
        }
        private void makeMix(string pathWavFolder, string pathWAV, string pathMp4, string filenameMOD)
        {
            try
            {
                string pathPRJCT = gparent.envdir;
                
               
                string pathMixTemp = pathWavFolder + "JACKRABBIT_mixtmp.mp3";

                /*               
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
                */

                
                
                
               // TimeSpan TIMEMIX = new TimeSpan(9, 30, 0);

                // TIMEMIX = new TimeSpan(8, 0, 0);




                // bool isMixOk = Concatenate_v2(pathMixTemp, pathMixTemp, pathWAV, pathWavFolder, TIMEMIX); //(pathMixTemp, pathWAV, TIMEMIX);  //

                long sizelimitfile = (long)692954000; //octets   (8go)  1000 *       //* 1000 * 50 * 1 * 1000  *  8

                //if (gparent.isWritingVIDEO != true)
                //{

                if(gparent.isRenamingMixFile != true) { 

                    bool mixready = concatMP3(pathMixTemp, pathWAV, sizelimitfile);
                
                    if (mixready)
                    {
                        gparent.isRenamingMixFile = true;
                        DateTime now = DateTime.Now;
                        string filenameMIXREADY = "JACKRABBIT_mix_" +
                            now.ToString("yyyy-MM-dd-HH-mm-ss")
                            + ".mp3";
                        string filenameMIXREADYVIDEOwithoutExtent = "JACKRABBIT_mix_" +
                                               now.ToString("yyyy-MM-dd-HH-mm-ss");
                                     

                        System.IO.File.Move(pathMixTemp, pathWavFolder + filenameMIXREADY);

                        gparent.isRenamingMixFile = false;
                        // if (gparent.isWritingVIDEO != true) { 

                        gparent.isWritingVIDEO = true;
                    
                        makeAVideo(pathWavFolder + filenameMIXREADY, pathMp4, filenameMIXREADYVIDEOwithoutExtent, pathPRJCT);
                        gparent.isWritingVIDEO = false;
                        //}
                    }
                }
                //}


                System.IO.File.Delete(pathWAV);

                //YOUTUBE(filenameMOD, pathFileMP4, pathPRJCT);
            }
            catch (Exception e){ gparent.isWritingOnMixFile = false; }
        }

        private bool concatMP3(string outputfilepath, string fp2, long sizelimitOctet)
        {
            string outputfilepathTEMP = outputfilepath + ".tmp";
          
            if (System.IO.File.Exists(outputfilepath))
            {
                if (System.IO.File.Exists(outputfilepathTEMP))
                    System.IO.File.Delete(outputfilepathTEMP);

                System.IO.File.Move(outputfilepath, outputfilepathTEMP);
            }

            bool ok = false;
            using (var fs = System.IO.File.OpenWrite(outputfilepath))
            {

                byte[] buffer = new byte[0];
                if(System.IO.File.Exists(outputfilepathTEMP))
                    buffer = System.IO.File.ReadAllBytes(outputfilepathTEMP);

                while (gparent.isWritingOnMixFile) ;

                gparent.isWritingOnMixFile = true;

                fs.Write(buffer, 0, buffer.Length);
                buffer = System.IO.File.ReadAllBytes(fp2);

                long size1 = buffer.Length + fs.Length;
                int delta1 = 0;
                if (size1 >= sizelimitOctet)
                {
                    delta1 = (int)(size1 - sizelimitOctet);
                    ok = true;
                }
                    

                fs.Write(buffer, 0, Math.Max(buffer.Length - delta1,0));
                fs.Flush();
                gparent.isWritingOnMixFile = false;

                if (fs.Length >= sizelimitOctet) ok = true;
            }
            System.IO.File.Delete(outputfilepathTEMP);
            return ok;
        }

        private void makeAVideo(string pathWAV, string pathMp4, string filenameMOD, string pathPRJCT)
        {
            string pathffmpeg = pathPRJCT + @"\_ffm\bin\";

            string pathFileMP4 = pathMp4 +
                    "\\" + filenameMOD + ".mkv";

            string pathFileMP4Guille = "\"" + pathFileMP4 + "\"";

            string args = "" +
                  "-loop 1 -r 4 -framerate 2 -i it1_560x340.png -i " +
                  "\"" + pathWAV + "\" " +
                  "-c:v libx264 -preset medium -tune stillimage -crf 18 -c:a copy -shortest -pix_fmt yuv420p " +
                   pathFileMP4Guille;

            ProcessStartInfo p = new ProcessStartInfo(pathffmpeg+"ffmpeg.exe");

            p.WorkingDirectory = pathffmpeg;

            p.RedirectStandardOutput = false; // true;
            p.Arguments = args;

            Process process = new Process();
            process.StartInfo = p;

            if (!Directory.Exists(pathMp4))
                Directory.CreateDirectory(pathMp4);

            process.Start();
            process.WaitForExit(60000*60*24);
            process.Close();
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

        private bool ConcatenateWavFilesV3__x(string destinationFile, string sourceFile, TimeSpan tspan)
        {
            bool isAfterTime = false;

            WaveFileWriter waveFileWriter = null;

            long sourceReadOffset = new System.IO.FileInfo(destinationFile).Length;
            //var sourceReadOffset = GetWaveFileSize(destinationFile);      previous

            try
            {
                using (var fs = System.IO.File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new WaveFileReader(fs))
                    {
                        waveFileWriter = new WaveFileWriter(destinationFile, reader.WaveFormat);
                        if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                        {
                            throw new InvalidOperationException(
                                "Can't append WAV Files that don't share the same format");
                        }

                        var startPos = sourceReadOffset - sourceReadOffset % reader.WaveFormat.BlockAlign;
                        var endPos = (int)reader.Length;
                        reader.Position = startPos;
                        var bytesRequired = (int)(endPos - reader.Position);
                        var buffer = new byte[bytesRequired];
                        if (bytesRequired > 0)
                        {
                            var bytesToRead = Math.Min(bytesRequired, buffer.Length);
                            var bytesRead = reader.Read(buffer, 0, bytesToRead);

                            if (bytesRead > 0)
                            {
                                waveFileWriter.Write(buffer, (int)startPos, bytesRead);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    isAfterTime = (waveFileWriter.TotalTime >= tspan);

                    waveFileWriter.Dispose();
                }
            }

            return isAfterTime;
        }

        public static bool Concatenate_v2(string outputFile, string pathMixTemp, string pathWAV, string pathWavFolder, TimeSpan tspan)
         {
            byte[] buffer1 = new byte[1024];
            byte[] buffer2 = new byte[1024];

            string indFileTemp = ".tmp";

            long read1 = 0;
            long read2 = 0;

            WaveFileWriter waveFileMIXWriter = null;
            WaveFileReader waveFileMIXreader = null;
            WaveFileReader trackreader = null;

            bool isAfterTime = false;
            try
            {
                try {
                    if (System.IO.File.Exists(pathMixTemp))
                    {
                        System.IO.File.Delete(pathMixTemp + indFileTemp);
                        System.IO.File.Move(pathMixTemp, pathMixTemp + indFileTemp);
                        
                        waveFileMIXreader = new WaveFileReader(pathMixTemp + indFileTemp);
                    }
                   

                }
                catch
                {
                    waveFileMIXreader = null;
                }


                // lire nouvelle musique 
                trackreader = new WaveFileReader(pathWAV);

                waveFileMIXWriter = new WaveFileWriter(pathMixTemp, trackreader.WaveFormat );



                /*

                // normalize DB
                float max = 0;
                int read;
                int countBlocks = 0;
                byte[] buffertst1 = new byte[1024];
                do
                {
                    read = trackreader.Read(buffertst1, 0, buffertst1.Length);
                    for (int n = 0; n < read; n++)
                    {
                        var abs = Math.Abs(buffertst1[n]);
                        if (abs > max) max = abs;

                        buffertst1[n]
                    }
                    countBlocks++;
                } while (read > 0);

                float delta = Math.Max(255 - max, 0);

                int countBlocks2 = 0;
                do
                {
                    for (int n = 0; n < buffertst1.Length; n++)
                    {

                        int orig = Convert.ToInt32(buffertst1[n]);
                        int final = orig + (int)delta;
                        final = Math.Min(final, 255);
                        final = Math.Max(final, 0);
                        byte deltabyte = Convert.ToByte(final);

                        buffertst1[n] = deltabyte;

                           
                    }
                    countBlocks2++;
                } while (countBlocks2 < countBlocks);
                */
                //waveFileMIXWriter.Write/*Data*/(buffertst1, 0, countBlocks);
                //waveFileMIXWriter.Write/*Data*/(buffertst1);


                //{

                // clone
                /* byte[] bytesAr = new byte[2048];
                 for (int i = 0; i < buffer1.Length; i++)
                     bytesAr[i] = buffer1[i];
                 // add
                 listofBuffersMix.Add(bytesAr);*/
                // }

                /*if (waveFileMIXreader != null) {
                    waveFileMIXWriter.Position = 0; //waveFileMIXreader.Position;
                    waveFileMIXWriter.Write(waveFileMIXreader.re, 0, (int)read1);
                }
                */

                if (waveFileMIXreader != null)
                    while ((read1 = waveFileMIXreader.Read(buffer1, 0, buffer1.Length)) > 0)
                        waveFileMIXWriter.Write(buffer1, 0, (int)read1);
                        

                while ((read2 = trackreader.Read(buffer2, 0, buffer2.Length)) > 0)
                    waveFileMIXWriter.Write(buffer2, 0, (int)read2);

               
                //for(int i = 0; i < read1; i++)
                //{

                // }
                //////// while ((read1 = trackreader.Read(buffer1, 0, buffer1.Length)) > 0) ;


                isAfterTime = (waveFileMIXWriter.TotalTime >= tspan);

                trackreader.Dispose();
                waveFileMIXWriter.Dispose();

                if (waveFileMIXreader != null)
                    waveFileMIXreader.Dispose();

                /* while ((read = trackreader.Read(buffer, 0, buffer.Length)) > 0)
                     trackreader.Write(buffer, 0, read);
                */


             }
            catch(Exception e) {
                try
                {
                    if (trackreader != null)
                        trackreader.Dispose();

                    if (waveFileMIXWriter != null)
                        waveFileMIXWriter.Dispose();

                    if (waveFileMIXreader != null)
                        waveFileMIXreader.Dispose();
                }
                catch { 
                }
            }

            try { 
                System.IO.File.Delete(pathWAV);
            }
            catch { }

            return isAfterTime;
        }

        public static void Concatenate_old1(string outputFile, List<string> sourceFiles, string pathWavFolder)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;
            WaveFileReader reader = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    if (reader != null) reader.Dispose();

                    reader = null;
                    if (sourceFile == outputFile) {
                        
                        try { 
                            System.IO.File.Delete(outputFile + "_tmp.wav");
                        } catch { }
                        System.IO.File.Copy(outputFile, outputFile + "_tmp.wav");
                        try
                        {
                            reader = new WaveFileReader(sourceFile + "_tmp.wav");
                        }
                        catch {
                            reader = null;
                        }
                    }
                    else
                        reader = new WaveFileReader(sourceFile);

                    if (reader != null) { 

                        if (waveFileWriter == null)    
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);             
                        else
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))                     
                                throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");      

                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                            waveFileWriter.Write/*Data*/(buffer, 0, read);

                        if (sourceFile == outputFile)
                            System.IO.File.Delete(outputFile + "_tmp.wav");


                        //  time > 9h30

                        TimeSpan total_tspn = new TimeSpan(9, 30, 0);
                        TimeSpan cur_tspn = waveFileWriter.TotalTime;

                        if (cur_tspn > total_tspn)
                        {
                            DateTime now = DateTime.Now;
                            string filename = "JACKRABBIT_mix_" +
                                now.ToString("yyyy-MM-dd-HH-mm-ss")
                                + ".wav";

                            System.IO.File.Move(outputFile, pathWavFolder + filename);
                        }

                    }
                }
            }
            catch(Exception e) { }

            if (waveFileWriter != null)
            {
                waveFileWriter.Dispose();
            }
            

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
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
// Author: Nathaniel Shah

namespace filesearch
{   
    class Program{
        static string help = "Usage: du[-s][-d][-b] < path >\r\n" +
            "Summarize disk usage of the set of FILES, recursively for directories.\r\n\n" +
            "You MUST specify one of the parameters, -s, -d, or -b\r\n" +
            "-s\tRun in single threaded mode\r\n" +
            "-d\tRun in parallel mode (uses all available processors)\r\n" +
            "-b\tRun in both parallel and single threaded mode.\r\n" +
            "\tRuns parallel followed by sequential mode\n";
        static string[] imageExtensions = new string[] { ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".svg", ".webp", ".bmp", ".ico", ".cur", ".tif", ".tiff"};
        static void Main(string[] args)
        {
            bool exists;
            var use = "";
            var path = "";
            try
            {
                use = args[0];
                path = args[1]; 
                exists = Directory.Exists(path);
            }
            catch (Exception)
            {
                printHelp();
                return;
            }
            if(use != "-s" &  use != "-d" &  use != "-b" | !exists)
            {
                printHelp();
                return;
            }
            if(use == "-s")
            {
                syncStart(path);
            }
            else if(use == "-d")
            {
                parallelSearch(path);
            }
            else
            {
                parallelSearch(path);
                syncStart(path);
            }
        }
        static private void printHelp()
    {
        Console.WriteLine(help);
    }
        static private void syncStart(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long imgCount = 0;
            long fileSize = 0;
            long fileCount = 0;
            long imgSize = 0;
            long foldercount = 0;
            var stats = syncSearch(path, imgSize, imgCount, fileCount, fileSize, foldercount);
            imgSize = stats[0];
            imgCount = stats[1];
            fileCount = stats[2];
            fileSize = stats[3];
            foldercount = stats[4];
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = "" + ts.Seconds + "." + ts.Milliseconds;
            Console.WriteLine("Sequential Calculated in : " + elapsedTime);
            Console.WriteLine(foldercount + " folders, " + fileCount + " files, " + fileSize + " bytes");
            Console.WriteLine(imgCount + " image files, " + imgSize + " bytes");
        }
        static private long[] syncSearch(string path, long imgSize, long imgCount, long fileCount, long fileSize, long foldercount)
        {
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                foldercount++;
                var stats = syncSearch(folder, imgSize, imgCount, fileCount, fileSize, foldercount);
                imgSize = stats[0];
                imgCount = stats[1];
                fileCount = stats[2];
                fileSize = stats[3];
                foldercount = stats[4];
            }
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                try
                {
                    var extension = Path.GetExtension(file);
                    if (imageExtensions.Contains(extension))
                    {
                        imgCount++;
                        var stats = new FileInfo(file);
                        imgSize += stats.Length;
                    }
                    else
                    {
                        fileCount++;
                        var stats = new FileInfo(file);
                        fileSize += stats.Length;
                    }
                }
                catch(Exception) {
                    continue;
                }
            }
            return new long[] {imgSize, imgCount, fileCount, fileSize, foldercount};
        }
        static private void parallelSearch(string path)
        {
            //
        }
    }
}
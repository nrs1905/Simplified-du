using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
// Author: Nathaniel Shah

namespace filesearch
{   
    class Program{
        //help is the help message to print whenever there is an invalid argument
        static string help = "Usage: du[-s][-d][-b] < path >\r\n" +
            "Summarize disk usage of the set of FILES, recursively for directories.\r\n\n" +
            "You MUST specify one of the parameters, -s, -d, or -b\r\n" +
            "-s\tRun in single threaded mode\r\n" +
            "-d\tRun in parallel mode (uses all available processors)\r\n" +
            "-b\tRun in both parallel and single threaded mode.\r\n" +
            "\tRuns parallel followed by sequential mode\n";
        //imageExtensions is a list of all known extensions image files can end with
        static string[] imageExtensions = new string[] { ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".svg", ".webp", ".bmp", ".ico", ".cur", ".tif", ".tiff"};
        static long icount = 0; //image count
        static long isize = 0; //image size
        static long fcount = 0; //file count
        static long fsize = 0; //file size
        static long fdcount = 0; //folder count
        static void Main(string[] args)
        {
            bool exists; //whether the provided directory exists
            var use = ""; //instantializes use flag
            var path = ""; //instantializes path 
            try
            {
                use = args[0];
                path = args[1]; 
                exists = Directory.Exists(path); //Check if the directory exists
            }
            //If there is any issue with accessing the path, print the help message then exit gracefully
            catch (Exception)
            {
                printHelp();
                return;
            }
            //If the provided flag is not recognized, or the path does not exist, print the help message then exit gracefully
            if(use != "-s" &  use != "-d" &  use != "-b" | !exists)
            {
                printHelp();
                return;
            }
            Console.WriteLine("Directory '" + path + "':\n");
            //If the sequential flag is given, run a sequential search
            if (use == "-s")
            {
                syncStart(path);
            }
            //If the parallel flag is given, run a parallel search
            else if(use == "-d")
            {
                parallelStart(path);
            }
            //If the both flag is given, run a parallel search then a sequential search
            else
            {
                parallelStart(path);
                syncStart(path);
            }
        }
        /*
         * printHelp
         * Prints the help message to the console
         */
        static private void printHelp()
        {
            Console.WriteLine(help);
        }
        /*
         * syncStart
         * Starts the sequential search by creating the necessary variables then making the first call to the sequential search method syncSearch,
         * then prints out the information gathered by the search
         * Args:    path    the path of the directory to search
         */
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
            String elapsedTime = ts.TotalSeconds + "s";
            Console.WriteLine("Sequential Calculated in: " + elapsedTime);
            Console.WriteLine(foldercount.ToString("n0") + " folders, " + fileCount.ToString("n0") + " files, " + fileSize.ToString("n0") + " bytes");
            if (imgCount > 0)
            {
                Console.WriteLine(imgCount.ToString("n0") + " image files, " + imgSize.ToString("n0") + " bytes\n");
            }
            else
            {
                Console.WriteLine("No image files found in the directory\n");
            }
        }
        /*
         * syncSearch
         * a sequential method that recursively searches through every directory in a parent directory then counts all the files and adds up their sizes, 
         * before returning that information
         * Args:    path        the directory to search
         *          imgSize     the current size of the images
         *          imgCount    the current count of the images
         *          fileCount   the current count of the files
         *          fileSize    the current size of the files
         *          foldercount the current count of the folders
         * Return:  a long array consisting of the imgSize, imgcount, fileCount, fileSize and foldercount
         */
        static private long[] syncSearch(string path, long imgSize, long imgCount, long fileCount, long fileSize, long foldercount)
        {
            string[] folders = Directory.GetDirectories(path);
            // Recursively go throughs all subdirectories, and adds their file count to the current count
            foreach (string folder in folders)
            {
                try
                {
                    var stats = syncSearch(folder, imgSize, imgCount, fileCount, fileSize, foldercount);
                    imgSize = stats[0];
                    imgCount = stats[1];
                    fileCount = stats[2];
                    fileSize = stats[3];
                    foldercount = stats[4];
                    foldercount++;
                }
                catch {
                    continue;
                }
            }
            // Gets an array of the names of all of the files contained within the directory
            string[] files = Directory.GetFiles(path); 
            // For every file check if it is an image, then add its size to the respective count
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
        /*
         * parallelStart
         * starts the parallel search, but mostly prints out the information acquired from the search
         */
        static private void parallelStart(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            parallelSearch(path);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            String elapsedTime = ts.TotalSeconds + "s";
            Console.WriteLine("Parallel Calculated in: " + elapsedTime);
            Console.WriteLine(fdcount.ToString("n0") + " folders, " + fcount.ToString("n0") + " files, " + fsize.ToString("n0") + " bytes");
            if (icount > 0)
            {
                Console.WriteLine(icount.ToString("n0") + " image files, " + isize.ToString("n0") + " bytes\n");
            }
            else
            {
                Console.WriteLine("No image files found in the directory\n");
            }
        }
        /*
         * parallelSearch
         * A parallel thread based method that recursively searches through every directory in a parent directory then counts all files and their size, along with the amount of folders
         * the information is then updated through the class variables isize, icount, fcount, fsize and fdcount
         */
        static private void parallelSearch(string path)
        {
            string[] folders = Directory.GetDirectories(path);
            Parallel.ForEach(folders, folder =>
            {
                try
                {
                    parallelSearch(folder);
                    Interlocked.Increment(ref fdcount);
                }
                catch
                {
                    //Swallow
                }
            }
            );
            string[] files = Directory.GetFiles(path);
            Parallel.ForEach(files, file =>
            {
                try
                {
                    var extension = Path.GetExtension(file);
                    if (imageExtensions.Contains(extension))
                    {
                        Interlocked.Increment(ref icount);
                        var stats = new FileInfo(file);
                        Interlocked.Add(ref isize, stats.Length);
                    }
                    else
                    {
                        Interlocked.Increment(ref fcount);
                        var stats = new FileInfo(file);
                        Interlocked.Add(ref fsize, stats.Length);
                    }
                }
                catch (Exception)
                {
                    //Swallow
                }
            }
            );
        }
    }
}
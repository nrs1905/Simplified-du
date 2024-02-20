using System;
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
        static void Main(string[] args)
        {
            var use = "";
            var path = "";
            try
            {
                use = args[0];
                path = args[1];
            }
            catch (Exception)
            {
                printHelp();
                return;
            }
            if(use != "-s" &  use != "-d" &  use != "-b")
            {
                printHelp();
                return;
            }
            if(use == "-s")
            {
                syncSearch(path);
            }
            else if(use == "-d")
            {
                parallelSearch(path);
            }
            else
            {
                parallelSearch(path);
                syncSearch(path);
            }
        }
        static private void printHelp()
    {
        Console.WriteLine(help);
    }
        static private void syncSearch(string path)
        {
            //
        }
        static private void parallelSearch(string path)
        {
            //
        }
    }
}
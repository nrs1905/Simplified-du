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
            Console.WriteLine("Reached" + path);
        }
        static public void printHelp()
    {
        Console.WriteLine(help);
    }
    }
}
/*
Usage: du[-s][-d][-b] < path >
Summarize disk usage of the set of FILES, recursively for directories.
You MUST specify one of the parameters, -s, -d, or -b
-s Run in single threaded mode
-d Run in parallel mode (uses all available processors)
-b Run in both parallel and single threaded mode.
Runs parallel followed by sequential mode
*/
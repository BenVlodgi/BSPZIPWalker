using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BSPZIPWalker
{
    class Program
    {
        static List<string> output;

        static int Main(string[] args)
        {
            string currentDir = string.Empty;
            string outputFile = string.Empty;
            bool appendToFile = false;

            if (args.Length > 0)
                currentDir = args[0];
#if DEBUG
            else
                currentDir = Environment.CurrentDirectory;
#endif

            if (args.Length > 1)
                outputFile = args[1];
#if DEBUG
            else
                outputFile = "DirectoryWalker.out.txt";
#endif

            if (args.Length > 2)
                appendToFile = args[2].StartsWith("-A") || args[2].StartsWith("-a");

            if (currentDir == string.Empty || outputFile == string.Empty)
            {
                Console.WriteLine("Need more parameters:" + Environment.NewLine
                    + "para[0] = Current Directory" + Environment.NewLine
                    + "para[1] = Output File" + Environment.NewLine
                    + "para[2]?= -A | (Optional Flag) If set, will append instead of overwrite."
                    );
                return -1;
            }

            if (!Directory.Exists(currentDir))
            {
                Console.WriteLine("Directory doesn't exist.");
                return -1;
            }

            // Ensure we've got the fully qualified path
            currentDir = new DirectoryInfo(currentDir).FullName;

            // Ensure root dir ends with '\' for replacement later
            string rootDir = currentDir;
            if (!rootDir.EndsWith(@"\"))
                rootDir += @"\";

            output = new List<string>();
            if (ProcessDirectory(rootDir, currentDir) != 0)
                return -1;

            // Write the final output
            if(appendToFile)
                File.AppendAllLines(outputFile, output);
            else
                File.WriteAllLines(outputFile, output);

            return 0;

        }

        static int ProcessDirectory(string root, string currentDir)
        {
            try
            {
                var files = Directory.GetFiles(currentDir);
                var directories = Directory.GetDirectories(currentDir);

                foreach (var file in files)
                {
                    string fullName = new FileInfo(file).FullName;
                    string relativeName = fullName.Replace(root, string.Empty);

                    output.Add(relativeName);
                    output.Add(fullName);
                }

                foreach (var directory in directories)
                    if (ProcessDirectory(root, new DirectoryInfo(directory).FullName) != 0)
                        return -1;
            }
            catch (Exception ex)
            {
                File.AppendAllText("BSPZIPWalker.error.txt", ex.ToString() + Environment.NewLine);
                Console.WriteLine("Exception in BSPZIPWalker:ProcessDirectory" + Environment.NewLine + ex.ToString());

                return -1;
            }

            return 0;
        }
    }
}

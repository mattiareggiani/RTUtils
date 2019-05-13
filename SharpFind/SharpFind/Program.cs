using System;
using System.Collections.Generic;
using System.IO;

namespace SharpFind
{
    class Program
    {
        static string logFile = "";
        static IDictionary<string, bool> notification;
        static void Main(string[] args)
        {
            notification = new Dictionary<string, bool>();
            if (args.Length < 4)
            {
                usage();
                return;
            }else if(args.Length == 5)
            {
                logFile = args[4];
            }

            if (args[0] == "name")
            {
                if(args[1] == "LocalShares")
                {
                    var localShares = Share.LocalShares();
                    if (localShares != null)
                    {
                        foreach (var share in localShares)
                        {
                            findFile(share, args[2], args[3]);
                        }
                    }
                }
                else if(args[1] == "LogicalDisk")
                {
                    var logicalDisks = Share.LogicalDisk();
                    if (logicalDisks != null)
                    {
                        foreach (var disk in logicalDisks)
                        {
                            findFile(disk, args[2], args[3]);
                        }
                    }
                }
                else
                {
                    findFile(args[1], args[2], args[3]);
                }
            }
            if(args[0] == "keyword")
            {
                if (args[1] == "LocalShares")
                {
                    var localShares = Share.LocalShares();
                    if (localShares != null)
                    {
                        foreach (var share in localShares)
                        {
                            findKeyword(share, args[2], args[3]);
                        }
                    }
                }
                else if (args[1] == "LogicalDisk")
                {
                    var logicalDisks = Share.LogicalDisk();
                    if (logicalDisks != null)
                    {
                        foreach (var disk in logicalDisks)
                        {
                            findKeyword(disk, args[2], args[3]);
                        }
                    }
                }
                else
                {
                    findKeyword(args[1], args[2], args[3]);
                }
            }
        }
        private static void usage()
        {
            Console.WriteLine("For searching by name: SharpFind.exe name {directory} {name} {extension} [outputFile]");
            Console.WriteLine("For searching by keyword: SharpFind.exe keyword {directory} {keyword} {extension} [outputFile] \n");
            Console.WriteLine("- directory: C:\\path\\to\\directory\\ or \\\\path\\to\\directory\\ or LocalShares (to search into all local network computers [net view]) or LogicalDisk (to search into all logical disks)");
            Console.WriteLine("- name and keyword: case insensitive");
            Console.WriteLine("- extension: *.* or *.txt\n");
        }
        private static void log(string content)
        {
            try
            {
                File.AppendAllText(logFile, content + Environment.NewLine);
            }
            catch
            {
                //Console.WriteLine("Error writing file");
            }
        }
        private static void findKeyword(string directory, string keyword , string extension)
        {
            IEnumerable<string> filePaths = GetFiles(directory, extension);
            foreach (string file in filePaths)
            {
                if (Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    if(Document.ParsePdf(file, keyword))
                    {
                        if (!notification.ContainsKey(file))
                        {
                            Console.WriteLine(String.Format("[+] {0}", file));
                            if (logFile != "") log(file);
                            notification.Add(file, true);
                        }
                        
                    }
                }
                else if (Path.GetExtension(file).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    if (Document.ParseDocx(file, keyword))
                    {
                        if (!notification.ContainsKey(file))
                        {
                            Console.WriteLine(String.Format("[+] {0}", file));
                            if (logFile != "") log(file);
                            notification.Add(file, true);
                        }
                    }
                }
                else if (Path.GetExtension(file).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    if(Document.ParseXslx(file, keyword))
                    {
                        if (!notification.ContainsKey(file))
                        {
                            Console.WriteLine(String.Format("[+] {0}", file));
                            if (logFile != "") log(file);
                            notification.Add(file, true);
                        }
                    }
                }
                else
                {
                    using (var streamReader = new StreamReader(file))
                    {
                        var contents = streamReader.ReadToEnd().ToLower();
                        if (contents.Contains(keyword))
                        {
                            if (!notification.ContainsKey(file))
                            {
                                Console.WriteLine(String.Format("[+] {0}", file));
                                if (logFile != "") log(file);
                                notification.Add(file, true);
                            }
                        }
                    }
                }
            }
        }
        private static void findFile(string directory, string filename, string extension)
        {
            IEnumerable<string> filePaths = GetFiles(directory, extension);
            foreach (string file in filePaths)
            {
                if(filename.Equals(Path.GetFileName(file), StringComparison.OrdinalIgnoreCase) || filename.Equals(Path.GetFileNameWithoutExtension(file), StringComparison.OrdinalIgnoreCase))
                {
                    if (!notification.ContainsKey(file))
                    {
                        Console.WriteLine(String.Format("[+] {0}", file));
                        if (logFile != "") log(file);
                        notification.Add(file, true);
                    }
                }
                
            }
        }
        private static IEnumerable<string> GetFiles(string directory, string extension)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(directory);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, extension, SearchOption.AllDirectories);
                }
                catch { }
                if (next != null && next.Length != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }
    }
}

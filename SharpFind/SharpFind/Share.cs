using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpFind
{
    class Share
    {
        public static IEnumerable<string> LocalShares()
        {
            NetworkBrowser networkBrowser = new NetworkBrowser();
            List<string> localShares = new List<string>();
            try
            {
                Console.WriteLine("[*] Getting Shares...");
                localShares = networkBrowser.getNetworkComputers();
            }
            catch
            {
                return null;
            }           
            return localShares;
        }
        public static IEnumerable<string> LogicalDisk()
        {
            List<string> logicalDisks = new List<string>();
            try
            {
                var ManagementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_MappedLogicalDisk");
                foreach (ManagementObject queryObj in ManagementObjectSearcher.Get())
                {
                    logicalDisks.Add(queryObj["ProviderName"].ToString());
                }
            }
            catch
            {
                return null;
            }
            return logicalDisks;
        }
    }
    public sealed class NetworkBrowser
    {
        // Class borrowed from https://www.codeproject.com/Articles/16113/Retreiving-a-list-of-network-computer-names-using

        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true),
        SuppressUnmanagedCodeSecurityAttribute]

        public static extern int NetServerEnum(
            string ServerNane,
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, 
            out int dwResumeHandle
            );
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int NetShareEnum(
         StringBuilder ServerName,
         int level,
         ref IntPtr bufPtr,
         int prefmaxlen,
         ref int entriesread,
         ref int totalentries,
         ref int resume_handle
         );
        [DllImport("Netapi32", SetLastError = true),
        SuppressUnmanagedCodeSecurityAttribute]

        public static extern int NetApiBufferFree(
            IntPtr pBuf);

        [StructLayout(LayoutKind.Sequential)]
        public struct _SERVER_INFO_100
        {
            internal int sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string sv100_name;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHARE_INFO_1
        {
            public string shi1_netname;
            public uint shi1_type;
            public string shi1_remark;
            public SHARE_INFO_1(string sharename, uint sharetype, string remark)
            {
                this.shi1_netname = sharename;
                this.shi1_type = sharetype;
                this.shi1_remark = remark;
            }
            public override string ToString()
            {
                return shi1_netname;
            }
        }
        const uint MAX_PREFERRED_LENGTH = 0xFFFFFFFF;
        const int NERR_Success = 0;
        private enum NetError : uint
        {
            NERR_Success = 0,
            NERR_BASE = 2100,
            NERR_UnknownDevDir = (NERR_BASE + 16),
            NERR_DuplicateShare = (NERR_BASE + 18),
            NERR_BufTooSmall = (NERR_BASE + 23),
        }
        private enum SHARE_TYPE : uint
        {
            STYPE_DISKTREE = 0,
            STYPE_PRINTQ = 1,
            STYPE_DEVICE = 2,
            STYPE_IPC = 3,
            STYPE_SPECIAL = 0x80000000,
        }
        public NetworkBrowser()
        {

        }

        public List<string> getNetworkComputers()
        {
            List<string> networkComputers = new List<string>();
            const int MAX_PREFERRED_LENGTH = -1;
            int SV_TYPE_WORKSTATION = 1;
            int SV_TYPE_SERVER = 2;
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;
            int sizeofINFO = Marshal.SizeOf(typeof(_SERVER_INFO_100));


            try
            {
                int ret = NetServerEnum(null, 100, ref buffer, MAX_PREFERRED_LENGTH,
                    out entriesRead,
                    out totalEntries, SV_TYPE_WORKSTATION | SV_TYPE_SERVER, null, out
                    resHandle);
                if (ret == 0)
                {
                    for (int i = 0; i < totalEntries; i++)
                    {
                        tmpBuffer = new IntPtr((int)buffer + (i * sizeofINFO));
                        _SERVER_INFO_100 svrInfo = (_SERVER_INFO_100)
                            Marshal.PtrToStructure(tmpBuffer, typeof(_SERVER_INFO_100));
                        if(svrInfo.sv100_name == Environment.MachineName)
                        {
                            continue;
                        }
                        int entriesread = 0;
                        int totalentries = 0;
                        int resume_handle = 0;
                        IntPtr nStructSize = new IntPtr(Marshal.SizeOf(typeof(SHARE_INFO_1)));
                        IntPtr bufPtr = IntPtr.Zero;
                        StringBuilder server = new StringBuilder(svrInfo.sv100_name);
                        int ret2 = NetShareEnum(server, 1, ref bufPtr, MAX_PREFERRED_LENGTH, ref entriesread, ref totalentries, ref resume_handle);
                        if (ret2 == NERR_Success)
                        {
                            IntPtr currentPtr = bufPtr;

                            for (int ii = 0; ii < entriesread; ii++)
                            {
                                SHARE_INFO_1 shi1 = (SHARE_INFO_1)Marshal.PtrToStructure(currentPtr, typeof(SHARE_INFO_1));
                                Console.WriteLine(String.Format("\\\\{0}\\{1}", svrInfo.sv100_name, shi1.shi1_netname));
                                networkComputers.Add(String.Format("\\\\{0}\\{1}", svrInfo.sv100_name, shi1.shi1_netname));
                                currentPtr = new IntPtr(currentPtr.ToInt64() + nStructSize.ToInt64());
                            }
                            NetApiBufferFree(bufPtr);

                        }
                        else
                        {
                            

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem with acessing network computers in NetworkBrowser " +
                    "\r\n\r\n\r\n" + ex.Message,
                    "Error");
                
            }
            finally
            {
                NetApiBufferFree(buffer);
            }
            return networkComputers;
        }

    }
}

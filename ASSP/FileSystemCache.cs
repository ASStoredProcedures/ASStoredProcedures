/*============================================================================
  File:    FileSystemCache.cs

  Summary: Exposes the Windows System File Cache usage and allows it to be
           cleared.

  Date:    February 23, 2009

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/Wiki/View.aspx?ProjectName=ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/
using System;
using System.Runtime.InteropServices;
using Microsoft.AnalysisServices.AdomdServer;

namespace AsStoredProcs
{
    public class FileSystemCache
    {
        public static void ClearFileSystemCache()
        {
            SetIncreaseQuotePrivilege();

            int iReturn;
            if (!Is64BitMode())
            {
                SYSTEM_CACHE_INFORMATION info = new SYSTEM_CACHE_INFORMATION();
                info.MinimumWorkingSet = uint.MaxValue; //means to clear the cache
                info.MaximumWorkingSet = uint.MaxValue; //means to clear the cache
                int iSize = Marshal.SizeOf(info);

                GCHandle gch = GCHandle.Alloc(info, GCHandleType.Pinned);
                iReturn = NtSetSystemInformation(SYSTEMCACHEINFORMATION, gch.AddrOfPinnedObject(), iSize);
                gch.Free();
            }
            else
            {
                SYSTEM_CACHE_INFORMATION_64_BIT info = new SYSTEM_CACHE_INFORMATION_64_BIT();
                info.MinimumWorkingSet = -1; //means to clear the cache
                info.MaximumWorkingSet = -1; //means to clear the cache
                int iSize = Marshal.SizeOf(info);

                GCHandle gch = GCHandle.Alloc(info, GCHandleType.Pinned);
                iReturn = NtSetSystemInformation(SYSTEMCACHEINFORMATION, gch.AddrOfPinnedObject(), iSize);
                gch.Free();
            }

            if (iReturn != 0)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static System.Data.DataTable GetFileSystemCacheBytes()
        {
            System.Data.DataTable tableReturn = new System.Data.DataTable();
            tableReturn.Columns.Add("Cache Bytes", typeof(long));
            tableReturn.Columns.Add("Cache GB", typeof(decimal));
            tableReturn.Columns.Add("Cache Bytes Peak", typeof(long));
            tableReturn.Columns.Add("Cache GB Peak", typeof(decimal));

            System.Diagnostics.PerformanceCounter pcCacheBytes = new System.Diagnostics.PerformanceCounter("Memory", "Cache Bytes", true);
            System.Diagnostics.PerformanceCounter pcCacheBytesPeak = new System.Diagnostics.PerformanceCounter("Memory", "Cache Bytes Peak", true);

            float fltCacheBytes = pcCacheBytes.NextValue();
            float fltCacheBytesPeak = pcCacheBytes.NextValue();
            pcCacheBytes.Close();
            pcCacheBytesPeak.Close();

            object[] row = new object[] { 
                (long)fltCacheBytes, 
                decimal.Round((decimal)(fltCacheBytes / 1024 / 1024 / 1024), 2), 
                (long)fltCacheBytesPeak, 
                decimal.Round((decimal)(fltCacheBytesPeak / 1024 / 1024 / 1024), 2) 
            };
            
            tableReturn.Rows.Add(row);
            return tableReturn;
        }

        #region Windows APIs
        public static bool Is64BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        [DllImport("NTDLL.dll", SetLastError = true)]
        internal static extern int NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInfo, int SystemInfoLength);

        private static int SYSTEMCACHEINFORMATION = 0x15;

#pragma warning disable 649 //disable the "is never assigned to" warning
        private struct SYSTEM_CACHE_INFORMATION
        {
            public uint CurrentSize;
            public uint PeakSize;
            public uint PageFaultCount;
            public uint MinimumWorkingSet;
            public uint MaximumWorkingSet;
            public uint Unused1;
            public uint Unused2;
            public uint Unused3;
            public uint Unused4;
        }

        private struct SYSTEM_CACHE_INFORMATION_64_BIT
        {
            public long CurrentSize;
            public long PeakSize;
            public long PageFaultCount;
            public long MinimumWorkingSet;
            public long MaximumWorkingSet;
            public long Unused1;
            public long Unused2;
            public long Unused3;
            public long Unused4;
        }
#pragma warning restore 649 //reenable the warning

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_QUERY = 0x00000008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";

        private static bool SetIncreaseQuotePrivilege()
        {
            try
            {
                bool retVal;
                TokPriv1Luid tp;
                IntPtr hproc = GetCurrentProcess();
                IntPtr htok = IntPtr.Zero;
                retVal = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
                tp.Count = 1;
                tp.Luid = 0;
                tp.Attr = SE_PRIVILEGE_ENABLED;
                retVal = LookupPrivilegeValue(null, SE_INCREASE_QUOTA_NAME, ref tp.Luid);
                retVal = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}

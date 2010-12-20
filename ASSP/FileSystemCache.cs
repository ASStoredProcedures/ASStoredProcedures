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
using System.Security.Principal;

namespace ASStoredProcs
{
    public class FileSystemCache
    {
        #region ClearFileSystemCache
        public static void ClearFileSystemCache()
        {
            ClearFileSystemCache(true); //clear the standby cache by default since this must be done to really test cold cache performance
        }

        public static void ClearFileSystemCache(bool ClearStandbyCache)
        {
            SetIncreasePrivilege(SE_INCREASE_QUOTA_NAME);

            //clear the active file system cache
            int iReturn;
            if (!Is64BitMode())
            {
                SYSTEM_CACHE_INFORMATION info = new SYSTEM_CACHE_INFORMATION();
                info.MinimumWorkingSet = uint.MaxValue; //means to clear the active file system cache
                info.MaximumWorkingSet = uint.MaxValue; //means to clear the active file system cache
                int iSize = Marshal.SizeOf(info);

                GCHandle gch = GCHandle.Alloc(info, GCHandleType.Pinned);
                iReturn = NtSetSystemInformation(SYSTEMCACHEINFORMATION, gch.AddrOfPinnedObject(), iSize);
                gch.Free();
            }
            else
            {
                SYSTEM_CACHE_INFORMATION_64_BIT info = new SYSTEM_CACHE_INFORMATION_64_BIT();
                info.MinimumWorkingSet = -1; //means to clear the active file system cache
                info.MaximumWorkingSet = -1; //means to clear the active file system cache
                int iSize = Marshal.SizeOf(info);

                GCHandle gch = GCHandle.Alloc(info, GCHandleType.Pinned);
                iReturn = NtSetSystemInformation(SYSTEMCACHEINFORMATION, gch.AddrOfPinnedObject(), iSize);
                gch.Free();
            }

            if (iReturn != 0)
            {
                throw new Exception("NtSetSystemInformation(SYSTEMCACHEINFORMATION) error: ", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
            }

            if (ClearStandbyCache)
            {
                try
                {
                    SetIncreasePrivilege(SE_PROFILE_SINGLE_PROCESS_NAME);


                    int iSize = Marshal.SizeOf(ClearStandbyPageList);
                    GCHandle gch = GCHandle.Alloc(ClearStandbyPageList, GCHandleType.Pinned);
                    iReturn = NtSetSystemInformation(SYSTEMMEMORYLISTINFORMATION, gch.AddrOfPinnedObject(), iSize);
                    gch.Free();

                    if (iReturn != 0)
                    {
                        throw new Exception("NtSetSystemInformation(SYSTEMMEMORYLISTINFORMATION) error: ", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
                    }
                }
                catch (Exception ex)
                {
                    Context.TraceEvent(1, 101, "Problem clearing standby cache with API call: " + ex.Message);

                    //this is a fallback if the API call doesn't work on an older OS
                    ClearStandbyFileSystemCacheByConsumingAvailableMemory();
                }
            }
        }
        #endregion

        #region ClearAllCaches
        /// <summary>
        /// Clear both the Analysis Services cache and the active+standby system file cache in one sproc call
        /// </summary>
        public static void ClearAllCaches()
        {
            XmlaDiscover xmla = new XmlaDiscover();
            xmla.ClearCache();

            ClearFileSystemCache(true);
        }
        #endregion

        #region ClearStandbyFileSystemCacheByConsumingAvailableMemory
        private static void ClearStandbyFileSystemCacheByConsumingAvailableMemory()
        {
            //consume all available memory then free it, which will wipe out the standby cache
            Context.TraceEvent(1, 1, "Clearing standby cache by consuming all available memory");

            //get the page size. will need to write at least one byte per page to make sure that page is committed to this process' working set: http://blogs.msdn.com/b/ntdebugging/archive/2007/11/27/too-much-cache.aspx?CommentPosted=true&PageIndex=2#comments
            SYSTEM_INFO sysinfo = new SYSTEM_INFO();
            GetSystemInfo(ref sysinfo);

            Context.TraceEvent(1, 2, "Page size on this server is " + sysinfo.dwPageSize + " bytes");

            System.Diagnostics.PerformanceCounter pcAvailableBytes = null;
            long lngAvailableBytes = 0;

            pcAvailableBytes = new System.Diagnostics.PerformanceCounter("Memory", "Available Bytes", true);
            lngAvailableBytes = (long)pcAvailableBytes.NextValue();
            Context.TraceEvent(1, 3, "Available Bytes after clearing active cache: " + lngAvailableBytes);

            long lngRemainingBytes = lngAvailableBytes - (1024 * 1024); //take up all available memory minus 1MB
            System.Collections.Generic.List<IntPtr> listPtrMem = new System.Collections.Generic.List<IntPtr>();
            try
            {
                Context.TraceEvent(1, 4, "Preparing to consume " + lngRemainingBytes + " bytes of memory");

                while (lngRemainingBytes > 0)
                {
                    //figure out the next allocation size
                    int iAllocLen = (int)Math.Min((long)(sysinfo.dwPageSize * 1024), lngRemainingBytes);
                    lngRemainingBytes -= iAllocLen;

                    //allocate this memory
                    listPtrMem.Add(Marshal.AllocHGlobal(iAllocLen));

                    //write one byte per page which is the minimum necessary to make sure this page gets committed to this process' working set
                    for (int j = 0; j < iAllocLen; j += (int)sysinfo.dwPageSize)
                    {
                        Marshal.WriteByte(listPtrMem[listPtrMem.Count - 1], j, (byte)1);
                    }
                }

                lngAvailableBytes = (long)pcAvailableBytes.NextValue();
                Context.TraceEvent(1, 5, "Available Bytes after consuming memory: " + lngAvailableBytes);

            }
            catch (OutOfMemoryException ex)
            {
                Context.TraceEvent(1, 5, "Received OutOfMemoryException: " + ex.Message);
                Context.TraceEvent(1, 10, "Was able to consume desired memory except for the following number of bytes: " + lngRemainingBytes);
            }
            finally
            {
                // dont forget to free up the memory. 
                foreach (IntPtr ptrMem in listPtrMem)
                {
                    if (ptrMem != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptrMem);
                }
            }

            lngAvailableBytes = (long)pcAvailableBytes.NextValue();
            Context.TraceEvent(1, 6, "Available Bytes after freeing consumed memory: " + lngAvailableBytes);
        }
        #endregion

        #region GetFileSystemCacheBytes
        public static System.Data.DataTable GetFileSystemCacheBytes()
        {
            System.Data.DataTable tableReturn = new System.Data.DataTable();
            tableReturn.Columns.Add("Cache Bytes", typeof(long));
            tableReturn.Columns.Add("Cache GB", typeof(decimal));
            tableReturn.Columns.Add("Cache Bytes Peak", typeof(long));
            tableReturn.Columns.Add("Cache GB Peak", typeof(decimal));
            tableReturn.Columns.Add("Standby Cache Bytes", typeof(long));
            tableReturn.Columns.Add("Standby Cache GB", typeof(decimal));

            System.Diagnostics.PerformanceCounter pcCacheBytes = new System.Diagnostics.PerformanceCounter("Memory", "Cache Bytes", true);
            System.Diagnostics.PerformanceCounter pcCacheBytesPeak = new System.Diagnostics.PerformanceCounter("Memory", "Cache Bytes Peak", true);

            float fltCacheBytes = pcCacheBytes.NextValue();
            float fltCacheBytesPeak = pcCacheBytesPeak.NextValue();

            float? fltStandbyCacheBytes = null;

            try
            {
                System.Diagnostics.PerformanceCounter pcStandbyCacheBytes = new System.Diagnostics.PerformanceCounter("Memory", "Standby Cache Normal Priority Bytes", true);
                fltStandbyCacheBytes = pcStandbyCacheBytes.NextValue();
                pcStandbyCacheBytes.Close();
            }
            catch { }

            pcCacheBytes.Close();
            pcCacheBytesPeak.Close();

            object[] row = new object[] { 
                (long)fltCacheBytes, 
                decimal.Round((decimal)(fltCacheBytes / 1024 / 1024 / 1024), 2), 
                (long)fltCacheBytesPeak, 
                decimal.Round((decimal)(fltCacheBytesPeak / 1024 / 1024 / 1024), 2), 
                (fltStandbyCacheBytes==null?null:fltStandbyCacheBytes), 
                (fltStandbyCacheBytes==null?(decimal?)null:(decimal?)decimal.Round((decimal)(fltStandbyCacheBytes / 1024 / 1024 / 1024), 2))
            };

            tableReturn.Rows.Add(row);
            return tableReturn;
        }
        #endregion

        #region Windows APIs
        public static bool Is64BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        [DllImport("NTDLL.dll", SetLastError = true)]
        internal static extern int NtSetSystemInformation(int SystemInformationClass, IntPtr SystemInfo, int SystemInfoLength);

        //SystemInformationClass values
        private static int SYSTEMCACHEINFORMATION = 0x15;
        private static int SYSTEMMEMORYLISTINFORMATION = 80;

        //SystemInfo values
        private static int ClearStandbyPageList = 4;

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

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

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
        private const string SE_PROFILE_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";


        private static bool SetIncreasePrivilege(string privilegeName)
        {
            try
            {
                using (WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent(TokenAccessLevels.AdjustPrivileges | TokenAccessLevels.Query))
                {
                    bool retVal;
                    TokPriv1Luid tp;
                    tp.Count = 1;
                    tp.Luid = 0;
                    tp.Attr = SE_PRIVILEGE_ENABLED;
                    retVal = LookupPrivilegeValue(null, privilegeName, ref tp.Luid);
                    if (!retVal) throw new Exception("Error in LookupPrivilegeValue: ", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
                    retVal = AdjustTokenPrivileges(currentIdentity.Token, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
                    if (!retVal) throw new Exception("Error in AdjustTokenPrivileges: ", new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
                    return retVal;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SetIncreasePrivilege(" + privilegeName + "):", ex);
            }
        }

        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            internal _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MaxPayneExternalTrainer
{
    class MemUtil
    {
        #region NativeFunctions
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, ulong dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, ulong dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, IntPtr[] lphModule, ulong cb, out ulong lpcbNeeded, ulong dwFilterFlag);

        [DllImport("psapi.dll")]
        public static extern ulong GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] char[] lpFilename, ulong nSize);

        const int PROCESS_QUERY_INFORMATION = 0x400;
        const int PROCESS_VM_OPERATION = 0x8;
        const int PROCESS_VM_READ = 0x10;
        const int PROCESS_VM_WRITE = 0x20;
        const int LIST_MODULES_ALL = 0x03;
        #endregion

        Process m_p;
        IntPtr m_handle;

        public MemUtil()
        {
            m_p = null;
            m_handle = IntPtr.Zero;
        }

        public bool OpenHandle(string name)
        {
            Process[] plist = Process.GetProcessesByName(name);
            if (plist.Length > 0)
                m_p = plist[0];
            if (m_p != null)
            {
                m_handle = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION, false, m_p.Id);
                Debug.WriteLine("Proces open");
                return true;
            }
            return false;
        }

        private byte[] ReadMemory(IntPtr baseAddr, ulong memorySize, out int readed)
        {
            if (memorySize > 2147483648) //Not abuse
            {
                readed = 0;
                return new byte[0];
            }

            byte[] memoryData = new byte[memorySize];
            ReadProcessMemory(m_handle, baseAddr, memoryData, memorySize, out readed);
            return memoryData;
        }

        public IntPtr ReadPtr(IntPtr adress)
        {
            ulong taille = 4, ptr = 0;
            int readed;
            byte[] memoryData = ReadMemory(adress, taille, out readed);
            for (int i = 3; i >= 0; i--)
            {
                ptr += memoryData[i];
                if (i != 0)
                    ptr = ptr << 8;
            }
            return (IntPtr)ptr;
        }

        public int WritePatternFromAddr(IntPtr addr, byte[] pattern)
        {
            int writen = 0;
            WriteProcessMemory(m_handle, addr, pattern, (ulong)pattern.Length, ref writen);
            return writen;
        }

        public IntPtr GetMainModuleAdress()
        {
            if (m_p != null)
                return m_p.MainModule.BaseAddress;
            return IntPtr.Zero;
        }

        public IntPtr GetModuleAdresseByName(string name)
        {
            foreach(ProcessModule m in m_p.Modules)
            {
                if (m.ModuleName == name)
                    return m.BaseAddress;
            }

            //Native
            ulong nmbrModules = 0;
            if (EnumProcessModulesEx(m_handle, null, 0, out nmbrModules, LIST_MODULES_ALL))
            {
                IntPtr[] modules = new IntPtr[nmbrModules / ((ulong)IntPtr.Size)];
                if (EnumProcessModulesEx(m_handle, modules, nmbrModules, out nmbrModules, LIST_MODULES_ALL))
                {
                    char[] mName = new char[1024];
                    string cmName;
                    foreach (IntPtr mHandle in modules)
                    {
                        ulong taille = GetModuleFileNameEx(m_handle, mHandle, mName, 1024);
                        cmName = new string(mName, 0,(int)taille);
                        cmName = Path.GetFileName(cmName);
                        //Debug.WriteLine("Module: " + cmName);
                        if (name == cmName)
                            return mHandle;
                    }
                }
            }    
            return IntPtr.Zero;
        }

        public bool CloseHandle()
        {
            if (m_handle != null)
            {
               return CloseHandle(m_handle);
            }
            return true; //no handle then closed yep
        }
    }
}

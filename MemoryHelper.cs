using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReadMemory
{
    public class MemoryHelper
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        private readonly Process process;
        private readonly string moduleName;

        public MemoryHelper(Process process, string moduleName)
        {
            this.process = process;
            this.moduleName = moduleName;
        }

        public int ReadMemoryAddress(int address, int[] offsets = null)
        {
            var hProc = OpenProcess(0x001F0FFF, false, process.Id);
            var modBase = GetModuleBaseAddress(process, moduleName);

            byte[] buffer = new byte[sizeof(int)];
            int read = -1;

            if (offsets != null && offsets.Length != 0)
            {
                var addr = FindDMAAddy(hProc, (IntPtr)(modBase + address), offsets);
                ReadProcessMemory((int)hProc, (int)addr, buffer, buffer.Length, ref read);
            }
            else
            {
                ReadProcessMemory((int)hProc, (int)(modBase + address), buffer, buffer.Length, ref read);
            }

            return BitConverter.ToInt32(buffer, 0);
        }

        private static IntPtr FindDMAAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
        {
            var buffer = new byte[IntPtr.Size];
            foreach (int i in offsets)
            {
                ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out var read);

                ptr = (IntPtr.Size == 4)
                ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i)
                : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
            }
            return ptr;
        }

        private static IntPtr GetModuleBaseAddress(Process proc, string modName)
        {
            IntPtr addr = IntPtr.Zero;

            foreach (ProcessModule m in proc.Modules)
            {
                if (m.ModuleName == modName)
                {
                    addr = m.BaseAddress;
                    break;
                }
            }
            return addr;
        }
    }
}

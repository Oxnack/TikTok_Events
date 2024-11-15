using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


using static System.Net.Mime.MediaTypeNames;
using System.Collections.Concurrent;

namespace TikTok_Events
{
    class Effects
    {
        public struct POINT
        {
            public int X;
            public int Y;
        }


        private static IntPtr hookId = IntPtr.Zero;
        private static LowLevelMouseProc proc = HookCallback;

        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool block);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_MOUSE_LL = 14; // Low-level mouse hook


        public async Task BlockMouseMovement(int time)
        {
            GetCursorPos(out POINT lpPoint);
            await Task.Delay(time);
            while (true)
            {
                Console.WriteLine("Setting cursor pos");
                SetCursorPos(lpPoint.X, lpPoint.Y);
            }
        }
        public async Task blockMouse(int time)
        {

            hookId = SetHook(proc);
            Console.WriteLine($"Blocking mous input for {time/1000} seconds...");
            BlockMouseMovement(time);
            Thread.Sleep(time);
            UnhookWindowsHookEx(hookId);
        }

        public async Task blockKeyboardAndMouse(int time)
        {
            Console.WriteLine($"Blocking all inputs for {time / 1000} seconds...");

            // Block mouse and keyboard input
            BlockInput(true);

            // Wait for the specified duration
            Thread.Sleep(time); // 5000 milliseconds = 5 seconds

            // Unblock mouse and keyboard input
            BlockInput(false);

            Console.WriteLine("Inputs unblocked.");

        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0) // If nCode is non-negative
            {
                return (IntPtr)1; // Suppress all mouse events
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam); // Pass the event to the next hook
        }


        public static void Main(string[] args)
        {
            Effects ef = new Effects();


            Task.WaitAll(ef.blockMouse(5000));
        }
        
    }


   
    

    
}

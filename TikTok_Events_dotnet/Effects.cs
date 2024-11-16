using System;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SystemEffects
{
    class MouseBlocker
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
            Stopwatch s = new Stopwatch();
            GetCursorPos(out POINT lpPoint);
            s.Start();
            
            
            while (s.Elapsed < TimeSpan.FromMilliseconds(time))
            {
                Console.WriteLine($"Setting cursor pos to {lpPoint.X}, {lpPoint.Y}");
                SetCursorPos(lpPoint.X, lpPoint.Y);
            }
        }
        public async Task blockMouse(int time) // mouse input
        {

            hookId = SetHook(proc);
            Console.WriteLine($"Blocking mouse input for {time/1000} seconds...");
            Task.WaitAll(BlockMouseMovement(time));
            
            UnhookWindowsHookEx(hookId);
        }

        public async Task blockKeyboardAndMouse(int time) // both
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


        
        
    }

    class KeyboardBlocker
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;

        public async Task blockKeyboard(int time)
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            Console.WriteLine($"Blocking keyboard for {time/1000} seconds...");
            Thread.Sleep(time); // delay
            Console.WriteLine("Unblocking keyboard");
            UnhookWindowsHookEx(_hookID); // remove hook
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN))
            {
                
                return (IntPtr)0; // return 1 to block input
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); // give control to next hook
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    class ScreenBlocker
    {
        public int WM_SYSCOMMAND = 0x0112;
        public int SC_MONITORPOWER = 0xF170;

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private void SetMonitorInState(int state) // -1 is on, 2 is off and 1 is standby
        {
            SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
        }

        public async Task BlockScreen(int time)
        {
            Stopwatch s = new Stopwatch();
            
            s.Start();


            Console.WriteLine($"Screen is off");
            while (s.Elapsed < TimeSpan.FromMilliseconds(time))
            {

                SetMonitorInState(2);
                
            }
            Console.WriteLine("Screen is on");
            SetMonitorInState(-1);
        }
    }

    class Effects // main class that controls all the other classes
    {
        private MouseBlocker _mb = new MouseBlocker();
        private KeyboardBlocker _kb = new KeyboardBlocker();
        private ScreenBlocker _sb = new ScreenBlocker();

        public async Task BlockScreen(int time) // all time in ms
        {
            Task.WaitAll(_sb.BlockScreen(time));
        }

        public async Task BlockMouse(int time)
        {
           Task.WaitAll( _mb.blockMouse(time));
        }
        public async Task BlockKeyboard(int time)
        {
            Task.WaitAll(_kb.blockKeyboard(time));
        }

        public static void Test(string[] args)
        {
            MouseBlocker mb = new MouseBlocker();
            KeyboardBlocker kb = new KeyboardBlocker();
            ScreenBlocker sb = new ScreenBlocker();


            //Task.WaitAll(sb.BlockScreen(10000));
        }
    }
   
    

    
}

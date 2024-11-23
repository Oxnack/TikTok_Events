// See https://aka.ms/new-console-template for more information
using System;
///  Запуск софта от имени администратора !!!
///  1. зайти в директорию с проектом 
///  2. написать в консоль dotnet build (требуется после изменения кода)
///  3. зайти в директорию ...\TikTok_Events_dotnet\bin\Debug\net8.0
///  4. запустить исполняемый файл .exe от имени администратора
///  5. Program has started!



namespace TikTok_Event_Effects     
{
    public class Program
    {
        public WebSocketClient _client = new WebSocketClient();
        public Effects _effects = new Effects();

        private int blockScreen_time = 500;  // time to do effect [ms]
        private int blockMouse_time = 500;
        private int blockKeyboard_time = 500;

        private string blockScreen_giftName = "0";  // Names of Gifts
        private string blockMouse_giftName = "1";
        private string blockKeyboard_giftName = "2";

        public static async Task Main(string[] args)
        {
            var program = new Program();

            program._client.OnEventHandler += program.OnEvent;
            await program._client.Connection();

            Console.ReadLine();
        }

        public void OnEvent(int n, string giftName)
        {
            Console.WriteLine("get event");

            if (giftName == "giftName2_ToBlockScreen")   //Screen
            {
                int time = n * blockScreen_time;
                _effects.BlockScreen(time);
            }
            if (giftName == "giftName1_ToBlockMouse")   //Mouse
            {
                int time = n * blockMouse_time;
                _effects.BlockMouse(time);
            }
            if (giftName == "giftName0_ToBlockKeyboard")   //Keyboard
            {
                int time = n * blockKeyboard_time;
                _effects.BlockKeyboard(time);
            }

        }
    }
}


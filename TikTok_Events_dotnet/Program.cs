// See https://aka.ms/new-console-template for more information
using System;

namespace TikTok_Event_Effects
{
    public class Program
    {
        public WebSocketClient _client = new WebSocketClient();

        public static async Task Main(string[] args)
        {
            var program = new Program();

            program._client.OnEventHandler += program.OnEvent;
            await program._client.Connection();

            Console.ReadLine();
        }

        public void OnEvent()
        {
            Console.WriteLine("get event");
        }
    }
}


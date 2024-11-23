// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;


namespace TikTok_Event_Effects
{
    public delegate void OnEvent(int n, string nameOfGift);
    public class WebSocketClient
    {

        public OnEvent OnEventHandler { get; set; }


        public async Task Connection()
        {
            await Task.Delay(1000);

            using (var ws = new WebSocketSharp.WebSocket("ws://localhost:21213/")) // Server URL
            {

                ws.Connect();
                // Sub to Message 
                ws.OnMessage += (sender, e) =>
                {
                    // Json Deserialize
                    var data = JsonConvert.DeserializeObject<JsonData>(e.Data);
                    //Console.WriteLine(e.Data);
                    if (data.Event == "gift")
                    {
                        string giftName = data.Data.giftName;
                        int n = data.Data.repeatCount;
                        OnEventHandler(n, giftName);
                        Console.WriteLine("SenderName: " + data.Data.nickname +  " ;SenderUniqueId: " + data.Data.uniqueId + " ;giftName: " + data.Data.giftName + "; GiftsCount: " + data.Data.repeatCount + "------------------------------->>>>>>>");
                    }

                };

                ws.Connect();


                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }

    public class JsonData  //Class to Deserialize
    {
        public string Event { get; set; }
        public Data Data { get; set; }
    }
    public class Data  //Class to Deserialize
    {
        public int repeatCount { get; set; }
        public string nickname { get; set; }
        public string uniqueId { get; set; }
        public string giftName { get; set; }
    }
}


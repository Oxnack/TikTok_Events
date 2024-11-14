// See https://aka.ms/new-console-template for more information
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WebSocketClient
{
    private ClientWebSocket _websocket = null;

    public async Task ConnectAsync()
    {
        if (_websocket != null && _websocket.State == WebSocketState.Open) return; // Already connected

        _websocket = new ClientWebSocket();
        try
        {
            await _websocket.ConnectAsync(new Uri("ws://localhost:21213/"), CancellationToken.None);
            Console.WriteLine("Connected");

            await ReceiveMessages();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
            _websocket = null;
            await Task.Delay(1000); // Schedule a reconnect attempt
            await ConnectAsync();
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024];

        while (_websocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await _websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    Console.WriteLine("Disconnected");
                    _websocket = null;
                    await Task.Delay(1000); // Schedule a reconnect attempt
                    await ConnectAsync();
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    HandleMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _websocket = null;
                await Task.Delay(1000); // Schedule a reconnect attempt
                await ConnectAsync();
            }
        }
    }

    private void HandleMessage(string message)
    {
        var parsedData = JsonConvert.DeserializeObject<ParsedData>(message); // Deserialize JSON data

        Console.WriteLine("Data received: " + JsonConvert.SerializeObject(parsedData, Formatting.Indented));

        // Here you can implement additional logic for handling the parsed data,
        // such as logging it or updating a UI component if this is a GUI application.
    }

    public class ParsedData
    {
        public string Event { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string UniqueId { get; set; }
        // Add other properties as needed
    }

    public static async Task Main(string[] args)
    {
        var client = new WebSocketClient();
        await client.ConnectAsync();
    }
}

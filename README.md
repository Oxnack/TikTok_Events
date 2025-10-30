# TikTok Event Effects

A .NET application that creates interactive effects on your computer when receiving gifts during TikTok live streams. The application connects to a WebSocket server to receive real-time gift events and triggers various system effects based on the type of gift received.

## 🚀 Features

- **Real-time TikTok Gift Monitoring**: Connects to a WebSocket server to receive live gift events
- **Multiple Interactive Effects**:
  - 🖥️ Screen Blocking - Turns off the monitor for a specified duration
  - 🖱️ Mouse Blocking - Temporarily disables mouse input
  - ⌨️ Keyboard Blocking - Temporarily disables keyboard input
- **Configurable Settings**: Customizable effect durations and gift triggers
- **Administrative Control**: Requires admin privileges for system-level operations

## 📋 Prerequisites

- **.NET 8.0 Runtime** or higher
- **Administrator privileges** (required for system-level input blocking)
- **WebSocket Server** running on `localhost:21213`

## 🛠️ Installation & Setup

1. **Clone or Download** the project files
2. **Build the Project**:
   ```bash
   dotnet build
   ```
3. **Navigate to Build Directory**:
   ```
   ...\TikTok_Events_dotnet\bin\Debug\net8.0\
   ```
4. **Run as Administrator**:
   - Right-click on the `.exe` file
   - Select "Run as administrator"

## ⚙️ Configuration

### Effect Durations (in Program.cs)
```csharp
private int blockScreen_time = 10000;    // 10 seconds
private int blockMouse_time = 2500;      // 2.5 seconds  
private int blockKeyboard_time = 10000;  // 10 seconds
```

### Gift Triggers (in Program.cs)
```csharp
private string blockScreen_giftName = "0";        // Gift name for screen blocking
private string blockMouse_giftName = "Rose";      // Gift name for mouse blocking
private string blockKeyboard_giftName = "Heart Me"; // Gift name for keyboard blocking
```

## 🎮 How It Works

1. **Application Start**: Run the executable as administrator
2. **WebSocket Connection**: Automatically connects to `ws://localhost:21213/`
3. **Event Listening**: Listens for TikTok gift events in real-time
4. **Effect Triggering**: When a configured gift is received:
   - Multiplies the base duration by the gift repeat count
   - Executes the corresponding system effect
   - Logs the event details to console

## 📝 Event Logging

The application logs all gift events to the console with details:
```
SenderName: [Username] ; SenderUniqueId: [UniqueID] ; giftName: [GiftName] ; GiftsCount: [Count]
```

## 🛡️ Security Notes

- Requires **administrator privileges** to block system inputs
- Only connects to local WebSocket server (`localhost:21213`)
- No personal data is stored or transmitted

## 🔧 Technical Details

### Built With
- **.NET 8.0**
- **WebSocketSharp** - For WebSocket communication
- **Newtonsoft.Json** - For JSON deserialization
- **Windows API** - For system-level input blocking

### Key Components
- **Program.cs** - Main application logic and event handling
- **Effects.cs** - System effect implementations (mouse, keyboard, screen blocking)
- **Client.cs** - WebSocket client for TikTok event reception

## ⚠️ Important Notes

- Ensure the WebSocket server is running before starting the application
- Effects are cumulative (duration = base time × gift count)
- Application must remain running to receive events
- Some antivirus software may flag system input blocking features

## 🐛 Troubleshooting

**Application won't start:**
- Verify .NET 8.0 is installed
- Run as administrator
- Check if WebSocket server is available

**Effects not working:**
- Confirm admin privileges
- Verify gift names match exactly
- Check console for connection/event logs

**Build errors:**
- Run `dotnet restore` to restore packages
- Ensure all project files are present

## 📄 License

This project is for educational and personal use. Ensure compliance with TikTok's Terms of Service when using this application.

---

**Disclaimer**: Use responsibly and ensure you have proper permissions when running system-level modifications.
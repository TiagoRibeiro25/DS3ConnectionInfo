# DS3ConnectionInfo

A simple tool showing Dark Souls III online session information, player count and Steam names. Runs on Linux through Proton alongside DS3.

## Features

- Shows how many players are in your current online session
- Displays Steam names of connected players
- Detects DS3 process and connects via Steam API

## Installation

### Windows

Build from source or download a release, then run `DS3ConnectionInfo.exe`.

### Linux (Proton)

1. Build for Windows:
    ```bash
    dotnet publish DS3ConnectionInfo/DS3ConnectionInfo.csproj -c Release -r win-x64 --self-contained false
    ```
2. Run the exe through the same Proton/Wine that runs DS3.

**Important:** The tool must run in the same Proton prefix as DS3 to access Steam P2P session data. DS3 memory reading does not work under Proton — only P2P-based player detection is used.

## Building

Requires .NET 8 SDK.

```bash
# Build for native Linux (testing)
dotnet build DS3ConnectionInfo/DS3ConnectionInfo.csproj

# Build for Windows (Proton)
dotnet publish DS3ConnectionInfo/DS3ConnectionInfo.csproj -c Release -r win-x64 --self-contained false
```

## How it works

The Steam API (`SteamFriends.GetCoplayFriendCount` + `SteamNetworking.GetP2PSessionState`) is used to enumerate active P2P connections from DS3. Each connected player's Steam name is displayed via `SteamFriends.GetFriendPersonaName`.

## Known limitations

- **No memory-based data under Proton:** The hardcoded DS3 memory addresses do not work through Wine, so character name, team, slot, and geolocation are not available when running via Proton.
- **No overlay, ping filter, or hotkeys:** These features were removed for Linux/Proton compatibility.
- **Requires same prefix:** The tool and DS3 must run in the same Proton/Wine prefix.

## Credits

Developers of the DS3 Grand Archives Cheat Table for the original player data pointers.
Tremwill and svew for the original DS3ConnectionInfo project, which this project is based on.

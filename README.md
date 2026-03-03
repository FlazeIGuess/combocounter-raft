# ComboCounter Mod for Raft

Track your combat prowess with a visual combo counter that displays consecutive hits below your crosshair.

## Features

- Real-time combo tracking for consecutive hits
- Kill counter integrated into combo display
- Smooth fade and blink animations when combo is about to expire
- Fully customizable colors (RGB sliders)
- Adjustable combo reset time (1-10 seconds)
- Toggle combo display on/off
- Lightweight and performance-friendly
- Client-side only (doesn't require all players to have it installed)

## Installation

1. Install [RaftModLoader](https://www.raftmodding.com/loader)
2. Download the latest `ComboCounter.rmod` file from the [releases page](https://github.com/FlazeIGuess/combocounter-raft/releases)
3. Place the `.rmod` file in your RaftModLoader mods folder
4. Launch Raft through RaftModLoader

## Configuration (Optional)

For in-game configuration, install the [Extra Settings API](https://www.raftmodding.com/mods/extra-settings-api) mod. This allows you to customize:

### Combo Settings
- Enable/disable combo counter display
- Combo reset time (1-10 seconds, default: 5 seconds)

### Color Settings
- Combo text color (RGB sliders, 0.0-1.0 for each channel)
- Default color: #F2E2C5 (warm beige)

Access settings via: Main Menu > Settings > Mods tab

The mod works perfectly fine without Extra Settings API using default values.

## How It Works

The combo counter tracks consecutive hits on enemies:
- Single hit: Shows "HIT!" or "KILLED!" (if fatal)
- Multiple hits: Shows "Xx COMBO!" with optional kill count
- Blinks when combo is about to expire (last 30% of reset time)
- Resets after configured time without hits (default: 5 seconds)

## Building from Source

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8
- Raft game installed
- RaftModLoader installed

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/FlazeIGuess/combocounter-raft.git
   cd combocounter-raft
   ```

2. Update the reference paths in `ComboCounter/ComboCounter.csproj` to match your Raft installation directory

3. Build the solution:
   ```bash
   msbuild ComboCounter.sln /p:Configuration=Debug
   ```
   
   Or open `ComboCounter.sln` in Visual Studio and build from there

4. The mod will be automatically packaged as `ComboCounter.rmod` in the root directory

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the GNU Affero General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [RaftModLoader](https://www.raftmodding.com/)
- Uses [Harmony](https://github.com/pardeike/Harmony) for runtime patching
- Created by Flaze

## Support

- Report bugs on the [Issues page](https://github.com/FlazeIGuess/combocounter-raft/issues)
- Join the [Raft Modding Discord](https://www.raftmodding.com/discord)

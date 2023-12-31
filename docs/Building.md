# Building SCPDiscord

## Dependencies
- dotnet-sdk (7.0 at time of writing, check project file)
- mono
- protobuf
- warp-packer (Optional, only used for packaging the bot)

## Building the plugin

### Via Rider or Visual Studio
Load the SCPDiscordPlugin project and build using the controls in the IDE.

Note: Rider doesn't support mixing mono and dotnet projects in the same solution so you have to open the projects individually, not the solution.

### Manually
Enter the SCPDiscord plugin directory and use the following command in order to build the plugin:
```bash
msbuild SCPDiscordPlugin.csproj -restore -p:PostBuildEvent=
```

## Building the bot

### Via Rider or Visual Studio
Load the SCPDiscordPlugin project and build using the controls in the IDE.

### Manually
Enter the SCPDiscord bot directory and use the following commands:
```bash
dotnet build --output bin/linux-x64 --configuration Release --runtime linux-x64
dotnet build --output bin/win-x64 --configuration Release --runtime win-x64
```

### Packaging the bot
Before the bot is released its library dependencies are baked into the executable in order to provide it as a single file.

This is done using the following commands in the bot directory:
```bash
warp-packer --arch linux-x64 --input_dir bin/linux-x64 --exec SCPDiscordBot --output ../SCPDiscordBot_Linux
warp-packer --arch windows-x64 --input_dir bin/win-x64 --exec SCPDiscordBot.exe --output ../SCPDiscordBot_Windows.exe
```

## Generating the network interface

The bot and plugin communicate using protobuf messages. These messages are constructed from protobuf schemas located in the schema directory which are then generated into the bot and plugin's interface directories.

If you edit the schema files you need to run the following command in the schema directory to generate the interface classes:
```bash
protoc --csharp_out "../SCPDiscordBot/Interface" --csharp_out "../SCPDiscordPlugin/Interface" --proto_path . *.proto ./BotToPlugin/*.proto ./PluginToBot/*.proto
```
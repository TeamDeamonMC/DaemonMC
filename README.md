# DaemonMC
**This software is still in development**

Fast and lightweight server software for Minecraft: Bedrock Edition optimised for mini games.
Instead of vanilla features and mechanics, this software will focus on performance, simplicity.
Discord: [https://discord.gg/A6BBcXSCj4](https://discord.gg/A6BBcXSCj4)

## Getting started

Download latest .zip from [Releases](https://github.com/laz1444/DaemonMC/releases). Unzip and run DaemonMC.exe.
This action will create: 
- Plugins (for plugin .dll files)
- Resource Packs (for resource pack .mcpack files and .key files for encrypted packs)
- Worlds (for world .mcworld files)
- DaemonMC.yaml (more info [wiki#daemonmcyaml](https://github.com/laz1444/DaemonMC/wiki#daemonmcyaml))

For updating you will need to download latest .dll from [Releases](https://github.com/laz1444/DaemonMC/releases) and replace with old one.

> [!NOTE]
Server don't have it's own world generator (only temporary flat world when starting server without .mcworld file), so you will need to use your own .mcworld file in Worlds folder.

## Features

**Up to date resources:** This software will always support latest world format, entities, sounds and all other things found in latest Minecraft version.

**Multiversion:** To make updating easier for players and servers, this software supports also previous game versions.
Just remember that because of the latest world format, players using older game versions won't be able too see blocks added in new versions.

**Multiworld:** You can have as many worlds as you want. Just specify spawn world name in ```DaemonMC.yaml``` and use API ([ChangeWorld(World, Vector3)](https://github.com/laz1444/DaemonMC/wiki/Plugin-API-(Methods)#changeworldworld-vector3).) to transfer players to other worlds.

**Simple plugin API:**  Instead of vanilla based mechanics, with this software game is completely driven by plugins.
Plugin tutoral, API and other useful things can be found in [Wiki](https://github.com/laz1444/DaemonMC/wiki).

Click here to learn how to create first plugin [Plugin tutorial](https://github.com/laz1444/DaemonMC/wiki/Plugin-tutorial)
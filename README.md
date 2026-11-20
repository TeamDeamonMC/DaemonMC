
# DaemonMC

Fast and lightweight server software for Minecraft: Bedrock Edition designed for mini games.

[![Discord](https://img.shields.io/discord/932359565612294224?logo=discord&logoColor=white&color=blue)](https://discord.gg/A6BBcXSCj4)
![Minecraft - Version](https://img.shields.io/badge/Minecraft_1.21.90_--_1.26.60-darkgreen)
![LevelDB](https://img.shields.io/badge/LevelDB_26.60-gray)

Instead of vanilla features and mechanics, here game is completely driven by plugins. DaemonMC provide only server core with simple API so you can add only what you need. No unnecessary server resources and network usage by various features like block tick or mobs.

List of publicly available plugins [Public plugins and tools](https://github.com/TeamDeamonMC/DaemonMC/wiki/.-Public-plugins-and-tools)

To learn how to create plugins check [Plugin tutorial](https://github.com/TeamDeamonMC/DaemonMC/wiki/.-Plugin-tutorial)

## Getting started

Download latest .zip from [Releases](https://github.com/TeamDeamonMC/DaemonMC/releases). Unzip and run DaemonMC.exe.
This action will create: 
- Plugins (for plugin .dll files)
- Plugins/SharedLibraries (for library .dll files that are used by plugins)
- Resource Packs (for resource pack .mcpack files and .key files for encrypted packs)
- Worlds (for world .mcworld files)
- DaemonMC.yaml (more info [wiki#daemonmcyaml](https://github.com/TeamDeamonMC/DaemonMC/wiki#daemonmcyaml))

For updating you will need to download latest .dll from [Releases](https://github.com/TeamDeamonMC/DaemonMC/releases) and replace with old one.

## Features

**Up to date resources:** This software will always support latest world format, entities, sounds and all other things found in latest Minecraft version.

**Multiversion:** To make updating easier for players and servers, this software supports also previous game versions.
Just remember that because of the latest world format, players using older game versions won't be able to see blocks added in new versions.

**Multiworld:** You can have as many worlds as you want. Just specify spawn world name in ```DaemonMC.yaml``` and use API ([ChangeWorld(World, Vector3)](https://github.com/TeamDeamonMC/DaemonMC/wiki/Plugin-API-(Methods)#changeworldworld-vector3).) to transfer players to other worlds.

**Simple plugin API:** Plugin tutoral, API and other useful things can be found in [Wiki](https://github.com/TeamDeamonMC/DaemonMC/wiki).

Want to contribute? That's really cool. Here's some useful information: [Contributing.md](https://github.com/TeamDeamonMC/DaemonMC/blob/main/Contributing.md)

## Servers running on DaemonMC

Want to see your server here? Make pull request and add your server to the list!

**lazon.top:19132** - Minigames server
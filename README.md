# DaemonMC
**This software is still in development**

Fast and lightweight server software for Minecraft: Bedrock Edition designed for mini games.
Instead of vanilla features and mechanics, here game is completely driven by plugins. All DaemonMC provide is server core with simple API so you can add only what you need. No unnecessary server resources and network usage by various core features like block tick or mobs.
Discord: [https://discord.gg/A6BBcXSCj4](https://discord.gg/A6BBcXSCj4)


**Servers / Test servers running DaemonMC**
csmpmg.minecraft.pe 19132

## Getting started

Download latest .zip from [Releases](https://github.com/TeamDeamonMC/DaemonMC/releases). Unzip and run DaemonMC.exe.
This action will create: 
- Plugins (for plugin .dll files)
- Resource Packs (for resource pack .mcpack files and .key files for encrypted packs)
- Worlds (for world .mcworld files)
- DaemonMC.yaml (more info [wiki#daemonmcyaml](https://github.com/TeamDeamonMC/DaemonMC/wiki#daemonmcyaml))

For updating you will need to download latest .dll from [Releases](https://github.com/TeamDeamonMC/DaemonMC/releases) and replace with old one.

> [!NOTE]
Server don't have it's own world generator (only temporary flat world when starting server without .mcworld file), so you will need to use your own .mcworld file in Worlds folder.

## Features

**Up to date resources:** This software will always support latest world format, entities, sounds and all other things found in latest Minecraft version.

**Multiversion:** To make updating easier for players and servers, this software supports also previous game versions.
Just remember that because of the latest world format, players using older game versions won't be able too see blocks added in new versions.

**Multiworld:** You can have as many worlds as you want. Just specify spawn world name in ```DaemonMC.yaml``` and use API ([ChangeWorld(World, Vector3)](https://github.com/TeamDeamonMC/DaemonMC/wiki/Plugin-API-(Methods)#changeworldworld-vector3).) to transfer players to other worlds.

**Simple plugin API:** Plugin tutoral, API and other useful things can be found in [Wiki](https://github.com/TeamDeamonMC/DaemonMC/wiki).

Click here to learn how to create first plugin [Plugin tutorial](https://github.com/TeamDeamonMC/DaemonMC/wiki/Plugin-tutorial)

Want to contribute? That's really cool. Here's some useful information: [Contributing.md](https://github.com/TeamDeamonMC/DaemonMC/blob/main/Contributing.md)

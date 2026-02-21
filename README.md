# THANK YOU ALL FOR 1000 DOWNLOADS ON THUNDERSTORE :D

# Bonelab Randomizer Weaponizer

**Bonelab Randomizer Weaponizer** - When life gives you lemons, you throw them at the Nullbodies.

If you are anything like me, you loved the Arena Trial with random weapons.  
The problem? It was not *that* random, plus it only included base game weapons (boooooring!!!).

Let me introduce you to the **Bonelab Randomizer Weaponizer**!

---

## Features

* **True Random Weapon Spawning** - Pulls a random weapon from a cached list of all valid weapons (yes, this includes modded weapons).  
* **Yes, Modded Ones Too** - Because mentioning it just once is NOT enough.  
* **Level Independent** - Should work pretty much everywhere!
* **Category Filtering** - Toggle which types of firearms or melee items are allowed.  
* **Persistent Settings** - Everything saves through MelonPreferences.  
* **Re-Cache System** - Rebuild your weapon pool on demand when you change filters (no need to restart!).  
* **BoneMenu Integration** - Configure and re-cache directly from an in-game menu.  
* **Debug Logging** - Optional logging for testing and development.  

---

## Installation

1. Install the latest version of [MelonLoader](https://melonwiki.xyz/) for BONELAB.  
2. **Install the following required dependencies:**
   - [Bonelib](https://thunderstore.io/c/bonelab/p/gnonme/BoneLib/)
   - [JeviLib](https://thunderstore.io/c/bonelab/p/extraes/JeviLibBL/)
3. Download the latest release of **Bonelab Randomizer Weaponizer**.  
4. Drop the `.dll` file into your `Mods` folder in your BONELAB directory.  
5. Launch the game. The mod will appear under the **"Random Weapon"** section in BoneMenu.  

> **Note:** The mod *will not function* without **Bonelib** and **JeviLib**, PLEASE do not skip them.

---

## Usage

### Weapon Categories

You can toggle which categories are included when the mod picks a random weapon:

**Firearms:**
- Pistol  
- SMG  
- Rifle  
- Shotgun  
- Other *(Disabled by default, see Known Issues)*  

**Melee:**
- Blade  
- Blunt  
- Other *(Disabled by default, see Known Issues)*  

---

## READ, READ, READ!!
After changing your categories, use the **Re-Cache Items** button to rebuild your weapon list.  
<sub><sub><sub><sub><sub>I am legally allowed to sacrifice you to the void entities if you do not do this and complain.</sub></sub></sub></sub></sub>

---

## Known Issues

- **Other Category** - Broken. Due to most items being tagged as "Other", it can spawn nearly anything, for some reason that includes Avatar Previews (scary!).  
- **Tagging Limitations** - If modders tag something wrong, it might spawn in the wrong category. Not much I can do about that.  
- **Empty Cache when Switching Levels** - Sometimes when you switch between levels, the cache will be cleared and you will need to rebuild it, no idea why. (yes, in-game, use the Re-Cache button).

---

## TODO

- [ ] Comment and document the entire project *(like that’s ever gonna happen).*  
- [ ] Add advanced filtering (e.g. by Pallet).
- [ ] Implement spawn weighting (e.g. 1.2x Shotguns, 0.7x Pistols).  
- [ ] Automatic re-cache when filters are changed.  
- [ ] BoneMenu layout improvements and visual polish.  

---

## Currently Working Features

- [x] Random weapon and melee spawning  
- [x] MelonPreferences saving/loading  
- [x] BoneMenu configuration and toggles  
- [x] Category-based filtering  
- [x] Manual re-cache system  
- [x] Debug mode logging  

---

## Other Notes & Information

- **Hot-Caching** - Untested, if you download via mod.io in Void G114 and re-cache it SHOULD include newly installed weapons, I have not tested that however.
- **Fusion Support** - Also Untested, imagine having friends... hah...
- **Lemonloader Support** - Untested-fest (Untestfest?)
- **Super Professional Programmer** - ...is not something that I am, I just mash stuff together I found while decompiling the Game, expect bugs, memory leaks! (If you actually know what you are doing PLEASE submit a PR).

---

## Credits & Thanks

- **Code & Core Implementation** - by yours truly.  
- **Bug Fixes & Mental Support** - by a good friend (you know who you are).  
- **[Yowchap / Gnonme](https://github.com/yowchap)** - Creator of **Bonelib**.  
- **[extraes](https://github.com/extraes)** - Creator of **JeviLib**.  

---

## License

Licensed under the MIT License, have fun!
I always appreciate credit, but no need for it!

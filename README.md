#Intent of this build

This build is based on the 1.08 version of the SotN Randomizer code that included the Khaos Mod.
The intent is to carry over the Twitch only updates, along with some balancing changes, to the old chatbot version.


## Associated Projects
* [SotnApi](https://github.com/TalicZealot/SotnApi)
* [SotnRandoTools](https://github.com/TalicZealot/SotnRandoTools/releases/latest)
* [SimpleLatestReleaseUpdater](https://github.com/TalicZealot/SimpleLatestReleaseUpdater)
* [SotN Randomizer Source](https://github.com/3snowp7im/SotN-Randomizer)

# Table of Contents
- [Table of Contents](#table-of-contents)
- [Axe Armor](#axe-armor)
  - [Mechanics](#mechanics)
  - [Controls](#controls)
  - [Damage Formulas](#damage-formulas)
- [Symphony of the Night Randomizer Tools](#symphony-of-the-night-randomizer-tools)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Updating](#updating)
  - [Autotracker](#autotracker)
  - [Co-Op](#co-op)
  - [Mayhem-Setup](#Mayhem-Setup)
  - [Useful links](#useful-links)
  - [Contributors](#contributors)
  - [Special Thanks](#special-thanks)

# Axe Armor
This section is very WIP and will be updated gradually each release.

## Mechanics
Axe Armor has been retooled to gain progression level upgrades to movement and navigation similar to Alucard.
Axe Armor can also use consumables + sub weapons to allow for more varied gameplay.

Gathering relics can also grant one or more of the following additional helpful effects:
	Physical Damage
	Spell Damage + Increased Spell Hits
	Defense
	Regenerate Hearts
	Damage while Faceplanting
	Subweapon Damage
	Subweapon Crit Chance

The following armors also bestow additional effects while held in inventory (not equipped):
	Spike breaker
	Healing Mail
	Brilliant Mail
	Mojo Mail

New Subweapon damage types + costs:
	Empty		  - Dark (2 Hearts)
	Holy Ashes    - Poison (2 Hearts)
	Agunea		  - Thunder (4 Hearts)
	Dagger 		  - Cut (4 Hearts)
	Axe 		  - Hit (8 Hearts)
	Cross		  - Dark, Curse (12 Hearts)
	Rebound Stone - Stone (12 Hearts)
	Holy Water 	  - Water, Holy (16 Hearts)
	Bible		  - Holy, Curse (20 Hearts)
	Stop Watch    - Ice (20 Hearts)

## Controls
Up - Revert To Alucard for warp / elevator functionality
Down - Revert To Crouched Alucard for dodging, warp, elevator functionality

L1 - Faceplant (4mp, drains MP over time): Grants I-Frames for whole duration.
R1 - Glide (drains MP over time)
R2 - Momentum

Square -  Swings Axe + Use held items (Note: Weapons now impact axe armor damage + damage type)
Triangle - Elemental Hellfire (25 MP)
	+ Neutral: Ice 
	+ Up:      Fire
	+ Down:    Lightning
Circle - Use subweapons / held items
X / Cross - Double Jump (upgrades to Triple Jump with Leap Stone)

Unique Commands (Momentum):
Left / Right: Build up stored speed.
Neutral - Slide or move in the air with stored speed.
Square - Now moves with stored speed.
Up + Jump - Gravity Jump (requires Gravity Boots)

Unique Commands (while Gliding):
Up (Requires Bat): Flight
Triangle   - Small fireballs that can hit breakable ceiling / floor tiles (15MP)
	+ Up   - Small fireballs shifted upwards
	+ Down - Small fireballs shfited downwards 

Unique Commands (while Faceplanting):
Hold Up - Goes through mist gates, Olrox narrow passageways (requires Form of Mist)
Square - MP to Speed Boost / Aerial Faceplant extension
Triangle - Hearts to MP Conversion
Circle - Subweapon to Hearts Conversion
R1 - Flight

Unique Command (while in hitstun):
Tap R1 when near the ground to "tech" the landing and act faster.

## Damage Formulas
WIP (patch notes will always have the most accurate details if updated)

-New Damage Formulas (Melee):
	1H Weapon x1: 	2 + .75 Weapon ATK 1 + Base STR [.67 <--> .75] + .75 Equipment STR
	1H Fist: 		4 + .75 Weapon ATK 1 + Base STR [0.83 <--> 0.91] + .50 Equipment STR
	1H Weapon x2: 	4 + .75 Weapon ATK 1 + .35 Weapon ATK 2 + Base STR [0.75 <--> 0.91] + .75 Equipment STR
	Fist Only x2: 	6 + .75 Weapon ATK 1 + .35 Weapon ATK 2 + Base STR [1.00 <--> 1.25] + .50 Equipment STR
	2H Weapon: 	  	6 + .75 Weapon ATK 1 + .35 Weapon ATK 2 + Base STR [1.00 <--> 1.25] + .75 Equipment STR
	Throwables:   	Consumes if held in main hand, not counted as a weapon in above calculations if in offhand.
	Shields:		See below chart
	Others:	 	 	1 (ignores Wolf Damage)
	Note: Negative ATK2 from offhand will be ignored as a weapon.
	Note: Dual Wield / 2H will use more favorable rounding for offhand damage.
-New Damage Formulas (Shield Right Hand):
	1H DMG-Less Shield x1: 		2 + .25 Base INT + .50 Equipment INT + Base STR [.50] + .25 Equipment STR
	1H DMG-Less Shield, Wpn: 	2 + .25 Base INT + .50 Equipment INT + Base STR [.50] + .25 Equipment STR + .35 Weapon ATK 2
	1H DMG-Less Shield x2: 		4 + .50 Base INT + .75 Equipment INT + Base STR [.50] + .25 Equipment STR
	1H DMG-Less Shld, DMG Shld: 4 + .50 Base INT + .75 Equipment INT + Base STR [.50] + .25 Equipment STR + .35 Weapon ATK 2
	1H DMG-Less Shield w/ SR/M: 6 + .75 Base INT + 1.0 Equipment INT + Base STR [.50] + .25 Equipment STR + .35 Weapon ATK 2
	1H DMG Shield x1, WPN 		4 + .25 Base INT + .50 Equipment INT + Base STR [0.75 <--> 0.91] + .75 Weapon ATK 1 + .50 Equipment STR + .35 Weapon ATK 2
	1H DMG Shield x1, DMG-Less: 4 + .50 Base INT + .75 Equipment INT + Base STR [0.67 <--> 0.75] + .75 Weapon ATK 1 + .50 Equipment STR
	1H DMG Shield x2: 			6 + .50 Base INT + .75 Equipment INT + Base STR [0.75 <--> 0.91] + .75 Weapon ATK 1 + .35 Weapon ATK 2 + .50 Equipment STR
	1H DMG Shield w/ SR/M:   	8 + .75 Base INT + 1.0 Equipment INT + Base STR [0.75 <--> 0.91] + .75 Weapon ATK 1 + .35 Weapon ATK 2 + .50 Equipment STR
-Current #Hit Damage Scaling rules:
	x1 Hit: 1.0 WPN ATK, -4 Flat Damage
	x2 Hit: .92 WPN ATK, -6 Flat Damage,  -.02 STR Bonus
	x3 Hit: .86 WPN ATK, -9 Flat Damage,  -.06 STR Bonus
	x4 Hit: .78 WPN ATK, -13 Flat Damage, -.12 STR Bonus

# Symphony of the Night Randomizer Tools

## Installation
This tool requires Bizhawk version 2.6 or higher.
Download the full version from the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) that looks like this `SotnRandoTools-x.x.x.zip`
Right click on it and select `Extract all...` then navigate to your BizHawk 2.6+ folder and press `Extract`.
File structure should look like this:
```
BizHawk
└───ExternalTools
│   │   SotnRandoTools.dll
│   │
│   └───SotnRandoTools
│       │   SotnApi.dll
│       │   ...
```

## Usage
After launching the game in BizHawk open through ```Tools > Extarnal Tool > Symphony of the Night Randomizer Tools```
Set your preferences and open the tool you want to use. You can then minimize the main tools window, but don't close it.
Every tool's window possition and the Tracker's size are all saved and will open where you last left them.
If the Extarnal Tool says that the game is not supported for the tool and BizHawk is displaying a question mark in the lower left corner your rom is either not recognized or you have to make sure the cue file is pointing to the correct files. I recommend creating a separate folder for Randomizer where you copy both tracks and the cue and replace track1 every time you randomize.

## Updating
On lunching the tool it will check for a new release and inform the user. If there is a newer release the update button apepars. Clicking it shuts down BizHawk and updates the tool. If it displays "Installation failed" please run the updater manually by going to ```BizHawk\ExternalTools\SotnRandoTools\Updater\SimpleLatestReleaseUpdater.exe``` or get the [latest release](https://github.com/TalicZealot/SotnRandoTools/releases/latest) from GitHub and update manually. If you get an error notifying you that your system lacks the necessary .NET version to run the updater click [the link](https://dotnet.microsoft.com/download/dotnet/5.0/runtime?utm_source=getdotnetcore&utm_medium=referral) and download the x64 and x86 redistributable packages for desktop apps.

## Autotracker
The new tracker has been re-written from the ground up for better performance and usability. Can be manually rescaled. Saves size and location. Locations are drawn on the game map iself instead of relying on BizHawk GUI. It doesn't rely on the PSX display mode anymore and automatically detects everything it needs.

## Co-Op
Coop requires the host to have the port they want to use forwarded. Hosting automatically copies your address(ip:port) to the clipboard. The other player uses that address to connect. Please be careful to not leak your IP!
Bindings over at: [https://taliczealot.github.io/coop/](https://taliczealot.github.io/coop/)

## Mayhem-Setup
* Video setup guide: SOON
* Set up StreamlabsChatbot.
* Turn on and adjust the StreamlabsChatbot currency.
* Follow these instructions: https://streamlabs.com/content-hub/post/chatbot-scripts-desktop
* Import the Khaos-Bot-Script from `BizHawk\ExternalTools\SotnRandoTools\Khaos`.
* Right click the script and select `Insert API Key`.
* Right click the script and select `Open Script Folder`. Inside `Scripts\Khaos-Bot-Script\Overlays` the index.html file is your dynamic commands widget for OBS.
* Click the settings button on the top right of the scripts tab and copy the API Key. Paste it inside SotnRandoTools in the `Khaos > Input > Bot API Key` field.
* Enable the script by clicking the checkbox on the right.
* Adjust the action costs and cooldowns through the script and properties through the tool to your preference.
* Script management commands available to mods and streamer:
  * !startmayhem
  * !stopmayhem
  * !pausemayhem
  * !unpausemayhem
* Useful commands for running Khaos: [https://raw.githubusercontent.com/TalicZealot/SotnRandoTools/main/BotCommands/KhaosHelperCommands.abcomg](https://raw.githubusercontent.com/TalicZealot/SotnRandoTools/main/BotCommands/KhaosHelperCommands.abcomg) `right click > Save Link As...` then import in the command ssection of StreamlabsChatbot.

## Useful links
* [SotN Randomizer](https://sotn.io)
* [Latest BizHawk release](https://github.com/TASVideos/BizHawk/releases/latest)

## Contributors
* [3snowp7im](https://github.com/3snowp7im) - SotN Randomizer developer
* [fatihG](https://twitter.com/fatihG_) - Familiar card icons, replay system concept.

## Special Thanks
* asdheyb
* fatihG
* EmilyNyx
* DinnerDog
* Gods666thChild
* LordalexZader
* TalicZealot
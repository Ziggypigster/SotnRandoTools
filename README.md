# Intent of this build


* [Latest Release](https://github.com/Ziggypigster/SotnRandoTools/releases)

This build is based on the 1.08 version of the SotN Randomizer code that included the Khaos Mod.
The intent is to carry over the Twitch only updates, along with some balancing changes, to the old chatbot version.

## Video Tutorials

* [Axe Armor Tutorials](https://www.youtube.com/playlist?list=PLt7RkGYW_ln0ah9cSqoGVPo1jM7MwslrI)


## Associated Projects
* [SotnApi](https://github.com/TalicZealot/SotnApi)
* [SotnRandoTools](https://github.com/TalicZealot/SotnRandoTools/releases/latest)
* [SimpleLatestReleaseUpdater](https://github.com/TalicZealot/SimpleLatestReleaseUpdater)
* [SotN Randomizer Source](https://github.com/3snowp7im/SotN-Randomizer)

# Table of Contents
- [Table of Contents](#table-of-contents)
- [Axe Armor](#axe-armor)
  - [Features](#features)
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

## Features
Axe Armor has been retooled to gain progression level upgrades to movement and navigation similar to Alucard.  <br />
Axe Armor weapons and shield now change the damage, damage type, and number of hits.  <br />
Axe Armor has unique sub weapon animations, with one additional subweapon if the the player does not have any subweapons held.  <br />
Axe Armor can also use consumables + sub weapons to allow for more varied gameplay.  <br />
MP now regenerates passively while in Axe Armor. This rate is doubled if you are only moving without attacking, or quadruples if standing still.  <br />

Gathering relics can also grant one or more of the following additional helpful effects: <br />
	Melee Damage <br />
	Spell Damage + Increased Spell Hits <br />
	Defense <br />
	Regenerate Hearts <br />
	Subweapon Damage <br />
	Subweapon Crit Chance <br />
	Increased Momentum Gain <br />
	Increased Max Momentum Speed <br />
	Damage while Faceplanting <br />
	Reduced Faceplant Conversion Costs <br />
	Reduced Fireball Spell Costs <br />
	Increased HP/MP/Hearts Max <br />
	Additional Items / Consumables <br />
	
The following weapons have additional effects while held in hand:  <br />
	Mourneblade: Heals on kill  <br />
	Shield Rod: Grants Greatly boosted damage for spells / subweapons  <br />
	Mablung Sword: Grants Greatly boosted damage for spells / subweapons  <br />
	Jewel Sword: Randomly spawns a jewel on kill <br />
	Gurthang: Temporarily increases damage after casting fireball (Right Hand Only) <br />

The following armors also bestow additional effects while held in inventory (not equipped): <br />
	Spike breaker: Automatic immunity to spikes <br />
	Healing Mail: Regenerate HP when moving / being hit <br />
	Brilliant Mail: Increases subweapon damage <br />
	Mojo Mail: Increases spell damage <br />
	Walk Armor: Increased granted Def Value in Castle 2 <br />

New Subweapon damage types + costs: <br />
	Empty		  - Dark (2 Hearts) <br />
	Holy Ashes    - Poison (2 Hearts) <br />
	Agunea		  - Thunder (4 Hearts) <br />
	Dagger 		  - Cut (4 Hearts) <br />
	Axe 		  - Hit (8 Hearts) <br />
	Cross		  - Dark, Curse (12 Hearts) <br />
	Rebound Stone - Stone (12 Hearts) <br />
	Holy Water 	  - Water, Holy (16 Hearts) <br />
	Bible		  - Holy, Curse (20 Hearts) <br />
	Stop Watch    - Ice (20 Hearts) <br />

## Mechanics
It is recommended to hold down the Momentum button to build speed and either attack or jump with a neutral input to make jumps your normally couldn't make.  <br />
You can buffer jumps mid attack to cause you to automatically ascend with the attack momentum.  <br />
It is possible to death skip without relics using Faceplant + Speed Boost.  <br />
Axe Armor's jumps and I-Frames allows additional sequence break relic checks compared to Alucard.  <br />

Reminder of Vanilla Behavior: Axe Armor is immune to status ailments, big toss, and most grab attacks.  <br />

## Controls
Up - Revert To Alucard for warp / elevator functionality <br />
Down - Revert To Crouched Alucard for dodging, warp, elevator functionality <br />

L1 - Faceplant (4mp to initiate, drains MP over time): Grants I-Frames for whole duration. <br />
R1 - Glide (drains MP over time) <br />
R2 - Momentum <br />

Square -  Swings Axe + Use held items (Note: Weapons now impact axe armor damage + damage type) <br />
Triangle - Elemental Hellfire (24 MP) <br />
	+ Neutral: Ice  <br />
	+ Up:      Fire <br />
	+ Down:    Lightning <br />
Circle - Use subweapons / held items <br />
X / Cross - Double Jump (upgrades to Triple Jump with Leap Stone) <br />

Unique Commands (Momentum): <br />
Left / Right: Build up stored speed. If you collide into an enemy, you will be damage boosted in the held direction with high speed. <br />
Neutral - Slide or move in the air with stored speed. <br />
Square - Now moves with stored speed. <br />
Up + Jump - Gravity Jump (requires Gravity Boots, can be chained if user has LeapStone/SoulOfWolf/FormOfMist/Attacks with a Thrust Sword in mid-air) <br />

Unique Commands (while Gliding): <br />
Up (Requires Bat): Flight <br />
Triangle   - Small fireballs that can hit breakable ceiling / floor tiles (18MP) <br />
	+ Up   - Small fireballs shifted upwards <br />
	+ Down - Small fireballs shfited downwards <br />

Unique Commands (while Faceplanting): <br />
Landing - Cancels Faceplant <br />
Hold Up - Goes through mist gates, Olrox narrow passageways (requires Form of Mist) <br />
Square - MP to Speed Boost / Aerial Faceplant extension <br />
Triangle - Hearts to MP Conversion <br />
Circle - Subweapon to HP + Hearts Conversion <br />
R1 - Flight <br />
X / Cross - Cancels Faceplant with a jump (requires LeapStone) <br />
Up + Jump - Cancels Faceplant Gravity Jump  (requires Gravity Boots) <br />

Unique Command (while in hitstun):<br />
Tap R1 when near the ground to "tech" the landing and act faster. <br />

## Damage Formulas
Note: Latest patch notes will always have the most accurate details.

Small Fireballs: <br />
	Damage = 6 + .24 Base + .36 Weapon INT + .36 Equipment INT <br />
	Soul of Bat, Echo of Bat, Force of Echo: <br />
		+ .04 Base INT Scaling  <br />
		+ .06 Equipment INT   <br />
		+ .06 Weapon INT   <br />
	Fire of Bat: <br />
		+ .08 Base INT  <br />
		+ .12 Equipment INT  <br />
		+ .12 Weapon INT  <br />
	Eye of Vlad, Mojo Mail: <br />
		+ 8 Base Damage  <br />

Big Fireballs: <br />
	Damage = 12 + .30 Base + .35 Weapon INT + .42 Equipment INT <br />
	Soul of Bat, Echo of Bat, Force of Echo: <br />
		+ .05 Base INT Scaling <br />
		+ .07 Equipment INT <br />
		+ .07 Weapon INT <br />
	Fire of Bat: <br />
		+ .10 Base INT  <br />
		+ .14 Equipment INT  <br />
		+ .14 Weapon INT  <br />
	Eye of Vlad, Mojo Mail: <br />
		+ 12 Base Damage  <br />

Subweapons: <br />
	Highest Int: Higher of Base Int or Equipment INT <br />
	Lowest Int: Lower of Base Int or Equipment INT, + Weapon INT <br />
	Damage Formula: (Relic Damage + 9 + (Highest INT * .7) + (Lowest INT * .3)) * Subweapon Multiplier <br />

Subweapon Multipliers:<br />
	Empty: .25 <br />
	Holy Ashes: .9 <br />
	Agunea: .4 <br />
	Dagger: 1.2 <br />
	Axe: 1 <br />
	Cross: 1.1 <br />
	Rebound Stone: 1.4 <br />
	Holy Water: .5 <br />
	Bible: .5 <br />
	Stop Watch: .5 <br />


Melee Weapon Damage Formula: (Main Hand Scaling * Hit Multiplier) - Hit Penalties - Hit STR Penalty + Offhand Scaling + Tooth of Vlad Scaling Special <br /> 

Tooth of Vlad: <br />
2H: 						+.280 Base STR <br />
Weapons (per hand):		 	+.105 Base STR <br />
Shields/Unarmed (per hand): +.070 Base STR <br />

Main Hand Scaling: <br />
2H Weapon: 	6 + 0.75 ATK1 + .67 Base STR + .50 Equipment STR <br />
DMG Shield: 3 + 0.75 ATK1 + .67 Base STR + .50 Equipment STR + .15 Shield INT <br />
1H Weapon: 	3 + 0.75 ATK1 + .67 Base STR + .50 Equipment STR <br />
Fist: 		1 + 0.75 ATK1 + .67 Base STR + .50 Equipment STR <br />
Shield: 	1 + 0.00 ATK1 + .50 Base STR + .50 Equipment STR + .15 Shield INT <br />

Offhand Scaling: <br />
2H Weapon:	3 + .35 Weapon ATK 2 + .33 Base STR +. 25 Equipment STR <br />
DMG Shield: 2 + .35 Weapon ATK 2 + .08 Base STR + .25 Equipment STR + .15 Shield INT <br />
1H Weapon:	2 + .35 Weapon ATK 2 + .08 Base STR + .25 Equipment STR <br />
Fist		1 + .00 Weapon ATK 2 + .08 Base STR + .25 Equipment STR <br />
Shield: 	1 + .00 Weapon ATK 2 + .08 Base STR + .25 Equipment STR + .15 Shield INT <br />

Special: Shield Rod / Mablung Sword gains additional base damage in addition to increasing Shield INT by 60. <br />

Current #Hit Damage Scaling rules: <br />
	x1 Hit: 1.0 WPN ATK, -4 Flat Damage <br />
	x2 Hit: .85 WPN ATK, -5 Flat Damage, -.15 STR Bonus <br />
	x3 Hit: .78 WPN ATK, -6 Flat Damage, -.22 STR Bonus <br />
	x4 Hit: .70 WPN ATK, -7 Flat Damage, -.30 STR Bonus <br />

## Shield INT Bonuses
+6 INT: Alucart Shield
+8 INT: Leather Shield
+12 INT: Knight Shield, Iron Shield
+16 INT: Skull Shield, Medusa Shield, Dark Shield
+20 INT: Fire Shield, Herald Shield
+24 INT: Shaman Shield, Goddess Shield
+28 INT: AxeLord Shield
+32 INT: Alucard Shield


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
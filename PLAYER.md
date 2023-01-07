# Player
[Back to README](./README.md)

## General Player Controller
Most player movement is controlled by scripting, due to a lack of familiarity with animation on my part. The majority of this is handled by the [PlayerControl](./Spamocalypse%20Infiltration/Assets/Scripts/Player/PlayerControl.cs) class, with the player's inventory and map handled by other classes.

### Inventory
The [Inventory](./Spamocalypse%20Infiltration/Assets/Scripts/Player/Inventory.cs) class manages the player's weapons, tools, and loot. This includes actually attacking.

### PlayerMap
The [PlayerMap](./Spamocalypse%20Infiltration/Assets/Scripts/Player/PlayerMap.cs) class displays an image of the current level's map or maps. If there is more than one for the level, buttons to cycle to the next image are visible.


## Player Interaction Highlighting
This is an adaptation/extension of [one of my other repos](https://github.com/aceade/Unity-Player-Highlighting/), where I attempted to replicate Thief's highlighting system of making objects glow when the player is looking directly at them.

The core of this is the [PlayerInteraction](./Spamocalypse%20Infiltration/Assets/Scripts/Player/Interactions/PlayerInteraction.cs) abstract class. This handles collision detection with the player, highlighting the object the player should look at, and freezing/unfreezing the player if necessary. Subclasses include the following:
- [ToggleLights](./Spamocalypse%20Infiltration/Assets/Scripts/Player/Interactions/ToggleLights.cs). Toggles one or more lights, updating the light detection system to go with it.
- [ReadItem](./Spamocalypse%20Infiltration/Assets/Scripts/Player/Interactions/ReadItem.cs). Displays an image of a page with text; the player's movement and rotation are frozen while doing this. The game is also paused while this happens.
- [Safe](./Spamocalypse%20Infiltration/Assets/Scripts/Player/Interactions/Safe.cs). Interacting with this would pause time, movement and rotation, allowing the player to use the keypad. Safe codes were inevitably written down on a note somewhere in the level.

## Player effects
This includes the following:
- A NightVision effect that came from the now-defunct Unity3D community wiki.
- An InvertedColours effect that was based off a shader from the now-defunct Unity3D community wiki.
- A DevConsole that allowed me to enable/disable certain features for testing, as well as use some third-party colour blindness simulators to check if the user interface was at least usable. The production version of this also allowed the InvertedColours effect or a Big Head Mode (because that is always funny).

### Sound effects
The sound effects for the player were limited to the following:
- Footsteps.
- Breathing when sprinting.
- Taking damage or dying.
- A briefing monologue for each level.
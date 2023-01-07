# Spamocalypse: Infiltration
A remake of a stealth-based game that involves avoiding a cult of spammers

[Spamocalypse: Aftermath](https://gamejolt.com/games/spamocalypse-aftermath/198961) is a game I released in 2017, spending 2 years of my spare time on it. It was a first-person stealth-based game in the style of Thief/Thief 2, where the player was a thief hired to steal ~The Utah Teapot~ The Huta Teapot from a museum that had been overrun by a deranged cult of spammers known as the Word of Turscar. Turscar is totally not the Irish Gaelic word for spam or rotten seaweed.

I was a fairly junior software engineer when I started it, and my old code shows:
- No source control
- Unused code
- No testing beyond manual testing
- Threading (for saving/loading games) relied on a third-party plugin from a tutorial site that eventually disappeared.

However, I learned a lot on it. So, I decided to redo it and update the graphics several years later.


## Existing Mechanisms
The core concept was, and still is, that the player has to stick to the shadows and move as quietly as possible. To this end, I spent some time trying to get sound and line-of-sight detection to work. Those have been documented in [DETECTION](./DETECTION.MD). Player-specific code is documented in [PLAYER](./PLAYER.md).

### NPC AI
The enemy NPCs could be broken down into the following types:
- [Spammer](./Spamocalypse%20Infiltration/Assets/Scripts/AI/SpammerFSM.cs). The base class. Basically zombies that lurched around looking for a non-spammer to vomit on. If they heard a decoy, they would investigate, stare at it a bit, and then look elsewhere for a bit. After a certain number of false attempts, they would ignore the decoy and search elsewhere.
- [Spambot](./Spamocalypse%20Infiltration/Assets/Scripts/AI/Spambot.cs). Even less intelligent than the spammers, but far tankier and possessing a search light.
- [Spammerised Moderator](./Spamocalypse%20Infiltration/Assets/Scripts/AI/Moderator.cs). Former moderators who had been forcibly turned, and retained enough intelligence to trace a decoy's launch position. They were also immune to Firewalls.

I had originally planned to include the following, but never did:
- Phishers. Rooptop-dwelling Gremlins that would sit and wait for their phishing rods to snare the player, and then sound a general alert to everyone. Partially inspired by the Barnacles of Half-Life.
- The Washing Machine Guy. Inspired by a particularly obnoxious spammer who kept clogging up the Unity development forums back in 2014-2015 with posts about washing machine repairs. This guy would have served as a boss, and be sufficiently aware of the meaning of SockPuppets to hurl an exploding washing machine into the darkest corners of the room in an attempt to flush out the player.

Bots could be in one of the following states:
- Idle
- Patrolling
- Searching
- Flamed. They appear to have burst into flames.
- Dead. The game manager would respawn them after a certain delay (since spammers _will_ create new accounts).
- Bamboozled. After being hit by a logic bomb (a flashbang analogue), they would be too frazzled to do anything for a bit.

### Sound Effects
Sound effects for the AI were generally one of the following:
- Their voice.
- Any movement noises. This only really applied to the Spambot class, as they were visually a giant robot.

Voice lines were originally "voiced" by a text-to-speech program with particular accents (Scottish, Northern England, or just plain old robotic) and a robotic effect applied in Audacity. The lines could be broken down into the following categories, based on their current state:
- Idle/patrolling sounds
- Saw something dodgy
- Heard something dodgy
- Realising they had been tricked by a decoy
- Attacking lines
- Attacked
- Dying
- Bumped into the player (e.g. "What the?!?")
- Found the player without bumping into them.

These were defined using Unity's ScriptableObject class, and can be found in the [SpammerVoice](./Spamocalypse%20Infiltration/Assets/Scripts/AI/SpammerVoice.cs) class. This allows the voice clips to be defined once and then shared across multiple GameObjects.

## New Mechanisms
TODO

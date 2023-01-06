# Spamocalypse: Infiltration
A remake of a stealth-based game that involves avoiding a cult of spammers

[Spamocalypse: Aftermath](https://gamejolt.com/games/spamocalypse-aftermath/198961) is a game I released in 2017, spending 2 years of my spare time on it. It was a first-person stealth-based game in the style of Thief/Thief 2, where the player was a thief hired to steal ~The Utah Teapot~ The Huta Teapot from a museum that had been overrun by a deranged cult of spammers known as the Word of Turscar. Turscar is totally not the Irish Gaelic word for spam or rotten seaweed.

I was a fairly junior software engineer when I started it, and my old code shows:
- No source control
- Unused code
- No testing beyond manual testing
- Threading (for saving/loading games) relied on a third-party plugin from a tutorial site that eventually disappeared.

However, I learned a lot on it. So, I decided to redo it and update the graphics several years later.


## Existing mechanisms
The core concept was, and still is, that the player has to stick to the shadows and move as quietly as possible. To this end, I spent some time trying to get sound and line-of-sight detection to work.

### Line-of-sight
Line of sight (handled in [the LineOfSight.cs class](./Spamocalypse%20Infiltration/Assets/Scripts/AI/LineOfSight.cs)) works as follows:
1. Each spambot/spammer/spammerised Moderator has an invisible box attached to their head, facing forwards. In practice, I found it best to use CapsuleColliders for this, due to their relative speed of processing compared to e.g. BoxColliders or MeshColliders, but allowing a defined area for line-of-sight unlike SphereColliders.
2. When an object enters the LineOfSight collider:
    a. Check if it is the player, a fellow spammer, or a corpse.
    b. If so, are we already tracking it?
    c. If not, add to a list of objects to be tracked.
    d. If this is the first time we are tracking anything, start the tracking loop.
3. During the tracking loop, for each entry:
    - Cast a line towards it using Unity's Physics.Linecast API. If the Linecast hits an object, compare to the object we are tracking.
        - If the Linecast's hit object matches what we are tracking, then we have line of sight.
            - Check if the lighting around this object matches or exceeds our minimum light sensitivity. If it's bright enough for us to see the object:
                - Check if this is the player. If so, increase the detection level in proportion to the game difficulty.
                    - If the detection level reaches a certain level (e.g 60%), alert the brain of a possible sighting
                    - The the detection level reaches 100%, alert the brain of a confirmed sighting.
                - If this is a corpse _and_ we are less than 5 metres away, alert the brain immediately. _A fellow Turscarite hath been felled!_
4. If the object leaves our detection collider:
    - Check if it's the player. If so:
        - Wait two seconds before starting to decrease the detection certainty.
    - If not, remove from consideration.
    - If there are no colliders left to consider, stop the tracking loop.

### Lighting calculation
Lighting originally used a ludicrously complicated mechanism where I wrote a custom pathfinding script based on what I did for my Master's thesis. In short, I built my own pathfinding system where each node had a potential illumination value, and the AI would query that. It worked, but was horrifically unoptimised and caused the potato laptop I used to run out of memory a lot.

The current mechanism is to use the built-in collision system, in a similar manner to regular line-of-sight. Any Light component in a scene apart from the sun includes a Collider. If something that implements the [ICalculateLight](/Spamocalypse%20Infiltration/Assets/Scripts/Lighting/ICalculateLight.cs) interface enters the Collider, the Light adds itself to the entity's list of current lights and begins to process it. Processing the light source adds lighting values according to the type of light:
- Spotlights have no intensity falloff to compensate for their shorter range. This means that their contribution towards being spotted is fixed; if you stand in a torch beam, you are going to be noticed!
- Point lights (lights which emit in all directions) will contribute according to the distance from the player. So, don't stand under a streetlamp!

The [Sun](/Spamocalypse%20Infiltration/Assets/Scripts/Lighting/Sun.cs) (or moon) works in a slightly different manner:
- It requires an assigned list of ICalculateLight instances, though it can gather them from the scene.
- At each physics timestep, check if it can see each of them by casting a Ray towards them. If the ray hits the object (instead of a building), then it can.
- If it can, increment their illumination level by a fixed amount.

#### How this is shown to the player
For the benefit of the player, a "compass" in the bottom of their screen includes a "light gem", similar to Thief/Thief 2. This glows with a certain intensity according to the player's current light value, and the emission value is read every 1/2 a second.

### Sound detection
Sound detection relies on a SphereCollider being attached to each entity. SphereColliders are the quickest collider for the physics engine to handle, and since sound isn't as directionally constrained as line-of-sight, they make perfect sense here. When a sound source enters the collider:
- Check if it is tagged as a decoy or is in a list of alert noises (e.g. another spammer's death cry). If so, alert the brain immediately.- If it is the player:
    - Ignore if there is something more pressing to investigate (a decoy, an ally dying, etc).
    - If not already alerted, increment the detection certainty if their footstep noise has increased (e.g. because they have come closer or have started running)
    - If the player's detection certainty reaches a certain level, alert the brain of a possible sighting.
    - If the detection certainty reaches 100%, alert the brain of a confirmed sighting.

Alert noises consisted of the following:
- Rustling bushes. The player could avoid this by moving very slowly.
- Discarded bottles being knocked over. This could easily be extended to cover other detritus being disturbed.
- Windows being broken (unless the player used their knife to do this...for gameplay reasons).
- Spammer death noises.
- Decoys/Sockpuppets.



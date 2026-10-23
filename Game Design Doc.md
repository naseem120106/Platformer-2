# Game Design Document: Untitled Underwater Platformer

## 1. Summary
The player takes on the role of an
underwater explorer in search for
treasure in this first-person 3D
platformer. They have to use platforms
and underwater creatures to navigate
the depths, avoid the obstacles and
hazards, and find all of the hidden
treasures. How fast can they complete
the levels, and how far can they get? (copied from the game concept 2 pdf)

## 2. Planned Features

### Required Features
1. Movement (REQUIRED)
   1. Move fowards/backwards
   2. Rotate left/right
   3. Strafe left/right
   4. Crouch
   5. Jump
   6. Sprint
   7. Movement suggests water resistance 
   8. No swimming

2. Levels (REQUIRED)
   1. At least 2 levels with different challenges for the player. 
   2. End of level 2: player enters area where oxygen tank breaks and must look for air bubbles to survive

3. Platform types - just examples (REQUIRED)
   1. Solid
   2. Wobbly/slippery
   3. One-time use jump pads
   4. Moving
   
4. Trap types - just examples (REQUIRED)
   1. Spiky fish
   2. Exploding fish
   3. Sticky trash (adds weight to player or affects vision)

5. Checkpoints (REQUIRED)
   1. Start position is default checkpoint
   2. Default checkpoint updated if player gets to next checkpoint
   3. Resets player postion to last checkpoint if they fall off platforms or get harmed by traps

6. Treasure (REQUIRED)
   1. Treasure pieces scattered around the level
   2. Update score and treasure count when collected
   3. Player progresses to the next level if they collect all, and they win if they finish the last level
   4. Optional treasure pieces for bonus points appears in hard to reach places
   5. Show score and treasure count in UI (i think its better to do score and treasure count, so if they fall that could reduce their total score, but it will not change treasure count) - also they dont say to show in hud, but im unsure if this should be put in important changes to required features

7. Creatures (REQUIRED)
   1. Swimming creatures controlled by AI (just basic movement from them)
   2. Helpful creatures: helps move between platforms, bring treasure etc
   3. Harmful creatures: hurts the player and adds danger that the player must avoid
   4. Stealth occasionally required to avoid detection and dodge attacks of large creatures

8. Evolving levels (REQUIRED)
   1. Track player performance
   2. The game gets harder or easier depending on player performance 

9. Additional Features (needs to be checked with staff next week)
   1. timer to track how quickly the player finishes a level as mentioned in summary about fast they are
   2. HUD: treasure counter, score, timer, last checkpoint status/update, oxygen level
   3. Main menu and pause menu
   4. Double jump: player can jump up to 2 times in a row until they touch the ground again (ADDITIONAL, unapproved)
   5. Sprint stamina: sprinting drains a stamina bar, and the player must recover some stamina before sprinting again (ADDITIONAL, unapproved)
   6. Sprint feedback: "SPRINTING" text, widened field of view and a screen tint while sprinting (ADDITIONAL, unapproved)

once these are approved they will be changed to the same format as the required features section and marked with "(ADDITIONAL)"
they also say nothing about the camera, but i guess we can just assume its first person since im pretty sure hes mentioned this before in a lecture, until we ask staff - i can prob check on monday before/after the lecture or in lab

### Important changes to required features
1. none at the moment, but if there is any they must be approved by our allocated member of staff, also not sure if score counts as a change since they literally mention points in the treasure section

### Core Features (First prototype, CW1)

They say this in the lab 4 pdf:
Assign each feature to one group member who will be mainly responsible for it (but not exclusively so;
several can collaborate for one feature, but one will be responsible for it being finished and integrated into
the game). 
<br>
<br>

|ID|Core Feature| responsible|
|---|---|---|
|C1| Movement with water resistance feel (can do this on a normal plane) | Naseem |
|C2|One test level with solid platform and one other platform type| |
|C3|Treasure on the map, updates counter and does win message when all is collected | |
|C4| checkpoint respawn | |

### Prototype status

|ID|Status|Notes|
|---|---|---|
|C1|Done|Water-feel movement, sprint, crouch, jump (double jump as an additional feature). Zero-friction player collider so the movement script controls speed.|
|C2|Done|Solid platforms and moving platforms. Moving platforms carry the player while they stand on them.|
|C3|Done|Treasure counter and score in the HUD, "You win!" message when all treasure is collected.|
|C4|Done|Start position is the default checkpoint, touching a checkpoint updates it, falling below the death height respawns the player there.|

Pretty much everything depends on c1 being done first

also im pretty sure camera must be set up first before anything really but not sure if that counts as a core feature

The hud should also be done if we manage to do them 4 features quickly i think



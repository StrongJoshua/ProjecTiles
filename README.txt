Rishi Raj rraj9@gatech.edu rraj9
Jan Risse jrisse3@gatech.edu jrisse3
David Riazati david.riazati@gatech.edu driazati3
Aaron Andrews aandrews34@gatech.edu aandrews34

GitHub Repository: https://github.com/StrongJoshua/ProjecTiles

Installation
-------------------------------------------------
Run the EXE
To view scenes in Unity, start with the MainMenu scene

Gameplay Instructions
-------------------------------------------------
Controls below are for an Xbox controller
Use the left joystick to navigate the menus and map.
Use A to confirm selections or open menus when a unit is selected
Use B to back out of menus or selections
Use the triggers to quickly switch between enemies
Use the start button to access the pause menu

View the options menu in-game to view the controls

Rubric
-------------------------------------------------
Enter the main game area to observe most of the requirements: 
Game Feel
Feels good.
Objective/Goal?
Defeat all enemies in each level. Player can easily track their progress through the enemy counter in the top right and view their units’ status through the HUD.
Communication of success
When a player defeats all enemies on a map they are presented with a victory screen with the option to continue onto the next level. If they complete the final level, they are presented a game completion screen.
Start menu
Upon starting the game the player is presented with tutorial screens after which they are shown the game’s main menu.
Reset and replay on failure
When all of the player’s units die, the player is presented with a “Game Over” screen where they can choose to retry the level.
Interesting choices with consequences?
Players have to decide how to balance usage of their AP resource between moving, shooting, and activating special attacks.
Player can engage with the world
There are various environmental objects that can be interacted with, such as an exploding barrel, a falling pillar, and destructible rocks.
Gameplay is balanced?
Players can level up their units, while enemies are left with preset stats.
Rewards success and punishes failure
Upon dealing damage to or defeating an enemy, a player’s units gain experience, which causes them to level up, increasing their stats. However, if a player’s units are defeated, they are defeated for good and remain dead if the level is completed (they are revived if the level is reattempted).
In-game learning and training?
Before seeing the main menu, the player is presented with tutorial screens which teaches them the mechanics. The training occurs as the levels progress.
Progression of difficulty?
Each new map has more enemies and a larger map to traverse.
Mecanim, blendtree, player controlled character?
The drone character (the special move from the Mech) moves freely through the map and can be used for high damage. It is completely mecanim and root motion based. The units also use mecanim for shooting and death, where the animation is available.
Controls are intuitive and appropriate
Controls are simple: 2 buttons (accept/open menu and cancel) and joystick
Fluid animation?
Animations have smooth transitions done through animator controllers
Camera is smooth?
We experienced consistent framerates of 60 FPS while testing the final game
Camera has no clipping?
Camera only can move where the player can select, avoiding any clipping  issues
Auditory feedback?
Projectiles have sounds for shooting and hitting enemies.
There are various sounds for the UI and in-game HUD.
Animation callback events?
Shooting and special animations employ callbacks that activate certain effects, such as launching a projectile to provide feedback at the visually correct time.
Synthesized new level, not an asset?
Levels are built from tmx files at runtime which define the layout of the tiles. Each tile is a prefab with various characteristics that affect the units passing over them.
Graphically and auditory representation of physical representations?
All characters have footstep, or other applicable movement sounds. Each unit state has an animation and associated animation events that trigger audio feedback. Projectiles (lasers, bombs, etc.) all have sounds as well.
Bounds player to playable space?
Player is constrained to dynamically constructed tilemap
Environment interaction? Scripted objects? Rigid bodies? Animated objects
Environment contains destructible barrels, boulders, and pillars. Pillars fall opposite of the direction they are shot and cause damage. Barrels explode on being shot and cause damage to surrounding units.
Rigid bodies with 6 degrees of simulation?
Destructible boulders release physics simulated rocks when destroyed. Rocks will damage units if they hit. The destructible pillars also move and fly in all dimensions when they are detached from their base.
Interactive environment
Environment has destructible boulders and falling pillars that will damage units. Exploding barrels will also cause damage when shot
Consistent spatial sim? Running speed remains same regardless of framerate
Framerate has no effect on gameplay.
Multiple AI states
Enemies have aggro states: move towards the player, move towards the player if the player units are close, or don’t move and only attack when enemies are in range.
Smooth locomotion of AI
Predominantly root motion for humanoids
The drone special demonstrates root motion as well as a fully blended mecanim tree
Believable and effective AI?
The AI does its best to defeat the player, acting as an opposing player would (with respect to each units’ aggro of course).
Fluid animation for AI
The AI units moves just as player units move.
Sensory feedback for AI state
The AI units’ aggros can be seen in the unit info window when the tile they are standing on is highlighted.
Difficulty against players appropriate? Is it fair?
AI skill and decision speed was adjusted after playtesting to address difficulty concerns.
Ai takes advantage of environment?
AI will blow up barrels on the map to damage the player.
UI is good?
Consistent visual style and custom made buttons
No debug output? No dev mode abilities?
All debug features are only accessible through the Unity editor environment, the build EXE has none.
Can exit anytime?
Main menu has exit button “quit,” in-game pause menu has “quit” button
Transitions between scenes is smooth?
Main menu has animations between screens. Level loading happens without visual glitches.
Environment acknowledges player
Proximity, surface changes, particle effects, audio
Artistic style?
Consistent low poly models (some manually edited)
No glitches? Escaping game world? Getting stuck? Stable?
We fixed all bugs we found in playtesting and did our own quality assurance runs once we finished the game
Fun?
Feedback from playtesters was positive and we have tuned the final game to their suggestions


Known Bugs
No turn in place for the mech drone

Resources used
Music by Waterflame and 95TurboSol
3D models from Poly by Google contributors and /u/QuaterniusDev on Reddit
Sounds from freesound.org

Jan Risse: Maps, UI, Shading, AI, Movement

Rishi Raj: Models, Camera, Sound, Projectiles, Tiles, Particle Effects, Video

Aaron Andrews: Animation, Projectiles, Environment Effects

David Riazati: UI, Animation, Environment Effects

Open MainMenu.unity to launch the main menu for the game.



  



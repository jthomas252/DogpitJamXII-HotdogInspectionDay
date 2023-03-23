# Models To Do (Priority is top to bottom)
- Rat model

- CRT Monitor
    - Screen should accept a separate material 

- Digital clock body
	- Will use a viewport texture to control the on-screen time

- Geiger counter

- Environment models
	- Optional, we can use the environment currently in Godot instead
    - Should include some basic enviromental details in the distance to give a sense of depth
    - Possibly add meathooks or small animation in the background 

- Cardboard box
    - Used if we do the secret condition 
    - Should be just large enough to fit a hotdog in 

- Arbitrary models (non-hotdogs)
    - Human finger
    - Banana
    - Dynamite
    - Bone
	- Non-interactives to leave on the desk 

- Cleaning bottle
    - Potential mechanic, low priority
	- Would need a particle animation
	- Only could be used to clean dirt 

# Interface
- Add credits

- Add between level screen 
    - Have text here to tell the player how they did

# Level Iteration
- Unlock new mechanics based on level progression

- Show screen between levels telling the player how they did

- Reset hotdogs on screen, item states, and spawner count
	- Use EmitSignal here

# Textures
- Add warning label decals

- Add scanning area decal

- Add spawn area decal

# Scoring
- Iterate a score in BaseScene

- Display Quota on the Computer Screen

- Issue citations if  >3 valid hotdogs are tossed in the trash

# Hotdog Generation
- Set up meats randomizer in Hotdog creation
	- Use random selection for non-trigger ones
	- Have list of good, okay, and bad ingredients
	- Representation of the quality score to determine failure / success

- Generate a radioactivity score (scale for failures)

- Generate a coldnesss stat (requires thawing)

- Generate mold stat and assign to the shader

# Audio
- Import new SFX 

- Add sound effects to the Hotdog to play on collision
	- Will need to put these somewhere on the rigidbody

- Set up ambient audio playback
	- Computer hum

# Instruction Pages
- Create basic instruction page explaining gameplay loop

- Create instructions page for meat selection
	- Use a score table to show which are good vs. bad 
	- Could use a datatable here to determine stats for each

- Create instructions for the geiger counter

- Create instructions for the heat lamp 

# Rat
- Add rat spawn
	- Rat is grabbable object and can be put in trash or conveyor

- Have rat calculate hotdog list

- Have rat pick up hotdog

- Have rat move back to spawn point

- Despawn rat & hotdog 

- Add rat model and animation
	- Set up animation parameters in other behaviors

# Tools
- Add secret box
	- It asks for a specific condition hotdog in a note
	- If fulfilled gives some kind of unique reward

- Add heat lamp triggers and functionality

- Add geiger counter
	- Scans nearby objects for radiation state
	- Ticks up if detecting radiation

- Add level timer
	- Display in world somewhere
	- Tick down and end the level at the threshold being reached

- Add a delay to the spawner button
	- Should allow for a few spawns before a wait time 
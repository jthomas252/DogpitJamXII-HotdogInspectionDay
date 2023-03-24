# Models To Do (Priority is top to bottom)
- CRT Monitor
    - Screen should accept a separate material 

- Geiger counter

- Uranium Rod
	- Green cylinder, just add some materials / glow to it

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
	- Non-interactives to leave on the desk, places for the rat to scurry
	- Pipes, wall objects, etc. 

- Digital clock body (Optional)

# Misc
- Need to add a seed to the random for Godot startup

- Have a "Start Shift" button, maybe on the spawner button arm
	- Trigger the spot lights to flash
	- Conveyor animation starts
	- Computer turns on
	- Gives the player a chance to read the manual or examine notices

# Interface
- Add credits

- Add between level screen 
    - Have text here to tell the player how they did

# Level Iteration
- Unlock new mechanics based on level progression

- Show screen between levels telling the player how they did

- Reset hotdogs on screen, item states, and spawner count
	- Remove the remaining hotdogs on the scene
	- Set the spawner back up to a full count
	- Reset the game timer

# Hotdog Generation
- Set up meats randomizer in Hotdog creation
	- Use random selection for non-trigger ones
	- Have list of good, okay, and bad ingredients
	- Representation of the quality score to determine failure / success

- Generate a radioactivity score (scale for failures)

# Audio
- Add sound effects to the Hotdog to play on collision
	- Will need to put these somewhere on the rigidbody

# Instruction Pages
- Create basic instruction page explaining gameplay loop

- Create instructions page for meat selection
	- Use a score table to show which are good vs. bad 
	- Could use a datatable here to determine stats for each

- Create instructions for the geiger counter
	- Explain that it will trigger when radiation is detected 
	- Explain that it can be picked up 

- Create instructions for the heat lamp 
	- Mention some hotdogs arrive frozen
	- Frozen hotdogs cant scan

- Warning page about rats 

- Export to images and apply effects

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

- Add geiger counter
	- Scans nearby objects for radiation state
	- Ticks up if detecting radiation

- Add a delay to the spawner button
	- Should allow for a few spawns before a wait time 

- Add multiple spawns to the spawner, delayed slightly 
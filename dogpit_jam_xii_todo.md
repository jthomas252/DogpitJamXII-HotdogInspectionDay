# Models To Do (Priority is top to bottom)
- Buttons with animation 
    - Should have ones that hang off the desk
    - At least one should fit the look of the spawner
- CRT Monitor
    - Screen should accept a separate material 
- Environment models
    - Can be untextured or loosely textured 
    - Should include some basic enviromental details in the distance to give a sense of depth
    - Room should follow a similar pattern to current layout in Godot with a back wall, right hand wall, opening leading into factory
    - Possibly add meathooks or small animation in the background 
- Geiger counter
- Clipboard 
    - Just the clipboard, documents exist as Sprite3D in game
- Rat model
- Cardboard box
    - Used if we do the secret condition 
    - Should be just large enough to fit a hotdog in 
- Arbitrary models (non-hotdogs)
    - Human finger
    - Banana
    - Dynamite
    - Bone
- Tesla coil
    - Attacks a rat if it wanders too close
    - Will also couple with fire sprite / burn mark 
- Cleaning bottle
    - Potential mechanic, low priority

# Interface
- Add start screen interface 

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

# Hotdog Conveyor
- Issue a score token if success (Set up event on BaseScene)
	- Issue a positive flash on the monitor 

- Issue a penalty token if failure (Set up event on BaseScene)
	- Issue a citation on the monitor explaining the error
	- Possibly put a label on these to detail the error

# Hotdog Generation
- Set up meats randomizer in Hotdog creation
	- Use random selection for non-trigger ones
	- Have list of good, okay, and bad ingredients
	- Representation of the quality score to determine failure / success

- Generate a radioactivity score (scale for failures)

- Generate a coldnesss stat (requires thawing)

# Audio
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
- Rat repelant
	- Tesla coil that will explode a rat if it gets too close

- Add secret box
	- It asks for a specific condition hotdog in a note
	- If fulfilled gives some kind of unique reward

- Add heat lamp
	- Applies heat to objects within its radius
	- Can use Godot lighting for this

- Add geiger counter
	- Scans nearby objects for radiation state
	- Ticks up if detecting radiation

- Add level timer
	- Display in world somewhere
	- Tick down and end the level at the threshold being reached

- Add counter object to spawner
	- Should display a countdown of remaining hotdogs

# Nice to have's
- Dynamite which randomly spawns instead of hotdog
	- If it explodes trigger a gameover or physics event
	- If sent up the conveyor triggers a unique ending?

- Add secret items
	- Scale that tells you the Hotdog quality
	- Only active when something is "sacrificed" 
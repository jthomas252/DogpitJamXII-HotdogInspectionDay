# Models To Do (Priority is top to bottom)
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
    - Dynamite
    - Bone
	- Non-interactives to leave on the desk, places for the rat to scurry
	- Pipes, wall objects, etc. 

# Builds
- Test and make builds for Web

- Test and make builds for Windows

- Test and make builds for Mac

# Misc
- Fix problems with rat moonwalking

- Fix issue where inspected objects have arbitrary Y position differences
	- Noticeable on poster vs. handbook
	- Ideally set these to write to a sprite in the inspect area 

- Have a "Start Shift" button, maybe on the spawner button arm
	- Trigger the spot lights to flash
	- Conveyor animation starts
	- Computer turns on
	- Gives the player a chance to read the manual or examine notices

- End and reset the level when the timer hits zero

# Sound setup
- Play title theme on the start screen

- Play in-game music 

- Play title theme on the between-days pop-up

- Add sound FX to the Hotdog to play on collisions 
	- Use an array of the different SFX
	- Modulate volume based on the impacts velocity 

- Add geiger clicks 
	- Increase the effect # when the radiation level is higher

# Interface
- Make the start screen start the game
	- Try to do a fadeout or something
	- Trigger the initial day text 

- Add between level screen 
    - Have text here to tell the player how they did
	- Play music here

- Scanning noise when Hotdog is sent through

- Spawner noise when out of hotdogs 

- Spawner specific button sounds

# Level Iteration
- Unlock new mechanics based on level progression
	- Day 1
		- Inspect serial numbers
		- Inspect for mold
		- Inspect for meat quality (basic)
	- Day 2
		- Rat spawns
		- Hotdogs arrive frozen
		- Random objects from spawner (non-dangerous)
	- Day 3
		- Meat quality adds more checks
		- Hotdogs arrive radioactive
	- Day 4
		- Dangerous random objects come from the spawner (dynamite, uranium rods, etc.)
		- Rats spawn more aggressively and move faster
	- Day 5
		- Ending / things devolve into chaos 
		- Final screen detailing how the factory was shut down due to excessive rats 

- Show screen between levels telling the player how they did
	- Let them know if they fulfilled quota
	- Fulfilled secret item
	- Number of citations issued 

- Reset hotdogs on screen, item states, and spawner count
	- Remove the remaining hotdogs on the scene
	- Set the spawner back up to a full count
	- Reset the game timer

# Hotdog Generation
- Set up meats randomizer in Hotdog creation
	- Use random selection for non-trigger ones
	- Have list of good, okay, and bad ingredients
	- Representation of the quality score to determine failure / success

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

- Remove the colliders for the additional pages 
	- Only need one with the main rulebook page

- Add credits poster

# Tools
- Add geiger counter
	- Scans nearby objects for radiation state
	- Ticks up if detecting radiation

- Add a delay to the spawner button
	- Should allow for a few spawns before a wait time 

- Add multiple spawns to the spawner, delayed slightly 

# Nice to have
- Add rotation to the snap points
	- Lerp into the appropriate position 

- Toggle the light on the spawner button

- Add gamepad support
	- Move a virtual cursor instead of the mouse one

- Glitch dogs
	- Randomly mess with a vertex shader
	- If scanned, they break the scanner for a period of time

- Improve the next/prev buttons 
	- Should probably just be generic arrows 

- General code cleanup
	- Move stuff to emitters and signals where it can be
	- Keep a consistent pattern

- Add secret box
	- It asks for a specific condition hotdog in a note
	- If fulfilled gives some kind of unique reward

- Make an ending segment
	- Newspaper article detailing how the factory was shut down
	- Have the facility be run down or full of rats

- Kill floor for stuff that falls off the desk
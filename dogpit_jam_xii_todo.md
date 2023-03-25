# Models To Do (Priority is top to bottom)
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

# Misc
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

# Interface
- Make the start screen start the game
	- Try to do a fadeout or something
	- Trigger the initial day text 

- Add between level screen 
    - Have text here to tell the player how they did
	- Play music here

- Spawner noise when out of hotdogs 

# Level Iteration
- Unlock new mechanics based on level progression

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

# Tools
- Add a delay to the spawner button
	- Should allow for a few spawns before a wait time 

- Add multiple spawns to the spawner, delayed slightly 

# Nice to have
- Fix problems with rat moonwalking

- Overlapping sounds for the geiger counter

- Apply radiation from the rod to nearby hotdogs	

- Add rotation to the snap points
	- Lerp into the appropriate position 

- Toggle the light on the spawner button

- Add gamepad support
	- Move a virtual cursor instead of the mouse one

- Improve the next/prev buttons 
	- Should probably just be generic arrows 

- Add secret box
	- It asks for a specific condition hotdog in a note
	- If fulfilled gives some kind of unique reward

- Make an ending segment
	- Newspaper article detailing how the factory was shut down
	- Have the facility be run down or full of rats

- Kill floor for stuff that falls off the desk

- Fix issue where inspected objects have arbitrary Y position differences
	- Noticeable on poster vs. handbook
	- Ideally set these to write to a sprite in the inspect area 
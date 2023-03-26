# Models To Do (Priority is top to bottom)
- Arbitrary models (non-hotdogs)
    - Human finger
    - Bone
	- Non-interactives to leave on the desk, places for the rat to scurry
	- Pipes, wall objects, etc. 

# Misc
- Have a "Start Shift" button, maybe on the spawner button arm
	- Trigger the spot lights to flash
	- Conveyor animation starts
	- Computer turns on
	- Gives the player a chance to read the manual or examine notices

- Spawn random objects 

# Sound setup
- Play title theme on the between-days pop-up

# Interface
- Make the start screen start the game
	- Try to do a fadeout or something
	- Trigger the initial day text 

# Level Iteration
- Set rat spawns to change on Level 2+

- Set spawner contents to change on Level 2+ 

- Show screen between levels telling the player how they did
	- Let them know if they fulfilled quota
	- Fulfilled secret item
	- Number of citations issued 

- Reset hotdogs on screen, item states, and spawner count
	- Remove the remaining hotdogs on the scene
	- Set the spawner back up to a full count
	- Reset the game timer
	- Use the LevelEnd / LevelStart signals on BaseScene for this 

# Tools
- Add a delay to the spawner button
	- Should allow for a few spawns before a wait time 

- Add multiple spawns to the spawner, delayed slightly 

# Nice to have
- Make the vent break open 
	- Change script to work off a signal / trigger
	- Figure out a sound to play 

- Make the start screen look nice

- Citations
	- Maybe spawn an object for this? 

- Dynamite
	- If it explodes causes a game over
	- Animation is burn_fuse, time is 1s

- Fix problems with rat moonwalking
	- Could have a child object that can run LookAt and copy the rotation from it
	- Change to use some from of DirectionTo() and SetAngularVelocity()? 

- Overlapping sounds for the geiger counter
	- Would have to create audio stream and replay it 

- Apply radiation from the rod to nearby hotdogs	
	- An area is already present on it, attach function in _Process

- Add rotation to the snap points
	- Lerp into the appropriate position 
	- Would benefit from fixing rat rotation first 

- Toggle the light on the spawner button

- Improve the next/prev buttons 
	- Should probably just be generic arrows 

- Make an ending segment
	- Newspaper article detailing how the factory was shut down
	- Have the facility be run down or full of rats
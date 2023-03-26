# Misc
- Make builds and submit to Itch.io

# Nice to have
- Make the vent break open 
	- Change script to work off a signal / trigger
	- Figure out a sound to play 

- Dynamite
	- If it explodes causes a game over
	- Animation is burn_fuse, time is 1s

- Fix problems with rat moonwalking
	- Could have a child object that can run LookAt and copy the rotation from it
	- Change to use some from of DirectionTo() and SetAngularVelocity()? 

- Apply radiation from the rod to nearby hotdogs	
	- An area is already present on it, attach function in _Process

- Add rotation to the snap points
	- Lerp into the appropriate position 
	- Would benefit from fixing rat rotation first 

- Make an ending segment
	- Have the facility be run down or full of rats
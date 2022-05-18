# unityrollback

https://www.youtube.com/watch?v=lCfouAH_N5w

Rollback netcode implementation in Unity.
Includes Fixed-Point Math implementation. 
Multithreaded deterministic game simulation and networking.
Custom networking implementation using System.sockets

	-Three Main Threads/Sections of the program (technically networking needs 2 threads)
	
	-Unity Main thread: this is the default thread Unity lets you write code on, we can access unity's built in classes and systems such as GameObjects and their physics system 
			-(although a lot of this isn't useful to us at it's non-deterministic)
			-We use Unity's (new) InputSystem to take in input from the local player
			-We use Unity at the end of the frame to render our current gamestate.
	
	-Networking thread(s) 
			- When we take in our local input we can send it off to the remote player
			- When we recieve remote input we can send it to our game simulation thread
			- Peer class looks for a TCP server at the other players IP, if it cant find one it will set up a server for the other player to find
			
	
	-Game Simlulation 
			- Stripped down version of our deterministic game
			- Uses Fixed point math for determinism
			- Need to store and load gamestates at will for rollbacks
			- Custom update loop so that we can update whenever we want
			- Gamestate/FrameInput dictionaries allow us to restore previous gamestates and apply new inputs to them - resimulating forward
			- Unity will ask for most recent gamestate before each render call (we dont need to render every frame in a rollback)
      
     

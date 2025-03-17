# Context
This is a FORK of the repository [Game Development Trial Project](https://github.com/AdriKat2022/TrialProjectUnity).  
The purpose of this fork is to make a package out of it so it can be reused in any project.

# Game Development Trial Project
Welcome to my game development trial project! This repository contains my implementations of the required tasks, demonstrating my skills in project setup, workflow management, gameplay mechanics, systems development, and maybe even custom editor tools.  
This project uses the Unity [6000.0.24f1](https://unity.com/releases/editor/archive) version.

## 📝 Overview
This trial consists of 4 tasks designed to prove my skills and train multiple areas of game development over 5 days.  
The tasks will evaluate:

- Project planning & estimation  
- Version control & workflow management  
- Problem-solving & critical thinking  
- Implementation of game mechanics & systems in the Unity engine

This project will follow a structured approach with proper documentation and respect a comprehensive commit history.

## ⏳ Task Breakdown & Time Estimation
| Task | Description | Estimated Time | Actual Time | Notes |
|-|-|-|-|-|
|Task 1	|Project setup, workflow diagram, and estimation |1h20m| 1h40m	| Editor installation took longer than expected. |
|Task 2	|Basic player controller, animations and camera |2h | 3h | I decided to spend some extra time to better refine the isometric movement and add particle effects for user feedback when moving on speed altering tiles. |
|Task 3	|Enemy AI state machine, nav mesh and animations |3h | 5h20m | NavMesh usage for 2D required some extra installation and understanding. I also greatly underestimated the task, especially that I need to rebuild the entire level to better fit an isometric world.|
|Task 4	|Extensive dialogue system, UI, branching and easy edit |4h | 5h30 | As usual, a bit underestimated. I used Knitting a very useful package that allows us to use Twine as our Dialogue editor. |
|Task 4 BIS	| Developing the dialogue system as a custom editor tool |6h | 10h | I spent a LOT of time discovering the custom editor tool environment. It was a very empowering experience, I have the impression to have learned loads of big chucks of knowledge. It sure got me time to understand it but I proud to have made this custom tool!  |

## 🎮 Gameplay Mechanics
- **Player Movement**: Use the arrow keys or WASD to move the player character around the level.
- **Speed Altering Tiles**: Some tiles will increase or decrease the player's movement speed. (Removed for now due to the map being redesigned)
- **Enemy AI**: Enemies will chase the player when close and return to patroling when the player is too far.

# Sources and Credits
## Tutorials
I used some inspiration from the following tutorials and resources to help me with the development of this project.  
It's the first time I'm working with 2D NavMesh and Isometric perspectives, so these tutorials were very helpful.  
NavMesh was especially tricky to get working in 2D since I only knew how to use it in 3D, but I managed to get it working in the end.
- **Isometric Tutorial**: [Beaver Joe](https://www.youtube.com/watch?v=ruDXAXcgqmE)
- **2D NavMesh Tutorial**: [King Dara](https://www.youtube.com/watch?v=QktWJHEJYlU)
- **Unity Dialogue System**: [Indie Wafflus](https://www.youtube.com/watch?v=nvELzBYMK1U&list=PL0yxB6cCkoWK38XT4stSztcLueJ_kTx5f&index=1)

Those were extremely helpful, especially that many areas of this project was a discovery (custom editor tools and isometric 2D).

## Packages
I used the following packages to help me with the development of this project.  
- [**Knitting**](https://github.com/Crafteurmax/Knitting): A big thanks to [CrafteurMax](https://github.com/Crafteurmax/) for this one!

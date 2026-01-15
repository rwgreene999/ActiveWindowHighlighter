# ActiveWindowHighlighter 
## This is the problem solved by this project 
* Multiple monitors
* multiple windows on each monitor 
* Dark Mode 
  
**How to quickly notice which window has focus**

Maybe this is not an issue for most people, but it is something I noticed I had a problem with after switching to dark mode. 

## Solution
* Solution create something that will highlight the current focused window
* Create this as a standalone EXE which can be copied from machine to machine or installed 
* Create this without extra installation onto the target machine 
* CoPilot said it could create this. 
* CoPilot lied 
  
## Current status
* CoPilot create the bulk of the code. 
* I argued CoPilot for 4 hours before it would compile cleanly
* We got something that sort of works. 

## Failings 
* It requires .NET 8, or expects to use it 
* CoPilot seems to have forgotten about the single EXE concept. 
* At the moment the solution is only under debug mode 
* The EXE resides in a folder with 464 items using 160 MB 
* It highlights the windows that is in focus, unless to window resizes or is moved. 

# Todos
* Required: Create single portable EXE 
* Required: Add versioning somewhere

# todos for fun 
* todo: Pick startup color at random 
* todo: create glowing colors or changing colors 
* todo: Provide option for adding to Windows Startup   

# Dones: 
* 1/15/25 BUG: FollowMovedWindows Notice when the focused window moves and relocate the highlight
* 1/15/25 Better tray icon 




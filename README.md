# Platformer-2

## how to run
1. Clone this repo
2. open project using unity hub
3. open main scene
4. press play

## make personal unity scene 
1. add this to the .gitignore:
```
   Assets/Scenes/yourname.unity
   Assets/Scenes/yourname.unity.meta
```
2. In the project bit at the bottom of the unity editor, click scenes then click main once and then use ctrl + D to duplicate it and then f2 to rename to your name (must be the same as what you wrote in .gitignore)

## play the web version
https://naseem120106.github.io/Platformer-2/build/index.html

Click inside the game to lock the mouse, press Esc to release it.

| Key | Action |
|---|---|
| W A S D / arrow keys | move |
| Mouse | look |
| Space | jump (press again in the air for a double jump) |
| Left Shift | sprint (uses stamina) |
| Left Ctrl | crouch |

## updating the web build
1. In Unity: File > Build Settings, switch the platform to WebGL, and make sure the scene to ship is ticked.
2. Build into the `build` folder at the root of this repo (replace the old contents).
3. Commit and push `build` to main. GitHub Pages serves it from main.

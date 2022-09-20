# Unity AR App

## This app will not run

You will need this paid asset from the store, then put the mobile prefabs into the Human script.

https://assetstore.unity.com/packages/3d/characters/humanoids/character-pack-office-workers-179532

## Setup HoloLens dev on Windows

It's not fun.
Follow this and keep debugging along the way:
https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/setup

## Build for HoloLens Emulator

This is soo stupid! But it works...
https://stackoverflow.com/a/71457184

Takes 20+ minutes on my laptop.

1. In Unity: Build for Universal Windows Platform
2. Open the resulting .sln file in Visual Studio
3. Pick HoloLens 2 Emulator (next to play button); then play
4. Deployment Errors. Continue? "No"
5. DO NOT CLOSE EMULATOR
6. Play again -> Runtime error -> Stop (red)
7. DO NOT CLOSE EMULATOR
8. Play again

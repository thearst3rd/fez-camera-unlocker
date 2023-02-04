# FEZ Camera Unlocker

A simple [HAT](https://github.com/Krzyhau/HAT) mod for [FEZ](https://fezgame.com) which allows you to unlock the camera and truly view the world from any perspective.

## Usage

With the mod installed, using the camera freelook input will rotate the camera around Gomez! By default, this will be the IJKL keys on keyboard and the right stick on controller. This behavior can be toggled by pressing the reset camera button, which is right shift on keyboard and clicking in the right stick on controller.

Physics behave strangely, since the game assumes that the camera is still viewing the level from its original perspective. Gomez does move along the actual axis perpedicular to the camera, so this means you can walk along diagonals which is pretty neat.

## Installation

* Follow the setup instructions for [HAT](https://github.com/Krzyhau/HAT)
* Download FezCameraUnlocker.zip from the [releases](https://git.thearst3rd.com/thearst3rd/fez-camera-unlocker/releases) page and put it in the HAT `Mods` folder
* Enjoy!

## Building

* Clone the repository
* Copy over the [dependencies](Dependencies/README.md)
* Open in Visual Studio
* Build the solution
* Create the folder `FezUnlockedCamera` in your HAT `Mods` directory and copy in the built `FezUnlockedCamera.dll` as well as [`Metadata.xml`](Metadata.xml)

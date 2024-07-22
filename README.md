# UnityGotchiThirdweb_Template
A template for starting an Aavegotchi game with AavegotchiKit and Thirdweb

## Getting Started

### Downloading this repo
1. Simply git clone this repo and you can get started right away with the default sample scene

### Integrating into an existing project

1. Install Thirdweb package
https://github.com/thirdweb-dev/unity-sdk/releases/tag/v4.16.8

2. Download this repo as a zip file
3. Find the Assets/Plugins/GotchiHub folder
4. Drag and drop GotchiHub folder into existing project
5. Install the AavegotchiKit plus the following dependencies

    AavegotchiKit
    https://github.com/bmateus/AavegotchiKit.git

    UniTask v2.2.5: Aavegotchi Kit uses UniTask for handling asyncronous operations
    https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask

    SimpleGraphQL v1.3.2: A graphQL client for reading the gotchi data from theGraph
    https://github.com/LastAbyss/SimpleGraphQL-For-Unity.git No longer supported but works fine

    Vector Graphics v2.0.0-preview.21 - bmateus fork: Used for rendering the SVGs This package doesn't seem to be supported by Unity and this for contains some fixes for the SVG renderer that allow it to work with the Aavegotchi SVGs
    https://github.com/bmateus/com.unity.vectorgraphics.git

    WebGL Threading Patcher (if making WebGL builds):
    https://github.com/VolodymyrBS/WebGLThreadingPatcher.git

6. Go into the AavegotchiKit in unity packages and comment out everything in the GotchiSDKAppearance file



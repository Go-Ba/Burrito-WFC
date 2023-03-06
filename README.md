# Burrito-WFC
Wave Function Collapse tool for Unity Engine

Edited from: https://github.com/selfsame/unity-wave-function-collapse <br />
Original algorithm: https://github.com/mxgmn/WaveFunctionCollapse

Selfsame did the work of making the algorithm work in unity, but they hooked it up to a system that was coupled to gameobjects. <br />
I cut that off and turned it into a more general texture generator.<br />

I've only hooked up the overlap model as that is what interested me, but the simple tiled model is in there just not being used by my class.

This comes with a prefab holding a Canvas, on the canvas is a RawImage with a script to test out the overlap model. <br />
Put one of the sample sprites from the folder into the Sprite field and play with settings to see results. Space bar is set to re-generate the model

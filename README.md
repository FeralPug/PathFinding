# Pathfinding
A small collection of scripts for a generic grid pathfinding solution for Unity

Here is a simple collection of code for creating pathfinding solutions for grid based games made in the Unity game engine. To use simply import the code in the AlgosAndInterfaces folder into your Unity project. From there you simply need to impliment the interfaces to work with your grid system. The main part of this is writing a distance function for one cell to a different cell, and providing ways for cells in the grid to provide their neighbooring cells. 

The "nodes" of the grid are completely stateless when it comes to pathfinding so there is never anything to reset and multi-threaded pathfinding is thread safe.

Once the interfaces are implemented for you grid cells you only need to call the one of the FindPath methods on an instance of the PathFinder class. One method is for getting an immediate response the other takes a callback that can be used with multithreading.

In the Examples folder there is an implimentation for a square based grid with only cardinal direction movement. You could import all of that code into your project if you want to see the path finding working. You will need to create a SquareGridManager game object with that script attached to it. Fill in the fields on that object. This will involve creating a SquareGridChunk prefab object, which is just an empty game object with a SquareGridChunk script attached. Add a MeshRenderer and a material if you want to visualize the grid. I have included a shader for the grid chunks to use for this purpose. Fill out the fields on the grid chunk object.

Click on a cell to set the start and click on a cell while holding left shift to set the end. Right click to clear selected cells. You can select multiple start cells by clicking multiple cells. The grid manager will then find the path using the A* algorithm and a priority queue min heap data structure. The example is set up in a threaded way so that all pathfinding happens outside of Unity's main player loop.

![](https://github.com/FeralPug/PathFinding/blob/main/Example/SquareGrid/Demo/squareGridDemo.gif)

﻿using System;
using System.Collections.Generic;

namespace LP2_P2
{
    /// <summary>
    /// Class responsible for creating a ghost and giving it an AI
    /// </summary>
    class Ghost : Object
    {
        // Creates a list for storing a temporary path
        private readonly List<Object> open = new List<Object>();
        // Creates an HashSet for storing a locked path
        private readonly HashSet<Object> closed = new HashSet<Object>();
        // Creates a List to store the neigbhoring pieces of the current Object 
        private readonly List<Object> neighbors = new List<Object>();
        // Creates a List to store all the Objects on the board
        private readonly List<Object> allPieces = new List<Object>();
        // Creates the corner the ghosts should go to when in scatter or 
        // frighten mode
        private readonly EmptySpace corner;
        private readonly EmptySpace center = new EmptySpace(13, 7);
        // Stores the state the ghosts are currently in
        public GhostState state;
        // Creates a variable of the type random
        private Random rnd = new Random();

        /// <summary>
        /// Constructor of the class Ghost
        /// </summary>
        /// <param name="x"> The wanted X position </param>
        /// <param name="y"> the wanted Y position </param>
        /// <param name="allMapPieces"> The List of all Physics Objects</param>
        public Ghost(int x, int y, List<Object> allMapPieces,
            int cornerX, int cornerY)
        {
            // Creates a new Position vector and assigns it the x and y value
            Pos = new Position(x, y);
            // Creates a new Position and assigns it the x value -1 and y value
            OldPos = new Position(x - 1, y);
            // Assigns a character to be displayed while rendering
            Visuals = 'U';
            // Creates the collider bounding box
            BoxCollider = new int[4] { x, y, x + 1, y + 1 };
            // Assigns this Object list the one passed as argument 
            allPieces = allMapPieces;
            // Assigns the ghost state to chase mode
            state = GhostState.chase;
            // Assigns the ghost it's respective corner
            corner = new EmptySpace(cornerX, cornerY);
        }

        /// <summary>
        /// A* algorithm for searching the bes path to a position
        /// </summary>
        /// <param name="target"> The end position it should arrive </param>
        /// <returns> A list of Position for the ghost to follow </returns>
        public List<Position> CalcuatePath(Object target)
        {
            if (state == GhostState.scatter)
                target = corner;

            if (state == GhostState.frightened)
            {
                while (target.GetType() == typeof(Player) ||
                    target.GetType() == typeof(MapPiece)) 
                {
                    target = allPieces[rnd.Next(0, allPieces.Count)];
                } 
            }
            if (state == GhostState.eaten)
            {
                if (Pos == center.Pos)
                    state = GhostState.chase;
                else
                    target = center;
            }

            // Clears the list to make sure they're empty before starting to
            // use them
            open.Clear();
            closed.Clear();
            neighbors.Clear();
            // Adds the the ghost position 'start' to the open list
            open.Add(this);

            // Searches for a path while the open list has something or it 
            // returned something
            while (open.Count > 0)
            {
                // Creates a variable and assigns it the first object on the
                // open list (initially the ghost itself)
                Object current = open[0];

                // Performs a loop while i is less than the amount of Objects
                // on Open list, ignoring the first Object (the Ghost)
                for (int i = 1; i < open.Count; i++)
                {
                    // Checks if the cost of a Object on the open list is less
                    // or equals than the cost of the current Object
                    if (open[i].combinedCost < current.combinedCost
                        || open[i].combinedCost == current.combinedCost)
                    {
                        // Checks which Object is clossest to the target
                        if (open[i].closenessCost < current.closenessCost)
                        {
                            // Sets the current Object to the current Object on
                            // the open list
                            current = open[i];
                        }
                    }
                }

                // Removes the current piece from the open list
                open.Remove(current);
                // Adds it to the locked path list
                closed.Add(current);

                // Checks if the current position is the same as the target 
                if (current.Pos == target.Pos)
                {
                    // Uses TracePath Method to generate a list and sends it
                    // back
                    return TracePath(current);
                }

                // Checks all the objects for the neighbours of the current
                // piece
                for (int c = 0; c < allPieces.Count; c++)
                {
                    // Checks if that piece is not the Player, a Wall or has
                    // the same Position has the last position of the ghost
                    if (allPieces[c].GetType() != typeof(Player) &&
                        allPieces[c].GetType() != typeof(MapPiece) &&
                        !(allPieces[c].Pos == OldPos))
                    {
                        // Compares the piece position to the cordinates either
                        // of the four neibghours should be
                        if (allPieces[c].Pos.X == current.Pos.X + 1
                            && allPieces[c].Pos.Y == current.Pos.Y ||
                            allPieces[c].Pos.X == current.Pos.X - 1
                            && allPieces[c].Pos.Y == current.Pos.Y ||
                            allPieces[c].Pos.Y == current.Pos.Y + 1
                            && allPieces[c].Pos.X == current.Pos.X ||
                            allPieces[c].Pos.Y == current.Pos.Y - 1
                            && allPieces[c].Pos.X == current.Pos.X)
                        {
                            // Checks if the selected piece is a teleporter
                            if (allPieces[c].GetType() == typeof(Teleporter)) 
                            {
                                // Creates a variable and assigns it a value
                                // if x = 0 x will be 26 else x will be 1
                                int x = allPieces[c].Pos.X == 0 ? 26 : 1;
                                // Creates and adds a new Object with the x
                                // created and the y of the allPieces[c]
                                neighbors.Add(new EmptySpace(x, allPieces[c].Pos.Y));
                            }
                            else
                                // Adds that Piece to neighbors list
                                neighbors.Add(allPieces[c]);
                        }
                    }
                }

                // Checks all the Objects on the neighbors list
                for (int b = 0; b < neighbors.Count; b++)
                {
                    // Checks if the Object is already in the locked path
                    if (!closed.Contains(neighbors[b]))
                    {
                        // Local variable combining the distance to the start and
                        // the distance between the current position and that
                        // neibhor
                        int newCostMov = current.distanceCost +
                            GetDistace(current.Pos, neighbors[b].Pos);

                        // Checks if that variable is lower than the current 
                        // distance of the Object and open list doesn't contain it
                        if (newCostMov < neighbors[b].distanceCost
                            || !open.Contains(neighbors[b]))
                        {
                            // Sets a new distance cost to that neighbor
                            neighbors[b].distanceCost = newCostMov;
                            // Sets a new closeness to that neighbor
                            neighbors[b].closenessCost =
                                GetDistace(neighbors[b].Pos, target.Pos);
                            // Sets the parent of that neighbor the current object
                            neighbors[b].parent = current;
                            // Adds that neighbor to the open list
                            open.Add(neighbors[b]);
                        }
                    }
                }
                // Clears the list of neighbors
                neighbors.Clear();
            }
            // If it fails to return a path returns null
            return null;
        }
        /// <summary>
        /// Forms a list of positions by getting the positions of the parents
        /// of the Object until the Object is the start position
        /// </summary>
        /// <param name="end"> The Object it targets </param>
        /// <returns> A list of all the positions of the defined path</returns>
        private List<Position> TracePath(Object end)
        {
            // Creates a list of positions to store the path
            List<Position> path = new List<Position>();
            // Creates a local Object variable and assigns it the passed Object
            Object currentPiece = end;

            // Runs the loop until the currentPiece is not the start
            while (currentPiece != this)
            {
                // Adds the position of the currentPiece to the list
                path.Add(currentPiece.Pos);
                // Assigns the currentPiece the parent of that piece
                currentPiece = currentPiece.parent;
            }
            // The path forms from end to start, so it needs to be reversed
            path.Reverse();
            // Returns the list
            return path;
        }
        /// <summary>
        /// A simple method for calculating distances
        /// </summary>
        /// <param name="A"> The first position </param>
        /// <param name="B"> The Second position </param>
        /// <returns> An Integer with the aprox distance between the two
        /// </returns>
        private int GetDistace(Position A, Position B) => 
            // Distance formula (square root of ((x-x^2) + (y-y^2))) 
            (int)Math.Sqrt(Math.Pow(Math.Abs(A.X - B.X), 2) +
             Math.Pow(Math.Abs(A.Y - B.Y), 2));
    }
}

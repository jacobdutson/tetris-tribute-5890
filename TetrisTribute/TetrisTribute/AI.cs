﻿// Jacob Dutson
// CS 5890
// Tetris Tribute
// AI class definition.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TetrisTribute
{
    class AI
    {
        //Constants from Game Piece class
        //integers representing a color
        //TODO NEED TO CHANGE VALUES
        const int green = 5;
        const int red = 5;
        const int purple = 5;
        const int blue = 6;
        const int yellow = 7;
        const int cyan = 7;
        const int orange = 6;
        const int empty = 0;


        // Define global constants:
        public const int GAME_BOARD_X_MIN = 0;
        public const int GAME_BOARD_Y_MIN = 0;
        public const int GAME_BOARD_X_MAX = 10;
        public const int GAME_BOARD_Y_MAX = 20;
        public const int NO_KEYS_PRESSED = 0;
        public const int LEFT_KEY_PRESS = 1;
        public const int RIGHT_KEY_PRESS = 2;
        public const int ROTATE_KEY_PRESS = 3;
        public const int SPEED_UP_KEY_PRESS = 4;
        public const int NUMBER_OF_ROTATIONS = 4;
        public const int LINE_CLEAR_BONUS = 20;
        public const int NO_GAPS_BONUS = 25;
        public const int LOWEST_BONUS = 10;

        // Declare class data members:
        private Queue<int> inputsQueue;		// Stores a queue of keys that the AI has pressed.
        private Piece previewPiece;		// The upcoming piece for next time.
        private Piece currentPiece;		// The current piece to move.
        private Piece movePiece;        // Temporary copy of the current piece, used in the functions that managed movement.
        private int[,] currentBoard;		// Keeps a snapshot of the map at the time the piece was dropped.
        private List<Location> locations;	// Stores each location detected in the map to place the piece.
        private Location chosenLocation;		// This is the location the AI will try to place the piece in.
        private bool kill;      // Gives the AI class a kill signal.
        private bool pause;     // Gives the AI class a pause signal.
        private bool resume;    // Gives the AI class a resume signal.
        
        // Define class methods:
        /**
         * AI Class Constructor
         * Initializes all of the data members of the AI class.
         * @param -- None.
         * @return -- Nothing.
         */
        public AI()
        {
            // Allocate space for data members:
            inputsQueue = new Queue<int>();
            previewPiece = new Piece();
            currentPiece = new Piece();
            currentBoard = new int [GAME_BOARD_X_MAX , GAME_BOARD_Y_MAX];
            locations = new List<Location>();
            chosenLocation = new Location();           
        }

        public bool Kill
        {
            get { return kill; }
            set { kill = value; }
        }

        public bool Pause
        {
            get { return pause; }
            set { pause = value; }
        }

        public bool Resume
        {
            get { return resume; }
            set { resume = value; }
        }

        /**
         * Plays the current game piece.
         * Makes copies of the game pieces and map
         * inside the AI class and then calls 
         * private methods to choose where to 
         * place the piece and to generate the
         * AI input signals to move the piece.
         * @param -- int [][] board, a reference to the 
         * current game board
         *        -- Piece cPiece, a reference to the 
         * current piece that the AI needs to play
         *        -- Piece pPiece, a reference to the 
         * next game piece that the AI will need to play
         * @return -- Nothing.
         */
        public void playCurrentPiece(int[][] board, int[][] cPiece, int[][] pPiece)
        {
            // Copy needed data from arguments to local stores:
            // Check if the board exists:
            if (board != null)
            {
                for (int row = 0; row < GAME_BOARD_Y_MAX; row++)
                {
                    for (int column = 0; column < GAME_BOARD_X_MAX; column++)
                    {
                        currentBoard[row, column] = board[row][column];
                    }
                }
            }
            else
            {
                throw new NullReferenceException("AI Class playCurrentPiece method:  The map object passed into the function was a NULL pointer.");
            }
            // Check if the current piece exists:
            if (cPiece != null)
            {
                currentPiece.PieceArray = cPiece;
                movePiece.PieceArray = currentPiece.PieceArray;
            }
            else
            {
                throw new NullReferenceException("AI Class playCurrentPiece method:  The current piece object passed into the function was a NULL pointer.");
            }
            // Check if the previous piece exists:
            if (pPiece != null)
            {
                previewPiece.PieceArray = pPiece;
            }
            else
            {
                throw new NullReferenceException("AI Class playCurrentPiece method:  The previous piece object passed into the function was a NULL pointer.");
            }

            // Loop until the AI player finds a successful way to reach the chosen location:
            bool needNewLocation;
            do
            {
                // Call function to chose a place on the board to steer the piece to:
                findLocation();
                needNewLocation = false;

                // Check if the current game piece make the next move:
                do
                {
                    // Loop twice to get both rotations if the piece can't move where it needs to with the first rotation:
                    for (int numRotations = 0; numRotations < 2; numRotations++)
                    {
                        // Could it move?
                        if (moveOnce())
                        {
                            break;
                        }
                        else 
                        {   // NO.  Has it already rotated?
                            if (numRotations == 0)
                            {
                                // NO.  Then rotate it and let it try again.
                                rotate();
                            }
                            else
                            {
                                // YES.  Reduce the board and let it try finding a new location.
                                reduceBoard();
                                needNewLocation = true;
                            }
                        }
                    }

                    // Check if we need a new location before we continue:
                    if (needNewLocation)
                    {
                        // Leave this loop and go to the one that finds locations:
                        break;
                    }

                } while (atDestination());

            } while (needNewLocation);

            // Call function to generate the input signals to tell the GamePlay class how the AI wants to move it's piece:
            
        }

        private void reduceBoard()
        {
            throw new NotImplementedException();
        }

        private bool rotate()
        {
            throw new NotImplementedException();
        }

        private bool atDestination()
        {
            // Declare local variables:
            bool atDest = false;
            Location moveLoc = new Location();
            
                
            // Check if the points inside of the move location
            // are the same points in the chosen location:
            /*for (int pointCounter = 0; pointCounter < 4; pointCounter++)
            {
                // Is the current point the same in both
                if (chosenPoint.X == movePiece.X && chosenPoint.Y == movePiece.Y)
                {
                    atDest = true;
                }
                else
                {
                    atDest = false;
                }
            }*/

            return atDest;
        }

        /**
         * Removes the next input value from
         * the inputs queue and returns it.
         * @param -- None.
         * @return -- Integer value representing an AI key press. 
         * This value can be compared to the AI class key 
         * constants to determine what key was pressed.
         */
        public int getKeyPressed()
        {
            return inputsQueue.Dequeue();
        }

        /**
         * Returns the number of elements stored in the inputs queue.
         * @param -- None.
         * @return -- Integer value equal the number of items in the inputs queue.
         */
        public int getNumberOfInputs()
        {
            return inputsQueue.Count;
        }

        /**
         * Loops through the game board's columns and calls a series of functions
         * to determine to the best location to place the piece.  The final 
         * function called sets the chosen location.
         * @param -- None.
         * @return -- None.
         */
        private void findLocation()
        {
            // Declare local variables:
            Location temp = new Location();
            int pieceWidth = 0; 
            int pieceHeight = 0;
            int index = 0;

            // Loop through the different rotations and rotate the game piece each time through:
            for (int rotationCounter = 0; rotationCounter < NUMBER_OF_ROTATIONS; rotationCounter++)
            {
                // Update the piece dimensions for each rotation:
                pieceWidth = currentPiece.getWidth();
                pieceHeight = currentPiece.getHeight();

                // Loop through the each column until the last column that will still allow the 
                // piece to stay on the game board.  Looping starts on the left side of the board:
                for (int columnCounter = 0; columnCounter < (GAME_BOARD_X_MAX - pieceWidth); columnCounter++)
                {
                    // Call method to set the points in temp to the landing position above the current column:
                    temp.setPoints(currentPiece, columnCounter);

                    // Call method to look for open space gaps under the location temp:
                    if (checkForGaps(temp) == 0)
                    {
                        temp.Rank += NO_GAPS_BONUS;
                    }
                   
                    // Call method to determine how many lines, if any, will be cleared by placing the piece in location temp:
                    temp.Rank += (numLinesCleared(temp) * LINE_CLEAR_BONUS);

                    // Call method to determine how many cells will be filled in the lines that temp crosses: 
                    // This is done to try and guess which locations have the best chances of assisting line completes.
                    //temp.Rank += numCellsFilled(temp);

                    // Add the temporary location to the list of locations that will be considered for the current move:
                    locations.Add(temp);
                }

                // Call method to perform one rotation on the current piece, so that the AI can check if that rotation 
                // finds a better location than previous locations:
                currentPiece = rotatePiece(currentPiece);

                // Reset temp by referencing it to a new heap location (new Location object):
                temp = new Location();
            }
            
            // Call method to find the lowest location on the game board out of the ones in the locations list:
            index = findLowestLocation();

            // Store the location at the index in the locations list into temp:
            temp = locations.ElementAt<Location>(index);

            // Update the rank on temp:
            temp.Rank += LOWEST_BONUS;

            // Choose the location with the best rank to be the AI player's destination choice:
            chooseMaxRankedLocation();
        }

        /**
         * Searches for the highest ranked location in the locations list. 
         * Sets chosen location to the location it found that had the
         * highest rank.
         * @param -- None.
         * @return -- Nothing.
         */
        private void chooseMaxRankedLocation()
        {
            // Declare local variables:
            int largestRank = 0;

            // Loop through the locations list 
            // and find the one with the largest rank:
            foreach (Location currentLocation in locations)
            {
                // Check if the current locations rank is 
                // larger than any previously found locations:
                if (currentLocation.Rank > largestRank)
                {
                    // Update the largest rank:
                    largestRank = currentLocation.Rank;
                    // Update the chosen location:
                    chosenLocation = currentLocation;
                }
            }


        }

        /**
         * Searches for the lowest location on the game board. 
         * Returns the row index of that location.
         * @param -- None.
         * @return -- Integer value, the index of the lowest location on the board.
         *         The lowest location is the one with the largest Y value for one of it's points.
         */
        private int findLowestLocation()
        {
            // Declare local variables:
            int indexOfMax = 0;
            int maximumY = 0;
            int currentListMaxY = 0;

            // Loop through the list of locations:
            for (int locationIndex = 0; locationIndex < locations.Count; locationIndex++ )
            {
                // Check to see if this location has a larger Y (lower value)
                // than the current largest Y found.
                currentListMaxY = locations.ElementAt<Location>(locationIndex).MaxY;
                if (currentListMaxY > maximumY)
                {
                    // If it does set it as the new highest.
                    maximumY = currentListMaxY;
                    indexOfMax = locationIndex;
                }
            }

            return indexOfMax;
        }

        /**
         * Copy of the Game Piece classes rotate piece method.
         * This one adds the argument.
         */ 
        private Piece rotatePiece(Piece cPiece)
        {
            int[][] curPiece = cPiece.PieceArray;
         
            if (curPiece.Length == 4)
            {
                int[][] rotated = new int[4][];
                for (int i = 0; i < 4; i++)
                {
                    rotated[i] = new int[] { empty, empty, empty, empty };
                }
                rotated[0][1] = curPiece[1][0];
                rotated[1][1] = curPiece[1][1];
                rotated[2][1] = curPiece[1][2];
                rotated[3][1] = curPiece[1][3];

                rotated[1][0] = curPiece[0][1];
                rotated[1][2] = curPiece[2][1];
                rotated[1][3] = curPiece[3][1];
                curPiece = rotated;
            }
            else if (curPiece.Length == 3)
            {
                int[][] rotated = new int[3][];
                for (int i = 0; i < 3; i++)
                {
                    rotated[i] = new int[] { empty, empty, empty };
                }

                rotated[0][0] = curPiece[2][0];
                rotated[0][1] = curPiece[1][0];
                rotated[0][2] = curPiece[0][0];

                rotated[1][0] = curPiece[2][1];
                rotated[1][1] = curPiece[1][1];
                rotated[1][2] = curPiece[0][1];

                rotated[2][0] = curPiece[2][2];
                rotated[2][1] = curPiece[1][2];
                rotated[2][2] = curPiece[0][2];

             
                curPiece = rotated;

            }
           
            return (new Piece(curPiece));
     
        }

        /**
         * Checks to see how many lines (rows) on the game board 
         * will clear if the piece is placed in this location.
         * @param -- Location temp, a location object to search for gaps beneath.
         * @return -- Integer value, the number of gaps below the piece.
         */
        private int numLinesCleared(Location temp)
        {
            // Declare local var0iables:
            int linesCleared = 0;
            int filledCellsInRow = 0;
            List<int> usedY = new List<int>(4);
            int cellsFilledByTemp = 0;
            
            // Loop through the location and check the gaps below each point:
            foreach (Point currentPoint in temp.getPoints())
            {
                // Reset the filled cells count:
                filledCellsInRow = 0;

                // Check to see if this row has already been checked.
                // Don't check it again if it has.
                if(!usedY.Contains(currentPoint.Y))
                {
                    // Add the current points y value to the list of used y values:
                    usedY.Add(currentPoint.Y);

                    // Loop through the game board and see how many cells are in each line:
                    for (int column = 0; column < GAME_BOARD_X_MAX; column++)
                    {
                        if (currentBoard[currentPoint.Y, column] != 0)
                        {
                            filledCellsInRow++;
                        }
                    }
                
                    // Determine the number of cells in temp in the current row:
                    foreach (Point cellInTemp in temp.getPoints())
                    {
                        // Check if this cell is in the current row:
                        if (cellInTemp.Y == currentPoint.Y)
                        {
                            cellsFilledByTemp++;
                        }
                    }

                    // Check if the cells in the location plus the cells already filled will complete the row:
                    if ((cellsFilledByTemp + filledCellsInRow) == GAME_BOARD_X_MAX)
                    {
                        linesCleared++;
                    }
                }
            }

            return linesCleared;
        }

        /**
         * Checks if any gaps are left underneath the location on the game board.
         * @param -- Location temp, a location object to search for gaps beneath.
         * @return -- Integer value, the number of gaps below the piece.
         */
        private int checkForGaps(Location temp)
        {
            // Declare local variables:
            int numberOfGaps = 0;

            // Loop through the location and check the gaps below each point:
            foreach (Point currentPoint in temp.getPoints())
            {
                // Check if the current point is one of the lowest points in the location:
                if( temp.MaxY == currentPoint.Y)
                {
                    // Search the cells in the game board below the current point:
                    for (int row = currentPoint.Y + 1; row < GAME_BOARD_Y_MAX; row++)
                    {
                        // Check to see if the space is currently filled:
                        if (currentBoard[row, currentPoint.X] == 0)
                        {
                            numberOfGaps++;
                        }
                        // Check to see if the space is empty (a gap):
                        else 
                        {
                            break;
                        }

                    }

                }
            }

            return numberOfGaps;
        }

        

        /**
         * Returns the number of elements stored in the inputs queue.
         * @param -- None.
         * @return -- Integer value equal the number of items in the inputs queue.
         */
        private bool moveOnce()
        {
            throw new NotImplementedException();
        }

        // Define sub classes:
        private class Point
        {
		    // Declare class data members:
		    private int x;
			private int y;
				
            // Define class member functions:
            public Point()
            {
                // Set the point up to exist at the game board's origin.
                X = 0;
                Y = 0;
            }

            /**
             *  Gets and Sets the value of x.
             */
            public int X	
            {
                get{return x;} 
                set{ // Checks if the point is a valid location on the game board.  
                    if (value < GAME_BOARD_X_MIN || value > GAME_BOARD_X_MAX)
                    {
                        // If it's not valid then it sets the point's x value to the game boards origin and throws an exception.
                        x = 0;  
                        throw new Exception("AI Point Class Error:  The game attempted to give a point a x-value out of the game boards bounds.");
                    }
                    else
                    {
                        x = value;
                    }
                }
            }
            /**
             *  Gets and Sets the value of y.
             */
            public int Y	
            {
                get { return y; }
                set
                { // Checks if the point is a valid location on the game board.  
                    if (value < GAME_BOARD_Y_MIN || value > GAME_BOARD_Y_MAX)
                    {
                        // If it's not valid then it sets the point's y value to the game boards origin and throws an exception.
                        y = 0;
                        throw new Exception("AI Point Class Error:  The game attempted to give a point a y-value out of the game boards bounds.");
                    }
                    else
                    {
                        y = value;
                    }
                }
            }
				
        }

        private class Location
        {
            // Declare class constants:
            private const int POINTS_ARRAY_SIZE = 4;    // Size of the points array.

            // Declare class data members:
		    private Point[] points_;	// Arrays of cells on the game board that will be used in this location.
			private int rank;          // The locations rank -- the higher the better it is to move the game piece to this location.
            private int maxY;          // The largest Y value in the set of points.
            private int maxX;          // The largest X value in the set of points.
            private int minY;          // The smallest Y value in the set of points.
            private int minX;          // The smallest X value in the set of points.

            // Define class methods:
            /**
             * Initialize the location to default values.
             *
             */
            public Location()
            {
                points_ = new Point[POINTS_ARRAY_SIZE];
                points_[0] = new Point();
                points_[1] = new Point();
                points_[2] = new Point();
                points_[3] = new Point();
                MaxY = 0;
                MaxY = 0;
                MinX = GAME_BOARD_X_MAX;
                MinY = GAME_BOARD_Y_MAX;
                Rank = 0;
            }
            /**
             * Gets and Sets the value of rank.
             *
             */
            public int Rank	 
            {
                get { return rank; }
                set { rank = value; }
            }
            /**
             * Gets and Sets the value of maxY.
             *
             */
            public int MaxY
            {
                get { return maxY; }
                set { maxY = value; }
            }
            /**
             * Gets and Sets the value of maxX.
             *
             */
            public int MaxX
            {
                get { return maxX; }
                set { maxX = value; }
            }
            /**
             * Gets and Sets the value of minY.
             *
             */
            public int MinY
            {
                get { return minY; }
                set { minY = value; }
            }
            /**
             * Gets and Sets the value of minX.
             *
             */
            public int MinX
            {
                get { return minX; }
                set { minX = value; }
            }
            /**
             * Adds a point object to the location's points array.
             * Checks to make sure that the array index is valid 
             * and throws an exception if it isn't.
             *
             */
            public void setPoint(int x, int y, int index)
            {
                if (index >= POINTS_ARRAY_SIZE || index < 0)
                {
                    points_[index].X = x;
                    points_[index].Y = y;

                    // Update the maximum Y value.
                    if (y >= MaxY)
                    {
                        MaxY = y;
                    }

                    // Update the minimum Y value.
                    if (y <= MinY)
                    {
                        MinY = y;
                    }

                    // Update the maximum X value.
                    if (x >= MaxX)
                    {
                        MaxX = x;
                    }

                    // Update the minimum X value.
                    if (x <= MinX)
                    {
                        MinX = x;
                    }
                }
                else
                {
                    throw new Exception("AI Location Class:  Invalid points array index passed into the setPoint function.  The valid index range is 0 to 4.");
                }
            }
            /**
             * Adds points to the location's points array.
             * Sets the location's points to match the points of  
             * the tempPiece adjusted so that the left most-side
             * of the tempPiece is in the column, column.
             * @param -- Piece tempPiece, a game piece object to 
             *        use to calculate the points.  The resulting points 
             *        will hold the same shape as this piece object, but 
             *        will translated to a new location on the board.
             *        -- int column, the column to use as the left
             *        side reference for the resulting location's points.
             * @return -- Nothing.
             */
            public void setPoints(Piece tempPiece, int column)  // NOT DONE!!!!!
            {
                // Declare local variables:
                int xAdjustment = 0;
                int yAdjustment = 0;

                // Calculate the adjustment difference for the x axis:
                //xAdjustment = tempPiece.x - column;
                //yAdjustment = tempPiece.y - map.getColumnHeight(column);

                // Loop to create point objects and add them to the location:
                for (int pointsCounter = 0; pointsCounter < POINTS_ARRAY_SIZE; pointsCounter++)
                {
                    // Call method to add the current point to the location.
              //      setPoint(tempPiece.x - xAdjustment, tempPiece.y - yAdjustment, pointsCounter);
                }

            }
            /**
             * Returns a point object from the location's points array.
             *
             */
            public Point getPoint(int index)
            {
                return points_[index];
            }
            /**
             * Returns the collection of point objects from the location's points array.
             *
             */
            public Point[] getPoints()
            {
                return points_;
            }
        }

        private class Piece
        {
            // Declare class data members:
            private int[][] pieceArray;
            private int x;
            private int y;

            // Define class methods:
            public Piece()
            {
                
                pieceArray = new int[4][] {new int [4],
                                             new int [4],
                                             new int [4],
                                             new int [4]};
            }

            public Piece(int[][] inPiece)
            {
                pieceArray = new int[4][] {new int [4],
                                             new int [4],
                                             new int [4],
                                             new int [4]};

                PieceArray = inPiece;
            }

            public int X
            {
                get { return x; }
               // set { x = value; }
            }

            public int Y
            {
                get { return y; }
               // set { y = value; }
            }

            public int[][] PieceArray
            {
                get { return pieceArray; }
                set 
                {
                    // Copy the 1st dimension of the array argument.
                    value.CopyTo(pieceArray, 0);

                    // Loop to copy 2nd dimension arrays from the arry argument.
                    for (int arrayCount = 0; arrayCount < value.GetLength(0); arrayCount++)
                    {
                        value[arrayCount].CopyTo(pieceArray[arrayCount], 0);
                    }

                }
            }

            /**
             * Determines how many blocks high the game piece is
             * at it's highest point.
             * @param -- None.
             * @return -- integer, the height of the game piece.
             */
            public int getHeight()
            {
                // Declare local variables:
                int[] heights = new int[4];
                int maxHeight = 0;

                // Loop through piece and check how many non-zero values are in the array:
                for (int row = 0; row < pieceArray.GetLength(0); row++)
                {
                    for (int col = 0; col < pieceArray.GetLength(1); col++)
                    {
                        // Check if the current cell is an actual game block:
                        if (pieceArray[row][col] != 0)
                        {
                            heights[col] += heights[col];
                        }
                    }
                }

                // Determine which height was the greatest:
                for (int heightCounter = 0; heightCounter < heights.Length; heightCounter++)
                {
                    // Check if the current height is greater than previous heights:
                    if (maxHeight <= heights[heightCounter])
                    {
                        maxHeight = heights[heightCounter];
                    }
                }

                return maxHeight;
            }

            /**
             * Determines how many blocks wide the game piece is
             * at it's widest point.
             * @param -- None.
             * @return -- integer, the width of the game piece.
             */
            public int getWidth()
            {
                // Declare local variables:
                int[] widths = new int[4];
                int maxWidth = 0;

                // Loop through piece and check how many non-zero values are in the array:
                for (int row = 0; row < pieceArray.GetLength(0); row++)
                {
                    for (int col = 0; col < pieceArray.GetLength(1); col++)
                    {
                        // Check if the current cell is an actual game block:
                        if (pieceArray[row][col] != 0)
                        {
                            widths[row] += widths[row];
                        }
                    }
                }

                // Determine which Width was the greatest:
                for (int widthCounter = 0; widthCounter < widths.Length; widthCounter++)
                {
                    // Check if the current Width is greater than previous Widths:
                    if (maxWidth <= widths[widthCounter])
                    {
                        maxWidth = widths[widthCounter];
                    }
                }

                return maxWidth;
            }
            
        }
    }
}

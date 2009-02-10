// Jacob Dutson
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
        // Define global constants:
        public const int GAME_BOARD_X_MIN = 0;
        public const int GAME_BOARD_Y_MIN = 0;
        public const int GAME_BOARD_X_MAX = GamePlay.COLUMNS;
        public const int GAME_BOARD_Y_MAX = GamePlay.ROWS;
        public const int NO_KEYS_PRESSED = 0;
        public const int LEFT_KEY_PRESS = 1;
        public const int RIGHT_KEY_PRESS = 2;
        public const int ROTATE_KEY_PRESS = 3;
        public const int SPEED_UP_KEY_PRESS = 4;
        public const int NUMBER_OF_ROTATIONS = 4;
        public const int LINE_CLEAR_BONUS = 20;
        public const int NO_GAPS_BONUS = 25;
        public const int LOWEST_BONUS = 15;

        // Declare class data members:
        private Queue<int> inputsQueue;	    // Stores a queue of keys that the AI has pressed.
        private Piece currentPiece;		    // The current piece to move.
        private Piece movePiece;            // Temporary copy of the current piece, used in the functions that manage movement.
        private int[,] currentBoard;	    // Keeps a snapshot of the map at the time the piece was dropped.
        private int[] columnTops;           // Array holding the current height of each column in the map.
        private List<Location> locations;	// Stores each location detected in the map to place the piece.
        private Location chosenLocation;	// This is the location the AI will try to place the piece in.
        private bool kill;                  // Gives the AI class a kill signal.
        private bool pause;                 // Gives the AI class a pause signal.
        private bool resume;                // Gives the AI class a resume signal.
        
        // Define class methods:
        /**
         * AI Class Constructor
         * Initializes all of the data members of the AI class.
         * @param -- None.
         * @return -- Nothing.
         */
        public AI()
        {
            reset();

            kill = false;
            pause = false;
            resume = false;
        }

        public void reset()
        {
            // Allocate space for data members:
            inputsQueue = new Queue<int>();
            currentPiece = new Piece();
            movePiece = new Piece();
            currentBoard = new int[GAME_BOARD_Y_MAX, GAME_BOARD_X_MAX];
            columnTops = new int[GAME_BOARD_X_MAX];
            locations = new List<Location>(50);
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
        public void playCurrentPiece(int[][] board, int[] colTops, int[][] cPiece)
        {
            // Declare local variables:
            int cPieceX = 3;
            int cPieceY = 0; 


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
            // Check if the array of column top locations exists:
            if (colTops != null)
            {
                // Call Array class member function to copy the array contents:
                colTops.CopyTo(columnTops, 0);
            }
            else
            {
                throw new NullReferenceException("AI Class playCurrentPiece method:  The column top locations array passed into the function was a NULL pointer.");
            }
            // Check if the current piece exists:
            if (cPiece != null)
            {
                currentPiece.PieceArray = cPiece;
                currentPiece.X = cPieceX;
                currentPiece.Y = cPieceY;
                movePiece.PieceArray = currentPiece.PieceArray;
                movePiece.X = currentPiece.X;
                movePiece.Y = currentPiece.Y;
            }
            else
            {
                throw new NullReferenceException("AI Class playCurrentPiece method:  The current piece object passed into the function was a NULL pointer.");
            }

            // Loop until the AI player finds a successful way to reach the chosen location:
            /*bool needNewLocation;
            do
            {*/
                // Call function to chose a place on the board to steer the piece to:
            if (!Pause)
            {
                inputsQueue.Clear();

                findLocation();                         

                // Check if the current game piece make the next move:
                while (!atDestination())
                {
                    try
                    {
                        // Could it move?
                        if (!moveOnce())
                        {
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.Out.WriteLine("AI Class moveOnce method threw an exception: " + err.Message + "\n");
                    }
     
                }

                // Add input signals for the final rotates:
                for (int rotateAdder = 0; rotateAdder < chosenLocation.RotateCount; rotateAdder++)
                {
                    inputsQueue.Enqueue(ROTATE_KEY_PRESS);
                }

            

                // Add move down signal:
                for (int speedUpInputCounter = 0; speedUpInputCounter < 60; speedUpInputCounter++)
                {
                    inputsQueue.Enqueue(SPEED_UP_KEY_PRESS);
                }
           //} while (needNewLocation);

            }
             
        }


        /**
         * Determines if the move piece has reached the 
         * chosen location.  Compares the points on the
         * board where each location is to see if they
         * are the same.
         * @param -- None.
         * @return -- Boolean value, TRUE if the AI piece has 
         * reached the chosen location and FALSE if the AI
         * piece has not reached that location.
         */
        private bool atDestination()            
        {
            // Declare local variables:
            bool atDest = false;
            //int numPointsEqual = 0;

            if (chosenLocation.MinX == movePiece.X)
            {
                atDest = true;
            }

            // Check if the points inside of the move location
            // are the same points in the chosen location:
            /*foreach(Point currentPoint in chosenLocation.getPoints())
            {
                // Loop through the points in the move piece:
                for (int row = 0; row < movePiece.PieceArray.Length; row++)
                {
                    for (int column = 0; column < movePiece.PieceArray.Length; column++)
                    {
                        if (movePiece.PieceArray[row][column] != 0)
                        {
                            // Check if the x and y coordinates of the current
                            // point are the same as the ones in the destination.
                            if (((movePiece.X + column) == currentPoint.X)) //&& ((movePiece.Y + row) == currentPoint.Y))
                            {
                                numPointsEqual++;
                            }
                        }
                    }
                }
            }

            // Check to see if all the points were equal:
            if (numPointsEqual == 4)
            {
                atDest = true;
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
            if (inputsQueue.Count == 0)
            {
                return 0;
            }
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
                for (int columnCounter = 0; columnCounter <= (GAME_BOARD_X_MAX - pieceWidth); columnCounter++)
                {

                    try
                    {

                        // Call method to set the points in temp to the landing position above the current column:
                        temp.adjustPointsForLocation(currentPiece, columnCounter, maxOfColumnTops(columnCounter, pieceWidth));
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Location class adjustPointsForLocation method threw an exception: " + err.Message + "\n");
                    
                    }

                    try
                    {

                    // Call method to look for open space gaps under the location temp:
                    if (!checkForGaps(temp))
                    {
                        temp.Rank += NO_GAPS_BONUS;
                    }
                    
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("AI class checkForGaps method threw an exception: " + err.Message + "\n");

                    }

                    // Call method to determine how many lines, if any, will be cleared by placing the piece in location temp:
                    temp.Rank += (numLinesCleared(temp) * LINE_CLEAR_BONUS);

                    // Call method to determine how many cells will be filled in the lines that temp crosses: 
                    // This is done to try and guess which locations have the best chances of assisting line completes.
                    //temp.Rank += numCellsFilled(temp);

                    // Update the rotation that temp is in:
                    temp.RotateCount = rotationCounter;

                    // Add the temporary location to the list of locations that will be considered for the current move:
                    locations.Add(temp);

                    // Reset temp by referencing it to a new heap location (new Location object):
                    temp = new Location();
                }

                // Call method to perform one rotation on the current piece, so that the AI can check if that rotation 
                // finds a better location than previous locations:
                currentPiece = rotatePiece(currentPiece);
            }
            
            // Call method to find the lowest location on the game board out of the ones in the locations list:
            try{

                index = findLowestLocation();
            
            }
            catch (Exception err)
            {
                Console.WriteLine("AI class findLowestLocation method threw an exception: " + err.Message + "\n");
            }

            // Store the location at the index in the locations list into temp:
            if (index < locations.Count)
            {
                temp = locations.ElementAt<Location>(index);
            }
            else
            {
                throw new Exception("AI Class findLocation method: invalid index into the locations array.");
            }

            // Update the rank on temp:
            temp.Rank += LOWEST_BONUS;

            // Choose the location with the best rank to be the AI player's destination choice:
            chooseMaxRankedLocation();
        }

        private int maxOfColumnTops(int colTop, int width)
        {
            // Declare local variables:
            int max = 19;

            // Loop through each column in the piece and find 
            // the one with the highest top:
            for (int colCount = 0; colCount < width; colCount++)
            {
                if (columnTops[colTop + colCount] < max)
                {
                    max = columnTops[colTop + colCount];
                }
            }

            return max;
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
            Random randomNumber = new Random();
            int indexOfMax = 0;
            int[] indicesOfMax = new int[50];
            int maxIndex = 0;
            int maximumY = 0;
            int currentListMaxY = 0;
            int tempIndex = 0;

            // Loop through the list of locations:
            for (int locationIndex = 0; locationIndex < locations.Count; locationIndex++ )
            {
                // Check to see if this location has a larger Y (lower value)
                // than the current largest Y found.
                currentListMaxY = locations.ElementAt<Location>(locationIndex).MaxY;
                if (currentListMaxY >= maximumY)
                {
                    // If it does set it as the new highest.
                    maximumY = currentListMaxY;
                    indicesOfMax[maxIndex++] = locationIndex;
                }
            }

            // Generate a random number:
            tempIndex = randomNumber.Next(0, maxIndex - 1);
            indexOfMax = indicesOfMax[tempIndex];


            return indexOfMax;
        }

        /**
         * Copy of the Game Piece classes rotate piece method.
         * This one adds the argument and changes return type.
         * Note: Jason Newbold wrote most of this.
         */ 
        private Piece rotatePiece(Piece cPiece)
        {
            int[][] curPiece = cPiece.PieceArray;
         
            if (curPiece.Length == 4)
            {
                int[][] rotated = new int[4][];
                for (int i = 0; i < 4; i++)
                {
                    rotated[i] = new int[] { GamePiece.empty, GamePiece.empty, GamePiece.empty, GamePiece.empty };
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
                    rotated[i] = new int[] { GamePiece.empty, GamePiece.empty, GamePiece.empty };
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
         * Copy of the Game Play classes canMove method.
         * Note: Jason Newbold wrote most of this.
         */ 
        public bool canMove(int curRow, int curColumn, int direction)
        {
            //direction  -1 left,  0 down,  1 right

            int down = (direction + 1) % 2;

            for (int row = 0; row < movePiece.PieceArray.Length; row++)
            {
                for (int column = 0; column < movePiece.PieceArray.Length; column++)
                {
                    //check if at a wall  ??? what about emtpy bottom row on curpiece
                    if ((curRow + row + down) >= GAME_BOARD_Y_MAX || (curColumn + column + direction) >= GAME_BOARD_X_MAX || (curColumn + column + direction) < 0)
                    {
                        if (movePiece.PieceArray[row][column] != GamePlay.EMPTY)
                        {
                            return false;
                        }

                    }
                    else if (currentBoard[curRow + row + down, curColumn + column + direction] != GamePlay.EMPTY && movePiece.PieceArray[row][column] != GamePlay.EMPTY)
                    {
                        return false;
                    }
                }
            }

            return true;
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
         * @return -- Boolean value, TRUE if gaps were found and FALSE if gaps weren't found.
         */
        private bool checkForGaps(Location temp)
        {
            // Declare local variables:
            int numberOfGaps = 0;
            bool dontCheck = false;

            // Loop through the location and check the gaps below each point:
            foreach (Point currentPoint in temp.getPoints())
            {
                // Check if we need to skip this point.
                foreach (Point otherPoint in temp.getPoints())
                {
                    if (currentPoint.X == otherPoint.X && otherPoint.Y > currentPoint.Y)
                    {
                        dontCheck = true;
                    }
                }
                 
                // Check if the current point is one of the lowest points in the location:
                if (!dontCheck)
                {
                    // Search the cells in the game board below the current point:
                    for (int row = currentPoint.Y + 1; row < GAME_BOARD_Y_MAX; row++)
                    {
                        /*if (currentPoint.X > GAME_BOARD_X_MAX)
                        {
                            throw new Exception("AI Class checkForGaps function found a point with an x value of " + currentPoint.X);
                        }*/

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
                else
                {
                    dontCheck = false;
                }
            }

            // Check for no gaps:
            if (numberOfGaps == 0)
            {
                return false;
            }
            return true;
        }

        /**
         * Tries to move the game piece and returns it's 
         * success.  It checks to see if the move to location
         * is empty (can move) or not (can't move).  It 
         * calculates whether it should move the piece 
         * left or right to reach the chosen destination. 
         * @param -- None.
         * @return -- Boolean value, TRUE if the piece moved without 
         * colliding, and FALSE if it couldn't move without colliding.
         */
        private bool moveOnce()
        {
            // Declare local variables:
            int xDisplacement = 0;

            // Calculate the column difference between the destination location's
            // left-most column and the left-most column of the move game piece.
            xDisplacement = chosenLocation.MinX - movePiece.X;

            // Check if the displacement is positive or negative:
            if (xDisplacement > 0)
            {
                return moveRight();
            }
            else if (xDisplacement < 0)
            {
                return moveLeft();
            }

            return true;
        }

        /**
         * Tries to move the game piece left and returns it's 
         * success.  It checks to see if the move to location
         * is empty (can move) or not (can't move).  
         * @param -- None.
         * @return -- Boolean value, TRUE if the piece moved without 
         * colliding, and FALSE if it couldn't move without colliding.
         */
        private bool moveLeft()         
        {
            // Declare local variables:
            int left = -1;

            // Check if it ran into another game piece:
            if (!canMove(movePiece.Y, movePiece.X, left) )  
            {
                return false;
            }
            else
            {
                // Move the piece left:
                movePiece.X--;

                // Update inputs queue:
                inputsQueue.Enqueue(LEFT_KEY_PRESS);
                //inputsQueue.Enqueue(NO_KEYS_PRESSED);
                return true;
            }
        }

        /**
         * Tries to move the game piece right and returns it's 
         * success.  It checks to see if the move to location
         * is empty (can move) or not (can't move).  
         * @param -- None.
         * @return -- Boolean value, TRUE if the piece moved without 
         * colliding, and FALSE if it couldn't move without colliding.
         */
        private bool moveRight()       
        {
            // Declare local variables:
            int right = 1;

            // Check if it ran into another game piece:
            if (!canMove(movePiece.Y, movePiece.X, right))
            {
                return false;
            }
            else
            {
                // Move the piece right.
                movePiece.X++;

                // Update inputs queue:
                inputsQueue.Enqueue(RIGHT_KEY_PRESS);
                //inputsQueue.Enqueue(NO_KEYS_PRESSED);
                return true;
            }
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
                    /*if (value < GAME_BOARD_X_MIN || value >= GAME_BOARD_X_MAX)
                    {
                        // If it's not valid then it sets the point's x value to the game boards origin and throws an exception.
                        x = 0;  
                        throw new Exception("AI Point Class Error:  The game attempted to give a point a x-value out of the game boards bounds.");
                    }
                    else
                    {*/
                        x = value;
                    //}
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
                   /* if (value < GAME_BOARD_Y_MIN || value >= GAME_BOARD_Y_MAX)
                    {
                        // If it's not valid then it sets the point's y value to the game boards origin and throws an exception.
                        y = 0;
                        throw new Exception("AI Point Class Error:  The game attempted to give a point a y-value out of the game boards bounds.");
                    }
                    else
                    {*/
                        y = value;
                    //}
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
            private int rotateCount;            // Keeps track of the number of rotates to perform before dropping the piece.

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
            public int RotateCount
            {
                get { return rotateCount; }
                set { rotateCount = value; }
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
                if (index < POINTS_ARRAY_SIZE && index >= 0)
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
            public void adjustPointsForLocation(Piece tempPiece, int column, int row) 
            {
                // Declare local variables:
                int index = 0;

                // Loop to create point objects and add them to the location:
                for (int rowCount = 0; rowCount < tempPiece.PieceArray.Length; rowCount++)
                {
                    for (int colCount = 0; colCount < tempPiece.PieceArray.Length; colCount++)
                    {
                        // Test if the current location in temp piece is an game piece block:
                        if (tempPiece.PieceArray[rowCount][colCount] != 0)
                        {
                            /*if (tempPiece.getHeight() == 1)
                            {
                                // Add the point to the location:
                                setPoint(colCount + column, row - tempPiece.PieceArray.Length + rowCount - 2, index);
                            }
                            else
                            {*/
                                // Add the point to the location:
                                setPoint(colCount + column, row - tempPiece.PieceArray.Length + rowCount + 1, index);
                            //}

                            

                            index++;
                        }
                    }
                }

                // If the location is not already sitting on the top of the stack
                // and can move down without problems, then move it down.
                int tempMaxY = this.MaxY;
                if (tempMaxY < row)
                {
                    for (int adjustYCount = 0; adjustYCount < POINTS_ARRAY_SIZE; adjustYCount++)
                    {
                        setPoint(this.points_[adjustYCount].X, this.points_[adjustYCount].Y + (row - tempMaxY), adjustYCount);
                    }
                }

                int tempMinX = this.MinX;
                if (tempMinX > column)
                {
                    for (int adjustXCount = 0; adjustXCount < POINTS_ARRAY_SIZE; adjustXCount++)
                    {
                        setPoint(this.points_[adjustXCount].X - (tempMinX - column), this.points_[adjustXCount].Y, adjustXCount);
                    }
                }
                /*if (canMove())        // MIGHT WANT TO TRY THIS AGAIN!!!!!
                {
                    for (int adjustCount = 0; adjustCount < POINTS_ARRAY_SIZE; adjustCount++)
                    {
                        setPoint(this.points_[index].X, this.points_[index].Y - (row - this.maxY), index);
                    }
                }*/


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
                
               /* pieceArray = new int[4][] {new int [4],
                                             new int [4],
                                             new int [4],
                                             new int [4]};*/
            }

            public Piece(int[][] inPiece)
            {
                /*pieceArray = new int[4][] {new int [4],
                                             new int [4],
                                             new int [4],
                                             new int [4]};*/

                PieceArray = inPiece;
            }

            public int X
            {
                get { return x; }
                set { x = value; }
            }

            public int Y
            {
                get { return y; }
                set { y = value; }
            }

            public int[][] PieceArray
            {
                get { return pieceArray; }
                set 
                {
                    pieceArray = value;
                    // Copy the 1st dimension of the array argument.
                    //value.CopyTo(pieceArray, 0);

                    // Loop to copy 2nd dimension arrays from the arry argument.
                    //for (int arrayCount = 0; arrayCount < value.GetLength(0); arrayCount++)
                    //{
                     //   value[arrayCount].CopyTo(pieceArray[arrayCount], 0);
                    //}

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
                int height = 0;
                //int maxWidth = 0;

                // Loop through piece and check how many non-zero values are in the array:
                for (int row = 0; row < pieceArray.Length; row++)
                {
                    for (int col = 0; col < pieceArray.Length; col++)
                    {
                        // Check if the current cell is an actual game block:
                        if (pieceArray[row][col] != 0)
                        {
                            heights[row]++;
                        }
                    }
                }

                // Determine height:
                for (int heightCounter = 0; heightCounter < heights.Length; heightCounter++)
                {
                    if (heights[heightCounter] > 0)
                    {
                        height++;
                    }
                }

                return height;
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
                int width = 0;
                //int maxWidth = 0;

                // Loop through piece and check how many non-zero values are in the array:
                for (int row = 0; row < pieceArray.Length; row++)
                {
                    for (int col = 0; col < pieceArray.Length; col++)
                    {
                        // Check if the current cell is an actual game block:
                        if (pieceArray[row][col] != 0)
                        {
                            widths[col]++;
                        }
                    }
                }

                // Determine width:
                for (int widthCounter = 0; widthCounter < widths.Length; widthCounter++)
                {
                    if (widths[widthCounter] > 0)
                    {
                        width++;
                    }
                }

                return width;
            }
            
        }
    }
}

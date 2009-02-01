// Jacob Dutson
// CS 5890
// Tetris Tribute
// AI class definition.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetrisTribute
{
    class AI
    {
        // Define global constants:
        const int GAME_BOARD_X_MIN = 0;
        const int GAME_BOARD_Y_MIN = 0;
        const int GAME_BOARD_X_MAX = 10;
        const int GAME_BOARD_Y_MAX = 18;
        const int MAX_NUM_LOCATIONS = 40;

        // Declare class data members:
        Queue<int> inputsQueue;		// Stores a queue of keys that the AI has pressed.
		//Piece previewPiece;		// The upcoming piece for next time.
		//Piece currentPiece;		// The current piece to move.
		int [,] currentMap;		// Keeps a snapshot of the map at the time the piece was dropped.
		Location [] locations;	// Stores each location detected in the map to place the piece.
		int [] maximums;			// Stores the maximum values for each location. Used to make findLowestLocation method run faster.
        Location chosenLocation;		// This is the location the AI will try to place the piece in.

        // Define class methods:
        /**
         * AI Class Constructor
         * Initializes all of the data members of the AI class.
         */
        public AI()
        {
            inputsQueue = new Queue<int>();
            currentMap = new int [GAME_BOARD_X_MAX , GAME_BOARD_Y_MAX];
            locations = new Location[MAX_NUM_LOCATIONS];
            maximums = new int [MAX_NUM_LOCATIONS];
        }

        // Define sub classes:
        class Point
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

        class Location
        {
            // Declare class constants:
            private const int POINTS_SIZE = 4;    // Size of the points array.

            // Declare class data members:
		    private Point [] points_;	// Arrays of cells on the game board that will be used in this location.
			private int rank_;          // The locations rank -- the higher the better it is to move the game piece to this location.
            private int maxY_;          // The largest Y value in the set of points.

            // Define class methods:
            /**
             * Initialize the location to default values.
             *
             */
            public Location()
            {
                points_ = new Point[POINTS_SIZE];
                points_[0] = new Point();
                points_[1] = new Point();
                points_[2] = new Point();
                points_[3] = new Point();
                MaxY = 0;
                Rank = 0;
            }
            /**
             * Gets and Sets the value of rank.
             *
             */
            public int Rank	 
            {
                get { return rank_; }
                set { rank_ = value; }
            }
            /**
             * Gets and Sets the value of maxY.
             *
             */
            public int MaxY
            {
                get { return maxY_; }
                set { maxY_ = value; }
            }
            /**
             * Adds a point object to the location's points array.
             * Checks to make sure that the array index is valid 
             * and throws an exception if it isn't.
             *
             */
            public void setPoint(int x, int y, int index)
            {
                if (index >= POINTS_SIZE || index < 0)
                {
                    points_[index].X = x;
                    points_[index].Y = y;

                    // Update the maximum Y value.
                    if (y > MaxY)
                    {
                        MaxY = y;
                    }
                }
                else
                {
                    throw new Exception("AI Location Class:  Invalid points array index passed into the setPoint function.  The valid index range is 0 to 4.");
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
        }
    }
}

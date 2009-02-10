using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Author Jason Newbold

namespace TetrisTribute
{
    class GamePiece
    {
        //integers representing a color
        public const int empty = 0;
        public const int blue = 1;
        public const int red = 2;
        public const int green = 3;
        public const int yellow = 4;
        public const int orange = 5;
        public const int cyan = 6; //gray
        public const int purple = 7;
                
        //used to get the next random piece
        Random random;

        //array containing the square piece
        int[][] square;
        //array containing the step-right piece
        int[][] stepRight;
        //array containing the step-left piece
        int[][] stepLeft;
        //array containing the pyrmid piece
        int[][] pyrmid;
        //array containing the straight piece
        int[][] straight;
        //array containing the l-shape piece
        int[][] lblock;
        //array containing the reverse l-shape piece
        int[][] reversel;

        //contains the current piece
        int[][] curPiece;
        //contains the next piece
        int[][] nextPiece;

        //current row the current piece is located
        int curRow;
        //current column the current piece is located
        int curColumn;

        public GamePiece()
        {
            random = new Random();

            //initialize pieces
            square = new int[2][] { new int[] { yellow, yellow }, 
                                    new int[] { yellow, yellow } };

            straight = new int[4][] { new int[] { empty, empty, empty, empty }, 
                                      new int[] { orange, orange, orange, orange }, 
                                      new int[] { empty, empty, empty, empty },
                                      new int[] { empty, empty, empty, empty } };

            stepRight = new int[3][]{ new int[] { empty, empty, empty}, 
                                      new int[] { empty, green, green},
                                      new int[] { green, green, empty} };

            stepLeft = new int[3][]{  new int[] { empty, empty, empty}, 
                                      new int[] { red,   red,   empty},
                                      new int[] { empty, red,   red} };

            pyrmid = new int[3][]{    new int[] { empty, empty, empty}, 
                                      new int[] { cyan,  cyan,  cyan},
                                      new int[] { empty, cyan,  empty} };

            lblock = new int[3][]{    new int[] { empty, empty, empty}, 
                                      new int[] { blue,  blue,  blue},
                                      new int[] { blue,  empty, empty} };

            reversel = new int[3][]{  new int[] { empty,  empty,  empty}, 
                                      new int[] { purple, purple, purple},
                                      new int[] { empty,  empty,  purple} };

            //get the peices
            curPiece = getRandomPiece();
            nextPiece = getRandomPiece();

            //the pieces start at -1, 3
            curRow = -1;
            curColumn = 3;

        }

        //returns the next piece
        public int[][] getNextPiece()
        {
            return nextPiece;
        }

        //returns the current piece
        public int[][] getCurPiece()
        {
            return curPiece;
        }

        //updates the pieces
        public void updatePiece()
        {
            curPiece = nextPiece;
            nextPiece = getRandomPiece();
            curColumn = 3;
            curRow = -1;
        }

        //rotate the current piece
        public int[][] rotatePiece()
        {
            //rotate the piece according to their size
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
           
            return curPiece;
        }

        //returns a random type of piece
        private int[][] getRandomPiece()
        {
            int[][] piece;

            switch (random.Next(0, 7))
            {
                case 0:
                    piece = square;
                    break;
                case 1:
                    piece = stepRight;
                    break;
                case 2:
                    piece = stepLeft;
                    break;
                case 3:
                    piece = pyrmid;
                    break;
                case 4:
                    piece = straight;
                    break;
                case 5:
                    piece = lblock;
                    break;
                case 6:
                    piece = reversel;
                    break;
                default:
                    piece = getRandomPiece();
                    break;
            }

            return piece;
        }

        //sets the current piece, used for the gravity effect
        public void setCurPiece(int[][] piece)
        {
            curPiece = piece;
        }

        //returns the current row the current piece is on
        public int getCurRow()
        {
            return curRow;
        }

        //returns the current column the current piece is on
        public int getCurColumn()
        {
            return curColumn;
        }

        //sets the cur row of the current piece
        public void setCurRow(int row)
        {
            curRow = row;
        }

        //set the current column of the current column
        public void setCurColumn(int column)
        {
            curColumn = column;
        }

    }
}

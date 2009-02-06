using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetrisTribute
{
    class GamePiece
    {
        //integers representing a color
        //TODO NEED TO CHANGE VALUES
        public const int green = 5;
        public const int red = 5;
        public const int purple = 5;
        public const int blue = 6;
        public const int yellow = 7;
        public const int cyan = 7;
        public const int orange = 6;
        public const int empty = 0;

        Random random;

        int[][] square;
        int[][] stepRight;
        int[][] stepLeft;
        int[][] pyrmid;
        int[][] straight;
        int[][] lblock;
        int[][] reversel;


        int[][] curPiece;
        int[][] nextPiece;

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

            curPiece = straight;// getRandomPiece();
            nextPiece = getRandomPiece();

         //   rotatePiece();
         //   rotatePiece();
           // rotatePiece();
           // rotatePiece();
           // rotatePiece();
            

        }

        public int[][] getNextPiece()
        {
            return nextPiece;
        }

        public int[][] getCurPiece()
        {
            return curPiece;
        }

        public void updatePiece()
        {
            curPiece = nextPiece;
            nextPiece = getRandomPiece();
            //return block;
        }

        public int[][] rotatePiece()
        {
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace TetrisTribute
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GamePlay : Microsoft.Xna.Framework.Game
    {
        //number of rows and columns on game board
        public const int ROWS = 20;
        public const int COLUMNS = 10;
       
        //menu items selections
        public const int PLAY = 0;
        public const int HIGH = 1;
        public const int CREDITS = 2;
        public const int EXIT = 3;

        int[][] displayBoard;

        //const to determine if gameboard is empty
        const int EMPTY = 0;

        //variables assiociated with the graphics
        public GraphicsDeviceManager graphics;
        GraphicsManager gm;
        SpriteBatch spriteBatch;

        //class that stores and loads high scores
        HighScores high;

        //class that manages the creation, rotation of game pieces
        GamePiece piece;

        //Input class to get user input from keyboard, gamepad, AI
        Input userInput;

        //delegate that is used to call the correct update function based on state
        delegate void updateDelegate(GameTime gameTime);
        //delegate that is used to call the correct draw function based on state
        delegate void drawDelegate(GameTime gameTime);

        updateDelegate update;
        drawDelegate draw;

        //double array that represents the current tetris board
        int[][] gameBoard;
        //stores the current user score
        int score;
        //milliseconds remaining till the peice drops
        double dropTime;
        //milliseconds between peice drops
        double dropSpeed;
        //stores if credits are finished being drawn
        bool creditsFinished;
        //when it reaches 0, the attract mode will start
        double idleTime;
        //time remaining till it exits from the credits
        double exitTime;
        //stores the menu item that is currently selected
        int selectedMenuItem;

        double moveTime;
        public GamePlay()
        {
            graphics = new GraphicsDeviceManager(this);
            gm = new GraphicsManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            selectedMenuItem = PLAY;
            // TODO: Add your initialization logic here
           
            //TODO set drop speed
            dropTime = 500;
            dropSpeed = 500;
            idleTime = 30000;
            exitTime = 30000;
            creditsFinished = false;
            score = 0;
            moveTime = 500;

            update = new updateDelegate(menuUpdate);
            draw = new drawDelegate(menuDraw);

            userInput = new Input();
            //initialize the game board
            gameBoard = new int[ROWS][];
            for (int i = 0; i < ROWS; i++)
            {
                gameBoard[i] = new int[COLUMNS];
                for (int j = 0; j < COLUMNS; j++)
                {
                    gameBoard[i][j] = 0;
                }
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gm.LoadContent(this);

            high = new HighScores();
            piece = new GamePiece();        
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //DO not add any code to this function unless it is needed for every state(menu, game, credits, scores)
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            userInput.updateInput();

            update(gameTime);           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //DO not add any code to this function unless it is needed for every state(menu, game, credits, scores)
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //this will call the function of the given state
            draw(gameTime);

            base.Draw(gameTime);
        }

        public void reset()
        {
            dropTime = 500;
            dropSpeed = 500;
            
            score = 0;
            moveTime = 500;

            
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    gameBoard[i][j] = 0;
                }
            }
        }


        /// <summary>
        /// Allows the game to run logic needed for the menu
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void menuUpdate(GameTime gameTime)
        {
             
            //if ((state.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
            //    || (gamePadState.DPad.Down == ButtonState.Pressed && gamePadState.DPad.Down == ButtonState.Released))
            if(userInput.Up  && !userInput.PreviousUp)
            { 
                //change the selected menu item up one position
                //??DO we want to wrap the selection if so have an else = 3
                if (selectedMenuItem > 0)
                {
                    idleTime = 30000;
                    selectedMenuItem--;
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            //if (state.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
            if(userInput.down && !userInput.PreviousDown)
            {
                //change selected item down one
                //??DO we want to wrap the selection if so have an else = 0
                if (selectedMenuItem < 3)
                {
                    idleTime = 30000;
                    selectedMenuItem++;
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            //if (state.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
            if(userInput.Enter && !userInput.PreviousEnter)
            {
                idleTime = 30000;
                //get selected item
                //change update and draw delegates
                if (selectedMenuItem == PLAY)
                {
                    reset();
                    userInput.setMenuControl(false);
                    update = new updateDelegate(gameUpdate);
                    draw = new drawDelegate(gameDraw);
                }
                else if (selectedMenuItem == HIGH)
                {
                    update = new updateDelegate(scoresUpdate);
                    draw = new drawDelegate(scoreDraw);
                }
                else if (selectedMenuItem == CREDITS)
                {
                    update = new updateDelegate(creditsUpdate);
                    draw = new drawDelegate(creditsDraw);
                }
                else if (selectedMenuItem == EXIT)
                {
                    //exit the game
                    this.Exit();
                }
            }
            idleTime = idleTime - gameTime.ElapsedGameTime.Milliseconds;
            if (idleTime < 0)
            {
                idleTime = 30000;
                //TODO CODE TO START AI
            }
        }

        /// <summary>
        /// Allows the game to run logic needed for the game being played
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void gameUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            bool slide = false;

            int right = 1;
            int down = 0;
            int left = -1;

            //if (state.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
            if(userInput.Right && !userInput.PreviousRight)
            {
                //move one space to the right
                if (canMove(piece.getCurRow(), piece.getCurColumn(), right))
                {
                    piece.setCurColumn(piece.getCurColumn() + right);
                }
            }

            //if (state.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
            if(userInput.Right && userInput.PreviousRight)
            {
                slide = true;
                //get time and move right accordingly
                moveTime = moveTime - gameTime.ElapsedGameTime.Milliseconds;
                if (moveTime < 0)
                {
                    //move one space to the right
                    if (canMove(piece.getCurRow(), piece.getCurColumn(), right))
                    {
                        piece.setCurColumn(piece.getCurColumn() + right);
                    }
                    moveTime = 75;
                }
            }

           // if (state.IsKeyDown(Keys.Left) && !oldState.IsKeyDown(Keys.Left))
            if( userInput.Left && !userInput.PreviousLeft)
            {
                //move one space to the Left
                if (canMove(piece.getCurRow(), piece.getCurColumn(), left))
                {
                    piece.setCurColumn(piece.getCurColumn() + left);
                }
            }

            //if (state.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
            if(userInput.Left && userInput.PreviousLeft)
            {
                slide = true;
                //get time and move Left accordingly
                moveTime = moveTime - gameTime.ElapsedGameTime.Milliseconds;
                if (moveTime < 0)
                {
                    //move one space to the right
                    if (canMove(piece.getCurRow(), piece.getCurColumn(), left))
                    {
                        piece.setCurColumn(piece.getCurColumn() + left);
                    }
                    moveTime = 75;
                }
            }

            //if (state.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
            if(userInput.Up && !userInput.PreviousUp)
            {
                //move one space to the Left
                piece.rotatePiece();
                if (!canMove(piece.getCurRow() + 1, piece.getCurColumn(), down))
                {
                    piece.rotatePiece();
                    piece.rotatePiece();
                    piece.rotatePiece();
                    //UNDUE rotate
                }
            }

            //if (state.IsKeyDown(Keys.Up) && oldState.IsKeyDown(Keys.Up))
            if(userInput.Up && userInput.PreviousUp)
            {
                //get time and move Left accordingly
            }

            //if (state.IsKeyDown(Keys.Down))
            if(userInput.Down)
            {
                //get time and move Left accordingly
                int mult = 8;
                dropTime = dropTime - (gameTime.ElapsedGameTime.Milliseconds * mult);
            }
            if (!slide)
            {
                moveTime = 300;
            }
            dropTime = dropTime - gameTime.ElapsedGameTime.Milliseconds;

            if (dropTime < 0)
            {
                if (canMove(piece.getCurRow(), piece.getCurColumn(), down))
                {
                    piece.setCurRow(piece.getCurRow() + 1);
                }
                else
                {
                    for (int i = 0; i < piece.getCurPiece().Length; i++)
                    {
                        for (int j = 0; j < piece.getCurPiece().Length; j++)
                        {
                            if (piece.getCurPiece()[i][j] != EMPTY && (piece.getCurRow() + i) < ROWS
                                && (piece.getCurColumn() + j) < COLUMNS)
                            {
                                gameBoard[piece.getCurRow() + i][piece.getCurColumn() + j] = piece.getCurPiece()[i][j];
                            }
                        }
                    }
                    printGame();
                    clearRows();
                    
                    piece.updatePiece();
                    if (!canMove(piece.getCurRow(), piece.getCurColumn(), down))
                    {
                        update = new updateDelegate(menuUpdate);
                        //draw = new drawDelegate(gameDraw);
                        //TODO GAME OVER
                        userInput.setMenuControl(true);
                        printGame();
                        int a = 5;
                    }
                }
                dropTime = dropSpeed;
            }

        }

        /// <summary>
        /// Allows the game to run logic needed for the credits
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void creditsUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            
            //TODO Escape key or any key???
            if((userInput.Enter && !userInput.PreviousEnter) || creditsFinished)
            {
                update = new updateDelegate(menuUpdate);
                draw = new drawDelegate(menuDraw);
            }
            exitTime = exitTime - gameTime.ElapsedGameTime.Milliseconds;

        }

        /// <summary>
        /// Allows the game to run logic needed for the highScores
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void scoresUpdate(GameTime gameTime)
        {
            //if got high score
            //input to get name etc
            //enter to finish
            //else entertoexit
        }

        /// <summary>
        /// This is called when the menu should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void menuDraw(GameTime gameTime)
        {

        }

        /// <summary>
        /// This is called when the game being played should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void gameDraw(GameTime gameTime)
        {

            gm.drawBoard(gameBoard);
            // call this, passing the gameboard to draw the board

             gm.drawPiece(piece.getCurPiece(), 250 + (30 * piece.getCurColumn()), -30 + (30 * piece.getCurRow()));
            // call this to draw a piece: aPiece at x and y

            // i'll fix this up later, but its a start for testing.
        }

        /// <summary>
        /// This is called when the credits should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void creditsDraw(GameTime gameTime)
        {
            //TODO set creditsFinished = true when credits are finished
        }

        /// <summary>
        /// This is called when the HighScores should be draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void scoreDraw(GameTime gameTime)
        {
            
        }

        //for debugging only
        private void printGame()
        {
            Console.WriteLine("********************");
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    Console.Write(gameBoard[i][j]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("********************");
        }

        private void clearRows()
        {
            for (int i = 0; i < ROWS; i++)
            {
                bool clear = true;
                for (int j = 0; j < COLUMNS && clear; j++)
                {
                    if (gameBoard[i][j] == EMPTY)
                    {
                        clear = false;
                    }
                }
                if (clear)
                {
                    for (int k = i; k > 0 && clear; k--)
                    {
                        for (int j = 0; j < COLUMNS && clear; j++)
                        {
                            gameBoard[k][j] = gameBoard[k - 1][j];
                        }
                    }
                    for (int j = 0; j < COLUMNS && clear; j++)
                    {
                        gameBoard[0][j] = EMPTY;
                    }
                    //i++;
                }
            }
        }

        private bool canMove(int curRow, int curColumn, int direction)
        {
            //direction  -1 left,  0 down,  1 right

            int down = (direction + 1) % 2;

            for (int i = 0; i < piece.getCurPiece().Length; i++)
            {
                for (int j = 0; j < piece.getCurPiece().Length; j++)
                {
                    //check if at a wall  ??? what about emtpy bottom row on curpiece
                    if ((curRow + i + down) >= ROWS || (curColumn + j + direction) >= COLUMNS || (curColumn + j + direction) < 0)
                    {
                        if (piece.getCurPiece()[i][j] != EMPTY)
                        {
                            return false;
                        }

                    }
                    else if (gameBoard[curRow + i + down][curColumn + j + direction] != EMPTY && piece.getCurPiece()[i][j] != EMPTY)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}

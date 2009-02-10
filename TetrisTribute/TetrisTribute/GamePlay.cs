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

        //const to determine if gameboard is empty
        public const int EMPTY = 0;

        //variables assiociated with the graphics
        public GraphicsDeviceManager graphics;
        GraphicsManager gm;
        SpriteBatch spriteBatch;

        //class that stores and loads high scores
        HighScores high;

        //class that manages the creation, rotation of game pieces
        GamePiece piece;

        // AI class to start and manage the AI player:
        AI ai;

        //Input class to get user input from keyboard, gamepad, AI
        Input userInput;

        //delegate that is used to call the correct update function based on state
        delegate void updateDelegate(GameTime gameTime);
        //delegate that is used to call the correct draw function based on state
        delegate void drawDelegate(GameTime gameTime);

        updateDelegate update;
        drawDelegate draw;

        bool highScore;

        //double array that represents the current tetris board
        int[][] gameBoard;
        int[] columnTops;
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
        int creditTime;
        //stores the menu item that is currently selected
        int selectedMenuItem;
        int level;
        int linesCleared;

        private SoundEffect effect;
        private SoundEffect clearEffect;
        private SoundEffect selection;


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

            
            dropTime = 350;
            dropSpeed = 350;
            idleTime = 5000;
            creditTime = 18000;
            creditsFinished = false;
            score = 0;
            moveTime = 500;
            level = 1;
            linesCleared = 0;
            highScore = false;


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

            // Initialize the array that stores the information about
            // where the highest piece in each column of the game
            // board is located:
            columnTops = new int[COLUMNS];
            for (int columnIndex = 0; columnIndex < COLUMNS; columnIndex++)
            {
                // Set the initial value to the bottom of the game board.
                columnTops[columnIndex] = ROWS - 1;
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
            gPiece = new GamePiece();

            effect = Content.Load<SoundEffect>("drop");
            clearEffect = Content.Load<SoundEffect>("slash");
            selection = Content.Load<SoundEffect>("selection");
            Song song = Content.Load<Song>("Tetris music");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;


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
            dropTime = 350;
            dropSpeed = 350;

            score = 0;
            moveTime = 500;

            linesCleared = 0;
            level = 0;
            pause = false;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    gameBoard[i][j] = 0;
                }
            }

            // Initialize the array that stores the information about
            // where the highest piece in each column of the game
            // board is located:
            for (int columnIndex = 0; columnIndex < COLUMNS; columnIndex++)
            {
                // Set the initial value to the bottom of the game board.
                columnTops[columnIndex] = ROWS - 1;
            }
        }


        /// <summary>
        /// Allows the game to run logic needed for the menu
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void menuUpdate(GameTime gameTime)
        {

             if (userInput.Up && !userInput.PreviousUp)
            {
                //change the selected menu item up one position
                if (selectedMenuItem > 0)
                {
                    idleTime = 5000;
                    selectedMenuItem--;
                    selection.Play();
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            if (userInput.Down && !userInput.PreviousDown)
            {
                //change selected item down one
                //??DO we want to wrap the selection if so have an else = 0
                if (selectedMenuItem < 3)
                {
                    idleTime = 30000;
                    selectedMenuItem++;
                    selection.Play();
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            if (userInput.Enter && !userInput.PreviousEnter)
            {
                idleTime = 5000;
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
                idleTime = 5000;

                reset();
                // Initialize the AI player:
                ai = new AI();

                update = new updateDelegate(attrackModeUpdate);
                draw = new drawDelegate(gameDraw);

            }
        }

        bool pause;
        /// <summary>
        /// Allows the game to run logic needed for the game being played
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void attrackModeUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            int nextAIInput = 0;
            int right = 1;
            int down = 0;
            int left = -1;

            // Check if the user wants to end Attrack Mode.
            if (userInput.Enter == true || userInput.Left == true || userInput.Right == true || userInput.Up == true || userInput.Down == true)
            {
                update = new updateDelegate(menuUpdate);
                draw = new drawDelegate(menuDraw);
            }

            try
            {
                // Tell AI player to play the current piece:
                ai.playCurrentPiece(gameBoard, columnTops, piece.getCurPiece());

            }
            catch (Exception err)
            {
                Console.Out.WriteLine(err.Message);
            }

            try
            {
                nextAIInput = ai.getKeyPressed();
            }
            catch (Exception err)
            {
                nextAIInput = 0;
            }



            ai.Pause = true;

            if (nextAIInput == AI.RIGHT_KEY_PRESS)
            {
                //move one space to the right
                if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), right))
                {
                    piece.setCurColumn(piece.getCurColumn() + right);
                }
            }

            if (nextAIInput == AI.LEFT_KEY_PRESS)
            {
                //move one space to the Left
                if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), left))
                {
                    piece.setCurColumn(piece.getCurColumn() + left);
                }
            }

            if (nextAIInput == AI.ROTATE_KEY_PRESS)
            {
                //move one space to the Left
                piece.rotatePiece();
                if (!canMove(piece, piece.getCurRow() + 1, piece.getCurColumn(), down))
                {
                    piece.rotatePiece();
                    piece.rotatePiece();
                    piece.rotatePiece();
                }
            }

            if (nextAIInput == AI.SPEED_UP_KEY_PRESS)
            {
                //get time and move Left accordingly
                int mult = 8;
                dropTime = dropTime - (gameTime.ElapsedGameTime.Milliseconds * mult);
            }


            dropTime = dropTime - gameTime.ElapsedGameTime.Milliseconds;
            if (dropTime < 0)
            {
                if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), down))
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
                                try
                                {

                                    gameBoard[piece.getCurRow() + i][piece.getCurColumn() + j] = piece.getCurPiece()[i][j];
                                }
                                catch (Exception err)
                                {
                                    Console.WriteLine(err.Message);
                                }

                                // Check to see if the top of the column needs to be updated:  
                                // (It won't if a previous block in this piece has been placed higher up on the board and already set the column top.)
                                if (columnTops[piece.getCurColumn() + j] >= piece.getCurRow() + i - 1)
                                {
                                    columnTops[piece.getCurColumn() + j] = piece.getCurRow() + i - 1;
                                }
                            }
                        }
                    }

                    clearRows();

                    piece.updatePiece();
                    try
                    {
                        ai.reset();
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                    ai.Pause = false;
                    if (!canMove(piece, piece.getCurRow() - 1, piece.getCurColumn(), down))
                    {
                        update = new updateDelegate(menuUpdate);
                        draw = new drawDelegate(menuDraw);
                        userInput.setMenuControl(true);
                    }
                    else
                    {
                        score += 10;
                    }
                }
                dropTime = dropSpeed;

            }


        }

        /// <summary>
        /// Allows the game to run logic needed for the game being played
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void gameUpdate(GameTime gameTime)
        {
            if (userInput.Pause && !userInput.PreviousPause)
            {
                if (pause)
                {
                    pause = false;
                }
                else
                {
                    pause = true;
                }
            }
            if (!pause)
            {
                KeyboardState state = Keyboard.GetState();
                bool slide = false;

                int right = 1;
                int down = 0;
                int left = -1;

                //if (state.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
                if (userInput.Right && !userInput.PreviousRight)
                {
                    //move one space to the right
                    if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), right))
                    {
                        piece.setCurColumn(piece.getCurColumn() + right);
                    }
                }

                //if (state.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
                if (userInput.Right && userInput.PreviousRight)
                {
                    slide = true;
                    //get time and move right accordingly
                    moveTime = moveTime - gameTime.ElapsedGameTime.Milliseconds;
                    if (moveTime < 0)
                    {
                        //move one space to the right
                        if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), right))
                        {
                            piece.setCurColumn(piece.getCurColumn() + right);
                        }
                        moveTime = 75;
                    }
                }

                // if (state.IsKeyDown(Keys.Left) && !oldState.IsKeyDown(Keys.Left))
                if (userInput.Left && !userInput.PreviousLeft)
                {
                    //move one space to the Left
                    if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), left))
                    {
                        piece.setCurColumn(piece.getCurColumn() + left);
                    }
                }

                //if (state.IsKeyDown(Keys.Left) && oldState.IsKeyDown(Keys.Left))
                if (userInput.Left && userInput.PreviousLeft)
                {
                    slide = true;
                    //get time and move Left accordingly
                    moveTime = moveTime - gameTime.ElapsedGameTime.Milliseconds;
                    if (moveTime < 0)
                    {
                        //move one space to the right
                        if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), left))
                        {
                            piece.setCurColumn(piece.getCurColumn() + left);
                        }
                        moveTime = 75;
                    }
                }

                //if (state.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
                if (userInput.Up && !userInput.PreviousUp)
                {
                    //move one space to the Left
                    piece.rotatePiece();
                    if (!canMove(piece, piece.getCurRow() - 1, piece.getCurColumn(), down))
                    {
                        //undo rotate
                        piece.rotatePiece();
                        piece.rotatePiece();
                        piece.rotatePiece();
                    }
                }

                if (userInput.Up && userInput.PreviousUp)
                {
                    //get time and move Left accordingly
                }

                if (userInput.Down)
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
                    if (canMove(piece, piece.getCurRow(), piece.getCurColumn(), down))
                    {
                        piece.setCurRow(piece.getCurRow() + 1);
                    }
                    else
                    {
                        effect.Play(1.0f);
                        for (int i = 0; i < piece.getCurPiece().Length; i++)
                        {
                            for (int j = 0; j < piece.getCurPiece().Length; j++)
                            {
                                if (piece.getCurPiece()[i][j] != EMPTY && (piece.getCurRow() + i) < ROWS
                                    && (piece.getCurRow() + i) > 0 && (piece.getCurColumn() + j) < COLUMNS)
                                {
                                    gameBoard[piece.getCurRow() + i][piece.getCurColumn() + j] = piece.getCurPiece()[i][j];
                                }

                            }
                        }

                        clearRows();

                        piece.updatePiece();
                        if (!canMove(piece, piece.getCurRow() - 1, piece.getCurColumn(), down))
                        {
                            userInput.setMenuControl(true);
                            update = new updateDelegate(scoresUpdate);
                            if (high.getRanking(score) != -1)
                            {
                                //new high score
                                highScore = true;
                                string[] updatedScore = new string[2];
                                updatedScore[0] = "";
                                updatedScore[1] = score.ToString();
                                high.upDateScores(updatedScore);
                                userInput.clearName();
                            }
                            else
                            {
                                highScore = false;
                            }

                        }
                        else
                        {
                            score += 10;
                        }
                    }
                    dropTime = dropSpeed;
                }
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
            if ((userInput.Enter && !userInput.PreviousEnter) || creditsFinished)
            {
                creditTime = 18000;
                creditsFinished = false;
                update = new updateDelegate(menuUpdate);
                draw = new drawDelegate(menuDraw);
            }
            creditTime = creditTime - gameTime.ElapsedGameTime.Milliseconds; if (creditTime < 0) { creditsFinished = true; }
        }

        /// <summary>
        /// Allows the game to run logic needed for the highScores
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void scoresUpdate(GameTime gameTime)
        {
            draw = new drawDelegate(scoreDraw);
            if (highScore)
            {
                if (userInput.Enter && !userInput.PreviousEnter)
                {
                    highScore = false;
                    effect.Play();
                    high.save();
                }

                high.updateScore(high.getRanking(score), userInput.Name);

            }
            else
            {
                if (userInput.Enter && !userInput.PreviousEnter)
                {
                    update = new updateDelegate(menuUpdate);
                    draw = new drawDelegate(menuDraw);
                }
            }
        }

        /// <summary>
        /// This is called when the menu should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void menuDraw(GameTime gameTime)
        {
            int x = 300;
            int y = 250;
            int space = 60;

            gm.drawDefaultBackground();

            if (selectedMenuItem == PLAY)
            {
                gm.drawString("Play Tetris", x, y, Color.Yellow, 1.5f);
            }
            else
            {
                gm.drawString("Play Tetris", x, y, Color.White, 1.5f);
            }

            if (selectedMenuItem == HIGH)
            {
                gm.drawString("High Scores", x, y + (space), Color.Yellow, 1.5f);
            }
            else
            {
                gm.drawString("High Scores", x, y + (space), Color.White, 1.5f);
            }

            if (selectedMenuItem == CREDITS)
            {
                gm.drawString("Credits", x, y + (space *2), Color.Yellow, 1.5f);
            }
            else
            {
                gm.drawString("Credits", x, y + (space *2), Color.White, 1.5f);
            }

            if (selectedMenuItem == EXIT)
            {
                gm.drawString("Exit", x, y + (space *3), Color.Yellow, 1.5f);
            }
            else
            {
                gm.drawString("Exit", x, y + (space *3), Color.White, 1.5f);
            }
        }

        /// <summary>
        /// This is called when the game being played should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void gameDraw(GameTime gameTime)
        {

            gm.drawBoard(gameBoard);
            // call this, passing the gameboard to draw the board

            gm.drawString(score.ToString(), 130, 225, Color.Black, 1.0f);
            gm.drawString(level.ToString(), 130, 310, Color.Black, 1.0f);
            gm.drawString(linesCleared.ToString(), 130, 400, Color.Black, 1.0f);

            gm.drawPiece(piece.getCurPiece(), 250 + (30 * piece.getCurColumn()), (30 * piece.getCurRow()));
            // call this to draw a piece: aPiece at x and y
            gm.drawPiece(piece.getNextPiece(), 620, 85);
            

        }

        /// <summary>
        /// This is called when the credits should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void creditsDraw(GameTime gameTime)
        {
            gm.drawCreditsBackground();

            gm.drawString("Tetris Tribute", 290, (creditTime / 8) - 1845, Color.White, 1.0f);
            gm.drawString("2009", 350, (creditTime / 8) - 1800, Color.White, 1.0f);

            gm.drawString("Jason Newbold", 350, (creditTime / 8) - 500, Color.White, 1.0f);
            gm.drawString("Game Play Coordinator", 290, (creditTime / 8) - 545, Color.White, 1.0f);

            gm.drawString("Jacob Dutson", 350, (creditTime / 8) - 1300, Color.White, 1.0f);
            gm.drawString("Artificial Intelligence", 290, (creditTime / 8) - 1345, Color.White, 1.0f);

            gm.drawString("Dallin Osmun", 350, (creditTime / 8) - 900, Color.White, 1.0f);
            gm.drawString("Graphic Designer", 290, (creditTime / 8) - 945, Color.White, 1.0f);

            gm.drawString("Special Thanks To", 290, (creditTime / 8) - 45, Color.White, 1.0f);
            gm.drawString("Dean Mathias", 350, (creditTime / 8), Color.White, 1.0f);

        }

        /// <summary>
        /// This is called when the HighScores should be draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void scoreDraw(GameTime gameTime)
        {
            gm.drawCreditsBackground();

            gm.drawString("High Scores", 250, 30, Color.White, 1.5f);
            for (int i = 0; i < 10; i++)
            {
                if (high.getRanking(score) != i)
                {
                    gm.drawString((i + 1) + ". " + high.getHighScores()[i][0], 100, (i * 50) + 80, Color.White, 1.0f);
                    gm.drawString(high.getHighScores()[i][1], 600, (i * 50) + 80, Color.White, 1.0f);
                }
                else
                {
                    gm.drawString((i + 1) + ". " + high.getHighScores()[i][0], 100, (i * 50) + 80, Color.Yellow, 1.0f);
                    gm.drawString(high.getHighScores()[i][1], 600, (i * 50) + 80, Color.Yellow, 1.0f);
                }

            }
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
            if (highScore && gamepad.IsConnected)
            {
                gm.drawString("Press X to advance letters, Press A when finished.", 150, 560, Color.Aquamarine, 1.0f);
            }

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
            bool cleared = false;
            int clearScore = 0;
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
                    cleared = true;
                    if (clearScore != 0)
                    {
                        clearScore = clearScore * 2;
                    }
                    else
                    {
                        clearScore = 100;
                    }
                    linesCleared++;
                    if (linesCleared % 10 == 0 && linesCleared <= 100)
                    {
                        level++;
                        dropSpeed = dropSpeed - 25;
                    }
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
                }
            }
            score += clearScore;

            if (cleared)
            {
                clearEffect.Play();
                gravityBoard = new int[ROWS][];
                for (int i = 0; i < ROWS; i++)
                {
                    gravityBoard[i] = new int[COLUMNS];
                }
                for (int i = 0; i < COLUMNS; i++)
                {
                    gravity((ROWS - 1), i);
                }
                compareGravity();
            }
        }


        private bool canMove(GamePiece curPiece, int curRow, int curColumn, int direction)
        {
            //direction  -1 left,  0 down,  1 right

            int down = (direction + 1) % 2;

            for (int i = 0; i < curPiece.getCurPiece().Length; i++)
            {
                for (int j = 0; j < curPiece.getCurPiece()[i].Length; j++)
                {
                    //check if at a wall 
                    if ((curRow + i + down) < 0)
                    {
                        //do nothing
                    }
                    else if ((curRow + i + down) >= ROWS || (curColumn + j + direction) >= COLUMNS || (curColumn + j + direction) < 0)
                    {
                        if (curPiece.getCurPiece()[i][j] != EMPTY)
                        {
                            return false;
                        }

                    }
                    else if (gameBoard[curRow + i + down][curColumn + j + direction] != EMPTY && curPiece.getCurPiece()[i][j] != EMPTY)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        int[][] gravityBoard;

        private void gravity(int curRow, int curColumn)
        {
            if (curRow >= 0 && curRow < ROWS && curColumn >= 0 && curColumn < COLUMNS)
            {
                if (gameBoard[curRow][curColumn] != EMPTY && gravityBoard[curRow][curColumn] != 1)
                {
                    gravityBoard[curRow][curColumn] = 1;

                    gravity(curRow - 1, curColumn);
                    gravity(curRow + 1, curColumn);
                    gravity(curRow, curColumn + 1);
                    gravity(curRow, curColumn - 1);
                }
            }
        }

        int[][] gravityPiece;
        GamePiece gPiece;

        private void compareGravity()
        {
            bool drop = false;
            gravityPiece = new int[ROWS][];
            for (int i = 0; i < ROWS; i++)
            {
                gravityPiece[i] = new int[COLUMNS];
                for (int j = 0; j < COLUMNS; j++)
                {
                    gravityPiece[i][j] = 0;
                }
            }
            for (int i = (ROWS - 1); i >= 0; i--)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    if (gameBoard[i][j] != EMPTY && gravityBoard[i][j] == EMPTY)
                    {
                        drop = true;
                        //drop peice
                        gravityPiece[i][j] = gameBoard[i][j];
                        gameBoard[i][j] = 0;
                    }
                }
            }
            if (drop)
            {
                gPiece.setCurPiece(gravityPiece);
                gPiece.setCurRow(0);
                gPiece.setCurColumn(0);

                while (canMove(gPiece, gPiece.getCurRow(), gPiece.getCurColumn(), 0))
                {
                    gPiece.setCurRow(gPiece.getCurRow() + 1);
                }


                for (int i = 0; i < gPiece.getCurPiece().Length; i++)
                {
                    for (int j = 0; j < gPiece.getCurPiece()[i].Length; j++)
                    {
                        if (gPiece.getCurPiece()[i][j] != EMPTY && (gPiece.getCurRow() + i) < ROWS
                            && (gPiece.getCurColumn() + j) < COLUMNS)
                        {
                            gameBoard[gPiece.getCurRow() + i][gPiece.getCurColumn() + j] = gPiece.getCurPiece()[i][j];
                        }
                    }
                }
                clearRows();

            }
        }
    }
}

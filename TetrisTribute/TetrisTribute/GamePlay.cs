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
        const int ROWS = 20; //about 18-22
        const int COLUMNS = 10;
       
        const int PLAY = 0;
        const int HIGH = 1;
        const int CREDITS = 2;
        const int EXIT = 3;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        HighScores high;
        GamePiece piece;
        String[][] highScores;
        private KeyboardState oldState;

        //delegate that is used to call the correct update function based on state
        delegate void updateDelegate(GameTime gameTime);
        //delegate that is used to call the correct draw function based on state
        delegate void drawDelegate(GameTime gameTime);

        updateDelegate update;
        drawDelegate draw;

        int[][] gameBoard;
        int score;
        double dropTime;
        double dropSpeed;
        bool creditsFinished;

        int selectedMenuItem;

        public GamePlay()
        {
            graphics = new GraphicsDeviceManager(this);
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
            score = 0;

            //TODO set drop speed
            dropTime = 5000;
            dropSpeed = 5000;
            creditsFinished = false;

            update = new updateDelegate(menuUpdate);
            draw = new drawDelegate(menuDraw);


            //initialize the game board
            gameBoard =new int[ROWS][];
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

        private void gameUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
            {
                //move one space to the right
            }

            if (state.IsKeyDown(Keys.Right) && oldState.IsKeyDown(Keys.Right))
            {
                //get time and move right accordingly
            }

            dropTime = dropTime - gameTime.ElapsedGameTime.Milliseconds;
            if (dropTime < 0)
            {
                //TODO drop piece
                dropTime = dropSpeed;
            }

            oldState = state;
        }

        private void menuUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            //TODO make sure key was released and add XBOX CONTROLS
            if (state.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
            {
                //change the selected menu item up one position
                //??DO we want to wrap the selection if so have an else = 3
                if (selectedMenuItem > 0)
                {
                    selectedMenuItem--;
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            if (state.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
            {
                //change selected item down one
                //??DO we want to wrap the selection if so have an else = 0
                if (selectedMenuItem < 3)
                {
                    selectedMenuItem++;
                    Console.WriteLine("Selection Changed to " + selectedMenuItem);
                }
            }
            if (state.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
            {
                //get selected item
                //change update and draw delegates
                if (selectedMenuItem == PLAY)
                {
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

            oldState = state;
        }

        private void creditsUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            
            //if user presses enter or esc return back to the menu
            if ((state.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
                || (state.IsKeyDown(Keys.Escape) && !oldState.IsKeyDown(Keys.Escape))
                || creditsFinished)
            {
                update = new updateDelegate(menuUpdate);
                draw = new drawDelegate(menuDraw);
            }
            exitTime = exitTime - gameTime.ElapsedGameTime.Milliseconds;

            oldState = state;
        }

        private void scoresUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            //if got high score
            //input to get name etc
            //enter to finish
            //else entertoexit
            oldState = state;
        }

        private void menuDraw(GameTime gameTime)
        {

        }

        private void gameDraw(GameTime gameTime)
        {

        }

        private void creditsDraw(GameTime gameTime)
        {
            //TODO set creditsFinished = true when credits are finished
        }

        private void scoreDraw(GameTime gameTime)
        {

        }

    }
}

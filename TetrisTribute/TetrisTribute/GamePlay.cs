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
        const int ROWS = 20; //about 18-22
        const int COLUMNS = 10;

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
            // TODO: Add your initialization logic here
            score = 0;
            //TODO set drop speed
            dropTime = 5000;
            dropSpeed = 5000;

            update = new updateDelegate(menuUpdate);


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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            draw(gameTime);

            base.Draw(gameTime);
        }

        private void gameUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

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

            if (state.IsKeyDown(Keys.Up))
            {
                //change selected item
            }
            if (state.IsKeyDown(Keys.Down))
            {
                //change selected item
            }
            if (state.IsKeyDown(Keys.Enter))
            {
                //get selected item
                //change update and draw delegates
                update = new updateDelegate (gameUpdate);
            }

            oldState = state;
        }

        private void creditsUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            oldState = state;
        }

        private void scoresUpdate(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

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

        }

        private void scoreDraw(GameTime gameTime)
        {

        }

    }
}

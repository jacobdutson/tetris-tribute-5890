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

using System.Text;

namespace TetrisTribute
{
    class GraphicsManager
    {
        const int TILESIZE = 32;

        GraphicsDeviceManager graphics;
        ContentManager m_content;
        SpriteBatch spriteBatch;
        Texture2D blocks;

        public GraphicsManager(GamePlay game)
        {
            graphics = game.graphics;
           // graphics = new GraphicsDeviceManager(game);
            m_content = new ContentManager(game.Services);
        }

        public void LoadContent(GamePlay game)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            // TODO: use this.Content to load your game content here\
            blocks = m_content.Load<Texture2D>(@"Content\blocks2");
        }

        public void drawPiece(int[][] aPiece, int x, int y)
        {
            spriteBatch.Begin();

            for (int i = 0; i < aPiece.Length; i++)
                for (int j = 0; j < aPiece[i].Length; j++)
                    if (aPiece[i][j] != 0)
                        spriteBatch.Draw(blocks, new Rectangle(x + j * TILESIZE, y + i * TILESIZE, TILESIZE, TILESIZE), new Rectangle(aPiece[i][j] * TILESIZE, 0, TILESIZE, TILESIZE), Color.White);
            spriteBatch.End();
        }

        public void drawBoard(int[][] gameboard)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(blocks, new Rectangle(0,0,240,600), new Rectangle(0, 0, TILESIZE, TILESIZE), Color.White);
            spriteBatch.Draw(blocks, new Rectangle(560,0,240,600), new Rectangle(0, 0, TILESIZE, TILESIZE), Color.White);
            spriteBatch.End();
            drawPiece(gameboard, 240, -32);
        }
    }
}

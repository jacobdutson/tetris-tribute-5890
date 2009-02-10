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
        const int TILESIZE = 30;
        const int WIDTH = 800;
        const int HEIGHT = 600;

        GraphicsDeviceManager graphics;
        ContentManager m_content;
        SpriteBatch spriteBatch;
        Texture2D blocks, tetrisbg, menubg, defaultbg, creditsbg, highbg;
        SpriteFont afont;
        
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
            afont = m_content.Load<SpriteFont>(@"Content\GameFont");
            tetrisbg = m_content.Load<Texture2D>(@"Content\TetrisBG");
            menubg = m_content.Load<Texture2D>(@"Content\MenuBg");
            defaultbg = m_content.Load<Texture2D>(@"Content\arcade84");
            creditsbg = m_content.Load<Texture2D>(@"Content\creditsbg");
            highbg = m_content.Load<Texture2D>(@"Content\highScorebg");
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
            spriteBatch.Draw(tetrisbg, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.End();
            drawPiece(gameboard, (WIDTH / 2 - TILESIZE * 5), 0);
        }

        public void drawMenuBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menubg, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.End();
        }

        public void drawDefaultBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(defaultbg, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.End();
        }

        public void drawCreditsBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(creditsbg, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.End();
        }

        public void drawScoreBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(highbg, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.End();
        }

        public void drawString(string aString, int x, int y, Color fontColor, float scale)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(afont, aString, new Vector2(x, y), fontColor, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}

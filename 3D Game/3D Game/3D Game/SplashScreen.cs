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


namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SplashScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Rectangle screenRectangle;
        Texture2D titleScreen;
        Texture2D instructions;
        Texture2D winScreenP1, winScreenP2;

        SoundManager soundManager;

        SpriteFont font;

        public SplashScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            titleScreen = Game.Content.Load<Texture2D>(@"Textures\SPLASHEND");
            instructions = Game.Content.Load<Texture2D>(@"Textures\SplashInstructions");
            winScreenP1 = Game.Content.Load<Texture2D>(@"Textures\GAMEOVER1");
            winScreenP2 = Game.Content.Load<Texture2D>(@"Textures\GAMEOVER2");
            font = Game.Content.Load<SpriteFont>(@"fonts\georgia");

            //Create sprite batch
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            int screenWidth;
            int screenHeight;
            screenWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                if (((Game1)Game).currentGameState == Game1.GameState.P1WIN ||
                    ((Game1)Game).currentGameState == Game1.GameState.P2WIN)
                {
                    ((Game1)Game).ChangeGameState(Game1.GameState.DANCING);
                    soundManager.selectSound.Play();
                    MediaPlayer.Play(soundManager.menuMusic);
                    MediaPlayer.IsRepeating = true;
                }
            if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                if (((Game1)Game).currentGameState == Game1.GameState.INSTRUCTIONS)
                {
                    ((Game1)Game).ChangeGameState(Game1.GameState.MENU);
                    soundManager.backSelectSound.Play();
                    //MediaPlayer.Play(soundManager.menuMusic);
                    //MediaPlayer.IsRepeating = true;
                }
                else if (((Game1)Game).currentGameState == Game1.GameState.P1WIN ||
                    ((Game1)Game).currentGameState == Game1.GameState.P2WIN)
                {
                    ((Game1)Game).ChangeGameState(Game1.GameState.MENU);
                    soundManager.selectSound.Play();
                    MediaPlayer.Play(soundManager.menuMusic);
                    MediaPlayer.IsRepeating = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1) && ((Game1)Game).currentGameState == Game1.GameState.MENU)
            {
                ((Game1)Game).ChangeGameState(Game1.GameState.INSTRUCTIONS);
                soundManager.selectSound.Play();
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (((Game1)Game).currentGameState == Game1.GameState.MENU)
                {
                    ((Game1)Game).ChangeGameState(Game1.GameState.PLAYING);
                    soundManager.selectSound.Play();
                }         
            }                               

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (((Game1)Game).currentGameState == Game1.GameState.INSTRUCTIONS)
            {
                spriteBatch.Draw(instructions,
                    screenRectangle, Color.White);
            }
            else if (((Game1)Game).currentGameState == Game1.GameState.MENU)
            {
                spriteBatch.Draw(titleScreen,
                    screenRectangle, Color.White);
            }
            else if (((Game1)Game).currentGameState == Game1.GameState.P1WIN)
            {
                spriteBatch.Draw(winScreenP1,
                    screenRectangle, Color.White);

                /*spriteBatch.DrawString(
                    font,
                    "Player 1 Wins!",
                    new Vector2(Game.Window.ClientBounds.Width / 2,
                        Game.Window.ClientBounds.Height) / 2,
                    Color.Red); 
                */
            }
            else if (((Game1)Game).currentGameState == Game1.GameState.P2WIN)
            {
                spriteBatch.Draw(winScreenP2,
                    screenRectangle, Color.White);

                /*spriteBatch.DrawString(
                    font,
                    "Player 2 Wins!",
                    new Vector2(Game.Window.ClientBounds.Width / 2,
                        Game.Window.ClientBounds.Height / 2),
                    Color.Red); 
                */
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void setSoundManager(SoundManager s)
        {
            soundManager = s;
        }
    }    
}

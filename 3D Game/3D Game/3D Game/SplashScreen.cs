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
        string textToDraw = "Start Game";
        string secondaryTextToDraw = "How to Play";
        SpriteFont font;
        SpriteFont menuFont;
        SpriteBatch spriteBatch;        

        public enum MenuOption { START, INSTRUCTIONS, NULL }
        public MenuOption currentOption = MenuOption.START;


        Texture2D titleScreen;
        Texture2D instructions;

        SoundManager soundManager;

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
            //Load fonts
            font = Game.Content.Load<SpriteFont>(@"fonts\georgia");
            menuFont = Game.Content.Load<SpriteFont>(@"fonts\menuFont");
            titleScreen = Game.Content.Load<Texture2D>(@"Textures\titleScreen");
            instructions = Game.Content.Load<Texture2D>(@"Textures\instructions");

            //Create sprite batch
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
              
            //Up assigns to start, Down assigns to instructions
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (((Game1)Game).currentGameState == Game1.GameState.MENU)
                    currentOption = MenuOption.START;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (((Game1)Game).currentGameState == Game1.GameState.MENU)
                    currentOption = MenuOption.INSTRUCTIONS;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                if (((Game1)Game).currentGameState == Game1.GameState.INSTRUCTIONS)                
                    ((Game1)Game).ChangeGameState(Game1.GameState.MENU);                
            }



            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                soundManager.selectSound.Play();

                switch (currentOption)
                {
                    case MenuOption.START:
                        ((Game1)Game).ChangeGameState(Game1.GameState.PLAYING);
                        currentOption = MenuOption.NULL;
                        break;

                    case MenuOption.INSTRUCTIONS:
                        ((Game1)Game).ChangeGameState(Game1.GameState.INSTRUCTIONS);
                        currentOption = MenuOption.NULL;
                        break;

                    /*case MenuOption.NULL:
                        if (((Game1)Game).currentGameState == Game1.GameState.INSTRUCTIONS)
                            ((Game1)Game).ChangeGameState(Game1.GameState.MENU);
                        break;*/
                }
            }                               

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (((Game1)Game).currentGameState == Game1.GameState.INSTRUCTIONS)
            {
                //Draw bg
                spriteBatch.Draw(instructions,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - instructions.Width / 2, Game.Window.ClientBounds.Height / 2
                        - instructions.Height / 2), Color.White);
            }
            else if (((Game1)Game).currentGameState == Game1.GameState.MENU)
            {
                //Draw bg
                spriteBatch.Draw(titleScreen,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - titleScreen.Width / 2, Game.Window.ClientBounds.Height / 2
                        - titleScreen.Height / 2), Color.White);
            }

            //Get size of string
            Vector2 TextSize = menuFont.MeasureString(textToDraw);

            //Assign color
            Color red = Color.Red;
            Color white = Color.White;

            if (currentOption == MenuOption.START)
            {
                //Draw text
                spriteBatch.DrawString(menuFont, textToDraw,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - TextSize.X / 2,
                        Game.Window.ClientBounds.Height / 2),
                        Color.Red);

                //Draw subtext
                spriteBatch.DrawString(menuFont, secondaryTextToDraw,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - menuFont.MeasureString(secondaryTextToDraw).X / 2,
                        Game.Window.ClientBounds.Height / 2 +
                        TextSize.Y + 10),
                        Color.White);
            }
            else if (currentOption == MenuOption.INSTRUCTIONS)
            {                
                //Draw text
                spriteBatch.DrawString(menuFont, textToDraw,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - TextSize.X / 2,
                        Game.Window.ClientBounds.Height / 2),
                        Color.White);

                //Draw subtext
                spriteBatch.DrawString(menuFont, secondaryTextToDraw,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - menuFont.MeasureString(secondaryTextToDraw).X / 2,
                        Game.Window.ClientBounds.Height / 2 +
                        TextSize.Y + 10),
                        Color.Red);
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

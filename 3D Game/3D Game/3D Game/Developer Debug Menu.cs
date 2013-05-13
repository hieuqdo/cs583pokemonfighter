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

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Developer_Debug_Menu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string p1, p2, stage;
        string speed1, speed2;
        string collision;
        float volume;
        SpriteFont font;
        SpriteBatch spriteBatch;

        public Developer_Debug_Menu(Game game)
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
            //Update p1's position
            p1 = "P1 Position: (" + ((Game1)Game).modelManager.getPosition1().X + 
                ", " + ((Game1)Game).modelManager.getPosition1().Y +
                ", " + ((Game1)Game).modelManager.getPosition1().Z + ")";

            //Update p2's position
            p2 = "P2 Position: (" + ((Game1)Game).modelManager.getPosition2().X +
                ", " + ((Game1)Game).modelManager.getPosition2().Y +
                ", " + ((Game1)Game).modelManager.getPosition2().Z + ")";

            //Update stage's position
            stage = "Stage Position: (" + ((Game1)Game).modelManager.getPositionStage().X +
                ", " + ((Game1)Game).modelManager.getPositionStage().Y +
                ", " + ((Game1)Game).modelManager.getPositionStage().Z + ")";

            //Update collision status
            collision = "P1 and P2 isColliding: " + ((Game1)Game).modelManager.isColliding();

            //Update p1 speed
            speed1 = "P1 Speed: " + ((Game1)Game).modelManager.getSpeed1();

            //Update p2 speed
            speed2 = "P2 Speed: " + ((Game1)Game).modelManager.getSpeed2();

            volume = SoundEffect.MasterVolume;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            float offset = 0;
         
            //Draw P1 position
            spriteBatch.DrawString(font, p1,
                new Vector2(10, 10), Color.Black);

            //Draw P2 position
            offset += font.MeasureString(p1).Y;
            spriteBatch.DrawString(font, p2,
                new Vector2(10, offset + 10), Color.Black);

            //Draw Stage position
            offset += font.MeasureString(p2).Y;
            spriteBatch.DrawString(font, stage,
                new Vector2(10, offset + 10), Color.Black);

            //Draw Collision status
            offset += font.MeasureString(stage).Y;
            spriteBatch.DrawString(font, collision,
                new Vector2(10, offset + 10), Color.Black);

            //Draw P1 speed
            offset += font.MeasureString(collision).Y;
            spriteBatch.DrawString(font, speed1,
                new Vector2(10, offset + 10), Color.Black);

            //Draw P2 speed
            offset += font.MeasureString(speed1).Y;
            spriteBatch.DrawString(font, speed2,
                new Vector2(10, offset + 10), Color.Black);

            //Draw currentGameState
            offset += font.MeasureString(speed2).Y;
            spriteBatch.DrawString(font, ((Game1)Game).currentGameState.ToString(),
                new Vector2(10, offset + 10), Color.Black);

            //Draw currentGameState
            //offset += font.MeasureString(speed2).Y;
            //spriteBatch.DrawString(font, ((Game1)Game).splashScreen.currentOption.ToString(),
            //    new Vector2(10, offset + 10), Color.Black);

            //Draw Volume Level
            offset += font.MeasureString(speed2).Y;
            spriteBatch.DrawString(font, volume.ToString(),
                new Vector2(10, offset + 10), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

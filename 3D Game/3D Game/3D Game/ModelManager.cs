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
    public class ModelManager : DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();
        BasicModel p1, p2, stage;

        public ModelManager(Game game)
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
            p1 = new Player1(
                Game.Content.Load<Model>(@"models\spaceship"));
            models.Add(p1);
            p2 = new Player2(
                Game.Content.Load<Model>(@"models\spaceship"));
            models.Add(p2);
            stage = new Stage1(
                Game.Content.Load<Model>(@"models\stage"));
            models.Add(stage);
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //Loop through all models and call Update
            for (int i = 0; i < models.Count; ++i)
            {
                models[i].Update();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Loop through and draw each model
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }

        public Vector3 getPosition1()
        {
            return p1.getPosition();
        }

        public Vector3 getPosition2()
        {
            return p2.getPosition();
        }

        public Vector3 getPositionStage()
        {
            return stage.getPosition();
        }

        public float getSpeed1()
        {
            return ((Player1)p1).getSpeed();
        }

        public float getSpeed2()
        {
            return ((Player1)p2).getSpeed();
        }

        public bool isColliding()
        {
            if (p1.CollidesWith(p2.model, p2.GetWorld()))
                return true;
            else return false;
        }

    }
}

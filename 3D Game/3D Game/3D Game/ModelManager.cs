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
        List<BasicModel> shots = new List<BasicModel>();
        BasicModel p1, p2, stage;
        Player1 player1, player2;
        BasicModel shield1, shield2;
        InteractionMediator mediator;

        SoundManager soundManager;

        SpriteBatch spriteBatch;
        SpriteFont percentFont;

        public enum sound { JUMP, DOUBLEJUMP, ATTACK, SMASH, SMASHHIT, 
                            SHOCK, SHOCKHIT, SHOCKSMASHHIT,
                            BULLET, SMASHBULLET,
                            SHIELD, SHIELDBREAK, PIKABREAK,
                            RESPAWN, DEATHCRY, DEATHBLAST }

        Texture2D stock;

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
            //Stage
            stage = new Stage1(
                Game.Content.Load<Model>(@"models\stage"));            

            //Players
            p1 = new Player1(
                Game.Content.Load<Model>(@"models\pikachu_final"));
            ((Player1)p1).setPosition(-25);
            models.Add(p1);
            p2 = new Player2(
                Game.Content.Load<Model>(@"models\pikachu_final"));
            ((Player1)p2).setPosition(25);
            models.Add(p2);

            shield1 = new BasicModel(
                Game.Content.Load<Model>(@"models\pokeball"));
            shield2 = new BasicModel(
                Game.Content.Load<Model>(@"models\pokeball"));

            //Lives
            stock = Game.Content.Load<Texture2D>(@"textures\stock");
            mediator = new InteractionMediator((Player1)p1, (Player1)p2);

            //Assign Mediator
            ((Player1)p1).mediator = mediator;
            ((Player1)p2).mediator = mediator;

            //Assign all the models this Model Manager
            foreach (BasicModel m in models)
                m.setModelManager(this);

            player1 = ((Player1)p1);
            player2 = ((Player1)p2);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            percentFont = Game.Content.Load<SpriteFont>(@"fonts\menuFont");

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
            stage.Update();

            UpdateShots();
            updateDeaths();
            updateFaceDirection();

            base.Update(gameTime);
        }

        private void updateFaceDirection()
        {
            if (getPosition1().X >= getPosition2().X)
            {
                if(!player1.moving)
                    player1.flipModifier = -1;
                if(!player2.moving)
                    player2.flipModifier = 1;
            }
            else
            {
                if (!player1.moving)
                    player1.flipModifier = 1;
                if (!player2.moving)
                    player2.flipModifier = -1;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Loop through and draw each model
            stage.Draw(((Game1)Game).camera);
            foreach (BasicModel bm in models)
            {
                bm.Draw(((Game1)Game).camera);
            }

            //Loop through and draw each shot
            foreach (BasicModel bm in shots)
            {
                bm.Draw(((Game1)Game).camera);
            }

            if (((Player1)p1).currStock > 0)
               drawPercentage(((Player1)p1).currPercentage, 1);
            if (((Player1)p2).currStock > 0)
               drawPercentage(((Player1)p2).currPercentage, 2);

            drawStock(((Player1)p1).currStock, 1);
            drawStock(((Player1)p2).currStock, 2);

            drawShields();

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

        public void reset()
        {
            models.Clear();
            LoadContent();
        }

        public void setSoundManager(SoundManager m)
        {
            soundManager = m;
        }

        public void playSound(sound s)
        {
            switch (s)
            {
                case sound.JUMP:
                    soundManager.jumpSound.Play();
                    break;

                case sound.DOUBLEJUMP:
                    soundManager.doubleJumpSound.Play();
                    break;

                case sound.ATTACK:
                    soundManager.attackSound.Play();
                    break;

                case sound.SMASH:
                    soundManager.smashAttackSound.Play();
                    break;

                case sound.SMASHHIT:
                    soundManager.smashHitSound.Play();
                    break;

                case sound.SHOCK:
                    soundManager.shockSound.Play();
                    break;

                case sound.SHOCKHIT:
                    soundManager.shockHitSound.Play();
                    break;

                case sound.SHOCKSMASHHIT:
                    soundManager.shockSmashHitSound.Play();
                    break;

                case sound.BULLET:
                    soundManager.bulletSound.Play();
                    break;

                case sound.SMASHBULLET:
                    soundManager.smashBulletSound.Play();
                    break;

                case sound.SHIELD:
                    soundManager.shieldSound.Play();
                    break;

                case sound.SHIELDBREAK:
                    soundManager.shieldBreakSound.Play();
                    break;

                case sound.PIKABREAK:
                    soundManager.pikaBreakSound.Play();
                    break;

                case sound.RESPAWN:
                    soundManager.respawnSound.Play();
                    break;

                case sound.DEATHCRY:
                    soundManager.deathCrySound.Play();
                    break;

                case sound.DEATHBLAST:
                    soundManager.deathExplosionSound.Play();
                    break;

            }
        }

        public void updateDeaths()
        {
            if ((getPosition1().Y > ((Stage1)stage).deathPlane_TOP ||
                getPosition1().Y < ((Stage1)stage).deathPlane_BOT ||
                getPosition1().X > ((Stage1)stage).deathPlane_LEFT ||
                getPosition1().X < ((Stage1)stage).deathPlane_RIGHT)
                && ((Player1)p1).isAlive == true)
            {
                ((Player1)p1).reset();
                ((Game1)Game).camera.rumble();
            }

            if ((getPosition2().Y > ((Stage1)stage).deathPlane_TOP ||
                getPosition2().Y < ((Stage1)stage).deathPlane_BOT ||
                getPosition2().X > ((Stage1)stage).deathPlane_LEFT ||
                getPosition2().X < ((Stage1)stage).deathPlane_RIGHT)
                && ((Player1)p2).isAlive == true)
            {
                ((Player1)p2).reset();
                ((Game1)Game).camera.rumble();
            }
        }

        public void drawShields()
        {
            spriteBatch.Begin();

            if (((Player1)p1).isShielding == true)
            {
                shield1.scale = player1.shieldScale / 5f;
                shield1.setWorld(player1.GetWorld());
                if (!models.Contains(shield1))
                    models.Add(shield1);
            }
            else
                if (models.Contains(shield1))
                    models.Remove(shield1);
            if (((Player1)p2).isShielding == true)
            {
                shield2.scale = player2.shieldScale / 5f;
                shield2.setWorld(player2.GetWorld());
                if (!models.Contains(shield2))
                    models.Add(shield2);
            }
            else
                if (models.Contains(shield2))
                    models.Remove(shield2);

            spriteBatch.End();
        }

        public void drawPercentage(int percent, int offset)
        {
            string temp = "" + percent + "%";

            spriteBatch.Begin();

            spriteBatch.DrawString(percentFont, temp,
                    new Vector2(
                        Game.Window.ClientBounds.Width / 7 * (offset * 2),                       
                        Game.Window.ClientBounds.Height / 5 * 4),
                        Color.White);

            spriteBatch.End();
        }

        public void drawStock(int lives, int offset)
        {
            spriteBatch.Begin();

            for (int i = 0; i < lives; i++)
            {
                spriteBatch.Draw(
                    stock,
                    new Vector2(
                        (Game.Window.ClientBounds.Width / 7 * (offset * 2) + i*23),
                        (Game.Window.ClientBounds.Height / 5 * 3) + 80),
                        Color.Yellow);
            }

            spriteBatch.End();
        }

        public void reportDeath(Object player)
        {
            if (player == player1)
                ((Game1)Game).ChangeGameState(Game1.GameState.P2WIN);
            else ((Game1)Game).ChangeGameState(Game1.GameState.P1WIN);
            MediaPlayer.Play(soundManager.themeMusic);
            MediaPlayer.IsRepeating = true;
        }

        public void AddShot(Vector3 position, Vector3 direction, 
                            Object owner, InteractionMediator.attackType attackType)
        {
            //Determine owner
            Player1 temp;
            if (owner == player1)
                temp = player1;
            else temp = player2;

            shots.Add(new Bullet(
                Game.Content.Load<Model>(@"models\ammo"),
                position, direction, temp, attackType));            
        }

        public void UpdateShots()
        {
            //Loop through shots
            for (int i = 0; i < shots.Count; i++)
            {
                //Update each shot
                ((Bullet)shots[i]).Update();

                //If shot is out of bounds, remove it from game
                if (shots[i].GetWorld().Translation.X > ((Stage1)stage).deathPlane_LEFT ||
                    shots[i].GetWorld().Translation.X < ((Stage1)stage).deathPlane_RIGHT)
                {
                    shots.RemoveAt(i);
                    --i;
                }
                else
                {
                    //If shot is still in play, check for collisions
                    for (int j = 0; j < models.Count; ++j)
                    {
                        if (models[j] != ((Bullet)shots[i]).getOwner() &&
                            shots[i].CollidesWith(models[j].model, models[j].GetWorld()))
                        {
                            //If attack collided, remove
                            if (mediator.processRangedAttack(
                                ((Bullet)shots[i]),
                                ((Player1)models[j]),
                                ((Bullet)shots[i]).getType()))
                            {                                
                                shots.RemoveAt(i);
                                --i;
                            }
                            break;
                        }                        
                    }

                }
            }
        }
    }
}

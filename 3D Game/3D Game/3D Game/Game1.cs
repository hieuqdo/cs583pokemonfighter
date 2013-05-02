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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; protected set; }
        ModelManager modelManager;

        public Random rnd { get; protected set; }

        float shotSpeed = 10;
        int shotDelay = 300;
        int shotCountdown = 0;

        Texture2D crosshairTexture;

        public enum GameState { START, PLAY, LEVEL_CHANGE, END }
        GameState currentGameState = GameState.START;

        SplashScreen splashScreen;
        int score = 0;

        SpriteFont scoreFont;

        int originalShotDelay = 300;
        public enum PowerUps { RAPID_FIRE }
        int shotDelayRapidFire = 100;
        int rapidFireTime = 10000;
        int powerUpCountdown = 0;
        string powerUpText = "";
        int powerUpTextTimer = 0;
        SpriteFont powerUpFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            rnd = new Random();

            graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 600;
            #if !DEBUG
                graphics.IsFullScreen = true;
            #endif
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
            camera = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            modelManager.Enabled = false;
            modelManager.Visible = false;

            //Splash screen component
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.SetData("Welcome to Space Defender!", 
                currentGameState);


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

            // TODO: use this.Content to load your game content here
            crosshairTexture = Content.Load<Texture2D>(@"textures\crosshair");
            scoreFont = Content.Load<SpriteFont>(@"Fonts\ScoreFont");
            powerUpFont = Content.Load<SpriteFont>(@"fonts\PowerupFont");
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

            // TODO: Add your update logic here
            //Only check for shots if you're in the play game state
            if (currentGameState == GameState.PLAY)
            {
                //See if the player has fired a shot
                FireShots(gameTime);
            }

            //Update power-up timer
            UpdatePowerUp(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);


            //Only draw crosshair if in play game state
            if (currentGameState == GameState.PLAY)
            {
                spriteBatch.Begin();

                spriteBatch.Draw(crosshairTexture,
                    new Vector2((Window.ClientBounds.Width / 2)
                        - (crosshairTexture.Width / 2),
                        (Window.ClientBounds.Height / 2)
                        - (crosshairTexture.Height / 2)),
                        Color.White);

                //Draw the current score
                string scoreText = "Score: " + score;
                spriteBatch.DrawString(scoreFont, scoreText,
                    new Vector2(10, 10), Color.Red);

                //Let the player know how many misses he has left
                spriteBatch.DrawString(scoreFont, "Misses Left: " +
                    modelManager.missesLeft,
                    new Vector2(10, scoreFont.MeasureString(scoreText).Y + 20),
                    Color.Red);

                //Let the player know how many models are left
                spriteBatch.DrawString(scoreFont, "Enemies Left: " +
                    modelManager.getModelsCount(),
                    new Vector2(10, scoreFont.MeasureString(scoreText).Y + 40),
                    Color.Red);

                //Display consecutive kills
                spriteBatch.DrawString(scoreFont, "Consecutive kills: " +
                    modelManager.consecutiveKills,
                    new Vector2(10, scoreFont.MeasureString(scoreText).Y + 60),
                    Color.Red);


                //If power-up text timer is live, draw power-up text
                if (powerUpTextTimer > 0)
                {
                    powerUpTextTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    Vector2 textSize = powerUpFont.MeasureString(powerUpText);
                    spriteBatch.DrawString(powerUpFont,
                        powerUpText,
                        new Vector2((Window.ClientBounds.Width / 2) -
                            (textSize.X / 2),
                            (Window.ClientBounds.Height / 2) -
                            (textSize.Y / 2)),
                            Color.Goldenrod);
                }

                spriteBatch.End();
            }
        }

        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                //Did player press space bar or left mouse button?
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    //Add a shot to the model manager
                    modelManager.AddShot(
                        camera.cameraPosition + new Vector3(0, -5, 0),
                        camera.GetCameraDirection * shotSpeed);

                    //Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public void ChangeGameState(GameState state, int level)
        {
            currentGameState = state;
            CancelPowerUps();

            switch (currentGameState)
            {
                case GameState.LEVEL_CHANGE:
                    splashScreen.SetData("Level " + (level + 1),
                        GameState.LEVEL_CHANGE);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    //Stop the soundtrack loop
                    //trackCue.Stop(AudioStopOptions.Immediate);
                    break;

                case GameState.PLAY:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;

                    //if (trackCue.IsPlaying)
                    //trackCue.Stop(AudioStopOptions.Immediate);

                    //To play a stopped cue, get the cue from the soundbank again
                    //trackCue = soundBank.GetCue("Tracks");
                    //trackCue.Play();
                    break;

                case GameState.END:
                    splashScreen.SetData("Game Over.\nLevel: " + (level + 1) +
                        "\nScore: " + score, GameState.END);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;

                    //Stop the soundtrack loop
                    //trackCue.Stop(AudioStopOptions.Immediate);
                    break;
            }
        }

        public void AddPoints(int points)
        {
            score += points;
        }

        private void CancelPowerUps()
        {
            shotDelay = originalShotDelay;
            modelManager.consecutiveKills = 0;
        }

        protected void UpdatePowerUp(GameTime gameTime)
        {
            if (powerUpCountdown > 0)
                powerUpCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            if (powerUpCountdown <= 0)
            {
                CancelPowerUps();
                powerUpCountdown = 0;
            }
        }

        public void StartPowerUp(PowerUps powerUp)
        {
            switch (powerUp)
            {
                case PowerUps.RAPID_FIRE:
                    shotDelay = shotDelayRapidFire;
                    powerUpCountdown = rapidFireTime;
                    powerUpText = "Rapid Fire Mode!";
                    powerUpTextTimer = 1000;
                    //soundBank.PlayCue("RapidFire");
                    break;
            }
        }
    }
}

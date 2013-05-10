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
        public ModelManager modelManager;
        public SoundManager soundManager;
        public Developer_Debug_Menu debug;

        public int writeFrequency = 0;
        private KeyboardState newState, oldState;


        public enum GameState { MENU, PLAYING, INSTRUCTIONS, P1WIN, P2WIN }
        public GameState currentGameState = GameState.MENU;

        public SplashScreen splashScreen;

        Texture2D bg;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
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
            camera = new Camera(this, new Vector3(0, 0, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            camera.addModelManager(modelManager);
            soundManager = new SoundManager();
            modelManager.setSoundManager(soundManager);

            debug = new Developer_Debug_Menu(this);

            oldState = Keyboard.GetState();

            modelManager.Enabled = false;
            modelManager.Visible = false;

            //Splash screen component
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);
            splashScreen.setSoundManager(soundManager);

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
            bg = Content.Load<Texture2D>(@"textures\stadium_bg");
    
            soundManager.LoadContent(Content);
            MediaPlayer.Play(soundManager.menuMusic);
            MediaPlayer.IsRepeating = true;            
            
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
            newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.OemTilde) && oldState.IsKeyUp(Keys.OemTilde) && currentGameState == Game1.GameState.PLAYING)
                toggleDebug();

            if (newState.IsKeyDown(Keys.OemMinus) && oldState.IsKeyUp(Keys.OemMinus))
                soundManager.decreaseVolume();
            if (newState.IsKeyDown(Keys.OemPlus) && oldState.IsKeyUp(Keys.OemPlus))
                soundManager.increaseVolume();
            MediaPlayer.Volume = soundManager.volume;

            if (newState.IsKeyDown(Keys.F2) && oldState.IsKeyUp(Keys.F2))
                toggleFullscreen();

            if (newState.IsKeyDown(Keys.Z) &&
                newState.IsKeyDown(Keys.X) &&
                newState.IsKeyDown(Keys.C) &&
                newState.IsKeyDown(Keys.V))
                if (currentGameState == Game1.GameState.PLAYING)
                {
                    ChangeGameState(Game1.GameState.MENU);
                    MediaPlayer.Play(soundManager.menuMusic);
                    MediaPlayer.IsRepeating = true;
                }

            oldState = newState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (currentGameState == GameState.PLAYING)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(
                    bg,
                    new Rectangle(0, 0, 
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight), 
                    Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
        protected void writeToGame(GameTime gameTime)
        {
            //Console.Out.WriteLine("test writeline");
        }

        private void toggleDebug()
        {
            if (Components.Contains(debug))
                Components.Remove(debug);
            else Components.Add(debug);
        }

        private void toggleFullscreen()
        {
            if (graphics.IsFullScreen)
                graphics.IsFullScreen = false;
            else graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        public void ChangeGameState(GameState state)
        {
            currentGameState = state;         

            switch (currentGameState)
            {
                case GameState.PLAYING:
                    modelManager.reset();
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;

                    MediaPlayer.Play(soundManager.battleMusic);
                    MediaPlayer.IsRepeating = true;
                    break;

                case GameState.MENU:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;          
                    break;

                case GameState.INSTRUCTIONS:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;                  
                    break;

                case GameState.P1WIN:
                case GameState.P2WIN:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;                  
                    break;
            }        
        }
    }
}

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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera { get; protected set; }
        public ModelManager modelManager;
        public SoundManager soundManager;
        public Developer_Debug_Menu debug;

        public int writeFrequency = 0;
        private KeyboardState newState, oldState;
        private GamePadState newGamepadState, oldGamepadState;

        public enum GameState { MENU, PLAYING, INSTRUCTIONS_KEY, INSTRUCTIONS_PAD,
                                P1WIN, P2WIN, DANCING, INTRO }
        public GameState currentGameState = GameState.INTRO;

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
            oldGamepadState = GamePad.GetState(PlayerIndex.One);

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
            //MediaPlayer.Play(soundManager.menuMusic);
            //MediaPlayer.IsRepeating = true;            
            
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true ||
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            newState = Keyboard.GetState();
            newGamepadState = GamePad.GetState(PlayerIndex.One);
            if (newState.IsKeyDown(Keys.OemTilde) && oldState.IsKeyUp(Keys.OemTilde) && currentGameState == Game1.GameState.PLAYING)
                toggleDebug();

            if (newState.IsKeyDown(Keys.OemMinus) && oldState.IsKeyUp(Keys.OemMinus) ||
                (newGamepadState.IsButtonDown(Buttons.DPadDown) && oldGamepadState.IsButtonUp(Buttons.DPadDown)))
                soundManager.decreaseVolume();
            if (newState.IsKeyDown(Keys.OemPlus) && oldState.IsKeyUp(Keys.OemPlus) ||
                (newGamepadState.IsButtonDown(Buttons.DPadUp) && oldGamepadState.IsButtonUp(Buttons.DPadUp)))
                soundManager.increaseVolume();
            MediaPlayer.Volume = soundManager.volume;

            if (newState.IsKeyDown(Keys.F2) && oldState.IsKeyUp(Keys.F2) ||
                newGamepadState.IsButtonDown(Buttons.RightShoulder))
                toggleFullscreen();

            if ((newState.IsKeyDown(Keys.Z) &&
                newState.IsKeyDown(Keys.X) &&
                newState.IsKeyDown(Keys.C) &&
                newState.IsKeyDown(Keys.V)) ||
                (newGamepadState.IsButtonDown(Buttons.LeftTrigger) &&
                newGamepadState.IsButtonDown(Buttons.RightTrigger) &&
                newGamepadState.IsButtonDown(Buttons.Y) &&
                newGamepadState.IsButtonDown(Buttons.B)))
                if (currentGameState == Game1.GameState.PLAYING)
                {
                    ChangeGameState(Game1.GameState.MENU);
                    MediaPlayer.Play(soundManager.menuMusic);
                    MediaPlayer.IsRepeating = true;
                }

            oldState = newState;
            oldGamepadState = newGamepadState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            if (currentGameState == GameState.PLAYING)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Draw(
                    bg,
                    new Rectangle(0, 0, 
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight), 
                        Color.White);
            }
            else if (currentGameState == GameState.INTRO)
                GraphicsDevice.Clear(Color.White);

            spriteBatch.End();

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
                case GameState.DANCING:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    splashScreen.startClip();
                    break;
                case GameState.INTRO:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    break;
                default:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;                  
                    break;
            }        
        }
    }
}

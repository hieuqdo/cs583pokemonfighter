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
using SkinnedModel;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SplashScreen : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        Rectangle screenRectangle;
        Texture2D titleScreen, instructions_key, instructions_pad, introLogo, FUNK1, FUNK2;
        Texture2D winScreenP1, winScreenP2;

        SoundManager soundManager;

        SpriteFont font;

        KeyboardState oldState, newState;
        GamePadState oldGamepadState, newGamepadState;

        // ANIMATIONS
        Model animatedModel;
        AnimationPlayer animationPlayer;
        AnimationClip animationClip;
        float cameraArc = 0;
        float cameraRotation = 0;
        float cameraDistance = 150;

        //FADE INTRO
        float mCurrentAlpha = 1.0f;
        double mUpdateAlphaInterval = .9f;
        float mAlphaIncrement = .01f;
        bool fadingIn = true;
        double pause = 1500f;

        public SplashScreen(Game game)
            : base(game)
        {
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
            //ANIMATION
            animatedModel = Game.Content.Load<Model>(@"models\pikachu_notail");
            SkinningData skinningData = animatedModel.Tag as SkinningData;
            if (skinningData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");
            animationPlayer = new AnimationPlayer(skinningData);
            animationClip = skinningData.AnimationClips["Take 001"];

            titleScreen = Game.Content.Load<Texture2D>(@"Textures\MENU");
            instructions_key = Game.Content.Load<Texture2D>(@"Textures\INSTRUCTIONS_KEY");
            instructions_pad = Game.Content.Load<Texture2D>(@"Textures\INSTRUCTIONS_PAD");
            FUNK1 = Game.Content.Load<Texture2D>(@"Textures\FUNK");
            FUNK2 = Game.Content.Load<Texture2D>(@"Textures\blackfunk");
            introLogo = Game.Content.Load<Texture2D>(@"Textures\team");
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
            newState = Keyboard.GetState();
            newGamepadState = GamePad.GetState(PlayerIndex.One);

            if (((Game1)Game).currentGameState == Game1.GameState.DANCING)
            {
                //ANIMATION
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);        
            }
            if (((Game1)Game).currentGameState == Game1.GameState.INTRO)
            {
                updateIntro(gameTime);
            }

            //If pressed Backspace (or B)
            if (Keyboard.GetState().IsKeyDown(Keys.Back) ||
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
            {
                switch (((Game1)Game).currentGameState)
                {
                    //During Instructions, go back to Menu
                    case Game1.GameState.INSTRUCTIONS_KEY:
                    case Game1.GameState.INSTRUCTIONS_PAD:
                        ((Game1)Game).ChangeGameState(Game1.GameState.MENU); 
                        soundManager.backSelectSound.Play();
                        break;

                    //During WinScreen, go back to Menu
                    case Game1.GameState.P1WIN:
                    case Game1.GameState.P2WIN:
                        ((Game1)Game).ChangeGameState(Game1.GameState.MENU); 
                        soundManager.selectSound.Play();
                        MediaPlayer.Play(soundManager.menuMusic);
                        MediaPlayer.IsRepeating = true;
                        break;

                    //During Dancing, go back to Menu
                    case Game1.GameState.DANCING:
                        ((Game1)Game).ChangeGameState(Game1.GameState.MENU); 
                        MediaPlayer.Play(soundManager.menuMusic);
                        MediaPlayer.IsRepeating = true;
                        break;
                }                           
            }

            //If pressed F1
            if ((Keyboard.GetState().IsKeyDown(Keys.F1)))
            {              
                switch (((Game1)Game).currentGameState)
                {
                    //During Menu, go into Instructions
                    case Game1.GameState.MENU:
                        ((Game1)Game).ChangeGameState(Game1.GameState.INSTRUCTIONS_KEY);
                        soundManager.selectSound.Play();
                        break;
                }        
            }
            //If pressed Left Shoulder
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftShoulder))
            {
                switch (((Game1)Game).currentGameState)
                {
                    //During Menu, go into Instructions
                    case Game1.GameState.MENU:
                        ((Game1)Game).ChangeGameState(Game1.GameState.INSTRUCTIONS_PAD);
                        soundManager.selectSound.Play();
                        break;
                }
            }

            //If pressed Enter (or Start)
            if ((newState.IsKeyDown(Keys.Enter) &&
                oldState.IsKeyUp(Keys.Enter))||
                (newGamepadState.IsButtonDown(Buttons.Start) &&
                oldGamepadState.IsButtonUp(Buttons.Start)))
            {
                switch (((Game1)Game).currentGameState)
                {
                    //During Menu, go into Playing
                    case Game1.GameState.MENU:
                        ((Game1)Game).ChangeGameState(Game1.GameState.PLAYING);
                        soundManager.selectSound.Play();
                        break;

                    //During WinScreen, go into Dancing
                    case Game1.GameState.P1WIN:
                    case Game1.GameState.P2WIN:
                        ((Game1)Game).ChangeGameState(Game1.GameState.DANCING);
                        soundManager.selectSound.Play();
                        MediaPlayer.Play(soundManager.danceMusic);
                        MediaPlayer.IsRepeating = true;
                        break;

                    //During Intro, go into Menu
                    case Game1.GameState.INTRO:
                        ((Game1)Game).ChangeGameState(Game1.GameState.MENU);
                        MediaPlayer.Play(soundManager.menuMusic);
                        MediaPlayer.IsRepeating = true;
                        break;
                }        
            }

            oldState = newState;
            oldGamepadState = newGamepadState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            switch (((Game1)Game).currentGameState)
            {
                case Game1.GameState.INSTRUCTIONS_KEY:
                    spriteBatch.Draw(instructions_key, screenRectangle, Color.White);
                    break;

                case Game1.GameState.INSTRUCTIONS_PAD:
                    spriteBatch.Draw(instructions_pad, screenRectangle, Color.White);
                    break;

                case Game1.GameState.MENU:
                    spriteBatch.Draw(titleScreen, screenRectangle, Color.White);
                    break;

                case Game1.GameState.P1WIN:
                    spriteBatch.Draw(winScreenP1, screenRectangle, Color.White);
                    break;

                case Game1.GameState.P2WIN:
                    spriteBatch.Draw(winScreenP2, screenRectangle, Color.White);
                    break;

                case Game1.GameState.DANCING:
                    drawDance(gameTime);
                    break;

                case Game1.GameState.INTRO:
                    drawIntro();
                    break;
            }    

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void setSoundManager(SoundManager s)
        {
            soundManager = s;
        }

        public void startClip()
        {
            animationPlayer.StartClip(animationClip);
        }

        public void drawDance(GameTime gameTime)
        {
            GraphicsDevice device = ((Game1)Game).graphics.GraphicsDevice;

            device.Clear(Color.Black);
            if(gameTime.TotalGameTime.Milliseconds % 800 < 400)
                spriteBatch.Draw(FUNK2, screenRectangle,
                    Color.Lerp(Color.White, Color.White, mCurrentAlpha));
            else
                spriteBatch.Draw(FUNK1, screenRectangle,
                    Color.Lerp(Color.White, Color.White, mCurrentAlpha));
            spriteBatch.End();
            spriteBatch.Begin();

            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Compute camera matrices.
            Matrix view = Matrix.CreateTranslation(0, -20, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    device.Viewport.AspectRatio,
                                                                    1,
                                                                    10000);

            // Render the skinned mesh.
            foreach (ModelMesh mesh in animatedModel.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }

        public void updateIntro(GameTime gameTime)
        {
            mUpdateAlphaInterval -= gameTime.ElapsedGameTime.TotalSeconds;

            if (fadingIn && mCurrentAlpha == 0)
            {
                if (pause > 0)
                    pause -= gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    pause = 1500f;
                    fadingIn = false;
                }
            }
            if (!fadingIn && mCurrentAlpha == 1)
            {
                if (pause > 0)
                    pause -= gameTime.ElapsedGameTime.Milliseconds;
                else
                {                   
                    MediaPlayer.Play(soundManager.menuMusic);
                    MediaPlayer.IsRepeating = true;
                    ((Game1)Game).ChangeGameState(Game1.GameState.MENU);
                }
            }

            if (mUpdateAlphaInterval <= 0)
            {
                if (fadingIn)
                {
                    mCurrentAlpha -= mAlphaIncrement;
                    if (mCurrentAlpha < 0 || mCurrentAlpha > 1)
                    {
                        mCurrentAlpha = MathHelper.Clamp(mCurrentAlpha, 0, 1);
                    }
                }
                else if (!fadingIn)
                {
                    mCurrentAlpha += mAlphaIncrement;
                    if (mCurrentAlpha < 0 || mCurrentAlpha > 1)
                    {
                        mCurrentAlpha = MathHelper.Clamp(mCurrentAlpha, 0, 1);
                    }
                }
            }  
        }

        public void drawIntro()
        {
            spriteBatch.Draw(introLogo, screenRectangle,
                Color.Lerp(Color.White, Color.Transparent, mCurrentAlpha));
        }
    }    
}

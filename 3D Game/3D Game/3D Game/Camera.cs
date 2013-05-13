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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera Vectors
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        float farLeft;
        float farRight;

        ModelManager modelMan;
        Vector3 posP1;
        Vector3 posP2;

        float minDist = 60;

        //bool isRumbling = false;
        //bool shook = false;
        float rumbleCooldownTimer = 0;
        float rumbleCooldown = 3000;

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // TODO: Construct any child components here
            //Build camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 3000);
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {           
            //Register farLeft, farRight
            posP1 = modelMan.getPosition1();
            posP2 = modelMan.getPosition2();
            if (posP1.X < posP2.X)
                farLeft = posP1.X;
            else  farLeft = posP2.X;
            if (posP1.X > posP2.X)
                farRight = posP1.X;
            else  farRight = posP2.X;

            /*
            //If rumbling, rumble
            if (rumbleCooldownTimer > 0)
            {
                isRumbling = true;

                if (shook)
                {
                    cameraPosition = new Vector3(cameraPosition.X - 50,
                                       cameraPosition.Y - 50,
                                       cameraPosition.Z);
                    shook = false;
                }
                else
                {
                    cameraPosition = new Vector3(cameraPosition.X + 50,
                                       cameraPosition.Y + 50,
                                       cameraPosition.Z);
                    shook = true;
                }


                CreateLookAt();

                //Tick Cooldown Timer
                rumbleCooldownTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else //do normal camera
            {*/
                //isRumbling = false;

                //Adjust camera position
                if (Math.Abs(posP1.X - posP2.X) / .5f > minDist || Math.Abs(posP1.Y - posP2.Y)/.5f > minDist)
                    cameraPosition = new Vector3((posP1.X + posP2.X) / 2, (posP1.Y + posP2.Y) / 2, Math.Max(Math.Abs(posP1.X - posP2.X) / .5f, Math.Abs(posP1.Y - posP2.Y) / .5f));
                else cameraPosition = new Vector3((posP1.X + posP2.X) / 2, (posP1.Y + posP2.Y) / 2, minDist);
                //Console.Out.WriteLine("Camera Zoom (Z) = " + Math.Abs(posP1.X - posP2.X) / .5f);

                //Recreate the camera view matrix
                CreateLookAt();
            /*}*/

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }

        public void addModelManager(ModelManager mm)
        {
            modelMan = mm;
        }

        public void rumble()
        {
            rumbleCooldownTimer = rumbleCooldown;
        }
    }
}

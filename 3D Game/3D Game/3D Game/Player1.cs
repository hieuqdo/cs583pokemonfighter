using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3D_Game
{
    class Player1 : SpinningEnemy
    {
        Matrix Ytranslation = Matrix.Identity;
        Matrix Xtranslation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix rollingMatrix;
        Vector3 left = new Vector3(-1,0,0);
        Vector3 right = new Vector3(1,0,0);

        bool jumping = false;
        bool rolling = false;
        float jumpMomentum = 0;
        float speed = 1;
        float rollingTimer = 0;


        public Player1(Model m)
            : base(m)
        {
        }

        public override void Update()
        {
            ApplyGravity();
            UpdateRoll();
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Jump();
            //if (Keyboard.GetState().IsKeyDown(Keys.Down))
                //crouch
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift))
                    Roll(left);
                else
                    Move(left);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.RightShift))
                    Roll(right);
                else
                    Move(right);
            }
        }
          
        public override Matrix GetWorld()
        {
            return world * Ytranslation * Xtranslation * rotation;
        }

        private void ApplyGravity()
        {
            if (jumping)
            {
                jumpMomentum -= 0.04f;
                if (jumpMomentum < -1)
                {
                    jumpMomentum = 0;
                    jumping = false;
                    speed = 1;
                }
            }
            Ytranslation *= Matrix.CreateTranslation(new Vector3(0, jumpMomentum, 0));
        }

        private void UpdateRoll()
        {
            if (rolling)
            {
                rollingTimer -= 0.02f;
                if (rollingTimer < 0)
                {      
                    rolling = false;
                    rotation = Matrix.Identity;
                }
                //else
                //    Xtranslation *= rollingMatrix;
            }
        }

        private void Move(Vector3 direction)
        {
            Xtranslation *= Matrix.CreateTranslation(direction * speed);
        }

        private void Jump()
        {
            if (!jumping)
            {
                speed = 0.4f;
                jumpMomentum = 1;
                jumping = true;
            }
        }

        private void Roll(Vector3 direction)
        {
            if (!rolling)
            {
                //make invulnerable for X frames..
                //rollingMatrix = Matrix.CreateTranslation(direction * 2 * speed);
                rotation *= Matrix.CreateRotationZ(MathHelper.Pi/90);
                rolling = true;
                rollingTimer = 1;
            }
        }
    }
}

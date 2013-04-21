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
        // CONSTANTS
        //
        const int PLAYER_SPEED = 1;
        const float ROLL_TIME = .2f;
        const float ROLL_COOLDOWN = .1f;
        const int ROLL_SPEED = 3;
        const int JUMP_TIME = 1;
        const float JUMP_SPEED = 0.4f;
        const float GRAVITY = 0.05f;
        const float TIME_COUNTDOWN = 0.01666666666f;

        Matrix Ytranslation = Matrix.Identity;
        Matrix Xtranslation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix rollingTranslation;
        Matrix rollingRotation;
        Vector3 left = new Vector3(-1,0,0);
        Vector3 right = new Vector3(1,0,0);

        bool jumping = false;
        bool rolling = false;
        float jumpMomentum = 0;
        float rollingTimer = 0;
        float speed = PLAYER_SPEED;
        float rollCooldown = 0;


        public Player1(Model m)
            : base(m)
        {
        }

        public override void Update()
        {
            ApplyGravity();
            UpdateRoll();
            ReadKeyboardInput();
        }

        private void ReadKeyboardInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                Jump();
            if (Keyboard.GetState().IsKeyDown(Keys.RightShift))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    Roll(left);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    Roll(right);
                }
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                    Move(left);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                    Move(right);
            }
        }
          
        public override Matrix GetWorld()
        {
            return world * rotation * Ytranslation * Xtranslation;
        }

        private void ApplyGravity()
        {
            if (jumping)
            {
                jumpMomentum -= GRAVITY;
                if (jumpMomentum < -1)
                {
                    jumpMomentum = 0;
                    jumping = false;
                    speed = PLAYER_SPEED;
                }
            }
            Ytranslation *= Matrix.CreateTranslation(new Vector3(0, jumpMomentum, 0));
        }

        private void UpdateRoll()
        {
            if (rolling)
            {
                rotation *= rollingRotation;
                Xtranslation *= rollingTranslation;
                rollingTimer -= TIME_COUNTDOWN;
                if (rollingTimer < 0)
                {
                    rolling = false;
                    rotation = Matrix.Identity;
                    rollCooldown = ROLL_COOLDOWN;
                }
            }
            else
            {
                if(rollCooldown > 0)
                    rollCooldown -= TIME_COUNTDOWN;
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
                speed = JUMP_SPEED;
                jumpMomentum = 1;
                jumping = true;
            }
        }

        private void Roll(Vector3 direction)
        {
            if (!rolling && rollCooldown <= 0)
            {
                //TODO: make invulnerable for X frames..
                rolling = true;
                rollingTranslation = Matrix.CreateTranslation(direction * 3 * speed);
                rollingRotation = Matrix.CreateRotationZ(direction.X * -1 * MathHelper.Pi/15);
                rollingTimer = ROLL_TIME;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3D_Game
{
    class Player1 : BasicModel
    {
        // CONSTANTS
        // general
        const int PLAYER_RUN_SPEED = 1;
        const int PLAYER_SPRINT_SPEED = 2;
        const float SPRINT_KEYPRESS_INTERVAL = 0.3f;
        const float TIME_COUNTDOWN = 0.01666666666f;
        const int NO_SPRINT_STATE = -1;
        const int FIRST_PRESS_STATE = 0;
        const int FIRST_RELEASE_STATE = 1;
        const int SECOND_PRESS_STATE = 2;
        // rolling
        const float ROLL_TIME = .2f;
        const float ROLL_COOLDOWN = .1f;
        const int ROLL_SPEED = 3;
        // jumping
        const int JUMP_TIME = 1;
        const float JUMP_MOMENTUM = 2f;
        const float DOUBLEJUMP_MOMENTUM = 2f;
        const float JUMP_SPEED = 0.4f;
        const float GRAVITY = 0.1f;
        const float JUMP_COOLDOWN = .1f;

        Matrix Ytranslation = Matrix.Identity;
        Matrix Xtranslation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix rollingTranslation;
        Matrix rollingRotation;
        Vector3 left = new Vector3(-1,0,0);
        Vector3 right = new Vector3(1,0,0);

        //history
        bool doubleJumped = false;
        bool jumping = false;
        bool rolling = false;
        bool sprintingLeft = false;
        bool sprintingRight = false;
        float jumpMomentum = 0;
        float rollingTimer = 0;
        float sprintCheckTimer = 0;
        float sprintCheckTimer2 = 0;
        float speed = PLAYER_RUN_SPEED;
        float rollCooldown = 0;
        float jumpCooldown = 0;
        KeyboardState oldState, newState;

        protected Keys upKey;
        protected Keys downKey;
        protected Keys leftKey;
        protected Keys rightKey;
        protected Keys shieldKey;

        public Player1(Model m)
            : base(m)
        {
            upKey = Keys.Up;
            downKey = Keys.Down;
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            shieldKey = Keys.RightShift;
            tint = Color.MediumVioletRed;
            oldState = Keyboard.GetState();
        }

        public override void Update()
        {
            newState = Keyboard.GetState();
            TickCooldowns();
            ApplyGravity();
            UpdateRoll();
            CheckSprint();
            ReadKeyboardInput();
            oldState = newState;
        }

        private void ReadKeyboardInput()
        {
            if (newState.IsKeyDown(upKey))
            {
                if (!jumping && oldState.IsKeyUp(upKey))
                    Jump();
                else if (oldState.IsKeyUp(upKey))
                    DoubleJump();
            }
            if (Keyboard.GetState().IsKeyDown(shieldKey))
            {
                if (Keyboard.GetState().IsKeyDown(leftKey))
                {
                    if (oldState.IsKeyUp(leftKey))
                    {
                        Roll(left);
                    }
                }
                if (Keyboard.GetState().IsKeyDown(rightKey))
                {
                    if (oldState.IsKeyUp(rightKey))
                    {
                        Roll(right);
                    }
                }
                return;
            }
            if (Keyboard.GetState().IsKeyDown(leftKey))
            {
                if (!rolling){
                    Move(left); 
                }
            }
            if (Keyboard.GetState().IsKeyDown(rightKey))
            {
                if(!rolling)
                    Move(right);
            }
        }
          
        public override Matrix GetWorld()
        {
            return world * rotation * Ytranslation * Xtranslation;
        }

        private void CheckSprint()
        {
            
        }

        private void shield()
        {
            
        }

        private void ApplyGravity()
        {
            if (jumping)
            {
                jumpMomentum -= GRAVITY;
                if (ModelReachedGround())
                {
                    jumpMomentum = 0;
                    jumping = false;
                    doubleJumped = false;
                    //speed = PLAYER_SPEED;
                    Ytranslation = Matrix.Identity;
                }
            }
            Ytranslation *= Matrix.CreateTranslation(new Vector3(0, jumpMomentum, 0));
        }

        private bool ModelReachedGround()
        {
            Matrix nextPosition =
                GetWorld() * Matrix.CreateTranslation(new Vector3(0, jumpMomentum, 0));
            if (nextPosition.Translation.Y < 0)
                return true;
            return false;
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
        }

        private void Move(Vector3 direction)
        {
            Xtranslation *= Matrix.CreateTranslation(direction * speed);
        }

        private void Jump()
        {
            if (!jumping)
            {
                //speed = JUMP_SPEED;
                jumpMomentum = JUMP_MOMENTUM;
                jumping = true;
                jumpCooldown = JUMP_COOLDOWN;
            }
        }

        private void DoubleJump()
        {
            if (jumpCooldown <= 0 && doubleJumped == false)
            {
                jumpMomentum = DOUBLEJUMP_MOMENTUM;
                doubleJumped = true;
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

        private void TickCooldowns()
        {
            if (jumpCooldown > 0)
                jumpCooldown -= TIME_COUNTDOWN;
            if (rollCooldown > 0)
                rollCooldown -= TIME_COUNTDOWN;
            if (sprintCheckTimer > 0)
                sprintCheckTimer -= TIME_COUNTDOWN;
        }
    }
}

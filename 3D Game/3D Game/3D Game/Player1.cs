using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    public class Player1 : BasicModel
    {
        // CONSTANTS
        // general
        const int PLAYER_RUN_SPEED = 1;
        const float PLAYER_SPRINT_SPEED = 1.75f;
        const float SPRINT_KEYPRESS_INTERVAL = 0.3f;
        const float SMASH_KEYPRESS_INTERVAL = 0.2f;
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
        const float LATERAL_MOMENTUM = 1.2f;
        const float JUMP_MINIMUM_MOMENTUM = -2f;
        // smashing
        const float SMASH_TIME = .2f;
        const float SMASH_COOLDOWN = .4f;
        const float SMASH_SPEED = 1;

        public Color DEFAULT_TINT = Color.Tan;
        public InteractionMediator mediator;

        Matrix Ytranslation = Matrix.Identity;
        Matrix Xtranslation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix rollingTranslation;
        Matrix smashingTranslation;
        Matrix rollingRotation;
        Vector3 left = new Vector3(-1,0,0);
        Vector3 right = new Vector3(1,0,0);

        //history
        bool doubleJumped = false;
        public bool jumping = false;
        public bool rolling = false;        
        public bool moving = false;
        bool sprintingLeft = false;
        bool sprintingRight = false;
        public bool isAlive = true; //for determining death camera
        public bool isShielding = false; //model manager will draw shield
        public float jumpMomentum = 0;
        float lateralMomentum = 0;
        public float stunTimer = 0;
        float rollingTimer = 0;
        float deathTimer = -1;
        float sprintCheckTimerLeft = 0;
        float sprintCheckTimerLeft2 = 0;
        float sprintCheckTimerRight = 0;
        float sprintCheckTimerRight2 = 0;
        float speed = PLAYER_RUN_SPEED;
        float rollCooldown = 0;
        float smashCooldown = 0;
        float jumpCooldown = 0;
        float smashTimerLeft = 0;
        float smashTimerRight = 0;
        public float shieldScale = 10f;
        KeyboardState oldState, newState;
        public int flipModifier = 1;

        //Controls
        GamePadState oldGamepadState, newGamepadState;
        public PlayerIndex myPlayerIndex;
        //Keyboard
        public Keys upKey;
        protected Keys downKey;
        protected Keys leftKey;
        protected Keys rightKey;
        protected Keys shieldKey;
        protected Keys attackKey;
        protected Keys secondAttackKey;
        //Gamepad
        public Buttons upButton;
        protected Buttons downButton;
        protected Buttons leftButton;
        protected Buttons rightButton;
        protected Buttons shieldButton1;
        protected Buttons shieldButton2;
        protected Buttons attackButton;
        protected Buttons secondAttackButton;
        protected Buttons smashLeft;
        public Buttons smashUp;
        protected Buttons smashRight;

        public int currPercentage;
        public int maxPercentage = 999;
        public int currStock;

        public Player1(Model m)
            : base(m)
        {
            //Player specific
            //Keyboard
            upKey = Keys.W;
            downKey = Keys.S;
            leftKey = Keys.A;
            rightKey = Keys.D;
            shieldKey = Keys.LeftShift;
            attackKey = Keys.Q;
            secondAttackKey = Keys.E;
            //Gamepad
            upButton = Buttons.LeftThumbstickUp;
            downButton = Buttons.LeftThumbstickDown;
            leftButton = Buttons.LeftThumbstickLeft;
            rightButton = Buttons.LeftThumbstickRight;
            shieldButton1 = Buttons.LeftTrigger;
            shieldButton2 = Buttons.RightTrigger;
            attackButton = Buttons.A;
            secondAttackButton = Buttons.X;
            smashLeft = Buttons.RightThumbstickLeft;
            smashRight = Buttons.RightThumbstickRight;
            smashUp = Buttons.RightThumbstickUp;

            tint = DEFAULT_TINT;
            myPlayerIndex = PlayerIndex.One;
            
            //Adjustable
            oldState = Keyboard.GetState();
            currPercentage = 0;
            currStock = 3;
        }

        public override void Update()
        {
            newState = Keyboard.GetState();
            newGamepadState = GamePad.GetState(myPlayerIndex);
            TickCooldowns();
            ApplyGravity();
            ApplyFriction();
            UpdateRoll();       
            updateRespawn(); //respawn char if deathTimer is ready
            CheckSprintLeft();
            CheckSprintRight();
            if (stunTimer <= 0)
            {
                if (smashCooldown <= 0)
                {
                    ReadKeyboardInput();
                    ReadAttackInput();
                }
                
                tint = DEFAULT_TINT;
            }
            else
                tint = Color.Red;
            oldState = newState;
            oldGamepadState = newGamepadState;
        }

        private void ReadKeyboardInput()
        {
            //Only read input if alive
            if (isAlive)
            {
                // Process Shielding
                if (newState.IsKeyDown(shieldKey) ||//Read Keyboard
                    newGamepadState.IsButtonDown(shieldButton1) ||//Read gamepad
                    newGamepadState.IsButtonDown(shieldButton2))
                {
                    //if (!inAir)
                    if (!rolling)
                        shield();
                    if ((newState.IsKeyDown(leftKey) && oldState.IsKeyUp(leftKey)) ||
                        (newGamepadState.IsButtonDown(leftButton) && oldGamepadState.IsButtonUp(leftButton)))
                        Roll(left);
                    if ((newState.IsKeyDown(rightKey) && oldState.IsKeyUp(rightKey)) ||
                        (newGamepadState.IsButtonDown(rightButton) && oldGamepadState.IsButtonUp(rightButton)))
                        Roll(right);
                    return;
                }
                else isShielding = false;

                //Only read other input if not shielding
                if (!isShielding)
                {
                    //Process Jumping for Keyboard
                    if (newState.IsKeyDown(upKey))
                    {
                        if (!jumping && oldState.IsKeyUp(upKey))
                            Jump();
                        else if (oldState.IsKeyUp(upKey))
                            DoubleJump();
                    }
                    //Process Jumping for GamePad
                    if (newGamepadState.IsButtonDown(upButton) ||
                        newGamepadState.IsButtonDown(Buttons.Y) ||
                        newGamepadState.IsButtonDown(Buttons.B))
                    {
                        if (!jumping && oldGamepadState.IsButtonUp(upButton) &&
                            oldGamepadState.IsButtonUp(Buttons.Y) &&
                            oldGamepadState.IsButtonUp(Buttons.B))
                            Jump();
                        else if (oldGamepadState.IsButtonUp(upButton) &&
                            oldGamepadState.IsButtonUp(Buttons.Y) &&
                            oldGamepadState.IsButtonUp(Buttons.B))
                            DoubleJump();
                    }

                    //Process left and right
                    if (newState.IsKeyDown(leftKey) ||//Read Keyboard
                        (newGamepadState.ThumbSticks.Left.X < 0))//Read Gamepad
                    {
                        if (oldState.IsKeyUp(leftKey) && oldGamepadState.IsButtonUp(leftButton))
                            smashTimerLeft = SMASH_KEYPRESS_INTERVAL;
                        if (!rolling)
                            Move(left);
                    }
                    else if (oldState.IsKeyDown(leftKey) || newGamepadState.ThumbSticks.Left.X == -1)
                    {
                        lateralMomentum = -speed * 1.2f;
                        rotation = Matrix.Identity;
                        moving = false;
                    }
                    if (newState.IsKeyDown(rightKey) ||//Read Keyboard
                        (newGamepadState.ThumbSticks.Left.X > 0))//Read Gamepad
                    {
                        if (oldState.IsKeyUp(rightKey) && oldGamepadState.IsButtonUp(rightButton))
                            smashTimerRight = SMASH_KEYPRESS_INTERVAL;
                        if (!rolling)
                            Move(right);
                    }
                    else if (oldState.IsKeyDown(rightKey) || newGamepadState.ThumbSticks.Left.X == 1)
                    {
                        lateralMomentum = speed * 1.2f;
                        rotation = Matrix.Identity;
                        moving = false;
                    }
                }
            }
        }

        private void Smash()
        {
            Xtranslation *= smashingTranslation;
            if (mediator.attack(this, InteractionMediator.attackType.SMASH))
            {
                myModelManager.playSound(ModelManager.sound.SMASHHIT);
                myModelManager.playSound(ModelManager.sound.SHOCK);
                    
            }
            smashCooldown = SMASH_COOLDOWN;
        }

        public void ReadAttackInput()
        {
            //Only read input if not shielding
            if (!isShielding)
            {
                // If smash left
                if ((((newState.IsKeyDown(attackKey) && newState.IsKeyDown(leftKey)) ||
                    (newGamepadState.IsButtonDown(attackButton) && newGamepadState.IsButtonDown(leftButton)))
                    &&
                    (smashTimerLeft > 0)) || 
                    (newGamepadState.IsButtonDown(smashLeft) && oldGamepadState.IsButtonUp(smashLeft)))
                {
                    myModelManager.playSound(ModelManager.sound.SMASH);
                    Vector3 direction = new Vector3(-1, 0, 0);
                    if (smashCooldown <= 0)
                    {
                        smashingTranslation = Matrix.CreateTranslation(direction * SMASH_SPEED);
                        Smash();
                    }
                }
                // If smash right
                else if ((((newState.IsKeyDown(attackKey) && newState.IsKeyDown(rightKey)) ||
                    (newGamepadState.IsButtonDown(attackButton) && newGamepadState.IsButtonDown(rightButton)))
                    &&
                    (smashTimerRight > 0)) || (newGamepadState.IsButtonDown(smashRight) && oldGamepadState.IsButtonUp(smashRight)))
                {
                    myModelManager.playSound(ModelManager.sound.SMASH);
                    Vector3 direction = new Vector3(1, 0, 0);
                    if (smashCooldown <= 0)
                    {
                        smashingTranslation = Matrix.CreateTranslation(direction * SMASH_SPEED);
                        Smash();
                    }
                }
                // If smash bullet left
                if (((newState.IsKeyDown(secondAttackKey) && newState.IsKeyDown(leftKey)) ||
                    (newGamepadState.IsButtonDown(secondAttackButton) && newGamepadState.IsButtonDown(leftButton)))
                    &&
                    (smashTimerLeft > 0))
                {
                    myModelManager.playSound(ModelManager.sound.SMASHBULLET);
                    Vector3 direction = new Vector3(-1, 0, 0);
                    if (smashCooldown <= 0)
                    {
                        mediator.rangedAttack(this, InteractionMediator.attackType.SMASHBULLET);
                        smashCooldown = SMASH_COOLDOWN;
                    }
                }
                // If smash bullet right
                else if (((newState.IsKeyDown(secondAttackKey) && newState.IsKeyDown(rightKey)) ||
                    (newGamepadState.IsButtonDown(secondAttackButton) && newGamepadState.IsButtonDown(rightButton)))
                    &&
                    (smashTimerRight > 0))
                {
                    myModelManager.playSound(ModelManager.sound.SMASHBULLET);
                    Vector3 direction = new Vector3(1, 0, 0);
                    if (smashCooldown <= 0)
                    {
                        mediator.rangedAttack(this, InteractionMediator.attackType.SMASHBULLET);
                        smashCooldown = SMASH_COOLDOWN;
                    }
                }
                // If basic attack
                else if ((newState.IsKeyDown(attackKey) && oldState.IsKeyUp(attackKey) && oldState.IsKeyUp(leftKey) && oldState.IsKeyUp(rightKey)) ||
                    (newGamepadState.IsButtonDown(attackButton) && oldGamepadState.IsButtonUp(attackButton) && oldGamepadState.IsButtonUp(leftButton)  && oldGamepadState.IsButtonUp(rightButton)))
                {
                    if (mediator.attack(this, InteractionMediator.attackType.BASIC) == true)
                        myModelManager.playSound(ModelManager.sound.ATTACK);
                }
                // If bullet attack
                else if ((newState.IsKeyDown(secondAttackKey) && oldState.IsKeyUp(secondAttackKey)) ||
                     (newGamepadState.IsButtonDown(secondAttackButton) && oldGamepadState.IsButtonUp(secondAttackButton)))
                {
                    if (mediator.rangedAttack(this, InteractionMediator.attackType.BULLET) == true)
                        myModelManager.playSound(ModelManager.sound.BULLET);
                }
                // smash up
                else if (GamePad.GetState(myPlayerIndex).IsButtonDown(smashUp) && oldGamepadState.IsButtonUp(smashUp))
                {
                    if (mediator.attack(this, InteractionMediator.attackType.BASIC) == true)
                        myModelManager.playSound(ModelManager.sound.SMASH);
                }
            }
        }
          
        public override Matrix GetWorld()
        {
            return Matrix.CreateScale(flipModifier,1,1) * world * rotation * Ytranslation * Xtranslation;
        }

        private void CheckSprintLeft()
        {
            if (sprintingLeft)
            {
                if (newState.IsKeyUp(leftKey) && newGamepadState.ThumbSticks.Left.X > -1)
                    sprintingLeft = false;
            }
            else
            {
                if (newState.IsKeyDown(leftKey) && oldState.IsKeyUp(leftKey))
                    sprintCheckTimerLeft = SPRINT_KEYPRESS_INTERVAL;
                else if (newState.IsKeyUp(leftKey) && sprintCheckTimerLeft > 0)
                    sprintCheckTimerLeft2 = SPRINT_KEYPRESS_INTERVAL;
                if (newState.IsKeyDown(leftKey) && sprintCheckTimerLeft2 > 0 ||
                    (newGamepadState.ThumbSticks.Left.X == -1 &&
                     (oldGamepadState.ThumbSticks.Left.X > -.25f)))
                    sprintingLeft = true;                
            }
        }

        private void CheckSprintRight()
        {
            if (sprintingRight)
            {             
                if (newState.IsKeyUp(rightKey) && newGamepadState.ThumbSticks.Left.X < 1)
                    sprintingRight = false;
            }
            else
            {
                if (newState.IsKeyDown(rightKey) && oldState.IsKeyUp(rightKey))
                    sprintCheckTimerRight = SPRINT_KEYPRESS_INTERVAL;
                else if (newState.IsKeyUp(rightKey) && sprintCheckTimerRight > 0)
                    sprintCheckTimerRight2 = SPRINT_KEYPRESS_INTERVAL;
                if (newState.IsKeyDown(rightKey) && sprintCheckTimerRight2 > 0 ||
                    (newGamepadState.ThumbSticks.Left.X == 1 &&
                     (oldGamepadState.ThumbSticks.Left.X < .25f)))
                    sprintingRight = true;
            }
            if (sprintingRight || sprintingLeft)
                speed = PLAYER_SPRINT_SPEED;
            else
                speed = PLAYER_RUN_SPEED;
        }

        private void shield()
        {
            rotation = Matrix.Identity;
            isShielding = true;
            shieldScale -= TIME_COUNTDOWN * 3;
            if (shieldScale <= 0)
            {
                myModelManager.playSound(ModelManager.sound.SHIELDBREAK);
                myModelManager.playSound(ModelManager.sound.PIKABREAK);
                stunTimer = 5;
                shieldScale = 5f;
                isShielding = false;
            }
            
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
                else
                    if (jumpMomentum <= JUMP_MINIMUM_MOMENTUM)
                        jumpMomentum = JUMP_MINIMUM_MOMENTUM;
            }
            Ytranslation *= Matrix.CreateTranslation(new Vector3(0, jumpMomentum, 0));
        }

        private void ApplyFriction()
        {
            if (lateralMomentum > 0)
            {
                lateralMomentum -= GRAVITY;
                if (lateralMomentum < 0)
                {
                    lateralMomentum = 0;
                }
            }
            if (lateralMomentum < 0)
            {
                lateralMomentum += GRAVITY;
                if (lateralMomentum > 0)
                {
                    lateralMomentum = 0;
                }
            }
            Xtranslation *= Matrix.CreateTranslation(new Vector3(lateralMomentum, 0, 0));
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
            flipModifier = (int)direction.X;
            float divider = 7f;
            if (sprintingLeft || sprintingRight)
                divider = 2.8f;
            rotation = Matrix.CreateRotationZ(-flipModifier * (float)Math.PI / divider);
            Xtranslation *= Matrix.CreateTranslation(direction * speed);            
            moving = true;
        }

        private void Jump()
        {
            if (!jumping)
            {
                //speed = JUMP_SPEED;
                jumpMomentum = JUMP_MOMENTUM;
                jumping = true;
                jumpCooldown = JUMP_COOLDOWN;
                myModelManager.playSound(ModelManager.sound.JUMP);
            }
        }

        private void DoubleJump()
        {
            if (jumpCooldown <= 0 && doubleJumped == false)
            {
                jumpMomentum = DOUBLEJUMP_MOMENTUM;
                doubleJumped = true;
                myModelManager.playSound(ModelManager.sound.DOUBLEJUMP);
            }
        }

        private void Roll(Vector3 direction)
        {
            if (!rolling && rollCooldown <= 0)
            {
                //TODO: make invulnerable for X frames..
                isShielding = false;
                rolling = true;
                rollingTranslation = Matrix.CreateTranslation(direction * ROLL_SPEED);
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
            if (smashCooldown > 0)
            {
                if(smashCooldown - TIME_COUNTDOWN <= 0)
                    rotation = Matrix.Identity; 
                smashCooldown -= TIME_COUNTDOWN;
            }
            if (smashTimerLeft > 0)
                smashTimerLeft -= TIME_COUNTDOWN;
            if (smashTimerRight > 0)
                smashTimerRight -= TIME_COUNTDOWN;
            if (sprintCheckTimerLeft > 0)
                sprintCheckTimerLeft -= TIME_COUNTDOWN;
            if (sprintCheckTimerLeft2 > 0)
                sprintCheckTimerLeft2 -= TIME_COUNTDOWN;
            if (sprintCheckTimerRight > 0)
                sprintCheckTimerRight -= TIME_COUNTDOWN;
            if (sprintCheckTimerRight2 > 0)
                sprintCheckTimerRight2 -= TIME_COUNTDOWN;
            if (shieldScale <= 5 && !rolling)
                shieldScale += TIME_COUNTDOWN * 2;
            if (stunTimer > 0)
                stunTimer -= TIME_COUNTDOWN;
            if (deathTimer - TIME_COUNTDOWN > 0)
                deathTimer -= TIME_COUNTDOWN;
            else if (deathTimer > 0)
                deathTimer = 0;
        }

        public float getSpeed()
        {
            return speed;
        }

        public void reset()
        {
            myModelManager.playSound(ModelManager.sound.DEATHCRY);
            myModelManager.playSound(ModelManager.sound.DEATHBLAST);
            isAlive = false;
            rotation = Matrix.Identity;
            if (currStock > 0)
            {
                --currStock;                      
                currPercentage = 0;
                lateralMomentum = 0;
                deathTimer = 2;               
            }
        }

        public void updateRespawn()
        {
            if (deathTimer == 0 && currStock > 0)
            {
                deathTimer = -1;
                Xtranslation = Matrix.Identity;
                myModelManager.playSound(ModelManager.sound.RESPAWN);
                isAlive = true;
            }
            else if (deathTimer == 0)
                myModelManager.reportDeath(this);
        }

        public void knockback(float momentum)
        {
            lateralMomentum += momentum;
        }

        public void setPosition(float xdistance)
        {
            Xtranslation *= Matrix.CreateTranslation(new Vector3(xdistance, 0, 0));
        }

        public override void Draw(Camera camera)
        {
            if (isAlive)
                base.Draw(camera);
        }
    }
}

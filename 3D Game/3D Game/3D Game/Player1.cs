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
        const float SMASH_KEYPRESS_INTERVAL = 0.3f;
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
        // smashing
        const float SMASH_TIME = .2f;
        const float SMASH_COOLDOWN = .1f;
        const float SMASH_SPEED = 1;

        public Color DEFAULT_TINT = Color.MediumVioletRed;
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
        bool jumping = false;
        public bool rolling = false;
        public bool smashing = false;
        bool smashHit = false;
        bool sprintingLeft = false;
        bool sprintingRight = false;
        bool inAir = false; //detect for shielding, maybe aerials
        public bool isAlive = true; //for determining death camera
        public bool isShielding = false; //model manager will draw shield
        float jumpMomentum = 0;
        float lateralMomentum = 0;
        public float stunTimer = 0;
        float rollingTimer = 0;
        float smashTimer = 0;
        float deathTimer = -1;
        float sprintCheckTimerLeft = 0;
        float sprintCheckTimerLeft2 = 0;
        float sprintCheckTimerRight = 0;
        float sprintCheckTimerRight2 = 0;
        float speed = PLAYER_RUN_SPEED;
        float rollCooldown = 0;
        float smashCooldown = 0;
        float jumpCooldown = 0;
        KeyboardState oldState, newState;
        public Texture2D myShield;
        public Vector2 shieldOrigin;
        public Color shieldColor = Color.Green;

        protected Keys upKey;
        protected Keys downKey;
        protected Keys leftKey;
        protected Keys rightKey;
        protected Keys shieldKey;
        protected Keys attackKey;

        public int currPercentage;
        public int maxPercentage = 999;
        public int currStock;

        public Player1(Model m)
            : base(m)
        {
            upKey = Keys.Up;
            downKey = Keys.Down;
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            shieldKey = Keys.RightShift;
            attackKey = Keys.OemQuestion;
            tint = DEFAULT_TINT;
            oldState = Keyboard.GetState();
            
            currPercentage = 0;
            currStock = 2;
        }

        public override void Update()
        {
            newState = Keyboard.GetState();
            if (getPosition().Y > 0)
                inAir = true;
            else inAir = false;
            TickCooldowns();
            ApplyGravity();
            ApplyFriction();
            UpdateRoll();
            UpdateSmash();
            updateRespawn(); //respawn char if deathTimer is ready
            updateShield(); //draw and/or adjust shield
            CheckSprintLeft();
            CheckSprintRight();
            if (stunTimer <= 0)
            {
                ReadKeyboardInput();
                ReadAttackInput();
                tint = DEFAULT_TINT;
            }
            else
                tint = Color.Red;
            oldState = newState;
        }

        private void ReadKeyboardInput()
        {
            //Only read input if alive
            if (isAlive)
            {
                // Process Shielding
                if (newState.IsKeyDown(shieldKey))
                {
                    //if (!inAir)
                    if (!rolling)
                        shield();
                    if (newState.IsKeyDown(leftKey) && oldState.IsKeyUp(leftKey))
                        Roll(left);
                    if (newState.IsKeyDown(rightKey) && oldState.IsKeyUp(rightKey))
                        Roll(right);
                    return;
                }
                else isShielding = false;

                //Only read other input if not shielding
                if (!isShielding)
                {
                    //Process Jumping
                    if (newState.IsKeyDown(upKey))
                    {
                        if (!jumping && oldState.IsKeyUp(upKey))
                            Jump();
                        else if (oldState.IsKeyUp(upKey))
                            DoubleJump();
                    }

                    //Process left and right
                    if (newState.IsKeyDown(leftKey))
                    {
                        if (!rolling)
                            Move(left);
                    }
                    else if (oldState.IsKeyDown(leftKey))
                        lateralMomentum = -speed * 1.2f;
                    if (newState.IsKeyDown(rightKey))
                    {
                        if (!rolling)
                            Move(right);
                    }
                    else if (oldState.IsKeyDown(rightKey))
                        lateralMomentum = speed * 1.2f;
                }
            }
        }

        private void UpdateSmash()
        {
            if (smashing)
            {
                Xtranslation *= smashingTranslation;
                smashTimer -= TIME_COUNTDOWN;
                if (mediator.smashAttack(this))
                {
                    myModelManager.playSound(ModelManager.sound.SMASHHIT);
                    myModelManager.playSound(ModelManager.sound.SHOCK);
                    smashHit = true;
                }
                if (smashTimer <= 0)
                {
                    smashing = false;
                    smashHit = false;
                    smashCooldown = SMASH_COOLDOWN;                    
                }
            }
        }

        public void ReadAttackInput()
        {
            //Only read input if not shielding
            if (!isShielding)
            {
                // If smash left
                if (newState.IsKeyDown(attackKey) && newState.IsKeyDown(leftKey) &&
                    (oldState.IsKeyUp(attackKey) && oldState.IsKeyUp(leftKey)) &&
                    !smashing)
                {
                    myModelManager.playSound(ModelManager.sound.SMASH);
                    Vector3 direction = new Vector3(-1, 0, 0);
                    if (!smashing && smashCooldown <= 0)
                    {
                        smashing = true;
                        smashingTranslation = Matrix.CreateTranslation(direction * SMASH_SPEED);
                        smashTimer = SMASH_TIME;
                    }
                }
                // If smash right
                else if (newState.IsKeyDown(attackKey) && newState.IsKeyDown(rightKey) &&
                    (oldState.IsKeyUp(attackKey) && oldState.IsKeyUp(rightKey)) &&
                    !smashing)
                {
                    myModelManager.playSound(ModelManager.sound.SMASH);
                    Vector3 direction = new Vector3(1, 0, 0);
                    if (!smashing && smashCooldown <= 0)
                    {
                        smashing = true;
                        smashingTranslation = Matrix.CreateTranslation(direction * SMASH_SPEED);
                        smashTimer = SMASH_TIME;
                    }
                }
                // If normal attack
                else if (newState.IsKeyDown(attackKey) && oldState.IsKeyUp(attackKey))
                {
                    if (!smashing && mediator.attack(this) == true)
                        myModelManager.playSound(ModelManager.sound.ATTACK);
                }
            }
        }
          
        public override Matrix GetWorld()
        {
            return world * rotation * Ytranslation * Xtranslation;
        }

        private void CheckSprintLeft()
        {
            if (sprintingLeft)
            {
                if (newState.IsKeyUp(leftKey))
                    sprintingLeft = false;
            }
            else
            {
                if (newState.IsKeyDown(leftKey) && oldState.IsKeyUp(leftKey))
                    sprintCheckTimerLeft = SPRINT_KEYPRESS_INTERVAL;
                else if (newState.IsKeyUp(leftKey) && sprintCheckTimerLeft > 0)
                    sprintCheckTimerLeft2 = SPRINT_KEYPRESS_INTERVAL;
                if (newState.IsKeyDown(leftKey) && sprintCheckTimerLeft2 > 0)
                    sprintingLeft = true;
            }
        }

        private void CheckSprintRight()
        {
            if (sprintingRight)
            {
                if (newState.IsKeyUp(rightKey))
                    sprintingRight = false;
            }
            else
            {
                if (newState.IsKeyDown(rightKey) && oldState.IsKeyUp(rightKey))
                    sprintCheckTimerRight = SPRINT_KEYPRESS_INTERVAL;
                else if (newState.IsKeyUp(rightKey) && sprintCheckTimerRight > 0)
                    sprintCheckTimerRight2 = SPRINT_KEYPRESS_INTERVAL;
                if (newState.IsKeyDown(rightKey) && sprintCheckTimerRight2 > 0)
                    sprintingRight = true;
            }
            if (sprintingRight || sprintingLeft)
                speed = PLAYER_SPRINT_SPEED;
            else
                speed = PLAYER_RUN_SPEED;
        }

        private void shield()
        {
            isShielding = true;
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
                smashCooldown -= TIME_COUNTDOWN;
            if (sprintCheckTimerLeft > 0)
                sprintCheckTimerLeft -= TIME_COUNTDOWN;
            if (sprintCheckTimerLeft2 > 0)
                sprintCheckTimerLeft2 -= TIME_COUNTDOWN;
            if (sprintCheckTimerRight > 0)
                sprintCheckTimerRight -= TIME_COUNTDOWN;
            if (sprintCheckTimerRight2 > 0)
                sprintCheckTimerRight2 -= TIME_COUNTDOWN;
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
                myModelManager.endGame(DEFAULT_TINT);
        }

        public void knockback(float momentum)
        {
            lateralMomentum = momentum;
        }

        public void updateShield()
        {
            shieldOrigin = new Vector2(
                getPosition().X + 200, 
                getPosition().Y + 200);
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

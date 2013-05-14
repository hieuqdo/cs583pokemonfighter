using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    public class InteractionMediator
    {
        Player1 p1, p2;

        //Damage
        const int damageDealt = 1;
        int damageModifier = 1;
        int damageTotal;

        //Knockback
        const int knockbackDealt = 1;
        float knockbackModifier = 1f;
        float knockbackTotal;

        float attackKnockbackCap = -1;
        float knockbackDistance = 0;
        float knockbackDirection;
        int direction;

        float stunValue;

        public enum attackType { BASIC, SMASH, RANGE,
                                    BULLET, SMASHBULLET}

        public InteractionMediator(Player1 player1, Player1 player2)
        {
            p1 = player1;
            p2 = player2;
        }

        //Does melee attacks and reads ranged attack inputs
        public bool attack(Player1 attacker, attackType attackType)
        {
            if (p1.isAlive == false || p2.isAlive == false)
                return false;
            Player1 target;
            if (attacker == p1)
                target = p2;
            else
                target = p1;

            //Blocked by shielding
            if (target.isShielding && attackType != InteractionMediator.attackType.RANGE)
            {
                shieldBlock(target);
                return false;
            }

            //Dodged by rolling
            if (target.rolling && attackType != InteractionMediator.attackType.RANGE)
            {
                return false;
            }

            //Do melee attacks
            if (attacker.CollidesWith(target.model, target.GetWorld()))//If colliding
            {
                target.stunTimer = 0.2f;
                // Up attack
                if (Keyboard.GetState().IsKeyDown(attacker.upKey) || GamePad.GetState(attacker.myPlayerIndex).IsButtonDown(attacker.upButton) || GamePad.GetState(attacker.myPlayerIndex).IsButtonDown(attacker.smashUp))
                {
                    damageModifier = 3;
                    knockbackModifier = 0.04f;
                    target.currPercentage += damageModifier;
                    target.jumpMomentum = knockbackModifier * target.currPercentage;
                    target.jumping = true;
                }
                else
                {
                    switch (attackType)
                    {
                        case attackType.BASIC:
                            damageModifier = 3;
                            knockbackModifier = 40f;
                            attackKnockbackCap = 1;
                            break;

                        case attackType.SMASH:
                            damageModifier = 5;
                            knockbackModifier = 20f;
                            attackKnockbackCap = -1;
                            break;
                    }

                    //Recognize the values from the switch
                    damageTotal = damageDealt * damageModifier;
                    knockbackTotal = knockbackDealt * knockbackModifier;

                    //Calculate and set damage
                    if (target.currPercentage + damageTotal > target.maxPercentage)
                        target.currPercentage = target.maxPercentage;
                    else
                        target.currPercentage += damageTotal;

                    //Calculate and set knockback
                    //
                    knockbackDirection = (target.getPosition().X - attacker.getPosition().X);
                    //Assess whether facing left or right
                    if (knockbackDirection > 0)
                        direction = 1;
                    else direction = -1;

                    if (knockbackDirection == 0)
                        knockbackDirection = .000000001f;
                    knockbackDirection = knockbackDirection / Math.Abs(knockbackDirection);

                    knockbackDistance = target.currPercentage / 1.2f / knockbackTotal * knockbackDirection;

                    //Cap knockback
                    if (attackKnockbackCap > 0 &&
                        Math.Abs(knockbackDistance) > Math.Abs(attackKnockbackCap))
                        target.knockback(attackKnockbackCap * direction);
                    else target.knockback(knockbackDistance);
                }
                return true;
            }

            return false;
        }

        public bool rangedAttack(Player1 attacker, attackType attackType)
        {
            //Aim in the direction player is facing
            direction = attacker.flipModifier;

            //Fire shot type
            attacker.myModelManager.AddShot(attacker.getPosition(), new Vector3(direction, 0, 0), attacker, attackType);
            return true;
        }

        //processRangedAttack is only called by modelManager when targets collide
        //returns true if the bullet collided and should disappear
        public bool processRangedAttack(Bullet bullet, Player1 target, attackType attackType)
        {
            if (target.isAlive == false)
                return false;

            //Blocked by shielding
            if (target.isShielding)
            {
                shieldBlock(target);
                return true;
            }

            //Dodged by rolling
            if (target.rolling)
            {
                return false;
            }

            //Do ranged attack
            switch (attackType)
            {
                case attackType.BULLET:
                    damageModifier = 1;
                    knockbackModifier = 0;
                    attackKnockbackCap = 0;
                    stunValue = 0;
                    target.myModelManager.playSound(ModelManager.sound.SHOCKHIT);
                    break;

                case attackType.SMASHBULLET:
                    damageModifier = 10;
                    knockbackModifier = 40f;
                    attackKnockbackCap = -1;
                    stunValue = .9f;
                    target.myModelManager.playSound(ModelManager.sound.SHOCKSMASHHIT);
                    break;
            }            

            //Recognize the values from the switch
            damageTotal = damageDealt * damageModifier;
            knockbackTotal = knockbackDealt * knockbackModifier;

            //Stun target
            target.stunTimer = stunValue;

            //Calculate and set damage
            if (target.currPercentage + damageTotal > target.maxPercentage)
                target.currPercentage = target.maxPercentage;
            else
                target.currPercentage += damageTotal;

            //Calculate and set knockback
            //
            knockbackDirection = (target.getPosition().X - bullet.getPosition().X);
            //Assess whether facing left or right
            if (knockbackDirection > 0)
                direction = 1;
            else direction = -1;

            //Calculate Distance
            knockbackDirection = knockbackDirection / Math.Abs(knockbackDirection);
            knockbackDistance = target.currPercentage / knockbackTotal * knockbackDirection;          

            //Cap knockback and knockback as appropriate
            if (attackKnockbackCap > 0 &&
                Math.Abs(knockbackDistance) > Math.Abs(attackKnockbackCap))
                target.knockback(attackKnockbackCap * direction);
            else if (knockbackTotal != 0 && knockbackDirection != 0)
                target.knockback(knockbackDistance);

            return true;
        }

        // Plays sound & reduces player shield
        public void shieldBlock(Player1 blocker)
        {
            blocker.myModelManager.playSound(ModelManager.sound.SHIELD);
            blocker.shieldScale -= .1f;
        }
    }
}

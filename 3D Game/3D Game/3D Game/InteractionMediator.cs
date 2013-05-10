using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_Game
{
    class InteractionMediator
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

        public enum attackType { BASIC, SMASH, RANGE }

        public InteractionMediator(Player1 player1, Player1 player2)
        {
            p1 = player1;
            p2 = player2;
        }

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
            if (target.isShielding)
            {
                shieldBlock(target);
                return false;
            }

            //Dodged by rolling
            if (target.rolling)
            {
                return false;
            }

            if(attacker.CollidesWith(target.model, target.GetWorld()))
            {
                target.stunTimer = 0.2f;                
                
                switch (attackType)
                {
                    case attackType.BASIC:
                        damageModifier = 1;
                        knockbackModifier = 40f;
                        attackKnockbackCap = 1;

                        break;

                    case attackType.SMASH:
                        damageModifier = 5;
                        knockbackModifier = 20f;
                        attackKnockbackCap = -1;

                        attacker.smashing = false;
                        break;

                    case attackType.RANGE:
                        damageModifier = 2;
                        knockbackModifier = 50f;
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

                knockbackDistance = target.currPercentage / knockbackTotal * knockbackDirection;

                //Cap knockback
                if (attackKnockbackCap > 0 && 
                    Math.Abs(knockbackDistance) > Math.Abs(attackKnockbackCap))
                    target.knockback(attackKnockbackCap * direction);
                else target.knockback(knockbackDistance);

                target.knockback(knockbackDistance);

                return true;
            }
            return false;
        }

        public void shieldBlock(Player1 blocker)
        {
            blocker.myModelManager.playSound(ModelManager.sound.SHIELD);
        }
    }
}

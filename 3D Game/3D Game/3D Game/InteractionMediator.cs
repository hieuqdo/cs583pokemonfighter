using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_Game
{
    class InteractionMediator
    {
        Player1 p1, p2;
        const int damageModifier = 1;
        const float attackKnockbackCapR = 1;
        const float attackKnockbackCapL = -1;
        float knockbackDistance = 0;


        public InteractionMediator(Player1 player1, Player1 player2)
        {
            p1 = player1;
            p2 = player2;
        }

        public bool attack(Player1 attacker)
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

            if(attacker.CollidesWith(target.model, target.GetWorld())){
                target.stunTimer = 0.2f;
                if (target.currPercentage + damageModifier > target.maxPercentage)
                    target.currPercentage = target.maxPercentage;
                else
                    target.currPercentage += damageModifier;

                float knockbackDirection = (target.getPosition().X - attacker.getPosition().X);
                if (knockbackDirection == 0)
                    knockbackDirection = .000000001f;
                knockbackDirection = knockbackDirection / Math.Abs(knockbackDirection);
                knockbackDistance = target.currPercentage / 40f * knockbackDirection;
                if (knockbackDistance > attackKnockbackCapR)
                    target.knockback(attackKnockbackCapR);
                else if (knockbackDistance < attackKnockbackCapL)
                    target.knockback(attackKnockbackCapL);
                else target.knockback(knockbackDistance);

                return true;
            }
            return false;
        }

        public bool smashAttack(Player1 attacker)
        {
            if (p1.isAlive == false || p2.isAlive == false)
                return false;
            Player1 target;
            if (attacker == p1)
                target = p2;
            else
                target = p1;

            //Blocked by shield
            if (target.isShielding)
            {
                shieldBlock(target);
                return false;
            }
            
            //Dodged by rolling
            if(target.rolling)
            {
                return false;
            }

            if (attacker.CollidesWith(target.model, target.GetWorld()))
            {
                target.stunTimer = 0.2f;
                if (target.currPercentage + damageModifier > target.maxPercentage)
                    target.currPercentage = target.maxPercentage;
                else
                    target.currPercentage += damageModifier * 5;

                float knockbackDirection = (target.getPosition().X - attacker.getPosition().X);
                if (knockbackDirection == 0)
                    knockbackDirection = .000000001f;
                knockbackDirection = knockbackDirection / Math.Abs(knockbackDirection);
                target.knockback(target.currPercentage / 20f * knockbackDirection);
                attacker.smashing = false;

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

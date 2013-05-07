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
            if(attacker.CollidesWith(target.model, target.GetWorld())){
                target.stunTimer = 0.2f;
                if (target.currPercentage + damageModifier > target.maxPercentage)
                    target.currPercentage = target.maxPercentage;
                else
                    target.currPercentage += damageModifier;

                float knockbackDirection = (target.getPosition().X - attacker.getPosition().X);
                knockbackDirection = knockbackDirection / Math.Abs(knockbackDirection);
                target.knockback(target.currPercentage / 50f * knockbackDirection);

                return true;
            }
            return false;
        }
    }
}

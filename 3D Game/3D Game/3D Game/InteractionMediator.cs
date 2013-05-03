using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3D_Game
{
    class InteractionMediator
    {
        Player1 p1, p2;

        public InteractionMediator(Player1 player1, Player1 player2)
        {
            p1 = player1;
            p2 = player2;
        }

        public void attack(Player1 attacker)
        {
            Player1 target;
            if (attacker == p1)
                target = p2;
            else
                target = p1;
            if(attacker.CollidesWith(target.model, target.GetWorld())){
                target.stunTimer = 0.2f;
            }
        }
    }
}

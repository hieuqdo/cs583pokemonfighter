using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _3D_Game
{
    class Player2 : Player1
    {
        
        public Player2(Model m)
            : base(m)
        {
            upKey = Keys.W;
            downKey = Keys.S;
            leftKey = Keys.A;
            rightKey = Keys.D;
            shieldKey = Keys.LeftShift;
            attackKey = Keys.Q;
            DEFAULT_TINT = Color.DarkBlue;
            tint = DEFAULT_TINT;
        }

    }
}

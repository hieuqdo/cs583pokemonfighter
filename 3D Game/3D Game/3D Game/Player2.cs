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
            upKey = Keys.Up;
            downKey = Keys.Down;
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            shieldKey = Keys.RightShift;
            attackKey = Keys.OemQuestion;
            secondAttackKey = Keys.OemPeriod;
            DEFAULT_TINT = Color.Black;
            tint = DEFAULT_TINT;
            flipModifier = -1;
        }

    }
}

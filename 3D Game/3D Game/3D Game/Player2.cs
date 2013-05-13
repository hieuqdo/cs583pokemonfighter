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
    class Player2 : Player1
    {
        
        public Player2(Model m)
            : base(m)
        {
            //Player Specific
            upKey = Keys.Up;
            downKey = Keys.Down;
            leftKey = Keys.Left;
            rightKey = Keys.Right;
            shieldKey = Keys.RightShift;
            attackKey = Keys.OemQuestion;
            secondAttackKey = Keys.OemPeriod;
            DEFAULT_TINT = Color.Black;
            tint = DEFAULT_TINT;
            myPlayerIndex = PlayerIndex.Two;
            flipModifier = -1;
        }

    }
}

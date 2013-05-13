using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    class Stage1: BasicModel
    {
        public int deathPlane_TOP;
        public int deathPlane_BOT;
        public int deathPlane_LEFT;
        public int deathPlane_RIGHT;

        public Stage1(Model m)
            : base(m)
        {
            scale = .7f;

            deathPlane_TOP = 150;
            deathPlane_BOT = -100;
            deathPlane_LEFT = 250;
            deathPlane_RIGHT = -250;
        }

        public override Matrix GetWorld()
        {
            return world * Matrix.CreateTranslation(new Vector3(300, -700, -270)) * Matrix.CreateScale(.3f);
        }
    }
}

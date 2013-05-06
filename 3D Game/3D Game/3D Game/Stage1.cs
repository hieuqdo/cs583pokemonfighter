using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            deathPlane_TOP = 100;
            deathPlane_BOT = -100;
            deathPlane_LEFT = 250;
            deathPlane_RIGHT = -250;
        }

        public override Matrix GetWorld()
        {
            // Matrix.CreateTranslation(new Vector3(735, -650, -1000))
            return world * Matrix.CreateTranslation(new Vector3(0, -685, 230));
        }
    }
}

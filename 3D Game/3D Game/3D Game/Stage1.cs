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

        public Stage1(Model m)
            : base(m)
        {
            scale = .1f;
        }

        public override Matrix GetWorld()
        {
            // Matrix.CreateTranslation(new Vector3(735, -650, -1000))
            return world * Matrix.CreateTranslation(new Vector3(735, -650, -1000)) * Matrix.CreateScale(scale);
        }
    }
}

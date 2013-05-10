using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    class SpinningEnemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Vector3 direction;

        public SpinningEnemy(Model m)
            : base(m)
        {
        }

        public SpinningEnemy(Model m, Vector3 position, Vector3 Direction)
            : base(m)
        {
            world = Matrix.CreateTranslation(position);
            direction = Direction;
            tint = Color.Blue;
        }

        public override void Update()
        {
            //Move model
            translation *= Matrix.CreateTranslation(direction);
        }

        public override Matrix GetWorld()
        {
            return world * translation;
        }
    }
}

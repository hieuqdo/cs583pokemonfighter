using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        protected Color tint = Color.White;
        //public Vector3 position;

        public float scale { get; set; }

        public BasicModel(Model m)
        {
            model = m;
            scale = 1;
        }

        public virtual void Update()
        {
        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EmissiveColor = tint.ToVector3();
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }
                mesh.Draw();    
            }
        }

        public virtual Matrix GetWorld()
        {
            return Matrix.CreateScale(scale) * world;
        }

        public Vector3 getPosition()
        {
            return GetWorld().Translation;
        }

        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            //Loop through each ModelMesh in both objects and compare
            //all bounding psheres for collisions
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(
                        GetWorld()).Intersects(
                        hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }
    }
}

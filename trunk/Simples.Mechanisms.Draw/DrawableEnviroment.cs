using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Camera;

namespace Simples.Mechanisms.Draw
{
    public class DrawableEnviroment: DrawableGameComponent
    {
        private ICamera camera;
        private MechanismEnviroment enviroment;
        
        public DrawableEnviroment(Game game, MechanismEnviroment enviroment, ICamera camera)
            :base(game)
        {
            this.enviroment = enviroment;
            this.camera = camera;

            Initialize();
            
        }

        public override void Draw(GameTime gameTime)
        {

            Matrix[] transforms = new Matrix[enviroment.SceneModel.Bones.Count];
            enviroment.SceneModel.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in enviroment.SceneModel.Meshes)
            {

                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = camera.View;

                    effect.Projection = camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index];
                    mesh.Draw();
                }

            }

            enviroment.Octree.Draw(GraphicsDevice, camera.Projection, camera.View);

        }


    }
}

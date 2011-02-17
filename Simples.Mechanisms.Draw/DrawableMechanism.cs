using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Simples.Camera;
using Simples.Mechanisms;

namespace Simples.Mechanisms.Draw
{
    public class DrawableMechanism: DrawableGameComponent
    {
        private ICamera camera;
        private Mechanism mechanism;

        public DrawableMechanism(Game game, Mechanism mechanism, ICamera camera)
            : base(game)
        {
            this.camera = camera;
            this.mechanism = mechanism;
            
            Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < mechanism.Links.Count; i++)
            {
                DrawLink(mechanism.Links[i]);
                
                foreach (OrientedBoundingBox obb in mechanism.Links[i].boundingBoxList)
                {
                    obb.Transforms = mechanism.Links[i].Transform * obb.BoxTransform;
                    obb.Draw(GraphicsDevice, camera.Projection, camera.View);
                }
            }
        }

        private void DrawLink(Link link)
        {
            
            Matrix[] transforms = new Matrix[link.Model.Bones.Count];
            link.Model.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in link.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] *
                        link.Transform;
                }
                mesh.Draw();
            }
            
        }
    }
}

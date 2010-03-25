using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Robotics.Camera;

namespace Simples.Robotics.Mechanisms
{
    public class DrawableN_ArticulatedPlanar: N_ArticulatedPlanar
    {
        private ICamera _camera;

        private Model _linkModel;

        public DrawableN_ArticulatedPlanar(Vector3 nextJoint, int linkCount, Matrix world, Model linkModel, ICamera camera)
            : base(nextJoint, linkCount, world)
        {
            this._linkModel = linkModel;
            this._camera = camera;

            Matrix[] transforms = new Matrix[_linkModel.Bones.Count];
            _linkModel.CopyBoneTransformsTo(transforms);
            _transform = transforms[1];


        }

        #region IDrawable Members

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < LinkCount; i++)
            {
                DrawLink(getLinkMatrix(i));
            }
        }



        #endregion

        private void DrawLink(Matrix matrix)
        {
            Matrix[] transforms = new Matrix[_linkModel.Bones.Count];
            _linkModel.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in _linkModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = _camera.View;
                    effect.Projection = _camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] *
                        matrix;
                }
                mesh.Draw();
            }

        }
    }
}

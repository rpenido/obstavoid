using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Robotics.Camera;

namespace Simples.Robotics.Mechanisms
{
    public class DrawableNArticulatedPlanar
    {
        private NArticulatedPlanar _NArticulatedPlanar;
        public Mechanism Mechanism
        {
            get { return _NArticulatedPlanar.Mechanism; }
        }
        private ICamera _camera;

        private Model _linkModel;

        public DrawableNArticulatedPlanar(Vector3 linkTranslation, int linkCount, Matrix world, Model linkModel, ICamera camera)
        {
            this._NArticulatedPlanar = new NArticulatedPlanar(linkTranslation, linkCount, world);
            this._linkModel = linkModel;
            this._camera = camera;
        }

    

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < _NArticulatedPlanar.Mechanism.Links.Count; i++)
            {
                DrawLink(_NArticulatedPlanar.Mechanism.Links[i].Transform);
            }
        }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Simples.Robotics.Mechanisms;
using Simples.Scene.Camera;


namespace Simples.Simulation.Planar2D
{
    public class NArticulatedPlanar
    {

        private ContentManager _content;
        private ICamera _camera;

        private Model _linkModel;


        #region Property:Mechanism
        private Mechanism _mechanism;
        public Mechanism Mechanism
        {
            get { return _mechanism; }
        }
        #endregion

        private Matrix _world;
        private Vector3 _linkTranslation;

        public NArticulatedPlanar(IServiceProvider service, Vector3 linkTranslation, int linkCount, Matrix world, ICamera camera)
        {
            this._content = new ContentManager(service, "Content");
            this._linkModel = _content.Load<Model>("model1");
            this._camera = camera;
            this._mechanism = new Mechanism();
            this._world = world;
            this._linkTranslation = linkTranslation;

            RevoluteJoint nextJoint = new RevoluteJoint(world, 0, world.Up);
            _mechanism.Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                Vector3 min = new Vector3(-10, -5, -10);
                Vector3 max = new Vector3(120, 5, 10);
                OrientedBoundingBox obb = new OrientedBoundingBox(min, max);
                Link link = new Link(nextJoint, obb);
                _mechanism.Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, _linkTranslation, 0, world.Up);
                    _mechanism.Joints.Add(nextJoint);
                }                
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Mechanism.Links.Count; i++)
            {
                DrawLink(Mechanism.Links[i].Transform);
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

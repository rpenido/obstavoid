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

        private ContentManager content;
        private ICamera camera;

        private Model linkModel;


        #region Property:Mechanism
        private Mechanism mechanism;
        public Mechanism Mechanism
        {
            get { return mechanism; }
        }
        #endregion

        private Matrix world;
        private Vector3 linkTranslation;

        public NArticulatedPlanar(IServiceProvider service, Vector3 linkTranslation, int linkCount, Matrix world, ICamera camera)
        {
            this.content = new ContentManager(service, "Content");
            this.linkModel = content.Load<Model>("model1");
            this.camera = camera;
            this.mechanism = new Mechanism();
            this.world = world;
            this.linkTranslation = linkTranslation;

            RevoluteJoint nextJoint = new RevoluteJoint(world, 0, world.Up);
            mechanism.Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                //Vector3 min = new Vector3(-10, -5, -10);
                //Vector3 max = new Vector3(120, 5, 10);

                Vector3 min = new Vector3(-13, -8, -13);
                Vector3 max = new Vector3(123, 8, 13);

                OrientedBoundingBox obb = new OrientedBoundingBox(min, max);
                Link link = new Link(nextJoint, obb);
                mechanism.Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, linkTranslation, 0, world.Up);
                    mechanism.Joints.Add(nextJoint);
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
            Matrix[] transforms = new Matrix[linkModel.Bones.Count];
            linkModel.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in linkModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] *
                        matrix;
                }
                mesh.Draw();
            }

        }

    }
}

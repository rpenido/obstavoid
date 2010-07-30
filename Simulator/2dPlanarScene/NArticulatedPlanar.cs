using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Runtime.Serialization;
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
            
            this.world = world;
            this.linkTranslation = linkTranslation;

            this.mechanism = new Mechanism();
            /*
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("c:\\teste.xml", FileMode.Open);
            this.mechanism = bf.Deserialize(fs) as Mechanism;    
            */

            Link baseLink = new Link(world);
            RevoluteJoint nextJoint = new RevoluteJoint(baseLink, Vector3.Zero, 0.0f, world.Up);
            mechanism.Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                //Vector3 min = new Vector3(-10, -5, -10);
                //Vector3 max = new Vector3(120, 5, 10);

                Vector3 min = new Vector3(-13, -8, -13);
                Vector3 max = new Vector3(123, 8, 13);

                Link link = new Link(nextJoint);
                OrientedBoundingBox obb = new OrientedBoundingBox(min, max);
                link.AddBoundingBox(obb);
                mechanism.Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, linkTranslation, 0, world.Up);
                    mechanism.Joints.Add(nextJoint);
                }                
            }
            
            FileStream fs = new FileStream("c:\\3planar.rob", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fs, mechanism);
            fs.Close();
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

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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Simples.Robotics.Mechanisms;
using Simples.Scene.Camera;


namespace Simples.Simulation
{
    [Serializable]
    public class GenericEnviroment
    {
        private List<OrientedBoundingBox> obstacleList;
        private List<Matrix> _boxes;

        [NonSerialized]
        private Model sceneModel;

        private string modelName;
        public string ModelName
        {
            get { return modelName; }
            set {
                modelName = value;
                //sceneModel =                     game.Content.Load<Model>(modelName); ;
            }
        }

        public GenericEnviroment(Game game, string modelName)
        {
            

            obstacleList = new List<OrientedBoundingBox>();
            _boxes = new List<Matrix>();

            createBox(180, 0, 10);
            createBox(100, 0, 10);
            createBox(150, 0, -150);
            createBox(-100, 0, 120);
            createBox(-200, 0, -100);

            createBox(100, 0, 100);
        }

        public List<OrientedBoundingBox> BoundingBoxList
        {
            get { return obstacleList; }
        }

        private void createBox(float X, float Y, float Z)
        {
            Vector3 origin = new Vector3(X, Y, Z);

            Matrix matrix = Matrix.CreateWorld(origin, Vector3.Backward, Vector3.Up);
            _boxes.Add(matrix);

            Vector3 min = new Vector3(0, 0, -50);
            Vector3 max = new Vector3(50, 50, 0);

            OrientedBoundingBox bb = new OrientedBoundingBox(min, max);
            bb.Transforms = matrix;
            obstacleList.Add(bb);
        }
        public bool isColliding(Mechanism mechanism)
        {
            foreach (Link link in mechanism.Links)
            {
                foreach (OrientedBoundingBox sceneBb in obstacleList)
                {
                    if (link.Intersects(sceneBb))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(GameTime gameTime, ICamera camera)
        {
            foreach (Matrix b in _boxes)
            {
                drawBox(b, camera);
            }
        }

        private void drawBox(Matrix matrix, ICamera camera)
        {
            Matrix[] transforms = new Matrix[sceneModel.Bones.Count];
            sceneModel.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in sceneModel.Meshes)
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
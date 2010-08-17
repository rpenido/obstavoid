using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Scene.Camera;
using Simples.Robotics.Mechanisms;
using System.Runtime.InteropServices;

namespace Simples.Simulation.Planar2D
{

    public class SceneBoxes: DrawableGameComponent
    {
        private ICamera _camera;

        private List<OrientedBoundingBox> obstacleList;
        private List<Matrix> _boxes;
        public Model _boxModel;
        

        public SceneBoxes(Game game, ICamera camera)
            :base(game)
        {
            
            this._boxModel = game.Content.Load<Model>("cube"); ;
            this._camera = camera;

            obstacleList = new List<OrientedBoundingBox>();
            _boxes = new List<Matrix>();
            /*
            createBox(180, 0, 10);
            createBox(100, 0, 10);
            createBox(150, 0, -150);
            createBox(-100, 0, 120);
            createBox(-200, 0, -100);
            */
            createBox(300, 0, 100);
            createBox(200, 0, 10);
            createBox(250, 0, -300);
            createBox(-100, 0, 120);
            createBox(-400, 0, -100);

            createBox(100, 0, 100);

        }

        public List<OrientedBoundingBox> BoundingBoxList
        {
            get { return obstacleList; }
        }

        private void createBox(float X, float Y, float Z)
        {
            Vector3 origin = new Vector3(X, Y, Z);
            
            Matrix matrix = Matrix.CreateWorld(origin, Vector3.Forward, Vector3.Up);
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

        public override void Draw(GameTime gameTime)
        {
            foreach (Matrix b in _boxes)
            {
                drawBox(b);
            }
        }

        private void drawBox(Matrix matrix)
        {
            Matrix[] transforms = new Matrix[_boxModel.Bones.Count];
            _boxModel.CopyBoneTransformsTo(transforms);
            
            int sizeinbytes = _boxModel.Meshes[0].VertexBuffer.SizeInBytes;
            int count = sizeinbytes/Marshal.SizeOf(Vector3.Zero);
            Vector3[] vertices = new Vector3[count];
            _boxModel.Meshes[0].VertexBuffer.GetData<Vector3>(vertices);
            foreach (ModelMesh mesh in _boxModel.Meshes)
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

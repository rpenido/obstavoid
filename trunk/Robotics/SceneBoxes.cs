using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simples.Robotics.Camera;
using Simples.Robotics.Collision;
using Simples.Robotics.Mechanisms;

namespace Simples.Robotics.Scene
{
    public class SceneBoxes
    {
        private ICamera _camera;

        private List<OrientedBoundingBox> _obstacleList;
        private List<Matrix> _boxes;
        private Model _boxModel;
        

        public SceneBoxes(Model boxModel, ICamera camera)
        {
            this._boxModel = boxModel;
            this._camera = camera;

            _obstacleList = new List<OrientedBoundingBox>();
            _boxes = new List<Matrix>();

            createBox(80, 0, 0);
            createBox(120, 0, 50);
            createBox(150, 0, -80);
            createBox(-100, 0, 100);
            createBox(-40, 0, -100);

        }

        public List<OrientedBoundingBox> BoundingBoxList
        {
            get { return _obstacleList; }
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
            _obstacleList.Add(bb);
        }

        public bool isColliding(Mechanism mechanism)
        {
            foreach (Link link in mechanism.Links)
            {
                foreach (OrientedBoundingBox sceneBb in _obstacleList)
                {
                    if (link.BoundingBox.Intersects(sceneBb))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(GameTime gameTime)
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

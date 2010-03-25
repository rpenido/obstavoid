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
using Simples.Robotics.Collision;


namespace Simples.Robotics.Mechanisms
{
    public class N_ArticulatedPlanar: Mechanism
    {
        private Matrix _world;
        private float[] _jointAngle;
        protected Matrix[] _linkMatrix;

        protected Matrix _transform;
        
        private Vector3 _nextJoint;
        private int _kinematicsPendingIndex;

        private OrientedBoundingBox[] _boundingBoxList;
        public OrientedBoundingBox[] BoundingBoxList
        {
            get { return _boundingBoxList; }
        }


        public N_ArticulatedPlanar(Vector3 nextJoint, int linkCount, Matrix world)
        {
            this._world = world;
            this._nextJoint = nextJoint;

            _jointAngle = new float[linkCount];
            _linkMatrix = new Matrix[linkCount];
            _boundingBoxList = new OrientedBoundingBox[linkCount];
            for (int i = 0; i < linkCount; i++)
            {
                createBoundingBox(i);
            }

            setPending(0);
        }


        protected int LinkCount
        {
            get { return _linkMatrix.Length; }
        }

        private void setPending(int index)
        {
            if (index < _kinematicsPendingIndex)
            {
                _kinematicsPendingIndex = index;
            }
        }

        private void createBoundingBox(int index)
        {
            Vector3 min = new Vector3(-10, -5, -10);
            Vector3 max = new Vector3(120, 5, 10);
            _boundingBoxList[index] = new OrientedBoundingBox(min, max);
            _boundingBoxList[index].Transforms = _linkMatrix[index];
            
        }

        private void updateBoundingBox(int index)
        {
            _boundingBoxList[index].Transforms = _linkMatrix[index];

        }

        private void setDone(int index)
        {
            Debug.Assert(index == _kinematicsPendingIndex);

            updateBoundingBox(index);

            _kinematicsPendingIndex = index+1;
        }

        private void calcKinematics(int index)
        {
            while (index > _kinematicsPendingIndex)
            {
                calcKinematics(_kinematicsPendingIndex);
            }

            if (index == _kinematicsPendingIndex)
            {
                Matrix parentMatrix;

                if (index == 0)
                {
                    parentMatrix = _world;
                }
                else
                {
                    parentMatrix = Matrix.CreateTranslation(_nextJoint) * _linkMatrix[index - 1];
                }
                _linkMatrix[index] = Matrix.CreateRotationY(MathHelper.ToRadians(_jointAngle[index]))
                    * parentMatrix;

                setDone(index);
            }

        }

        public void setJointAngle(int index, float angle)
        {
            _jointAngle[index] = angle;
            setPending(index);
        }

        public void stepJointAngle(int index, float stepAngle)
        {
            _jointAngle[index] += stepAngle;
            setPending(index);
        }

        public Matrix getLinkMatrix(int index)
        {
            calcKinematics(index);
            return _linkMatrix[index];
        }
    }
}

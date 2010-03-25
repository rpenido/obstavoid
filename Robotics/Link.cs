using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Simples.Robotics.Collision;

namespace Simples.Robotics.Mechanisms
{
    public class Link
    {
        private OrientedBoundingBox _boundingBox;
        public OrientedBoundingBox BoundingBox
        {
            get
            {
                _boundingBox.Transforms = Transform;
                return _boundingBox;
            }
        }

        protected Joint _joint;

        public Matrix Transform
        {
            get { return _joint.Transform; }
        }

        public Link(Joint joint, OrientedBoundingBox boudingBox)
        {
            this._joint = joint;
            this._boundingBox = boudingBox;
        }
    }
}

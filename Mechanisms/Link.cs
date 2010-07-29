using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class Link
    {
        #region Property:BoundingBox
        private OrientedBoundingBox boundingBox;

        public OrientedBoundingBox BoundingBox
        {
            get
            {
                boundingBox.Transforms = Transform;
                return boundingBox;
            }
        }
        #endregion

        protected Joint joint;

        public Matrix Transform
        {
            get { return joint.Transform; }
        }

        public Link(Joint joint, OrientedBoundingBox boudingBox)
        {
            this.joint = joint;
            this.boundingBox = boudingBox;
        }
    }
}

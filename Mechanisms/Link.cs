using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace Simples.Robotics.Mechanisms
{
    [Serializable]
    public class Link
    {
        //private
        public List<OrientedBoundingBox> boundingBoxList;

        protected Joint joint;

        private Matrix transform;
        public Matrix Transform
        {
            get
            {
                if (joint != null)
                {
                    return joint.Transform;
                }
                else
                {
                    return transform;
                }
            }
        }

        private Link()
        {
            this.boundingBoxList = new List<OrientedBoundingBox>();
        }

        public Link(Matrix transform)
        {
            this.transform = transform;
            this.boundingBoxList = new List<OrientedBoundingBox>();
        }

        public Link(Joint joint)
        {
            this.joint = joint;
            this.boundingBoxList = new List<OrientedBoundingBox>();
        }

        public void AddBoundingBox(OrientedBoundingBox boudingBox)
        {
            boundingBoxList.Add(boudingBox);
        }

        public bool Intersects(OrientedBoundingBox other)
        {
            foreach (OrientedBoundingBox obb in boundingBoxList)
            {
                obb.Transforms = Transform * obb.BoxTransform;
                if (other.Intersects(obb))
                    return true;
            }
            return false;
        }
    }
}

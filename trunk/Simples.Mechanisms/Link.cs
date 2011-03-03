using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Serialization;

namespace Simples.Mechanisms
{
    [Serializable]
    public class Link
    {        
        public List<OrientedBoundingBox> boundingBoxList;

        private Joint joint;

        [NonSerializedAttribute]
        private Model model = null;
        
        
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

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
        /*
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
        */
        public bool Intersects(TriangleData other)
        {
            foreach (OrientedBoundingBox obb in boundingBoxList)
            {
                obb.Transforms = Transform * obb.BoxTransform;
                if (obb.Intersects(other))
                    return true;
            }
            return false;
        }
    }
}

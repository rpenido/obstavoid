using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Mechanisms
{
    [Serializable]
    public class PrismaticJoint : Joint
    {
        
        protected Vector3 axis;
        public Vector3 Axis
        {
            get { return axis; }
        }
        

        public PrismaticJoint(Link parentLink, Vector3 position, float displacement, Vector3 axis, double minValue, double maxValue, double velocity)
            : base(parentLink, position, minValue, maxValue, velocity)
        {
            this.Value = displacement;
            this.axis = axis;
        }
       

        protected override Matrix getTransform()
        {
            Vector3 translation = Vector3.Multiply(axis, (float)Value);
            return Matrix.CreateTranslation(translation) * base.getTransform();
        }
    }
}

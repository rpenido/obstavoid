using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Mechanisms
{
    [Serializable]
    public class RevoluteJoint: Joint
    {
        #region Property:Axis
        protected Vector3 axis;
        public Vector3 Axis
        {
            get { return axis; }
        }
        #endregion
        /*
        #region Property:Value (Override)
        protected override void setValue(double value)
        {
            double rem = value % 360;
            if (rem >= 0)
            {
                base.setValue(value);
            }
            else
            {
                base.setValue(360 + value);
            }
            
        }      
        #endregion
        */

        public double RadiansValue
        {
            get { return Math.PI * Value / 180.0; }
        }

        public RevoluteJoint(Link parentLink, Vector3 position, float angle, Vector3 axis, double minValue, double maxValue, double velocity)
            : base(parentLink, position, minValue, maxValue, velocity)
        {
            this.Value = angle;
            this.axis = axis;
        }
        /*
        public RevoluteJoint(Matrix world, float angle, Vector3 axis)
            : base(world)
        {
            this.Value = angle;
            this.axis = axis;
        }
        */
        protected override Matrix getTransform()
        {
            
            return Matrix.CreateFromAxisAngle(axis, Convert.ToSingle(RadiansValue)) * base.getTransform();
        }
    }
}
